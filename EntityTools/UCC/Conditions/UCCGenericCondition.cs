using Astral.Logic.UCC.Classes;
using Astral.Logic.UCC.Ressources;
using System.ComponentModel;
using System.Xml.Serialization;
using Astral.Quester.Classes;
using EntityTools.Editors;
using System.Drawing.Design;
using System;

namespace EntityTools.UCC.Conditions
{
    public class UCCGenericCondition : UCCCondition, ICustomUCCCondition
    {
        #region ICustomUCCCondition
        bool ICustomUCCCondition.IsOK(UCCAction refAction/* = null*/)
        {
            return base.IsOK(refAction);
        }

        bool ICustomUCCCondition.Loked { get => base.Locked; set => base.Locked = value; }

        string ICustomUCCCondition.TestInfos(UCCAction refAction)
        {
            return $"{base.Target} {base.Tested} : {base.getRefValue(refAction, out ConditionType t).ToString()}";
        }
        #endregion

        public override string ToString()
        {
            return $"{GetType().Name}: {base.Target} {base.Tested} {base.Value}";
        }

        [Editor(typeof(UccGenericConditionEditor), typeof(UITypeEditor))]
        public new string Value { get => base.Value; set => base.Value = value; }
    }
}
