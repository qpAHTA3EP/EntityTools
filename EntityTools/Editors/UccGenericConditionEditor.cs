using System;
using System.ComponentModel;
using System.Drawing.Design;
using Astral.Quester.Forms;
using EntityTools.UCC.Conditions;

namespace EntityTools.Editors
{
#if DEVELOPER
    public class UccGenericConditionEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context?.Instance is UCCGenericCondition condition)
            {
                if (condition.Tested == Astral.Logic.UCC.Ressources.Enums.ActionCond.HasAura ||
                    condition.Tested == Astral.Logic.UCC.Ressources.Enums.ActionCond.AuraStacks)
                {
                    string newVal = string.Empty;
                    if (EntityTools.Core.UserRequest_SelectAuraId(ref newVal))
                    {
                        if (condition.Tested == Astral.Logic.UCC.Ressources.Enums.ActionCond.AuraStacks)
                        {
                            string[] array = condition.Value.Split(':');
                            if (array.Length > 1)
                                return string.Concat(array[0], ':', newVal);
                            return string.Concat("0:", newVal);
                        }

                        return newVal;
                    }
                }
                else if(condition.Tested == Astral.Logic.UCC.Ressources.Enums.ActionCond.SpellIsActive ||
                        condition.Tested == Astral.Logic.UCC.Ressources.Enums.ActionCond.SpellOnCooldown)
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
#endif
}
