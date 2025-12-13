using Image_Decoder.common;
using Image_Decoder.forms;
using Image_Decoder.utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using static Image_Decoder.utils.Binary;


namespace Image_Decoder
{
    public partial class Source : Form
    {
        public Source()
        {
            InitializeComponent();
        }

        // 読み込んだデータの格納先
        byte[] LoadedData;

        // 元ファイル
        string originFile;

        Binary bin = new Binary();

        private void OpenButton_Click(object sender, System.EventArgs e)
        {
            using (var ofd = new OpenFileDialog() { Filter = "Binary Files（*.bin） | *.bin; | All Files（*.) | *.*;" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    originFile = ofd.FileName;
                    LoadedData = File.ReadAllBytes(ofd.FileName);
                    List<Segment> pngs = bin.SearchSignature(LoadedData);

                    try
                    {
                        // クリア
                        OffsetsListView.Items.Clear();

                        // シグネチャを検索
                        foreach (var png in pngs)
                        {
                            ListViewItem item = new ListViewItem(png.StartHex);

                            item.SubItems.Add(png.EndHex);
                            item.SubItems.Add($"{png.Length} bytes");

                            item.Tag = png;

                            OffsetsListView.Items.Add(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        StatusLabel.Text = ex.Message;
                    }
                }
            }
        }

        private void OffsetListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OffsetsListView.SelectedItems.Count == 0)
            {
                return;
            }

            var png = (Segment)OffsetsListView.SelectedItems[0].Tag;

            byte[] imageBytes = new byte[png.Length];
            Array.Copy(LoadedData, png.StartOffset, imageBytes, 0, png.Length);

            using (var ms = new MemoryStream(imageBytes))
            {
                try
                {
                    pictureBox.Image = Image.FromStream(ms);
                }
                catch (Exception ex)
                {
                    StatusLabel.Text = ex.Message;
                }
            }
        }

        private void RunHexEditorButton_Click(object sender, EventArgs e)
        {
            using (var HexEditor = new HexEditor(originFile))
            {
                HexEditor.ShowDialog();
            }
        }

        private void SaveImageButton_Click(object sender, EventArgs e)
        {
            if (OffsetsListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select an image segment from the list.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var png = (Segment)OffsetsListView.SelectedItems[0].Tag;
            byte[] imageBytes = new byte[png.Length];
            Array.Copy(LoadedData, png.StartOffset, imageBytes, 0, png.Length);

            using (var sfd = new SaveFileDialog() { Filter = "PNG Files（*.png） | *.png; | All Files（*.) | *.*;" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllBytes(sfd.FileName, imageBytes);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving image {ex.Message}", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void RSaveImageButton_Click(object sender, EventArgs e)
        {
            SaveImageButton_Click(sender, e);
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            pictureBox.Image = null;
        }

        ImageEdit IE = new ImageEdit();

        private void GrayscaleButton_Click(object sender, EventArgs e)
        {
            pictureBox.Image = IE.GrayScale(pictureBox.Image);
        }

        private void ReverseButton_Click(object sender, EventArgs e)
        {
            pictureBox.Image = IE.Reverse(pictureBox.Image);
        }

        private void SepiatoneButton_Click(object sender, EventArgs e)
        {
            pictureBox.Image = IE.SepiaTone(pictureBox.Image);
        }

        private void histgramButton_Click(object sender, EventArgs e)
        {
            pictureBox.Image = IE.HistogramEqualization(pictureBox.Image);
        }
    }
}
