using Image_Decoder.utils;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Image_Decoder
{
    public partial class Source : Form
    {
        public Source()
        {
            InitializeComponent();
        }

        // PNGのシグネチャ
        byte[] pngSig = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

        // PNGのIENDチャンク
        byte[] pngIEND = new byte[] { 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82 };

        // 読み込んだデータの格納先
        byte[] LoadedData;

        Binary bin = new Binary();

        private void OpenButton_Click(object sender, System.EventArgs e)
        {
            using (var ofd = new OpenFileDialog() { Filter = "Binary Files（*.bin） | *.bin; | All Files（*.) | *.*;" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    LoadedData = File.ReadAllBytes(ofd.FileName);

                    try
                    {
                        // シグネチャを検索
                        OffsetListView.Items.Add(bin.SearchSignature(LoadedData, pngSig));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void OffsetListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OffsetListView.SelectedItems == null)
            {
                return;
            }

            // OFFSETの抽出
            string SELECTED_OFFSET = OffsetListView.SelectedItems.ToString();

            string HEX_OFFSET = SELECTED_OFFSET.Split(':')[1].Trim();

            int OFFSET = Convert.ToInt32(HEX_OFFSET, 16);

            
            int IEND = bin.SearchByte(LoadedData, pngIEND, OFFSET);
            if (IEND == -1)
            {
                MessageBox.Show("NOT FOUND IT!");
                return;
            }

            // バイナリから画像の抽出
            int pngLength = (IEND - OFFSET) + pngIEND.Length;

            // 画像の切り出し
            byte[] pngData = new byte[pngLength];
            Buffer.BlockCopy(LoadedData, OFFSET, pngData, 0, pngLength);

            // PictureBoxへ表示
            using (var ms = new MemoryStream(pngData))
            {
                try
                {
                    pictureBox.Image = new Bitmap(ms);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
