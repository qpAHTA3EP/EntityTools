using Astral.Logic.UCC.Classes;
using Astral.Quester.UIEditors;
using EntityTools.Core.Interfaces;
using EntityTools.Core.Proxies;
using EntityTools.Enums;
using MyNW.Classes;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.Serialization;
using EntityTools.Editors;
using EntityTools.UCC.Conditions;
using Unit = Astral.Logic.UCC.Ressources.Enums.Unit;

namespace EntityTools.UCC.Actions
{
    [Serializable]
    public class PluggedSkill : UCCAction
    {
        #region Опции команды
#if DEVELOPER
        [Editor(typeof(PowerAllIdEditor), typeof(UITypeEditor))]
        [Category("Required")]
#else
        [Browsable(false)]
#endif
        public PluggedSkillSource Source
        {
            get => _source;
            set
            {
                if (_source != value)
                {
                    _source = value;
                    NotifyPropertyChanged();
                }
            }
        }
        internal PluggedSkillSource _source = PluggedSkillSource.Mount;

#if CUSTOM_UCC_CONDITION_EDITOR
#if DEVELOPER
        [Category("Optional")]
        [Editor(typeof(UccConditionListEditor), typeof(UITypeEditor))]
        [Description("Набор нестандартных UCC-условий, которые проверяют после основных")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Browsable(false)]
#else
        [Browsable(false)]
#endif        
        public UCCConditionPack CustomConditions
        {
            get => _customConditions;
            set
            {
                if (ReferenceEquals(_customConditions, value))
                    return;

                _customConditions = value;
                if (value != null)
                    Conditions.Add(value);
                NotifyPropertyChanged();
            }
        }
        internal UCCConditionPack _customConditions = new UCCConditionPack();
#else
        [Browsable(false)]
        public UCCConditionPack CustomConditions
        {
            get => null;
            set
            {
                if(value != null)
                    Conditions.Add(value);
            }
        }
        public bool ShouldSerializeCustomConditions() => false;
#endif

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

        public PluggedSkill()
        {
            CoolDown = 60_000;
            Engine = MakeProxy();
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
            return BaseClone(new PluggedSkill
            {
                _source = _source,
                //_powerId = _powerId,
                //_specificTarget = _specificTarget,
                //_checkPowerCooldown = _checkPowerCooldown,
                //_checkInTray = _checkInTray,
                //_castingTime = _castingTime,
                //_forceMaintain = _forceMaintain
                
            });
        }
    }
}
