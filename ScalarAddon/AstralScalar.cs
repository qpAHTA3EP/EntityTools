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

namespace ScalarAddon
{
    public class AstralScalars : Astral.Addons.Plugin
    {
        private static int _val = 0;
        private static String _name = String.Empty;

        public override string Name => GetType().Name;

        public override string Author => "MichaelProg";

        public override System.Drawing.Image Icon => Properties.Resources.MathOpIcon;

        public override BasePanel Settings => new Forms.ScalarAddonPanel();

        public static Dictionary<String, int> Scalars = new Dictionary<String, int>();

        public static bool GetValue(String name, out int val)
        {
            val = _val;
            return true;
        }
        public static int GetValue(String name)
        {
            if (name == _name)
            {
                return _val;
            }
            else return 0;
        }

        public static bool SetValue(String name, int val)
        {
            _val = val;
            _name = name;
            return true;
        }

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
