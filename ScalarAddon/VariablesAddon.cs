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
using AstralVars.Classes;

namespace AstralVars
{
    public class VariablesAddon : Astral.Addons.Plugin
    {
        public static VariableCollection Variables = new VariableCollection();

        public static string LoggerPredicate = "[VariablesAddon]:";

        public override string Name => GetType().Name;

        public override string Author => "MichaelProg";

        public override System.Drawing.Image Icon => Properties.Resources.MathOpIcon;

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


namespace Astral.Quester.UIEditors
{
    internal class ScalarNamesEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            String varName = "TestVarName";
            if (!string.IsNullOrEmpty(varName))
            {
                return varName;
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
