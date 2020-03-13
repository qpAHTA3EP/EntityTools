using System;
using System.ComponentModel;
using System.Drawing.Design;
using Astral.Quester.Forms;
using EntityTools.UCC.Conditions;
using ActionCond = Astral.Logic.UCC.Ressources.Enums.ActionCond;

namespace EntityTools.Editors
{
    public class UccGenericConditionEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context.Instance is UCCGenericCondition condition)
            {
                if(condition.Tested == ActionCond.HasAura
                    || condition.Tested == ActionCond.AuraStacks)
                {
                    string newVal = string.Empty;
                    if (EntityTools.Core.GUIRequest_AuraId(ref newVal))
                    {
                        if (condition.Tested == ActionCond.AuraStacks)
                        {
                            string[] array = condition.Value.Split(new char[] { ':' });
                            if (array.Length > 1)
                                return string.Concat(array[0], ':', newVal);
                            else return string.Concat("0:", newVal);
                        }
                        else return newVal;
                    }
                }
                else if(condition.Tested == ActionCond.SpellIsActive
                        || condition.Tested == ActionCond.SpellOnCooldown)
                {
                    string power = GetAnId.GetPower(true);
                    if (!string.IsNullOrEmpty(power))
                        return power;
                }
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
