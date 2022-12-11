//#define DEBUG_LOG

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Xml.Serialization;
using ACTP0Tools.Reflection;
using Astral.Logic.UCC.Classes;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Tools;
using EntityTools.UCC.Conditions;

namespace EntityTools.UCC.Actions
{
    /// <summary>
    /// UCC-команда со списком специальных условий
    /// </summary>
    [Serializable]
    // ReSharper disable once InconsistentNaming
    public class SpecializedUCCAction : UCCAction
    {
#if DEBUG && DEBUG_LOG
        static StringBuilder debugStr = new StringBuilder();
#endif

        #region Опции команды
#if DEVELOPER
        [Category("Managed Action")]
        [Editor(typeof(UccActionEditor), typeof(UITypeEditor))]
        [Description("Основная ucc-команда, которой транслируется вызов")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
#else
        [Browsable(false)]
#endif
        public UCCAction ManagedAction { get; set; }

#if CUSTOM_UCC_CONDITION_EDITOR
#if DEVELOPER
        [Category("Custom Conditions")]
        [Description("Список нестандартных условий, реализованных в плагинах")]
        [Editor(typeof(UccConditionListEditor), typeof(UITypeEditor))]
#else
        [Browsable(false)]
#endif
        public List<UCCCondition> CustomConditions { get; set; }

#if DEVELOPER
        [Category("Custom Conditions")]
        [Description("Отрицание результата проверки условий (логическое НЕ)")]
#else
        [Browsable(false)]
#endif
        public bool Not { get; set; }

#if DEVELOPER
        [Category("Custom Conditions")]
        [Description("Логическое правило проверки набора условий. Logical rule of the Conditions checks\n" +
            "Conjunction: Все условия должны быть истины. All Conditions have to be True (Logical AND)\n" +
            "Disjunction: Хотя бы одно условие должно быть истино. At least one of the Conditions have to be True (Logical OR)")]
#else
        [Browsable(false)]
#endif
        public LogicRule CustomConditionCheck { get; set; } 
#else
        /// <summary>
        /// Данное свойство необходимо в целях обеспечения совместимости со старой версией <see cref="SpecializedUCCAction"/>,
        /// в которой дополнительные условия содержались в отдельном списке <see cref="CustomConditions"/>.<br/>
        /// При десериализации <see cref="SpecializedUCCAction"/> стандартные <see cref="XmlSerializer"/> сначала инициализирует пустой список,
        /// а затем добавляет в него десериализованные элементы методом <see cref="ICollection{UCCCondition}.Add"/>.<br/>
        /// В новой версии <see cref="SpecializedUCCAction"/> все условия хранятся в одном списке <see cref="UCCAction.Conditions"/>,
        /// поэтому приходится отслеживать момент добавления условия в список <see cref="CustomConditions"/>, чтобы в этот момент обернуть его в <see cref="UCCConditionPack"/> и добавить его в <see cref="UCCAction.Conditions"/>.<br/>
        /// 
        /// </summary>
        [Browsable(false)]
        public ObservableCollection<UCCCondition> CustomConditions
        {
            get
            {
                if (_customConditions is null)
                {
                    _customConditions = new UCCConditionPack();
                    _customConditions.Conditions.CollectionChanged += OnCustomConditionChanged;
                }
                return _customConditions.Conditions;
            }
            set
            {
                if (value != null)
                {
                    if (_customConditions is null)
                    {
                        _customConditions = new UCCConditionPack { Conditions = value };
                        if (value.Count > 0)
                            base.Conditions.Add(_customConditions);
                        else _customConditions.Conditions.CollectionChanged += OnCustomConditionChanged;
                        return;
                    }

                    if (ReferenceEquals(_customConditions.Conditions, value))
                        return;
                    
                    _customConditions.Conditions.CollectionChanged -= OnCustomConditionChanged;
                    _customConditions.Conditions = value;
                    if(value.Count > 0)
                        base.Conditions.Add(_customConditions);
                    else _customConditions.Conditions.CollectionChanged += OnCustomConditionChanged;
                }
                else _customConditions?.Conditions.Clear();
            }
        }
        /// <summary>
        /// Метод для отследживания момент добавления условия в список <see cref="CustomConditions"/> и добавления в этот момент <see cref="_customConditions"/> в список <see cref="UCCAction.Conditions"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCustomConditionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_customConditions is null 
                || !ReferenceEquals(sender, _customConditions.Conditions))
            {
                ((ObservableCollection<UCCCondition>)sender).CollectionChanged -= OnCustomConditionChanged;
                return;
            }
            if (e.Action == NotifyCollectionChangedAction.Add
                && !base.Conditions.Contains(_customConditions))
            {
                base.Conditions.Add(_customConditions);
                _customConditions.Conditions.CollectionChanged -= OnCustomConditionChanged;
            }
        }
        public bool ShouldSerializeCustomConditions() => false;

        [Browsable(false)]
        public bool Not
        {
            get => _customConditions?.Not ?? false; 
            set => (_customConditions ?? (_customConditions = new UCCConditionPack())).Not = value;
        }
        public bool ShouldSerializeNot() => false;

        [Browsable(false)]
        public LogicRule CustomConditionCheck
        {
            get => _customConditions?.TestRule ?? LogicRule.Conjunction;
            set => (_customConditions ?? (_customConditions = new UCCConditionPack())).TestRule = value;
        }
        public bool ShouldSerializeCustomConditionCheck() => false;

        private UCCConditionPack _customConditions;
#endif

#if DEVELOPER
        [Category("SpecificTimer")]
#else
        [Browsable(false)]
#endif
        public string TimerName { get; set; } = string.Empty;

#if DEVELOPER
        [Category("SpecificTimer")]
#else
        [Browsable(false)]
#endif
        public int Timeout { get; set; } = 0; 
        #endregion

        public SpecializedUCCAction()
        {
            Target = Astral.Logic.UCC.Ressources.Enums.Unit.Player;
        }

        public override bool NeedToRun
        {
            get
            {
                if (ManagedAction is null) 
                    return false;

                return ManagedAction.NeedToRun;
            }
        }

        public override bool Run()
        {
            bool result = ManagedAction?.Run() == true;
            if(result && !string.IsNullOrEmpty(TimerName) && Timeout > 0)
            {
                //TODO Заменить Environment.TickCount на DateTime.Ticks 
                // Запуск таймера
                if (UCCTools.SpecialTimers.ContainsKey(TimerName))
                    UCCTools.SpecialTimers[TimerName] = new Pair<int, int>(Environment.TickCount, Environment.TickCount + Timeout);
                else UCCTools.SpecialTimers.Add(TimerName, new Pair<int, int>(Environment.TickCount, Environment.TickCount + Timeout));
#if DEBUG && DEBUG_LOG
                debugStr.Append("False");
#endif
            }
            return result;
        }

        public override string ToString()
        {
            return ManagedAction is null 
                 ? $"Property '{nameof(ManagedAction)}' does not set !" 
                 : "(SP) " + ManagedAction.ToString();
        }

        public override UCCAction Clone()
        {
            return BaseClone(new SpecializedUCCAction
            {
                ManagedAction = ManagedAction.CreateXmlCopy(),
                CustomConditionCheck = CustomConditionCheck,
                CustomConditions = new ObservableCollection<UCCCondition>(CustomConditions.Select(cnd => cnd is ICustomUCCCondition cstCnd
                                                                                                                       ? (UCCCondition)cstCnd.Clone()
                                                                                                                       : cnd.Clone()))
            });
        }

        #region Hide Inherited Properties
        [XmlIgnore]
        [Browsable(false)]
        public new int Random
        {
            get => (ManagedAction == null) ? 0 : ManagedAction.Random;
            set
            {
                if(ManagedAction != null)
                    ManagedAction.Random = value;
                base.Random = value;
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        public new int Range
        {
            get => (ManagedAction is null) ? 0 : ManagedAction.Range;
            set
            {
                if (ManagedAction != null)
                    ManagedAction.Range = value;
                base.Range = value;
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        public new int CoolDown
        {
            get => (ManagedAction is null) ? 0 : ManagedAction.CoolDown;
            set
            {
                if (ManagedAction != null)
                    ManagedAction.CoolDown = value;
                base.CoolDown = value;
            }
        }
        [XmlIgnore]
        [Browsable(false)]
        public new Astral.Logic.UCC.Ressources.Enums.Unit Target
        {
            get => (ManagedAction is null) ? Astral.Logic.UCC.Ressources.Enums.Unit.Player : ManagedAction.Target;
            set
            {
                if (ManagedAction != null)
                    ManagedAction.Target = value;
                base.Target = value;
            }
        }

        // Без данного свойства в окне параметров команды отображается бесполезное свойство 'ActionName'
        [XmlIgnore]
        [Browsable(false)]
        public new string ActionName { get; set; } = string.Empty;
        #endregion
    }
}
