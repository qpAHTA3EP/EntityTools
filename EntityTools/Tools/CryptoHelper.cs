using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace EntityTools.Tools
{
    public static class CryptoHelper
    {
        public static bool DecryptFile_Rijndael(byte[] data, byte[] key, out byte[] decryptedData)
        {
            if (data != null && data.Length > 0)
            {
                if (key != null && key.Length > 0)
                {
                    //PasswordDeriveBytes
                    SHA256 sha256 = SHA256.Create();
                    // Если длина ключа не соответствует требованиям
                    // в качестве ключа используется 256 битный хэш ключа
                    byte[] keyHash = (key.Length % 8 == 0 && key.Length <= 32) ? key : sha256.ComputeHash(key);

                    try
                    {
                        // *1* Считываем Соль из файла зашифрованных данных
                        int intLen = BitConverter.GetBytes(default(int)).Length;
                        int offset = 0;
                        int lenIV = BitConverter.ToInt32(data, offset);
                        offset += intLen;
                        byte[] IV = new byte[lenIV];
                        Array.Copy(data, offset, IV, 0, IV.Length);
                        offset += IV.Length;


                        using (MemoryStream memStream = new MemoryStream())
                        {
                            // Create a new instance of the RijndaelManaged class and encrypt the stream.  
                            using (RijndaelManaged rijndael = new RijndaelManaged())
                            {
                                rijndael.KeySize = keyHash.Length * 8;
                                rijndael.BlockSize = rijndael.KeySize;

                                //Create a CryptoStream, and decrypt MemoryStream with the Rijndael class.  
                                using (CryptoStream cryptoStream = new CryptoStream(memStream,
                                           rijndael.CreateDecryptor(keyHash, IV),
                                           CryptoStreamMode.Write))
                                {
                                    // Дешифруем данные без СОЛИ
                                    cryptoStream.Write(data, offset, data.Length - offset);
                                    cryptoStream.FlushFinalBlock();

                                    if (memStream.Length > 0)
                                    {
                                        decryptedData = memStream.ToArray();
                                        return decryptedData != null && decryptedData.Length > 0;
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }

            decryptedData = null;
            return false;
        }

        public static bool Encrypt_Astral(byte[] data, string cryptKey, out byte[] encryptedData)
        {
            if (data?.Length > 0)
            {
                using (MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider())
                {
                    byte[] key = md5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(cryptKey));
                    using (TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider())
                    {
                        tripleDESCryptoServiceProvider.Key = key;
                        tripleDESCryptoServiceProvider.Mode = CipherMode.ECB;
                        tripleDESCryptoServiceProvider.Padding = PaddingMode.PKCS7;
                        try
                        {
                            encryptedData = tripleDESCryptoServiceProvider.CreateEncryptor().TransformFinalBlock(data, 0, data.Length);
                        }
                        finally
                        {
                            tripleDESCryptoServiceProvider.Clear();
                            md5CryptoServiceProvider.Clear();
                        }
                        return encryptedData != null && encryptedData.Length > 0;
                    } 
                }
            }
            encryptedData = null;
            return false;
        }

        public static bool Decrypt_Astral(byte[] data, string cryptKey, out byte[] decryptedData)
        {
            if (data != null && data.Length > 0)
            {
                try
                {
                    using (MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider())
                    {
                        byte[] key = md5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(cryptKey));

                        using (TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider())
                        {
                            tripleDESCryptoServiceProvider.Key = key;
                            tripleDESCryptoServiceProvider.Mode = CipherMode.ECB;
                            tripleDESCryptoServiceProvider.Padding = PaddingMode.PKCS7;
                            try
                            {
                                decryptedData = tripleDESCryptoServiceProvider.CreateDecryptor().TransformFinalBlock(data, 0, data.Length);
                            }
                            finally
                            {
                                tripleDESCryptoServiceProvider.Clear();
                                md5CryptoServiceProvider.Clear();
                            }
                            return decryptedData != null && decryptedData.Length > 0;  
                        }
                    }
                }
                catch
                {
                    // ignored
                }
            }
            decryptedData = null;
            return false;
        }

        public static string MD5_HashString(byte[] bytes)
        {
            byte[] array = MD5.Create().ComputeHash(bytes);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
            {
                stringBuilder.Append(array[i].ToString("X2"));
            }
            return stringBuilder.ToString();
        }
    }
}
