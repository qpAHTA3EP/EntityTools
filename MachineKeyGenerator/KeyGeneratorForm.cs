using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using Extensions;
using System.Text;
using System.Windows.Forms;

namespace MachineKeyGenerator
{
    public partial class KeyGeneratorForm : Form
    {
#if TEST
        private string machineId = string.Empty;
        private string encrMachineId = string.Empty;
        private string decrMachineId = string.Empty;
        private byte[] machineIdByte = null;
        private byte[] encrMachineIdByte = null;
        private byte[] decrMachineIdByte = null;
#endif
        public KeyGeneratorForm()
        {
            InitializeComponent();
#if TEST
            btnCopy.Text = "Generate";
            btnCopy.Click -= btnCopy_Click;
            btnCopy.Click += btnGenerate_Click;

            btnSave.Text = "Decrypt";
            btnSave.Click -= btnSave_Click;
            btnSave.Click += btnDecrypt_Click;
#endif
        }

        private void KeyGeneratorForm_Load(object sender, EventArgs e)
        {
#if !TEST
            tbId.Text = SysInfo.SysInformer.GetMashineID(true);
#endif
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Close();
            if(!string.IsNullOrEmpty(tbId.Text))
                Clipboard.SetText(tbId.Text);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.AddExtension = true;
            saveDialog.DefaultExt = "key";
            saveDialog.Filter = "Key files (*.key)|*.key";
            saveDialog.InitialDirectory = Environment.CurrentDirectory;
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveDialog.FileName, tbId.Text);
                //System.Diagnostics.Process.Start(saveDialog.FileName);
            }
            Close();
        }
#if TEST
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            machineId = SysInfo.SysInformer.GetMashineID(false);
            tbId.Text = machineId;
            machineIdByte = machineId.TextToBytes();
            encrMachineIdByte = Encryptor.CryptoHelper.Encrypt_Astral(machineIdByte, SysInfo.SysInformer.publicKey);
            if (encrMachineIdByte != null)
            {
                encrMachineId = encrMachineIdByte.ToHexString();
                tbId.Text += string.Concat("\r\n=================================\r\n", encrMachineId);
            }
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(encrMachineId))
            {
                decrMachineIdByte = Encryptor.CryptoHelper.Decrypt_Astral(encrMachineId.HexToBytes(), SysInfo.SysInformer.publicKey);
                if (decrMachineIdByte != null)
                {
                    decrMachineId = decrMachineIdByte.ToNormalString();
                    tbId.Text += string.Concat("\r\n=================================\r\n", decrMachineId);
                }
            }
        }
#endif
    }
}
#if false
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
        /*public static string MD5_Bytes2String(byte[] bytes)
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
        }*/

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

        /*internal static string Decrypt(string data, string cryptKey)
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
        }*/
    }
} 
#endif