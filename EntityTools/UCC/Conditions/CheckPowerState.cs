using Astral.Logic.UCC.Classes;
using EntityTools.Core.Interfaces;
using EntityTools.Core.Proxies;
using EntityTools.Editors;
using EntityTools.Enums;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.Serialization;

namespace EntityTools.UCC.Conditions
{
    public class CheckPowerState : UCCCondition, ICustomUCCCondition
    {
        #region Опции
#if DEVELOPER
        [Editor(typeof(PowerIdEditor), typeof(UITypeEditor))]
        [Category("Required")]
#else
        [Browsable(false)]
#endif
        public string PowerId
        {
            get => _powerId;
            set
            {
                if (_powerId != value)
                {
                    _powerId = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _powerId = string.Empty;

        [Category("Required")]
        public PowerState CheckState
        {
            get => checkState;
            set
            {
                if (checkState != value)
                {
                    checkState = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private PowerState checkState;


        #region Hide Inherited Properties
        [XmlIgnore]
        [Browsable(false)]
        public new string Value { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public new Astral.Logic.UCC.Ressources.Enums.Unit Target { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public new Astral.Logic.UCC.Ressources.Enums.ActionCond Tested { get; set; }
        #endregion
        #endregion


        #region ICustomUCCCondition
        bool ICustomUCCCondition.IsOK(UCCAction refAction) =>
            LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).IsOK(refAction);

        bool ICustomUCCCondition.Locked { get => base.Locked; set => base.Locked = value; }

        ICustomUCCCondition ICustomUCCCondition.Clone()
        {
            return new CheckPowerState
            {
                _powerId = _powerId,
                checkState = checkState,
                Sign = Sign,
                Locked = Locked,
                Target = Target,
                Value = Value
            };
        }

        string ICustomUCCCondition.TestInfos(UCCAction refAction) =>
            LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).TestInfos(refAction);
        #endregion


        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        [NonSerialized]
        internal IUccConditionEngine Engine;

        public CheckPowerState()
        {
            Target = Astral.Logic.UCC.Ressources.Enums.Unit.Target;
            Engine = new UccConditionProxy(this);
        }
        private IUccConditionEngine MakeProxy()
        {
            return new UccConditionProxy(this);
        }

        public override string ToString() => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).Label();
    }
}
