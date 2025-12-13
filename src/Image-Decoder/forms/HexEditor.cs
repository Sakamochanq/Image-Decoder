using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Image_Decoder.forms
{
    public partial class HexEditor : Form
    {
        private readonly string LoadedData;

        string hexeditor;

        public HexEditor(string Data)
        {
            InitializeComponent();

            // 読み込んだデータの格納先
            LoadedData = Data;
        }

        private void HexEditor_Load(object sender, EventArgs e)
        {
            hexeditor = this.Text;

            try
            {
                byte[] data = File.ReadAllBytes(LoadedData);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in data)
                {
                    sb.Append(b.ToString("X2")).Append(" ");
                }

                HexTextBox.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please load the object.", hexeditor, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void SaveAsButton_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog() { Filter = "Binary Files（*.bin） | *.bin; | All Files（*.) | *.*;" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllText(sfd.FileName, HexTextBox.Text);
                        MessageBox.Show("File saved successfully.", hexeditor, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving file", hexeditor, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
