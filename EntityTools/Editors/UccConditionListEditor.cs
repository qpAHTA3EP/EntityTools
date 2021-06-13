using EntityTools.UCC.Actions;
using EntityTools.UCC.Conditions;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using ConditionList = System.Collections.Generic.List<Astral.Logic.UCC.Classes.UCCCondition>;


namespace EntityTools.Editors
{
#if DEVELOPER
    class UccConditionListEditor : UITypeEditor
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
                        if (EntityTools.Core.UserRequest_EditUccConditions(ref conditions, ref logic, ref negation))
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
                        if (EntityTools.Core.UserRequest_EditUccConditions(ref conditions, ref logic, ref negation))
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
                case ConditionList conditionList when EntityTools.Core.UserRequest_EditUccConditions(ref conditionList):
                    return conditionList;
                case UCCConditionPack conditionPack:
                {
                    var conditions = conditionPack.Conditions;
                    var logic = conditionPack.TestRule;
                    var negation = conditionPack.Not;
                    if (EntityTools.Core.UserRequest_EditUccConditions(ref conditions, ref logic, ref negation))
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
#endif
}
