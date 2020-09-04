using Astral.Logic.UCC.Classes;
using Astral.Logic.UCC.Ressources;
using System.ComponentModel;
using System.Xml.Serialization;
using Astral.Quester.Classes;
using EntityTools.Editors;
using System.Drawing.Design;
using System;
using MyNW.Internals;

namespace EntityTools.UCC.Conditions
{
    public class UCCGenericCondition : UCCCondition, ICustomUCCCondition
    {
        #region ICustomUCCCondition
        bool ICustomUCCCondition.IsOK(UCCAction refAction/* = null*/)
        {
            if (base.Target != Astral.Logic.UCC.Ressources.Enums.Unit.Target
                || EntityManager.LocalPlayer.Character.CurrentTarget.IsValid)
                return base.IsOK(refAction);
            else return false;
        }

        bool ICustomUCCCondition.Loked { get => base.Locked; set => base.Locked = value; }

#if !DEVELOPER
        [Browsable(false)]
#endif
        string ICustomUCCCondition.TestInfos(UCCAction refAction)
        {
            if (base.Target != Astral.Logic.UCC.Ressources.Enums.Unit.Target
                || EntityManager.LocalPlayer.Character.CurrentTarget.IsValid)
                return $"{base.Target} {base.Tested} : {base.getRefValue(refAction, out ConditionType t).ToString()}";
            else return $"There is no valid Target to test the Condition:\n\r" +
                    $"{GetType().Name}: {base.Target} {base.Tested} {base.Value}";
        }
        #endregion

        public override string ToString()
        {
            return $"{GetType().Name}: {base.Target} {base.Tested} {base.Value}";
        }

#if DEVELOPER
        [Editor(typeof(UccGenericConditionEditor), typeof(UITypeEditor))]
#else
        [Browsable(false)]
#endif
        public new string Value { get => base.Value; set => base.Value = value; }
    }
}
