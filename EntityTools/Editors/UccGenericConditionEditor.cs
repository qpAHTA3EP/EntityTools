using System;
using System.ComponentModel;
using System.Drawing.Design;
using Astral.Quester.Forms;
using EntityTools.Forms;
using EntityTools.UCC.Conditions;

namespace EntityTools.Editors
{
    internal class UccGenericConditionEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context?.Instance is UCCGenericCondition condition)
            {
                switch (condition.Tested)
                {
                    case Astral.Logic.UCC.Ressources.Enums.ActionCond.HasAura:
                    case Astral.Logic.UCC.Ressources.Enums.ActionCond.AuraStacks:
                    {
                        string newVal = AuraViewer.GUIRequest();
                        if (!string.IsNullOrEmpty(newVal))
                        {
                            if (condition.Tested == Astral.Logic.UCC.Ressources.Enums.ActionCond.AuraStacks)
                            {
                                string[] array = condition.Value.Split(':');
                                return array.Length > 1 
                                    ? $"{array[0]}:{newVal}" 
                                    : $"0:{newVal}";
                            }

                            return newVal;
                        }
                        break;
                    }
                    case Astral.Logic.UCC.Ressources.Enums.ActionCond.SpellIsActive:
                    case Astral.Logic.UCC.Ressources.Enums.ActionCond.SpellOnCooldown:
                    {
                        string power = GetAnId.GetPower(true);
                        if (!string.IsNullOrEmpty(power))
                            return power;
                        break;
                    }
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
