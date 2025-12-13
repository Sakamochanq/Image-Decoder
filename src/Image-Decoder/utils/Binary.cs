using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Image_Decoder.utils
{
    internal class Binary
    {
        // PNGのシグネチャ
        readonly byte[] pngSig = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        // PNGのIENDチャンク
        readonly byte[] pngIEND = { 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82 };

        public class Segment
        {
            public int StartOffset { get; set; }
            public int EndOffset { get; set; }

            public int Length => EndOffset - StartOffset;

            public string StartHex => $"0x{StartOffset:X}";
            public string EndHex => $"0x{EndOffset:X}";
        }

        public List<Segment> SearchSignature(byte[] data)
        {
            List<Segment> results = new List<Segment>();

            for (int i = 0; i <= data.Length - pngSig.Length; i++)
            {
                if (!Match(data, i, pngSig))
                    continue;

                int end = SearchIEND(data, i);
                if (end == -1)
                    continue;

                results.Add(new Segment
                {
                    StartOffset = i,
                    EndOffset = end
                });
            }

            return results;
        }


        private int SearchIEND(byte[] data, int start)
        {
            for (int i = start; i <= data.Length - pngIEND.Length; i++)
            {
                if (Match(data, i, pngIEND))
                    return i + pngIEND.Length;
            }
            return -1;
        }

        private bool Match(byte[] data, int offset, byte[] sig)
        {
            for (int i = 0; i < sig.Length; i++)
            {
                if (data[offset + i] != sig[i])
                    return false;
            }
            return true;
        }
    }
}
