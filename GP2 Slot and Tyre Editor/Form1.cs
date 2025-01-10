using Microsoft.VisualBasic;
using System;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using TextBox = System.Windows.Forms.TextBox;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using GroupBox = System.Windows.Forms.GroupBox;
using System.Net;
using System.Text.RegularExpressions;
using Button = System.Windows.Forms.Button;
using System.Runtime.CompilerServices;
using static System.Windows.Forms.LinkLabel;

namespace GP2_Slot_and_Tyre_Editor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadIniFile();
            slotCombo.SelectedIndex = 0;
        }

        private void gp2LocationButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // Configure the file dialog
                openFileDialog.Filter = "GP2.exe|GP2.exe";
                openFileDialog.Title = "Select a file to open";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string gp2_location = openFileDialog.FileName;
                    SaveLocation(gp2_location);
                    gp2LocationLabel.Text = gp2_location;
                }
            }
        }

        private void LoadIniFile()
        {
            string iniFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");

            try
            {
                // Check if the file exists
                if (!File.Exists(iniFilePath))
                {
                    // Create the file if it doesn't exist
                    string file_data = "GP2Location=\n";
                    File.WriteAllText(iniFilePath, file_data);
                    return;
                }

                // Read the ini file line by line
                string[] lines = File.ReadAllLines(iniFilePath);

                // Search for a line starting with "GP2Location="
                foreach (string line in lines)
                {
                    if (line.StartsWith("GP2Location="))
                    {
                        string location = line.Substring("GP2Location=".Length);
                        if (location.Length > 0)
                        {
                            gp2LocationLabel.Text = $"{location}";
                        }
                        return;
                    }
                }

                // If no matching line is found
                gp2LocationLabel.Text = "GP2Location not found in the INI file.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveLocation(string location)
        {
            string iniFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");

            try
            {
                if (File.Exists(iniFilePath))
                {
                    bool locationUpdated = false;
                    string[] lines = File.ReadAllLines(iniFilePath);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        Debug.WriteLine(lines[i]);
                        if (lines[i].StartsWith("GP2Location="))
                        {
                            Console.WriteLine(i);
                            lines[i] = $"GP2Location={location}"; // Update the line
                            locationUpdated = true;
                            break;
                        }
                    }
                    if (locationUpdated)
                    {
                        File.WriteAllLines(iniFilePath, lines);
                        MessageBox.Show("file saved!");
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GP2Handler gp2Handler = new GP2Handler(gp2LocationLabel.Text);
            int slot = slotCombo.SelectedIndex;
            TabPage tabPage = tabsPage.TabPages["magicDataPage"];
            for (int i = 1; i < 25; i++)
            {
                int address_item = (slot * 24) + i - 1;
                long address = GP2Addresses.SlotAddresses[address_item];
                int value = gp2Handler.ReadWord(address);
                string textBoxName = "textBox" + i;
                TextBox textBox = (TextBox)tabPage.Controls[textBoxName];
                if (textBox != null)
                {
                    textBox.Text = $"{value}";
                }
            }
            gp2Handler.Close();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            TabPage tabPage = tabsPage.TabPages["magicDataPage"];
            bool isNum;
            bool allNum = true;
            for (int i = 1; i <= 24; i++)
            {
                string textBoxName = "textBox" + i;
                TextBox textBox = (TextBox)tabPage.Controls[textBoxName];
                if (textBox != null)
                {
                    isNum = int.TryParse(textBox.Text, out int result);
                    if (isNum)
                    {
                        ushort value = ushort.Parse(textBox.Text);
                        if (value > 65535) { allNum = false; }
                    }
                    else
                    { allNum = false; }
                }
            }
            if (!allNum)
            {
                MessageBox.Show("All fields are required! Maximum value is 65535");
                return;
            }
            GP2Handler gp2Handler = new GP2Handler(gp2LocationLabel.Text);
            int slot = slotCombo.SelectedIndex;
            for (int i = 1; i < 25; i++)
            {
                int address_item = (slot * 24) + i - 1;
                long address = GP2Addresses.SlotAddresses[address_item];
                string textBoxName = "textBox" + i;
                TextBox textBox = (TextBox)tabPage.Controls[textBoxName];
                if (textBox != null)
                {
                    bool isNumber = int.TryParse(textBox.Text, out int result);
                    if (isNumber)
                    {
                        ushort value = ushort.Parse(textBox.Text);
                        gp2Handler.WriteWord(address, value);
                    }
                }
            }
            gp2Handler.Close();
            MessageBox.Show("Magic Data Exported!");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                TabPage tabPage = tabsPage.TabPages["magicDataPage"];
                // Configure the file dialog
                openFileDialog.Filter = ".m2d files|*.m2d";
                openFileDialog.Title = "Select a file to open";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string md_file = openFileDialog.FileName;
                    string[] lines = File.ReadAllLines(md_file);
                    for (int i = 1; i < 25; i++)
                    {
                        bool isNumber = int.TryParse(lines[i - 1], out int result);
                        if (isNumber)
                        {
                            string textBoxName = "textBox" + i;
                            TextBox textBox = (TextBox)tabPage.Controls[textBoxName];
                            if (textBox != null)
                            {
                                textBox.Text = lines[i - 1];
                            }
                        }
                    }
                    Label label3 = (Label)tabPage.Controls["label3"];
                    label3.Text = md_file;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            TabPage tabPage = tabsPage.TabPages["magicDataPage"];
            bool isNum;
            bool allNum = true;
            for (int i = 1; i <= 24; i++)
            {
                string textBoxName = "textBox" + i;
                TextBox textBox = (TextBox)tabPage.Controls[textBoxName];
                if (textBox != null)
                {
                    isNum = int.TryParse(textBox.Text, out int result);
                    if (isNum)
                    {
                        ushort value = ushort.Parse(textBox.Text);
                        if (value > 65535) { allNum = false; }
                    }
                    else
                    { allNum = false; }
                }
            }
            if (!allNum)
            {
                MessageBox.Show("All fields are required! Maximum value is 65535");
                return;
            }
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                int slot = slotCombo.SelectedIndex + 1;
                saveFileDialog.FileName = $"md-slot-{slot}.m2d";
                saveFileDialog.Filter = ".m2d files|*.m2d";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;

                    try
                    {
                        Debug.WriteLine("trying...");
                        using (StreamWriter writer = new StreamWriter(filePath))
                        {
                            for (int i = 1; i <= 24; i++)
                            {
                                string textBoxName = "textBox" + i;
                                TextBox textBox = (TextBox)tabPage.Controls[textBoxName];

                                if (textBox != null)
                                {
                                    writer.WriteLine(textBox.Text);
                                }
                            }
                        }

                        // Notify the user that the file has been saved
                        MessageBox.Show("Magic Data saved to file " + filePath);
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions that might occur while saving the file
                        MessageBox.Show("An error occurred while saving the file: " + ex.Message);
                    }
                }
            }
        }

        private void physicsImportButton_Click(object sender, EventArgs e)
        {
            GP2Handler gp2Handler = new GP2Handler(gp2LocationLabel.Text);
            TabPage tabPage = tabsPage.TabPages["physicsPage"];
            for (int i = 1; i < 53; i++)
            {
                long address = GP2Addresses.PhysicsAddresses[i - 1];
                int value = gp2Handler.ReadWord(address);
                string textBoxName = "ptextBox" + i;
                GroupBox groupBox = (GroupBox)tabPage.Controls["groupBox1"];
                TextBox textBox = (TextBox)groupBox.Controls[textBoxName];
                if (textBox == null)
                {
                    groupBox = (GroupBox)tabPage.Controls["groupBox2"];
                    textBox = (TextBox)groupBox.Controls[textBoxName];
                }
                if (textBox == null)
                {
                    groupBox = (GroupBox)tabPage.Controls["groupBox1"];
                    GroupBox groupBox3 = (GroupBox)groupBox.Controls["groupBox3"];
                    textBox = (TextBox)groupBox3.Controls[textBoxName];
                }
                if (textBox != null)
                {
                    if (i > 11 && i < 48)
                    {
                        value -= 62082;
                    }
                    textBox.Text = $"{value}";
                }
            }
            gp2Handler.Close();
        }

        private void physicsExportButton_Click(object sender, EventArgs e)
        {
            TabPage tabPage = tabsPage.TabPages["physicsPage"];
            GroupBox groupBox = (GroupBox)tabPage.Controls["groupBox1"];
            bool allNum = true;
            for (int i = 1; i <= 52; i++)
            {
                string textBoxName = "ptextBox" + i;
                groupBox = (GroupBox)tabPage.Controls["groupBox1"];
                TextBox textBox = (TextBox)groupBox.Controls[textBoxName];
                if (textBox == null)
                {
                    groupBox = (GroupBox)tabPage.Controls["groupBox2"];
                    textBox = (TextBox)groupBox.Controls[textBoxName];
                }
                if (textBox == null)
                {
                    groupBox = (GroupBox)tabPage.Controls["groupBox1"];
                    GroupBox groupBox3 = (GroupBox)groupBox.Controls["groupBox3"];
                    textBox = (TextBox)groupBox3.Controls[textBoxName];
                }
                if (textBox != null)
                {
                    allNum = int.TryParse(textBox.Text, out int result);
                }
            }
            if (!allNum)
            {
                MessageBox.Show("All fields are required!");
                return;
            }
            GP2Handler gp2Handler = new GP2Handler(gp2LocationLabel.Text);
            for (int i = 1; i < 53; i++)
            {
                long address = GP2Addresses.PhysicsAddresses[i - 1];
                string textBoxName = "ptextBox" + i;
                groupBox = (GroupBox)tabPage.Controls["groupBox1"];
                TextBox textBox = (TextBox)groupBox.Controls[textBoxName];
                if (textBox == null)
                {
                    groupBox = (GroupBox)tabPage.Controls["groupBox2"];
                    textBox = (TextBox)groupBox.Controls[textBoxName];
                }
                if (textBox == null)
                {
                    groupBox = (GroupBox)tabPage.Controls["groupBox1"];
                    GroupBox groupBox3 = (GroupBox)groupBox.Controls["groupBox3"];
                    textBox = (TextBox)groupBox3.Controls[textBoxName];
                }
                if (textBox != null)
                {
                    bool isNumber = int.TryParse(textBox.Text, out int result);
                    if (isNumber)
                    {
                        ushort value = ushort.Parse(textBox.Text);
                        if (i > 11 && i < 48)
                        {
                            value += 62082;
                        }
                        gp2Handler.WriteWord(address, value);
                    }
                }
            }
            gp2Handler.Close();
            MessageBox.Show("Physics Data Exported!");
        }

        private void physicsLoadButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                TabPage tabPage = tabsPage.TabPages["physicsPage"];
                // Configure the file dialog
                openFileDialog.Filter = ".g2p files|*.g2p";
                openFileDialog.Title = "Select a file to open";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string physics_file = openFileDialog.FileName;
                    string[] lines = File.ReadAllLines(physics_file);
                    GroupBox groupBox = (GroupBox)tabPage.Controls["groupBox1"];
                    for (int i = 1; i < 53; i++)
                    {
                        bool isNumber = int.TryParse(lines[i - 1], out int result);
                        if (isNumber)
                        {
                            string textBoxName = "ptextBox" + i;
                            groupBox = (GroupBox)tabPage.Controls["groupBox1"];
                            TextBox textBox = (TextBox)groupBox.Controls[textBoxName];
                            if (textBox == null)
                            {
                                groupBox = (GroupBox)tabPage.Controls["groupBox2"];
                                textBox = (TextBox)groupBox.Controls[textBoxName];
                            }
                            if (textBox == null)
                            {
                                groupBox = (GroupBox)tabPage.Controls["groupBox1"];
                                GroupBox groupBox3 = (GroupBox)groupBox.Controls["groupBox3"];
                                textBox = (TextBox)groupBox3.Controls[textBoxName];
                            }
                            if (textBox != null)
                            {
                                textBox.Text = lines[i - 1];
                            }
                        }
                    }
                    groupBox = (GroupBox)tabPage.Controls["groupBox1"];
                    Label physicsLabel = (Label)groupBox.Controls["physicsFileLabel"];
                    physicsLabel.Text = physics_file;
                }
            }
        }

        private void physicsSaveButton_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.FileName = $"physics.g2p";
                saveFileDialog.Filter = ".g2p files|*.g2p";
                TabPage tabPage = tabsPage.TabPages["physicsPage"];
                GroupBox groupBox = (GroupBox)tabPage.Controls["groupBox1"];
                bool allNum = true;
                for (int i = 1; i <= 52; i++)
                {
                    string textBoxName = "ptextBox" + i;
                    groupBox = (GroupBox)tabPage.Controls["groupBox1"];
                    TextBox textBox = (TextBox)groupBox.Controls[textBoxName];
                    if (textBox == null)
                    {
                        groupBox = (GroupBox)tabPage.Controls["groupBox2"];
                        textBox = (TextBox)groupBox.Controls[textBoxName];
                    }
                    if (textBox == null)
                    {
                        groupBox = (GroupBox)tabPage.Controls["groupBox1"];
                        GroupBox groupBox3 = (GroupBox)groupBox.Controls["groupBox3"];
                        textBox = (TextBox)groupBox3.Controls[textBoxName];
                    }
                    if (textBox != null)
                    {
                        allNum = int.TryParse(textBox.Text, out int result);
                    }
                }
                if (!allNum)
                {
                    MessageBox.Show("All fields are required!");
                    return;
                }

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;

                    try
                    {
                        using (StreamWriter writer = new StreamWriter(filePath))
                        {
                            for (int i = 1; i <= 52; i++)
                            {
                                string textBoxName = "ptextBox" + i;
                                groupBox = (GroupBox)tabPage.Controls["groupBox1"];
                                TextBox textBox = (TextBox)groupBox.Controls[textBoxName];
                                if (textBox == null)
                                {
                                    groupBox = (GroupBox)tabPage.Controls["groupBox2"];
                                    textBox = (TextBox)groupBox.Controls[textBoxName];
                                }
                                if (textBox == null)
                                {
                                    groupBox = (GroupBox)tabPage.Controls["groupBox1"];
                                    GroupBox groupBox3 = (GroupBox)groupBox.Controls["groupBox3"];
                                    textBox = (TextBox)groupBox3.Controls[textBoxName];
                                }
                                if (textBox != null)
                                {
                                    writer.WriteLine(textBox.Text);
                                }
                            }
                            groupBox = (GroupBox)tabPage.Controls["groupBox1"];
                            Label physicsLabel = (Label)groupBox.Controls["physicsFileLabel"];
                            physicsLabel.Text = filePath;
                        }
                        MessageBox.Show("Physics saved to file " + filePath);
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions that might occur while saving the file
                        MessageBox.Show("An error occurred while saving the file: " + ex.Message);
                    }
                }
            }
        }

        private void miscImportButton_Click(object sender, EventArgs e)
        {
            GP2Handler gp2Handler = new GP2Handler(gp2LocationLabel.Text);
            TabPage tabPage = tabsPage.TabPages["miscPage"];
            long address = GP2Addresses.HumanGripLevelAddress;
            int value = gp2Handler.ReadWord(address);
            TextBox textBox = (TextBox)tabPage.Controls["miscBox11"];
            textBox.Text = $"{value}";
            address = GP2Addresses.SimultaneousCCAddress;
            value = gp2Handler.ReadByte(address);
            textBox = (TextBox)tabPage.Controls["miscBox12"];
            textBox.Text = $"{value}";

            for (int i = 1; i < 9; i++)
            {
                address = GP2Addresses.TireAddresses[i - 1];
                value = gp2Handler.ReadWord(address);
                string textBoxName = "miscBox" + i;
                textBox = (TextBox)tabPage.Controls[textBoxName];
                if (textBox != null)
                {
                    textBox.Text = $"{value}";
                }
                if (i == 8)
                {
                    address = GP2Addresses.TireAddresses[8];
                    miscBox18.Text = $"{gp2Handler.ReadWord(address)}";
                }
            }
            for (int i = 9; i < 11; i++)
            {
                address = GP2Addresses.CCGripLevelAddresses[i - 9];
                value = gp2Handler.ReadWord(address);
                string textBoxName = "miscBox" + i;
                textBox = (TextBox)tabPage.Controls[textBoxName];
                if (textBox != null)
                {
                    textBox.Text = $"{value}";
                }
            }
            for (int i = 15; i < 17; i++)
            {
                address = GP2Addresses.RefuelTimeAddresses[i - 15];
                value = gp2Handler.ReadWord(address);
                string textBoxName = "miscBox" + i;
                textBox = (TextBox)tabPage.Controls[textBoxName];
                if (textBox != null)
                {
                    textBox.Text = $"{value}";
                }
            }

            gp2Handler.Close();
        }

        private void miscExportButton_Click(object sender, EventArgs e)
        {
            TabPage tabPage = tabsPage.TabPages["miscPage"];
            bool isNum;
            bool allNum = true;
            for (int i = 1; i <= 16; i++)
            {
                if (i < 13 || i > 14)
                {
                    string textBoxName = "miscBox" + i;
                    TextBox textBox = (TextBox)tabPage.Controls[textBoxName];
                    if (textBox != null)
                    {
                        isNum = int.TryParse(textBox.Text, out int result);
                        if (isNum)
                        {
                            ushort value = ushort.Parse(textBox.Text);
                            if (value > 32767) { allNum = false; }
                            if (i == 12 && value > 26) { allNum = false; }
                        }
                        else
                        { allNum = false; }
                    }
                }
            }
            if (!allNum)
            {
                MessageBox.Show("All fields are required! Maximum value is 32767. Maximum value for Simultaneous cars limit is 26");
                return;
            }
            GP2Handler gp2Handler = new GP2Handler(gp2LocationLabel.Text);
            var addresses = new long[16];
            GP2Addresses.TireAddresses.CopyTo(addresses, 0);
            GP2Addresses.CCGripLevelAddresses.CopyTo(addresses, GP2Addresses.TireAddresses.Length);
            addresses[10] = GP2Addresses.HumanGripLevelAddress;
            addresses[11] = GP2Addresses.SimultaneousCCAddress;
            addresses[12] = 0; addresses[13] = 0;
            GP2Addresses.RefuelTimeAddresses.CopyTo(addresses, 14);
            exportTireData(gp2Handler);
            for (int i = 9; i < 17; i++)
            {
                if (i < 13 || i > 14)
                {
                    long address = addresses[i - 1];
                    string textBoxName = "miscBox" + i;
                    TextBox textBox = (TextBox)tabPage.Controls[textBoxName];
                    if (textBox != null)
                    {
                        bool isNumber = int.TryParse(textBox.Text, out int result);
                        if (isNumber)
                        {
                            if (i == 12)
                            {
                                byte value = byte.Parse(textBox.Text);
                                gp2Handler.WriteByte(address, value);
                            }
                            else
                            {
                                ushort value = ushort.Parse(textBox.Text);
                                gp2Handler.WriteWord(address, value);
                            }
                        }
                    }
                }
            }
            gp2Handler.Close();
            MessageBox.Show("Misc Data Exported!");
        }

        private void exportTireData(GP2Handler gp2Handler)
        {
            TabPage tabPage = tabsPage.TabPages["miscPage"];
            for (int i = 1; i < 9; i++)
            {
                long address = GP2Addresses.TireAddresses[i - 1];
                string textBoxName = "miscBox" + i;
                TextBox textBox = (TextBox)tabPage.Controls[textBoxName];
                if (textBox != null)
                {
                    bool isNumber = int.TryParse(textBox.Text, out int result);
                    if (isNumber)
                    {
                        ushort value = ushort.Parse(textBox.Text);
                        gp2Handler.WriteWord(address, value);
                    }
                }
            }
        }

        private void exportCCGripData(GP2Handler gp2Handler)
        {
            TabPage tabPage = tabsPage.TabPages["miscPage"];
            for (int i = 9; i < 11; i++)
            {
                long address = GP2Addresses.CCGripLevelAddresses[i - 9];
                string textBoxName = "miscBox" + i;
                TextBox textBox = (TextBox)tabPage.Controls[textBoxName];
                if (textBox != null)
                {
                    bool isNumber = int.TryParse(textBox.Text, out int result);
                    if (isNumber)
                    {
                        ushort value = ushort.Parse(textBox.Text);
                        gp2Handler.WriteWord(address, value);
                    }
                }
            }
        }

        private void exportHumanGrip(GP2Handler gp2Handler)
        {
            TabPage tabPage = tabsPage.TabPages["miscPage"];
            TextBox textBox = (TextBox)tabPage.Controls["miscBox11"];
            if (textBox != null)
            {
                bool isNumber = int.TryParse(textBox.Text, out int result);
                if (isNumber)
                {
                    ushort value = ushort.Parse(textBox.Text);
                    gp2Handler.WriteWord(GP2Addresses.HumanGripLevelAddress, value);
                }
            }
        }

        private void exportSimultaneousCCs(GP2Handler gp2Handler)
        {
            TabPage tabPage = tabsPage.TabPages["miscPage"];
            TextBox textBox = (TextBox)tabPage.Controls["miscBox12"];
            if (textBox != null)
            {
                bool isNumber = int.TryParse(textBox.Text, out int result);
                if (isNumber)
                {
                    byte value = byte.Parse(textBox.Text);
                    gp2Handler.WriteByte(GP2Addresses.SimultaneousCCAddress, value);
                }
            }
        }

        private void exportRefuelFactors(GP2Handler gp2Handler)
        {
            TabPage tabPage = tabsPage.TabPages["miscPage"];
            for (int i = 15; i < 17; i++)
            {
                long address = GP2Addresses.RefuelTimeAddresses[i - 15];
                string textBoxName = "miscBox" + i;
                TextBox textBox = (TextBox)tabPage.Controls[textBoxName];
                if (textBox != null)
                {
                    bool isNumber = int.TryParse(textBox.Text, out int result);
                    if (isNumber)
                    {
                        ushort value = ushort.Parse(textBox.Text);
                        gp2Handler.WriteWord(address, value);
                    }
                }
            }
        }

        private void exportTrackSizeLimit(GP2Handler gp2Handler, string limit)
        {
            bool isNumber = int.TryParse(limit, out int result);
            if (isNumber)
            {
                ushort value = ushort.Parse(limit);
                gp2Handler.WriteWord(GP2Addresses.TrackSizeAddress, value);
            }
        }

        private void patchRefuel(GP2Handler gp2Handler, bool refuel_off)
        {
            ushort patch_value_1;
            ushort patch_value_2;
            if (refuel_off)
            {
                patch_value_1 = 37008;
                patch_value_2 = 13291;
            }
            else
            {
                patch_value_1 = 3189;
                patch_value_2 = 35686;
            }
            gp2Handler.WriteWord(GP2Addresses.RefuelPatchAddresses[0], patch_value_1);
            gp2Handler.WriteWord(GP2Addresses.RefuelPatchAddresses[1], patch_value_2);
        }

        private void tireExportButton_Click(object sender, EventArgs e)
        {
            TabPage tabPage = tabsPage.TabPages["miscPage"];
            bool isNum;
            bool allNum = true;
            for (int i = 1; i < 9; i++)
            {
                string textBoxName = "miscBox" + i;
                TextBox textBox = (TextBox)tabPage.Controls[textBoxName];
                if (textBox != null)
                {
                    isNum = int.TryParse(textBox.Text, out int result);
                    if (isNum)
                    {
                        ushort value = ushort.Parse(textBox.Text);
                        if (value > 32767) { allNum = false; }
                        if (i < 4 && value > 255) { allNum = false; }
                    }
                    else
                    { allNum = false; }
                }

            }
            if (!allNum)
            {
                MessageBox.Show("All fields are required! Maximum value for Grip is 32767. Maximum value for Wear limit is 255");
                return;
            }
            GP2Handler gp2Handler = new GP2Handler(gp2LocationLabel.Text);
            exportTireData(gp2Handler);
            gp2Handler.Close();
            MessageBox.Show("Tire Data Exported!");
        }

        private void ccGripExportButton_Click(object sender, EventArgs e)
        {
            TabPage tabPage = tabsPage.TabPages["miscPage"];
            bool isNum;
            bool allNum = true;
            for (int i = 9; i < 11; i++)
            {
                string textBoxName = "miscBox" + i;
                TextBox textBox = (TextBox)tabPage.Controls[textBoxName];
                if (textBox != null)
                {
                    isNum = int.TryParse(textBox.Text, out int result);
                    if (isNum)
                    {
                        ushort value = ushort.Parse(textBox.Text);
                        if (value > 32767) { allNum = false; }
                    }
                    else
                    { allNum = false; }
                }

            }
            if (!allNum)
            {
                MessageBox.Show("All fields are required! Maximum value for is 32767.");
                return;
            }
            GP2Handler gp2Handler = new GP2Handler(gp2LocationLabel.Text);
            exportCCGripData(gp2Handler);
            gp2Handler.Close();
            MessageBox.Show("Overall CC Grip Levels Exported!");
        }

        private void hGripExportButton_Click(object sender, EventArgs e)
        {
            TabPage tabPage = tabsPage.TabPages["miscPage"];
            TextBox textBox = (TextBox)tabPage.Controls["miscBox11"];
            bool isNum = false;
            if (textBox != null)
            {
                isNum = int.TryParse(textBox.Text, out int result);
                if (isNum)
                {
                    ushort value = ushort.Parse(textBox.Text);
                    if (value > 32767) { isNum = false; }
                }
                else
                { isNum = false; }
            }
            if (!isNum)
            {
                MessageBox.Show("Required! Maximum value is 32767.");
                return;
            }
            GP2Handler gp2Handler = new GP2Handler(gp2LocationLabel.Text);
            exportHumanGrip(gp2Handler);
            gp2Handler.Close();
            MessageBox.Show("Human Grip Level Exported!");
        }

        private void simultaneosCarsButton_Click(object sender, EventArgs e)
        {
            TabPage tabPage = tabsPage.TabPages["miscPage"];
            TextBox textBox = (TextBox)tabPage.Controls["miscBox12"];
            bool isNum = false;
            if (textBox != null)
            {
                isNum = int.TryParse(textBox.Text, out int result);
                if (isNum)
                {
                    byte value = byte.Parse(textBox.Text);
                    if (value > 26) { isNum = false; }
                }
                else
                { isNum = false; }
            }
            if (!isNum)
            {
                MessageBox.Show("Required! Maximum value is 26.");
                return;
            }
            GP2Handler gp2Handler = new GP2Handler(gp2LocationLabel.Text);
            exportSimultaneousCCs(gp2Handler);
            gp2Handler.Close();
            MessageBox.Show("Simultaneous Cars Limit Exported!");
        }

        private void refuelExportButton_Click(object sender, EventArgs e)
        {
            TabPage tabPage = tabsPage.TabPages["miscPage"];
            bool isNum;
            bool allNum = true;
            for (int i = 15; i < 17; i++)
            {
                string textBoxName = "miscBox" + i;
                TextBox textBox = (TextBox)tabPage.Controls[textBoxName];
                if (textBox != null)
                {
                    isNum = int.TryParse(textBox.Text, out int result);
                    if (isNum)
                    {
                        ushort value = ushort.Parse(textBox.Text);
                        if (value > 32767) { allNum = false; }
                    }
                    else
                    { allNum = false; }
                }

            }
            if (!allNum)
            {
                MessageBox.Show("All fields are required! Maximum value for is 32767.");
                return;
            }
            GP2Handler gp2Handler = new GP2Handler(gp2LocationLabel.Text);
            exportRefuelFactors(gp2Handler);
            gp2Handler.Close();
            MessageBox.Show("Pit Time Factors Exported!");
        }

        private void trackSizeOriginalButton_Click(object sender, EventArgs e)
        {
            GP2Handler gp2Handler = new GP2Handler(gp2LocationLabel.Text);
            string limit = "62000";
            exportTrackSizeLimit(gp2Handler, limit);
            gp2Handler.Close();
            MessageBox.Show("Track Size Limit set to 62000 bytes!");
        }

        private void trackSizeMaxButton_Click(object sender, EventArgs e)
        {
            GP2Handler gp2Handler = new GP2Handler(gp2LocationLabel.Text);
            string limit = "65535";
            exportTrackSizeLimit(gp2Handler, limit);
            gp2Handler.Close();
            MessageBox.Show("Track Size Limit set to 65535 bytes!");
        }

        private void refuelPatchButton_Click(object sender, EventArgs e)
        {
            GP2Handler gp2Handler = new GP2Handler(gp2LocationLabel.Text);
            patchRefuel(gp2Handler, true);
            gp2Handler.Close();
            MessageBox.Show("Refueling disabled!");
        }

        private void refuelUnpatchButton_Click(object sender, EventArgs e)
        {
            GP2Handler gp2Handler = new GP2Handler(gp2LocationLabel.Text);
            patchRefuel(gp2Handler, false);
            gp2Handler.Close();
            MessageBox.Show("Refueling enabled!");
        }

        private void button37_Click(object sender, EventArgs e)
        {
            string location = gp2LocationLabel.Text;
            string gp2lap_location;
            if (location.IndexOf("GP2.EXE", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                string folderPath = Path.GetDirectoryName(location)!;
                if (Directory.Exists(folderPath))
                {
                    gp2lap_location = folderPath + "\\gp2lap.cfg";
                }
                else
                {
                    MessageBox.Show("Could not find GP2 folder!");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Could not find GP2.EXE!");
                return;
            }
            if (File.Exists(gp2lap_location))
            {
                List<string> trackFiles = new List<string>();
                try
                {
                    string[] lines = File.ReadAllLines(gp2lap_location);

                    // Regex to extract content inside quotes
                    Regex regex = new Regex(@"f1ct\d+\s*=\s*""([^""]+)""");
                    TabPage tabPage = tabsPage.TabPages["trackPage"];
                    int i = 1;
                    int k = 104;
                    foreach (string line in lines)
                    {
                        // Check if the line matches the pattern for f1ct entries
                        Match match = regex.Match(line);
                        if (match.Success)
                        {
                            Label label = (Label)tabPage.Controls["label" + k];
                            if (label != null)
                            {
                                label.Text = match.Groups[1].Value;
                                string folderPath = Path.GetDirectoryName(location)!;
                                string[] data = getTrackData(folderPath + "\\" + label.Text);
                                if (data.Length > 0)
                                {
                                    Button button = (Button)tabPage.Controls["trackButton" + i];
                                    button.Text = data[2];
                                }
                                else
                                {
                                    Button button = (Button)tabPage.Controls["trackButton" + i];
                                    button.Text = label.Text.Split('\\')[1];
                                }

                                i += 1;
                                k += 1;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("An error occurred: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Could not find GP2Lap.cfg!");
            }
        }

        private bool trackLabelsValid()
        {
            bool result = true;
            Label[] labels = new Label[]
                    {
                        label104, label105, label106, label107, label108, label109, label110, label111,
                        label112, label113, label114, label115, label116, label117, label118, label119
                    };
            foreach (Label label in labels)
            {
                if (label.Text == "track file" || label.Text.Length == 0)
                {
                    result = false;
                }
            }
            return result;
        }

        private void button38_Click(object sender, EventArgs e)
        {
            if (!trackLabelsValid())
            {
                MessageBox.Show("Please import or load all tracks before exporting!");
                return;
            }
            string location = gp2LocationLabel.Text;
            string gp2lap_location;
            if (location.IndexOf("GP2.EXE", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                string folderPath = Path.GetDirectoryName(location)!;
                if (Directory.Exists(folderPath))
                {
                    gp2lap_location = folderPath + "\\gp2lap.cfg";
                }
                else
                {
                    MessageBox.Show("Could not find GP2 folder!");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Could not find GP2.EXE!");
                return;
            }
            if (File.Exists(gp2lap_location))
            {
                try
                {
                    string[] lines = File.ReadAllLines(gp2lap_location);
                    Label[] labels = new Label[]
                    {
                        label104, label105, label106, label107, label108, label109, label110, label111,
                        label112, label113, label114, label115, label116, label117, label118, label119
                    };
                    int label_index = 0;
                    for (int i = 0; i < lines.Length; i++)
                    {
                        // Check if the line starts with "f1ct" (case-sensitive)
                        if (lines[i].StartsWith("f1ct"))
                        {
                            // Update the line with the text from the corresponding label
                            lines[i] = $"{lines[i].Substring(0, lines[i].IndexOf("=") + 1)} \"{labels[label_index].Text}\"";
                            label_index++;
                        }
                    }

                    // Write the updated content back to the file
                    File.WriteAllLines(gp2lap_location, lines);

                    MessageBox.Show("File updated successfully!");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("An error occurred: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Could not find GP2Lap.cfg!");
            }
        }

        private string[] getTrackData(string trackFile)
        {
            try
            {
                // Read the entire file content as a single string
                string fileContent = File.ReadAllText(trackFile);
                if (!string.IsNullOrEmpty(fileContent))
                {
                    string firstChar = fileContent.Substring(0, 1);
                    if (firstChar.Contains("#"))
                    {
                        string[] data = fileContent.Substring(0, 500).Split('|');
                        return data;
                    }
                    else
                    {
                        return Array.Empty<string>();
                    }
                }
                return Array.Empty<string>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return Array.Empty<string>();
            }
        }

        private void displayTrackData(int slot, Label label)
        {
            string folderPath = Path.GetDirectoryName(gp2LocationLabel.Text)!;
            if (Directory.Exists(folderPath))
            {
                string filePath = folderPath + "\\" + label.Text;
                if (File.Exists(filePath))
                {
                    string[] data = getTrackData(folderPath + "\\" + label.Text);
                    if (data.Length > 0)
                    {
                        string displayData = "";
                        displayData += $"Slot {slot}\r\n";
                        displayData += $"Name: {data[2]}\r\n";
                        displayData += $"Country: {data[4]}\r\n";
                        displayData += $"by: {data[8]}\r\n";
                        displayData += $"Laps: {data[16]}\r\n";
                        displayData += $"Length: {data[22]}m\r\n";
                        displayData += $"Tire Wear: {data[20]}\r\n";
                        TabPage tabPage = tabsPage.TabPages["trackPage"];
                        TextBox displayBox = (TextBox)tabPage.Controls["displayBox"];
                        Button button = (Button)tabPage.Controls["trackButton" + slot];
                        if (displayBox != null)
                        {
                            displayBox.Text = displayData;
                            button.Text = data[2];
                        }
                    }
                    else
                    {
                        TabPage tabPage = tabsPage.TabPages["trackPage"];
                        Button button = (Button)tabPage.Controls["trackButton" + slot];
                        button.Text = label.Text.Split("\\")[1];
                        displayBox.Text = $"Slot{slot}\r\nOriginal {label.Text}";
                    }
                }
                else
                {
                    MessageBox.Show($"Can't find {filePath}");
                }
            }
            else
            {
                MessageBox.Show($"Can't find {folderPath}");
            }
        }

        private void trackButton1_Click(object sender, EventArgs e)
        {
            displayTrackData(1, label104);
        }

        private void trackButton2_Click(object sender, EventArgs e)
        {
            displayTrackData(2, label105);
        }

        private void trackButton3_Click(object sender, EventArgs e)
        {
            displayTrackData(3, label106);
        }

        private void trackButton4_Click(object sender, EventArgs e)
        {
            displayTrackData(4, label107);
        }

        private void trackButton5_Click(object sender, EventArgs e)
        {
            displayTrackData(5, label108);
        }

        private void trackButton6_Click(object sender, EventArgs e)
        {
            displayTrackData(6, label109);
        }

        private void trackButton7_Click(object sender, EventArgs e)
        {
            displayTrackData(7, label110);
        }

        private void trackButton8_Click(object sender, EventArgs e)
        {
            displayTrackData(8, label111);
        }

        private void trackButton9_Click(object sender, EventArgs e)
        {
            displayTrackData(9, label112);
        }

        private void trackButton10_Click(object sender, EventArgs e)
        {
            displayTrackData(10, label113);
        }

        private void trackButton11_Click(object sender, EventArgs e)
        {
            displayTrackData(11, label114);
        }

        private void trackButton12_Click(object sender, EventArgs e)
        {
            displayTrackData(12, label115);
        }

        private void trackButton13_Click(object sender, EventArgs e)
        {
            displayTrackData(13, label116);
        }

        private void trackButton14_Click(object sender, EventArgs e)
        {
            displayTrackData(14, label117);
        }

        private void trackButton15_Click(object sender, EventArgs e)
        {
            displayTrackData(15, label118);
        }

        private void trackButton16_Click(object sender, EventArgs e)
        {
            displayTrackData(16, label119);
        }

        private void loadTrackFile(int slot)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // Configure the file dialog
                openFileDialog.Filter = ".dat files|*.dat";
                openFileDialog.Title = "Select a file to open";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string trackLocation = openFileDialog.FileName;
                    TabPage tabPage = tabsPage.TabPages["trackPage"];
                    Label label = (Label)tabPage.Controls["label" + (slot + 103)];
                    string fileName = Path.GetFileName(trackLocation);
                    string rootPath = Path.GetDirectoryName(gp2LocationLabel.Text)!;
                    string relativePath = GetRelativePath(rootPath, trackLocation);
                    label.Text = relativePath;
                    displayTrackData(slot, label);
                }
            }
        }

        private string GetRelativePath(string rootPath, string fullPath)
        {
            // Ensure the root path ends with a directory separator
            if (!rootPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                rootPath += Path.DirectorySeparatorChar;
            }

            // Get the relative path by subtracting the root directory from the full file path
            Uri rootUri = new Uri(rootPath);
            Uri fullFileUri = new Uri(fullPath);

            // Compute the relative URI
            Uri relativeUri = rootUri.MakeRelativeUri(fullFileUri);

            // Return the relative path
            return Uri.UnescapeDataString(relativeUri.ToString()).Replace('/', Path.DirectorySeparatorChar);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            loadTrackFile(1);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            loadTrackFile(2);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            loadTrackFile(3);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            loadTrackFile(4);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            loadTrackFile(5);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            loadTrackFile(6);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            loadTrackFile(7);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            loadTrackFile(8);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            loadTrackFile(9);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            loadTrackFile(10);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            loadTrackFile(11);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            loadTrackFile(12);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            loadTrackFile(13);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            loadTrackFile(14);
        }

        private void button19_Click(object sender, EventArgs e)
        {
            loadTrackFile(15);
        }

        private void button20_Click(object sender, EventArgs e)
        {
            loadTrackFile(16);
        }

        private void label150_Click(object sender, EventArgs e)
        {

        }

        private void loadSaveButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "GP2 Saved Games|*.qr*; *.ra*";
                openFileDialog.Title = "Select a file to open";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string save_file = openFileDialog.FileName;
                    TabPage savePage = tabsPage.TabPages["savePage"];
                    Label label = (Label)savePage.Controls["loadSaveLabel"];
                    label.Text = save_file;
                    GP2Lib gp2Lib = new GP2Lib();

                    // Open and decompress the file
                    gp2Lib.OpenFile(save_file);

                    // Get the driver names
                    string[] driverNames = gp2Lib.GetDriverNames();
                    int number_of_cars = gp2Lib.GetNumberOfCars();

                    for (int i = 0; i < number_of_cars; i++)
                    {
                        Label driver_label = (Label)savePage.Controls[$"driver{i}"];
                        driver_label.Text = driverNames[i];
                    }

                    string info = gp2Lib.GetBasicInfo();
                    infoBox.Text = info;

                    setAll("fw", 0, gp2Lib);
                    setAll("rw", 1, gp2Lib);
                    setAll("g1", 2, gp2Lib);
                    setAll("g2", 3, gp2Lib);
                    setAll("g3", 4, gp2Lib);
                    setAll("g4", 5, gp2Lib);
                    setAll("g5", 6, gp2Lib);
                    setAll("g6", 7, gp2Lib);
                    setAll("tt", 8, gp2Lib);
                }
            }
        }

        void setAll(string boxName, int index, GP2Lib gp2lib)
        {
            TabPage savePage = tabsPage.TabPages["savePage"];
            int[][] setups = gp2lib.GetAllSetups();
            int number_of_cars = gp2lib.GetNumberOfCars();
            int[] driverNumbers = gp2lib.GetDriverNumbers();
            int[] playerNumbers = gp2lib.GetPlayerNumbers();
            int offset = 0;
            for (int i = 0; i < playerNumbers.Length; i++)
            {
                if (playerNumbers[i] == driverNumbers[i])
                {
                    offset++;
                }
            }
            for (int i = 0; i < number_of_cars; i++)
            {
                TextBox field_box = (TextBox)savePage.Controls[$"{boxName}{i}"];
                field_box.Text = $"{setups[i][index + 9 * offset]}";
            }
        }
    }
}