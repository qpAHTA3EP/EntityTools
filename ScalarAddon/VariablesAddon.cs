using System;
using System.Collections.Generic;
using Astral.Forms;
using MyNW.Classes;
using System.ComponentModel;
using Astral.Logic.Classes.Map;
using Astral.Quester.Classes;
using System.Drawing.Design;
using Astral.Quester.Forms;
using Astral.Quester.UIEditors;
using AstralVariables.Forms;
using AstralVariables;

namespace AstralVariables
{
    public class VariablesAddon : Astral.Addons.Plugin
    {
        //public static VarCollection Variables = new VarCollection();

        public static Dictionary<string, double> Variables = new Dictionary<string, double>();

        //public static string LoggerPredicate = nameof(VariablesAddon) + ':';

        public override string Name => "Variable Tools";

        public override string Author => "MichaelProg";

        public override System.Drawing.Image Icon => Properties.Resources.MathOp;

        public override BasePanel Settings => new Forms.VariablesAddonPanel();

       public override void OnBotStart()
        {
            
        }

        public override void OnBotStop()
        {
            
        }

        public override void OnLoad()
        {
            
        }

        public override void OnUnload()
        {
            
        }
    }
 }