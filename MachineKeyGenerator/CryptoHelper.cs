using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Encryptor
{
    public static class CryptoHelper
    {
#if ENCRYPTOR
        public static void DecryptFile(string dataFile, string keyFile)
        {
            if (File.Exists(dataFile))
            {
                if (File.Exists(keyFile))
                {
                    throw new NotImplementedException();
                }
                else Console.WriteLine($"Not found KeyFile: {keyFile}");
            }
            else Console.WriteLine($"Not found DataFile: {dataFile}");
        }

        /// <summary>
        /// https://www.fluxbytes.com/csharp/encrypt-and-decrypt-files-in-c/
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        /// <param name="skey"></param>
        public static void DecryptFile(string inputFile, string outputFile, string skey)
        {
            try
            {
                using (RijndaelManaged aes = new RijndaelManaged())
                {
                    byte[] key = ASCIIEncoding.UTF8.GetBytes(skey);

                    /* This is for demostrating purposes only. 
                     * Ideally you will want the IV key to be different from your key and you should always generate a new one for each encryption in other to achieve maximum security*/
                    byte[] IV = ASCIIEncoding.UTF8.GetBytes(skey);

                    using (FileStream fsCrypt = new FileStream(inputFile, FileMode.Open))
                    {
                        using (FileStream fsOut = new FileStream(outputFile, FileMode.Create))
                        {
                            using (ICryptoTransform decryptor = aes.CreateDecryptor(key, IV))
                            {
                                using (CryptoStream cs = new CryptoStream(fsCrypt, decryptor, CryptoStreamMode.Read))
                                {
                                    int data;
                                    while ((data = cs.ReadByte()) != -1)
                                    {
                                        fsOut.WriteByte((byte)data);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // failed to decrypt file
            }
        }

        /// <summary>
        /// Расшифровывает криптованного сообщения
        /// https://razilov-code.ru/2018/01/08/aes-256-c-sharp/
        /// </summary>
        /// <param name="shifr">Шифротекст в байтах</param>
        /// <returns>Возвращает исходную строку</returns>
        public static string DecryptFile_Aes256(byte[] shifr)
        {
            byte[] bytesIv = new byte[16];
            byte[] mess = new byte[shifr.Length - 16];

            //Списываем соль
            for (int i = shifr.Length - 16, j = 0; i < shifr.Length; i++, j++)
                bytesIv[j] = shifr[i];

            //Списываем оставшуюся часть сообщения
            for (int i = 0; i < shifr.Length - 16; i++)
                mess[i] = shifr[i];

            //Объект класса Aes
            Aes aes = Aes.Create();
            //Задаем тот же ключ, что и для шифрования
            aes.Key = aeskey;
            //Задаем соль
            aes.IV = bytesIv;
            //Строковая переменная для результата
            string text = "";
            byte[] data = mess;
            ICryptoTransform crypt = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream(data))
            {
                using (CryptoStream cs = new CryptoStream(ms, crypt, CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        //Результат записываем в переменную text в вие исходной строки
                        text = sr.ReadToEnd();
                    }
                }
            }
            return text;
        }

        public static void EncryptFile(string dataFile, string keyFile)
        {
            if (File.Exists(dataFile))
            {
                if (File.Exists(keyFile))
                {
                    throw new NotImplementedException();
                }
            }
        }

        public static void EncryptFile1(string dataFile, string keyFile)
        {
            if (File.Exists(dataFile))
            {
                if (File.Exists(keyFile))
                {
                    byte[] encryptedKeyData = File.ReadAllBytes(keyFile);
                    if (encryptedKeyData != null && encryptedKeyData.Length > 0)
                    {
                        byte[] keyData = SysInfo.SysInformer.Decrypt(encryptedKeyData, );
                        byte[] key = SHA256.Create().ComputeHash(encryptedKeyData);

                        byte[] inData = File.ReadAllBytes(dataFile);
                        if (inData.Length > 0)
                        {
                            string encryptedFile = Path.Combine(Path.GetDirectoryName(dataFile), Path.GetFileNameWithoutExtension(dataFile) + ".encrypt");
                            try
                            {
                                using (FileStream outDataStream = new FileStream(encryptedFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                                {
                                    //Create a new instance of the RijndaelManaged class  
                                    // and encrypt the stream.  
                                    RijndaelManaged rmCrypto = new RijndaelManaged();

                                    // byte[] key = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
                                    byte[] iv = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };

                                    //Create a CryptoStream, and encrypt FileStream with the Rijndael class.  
                                    CryptoStream cryptStream = new CryptoStream(outDataStream,
                                                                                rmCrypto.CreateEncryptor(key, iv),
                                                                                CryptoStreamMode.Write);

                                    //Create a StreamWriter for easy writing to the   
                                    //network stream.  
                                    StreamWriter sWriter = new StreamWriter(cryptStream);

                                    throw new NotImplementedException();

                                    //Write to the stream.  
                                    sWriter.WriteLine("Hello World!");

                                    //Inform the user that the message was written  
                                    //to the stream.  
                                    Console.WriteLine("The message was sent.");

                                    //Close all the connections.  
                                    sWriter.Close();
                                    cryptStream.Close();
                                }
                            }
                            catch
                            {
                                //Inform the user that an exception was raised.  
                                Console.WriteLine("The connection failed.");
                            }
                        }
                    }
                }
                else Console.WriteLine($"Not found KeyFile: {keyFile}");
            }
            else Console.WriteLine($"Not found DataFile: {dataFile}");
        }

        /// <summary>
        /// Шифрует исходное сообщение AES ключом (добавляет соль)
        /// https://razilov-code.ru/2018/01/08/aes-256-c-sharp/
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static byte[] EncryptFile_Aes256(string src)
        {
            //Объявляем объект класса AES
            Aes aes = Aes.Create();
            //Генерируем соль
            aes.GenerateIV();
            //Присваиваем ключ. aeskey - переменная (массив байт), сгенерированная методом GenerateKey() класса AES
            aes.Key = aeskey;
            byte[] encrypted;
            ICryptoTransform crypt = aes.CreateEncryptor(aes.Key, aes.IV);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, crypt, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(src);
                    }
                }
                //Записываем в переменную encrypted зашиврованный поток байтов
                encrypted = ms.ToArray();
            }
            //Возвращаем поток байт + крепим соль
            return encrypted.Concat(aes.IV).ToArray();
        }


        /// <summary>
        /// https://www.fluxbytes.com/csharp/encrypt-and-decrypt-files-in-c/
        /// http://csharpcoderr.com/2012/08/blog-post.html
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        /// <param name="skey"></param>
        public static void EncryptFile(string inputFile, string outputFile, string skey)
        {
            RijndaelManaged aes = new RijndaelManaged();

            try
            {
                byte[] key = ASCIIEncoding.UTF8.GetBytes(skey);
                using (FileStream fsCrypt = new FileStream(outputFile, FileMode.Create))
                {
                    /* This is for demostrating purposes only. 
                     * Ideally you will want the IV key to be different from your key and you should always generate a new one for each encryption in other to achieve maximum security*/
                    using (CryptoStream cs = new CryptoStream(fsCrypt, aes.CreateEncryptor(key, key), CryptoStreamMode.Write))
                    {
                        using (FileStream fsIn = new FileStream(inputFile, FileMode.Open))
                        {
                            int data;
                            while ((data = fsIn.ReadByte()) != -1)
                            {
                                cs.WriteByte((byte)data);
                            }
                            aes.Clear();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                aes.Clear();
            }
        }

        public static void MakeHash(string dataFile)
        {
            byte[] bytes = File.ReadAllBytes(dataFile);
            if (bytes != null && bytes.Length > 0)
            {
                string hashFile = string.Concat(Path.GetDirectoryName(dataFile), Path.PathSeparator, Path.GetFileNameWithoutExtension(dataFile), ".hash");
                string hashCode = MD5_HashString(bytes);
                File.WriteAllText(hashFile, hashCode);
                Console.WriteLine("HashCode of the file: ");
                Console.Write(dataFile);
                Console.WriteLine(hashCode);
                Console.ReadKey();
            }
        } 
#endif
        public static byte[] Encrypt_Astral(byte[] data, string cryptKey)
        {
            if (data == null || data.Length == 0)
                return null;

            MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider();
            byte[] key = md5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(cryptKey));
            TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider
            {
                Key = key,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            byte[] inArray = null;
            try
            {
                inArray = tripleDESCryptoServiceProvider.CreateEncryptor().TransformFinalBlock(data, 0, data.Length);
            }
            finally
            {
                tripleDESCryptoServiceProvider.Clear();
                md5CryptoServiceProvider.Clear();
            }
            return inArray;
        }

        public static byte[] Decrypt_Astral(byte[] data, string cryptKey)
        {
            byte[] result = null;
            try
            {
                MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider();
                byte[] key = md5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(cryptKey));
                TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider
                {
                    Key = key,
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                };
                byte[] bytes;
                try
                {
                    bytes = tripleDESCryptoServiceProvider.CreateDecryptor().TransformFinalBlock(data, 0, data.Length);
                }
                finally
                {
                    tripleDESCryptoServiceProvider.Clear();
                    md5CryptoServiceProvider.Clear();
                }
                result = bytes;
            }
            catch
            {
                result = null;
            }
            return result;
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
#if ENCRYPTOR
        public static string SHA1_HashString(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return string.Empty;

            byte[] sha1Hash = new SHA1CryptoServiceProvider().ComputeHash(bytes);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < sha1Hash.Length; i++)
            {
                stringBuilder.Append(sha1Hash[i].ToString("X2"));
            }
            return stringBuilder.ToString();
        }
#endif
    }
}
