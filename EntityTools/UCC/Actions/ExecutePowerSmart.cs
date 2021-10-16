#if DEBUG
#define DEBUG_ExecuteSpecificPower
#endif
#define REFLECTION_ACCESS

using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.Serialization;
using Astral.Classes.ItemFilter;
using Astral.Logic.UCC.Classes;
using Astral.Quester.UIEditors;
using EntityTools.Core.Interfaces;
using EntityTools.Core.Proxies;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Tools;
using MyNW.Classes;
using Unit = Astral.Logic.UCC.Ressources.Enums.Unit;

namespace EntityTools.UCC.Actions
{
    [Serializable]
    public class ExecutePowerSmart : UCCAction
    {
        #region Опции команды
#if DEVELOPER
        [Editor(typeof(PowerAllIdEditor), typeof(UITypeEditor))]
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
        internal string _powerId = string.Empty;
#if DEVELOPER
        [Category("Required")]
#else
        [Browsable(false)]
#endif
        public ItemFilterStringType PowerIdType
        {
            get => _idType;
            set
            {
                if (_idType != value)
                {
                    _idType = value;
                    NotifyPropertyChanged();
                }
            }
        }
        internal ItemFilterStringType _idType = ItemFilterStringType.Simple;

#if DEVELOPER
        [Category("Required")]
#else
        [Browsable(false)]
#endif
        public UccTarget SpecificTarget
        {
            get => _specificTarget;
            set
            {
                if (_specificTarget != value)
                {
                    _specificTarget = value;
                    NotifyPropertyChanged();
                }
            }
        }

        internal UccTarget _specificTarget = UccTarget.Auto;

#if DEVELOPER
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
        public bool CheckPowerCooldown
        {
            get => _checkPowerCooldown;
            set
            {
                if (_checkPowerCooldown != value)
                {
                    _checkPowerCooldown = value;
                    NotifyPropertyChanged();
                }
            }
        }
        internal bool _checkPowerCooldown;

#if DEVELOPER
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
        public bool CheckInTray
        {
            get => _checkInTray;
            set
            {
                if (_checkInTray != value)
                {
                    _checkInTray = value;
                    NotifyPropertyChanged();
                }
            }
        }
        internal bool _checkInTray;

#if DEVELOPER
        [Category("Optional")]
        [DisplayName("CastingTime (ms)")]
#else
        [Browsable(false)]
#endif
        public int CastingTime
        {
            get => _castingTime;
            set
            {
                if (_castingTime != value)
                {
                    _castingTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        internal int _castingTime;

#if DEVELOPER
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
        public bool ForceMaintain
        {
            get => _forceMaintain;
            set
            {
                if (_forceMaintain != value)
                {
                    _forceMaintain = value;
                    NotifyPropertyChanged();
                }
            }
        }
        internal bool _forceMaintain;


        #region Hide Inherited Properties
        [XmlIgnore]
        [Browsable(false)]
        public new string ActionName { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public new Unit Target { get => base.Target; set => base.Target = value; }

        #endregion
        #endregion

        #region Взаимодействие с EntityToolsCore
        [NonSerialized]
        internal IUccActionEngine Engine;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            //Engine.OnPropertyChanged(this, propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ExecutePowerSmart()
        {
            Engine = new UccActionProxy(this);
        }
        private IUccActionEngine MakeProxy()
        {
            return new UccActionProxy(this);
        }
        #endregion

        #region Интерфейс команды
        public override bool NeedToRun => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).NeedToRun;
        public override bool Run() => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).Run();
        [XmlIgnore]
        [Browsable(false)]
        public Entity UnitRef => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).UnitRef;
        public override string ToString() => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).Label();
        #endregion

        public override UCCAction Clone()
        {
            return BaseClone(new ExecutePowerSmart
            {
                _powerId = _powerId,
                _specificTarget = _specificTarget,
                _checkPowerCooldown = _checkPowerCooldown,
                _checkInTray = _checkInTray,
                _castingTime = _castingTime,
                _forceMaintain = _forceMaintain
            });
        }
    }
}
