using Astral.Forms;
using VariableTools.Expressions;
using System.Collections.Generic;
using VariableTools.Classes;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace VariableTools
{
    public class VariablesTools : Astral.Addons.Plugin
    {
        #region Inherited
        public override string Name => GetType().Name;
        public override string Author => "MichaelProg";
        public override System.Drawing.Image Icon => Properties.Resources.MathOp;
        public override BasePanel Settings => new Forms.VariablesAddonPanel();
        public override void OnBotStart() { }
        public override void OnBotStop() { }
        public override void OnLoad()
        {
            SaveToFile();
        }

        public static void SaveToFile()
        {
            string fullFileName = Path.Combine(Astral.Controllers.Directories.SettingsPath, GetType().Name, "Variables.xml");

            if (!Directory.Exists(Path.GetDirectoryName(fullFileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fullFileName));

            XmlSerializer serialiser = new XmlSerializer(typeof(VariableContainer));
            using (TextWriter FileStream = new StreamWriter(fullFileName))
            {
                using (var var = Variables.GetEnumerator())
                {
                    while (var.MoveNext())
                    {
                        serialiser.Serialize(FileStream, var.Current);
                    }
                }
            }
        }

        public override void OnUnload()
        {
        }
        #endregion

        /// <summary>
        /// Коллекция переменных
        /// </summary>
        //public static Dictionary<string, double> Variables = new Dictionary<string, double>();

        internal static VariableCollection Variables = new VariableCollection();
    }
 }