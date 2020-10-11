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

        public static readonly string defaultExportFolderStates = Path.Combine(Directories.LogsPath, "States");
        public static readonly string defaultExportFolderInterfaces = Path.Combine(Directories.LogsPath, "Interfaces");
        public static readonly string defaultExportFolderAuras = Path.Combine(Directories.LogsPath, "Auras");
        public static readonly string defaultExportFolderMissions = Path.Combine(Directories.LogsPath, "Missions");
        public static readonly string defaultFileStates = "States.txt";
        public static readonly string defaultFileInterfaces = "Interfaces.xml";
        public static readonly string defaultFileAuras = "Auras.xml";
        public static readonly string defaultFileMissions = "Missions.xml";
        public static readonly string defaultExportFileName = "%character%_%account%_%datetime%.xml";

        public static readonly string SettingsFile = Path.Combine(Directories.SettingsPath, nameof(EntityTools), nameof(EntityTools) + ".xml");
        //public static bool ReplaceMask(ref string str)
        //{
        //    if (!string.IsNullOrEmpty(str))
        //    {
        //        string result = str.Replace(MaskAccount, EntityManager.LocalPlayer?.AccountLoginUsername);
        //        result = result.Replace(MaskCharacter, EntityManager.LocalPlayer?.Name);
        //        result = result.Replace(MaskDateTime, DateTime.Now.ToString());
        //        result = result.Replace(MaskAD, EntityManager.LocalPlayer.Inventory.AstralDiamonds.ToString());
        //        result = result.Replace(MaskRAD, EntityManager.LocalPlayer.Inventory.AstralDiamondsRough.ToString());

        //        if (str != result)
        //        {
        //            str = result;
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        public static string ReplaceMask(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                if (str.IndexOf('%') != -1)
                {
                    string result = str.Replace(MaskAccount, EntityManager.LocalPlayer?.AccountLoginUsername);
                    result = result.Replace(MaskCharacter, EntityManager.LocalPlayer?.Name);
                    result = result.Replace(MaskDateTime, $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}_{DateTime.Now.Hour}-{DateTime.Now.Minute}-{DateTime.Now.Second}");
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
