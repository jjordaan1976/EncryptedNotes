using EncryptedNotes;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace TabbedEditor
{
    public partial class NotePad : Form
    {
        private int tabCounter = 1;
        private readonly List<(byte[], string)> encryptedNotes = new List<(byte[], string)>();
        private const int KeyLength = 32;
        private const int PassLength = 16;

        private TextBox saveLocationTextBox; 

        public NotePad()
        {
            InitializeUI();
        }

        private TabControl tabControl;
        private Button saveButton;
        private Button loadButton;
        private TextBox KeyText;
        private TextBox PasswordText;


        private void InitializeUI()
        {
            this.Text = "Tabbed Editor";
            this.Width = 800;
            this.Height = 600;

            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Bottom;
            tabControl.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tabControl.Top = 30;
            tabControl.Height = 570;
            tabControl.Width = 800;
            tabControl.ContextMenuStrip = new ContextMenuStrip();
            ToolStripMenuItem renameItem = new ToolStripMenuItem("Rename");
            renameItem.Click += Rename_Click;

            tabControl.ContextMenuStrip.Items.Add(renameItem);
            saveButton = new Button();
            saveButton.Text = "Save";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Top = 0;
            saveButton.Left = 0;
            saveButton.Width = 100;
            saveButton.Height = 30;
            saveButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            saveButton.Click += Save_Click;

            KeyText = new TextBox();
            KeyText.Left = 100;
            KeyText.Width = 200;
            KeyText.Top = 0;
            KeyText.Text = "Key";
            KeyText.PasswordChar = '*';

            PasswordText = new TextBox();
            PasswordText.Left = 300;
            PasswordText.Width = 200;
            PasswordText.Top = 0;
            PasswordText.Text = "Password";
            PasswordText.PasswordChar = '*';

            loadButton = new Button();
            loadButton.Text = "Load";
            loadButton.UseVisualStyleBackColor = true;
            loadButton.Top = 0;
            loadButton.Left = 500;
            loadButton.Width = 80;
            loadButton.Height = 30;
            loadButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            loadButton.Click += Load_Click;

            saveLocationTextBox = new TextBox();
            saveLocationTextBox.Left = 580; 
            saveLocationTextBox.Width = 200;
            saveLocationTextBox.Top = 0;
            saveLocationTextBox.Text = ConfigManager.LoadConfig(); 
            saveLocationTextBox.Anchor = AnchorStyles.Right | AnchorStyles.Top;

            base.Controls.Add(KeyText);
            base.Controls.Add(PasswordText);
            base.Controls.Add(saveLocationTextBox);
            base.Controls.Add(saveButton);
            base.Controls.Add(loadButton);

            this.Controls.Add(tabControl);

            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;

            AddNewTab();
            this.FormClosing += NotePad_FormClosing;
        }

        private void Load_Click(object sender, EventArgs e)
        {
            var files = Directory.GetFiles(saveLocationTextBox.Text);
            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];
                string decryptedData = DecryptionHelper.ReadAndDecryptFile(file, KeyText.Text, PasswordText.Text);
                if (decryptedData != null)
                {                    
                    var tab = tabControl.TabPages[tabControl.TabPages.Count - 1];
                    tab.Text = PathHelper.GetFileNameWithoutExtension(file);
                    var textBox = tab.Controls[0] as TextBox;
                    if (textBox != null)
                    {
                        textBox.Text = decryptedData;
                    }
                }
                if (i < files.Length - 1)
                {
                    AddNewTab();
                }
            }
        }

        private void Save_Click(object sender, System.EventArgs e)
        {
            encryptedNotes.Clear();
            EncryptData();
            SaveEncryptedData();
        }

        private void EncryptData()
        {
            using (var aes = Aes.Create())
            {

                aes.Key = Encoding.UTF8.GetBytes(KeyText.Text.Length > KeyLength ? KeyText.Text.Substring(0, KeyLength) : KeyText.Text.PadRight(KeyLength));
                aes.IV = Encoding.UTF8.GetBytes(PasswordText.Text.Length > PassLength ? PasswordText.Text.Substring(0, PassLength) : PasswordText.Text.PadRight(PassLength));

                foreach (var tab in tabControl.TabPages)
                {
                    var tabPage = tab as TabPage;
                    if (tabPage != null)
                    {
                        TextBox textBox = tabPage.Controls.Count > 0 ? tabPage.Controls[0] as TextBox : null;
                        if (textBox != null)
                        {
                            var encryptor = aes.CreateEncryptor();
                            var plainBytes = Encoding.UTF8.GetBytes(textBox.Text);
                            var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                            encryptedNotes.Add((cipherBytes, tabPage.Text));
                        }
                    }
                }
            }            
        }

        private void SaveEncryptedData()
        {
            foreach ((byte[], string) bytearray in encryptedNotes)
            {
                string saveLocation = saveLocationTextBox.Text;
                if (!saveLocation.EndsWith("\\"))
                {
                    saveLocation += "\\";
                }
                string fileName = saveLocation + bytearray.Item2 + ".txt";

                try
                {
                    Directory.CreateDirectory(saveLocation);
                    File.WriteAllBytes(fileName, bytearray.Item1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving file: {ex.Message}", "Save Error");
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.N)
            {
                AddNewTab();
                e.Handled = true;
            }
        }

        private void AddNewTab()
        {
            var tabPage = new TabPage($"Tab {tabCounter++}");

            var textBox = new TextBox
            {
                Multiline = true,
                Dock = DockStyle.Fill,
                ScrollBars = ScrollBars.Both,
                AcceptsReturn = true,
                AcceptsTab = true,
                WordWrap = false
            };

            tabPage.Controls.Add(textBox);
            tabControl.TabPages.Add(tabPage);
            tabControl.SelectedTab = tabPage;            
        }
        private void Rename_Click(object sender, EventArgs e)
        {
            TabPage selectedTab = ((TabControl)this.tabControl).SelectedTab;
            string newName = Interaction.InputBox("Enter new tab name:", "Rename Tab", selectedTab.Text);

            if (!string.IsNullOrEmpty(newName))
            {
                selectedTab.Text = newName;
            }
        }
        private void NotePad_FormClosing(object sender, FormClosingEventArgs e)
        {
            ConfigManager.SaveConfig(saveLocationTextBox.Text);
        }
    }
}
