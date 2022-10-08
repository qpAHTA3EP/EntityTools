using System;
using System.IO;
using Astral.Controllers;
using MyNW.Internals;

namespace EntityTools.Tools
{
    public static class FileTools
    {
        /// <summary>
        /// Стандартные Маски-замены для вставки значений в строки
        /// </summary>
        public static readonly string MaskAccount = "%account%";
        public static readonly string MaskCharacter = "%character%";
        public static readonly string MaskDateTime = "%dateTime%";

        public static readonly string MaskAD = "%AD%";
        public static readonly string MaskRAD = "%rAD%";

        public static readonly string DefaultExportFolderStates = Path.Combine(Directories.LogsPath, "States");
        public static readonly string DefaultExportFolderInterfaces = Path.Combine(Directories.LogsPath, "Interfaces");
        public static readonly string DefaultExportFolderAuras = Path.Combine(Directories.LogsPath, "Auras");
        public static readonly string DefaultExportFolderMissions = Path.Combine(Directories.LogsPath, "Missions");
        public static readonly string DefaultFileStates = "States.txt";
        public static readonly string DefaultFileInterfaces = "Interfaces.xml";
        public static readonly string DefaultFileAuras = "Auras.xml";
        public static readonly string DefaultFileMissions = "Missions.xml";
        public static readonly string DefaultExportFileName = "%character%_%account%_%dateTime%.xml";

        public static readonly string SettingsFile = Path.Combine(Directories.SettingsPath, nameof(EntityTools), nameof(EntityTools) + ".xml");
        public static readonly string UccEditorSettingsFile = Path.Combine(Directories.SettingsPath, nameof(EntityTools), "UccEditor.xml");
        public static readonly string QuesterEditorSettingsFile = Path.Combine(Directories.SettingsPath, nameof(EntityTools), "QuesterEditor.xml");

        public static string ReplaceMask(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                if (str.IndexOf('%') != -1)
                {
                    string result = str.Replace(MaskAccount, EntityManager.LocalPlayer.AccountLoginUsername);
                    result = result.Replace(MaskCharacter, EntityManager.LocalPlayer.Name);
                    result = result.Replace(MaskDateTime, DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss"));
                    result = result.Replace(MaskAD, EntityManager.LocalPlayer.Inventory.AstralDiamonds.ToString());
                    result = result.Replace(MaskRAD, EntityManager.LocalPlayer.Inventory.AstralDiamondsRough.ToString());

                    return result;
                }
                return str;
            }
            return string.Empty;
        }
    }
}
