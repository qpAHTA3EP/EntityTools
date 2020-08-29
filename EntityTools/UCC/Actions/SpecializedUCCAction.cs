//#define DEBUG_LOG

using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;
using Astral.Logic.UCC.Classes;
using EntityTools.Reflection;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Extensions;
using EntityTools.Tools;
using EntityTools.UCC.Conditions;
using Unit = Astral.Logic.UCC.Ressources.Enums.Unit;
using System.Text;

namespace EntityTools.UCC.Actions
{
    /// <summary>
    /// UCC-команда со списком специальных условий
    /// </summary>
    [Serializable]
    
    public class SpecializedUCCAction : UCCAction
    {
#if DEBUG && DEBUG_LOG
        static StringBuilder debugStr = new StringBuilder(1000);
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

#if DEVELOPER
        [Category("Custom Conditions")]
        [Description("Список нестандартных условий, реализованных в плагинах")]
        [Editor(typeof(UCCConditionListEditor), typeof(UITypeEditor))]
#else
        [Browsable(false)]
#endif
        public List<UCCCondition> CustomConditions { get; set; }

#if DEVELOPER
        [Category("Custom Conditions")]
        [Description("The negation of the result of the ConditionPack")]
#else
        [Browsable(false)]
#endif
        public bool Not { get; set; }

#if DEVELOPER
        [Category("Custom Conditions")]
        [Description("Logical rule of the Conditions checks\n" +
            "Conjunction: All Conditions have to be True (Logical AND)\n" +
            "Disjunction: At least one of the Conditions have to be True (Logical OR)")]
#else
        [Browsable(false)]
#endif
        public LogicRule CustomConditionCheck { get; set; }

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
            Target = Unit.Player;
        }

        public override bool NeedToRun
        {
            get
            {
                if (ManagedAction != null)
                {
                    if (!ManagedAction.NeedToRun)
                        return false;

                    if (CustomConditions != null && CustomConditions.Count > 0)
                    {
                        bool result = true;
#if DEBUG && DEBUG_LOG
                        debugStr.Clear();
                        debugStr.Append(GetType().Name).Append('[').Append(GetHashCode().ToString("X2")).Append(']').Append(MethodBase.GetCurrentMethod().Name).Append(":: ").Append(CustomConditionCheck).Append(" of the Conditions is %RES%");
                        if(Not)
                            debugStr.Append(" => Not [");
                        else debugStr.Append(" => [");
#endif
                        if (CustomConditionCheck == LogicRule.Disjunction)
                        {
                            int lockedNum = 0;
                            int okUnlockedNum = 0;
                            bool lockedTrue = true;
#if DEBUG && DEBUG_LOG
                            int num = 0;
#endif
                            foreach (UCCCondition c in CustomConditions)
                            {
                                if (c is ICustomUCCCondition iCond)
                                {
#if DEBUG && DEBUG_LOG
                                    if (num > 0)
                                        debugStr.Append("; ");
                                    debugStr.Append(iCond.GetType().Name);
#endif
                                    if (iCond.Loked)
                                    {
#if DEBUG && DEBUG_LOG
                                        debugStr.Append("(L) ");
#endif
                                        if (!iCond.IsOK(ManagedAction))
                                        {
#if DEBUG && DEBUG_LOG
                                            debugStr.Append("False");
#endif
                                            lockedTrue = false;
                                            break;
                                        }
#if DEBUG && DEBUG_LOG
                                        else debugStr.Append("True");
#endif
                                        lockedNum++;
                                    }
                                    else
                                    {
#if DEBUG && DEBUG_LOG
                                        debugStr.Append("(U) ");
#endif
                                        if (iCond.IsOK(ManagedAction))
                                        {
#if DEBUG && DEBUG_LOG
                                            debugStr.Append("True");
#endif
                                            okUnlockedNum++;
                                        }
#if DEBUG && DEBUG_LOG
                                        else debugStr.Append("False");
#endif
                                    }
                                }
                                else
                                {
#if DEBUG && DEBUG_LOG
                                    if (num > 0)
                                        debugStr.Append(" ;");
                                    debugStr.Append(c.Tested);
#endif
                                    if (c.Locked)
                                    {
#if DEBUG && DEBUG_LOG
                                        debugStr.Append("(L) ");
#endif
                                        if (!c.IsOK(ManagedAction))
                                        {
#if DEBUG && DEBUG_LOG
                                            debugStr.Append("False");
#endif
                                            lockedTrue = false;
                                            break;
                                        }
#if DEBUG && DEBUG_LOG
                                        else debugStr.Append("True");
#endif
                                        lockedNum++;
                                    }
                                    else
                                    {
#if DEBUG && DEBUG_LOG
                                        debugStr.Append("(U) ");
#endif
                                        if (c.IsOK(ManagedAction))
                                        {
#if DEBUG && DEBUG_LOG
                                            debugStr.Append("True");
#endif
                                            okUnlockedNum++;
                                        }
#if DEBUG && DEBUG_LOG
                                        else debugStr.Append("False");
#endif
                                    }
                                }
#if DEBUG && DEBUG_LOG
                                num++;
#endif
                            }
#if DEBUG && DEBUG_LOG
                            debugStr.Append(']');
#endif

                                // Если множество незалоченных условий пустое, тогда условие истинно
                                // Если оно НЕ пустое, тогда должно встретиться хотя бы одно истинно 
                                result = lockedTrue && (CustomConditions.Count == lockedNum || okUnlockedNum > 0);

                            // отрицание результата, если задан флаг
                            if (Not)
                                result = !result;
#if DEBUG && DEBUG_LOG
                            debugStr.Replace("%RES%", (result) ? "True " : "False");
#endif
                        }
                        else
                        {
                            // Проверка всех условий
#if DEBUG && DEBUG_LOG
                            int num = 0;
#endif
                            foreach (UCCCondition c in CustomConditions)
                            {
#if DEBUG && DEBUG_LOG
                                if (num > 0)
                                    debugStr.Append("; ");
#endif
                                if (c is ICustomUCCCondition iCond)
                                {
#if DEBUG && DEBUG_LOG
                                    debugStr.Append(iCond.GetType().Name);
#endif
                                    if (!iCond.IsOK(ManagedAction))
                                    {
#if DEBUG && DEBUG_LOG
                                        debugStr.Append(" False");
#endif
                                        result = false;
                                        break;
                                    }
#if DEBUG && DEBUG_LOG
                                    else debugStr.Append(" True");
#endif
                                }
                                else
                                {
#if DEBUG && DEBUG_LOG
                                    debugStr.Append(c.Tested);
#endif
                                    if (!c.IsOK(ManagedAction))
                                    {
#if DEBUG && DEBUG_LOG
                                        debugStr.Append(" False");
#endif
                                        result = false;
                                        break;
                                    }
#if DEBUG && DEBUG_LOG
                                    else debugStr.Append(" True");
#endif

                                }
#if DEBUG && DEBUG_LOG
                                num++;
#endif
                            }
#if DEBUG && DEBUG_LOG
                            debugStr.Append(']');
#endif

                            if (Not)
                                result = !result;

#if DEBUG && DEBUG_LOG
                            debugStr.Replace("%RES%", (result) ? "True " : "False");
#endif
                        }
#if DEBUG && DEBUG_LOG
                        ETLogger.WriteLine(LogType.Debug, debugStr.ToString());
#endif
                        return result;
                    }
                }

                return false;
            }
        }

        public override bool Run()
        {
            bool result = ManagedAction?.Run() == true;
            if(result && !string.IsNullOrEmpty(TimerName) && Timeout > 0)
            {
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
            if (ManagedAction is null)
                return $"Property '{nameof(ManagedAction)}' does not set !";
            else return "(SP) " + ManagedAction.ToString();
        }

        public override UCCAction Clone()
        {
            return base.BaseClone(new SpecializedUCCAction()
            {
                ManagedAction = CopyHelper.CreateDeepCopy(this.ManagedAction),
                CustomConditionCheck = this.CustomConditionCheck,
                CustomConditions = this.CustomConditions.Clone() ?? new List<UCCCondition>()
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
        //[XmlIgnore]
        //[Browsable(false)]
        //public new bool OneCondMustGood
        //{
        //    get => (CurrentAction == null) ? false : CurrentAction.OneCondMustGood;
        //    set
        //    {
        //        if (CurrentAction != null)
        //            CurrentAction.OneCondMustGood = value;
        //        base.OneCondMustGood = value;
        //    }
        //}
        [XmlIgnore]
        [Browsable(false)]
        public new int Range
        {
            get => (ManagedAction == null) ? 0 : ManagedAction.Range;
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
            get => (ManagedAction == null) ? 0 : ManagedAction.CoolDown;
            set
            {
                if (ManagedAction != null)
                    ManagedAction.CoolDown = value;
                base.CoolDown = value;
            }
        }
        [XmlIgnore]
        [Browsable(false)]
        public new Unit Target
        {
            get => (ManagedAction == null) ? Unit.Player : ManagedAction.Target;
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
