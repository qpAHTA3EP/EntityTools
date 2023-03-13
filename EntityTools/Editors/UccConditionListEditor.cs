using System;
using System.ComponentModel;
using System.Drawing.Design;
using EntityTools.Forms;
using EntityTools.UCC.Actions;
using EntityTools.UCC.Conditions;

using ConditionList = System.Collections.ObjectModel.ObservableCollection<Astral.Logic.UCC.Classes.UCCCondition>;

namespace EntityTools.Editors
{
    internal class UccConditionListEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {

            var instance = context?.Instance;
            if (instance != null)
            {
                switch (instance)
                {
                    case UCCConditionPack conditionPack:
                    {
                        var conditions = conditionPack.Conditions;
                        var logic = conditionPack.TestRule;
                        var negation = conditionPack.Not;
                        if (ConditionListForm.UserRequest(ref conditions, ref logic, ref negation))
                        {
                            conditionPack.TestRule = logic;
                            conditionPack.Not = negation;
                            return conditions;
                        }

                        return value;
                    }
                    case SpecializedUCCAction spUccAction:
                    {
                        var conditions = spUccAction.CustomConditions;
                        var logic = spUccAction.CustomConditionCheck;
                        var negation = spUccAction.Not;
                        if (ConditionListForm.UserRequest(ref conditions, ref logic, ref negation))
                        {
                            spUccAction.CustomConditionCheck = logic;
                            spUccAction.Not = negation;
                            return conditions;
                        }

                        return value;
                    }
                }
            }
            switch (value)
            {
                case ConditionList conditionList:
                        return ConditionListForm.UserRequest(conditionList) ?? value;
                case UCCConditionPack conditionPack:
                {
                    var conditions = conditionPack.Conditions;
                    var logic = conditionPack.TestRule;
                    var negation = conditionPack.Not;
                    if (ConditionListForm.UserRequest(ref conditions, ref logic, ref negation))
                    {
                        var newConditionPack = new UCCConditionPack
                        {
                            Conditions = conditions,
                            TestRule = logic,
                            Not = negation
                        };
                        return newConditionPack;
                    }

                    break;
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
