using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
#if ENCRYPTOR
using Extensions;
using Encrypter;
#else

#endif

namespace EntityTools.Tools
{
    public static class CryptoHelper
    {
#if ENCRYPTOR || ENTITYTOOLS
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
#if DEBUG && !ENTITYTOOLS
                    File.WriteAllText("keyHash_decrypt", keyHash.ToHexString());
#endif

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
                        // *2*
                        //byte[] iv = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
#if DEBUG && !ENTITYTOOLS
                        File.WriteAllText("ir_decrypt", IV.ToHexString());
                        File.WriteAllText("encrDataFull_decrypt", data.ToHexString());
#endif
#if false
                        // Удаляем из массива исходных данных Соль (IV)
                        byte[] encryptedData = new byte[data.Length - 16];
                        Array.Copy(data, encryptedData, encryptedData.LongLength);
                        //Array.Resize(ref data, data.Length - 16);
                        //data.CopyTo(out byte[] encryptedData, data.Length - 16);
#if DEBUG && !ENTITYTOOLS
                        File.WriteAllText("encrData_decrypt", encryptedData.ToHexString());
#endif  
#endif

                        using (MemoryStream memStream = new MemoryStream())
                        {
                            //Create a new instance of the RijndaelManaged class  
                            // and encrypt the stream.  
                            RijndaelManaged rijndael = new RijndaelManaged();
                            rijndael.KeySize = keyHash.Length * 8;
                            rijndael.BlockSize = rijndael.KeySize;
                            //rijndael.Key = keyHash;
                            //rijndael.IV = iv;

                            //Create a CryptoStream, and decrypt MemoryStream with the Rijndael class.  
                            using (CryptoStream cryptoStream = new CryptoStream(memStream,
                                                            rijndael.CreateDecryptor(keyHash, IV),
                                                            CryptoStreamMode.Write))
                            {
                                // *4*
                                // Дешифруем данные без СОЛИ
                                cryptoStream.Write(data, offset, data.Length - offset);
                                cryptoStream.FlushFinalBlock();

                                if (memStream.Length > 0)
                                {
                                    decryptedData = memStream.ToArray();
                                    return decryptedData != null && decryptedData.Length > 0;
                                }
                                // *3*
                                //using (MemoryStream outMemStream = new MemoryStream())
                                //{
                                //    int read;
                                //    while(cryptoStream.CanRead
                                //          && (read = cryptoStream.ReadByte()) != -1)
                                //    {
                                //        outMemStream.WriteByte((byte)read);
                                //    }
                                //    decryptedData = outMemStream.ToArray();
                                //    return decryptedData != null && decryptedData.Length > 0;
                                //}
                                // *2*
                                //if (cryptoStream.CanRead)
                                //{
                                //    long len = cryptoStream.Length;
                                //    decryptedData = new byte[len];
                                //    int readed = cryptoStream.Read(decryptedData, 0, (int)len);
                                //    return true;
                                //}

                                // *1*
                                //using (BinaryReader reader = new BinaryReader(cryptoStream))
                                //{
                                //    //Результат записываем в переменную text в вие исходной строки
                                //    long len = reader.BaseStream.Length;
                                //    decryptedData = reader.ReadBytes((int)len);
                                //    return true;
                                //}
                                cryptoStream.Close();
                            }
                            memStream.Close();
                        }
                    }
#if ENCRYPTOR
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                else Console.WriteLine($"Incorrect Key"); 
            }
            else Console.WriteLine($"Incorrect Data");
#else
                    catch
                    {
                        // ignored
                    }
                }
            }
#endif

            decryptedData = null;
            return false;
        }
#if false
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
#endif
#endif
#if ENCRYPTOR 
        public static bool EncryptFile_Rijndael(byte[] data, byte[] key, out byte[] encryptedData)
        {
            if (data != null && data.Length > 0)
            {
                if (key != null && key.Length > 0)
                {
                    SHA256 sha256 = SHA256.Create();
                    // Если длина ключа не соответствует требованиям
                    // в качестве ключа используется 256 битный хэш ключа
                    byte[] keyHash = (key.Length % 8 == 0 && key.Length <= 32) ? key : sha256.ComputeHash(key);
#if DEBUG
                    File.WriteAllText("keyHash_encrypt", keyHash.ToHexString());
#endif

                    MemoryStream memStream = null;
                    CryptoStream cryptoStream = null;
                    try
                    {
                        memStream = new MemoryStream();

                        //Create a new instance of the RijndaelManaged class  
                        // and encrypt the stream.  
                        RijndaelManaged rijndael = new RijndaelManaged();

                        // _1_ Соль генерируется случайно
                        //rijndael.GenerateIV();
                        //rijndael.KeySize = keyHash.Length * 8;
                        //rijndael.BlockSize = 256;
                        //byte[] IV = rijndael.IV;
                        // _2_ Фиксированая соль
                        //byte[] IV = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };

                        // _3_ Соль генерируется случайно
                        rijndael.GenerateIV();
                        rijndael.KeySize = keyHash.Length * 8;
                        rijndael.BlockSize = rijndael.KeySize;
                        byte[] IV = rijndael.IV;
                        // записываем Соль в MemStream
                        byte[] lenIVbytes = BitConverter.GetBytes(IV.Length);
                        memStream.Write(lenIVbytes, 0, lenIVbytes.Length);
                        memStream.Write(IV, 0, IV.Length);

#if DEBUG
                        File.WriteAllText("ir_encrypt", IV.ToHexString());
#endif
                        //Create a CryptoStream, and encrypt MemoryStream with the Rijndael class.  
                        cryptoStream = new CryptoStream(memStream,
                                                        rijndael.CreateEncryptor(keyHash, IV),
                                                        CryptoStreamMode.Write);

                        cryptoStream.Write(data, 0, data.Length);
                        cryptoStream.FlushFinalBlock();
#if DEBUG
                        File.WriteAllText("encrData_encrypt", memStream.ToArray().ToHexString());
#endif
                        // добавляем соль и выводим результат
                        if (memStream.Length > IV.Length + lenIVbytes.Length)
                        {
#if _1_ || _2_
                            memStream.Write(IV, 0, IV.Length);
#endif
                            encryptedData = memStream.ToArray();
                            cryptoStream.Close();
                            memStream.Close();
                            return encryptedData != null && encryptedData.Length >= IV.Length + lenIVbytes.Length;
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    finally
                    {
                        if (cryptoStream != null)
                        {
                            //cryptoStream.Close();
                            cryptoStream.Dispose();
                        }
                        if (memStream != null)
                        {
                            //memStream.Close();
                            memStream.Dispose();
                        }
                    }
                }
#if ENCRYPTOR
                else Console.WriteLine($"Incorrect Key");
            }
            else Console.WriteLine($"Incorrect Data");
#else   
            }
#endif
            encryptedData = null;
            return false;
        }
#if false
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
            if(encrypted != null && encrypted.Length >0)
                return encrypted.Concat(aes.IV).ToArray();
            return encrypted;
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
                Console.Read();
            }
        } 
#endif
#endif
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
