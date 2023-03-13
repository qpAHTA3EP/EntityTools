using ACTP0Tools;
using ACTP0Tools.Reflection;
using Astral.Functions;
using DevExpress.XtraEditors;
using Microsoft.Win32;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EntityTools.Forms
{
    public partial class CredentialManager2 : XtraForm
    {
        private Credentials credentials;

        public CredentialManager2()
        {
            InitializeComponent();
        }

        private void handler_Load(object sender, EventArgs e)
        {
            var creds = AstralAccessors.General.Credentials;
			if(creds is null)
				return;
            credentials = CopyHelper.CreateDeepCopy(creds);

            bindingSource.DataSource = creds.GameAccounts;

            listAccounts.DataSource = bindingSource;
            listAccounts.DisplayMember = nameof(CredAccount.CachedAccountName);
            listAccounts.SelectedIndex = 0;

            handler_SelectedAccountChanged(this);
        }

        private void handler_SelectedAccountChanged(object sender, EventArgs e = null)
        {
            if (listAccounts.SelectedItem is CredAccount account)
            {
                listCharacters.DataSource = account.CachedCharacters;
                listCharacters.SelectedIndex = 0;
                tbLogin.Text = account.Login;
                tbPassword.Text = account.Password;
                tbMachineId.Text = string.Empty;
            }
            else
            {
                listCharacters.DataSource = null;
                tbLogin.Text = string.Empty;
                tbPassword.Text = string.Empty;
                tbMachineId.Text = string.Empty;
            }
        }

        private void handler_ImportLogin(object sender, EventArgs e)
        {
            
        }

        private void btnAccountImport_Click(object sender, EventArgs e)
        {
            var player = EntityManager.LocalPlayer;
            string accountName = string.Empty;
            if (player.IsValid)
                accountName = player.AccountLoginUsername;
            else
            {
                var gameData = Game.CharacterSelectionData;
                if (gameData.IsValid)
                    accountName = gameData.PublicAccountName;
            }

            if (string.IsNullOrEmpty(accountName))
            {
                XtraMessageBox.Show("Impossible to game account name !");
                return;
            }

            var currentAccount = credentials.GameAccounts.FirstOrDefault(cred => cred.CachedAccountName == accountName);
            if (currentAccount is null)
            {
                currentAccount = new CredAccount { CachedAccountName = accountName };

                credentials.GameAccounts.Add(currentAccount);
                bindingSource.DataSource = credentials.GameAccounts;
            }
            else XtraMessageBox.Show($"Current account '@{accountName}' already is in the list!");
            listAccounts.SelectedItem = currentAccount;
        }

        private void handler_ImportMachineId(object sender, EventArgs e)
        {
            using (var crypticCoreKey = Registry.CurrentUser.OpenSubKey(@"Software\Cryptic\Core"))
            {
                if (crypticCoreKey != null)
                {
                    var machineId = crypticCoreKey.GetValue("machineId");
                    tbMachineId.Text = machineId.ToString();
                }
            }
        }

        private void handler_ChangePasswordVisibility(object sender, EventArgs e)
        {
            tbPassword.Properties.UseSystemPasswordChar = !tbPassword.Properties.UseSystemPasswordChar;
        }

        private void handler_ImportCharacters(object sender, EventArgs e)
        {
            var account = bindingSource.Current as CredAccount;
            if(account is null)
                return;
            
#if true
            var ingameCharacters 
                        = from character in Game.CharacterSelectionData.CharacterChoices.Characters
                          //where !character.UGCEditAllowed
                          select character.CharacterName;

            List<string> characters = new List<string>(ingameCharacters);
            if (characters.Count == 0)
            {
                XtraMessageBox.Show("List is empty, go to character selection list before !");
                return;
            }

            Func<IEnumerable<string>> source = () => characters;
#else
            Func<IEnumerable<string>> source = () => from character in Game.CharacterSelectionData.CharacterChoices.Characters
                                                     where !character.UGCEditAllowed
                                                     select character.CharacterName; 
#endif
            IList<string> selectedItems = new List<string>(characters.Count);
            if (EntityTools.Core.UserRequest_SelectItemList(source, ref selectedItems))
            {
                var cachedCharacters = account.CachedCharacters;
                cachedCharacters.AddRange(from character in selectedItems
                                          where !cachedCharacters.Contains(character)
                                          select character);
            }
        }
    }
}