using System.ComponentModel;
using System.Drawing.Design;
using Astral.Logic.UCC.Classes;
using EntityTools.Editors;
using MyNW.Internals;

namespace EntityTools.UCC.Conditions
{
    public class UCCGenericCondition : UCCCondition, ICustomUCCCondition
    {
        #region ICustomUCCCondition
        public new bool IsOK(UCCAction refAction/* = null*/)
        {
            if (Target != Astral.Logic.UCC.Ressources.Enums.Unit.Target
                || EntityManager.LocalPlayer.Character.CurrentTarget.IsValid)
                return base.IsOK(refAction);
            return false;
        }

        public new bool Locked { get => base.Locked; set => base.Locked = value; }

        public new ICustomUCCCondition Clone()
        {
            var copy = new UCCGenericCondition
            {
                Sign = Sign,
                Locked = base.Locked,
                Target = Target,
                Tested = Tested,
                Value = Value
            };

            return copy;
        }

#if !DEVELOPER
        [Browsable(false)]
#endif
        public string TestInfos(UCCAction refAction)
        {
            if (Target != Astral.Logic.UCC.Ressources.Enums.Unit.Target
                || EntityManager.LocalPlayer.Character.CurrentTarget.IsValid)
                return $"{Target} {Tested} : {getRefValue(refAction, out ConditionType t)}";
            return "There is no valid Target to test the Condition:\n\r" +
                   $"{GetType().Name}: {Target} {Tested} {base.Value}";
        }
        #endregion

        public override string ToString()
        {
            return $"{GetType().Name}: {Target} {Tested} {base.Value}";
        }

#if DEVELOPER
        [Editor(typeof(UccGenericConditionEditor), typeof(UITypeEditor))]
#else
        [Browsable(false)]
#endif
        public new string Value { get => base.Value; set => base.Value = value; }
    }
}
