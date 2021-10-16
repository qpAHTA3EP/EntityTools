using System;
using System.ComponentModel;
using AcTp0Tools.Classes.Targeting;
using EntityTools.Enums;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace EntityTools.Tools.Targeting
{


    /// <summary>
    /// Целеуказатель на врагов, атакующих члена группы, заданного <see cref="Teammate"/>
    /// </summary>
    [Serializable]
    public class TeammateSupport : TargetSelector
    {
        /// <summary>
        /// Задает члена группы, которому оказывается поддержка
        /// </summary>
        [Description("Team member which will support.\n" +
                     "Sturdiest - the Teammate with the largest maximum HP")]
        [NotifyParentProperty(true)]
        public Teammates Teammate
        {
            get => teammate;
            set 
            {
                teammate = value;
                _label = string.Empty;
                OnPropertyChanged();
            }
        }
        internal Teammates teammate;


#if false
        [NotifyParentProperty(true)]
        public int TeammateMinHP
        {
            get => teammateMinHP;
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > 100)
                    value = 100;
                teammateMinHP = value;
            }
        }
        internal int teammateMinHP = 0; 
#endif

        /// <summary>
        /// Задает принцип выбора противника
        /// </summary>
        [Description("The choosing an enemy targeting algorithm.\n" +
                     "TeammatesTarget - assist selected Teammate (attack Teammate's target).\n" +
                     "Sturdiest - the enemy with the largest maximum HP.\n" +
                     "Weakest - the enemy with the smallest maximum HP\n")]
        [NotifyParentProperty(true)]
        public PreferredFoe FoePreference
        {
            get => foePreference;
            set
            {
                foePreference = value;
                _label = string.Empty;
                OnPropertyChanged();
            }
        }
        internal PreferredFoe foePreference;

        public override TargetSelector Clone()
        {
            return new TeammateSupport
            {
                teammate = teammate,
                foePreference = foePreference
            };
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(_label))
            {
                _label = foePreference == PreferredFoe.TeammatesTarget 
                    ? $"Assist {teammate}" 
                    : $@"Guard {teammate} from {foePreference}";
            }
            return _label;
        }
        private string _label;
    }
}
