using AcTp0Tools.Classes.UCC;
using EntityTools.Editors;
using EntityTools.Enums;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;

// ReSharper disable UnusedMember.Global

namespace EntityTools.UCC.Actions.TargetSelectors
{


    /// <summary>
    /// Целеуказатель на врагов, атакующих члена группы, заданного <see cref="Teammate"/>
    /// </summary>
    [Serializable]
    public class TeammateSupport : UccTargetSelector
    {
        /// <summary>
        /// Задает члена группы, которому оказывается поддержка
        /// </summary>
        [Description("Team member which will support.\n" +
                     "Sturdiest - the Teammate with the largest maximum HP")]
        public Teammates Teammate
        {
            get => _teammate;
            set 
            {
                _teammate = value;
                _label = string.Empty;
                OnPropertyChanged();
            }
        }
        internal Teammates _teammate;

        /// <summary>
        /// Задает принцип выбора противника
        /// </summary>
        [Description("The principle of enemy targeting.\n" +
                     "TeammatesTarget - assist selected Teammate (attack Teammate's target.\n" +
                     "Sturdiest - the enemy with the largest maximum HP")]
        public PreferredFoe FoePreference
        {
            get => _foePreference;
            set
            {
                _foePreference = value;
                _label = string.Empty;
                OnPropertyChanged();
            }
        }
        internal PreferredFoe _foePreference;

        public override UccTargetSelector Clone()
        {
            return new TeammateSupport();
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(_label))
            {
                if (_foePreference == PreferredFoe.TeammatesTarget)
                    _label = $"Assist {_teammate}";
                else
                {
                    _label = $@"Guard {_teammate} from {_foePreference}";
                }
            }
            return _label;
        }
        private string _label;
    }
}
