using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfNetAssit.Communicate
{
    public class HexStringConvertor
    {
        /// <summary>
        /// 将数据转为 16进制显示
        /// </summary>
        /// <param name="buf"></param>
        /// <returns></returns>
        static public string HexToString(byte[] buf)
        {
            StringBuilder sb = new StringBuilder(buf.Length * 3 + 3);
            for (int i = 0; i < buf.Length; i++)
            {
                sb.Append(buf[i].ToString("X2"));
                sb.Append(" ");
            }
            return sb.ToString();
        }

        private static byte ToHexValue(char v)
        {
            switch (v)
            {
                case '0':
                    return 0;
                case '1':
                    return 1;
                case '2':
                    return 2;
                case '3':
                    return 3;
                case '4':
                    return 4;
                case '5':
                    return 5;
                case '6':
                    return 6;
                case '7':
                    return 7;
                case '8':
                    return 8;
                case '9':
                    return 9;
                case 'a':
                case 'A':
                    return 10;
                case 'b':
                case 'B':
                    return 11;
                case 'c':
                case 'C':
                    return 12;
                case 'd':
                case 'D':
                    return 13;
                case 'e':
                case 'E':
                    return 14;
                case 'f':
                case 'F':
                    return 15;
                default:
                    {
                        throw new Exception("字符不是16进制字符");
                    }
            }
        }

        /// <summary>
        /// 将16进制文本转成字节数组
        /// </summary>
        /// <param name="text">16进制文本</param>
        /// <returns>字节数组</returns>
        internal static byte[] StringToHex(string text)
        {
            string newString = "";
            StringBuilder sb = new StringBuilder();

            bool bPair = false;
            char pc = '0';
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (Uri.IsHexDigit(c))
                {
                    if (bPair)
                    {
                        sb.Append(pc);
                        sb.Append(c);
                        bPair = false;
                    }
                    else
                    {
                        pc = c;
                        bPair = true;
                    }
                }
                else
                    bPair = false;
            }
            newString = sb.ToString();

            int byteLength = newString.Length / 2;
            byte[] bytes = new byte[byteLength];
            int j = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)((ToHexValue(newString[j]) << 4) + ToHexValue(newString[j + 1]));
                j += 2;
            }
            return bytes;
        }

        public static bool IsHex(char c)
        {
            return (c >= '0' && c <= '9') ||
                     (c >= 'a' && c <= 'f') ||
                     (c >= 'A' && c <= 'F');
        }

        /// <summary>
        /// 将输入的字符过滤成只包含16进制数的结果
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string HexInput(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if (IsHex(c) || c == ' ')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}
