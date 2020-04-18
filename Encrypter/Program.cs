using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Extensions;
using System.Text;
using System.Reflection;

namespace Encrypter
{
    internal static partial class Constants
    {
        /// <summary>
        /// Время компиляции программы
        /// </summary>
        /// Решение:
        /// https://stackoverflow.com/users/1082110/dmitry-gusarov
        /// В файле проекта добавлена цель, генерирующая файл CompileTimestamp.cs с кодом инициализации переменной:
        /// <Target Name="Date" BeforeTargets="CoreCompile">
        ///  <WriteLinesToFile File = "$(IntermediateOutputPath)CompileTimestamp.cs" Lines="namespace Encrypter {&#xD;&#xA;internal static partial class Constants {&#xD;&#xA;static Constants(){ CompileTime = new System.DateTime(637225287830713522, System.DateTimeKind.Utc) %3B }&#xD;&#xA;}&#xD;&#xA;}" Overwrite="true" />
        ///  <ItemGroup>
        ///    <Compile Include = "$(IntermediateOutputPath)CompileTimestamp.cs" />
        ///  </ ItemGroup >
        /// </ Target >
        /// Символ '%3B' означает ';'
        /// Использование для этих целей кода &#xD; не подходит, т.к. студия производит замену и в сгенерированном файле символ ';' отсутствует, что приводит к ошибке времени компиляции

        internal static readonly DateTime CompileTime;
    }

    class Program
    {
        static readonly string DefaultKeyFile = "EntityTools.key";

        static void Main(string[] args)
        {
            //Console.WriteLine(Constants.CompileTime != null ? Constants.CompileTime.ToString() : "Empty");

            string dataFile = null;
            string keyFile = null;
            string targetFileName = null;

            if (args.Length > 0)
            {
                string arg1 = args[0];
#if DEBUG
                if(arg1 == "-test")
                {
                    string testKeyFile = "Test.key";
                    string testDataFile = "EntityCore.dll";
                    if (!File.Exists(testKeyFile))
                        GenerateKey(testKeyFile);
                    if(EncryptFile(testDataFile, testKeyFile, out byte[] encrData, null))
                        DecryptFile(Path.GetFileNameWithoutExtension(testDataFile) + ".encrypted", testKeyFile, out byte[] edcrData, null);
                }
                else 
#endif
                if (arg1 == "-e")
                {
                    if (args.Length > 1)
                    {
                        dataFile = args[1];
                        if (!File.Exists(dataFile))
                        {
                            Console.WriteLine($"Not found DataFile: {dataFile}");
                            Console.Read();
                            return;
                        }
                    }
                    if (args.Length > 2)
                    {
                        keyFile = args[2];
                        if (!File.Exists(keyFile))
                        {
                            Console.WriteLine($"Not found KeyFile: {keyFile}");
                            Console.Read();
                            return;
                        }
                    }
                    if (args.Length > 4)
                    {
                        string arg3 = args[3];
                        if (arg3 == "-to")
                            targetFileName = args[4];
                        else targetFileName = null;
                    }
                    else targetFileName = string.Empty;
                    EncryptFile(dataFile, keyFile, out byte[] decrData, targetFileName);
                }
                else   if(arg1 == "-d")
                {
                    if (args.Length > 1)
                    {
                        dataFile = args[1];
                        if (!File.Exists(dataFile))
                        {
                            Console.WriteLine($"Not found DataFile: {dataFile}");
                            Console.Read();
                            return;
                        }
                    }
                    if (args.Length > 2)
                    {
                        keyFile = args[2];
                        if (!File.Exists(keyFile))
                        {
                            Console.WriteLine($"Not found KeyFile: {keyFile}");
                            Console.Read();
                            return;
                        }
                    }
                    if (args.Length > 4)
                    {
                        string arg3 = args[3];
                        if (arg3 == "-to")
                            targetFileName = args[4];
                        else targetFileName = null;
                    }
                    else targetFileName = string.Empty;
                    DecryptFile(dataFile, keyFile, out byte[] decrData, targetFileName);
                }
                else if (arg1 == "-k")
                {
                    if (args.Length > 1)
                    {
                        keyFile = args[1];
                        if (!File.Exists(keyFile))
                        {
                            Console.WriteLine($"Not found KeyFile: {keyFile}");
                            keyFile = DefaultKeyFile;
                            Console.WriteLine($"Generate default: {keyFile}");
                        }
                    }
                    else
                    {
                        keyFile = DefaultKeyFile;
                        Console.WriteLine($"Generate default: {keyFile}");
                    }
                    GenerateKey(keyFile);
                }
                else if (arg1 == "-dk")
                {
                    if (args.Length > 1)
                    {
                        keyFile = args[1];
                        if (!File.Exists(keyFile))
                        {
                            Console.WriteLine($"Not found KeyFile: {keyFile}");
                            Console.Read();
                            keyFile = DefaultKeyFile;
                            Console.WriteLine($"Try default: {keyFile}");
                        }
                    }
                    else
                    {
                        keyFile = DefaultKeyFile;
                        Console.WriteLine($"Try default: {keyFile}");
                    }
                    DecryptKey(keyFile);
                }
                else if (arg1 == "-h" || arg1 == "-help")
                {
                    Console.WriteLine(@"encrypter -e <DataFile> <KeyFile>");
                    Console.WriteLine("\tEncrypt <DataFile> with the key information from the <KeyFile>");
                    Console.WriteLine();
                    Console.WriteLine(@"encrypter <DataFile> <KeyFile>");
                    Console.WriteLine("\tEncrypt <DataFile> with the key information from the <KeyFile>");
                    Console.WriteLine("\tEncrypt mode is used if flag was skipped and specified\n\r" +
                                      "\tonly <DataFile> and <KeyFile> ");
                    Console.WriteLine();
                    Console.WriteLine(@"encrypter -d <DataFile> <KeyFile>");
                    Console.WriteLine("\tDecrypt <DataFile> with key information from the <KeyFile>");
                    Console.WriteLine();
                    Console.WriteLine(@"encrypter -k <KeyFile>");
                    Console.WriteLine("\tGenerage <KeyFile> for the current PC");
                    Console.WriteLine();
                    Console.WriteLine(@"encrypter -dk <KeyFile>");
                    Console.WriteLine("\tRead <KeyFile>, decrypt it and type the information");
                    Console.WriteLine();
                    Console.WriteLine(@"encrypter -md5 <DataFile>");
                    Console.WriteLine("\tGenerate MD5 hash for the <DataFile>");
                    Console.WriteLine();
                    Console.WriteLine(@"encrypter -h | -help");
                    Console.WriteLine("\tThis help message");
                    Console.WriteLine();
                    Console.Read();
                    return;
                }
#if false
                else if (arg1 == "-md5")
                {
                    if (args.Length > 1)
                    {
                        dataFile = args[1];
                        if (!File.Exists(dataFile))
                        {
                            Console.WriteLine($"Not found source File: {dataFile}");
                            Console.Read();
                            return;
                        }
                        else keyFile = DefaultKeyFile;
                    }
                    CryptoHelper.MakeHash(dataFile);
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
                                Console.Read();
                                return;
                            }
                            CryptoHelper.EncryptFile(dataFile, keyFile);
                        }
                        else
                        {
                            Console.WriteLine($"No valid arguments!");
                            Console.Read();
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Not found DataFile: {arg1}");
                        Console.Read();
                        return;
                    }
                } 
#endif
            }
            Console.Read();
        }

        private static bool GenerateKey(string keyFile)
        {
            if (string.IsNullOrEmpty(keyFile) || string.IsNullOrWhiteSpace(keyFile))
                return false;

            string keyCode = SysInfo.SysInformer.GetMashineID(true);
            File.WriteAllText(keyFile, keyCode);

            //using (TextWriter file = new StreamWriter(keyFile))
            //{
            //    file.Write(SysInfo.SysInformer.GetMashineID(true));
            //}
            Console.WriteLine("KeyCode: ");
            Console.WriteLine(keyCode);
            return true;
        }

        private static void DecryptKey(string keyFile)
        {
            if (!string.IsNullOrEmpty(keyFile) && !string.IsNullOrWhiteSpace(keyFile)
                && File.Exists(keyFile))
            {
                Console.WriteLine($"KeyFile:\t{keyFile}");
                string hexKeyStr = File.ReadAllText(keyFile);
                Console.WriteLine($"Encryptedkey:\r\n{hexKeyStr}");
                if (SysInfo.SysInformer.MashineIDFromKey(hexKeyStr, out byte[] mashineIdBytes, out string mashinIdStr))
                {
                    Console.WriteLine($"Decryptedkey:\r\n{mashineIdBytes.ToHexString()}");
                    Console.WriteLine($"MashineInfo:\r\n{mashinIdStr}");
                    return;
                }
            }
            Console.WriteLine($"Incorrect KeyFile: {keyFile}");
        }

        /// <summary>
        /// Шифрование файла
        /// </summary>
        /// <param name="dataFile"></param>
        /// <param name="keyFile"></param>
        /// <param name="encryptedData"></param>
        /// <param name="targetFile">Имя файла, в который записываются зашифрованные данные
        /// Если Empty - Имя файл разультата генерируется из имени исходного файла ( с расширением ".encrypted")
        /// Если null - Результат не сохраняется в файл</param>
        /// <returns></returns>
        private static bool  EncryptFile(string dataFile, string keyFile, out byte[] encryptedData, string targetFile = "")
        {
            if (!string.IsNullOrEmpty(dataFile) && !string.IsNullOrWhiteSpace(dataFile)
                && File.Exists(dataFile))
            {
                if (!string.IsNullOrEmpty(keyFile) && !string.IsNullOrWhiteSpace(keyFile)
                    && File.Exists(keyFile))
                {
                    byte[] data = File.ReadAllBytes(dataFile);

#if DEBUG
                    string fullDataFile = Path.GetFullPath(dataFile);
                    string md5File = string.Concat(Path.GetDirectoryName(fullDataFile), Path.DirectorySeparatorChar, Path.GetFileNameWithoutExtension(fullDataFile), ".md5");
                    File.WriteAllText(md5File, CryptoHelper.MD5_HashString(data));
#endif


                    string hexKeyStr = File.ReadAllText(keyFile);
                    if (SysInfo.SysInformer.MashineIDFromKey(hexKeyStr, out byte[] mashineIdBytes, out string mashinIdStr))
                    {
                        if(CryptoHelper.EncryptFile_Rijndael(data, mashineIdBytes, out encryptedData))
                        {
                            if (targetFile != null)
                            {
                                // Необходимо сохранить в файл
                                // Имя целевого файла может быть задано абсолютным путем, тогда первое вхождение ':' будет относиться к идентификатору диска
                                int firstInd = targetFile.IndexOf(':');
                                bool isFullPath = false;
                                int lastInd = (isFullPath = targetFile.Contains(@":\")) ? targetFile.LastIndexOf(':', firstInd + 1) : targetFile.IndexOf(':');
                                if ((isFullPath && lastInd > firstInd) || (!isFullPath && lastInd > 0))
                                {
                                    // Проверяем наличие идентификатора альтернативного потока
                                    string targetFileName = targetFile.Substring(0, lastInd);
                                    string targetFileStream = targetFile.Substring(lastInd + 1);
                                    if (!string.IsNullOrEmpty(targetFileStream) && targetFileStream.Length > 0)
                                    {
                                        // Сохранить необходимо в альтернативный поток
                                        using (FileStream fs = FileStreamHelper.OpenWithStream(targetFileName, targetFileStream, FileMode.Create, FileAccess.Write))
                                        {
                                            fs.Write(encryptedData, 0, encryptedData.Length);
                                            Console.WriteLine($"Succeeded! EncryptedFile: {targetFile}");
                                            return true;
                                        }
                                    }
                                }
                                else
                                {
                                    // Сохранение в обычный файл
                                    string fileName = (targetFile != string.Empty) ? targetFile :
                                                                Path.Combine(Path.GetDirectoryName(dataFile), Path.GetFileNameWithoutExtension(dataFile) + ".encrypted");
                                    File.WriteAllBytes(fileName, encryptedData);
                                    Console.WriteLine($"Succeeded! EncryptedFile: {fileName}");
                                }
                                    
                            }
                            else Console.WriteLine($"Succeeded! The encryption is finished");
                            return true;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Encryption failed");
                    }
                }
                else Console.WriteLine($"Incorrect KeyFile: {keyFile}");
            }
            else Console.WriteLine($"Incorrect DataFile: {dataFile}");
            encryptedData = null;
            return false;
        }

        /// <summary>
        /// Дешифровка файла
        /// </summary>
        /// <param name="encryptedDataFile"></param>
        /// <param name="keyFile"></param>
        /// <param name="decryptedData"></param>
        /// <param name="targetFile">Имя файла, в который записываются расшифрованные данные
        /// Если Empty - Имя файл разультата генерируется из имени исходного файла ( с расширением ".decrypted")
        /// Если null - Результат не сохраняется в файл</param>
        /// <returns></returns>
        private static bool DecryptFile(string encryptedDataFile, string keyFile, out byte[] decryptedData, string targetFile = "")
        {
            if (!string.IsNullOrEmpty(encryptedDataFile) && !string.IsNullOrWhiteSpace(encryptedDataFile)
                && File.Exists(encryptedDataFile))
            {
                if (!string.IsNullOrEmpty(keyFile) && !string.IsNullOrWhiteSpace(keyFile)
                    && File.Exists(keyFile))
                {
                    byte[] data = File.ReadAllBytes(encryptedDataFile);

                    string hexKeyStr = File.ReadAllText(keyFile);
                    if (SysInfo.SysInformer.MashineIDFromKey(hexKeyStr, out byte[] mashineIdBytes, out string mashinIdStr))
                    {
                        if (CryptoHelper.DecryptFile_Rijndael(data, mashineIdBytes, out decryptedData))
                        {
                            if (targetFile != null)
                            {
                                // Необходимо сохранить в файл
                                // Имя целевого файла может быть задано абсолютным путем, тогда первое вхождение ':' будет относиться к идентификатору диска
                                int firstInd = targetFile.IndexOf(':');
                                bool isFullPath = false;
                                int lastInd = (isFullPath = targetFile.Contains(@":\")) ? targetFile.LastIndexOf(':', firstInd + 1) : targetFile.IndexOf(':');
                                if ((isFullPath && lastInd > firstInd) || (!isFullPath && lastInd > 0))
                                {
                                    // Проверяем наличие идентификатора альтернативного потока
                                    string targetFileName = targetFile.Substring(0, lastInd);
                                    string targetFileStream = targetFile.Substring(lastInd + 1);
                                    if (!string.IsNullOrEmpty(targetFileStream) && targetFileStream.Length > 0)
                                    {
                                        // Сохранить необходимо в альтернативный поток
                                        using (FileStream fs = FileStreamHelper.OpenWithStream(targetFileName, targetFileStream, FileMode.Create, FileAccess.Write))
                                        {
                                            fs.Write(decryptedData, 0, decryptedData.Length);
                                            Console.WriteLine($"Succeeded! DecryptedFile: {targetFile}");
                                            return true;
                                        }
                                    }
                                }
                                if (!string.IsNullOrEmpty(targetFile))
                                {
                                    string fileName = (targetFile != string.Empty) ? targetFile :
                                                            Path.Combine(Path.GetDirectoryName(encryptedDataFile), Path.GetFileNameWithoutExtension(encryptedDataFile) + ".decrypted");
                                    File.WriteAllBytes(fileName, decryptedData);
                                    Console.WriteLine($"Succeeded! DecryptedFile: {fileName}");
                                }
                            }
                            else Console.WriteLine($"Succeeded! The decryption is finished");
                            return true;
                        }
                        else
                        {
                            Console.WriteLine($"Decryption failed");
                            decryptedData = null;
                            return false;
                        }
                    }
                }
                Console.WriteLine($"Incorrect KeyFile: {keyFile}");
            }
            else Console.WriteLine($"Incorrect DataFile: {encryptedDataFile}");
            decryptedData = null;
            return false;
        }
    }
}
