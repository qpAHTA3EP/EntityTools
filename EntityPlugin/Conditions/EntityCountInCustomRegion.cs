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
using EntityPlugin.Tools;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityPlugin.Conditions
{
    /// <summary>
    /// Проверка наличия хотя бы одного объекта Entity, подпадающих под шаблон EntityID,
    /// в регионе CustomRegion, заданным в CustomRegionNames
    /// 
    /// Варианты реализации:
    /// 1)  Подсчет количества подходящих объектов Entity в регионе CustomRegionNames и 
    ///     сопоставление (больше/ меньше/равно)его с заданной величиной Value
    /// 2)  Тоже самое на со списком регионов CustomRegionNames:
    /// 2.1)    Проверка наличия подходящих объектов Entity в любом из CustomRegionNames
    /// 2.2)    Подсчет количества подходящих объектов Entity в любом из CustomRegionNames
    /// </summary>

    [Serializable]
    public class EntityCountInCustomRegions : Condition
    {
        public Condition.Presence Tested { get; set; }

        //[Editor(typeof(CustomRegionEditor), typeof(UITypeEditor))]
        //public string CustomRegionName { get; set; }
        [Description("CustomRegion names collection")]
        [Editor(typeof(MultiCustomRegionSelectEditor), typeof(UITypeEditor))]
        public List<string> CustomRegionNames { get; set; }

        [Description("ID (an untranslated name) of the Entity for the search (regex)")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        public string EntityID { get; set; }

        [Description("Threshold value of the Entity number for comparison by 'Sign'")]
        public uint Value { get; set; }

        [Description("The comparison type for 'Value'")]
        public Relation Sign { get; set; }

        public EntityCountInCustomRegions()
        {
            Tested = Condition.Presence.Equal;
            EntityID = string.Empty;

            Value = 0;
            Sign = Relation.Superior;

            CustomRegionNames = new List<string>();
            foreach (CustomRegion customRegion in Astral.Quester.API.CurrentProfile.CustomRegions)
            {
                if (customRegion.IsIn)
                {
                    CustomRegionNames.Add(customRegion.Name);
                    break;
                }
            }
        }

        public override string ToString()
        {
            return $"{GetType().Name} {Sign} {Value}";
        }

        public override bool IsValid
        {
            get
            {
                if (!string.IsNullOrEmpty(EntityID) && CustomRegionNames.Count > 0)
                {
                    List<Entity> entities = EntityManager.GetEntities().FindAll((Entity x) => Regex.IsMatch(x.NameUntranslated, EntityID));

                    List<CustomRegion> customRegions = Astral.Quester.API.CurrentProfile.CustomRegions.FindAll((CustomRegion cr) =>
                                                               CustomRegionNames.Exists((string regName) => regName == cr.Name));
                    uint entCount = 0;

                    foreach (Entity entity in entities)
                    {
                        bool match = false;

                        foreach (CustomRegion cr in customRegions)
                        {
                            if (SelectionTools.IsInCustomRegion(entity, cr))
                            {
                                match = true;
                                break;
                            }
                        }

                        if (Tested == Presence.Equal && match)
                            entCount++;
                        if (Tested == Presence.NotEquel && !match)
                            entCount++;
                    }

                    switch(Sign)
                    {
                        case Relation.Equal:
                            return entCount == Value;
                        case Relation.NotEqual:
                            return entCount != Value;
                        case Relation.Inferior:
                            return entCount < Value;
                        case Relation.Superior:
                            return entCount > Value;
                    }
                }
                return false;
            }
        }

        public override string TestInfos
        {
            get
            {

                if (!string.IsNullOrEmpty(EntityID) && CustomRegionNames.Count > 0)
                {
                    List<Entity> entities = EntityManager.GetEntities().FindAll((Entity x) => Regex.IsMatch(x.NameUntranslated, EntityID));

                    //List<CustomRegion> customRegions = Astral.Quester.API.CurrentProfile.CustomRegions.FindAll((CustomRegion cr) =>
                    //                                           CustomRegionNames.Exists((string regName) => regName == cr.Name));

                    StringBuilder strBldr = new StringBuilder();
                    strBldr.AppendLine();
                    uint entCount = 0;

                    foreach (Entity entity in entities)
                    {
                        StringBuilder strBldr2 = new StringBuilder();
                        bool match = false;

                        foreach (CustomRegion customRegion in Astral.Quester.API.CurrentProfile.CustomRegions) //customRegions)
                        {
                            if (SelectionTools.IsInCustomRegion(entity, customRegion))
                                match = true;
                            if (strBldr2.Length > 0)
                                strBldr2.Append(", ");
                            strBldr2.Append($"[{customRegion.Name}]");
                        }

                        if (Tested == Presence.Equal && match)
                            entCount++;
                        if (Tested == Presence.NotEquel && !match)
                            entCount++;

                        strBldr.Append($"[{entity.InternalName}] is in CustomRegions: ").Append(strBldr2).AppendLine();

                    }

                    if (Tested == Presence.Equal)
                        strBldr.Insert(0, $"Total {entCount} Entities [{EntityID}] are detected in {CustomRegionNames.Count} CustomRegion:");
                    if(Tested == Presence.NotEquel)
                        strBldr.Insert(0, $"Total {entCount} Entities [{EntityID}] are detected out of {CustomRegionNames.Count} CustomRegion:");

                    return strBldr.ToString();
                }
                return "EntityID or CustomRegionNames properties are not set";
            }
        }

        public override void Reset()
        {
        }
    }
}
