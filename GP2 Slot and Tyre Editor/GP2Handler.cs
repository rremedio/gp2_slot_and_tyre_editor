using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GP2_Slot_and_Tyre_Editor
{
    public class GP2Handler
    {
        private string filePath;
        private FileStream? fileStream;
        private BinaryReader? reader;
        private BinaryWriter? writer;

        // Constructor to initialize file path and open file stream
        public GP2Handler(string filePath)
        {
            this.filePath = filePath;
            OpenFile();
        }

        // Open the file in read-write mode and prepare the reader and writer
        private void OpenFile()
        {
            // Open the file in read-write mode. If it doesn't exist, it will be created.
            fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            reader = new BinaryReader(fileStream);
            writer = new BinaryWriter(fileStream);
        }

        // Close the file and clean up resources
        public void Close()
        {
            reader?.Close();
            writer?.Close();
            fileStream?.Close();
        }

        // Read a byte value from a specified address (file offset)
        public byte ReadByte(long address)
        {
            if (fileStream == null || reader == null)
            {
                throw new InvalidOperationException("File stream or reader is not initialized.");
            }

            fileStream.Seek(address, SeekOrigin.Begin);
            return reader.ReadByte();
        }

        // Read a word (2 bytes, short) value from a specified address
        public ushort ReadWord(long address)
        {
            if (fileStream == null || reader == null)
            {
                throw new InvalidOperationException("File stream or reader is not initialized.");
            }

            fileStream.Seek(address, SeekOrigin.Begin);
            return reader.ReadUInt16();
        }

        // Read a long (4 bytes, int) value from a specified address
        public uint ReadLong(long address)
        {
            if (fileStream == null || reader == null)
            {
                throw new InvalidOperationException("File stream or reader is not initialized.");
            }

            fileStream.Seek(address, SeekOrigin.Begin);
            return reader.ReadUInt32();
        }

        // Write a byte value at the specified address
        public void WriteByte(long address, byte value)
        {
            if (fileStream == null || writer == null)
            {
                throw new InvalidOperationException("File stream or writer is not initialized.");
            }

            fileStream.Seek(address, SeekOrigin.Begin);
            writer.Write(value);
        }

        // Write a word (2 bytes, short) value at the specified address
        public void WriteWord(long address, ushort value)
        {
            if (fileStream == null || writer == null)
            {
                throw new InvalidOperationException("File stream or writer is not initialized.");
            }

            fileStream.Seek(address, SeekOrigin.Begin);
            writer.Write(value);
        }

        // Write a long (4 bytes, int) value at the specified address
        public void WriteLong(long address, uint value)
        {
            if (fileStream == null || writer == null)
            {
                throw new InvalidOperationException("File stream or writer is not initialized.");
            }

            fileStream.Seek(address, SeekOrigin.Begin);
            writer.Write(value);
        }
    }
}
