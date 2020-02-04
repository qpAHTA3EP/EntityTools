﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;
using Astral.Classes;
using Astral.Classes.ItemFilter;
using Astral.Controllers;
using Astral.Logic.NW;
using Astral.Logic.UCC.Actions;
using Astral.Logic.UCC.Classes;
using Astral.Logic.UCC.Ressources;
using Astral.Quester.FSM.States;
using EntityTools;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Extensions;
using EntityTools.Tools;
using EntityTools.UCC.Conditions;
using MyNW.Classes;
using MyNW.Internals;
using Unit = Astral.Logic.UCC.Ressources.Enums.Unit;

namespace EntityTools.UCC
{
    /// <summary>
    /// UCC-команда со списком специальных условий
    /// </summary>
    [Serializable]
    
    public class SpecializedUCCAction : UCCAction
    {
        [Category("Managed Action")]
        [Editor(typeof(UccActionEditor), typeof(UITypeEditor))]
        [Description("Основная ucc-команда, которой транслируется вызов")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public UCCAction ManagedAction { get; set; }

        [Category("Custom Conditions")]
        [Description("Список нестандартных условий, реализованных в плагинах")]
        [Editor(typeof(UCCConditionListEditor), typeof(UITypeEditor))]
        public List<UCCCondition> CustomConditions { get; set; }

        [Category("Custom Conditions")]
        [Description("The negation of the result of the ConditionPack")]
        public bool Not { get; set; }

        [Category("Custom Conditions")]
        [Description("Logical rule of the Conditions checks\n" +
            "Conjunction: All Conditions have to be True (Logical AND)\n" +
            "Disjunction: At least one of the Conditions have to be True (Logical OR)")]
        public LogicRule CustomConditionCheck { get; set; }

        [Category("Timer")]
        public string TimerName { get; set; } = string.Empty;

        [Category("Timer")]
        public int Timeout { get; set; } = 0;

        public override bool NeedToRun
        {
            get
            {
                if (ManagedAction != null)
                {
                    if (!ManagedAction.NeedToRun)
                        return false;

                    if (CustomConditions != null && CustomConditions.Count > 0)
                        if (CustomConditionCheck == LogicRule.Disjunction)
                        {
                            int lockedNum = 0;
                            int okUnlockedNum = 0;
                            foreach (UCCCondition c in CustomConditions)
                            {
                                if (c is ICustomUCCCondition iCond)
                                {
                                    if (iCond.Loked)
                                    {
                                        if (!iCond.IsOK(ManagedAction))
                                            return false;
                                        lockedNum++;
                                    }
                                    else if (c.IsOK(ManagedAction))
                                        okUnlockedNum++;
                                }
                                else
                                {
                                    if (c.Locked)
                                    {
                                        if (!c.IsOK(ManagedAction))
                                            return false;
                                        lockedNum++;
                                    }
                                    else if (c.IsOK(ManagedAction))
                                        okUnlockedNum++;
                                }
                            }

                            // Если множетство незалоченных условий пустое, тогда условие истино
                            // Если оно НЕ пустое, тогда должно встретиться хотя бы одно истиное
                            return (CustomConditions.Count > lockedNum) ? okUnlockedNum > 0 : true;
                        }
                        else
                        {
                            // Проверка всех условий
                            foreach (UCCCondition c in CustomConditions)
                                if (c is ICustomUCCCondition iCond)
                                {
                                    if (!iCond.IsOK(ManagedAction))
                                        return false;
                                }
                                else if (!c.IsOK(ManagedAction))
                                    return false;

                            return true;
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
            }
            return result;
        }

        public override string ToString()
        {
            if (ManagedAction != null)
                return "(SP) " + ManagedAction.ToString();
            else return "Property 'CurrentAction' does not set !";
        }

        public SpecializedUCCAction()
        {
            Target = Unit.Player;
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
