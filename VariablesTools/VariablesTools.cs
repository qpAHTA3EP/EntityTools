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
                        VariableCollection vars = serialiser.Deserialize(file) as VariableCollection;
                        if (vars != null)
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
        public static string GetScopeQualifier(AccountScopeType asq, bool psq)
        {
            sb.Clear();
            // Подстрока с относительным путем к профилю
            if (psq)
            {
                if (Astral.API.CurrentSettings.LastQuesterProfile.IndexOf(Astral.Controllers.Directories.ProfilesPath) >= 0
                    && Astral.Controllers.Directories.ProfilesPath.Length < Astral.API.CurrentSettings.LastQuesterProfile.Length)
                        sb.Append(Astral.API.CurrentSettings.LastQuesterProfile.Substring(Astral.Controllers.Directories.ProfilesPath.Length));
                else sb.Append(Astral.API.CurrentSettings.LastQuesterProfile);
                if (asq != AccountScopeType.Global)
                    sb.Append("&&");
            }

            
            switch (asq)
            {
                case AccountScopeType.Character:
                    if (EntityManager.LocalPlayer.IsValid)
                        sb.Append(EntityManager.LocalPlayer.InternalName);
                    else if(GetLastLoggedCharacter(out string charName))
                        sb.Append(charName);
                    else throw new InvalidOperationException("Unable to set AccountScope to 'Character' because of character is not in the Game!");
                    break;
                case AccountScopeType.Account:
                    if (!string.IsNullOrEmpty(EntityManager.LocalPlayer.AccountLoginUsername))
                        sb.Append(EntityManager.LocalPlayer.AccountLoginUsername);
                    else if (GetLastLoggedAccount(out string account))
                        sb.Append(account);
                    else throw new InvalidOperationException("Unable to set AccountScope to 'Account' because of character is not in the Game!");
                    break;
                //case AccountScopeType.Global:
                //    return profQualif;
                //default:
                //    return profQualif;
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
        internal static bool GetLastLoggedCharacter(out string charName)
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
        internal static bool GetLastLoggedAccount(out string account)
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