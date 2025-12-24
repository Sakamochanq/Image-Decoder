using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
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
            catch (Exception)
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
                    catch (Exception)
                    {
                        MessageBox.Show($"Error saving file", hexeditor, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private readonly string pngSig = "89 50 4E 47 0D 0A 1A 0A";
        private int _currentIndex = -1;

        private void SelectFoundText(int index)
        {
            HexTextBox.SelectionStart = index;
            HexTextBox.SelectionLength = pngSig.Length;
            HexTextBox.ScrollToCaret();
            HexTextBox.Focus();
        }

        private void SearchpngSigButton_Click(object sender, EventArgs e)
        {
            _currentIndex = HexTextBox.Text.IndexOf(pngSig, StringComparison.Ordinal);

            if (_currentIndex >= 0)
            {
                SelectFoundText(_currentIndex);
            }
            else
            {
                MessageBox.Show("No text was found.", hexeditor, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SearchUpButton_Click(object sender, EventArgs e)
        {
            if (_currentIndex <= 0) return;

            int prevIndex = HexTextBox.Text.LastIndexOf(
                pngSig,
                _currentIndex - 1,
                StringComparison.Ordinal);

            if (prevIndex >= 0)
            {
                _currentIndex = prevIndex;
                SelectFoundText(_currentIndex);
            }
            else
            {
                MessageBox.Show("No previous matches found.", hexeditor, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void SearchDownButton_Click(object sender, EventArgs e)
        {
            if (_currentIndex < 0) return;

            int nextIndex = HexTextBox.Text.IndexOf(
                pngSig,
                _currentIndex + pngSig.Length,
                StringComparison.Ordinal);

            if (nextIndex >= 0)
            {
                _currentIndex = nextIndex;
                SelectFoundText(_currentIndex);
            }
            else
            {
                MessageBox.Show("No more matches found.", hexeditor, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
