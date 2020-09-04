using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extensions
{
    public static class HexConverter
    {
        public static string ToHexString(this byte[] bytes)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                stringBuilder.Append(bytes[i].ToString("X2"));
            }
            return stringBuilder.ToString();
        }

        public static string ToNormalString(this byte[] bytes)
        {
            if(bytes!= null && bytes.Length > 0)
                return Encoding.UTF8.GetString(bytes);
            return string.Empty;
        }

        public static byte[] TextToBytes(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return new byte[0];
            else return Encoding.UTF8.GetBytes(str);
        }

#if ENCRYPTOR || TEST

        public static void CopyTo(this byte[] from, out byte[] to, long len)
        {
            if(from.Length > 0 && len > 0)
            {
                long length = Math.Min(from.Length, len);
                long iter = length / 32;
                to = new byte[length];
                for(long i = 0; i < iter; i++)
                {
                    to[i] = from[i];
                    to[i + 1] = from[i + 1];
                    to[i + 2] = from[i + 2];
                    to[i + 3] = from[i + 3];
                    to[i + 4] = from[i + 4];
                    to[i + 5] = from[i + 5];
                    to[i + 6] = from[i + 6];
                    to[i + 7] = from[i + 7];

                    to[i + 8] = from[i + 8];
                    to[i + 9] = from[i + 9];
                    to[i + 10] = from[i + 10];
                    to[i + 11] = from[i + 11];
                    to[i + 12] = from[i + 12];
                    to[i + 13] = from[i + 13];
                    to[i + 14] = from[i + 14];
                    to[i + 15] = from[i + 15];

                    to[i + 16] = from[i + 16];
                    to[i + 17] = from[i + 17];
                    to[i + 18] = from[i + 18];
                    to[i + 19] = from[i + 19];
                    to[i + 20] = from[i + 20];
                    to[i + 21] = from[i + 21];
                    to[i + 22] = from[i + 22];
                    to[i + 23] = from[i + 23];

                    to[i + 24] = from[i + 24];
                    to[i + 25] = from[i + 25];
                    to[i + 26] = from[i + 26];
                    to[i + 27] = from[i + 27];
                    to[i + 28] = from[i + 28];
                    to[i + 29] = from[i + 29];
                    to[i + 30] = from[i + 30];
                    to[i + 31] = from[i + 31];
                }
                for(long i = iter * 32; i < length; i++)
                {
                    to[i] = from[i];
                }
                return;
            }
            to = null;
        }

        public static byte[] HexToBytes(this string hex)
        {
            if (hex.Length % 2 == 1)
                throw new Exception("The binary key cannot have an odd number of digits");

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        } 
        private static int GetHexVal(char hex)
        {
            int val = (int)hex;
            //For uppercase A-F letters:
            return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            //return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }
#endif
    }
}
