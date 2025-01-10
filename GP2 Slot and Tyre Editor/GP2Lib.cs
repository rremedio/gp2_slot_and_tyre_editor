using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.DirectoryServices.ActiveDirectory;

namespace GP2_Slot_and_Tyre_Editor
{
    public class GP2Lib
    {
        [DllImport("./lib/gp2lib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int GP2_Decrunch(IntPtr data, IntPtr output, int dataSize);

        [DllImport("./lib/gp2lib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void GP2_CalcChecksum(IntPtr data, int size, IntPtr checksumA, IntPtr checksumB);

        public byte[] file;
        public List<int> carNumbers;
        public List<string> driverNames;
        public List<int> playerNumbers;

        public GP2Lib()
        {
            file = Array.Empty<byte>();
            carNumbers = new List<int>();
            driverNames = new List<string>();
            playerNumbers = new List<int>();
        }

        public void OpenFile(string filename)
        {
            file = Array.Empty<byte>();
            if (File.Exists(filename))
            {
                file = File.ReadAllBytes(filename);
            }

            if (file.Length > 4 && file[4] > 127)
            {
                file = Decompress(file);
            }
        }

        private byte[] Decompress(byte[] inputFile)
        {
            byte[] header = new byte[32];
            Array.Copy(inputFile, 0, header, 0, 32);
            byte[] data = new byte[inputFile.Length - 32];
            Array.Copy(inputFile, 32, data, 0, data.Length);

            header[4] = (byte)(header[4] - 128);

            IntPtr dataPtr = Marshal.AllocHGlobal(data.Length);
            Marshal.Copy(data, 0, dataPtr, data.Length);

            int decompressedSize = GP2_Decrunch(dataPtr, IntPtr.Zero, data.Length);

            byte[] decompressedData = new byte[decompressedSize];
            IntPtr outputPtr = Marshal.AllocHGlobal(decompressedSize);

            GP2_Decrunch(dataPtr, outputPtr, data.Length);
            Marshal.Copy(outputPtr, decompressedData, 0, decompressedSize);

            Marshal.FreeHGlobal(dataPtr);
            Marshal.FreeHGlobal(outputPtr);

            byte[] trimmedDecompressedData = new byte[decompressedSize - 4];
            Array.Copy(decompressedData, 0, trimmedDecompressedData, 0, decompressedSize - 4);

            return CalculateChecksum(CombineArrays(header, trimmedDecompressedData));
        }

        private byte[] CalculateChecksum(byte[] data)
        {
            byte[] checksumA = new byte[2];
            byte[] checksumB = new byte[2];

            IntPtr dataPtr = Marshal.AllocHGlobal(data.Length);
            IntPtr checksumAPtr = Marshal.AllocHGlobal(2);
            IntPtr checksumBPtr = Marshal.AllocHGlobal(2);

            Marshal.Copy(data, 0, dataPtr, data.Length);

            GP2_CalcChecksum(dataPtr, data.Length, checksumAPtr, checksumBPtr);

            Marshal.Copy(checksumAPtr, checksumA, 0, 2);
            Marshal.Copy(checksumBPtr, checksumB, 0, 2);

            Marshal.FreeHGlobal(dataPtr);
            Marshal.FreeHGlobal(checksumAPtr);
            Marshal.FreeHGlobal(checksumBPtr);

            return CombineArrays(data, checksumA, checksumB);
        }

        private static byte[] CombineArrays(byte[] first, byte[] second, byte[] third = null)
        {
            int totalLength = first.Length + second.Length + (third?.Length ?? 0);
            byte[] result = new byte[totalLength];

            Array.Copy(first, 0, result, 0, first.Length);
            Array.Copy(second, 0, result, first.Length, second.Length);
            if (third != null)
            {
                Array.Copy(third, 0, result, first.Length + second.Length, third.Length);
            }

            return result;
        }

        public string GetBasicInfo()
        {
            string[] sessions = { "Unlimited Practice", "Friday Practice", "Pre-Race Warm Up", "Saturday Practice", "Friday Qualifying", "Saturday Qualifying", "Race" };
            int[] sessionValues = { 0x00, 0x01, 0x02, 0x03, 0x40, 0x44, 0x80 };
            string[] types = { "Quickrace", "Unlimited Practice", "Non-Championship Race", "Championship Race" };
            int[] typeValues = { 5, 6, 7, 8 };

            int gameType = file[4];
            string gameTypeName = types[Array.IndexOf(typeValues, gameType)];
            int sessionType = file[0x0B];
            string sessionTypeName = sessions[Array.IndexOf(sessionValues, sessionType)];

            string trackName = Encoding.ASCII.GetString(file, 0x182C0, 38).Split('\0')[0];

            float frameRate = 256.0f / BitConverter.ToUInt32(file, 0x3A);

            if (gameType == 7 && sessionType == 0)
            {
                sessionTypeName = "Waiting for next session";
            }

            return $"{gameTypeName}\r\nSession: {sessionTypeName}\r\nTrack: {trackName}\r\nFrame Rate: {frameRate.ToString()}";
        }

        public int[] GetDriverNumbers()
        {
            List<int> numbers = new List<int>();
            for (int i = 7898; i < 7926; i++)
            {
                byte value = file[i];
                if (value > 50)
                {
                    value -= 128;
                    playerNumbers.Add(value);
                }
                if (value != 0) numbers.Add(value);
            }
            return numbers.ToArray();
        }

        public string[] GetDriverNames()
        {
            int[] carNumbers = GetDriverNumbers();
            driverNames.Clear();
            int carCount = GetNumberOfCars();
            for (int i = 0; i < carCount; i++)
            {
                int offset = 8038 - 24 + carNumbers[i] * 24;
                string name = Encoding.ASCII.GetString(file, offset, 24).Split('\0')[0];
                driverNames.Add(name);
            }
            return driverNames.ToArray();
        }

        public int GetNumberOfCars()
        {
            return file[0x2DD3];
        }

        public int[] GetStructOrder()
        {
            List<int> numbers = new List<int>();
            for (int i = 0x2dd7; i < 0x2dd7 + 28; i++)
            {
                byte value = file[i];
                if (value > 50) value -= 128;
                if (value != 0) numbers.Add(value);
            }
            return numbers.ToArray();
        }

        public int[][] GetAllSetups ()
        {
            int number_of_cars = GetNumberOfCars();
            int[] carNumbers = GetDriverNumbers();
            int[][] data = new int[number_of_cars][];

            for (int i = 0; i < number_of_cars; i++)
            {
                // Calculate the start and end offsets for this car's data
                int startOffset = 296 + carNumbers[i] * 48;
                int endOffset = 304 + carNumbers[i] * 48;

                // Determine the length of data for this car
                int length = endOffset - startOffset;

                // Initialize the sub-array for this car
                data[i] = new int[9];

                // Copy data from `file` into the sub-array
                int k = 0;
                for (int j = startOffset; j < endOffset; j++)
                {

                    data[i][k] = file[j];
                    k++;
                }
            }

            return data;
        }

        public int[] GetPlayerNumbers()
        {
            return playerNumbers.ToArray();
        }
    }
}
