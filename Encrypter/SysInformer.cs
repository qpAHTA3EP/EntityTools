using MachineKeyGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace SysInfo
{
    internal static class SysInformer
    {
        public static readonly string publicKey = "fs5er4z6#'f1dsg3regjuty6k@(";

        internal static string GetMashineID(bool encrypt)
        {

            StringBuilder idBuilder = new StringBuilder();
            ManagementClass mc = new ManagementClass("Win32_Processor");
            foreach (ManagementObject processor in mc.GetInstances())
            {
                string val = processor.GetPropertyValue("UniqueId")?.ToString();
                if (!string.IsNullOrEmpty(val))
                {
                    if (idBuilder.Length > 0)
                        idBuilder.Append('_');
                    idBuilder.Append(val);
                }
                else
                {
                    val = processor.GetPropertyValue("ProcessorId")?.ToString();
                    if (!string.IsNullOrEmpty(val))
                    {
                        if (idBuilder.Length > 0)
                            idBuilder.Append('_');
                        idBuilder.Append(val);
                    }
                }
            }

            // Альтернативный метод получения информации
            //string ProcessorId = string.Empty;
            //ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_Processor");
            //foreach (ManagementObject mObj in searcher.Get())
            //{
            //    //ProcessorId = manadgeObj.Path.Path;
            //    /*foreach (PropertyData property in mObj.Properties)
            //    {

            //        if(property.Name == "ProcessorId")
            //        {
            //            //ProcessorId += '_' + property.Name;
            //            ProcessorId = property.Value.ToString();
            //            break;
            //        }
            //    }*/
            //    ProcessorId = mObj.Properties["ProcessorId"].Value.ToString();
            //    if (!string.IsNullOrEmpty(ProcessorId))
            //        break;
            //}//*/
            /*ManagementPath mPath = new ManagementPath(ManagementPath.DefaultPath + @":Win32_Processor");
            ManagementObject processor = new ManagementObject(mPath);
            ProcessorId = processor.Properties["ProcessorId"].Value?.ToString();//*/

            mc = new ManagementClass("Win32_BaseBoard");
            foreach (ManagementObject babeBoard in mc.GetInstances())
            {
                string val = babeBoard.GetPropertyValue("SerialNumber")?.ToString();
                if (!string.IsNullOrEmpty(val))
                    idBuilder.Append('_').Append(val);
            }

            mc = new ManagementClass("Win32_BIOS");
            foreach (ManagementObject bios in mc.GetInstances())
            {
                string val = bios.GetPropertyValue("Version")?.ToString();
                if (!string.IsNullOrEmpty(val))
                    idBuilder.Append('_').Append(val);
            }

            mc = new ManagementClass("Win32_DiskDrive");
            foreach (ManagementObject disk in mc.GetInstances())
            {
                string diskInfo = string.Empty;
                string val = disk.GetPropertyValue("Model")?.ToString();
                if (!string.IsNullOrEmpty(val))
                    diskInfo = '_' + val;
                val = disk.GetPropertyValue("FirmwareRevision")?.ToString();
                if (!string.IsNullOrEmpty(val))
                    diskInfo += '_' + val;
                if (!string.IsNullOrEmpty(diskInfo))
                    idBuilder.Append('_').Append(diskInfo);
            }

            mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            foreach (ManagementObject networkAdaptr in mc.GetInstances())
            {
                string val = networkAdaptr.GetPropertyValue("MACAddress")?.ToString();
                if (!string.IsNullOrEmpty(val))
                    idBuilder.Append('_').Append(val);
            }

            if (idBuilder.Length > 0)
            {
                if (encrypt)
                {
                    byte[] bytes = Encrypt(Encoding.UTF8.GetBytes(idBuilder.ToString()), publicKey);
                    if (bytes != null)
                    {
                        return bytes.ToHexString();
                    }
                }
                else return idBuilder.ToString();
            }
            return string.Empty;
        }

        public static string MD5_Bytes2String(byte[] bytes)
        {
            byte[] array = MD5.Create().ComputeHash(bytes);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
            {
                stringBuilder.Append(array[i].ToString("X2"));
            }
            return stringBuilder.ToString();
        }

        public static string SHA1_Bytes2String(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return string.Empty;

            byte[] array = new SHA1CryptoServiceProvider().ComputeHash(bytes);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
            {
                stringBuilder.Append(array[i].ToString("X2"));
            }
            return stringBuilder.ToString();
        }

        internal static byte[] Encrypt(byte[] data, string cryptKey)
        {
            if (data == null || data.Length == 0)
                return null;

            MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider();
            byte[] key = md5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(cryptKey));
            TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider {
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

        internal static byte[] Decrypt(byte[] data, string cryptKey)
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
    }
}
