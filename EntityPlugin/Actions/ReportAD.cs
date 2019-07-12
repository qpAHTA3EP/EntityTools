using Astral;
using Astral.Logic.Classes.Map;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace QuesterAssistant.Actions
{
    [Serializable]
    public class ReportAD : Astral.Quester.Classes.Action
    {
        public override string ActionLabel => GetType().Name;
        //public override string Category => Core.Category;
        public override bool NeedToRun => true;
        public override string InternalDisplayName => GetType().Name;
        public override bool UseHotSpots => false;
        protected override bool IntenalConditions => true;
        protected override Vector3 InternalDestination => new Vector3();
        protected override ActionValidity InternalValidity
        {
            get
            {
                if (string.IsNullOrEmpty(FilePath) || (FilePath.IndexOfAny(System.IO.Path.GetInvalidPathChars()) != -1))
                    return new ActionValidity("Invalid 'FilePath'. Default will be used");
                if (string.IsNullOrEmpty(FileName) || (FileName.IndexOfAny(System.IO.Path.GetInvalidPathChars()) != -1) || FileName.IndexOf('\\') != -1)
                    return new ActionValidity("Invalid 'FileName'. Default will be used");
                return new ActionValidity();
            }
        }
        public override void GatherInfos() { }
        public override void InternalReset() { }
        public override void OnMapDraw(GraphicsNW graph) { }

        [Description("Directory where file vile be saved\n" +
            "Default is 'Astral\\ReportAD'")]
        public string FilePath { get; set; }

        [Description("Default is '%account%'")]
        public string FileName { get; set; }

        [Description("Output data format allows the following patterns:\n" +
            "%dateTime%: current Date and time;\n" +
            "%account%: current Account name;\n" +
            "%name%: character name;\n" +
            "%AD%: the number of the Astral Diamond;\n" +
            "%rAD%: the number of the rough Astral Diamond")]
        public string DataFormat { get; set; }

        public ReportAD()
        {
            FileName = "%account%";
            FilePath = /*Astral.Controllers.Directories.AstralStartupPath + */".\\ADReports";
            DataFormat = "%dateTime%;%account%;%name%;%AD%;%rAD%";
        }

        public override ActionResult Run()
        {
            string curAccount = EntityManager.LocalPlayer.AccountLoginUsername;
            string curChar = EntityManager.LocalPlayer.Name;
            string filePath, fileName;
            if (string.IsNullOrEmpty(FilePath) || (FilePath.IndexOfAny(System.IO.Path.GetInvalidPathChars()) != -1))
                filePath = Astral.Controllers.Directories.AstralStartupPath + "\\ADReports";
            else filePath = FilePath;
            if (string.IsNullOrEmpty(FileName) || (FileName.IndexOfAny(System.IO.Path.GetInvalidPathChars()) != -1) || FileName.IndexOf('\\') != -1)
                fileName = EntityManager.LocalPlayer.AccountLoginUsername;
            else fileName = FileName.Replace("%account%", EntityManager.LocalPlayer.AccountLoginUsername);

            string fullFilePath = System.IO.Path.Combine(filePath, fileName + ".txt");

            try
            {
                using (StreamWriter fstream = new StreamWriter(fullFilePath, true))
                {
                    string output = DataFormat;
                    output = output.Replace("%dateTime%", DateTime.Now.ToString());
                    output = output.Replace("%account%", curAccount);
                    output = output.Replace("%name%", curChar);
                    output = output.Replace("%AD%", EntityManager.LocalPlayer.Inventory.AstralDiamonds.ToString());
                    output = output.Replace("%rAD%", EntityManager.LocalPlayer.Inventory.AstralDiamondsRough.ToString());

                    fstream.WriteLine(output);
                }
                return ActionResult.Completed;
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Failed to access the file");
                Logger.WriteLine(ex.ToString());
                return ActionResult.Fail;
            }
        }
    }
}
