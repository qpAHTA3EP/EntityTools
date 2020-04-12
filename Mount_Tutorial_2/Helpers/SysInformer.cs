using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace SysInfo
{
    internal static class Helper
    {
        internal static string GetMashineID()
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

            return Encrypt(idBuilder.ToString(), "fs5er4z6#'f1dsg3regjuty6k@(");
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

        public static string SHA1_Bytes2String(byte[] byte_0)
        {
            byte[] array = new SHA1CryptoServiceProvider().ComputeHash(byte_0);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
            {
                stringBuilder.Append(array[i].ToString("X2"));
            }
            return stringBuilder.ToString();
        }

        internal static string Encrypt(string data, string cryptKey)
        {
            UTF8Encoding utf8Encoding = new UTF8Encoding();
            MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider();
            byte[] key = md5CryptoServiceProvider.ComputeHash(utf8Encoding.GetBytes(cryptKey));
            TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();
            tripleDESCryptoServiceProvider.Key = key;
            tripleDESCryptoServiceProvider.Mode = CipherMode.ECB;
            tripleDESCryptoServiceProvider.Padding = PaddingMode.PKCS7;
            byte[] bytes = utf8Encoding.GetBytes(data);
            byte[] inArray;
            try
            {
                inArray = tripleDESCryptoServiceProvider.CreateEncryptor().TransformFinalBlock(bytes, 0, bytes.Length);
            }
            finally
            {
                tripleDESCryptoServiceProvider.Clear();
                md5CryptoServiceProvider.Clear();
            }
            return Convert.ToBase64String(inArray);
        }

        internal static string Decrypt(string data, string cryptKey)
        {
            string result;
            try
            {
                UTF8Encoding utf8Encoding = new UTF8Encoding();
                MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider();
                byte[] key = md5CryptoServiceProvider.ComputeHash(utf8Encoding.GetBytes(cryptKey));
                TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();
                tripleDESCryptoServiceProvider.Key = key;
                tripleDESCryptoServiceProvider.Mode = CipherMode.ECB;
                tripleDESCryptoServiceProvider.Padding = PaddingMode.PKCS7;
                byte[] array = Convert.FromBase64String(data);
                byte[] bytes;
                try
                {
                    bytes = tripleDESCryptoServiceProvider.CreateDecryptor().TransformFinalBlock(array, 0, array.Length);
                }
                finally
                {
                    tripleDESCryptoServiceProvider.Clear();
                    md5CryptoServiceProvider.Clear();
                }
                result = utf8Encoding.GetString(bytes);
            }
            catch
            {
                result = string.Empty;
            }
            return result;
        }
    }
}
