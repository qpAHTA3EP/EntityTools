using Extensions;
using System;
using System.IO;
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