#define DEBUG_CHANGE_TARGET

using Astral.Logic.UCC.Classes;
using EntityTools.Core.Interfaces;
using EntityTools.Core.Proxies;
using MyNW.Classes;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.Serialization;
using AcTp0Tools.Annotations;
using EntityTools.Editors;
using EntityTools.Tools.Targeting;
using EntityTools.UCC.Conditions;

namespace EntityTools.UCC.Actions
{
    [Serializable]
    public class ChangeTarget : UCCAction, INotifyPropertyChanged
    {
        #region Опции команды
#if DEVELOPER
        [Category("General")]
        [Editor(typeof(UccTargetSelectorEditor), typeof(UITypeEditor))]
        [Description("Target selector algorithm.")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [XmlElement(typeof(EntityTarget))]
        [XmlElement(typeof(TeammateSupport))]
#else
        [Browsable(false)]
#endif
        public TargetSelector TargetSelector
        {
            get => targetSelector;
            set
            {
                if (targetSelector != value)
                {
                    targetSelector = value;
                    OnPropertyChanged();
                } 
            }
        }
        private TargetSelector targetSelector = new EntityTarget();

        public new int Range
        {
            get => base.Range;
            set
            {
                if (value < 0)
                    value = 0;
                if (value != base.Range)
                {
                    base.Range = value;
                    OnPropertyChanged();
                }
            }
        }

        [Description("Cooldown on action activation (ms). Minimum is 500 ms")]
        public new int CoolDown
        {
            get => base.CoolDown;
            set
            {
                if (value < 1000)
                    value = 1000;
                if (value != base.CoolDown)
                {
                    base.CoolDown = value;
                    OnPropertyChanged();
                }
            }
        }

#if DEVELOPER
        [Category("Optional")]
        [Editor(typeof(UccConditionListEditor), typeof(UITypeEditor))]
        [Description("Custom UCC-conditions set.")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
#else
        [Browsable(false)]
#endif
        public UCCConditionPack CustomConditions 
        { 
            get => _customConditions;
            set
            {
                if(ReferenceEquals(_customConditions, value))
                    return;
                
                _customConditions = value;
                OnPropertyChanged();
            }
        }
        private UCCConditionPack _customConditions = new UCCConditionPack();

#if DEVELOPER && DEBUG_CHANGE_TARGET
        [Category("General")]
        [XmlIgnore]
        [Editor(typeof(TargetSelectorTestEditor), typeof(UITypeEditor))]
        public string TestInfos => @"Push button '...' =>";
#endif
        #endregion

        #region Взаимодействие с EntityToolsCore

#if DEVELOPER && DEBUG_CHANGE_TARGET
        [Category("DEBUG")]
        [Browsable(false)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [XmlIgnore]
        public object Engine => _engine;
#endif
        [NonSerialized] 
        private IUccActionEngine _engine;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ChangeTarget()
        {
            _engine = new UccActionProxy(this);
            CoolDown = 500;
        }
        private IUccActionEngine MakeProxy()
        {
            return new UccActionProxy(this);
        }

        public void Bind(IUccActionEngine engine)
        {
            _engine = engine;
        }
        public void Unbind()
        {
            _engine = new UccActionProxy(this);
            PropertyChanged = null;
        }
        #endregion

        #region Интерфейс команды

        public override bool NeedToRun => LazyInitializer.EnsureInitialized(ref _engine, MakeProxy).NeedToRun;
        public override bool Run() => LazyInitializer.EnsureInitialized(ref _engine, MakeProxy).Run();
        [XmlIgnore]
        [Browsable(false)]
        public Entity UnitRef => LazyInitializer.EnsureInitialized(ref _engine, MakeProxy).UnitRef;
        public override string ToString() => LazyInitializer.EnsureInitialized(ref _engine, MakeProxy).Label();
        #endregion

        public override UCCAction Clone()
        {
            var tarClone = targetSelector.Clone();
            return BaseClone(new ChangeTarget { targetSelector = tarClone });
        }

        #region Hide Inherited Properties
        [XmlIgnore]
        [Browsable(false)]
        public new string ActionName { get; set; }
        [XmlIgnore]
        [Browsable(false)]
        public new Astral.Logic.UCC.Ressources.Enums.Unit Target { get; set; }
        #endregion
    }
}
