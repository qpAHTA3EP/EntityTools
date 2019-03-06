using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Astral.Quester;
using Astral.Quester.Classes;
using Astral.Quester.UIEditors;
using EntityPlugin.Editors;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityPlugin.Conditions
{
    /// <summary>
    /// Проверка наличия хотя бы одного объекта Entity, подпадающих под шаблон EntityID,
    /// в регионе CustomRegion, заданным в CustomRegionName
    /// 
    /// Варианты реализации:
    /// 1)  Подсчет количества подходящих объектов Entity в регионе CustomRegionName и 
    ///     сопоставление (больше/ меньше/равно)его с заданной величиной Value
    /// 2)  Тоже самое на со списком регионов CustomRegionNames:
    /// 2.1)    Проверка наличия подходящих объектов Entity в любом из CustomRegionNames
    /// 2.2)    Подсчет количества подходящих объектов Entity в любом из CustomRegionNames
    /// </summary>

    [Serializable]
    public class IsEntitiesInCustomRegions : Condition
    {
        public Condition.Presence Tested { get; set; }

        [Editor(typeof(CustomRegionEditor), typeof(UITypeEditor))]
        public string CustomRegionName { get; set; }

        [Description("ID (an untranslated name) of the Entity for the search (regex)")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        public string EntityID { get; set; }

        public IsEntitiesInCustomRegions()
        {
            Tested = Condition.Presence.Equal;
            EntityID = string.Empty;

            foreach (CustomRegion customRegion in Core.Profile.CustomRegions)
            {
                if (customRegion.IsIn)
                {
                    CustomRegionName = customRegion.Name;
                    break;
                }
            }
        }

        public override string ToString()
        {
            return "CustomRegion " + Tested + " " + CustomRegionName;
        }

        public override bool IsValid
        {
            get
            {
                List<Entity> allEntities = EntityManager.GetEntities();

                List<Entity> entities = allEntities.FindAll((Entity x) => Regex.IsMatch(x.NameUntranslated, EntityID));

                foreach (Entity entity in entities)
                {
                    foreach (CustomRegion customRegion in Core.Profile.CustomRegions)
                    {
                        if (customRegion.Name == this.CustomRegionName)
                        {
                            Condition.Presence tested = this.Tested;
                            if (tested == Condition.Presence.Equal)
                            {
                                return Tools.IsInCustomRegion(entity, customRegion);
                            }
                            if (tested == Condition.Presence.NotEquel)
                            {
                                return !Tools.IsInCustomRegion(entity, customRegion); ;
                            }
                        }
                    }
                }
                return false;
            }
        }

        public override string TestInfos
        {
            get
            {
                List<Entity> allEntities = EntityManager.GetEntities();

                List<Entity> entities = allEntities.FindAll((Entity x) => Regex.IsMatch(x.NameUntranslated, EntityID));

                string text = $"Entities [{EntityID}] are detected in custom regions :";

                foreach (Entity entity in entities)
                {
                    foreach (CustomRegion customRegion in Core.Profile.CustomRegions)
                    {
                        if (Tools.IsInCustomRegion(entity, customRegion))
                        {
                            text = text + customRegion.Name + Environment.NewLine;
                        }
                    }
                }
                return text;
            }
        }

        public override void Reset()
        {
        }
    }
}
