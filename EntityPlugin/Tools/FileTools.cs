using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EntityPlugin.Tools
{
    public class FileTools
    {
        /// <summary>
        /// Стандартные Маски-замены для вставки значений в строки
        /// </summary>
        public static readonly string MaskAccount = "%account%";
        public static readonly string MaskCharacter = "%character%";
        public static readonly string MaskDateTime = "%dateTime%";

        public static readonly string MaskAD = "%AD%";
        public static readonly string MaskRAD = "%rAD%";

        public static readonly string defaulExportFolderAuras = Path.Combine(Astral.Controllers.Directories.LogsPath, "Auras");
        public static readonly string defaulExportFolderMissions = Path.Combine(Astral.Controllers.Directories.LogsPath, "Missions");
        public static readonly string defaulFileAuras = "Auras.txt";
        public static readonly string defaulFileMissions = "Missions.txt";

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
                    result = result.Replace(MaskDateTime, DateTime.Now.ToString());
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
