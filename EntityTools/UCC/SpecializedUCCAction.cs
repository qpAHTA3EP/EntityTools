using System;
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
using Astral.Quester.FSM.States;
using EntityTools;
using EntityTools.Editors;
using EntityTools.Tools;
using EntityTools.UCC.Conditions;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityTools.UCC
{
    /// <summary>
    /// UCC-команда со списком специальных условий
    /// </summary>
    [Serializable]
    public class SpecializedUCCAction : UCCAction
    {
        [Category("Required")]
        [Editor(typeof(UccActionEditor), typeof(UITypeEditor))]
        [Description("Основная ucc-команда, которой транслируется вызов")]
        public UCCAction CurrentAction { get; set; }

        [Category("Required")]
        [Description("Список нестандартных условий, реализованных в плагинах")]
        [Editor(typeof(UCCConditionListEditor), typeof(UITypeEditor))]
        public List<CustomUCCCondition> CustomConditions { get; set; }

        public override bool NeedToRun
        {
            get
            {
                return CurrentAction?.NeedToRun == true;
            }
        }

        public override bool Run()
        {
            return CurrentAction?.Run() == true;
        }

        public override string ToString()
        {
            if (CurrentAction != null)
                return "(SP) " + CurrentAction.ToString();
            else return "Property 'CurrentAction' do not selected";
        }

        public SpecializedUCCAction()
        {
            Target = Astral.Logic.UCC.Ressources.Enums.Unit.Player;
        }
        public override UCCAction Clone()
        {
            return base.BaseClone(new SpecializedUCCAction()
            {
                CurrentAction = this.CurrentAction
            });
        }

        // Переопределение унаследованных свойств
        public new bool ConditionsAreOK
        {
            get
            {
                if (base.ConditionsAreOK)
                {
                    if (OneCondMustGood)
                    {
                        int lockedNum = 0;
                        int okUnlockedNum = 0;
                        foreach (CustomUCCCondition c in CustomConditions)
                        {
                            if (c.Locked)
                            {
                                if (!c.IsOK(this))
                                    return false;
                                lockedNum++;
                            }
                            else if (c.IsOK(this))
                                okUnlockedNum++;
                        }

                        // Если множетство незалоченных условий пустое, тогда условие истино
                        // Если оно НЕ пустое, тогда должно встретиться хотя бы одно истиное
                        return (CustomConditions.Count > lockedNum) ? okUnlockedNum > 0 : true;
                    }
                    else
                    {
                        // Проверка всех условий
                        foreach (CustomUCCCondition c in CustomConditions)
                            if (!c.IsOK(this))
                                return false;

                        return true;
                    }
                }
                return false;
            }
        }
        public new int Random
        {
            get => (CurrentAction == null) ? 0 : CurrentAction.Random;
            set
            {
                if(CurrentAction != null)
                    CurrentAction.Random = value;
                base.Random = value;
            }
        }
        public new bool OneCondMustGood
        {
            get => (CurrentAction == null) ? false : CurrentAction.OneCondMustGood;
            set
            {
                if (CurrentAction != null)
                    CurrentAction.OneCondMustGood = value;
                base.OneCondMustGood = value;
            }
        }
        public new int Range
        {
            get => (CurrentAction == null) ? 0 : CurrentAction.Range;
            set
            {
                if (CurrentAction != null)
                    CurrentAction.Range = value;
                base.Range = value;
            }
        }

        public new int CoolDown
        {
            get => (CurrentAction == null) ? 0 : CurrentAction.CoolDown;
            set
            {
                if (CurrentAction != null)
                    CurrentAction.CoolDown = value;
                base.CoolDown = value;
            }
        }
        public new Astral.Logic.UCC.Ressources.Enums.Unit Target
        {
            get => (CurrentAction == null) ? Astral.Logic.UCC.Ressources.Enums.Unit.Player : CurrentAction.Target;
            set
            {
                if (CurrentAction != null)
                    CurrentAction.Target = value;
                base.Target = value;
            }
        }
        //public string Label { get; }
        //public bool TempAction { get; set; }
        //public bool Enabled { get; set; }
        //public Timeout CurrentTimeout { get; set; }
        //public int Timer { get; set; }

        // Без данного свойства в окне параметров команды отображается бесполезное свойство 'Name'
        [XmlIgnore]
        [Browsable(false)]
        public new string ActionName { get; set; } = string.Empty;

    }
}
