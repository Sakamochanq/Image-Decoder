using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Image_Decoder.utils
{
    internal class Binary
    {
        public string SearchSignature(byte[] data, byte[] Sig)
        {
            for (int i = 0; i < data.Length - Sig.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < Sig.Length; j++)
                {
                    if (data[i + j] != Sig[j])
                    {
                        match = false;
                        break;
                    }
                }
                if (match == true)
                {
                    return $"OFFSET 0x{i:X}";
                }
            }

            return string.Empty;
        }

        public int SearchByte(byte[] data, byte[] pattern, int index)
        {
            for (int i = index; i < data.Length - pattern.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < pattern.Length; j++)
                {
                    if (data[i + j] != pattern[j])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
