using EntityTools.Extensions;
using EntityTools.Tools;
using System.Management;
using System.Text;

// ReSharper disable once CheckNamespace
namespace SysInfo
{
    internal static class SysInformer
    {
        private static readonly string publicKey = "fs5er4z6#'f1dsg3regjuty6k@(";

        internal static string GetMashineID(bool encrypt)
        {

            var idBuilder = new StringBuilder();
            var mc = new ManagementClass("Win32_Processor");
            foreach (var o in mc.GetInstances())
            {
                var processor = (ManagementObject) o;
                var val = processor.GetPropertyValue("UniqueId")?.ToString();
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
            foreach (var o in mc.GetInstances())
            {
                var babeBoard = (ManagementObject) o;
                var val = babeBoard.GetPropertyValue("SerialNumber")?.ToString();
                if (!string.IsNullOrEmpty(val))
                    idBuilder.Append('_').Append(val);
            }

            mc = new ManagementClass("Win32_BIOS");
            foreach (var o in mc.GetInstances())
            {
                var bios = (ManagementObject) o;
                var val = bios.GetPropertyValue("Version")?.ToString();
                if (!string.IsNullOrEmpty(val))
                    idBuilder.Append('_').Append(val);
            }

            mc = new ManagementClass("Win32_DiskDrive");
            foreach (var o in mc.GetInstances())
            {
                var disk = (ManagementObject) o;
                var diskInfo = string.Empty;
                var val = disk.GetPropertyValue("Model")?.ToString();
                if (!string.IsNullOrEmpty(val))
                    diskInfo = '_' + val;
                val = disk.GetPropertyValue("FirmwareRevision")?.ToString();
                if (!string.IsNullOrEmpty(val))
                    diskInfo += '_' + val;
                if (!string.IsNullOrEmpty(diskInfo))
                    idBuilder.Append('_').Append(diskInfo);
            }

            //mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            //foreach (var o in mc.GetInstances())
            //{
            //    var networkAdapter = (ManagementObject) o;
            //    var val = networkAdapter.GetPropertyValue("MACAddress")?.ToString();
            //    if (!string.IsNullOrEmpty(val))
            //        idBuilder.Append('_').Append(val);
            //}

            if (idBuilder.Length > 0)
            {
                if (encrypt)
                {
                    if (CryptoHelper.Encrypt_Astral(idBuilder.ToString().TextToBytes(), publicKey, out var bytes))
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
