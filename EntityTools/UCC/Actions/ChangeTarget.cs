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
using AcTp0Tools.Classes.Targeting;
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
        [Description("Алгоритм выбора цели")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
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
        internal TargetSelector targetSelector = new EntityTarget();

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
        [Description("Набор нестандартных UCC-условий, которые проверяют после основных")]
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
        internal UCCConditionPack _customConditions = new UCCConditionPack();

#if DEVELOPER && DEBUG_CHANGE_TARGET
        [XmlIgnore]
        [Editor(typeof(TargetSelectorTestEditor), typeof(UITypeEditor))]
        public string TestInfos { get; } = @"Нажми на кнопку '...' чтобы увидеть больше =>";
#endif
        #endregion

        #region Взаимодействие с EntityToolsCore
#if false && DEVELOPER && DEBUG_CHANGE_TARGET
        [Category("DEBUG")]
        [Browsable(false)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public object InternalEngine => Engine;
#endif
        [NonSerialized] 
        internal IUccActionEngine Engine;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ChangeTarget()
        {
            Engine = new UccActionProxy(this);
            CoolDown = 500;
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
