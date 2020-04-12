using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Encrypter
{
    class Program
    {
        public enum Mode
        {
            None,
            Encrypt,
            Decrypt,
            Hash,
            GenerateKey
        }

        static void Main(string[] args)
        {
            Mode mode = Mode.None;
            string dataFile = null;
            string keyFile = null;

            if(args.Length > 0)
            {
                string arg1 = args[0];
                if(arg1 == "-e")
                {
                    if(args.Length > 1)
                    {
                        dataFile = args[1];
                        if (!File.Exists(dataFile))
                        {
                            Console.WriteLine($"Not found DataFile: {dataFile}");
                            Console.ReadKey();
                            return;
                        }
                    }
                    if(args.Length > 2)
                    {
                        keyFile = args[2];
                        if (!File.Exists(keyFile))
                        {
                            Console.WriteLine($"Not found KeyFile: {keyFile}");
                            Console.ReadKey();
                            return;
                        }
                    }
                    mode = Mode.Encrypt;
                }
                else if(arg1 == "-d")
                {
                    if (args.Length > 1)
                    {
                        dataFile = args[1];
                        if (!File.Exists(dataFile))
                        {
                            Console.WriteLine($"Not found DataFile: {dataFile}");
                            Console.ReadKey();
                            return;
                        }
                    }
                    if (args.Length > 2)
                    {
                        keyFile = args[2];
                        if (!File.Exists(keyFile))
                        {
                            Console.WriteLine($"Not found KeyFile: {keyFile}");
                            Console.ReadKey();
                            return;
                        }
                    }
                    mode = Mode.Decrypt;
                }
                else if (arg1 == "-k")
                {
                    if (args.Length > 1)
                    {
                        keyFile = args[1];
                        if (!File.Exists(keyFile))
                        {
                            Console.WriteLine($"Not found KeyFile: {keyFile}");
                            Console.ReadKey();
                            return;
                        }
                        else keyFile = "EntityToolKey.key";
                    }
                    mode = Mode.GenerateKey;
                }
                else if(arg1 == "-md5")
                {
                    if (args.Length > 1)
                    {
                        dataFile = args[1];
                        if (!File.Exists(dataFile))
                        {
                            Console.WriteLine($"Not found source File: {dataFile}");
                            Console.ReadKey();
                            return;
                        }
                        else keyFile = "EntityToolKey.key";
                    }
                    mode = Mode.Hash;
                }
                else if(arg1 == "-h" || arg1 == "-help")
                {
                    Console.WriteLine(@"encrypter [-e <DataFile> <KeyFile>] | [-d <DataFile> <KeyFile>] | -h");
                    Console.WriteLine(@"encrypter <DataFile> <KeyFile>");
                    Console.WriteLine(@"encrypter [-k] [<KeyFile>]");
                    Console.WriteLine("\t-e\tEncrypt <DataFile> with the key information from the <KeyFile>");
                    Console.WriteLine("\t\tEncrypt mode is used if flag was skipped and specified only <DataFile> and <KeyFile> ");
                    Console.WriteLine("\t-d\tDecrypt encrypted <DataFile> with file <KeyFile>");
                    Console.WriteLine("\t-k\tGenerage <KeyFile>");
                    Console.WriteLine("\t-h\tThis help message");
                    Console.ReadKey();
                    return;
                }
                else
                {
                    if (!File.Exists(arg1))
                    {
                        dataFile = arg1;
                        if (args.Length > 1)
                        {
                            keyFile = args[1];
                            if (!File.Exists(keyFile))
                            {
                                Console.WriteLine($"Not found KeyFile: {keyFile}");
                                Console.ReadKey();
                                return;
                            }
                            mode = Mode.Encrypt;
                        }
                        else
                        {
                            Console.WriteLine($"No valid arguments!");
                            Console.ReadKey();
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Not found DataFile: {arg1}");
                        Console.ReadKey();
                        return;
                    }
                }
            }

            switch(mode)
            {
                case Mode.GenerateKey:
                    if (string.IsNullOrEmpty(keyFile))
                        keyFile = "EntityToolKey.key";
                    string keyCode = SysInfo.SysInformer.GetMashineID(true);
                    File.WriteAllText(keyFile, keyCode);

                    //using (TextWriter file = new StreamWriter(keyFile))
                    //{
                    //    file.Write(SysInfo.SysInformer.GetMashineID(true));
                    //}
                    Console.WriteLine("KeyCode: ");
                    Console.WriteLine(keyCode);
                    Console.ReadKey();
                    return;
                case Mode.Hash:
                    byte[] bytes = File.ReadAllBytes(dataFile);
                    if(bytes != null && bytes.Length > 0)
                    {
                        string hashFile = string.Concat(Path.GetDirectoryName(dataFile), Path.PathSeparator, Path.GetFileNameWithoutExtension(dataFile), ".hash");
                        string hashCode = SysInfo.SysInformer.MD5_Bytes2String(bytes);
                        File.WriteAllText(hashFile, hashCode);
                        Console.WriteLine("HashCode of the file: ");
                        Console.Write(dataFile);
                        Console.WriteLine(hashCode);
                        Console.ReadKey();
                    }
                    return;
                case Mode.Encrypt:
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
                                    string encryptedFile = Path.Combine(Path.GetDirectoryName(dataFile), Path.GetFileNameWithoutExtension(dataFile) + ".etcrypt");
                                    try
                                    {
                                        using (FileStream outDataStream = new FileStream(encryptedFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                                        {
                                            //Create a new instance of the RijndaelManaged class  
                                            // and encrypt the stream.  
                                            RijndaelManaged rmCrypto = new RijndaelManaged();

                                            // byte[] key = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
                                            byte[] iv = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };

                                            //Create a CryptoStream, pass it the NetworkStream, and encrypt   
                                            //it with the Rijndael class.  
                                            CryptoStream cryptStream = new CryptoStream(outDataStream,
                                                                                        rmCrypto.CreateEncryptor(key, iv),
                                                                                        CryptoStreamMode.Write);

                                            //Create a StreamWriter for easy writing to the   
                                            //network stream.  
                                            StreamWriter sWriter = new StreamWriter(cryptStream);

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
                    return;
                case Mode.Decrypt:
                    if (File.Exists(dataFile))
                    {
                        if (File.Exists(keyFile))
                        {

                        }
                        else Console.WriteLine($"Not found KeyFile: {keyFile}");
                    }
                    else Console.WriteLine($"Not found DataFile: {dataFile}");
                    return;
            }
            Console.ReadKey();
        }
    }
}
