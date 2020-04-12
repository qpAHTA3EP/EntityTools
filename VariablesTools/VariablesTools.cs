using Astral.Forms;
using VariableTools.Expressions;
using System.Collections.Generic;
using VariableTools.Classes;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Astral;
using System;
using MyNW.Internals;
using System.Reflection;

namespace VariableTools
{
    public class VariableTools : Astral.Addons.Plugin
    {
        #region Inherited
        public override string Name => GetType().Name;
        public override string Author => "MichaelProg";
        public override System.Drawing.Image Icon => Properties.Resources.MathOp;
        public override BasePanel Settings => new Forms.ExtendedVariablesToolsPanel();//new Forms.VariablesAddonPanel();
        public override void OnBotStart() { }
        public override void OnBotStop() { }
        public override void OnLoad()
        {
            LoadFromFile();
        }
        public override void OnUnload()
        {
            SaveToFile();
        }
        #endregion

        public static bool DebugMessage { get; set; }

        public static void SaveToFile()
        {
            string fullFileName = Path.Combine(Astral.Controllers.Directories.SettingsPath, nameof(VariableTools), "Variables.xml");

            if (!Directory.Exists(Path.GetDirectoryName(fullFileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fullFileName));

            if (File.Exists(fullFileName))
            {
                if (File.Exists(fullFileName + ".bak"))
                    File.Delete(fullFileName + ".bak");
                File.Move(fullFileName, fullFileName + ".bak");
            }

            try
            {
                XmlSerializer serialiser = new XmlSerializer(typeof(VariableCollection));
                using (TextWriter file = new StreamWriter(fullFileName))
                {
                    serialiser.Serialize(file, Variables);
                }
                Logger.WriteLine(Logger.LogType.Debug, $"{nameof(VariableTools)}: Variables saved to the file \"{fullFileName}\".");

            }
            catch (Exception e)
            {
                if (File.Exists(fullFileName))
                    File.Delete(fullFileName);
                if(File.Exists(fullFileName + ".bak"))
                    File.Move(fullFileName + ".bak", fullFileName);
                Logger.WriteLine(Logger.LogType.Debug, $"{nameof(VariableTools)}: Failed to save variables to the file \"{fullFileName}\".");
                Logger.WriteLine(Logger.LogType.Debug, e.Message);
            }
        }

        public static void LoadFromFile()
        {
            string fullFileName = Path.Combine(Astral.Controllers.Directories.SettingsPath, nameof(VariableTools), "Variables.xml");

            if (!Directory.Exists(Path.GetDirectoryName(fullFileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fullFileName));
            try
            {
                if (File.Exists(fullFileName))
                {
                    XmlSerializer serialiser = new XmlSerializer(typeof(VariableCollection));
                    using (StreamReader file = new StreamReader(fullFileName))
                    {
                        if (serialiser.Deserialize(file) is VariableCollection vars)
                        {
                            Variables = vars;
                            Logger.WriteLine(Logger.LogType.Debug, $"{nameof(VariableTools)}: Load {Variables.Count} variables from file \"{fullFileName}\".");
                        }
                        else Logger.WriteLine(Logger.LogType.Debug, $"{nameof(VariableTools)}: No variables loaded from file \"{fullFileName}\".");
                    }
                }
                else Logger.WriteLine(Logger.LogType.Debug, $"Unable to load variables because no file found.");
            }
            catch(Exception e)
            {
                Logger.WriteLine(Logger.LogType.Debug, $"{nameof(VariableTools)}: Failed to load variables from the file \"{fullFileName}\".");
                Logger.WriteLine(Logger.LogType.Debug, e.Message);
            }
        }

        /// <summary>
        /// Коллекция переменных
        /// </summary>
        //public static Dictionary<string, double> Variables = new Dictionary<string, double>();
        internal static VariableCollection Variables = new VariableCollection();

        internal static StringBuilder sb = new StringBuilder();
        public static string GetScopeQualifier(AccountScopeType asq, ProfileScopeType psq)
        {
            sb.Clear();
            // Подстрока с относительным путем к профилю
            if (psq == ProfileScopeType.Profile)
            {
                bool hasExtention = Astral.API.CurrentSettings.LastQuesterProfile.EndsWith(@".amp.zip");

                if (Astral.API.CurrentSettings.LastQuesterProfile.StartsWith(Astral.Controllers.Directories.ProfilesPath))
                {
                    // Length + 1 нужно чтобы удалить символ '\' перед именем профиля
                    sb.Append(Astral.API.CurrentSettings.LastQuesterProfile.Substring(Astral.Controllers.Directories.ProfilesPath.Length + 1,
                        Astral.API.CurrentSettings.LastQuesterProfile.Length - Astral.Controllers.Directories.ProfilesPath.Length - (hasExtention ? 9 : 1)));
                }
                else if (Astral.API.CurrentSettings.LastQuesterProfile.StartsWith(@".\Profiles\", StringComparison.OrdinalIgnoreCase))
                    sb.Append(Astral.API.CurrentSettings.LastQuesterProfile.Substring(11, Astral.API.CurrentSettings.LastQuesterProfile.Length - Astral.Controllers.Directories.ProfilesPath.Length - (hasExtention ? 9 : 1)));
                //if (Astral.API.CurrentSettings.LastQuesterProfile.StartsWith(Astral.Controllers.Directories.AstralStartupPath))
                //    sb.Append(Astral.API.CurrentSettings.LastQuesterProfile.Replace(Astral.Controllers.Directories.AstralStartupPath, @"."));
                else sb.Append(Astral.API.CurrentSettings.LastQuesterProfile);
                if (asq != AccountScopeType.Global)
                    sb.Append("&&");
            }


            if (asq == AccountScopeType.Character)
            {
                //if (EntityManager.LocalPlayer.IsValid)
                if(!string.IsNullOrEmpty(EntityManager.LocalPlayer.InternalName))
                    sb.Append(EntityManager.LocalPlayer.InternalName);
                else if (GetLastLoggedCharacter(out string charName)
                         && GetLastLoggedAccount(out string accName))
                {
                    sb.Append(charName).Append(accName);
                    Logger.WriteLine(Logger.LogType.Debug, "ScopeQualifier constracted with LastPlayedCharacter because of character is not in the Game!");
                }
                else
                {
                    //throw new InvalidOperationException("Unable to set AccountScope to 'Character' because of character is not in the Game!");
                    Logger.WriteLine(Logger.LogType.Debug, "Unable to constract ScopeQualifier with scope 'Character' because of character is not in the Game!");
                    return string.Empty;
                }
            }
            else if (asq == AccountScopeType.Account)
            { 
                if (!string.IsNullOrEmpty(EntityManager.LocalPlayer.AccountLoginUsername))
                    sb.Append(EntityManager.LocalPlayer.AccountLoginUsername);
                else if (GetLastLoggedAccount(out string account))
                {
                    sb.Append(account);
                    Logger.WriteLine(Logger.LogType.Debug, "ScopeQualifier constracted with LastPlayedAccount because the Game is not logged in!");
                }
                else
                {
                    //throw new InvalidOperationException("Unable to set AccountScope to 'Account' because of character is not in the Game!");
                    Logger.WriteLine(Logger.LogType.Debug, "Unable to constract ScopeQualifier with scope 'Account' because the Game is not logged in!");
                    return string.Empty;
                }
            }
            return sb.ToString();
        }

        #region Accesse to Relogger
        internal static FieldInfo charFiledInfo;
        internal static FieldInfo accFieldInfo;
        /// <summary>
        /// Считывание имени последнего загруженного персонажа из Astral.Controler.Relogger
        /// </summary>
        /// <param name="charName"></param>
        /// <returns></returns>
        private static bool GetLastLoggedCharacter(out string charName)
        {
            charName = string.Empty;
            if(charFiledInfo == null)
                charFiledInfo = typeof(Astral.Controllers.Relogger).GetField("lastCharName", BindingFlags.NonPublic | BindingFlags.Static);
            if (charFiledInfo != null)
            {
                charName = charFiledInfo.GetValue(typeof(string)) as string;
                return !string.IsNullOrEmpty(charName);
            }
            return false;
        }
        /// <summary>
        /// Считывание последнего загруженного аккаунта из Astral.Controler.Relogger
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        private static bool GetLastLoggedAccount(out string account)
        {
            account = string.Empty;
            if(accFieldInfo == null)
                accFieldInfo = typeof(Astral.Controllers.Relogger).GetField("lastAccountName", BindingFlags.NonPublic | BindingFlags.Static);
            if (accFieldInfo != null)
            {
                account = accFieldInfo.GetValue(typeof(string)) as string;
                return !string.IsNullOrEmpty(account);
            }
            return false;
        }
        #endregion
    }
}