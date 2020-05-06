﻿using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using Encryptor;

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
                    byte[] bytes = CryptoHelper.Encrypt_Astral(idBuilder.ToString().TextToBytes(), publicKey);
                    if (bytes != null)
                    {
                        return bytes.ToHexString();
                    }
                }
                else return idBuilder.ToString();
            }
            return string.Empty;
        }
    }
}