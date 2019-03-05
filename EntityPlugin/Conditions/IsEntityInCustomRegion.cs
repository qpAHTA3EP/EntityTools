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
    public class IsEntityInCustomRegions : Condition
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

        public IsEntityInCustomRegions()
        {
            Tested = Condition.Presence.Equal;
            EntityID = string.Empty;

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
            return GetType().Name;
        }

        public override bool IsValid
        {
            get
            {
                if (!string.IsNullOrEmpty(EntityID) && CustomRegionNames.Count > 0)
                {
                    List<Entity> allEntities = EntityManager.GetEntities();

                    List<Entity> entities = allEntities.FindAll((Entity x) => Regex.IsMatch(x.NameUntranslated, EntityID));

                    List<CustomRegion> customRegions = Astral.Quester.API.CurrentProfile.CustomRegions.FindAll((CustomRegion cr) =>
                                                               CustomRegionNames.Exists((string regName) => regName == cr.Name));

                    foreach (Entity entity in entities)
                    {
                        foreach (CustomRegion cr in customRegions)
                        {
                            if (Tested == Condition.Presence.Equal)
                            {
                                return Tools.IsInCustomRegion(entity, cr);
                            }
                            if (Tested == Condition.Presence.NotEquel)
                            {
                                return !Tools.IsInCustomRegion(entity, cr); ;
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

                if (!string.IsNullOrEmpty(EntityID) && CustomRegionNames.Count > 0)
                {
                    List<Entity> allEntities = EntityManager.GetEntities();

                    List<Entity> entities = allEntities.FindAll((Entity x) => Regex.IsMatch(x.NameUntranslated, EntityID));

                    List<CustomRegion> customRegions = Astral.Quester.API.CurrentProfile.CustomRegions.FindAll((CustomRegion cr) =>
                                                               CustomRegionNames.Exists((string regName) => regName == cr.Name));

                    StringBuilder strBldr = new StringBuilder();
                    strBldr.AppendLine();
                    uint entCount = 0;

                    foreach (Entity entity in entities)
                    {
                        StringBuilder strBldr2 = new StringBuilder();

                        foreach (CustomRegion customRegion in customRegions)
                        {
                            if (Tools.IsInCustomRegion(entity, customRegion))
                            {
                                if (strBldr2.Length > 0)
                                    strBldr2.Append(", ");
                                strBldr2.Append($"[{customRegion.Name}]");
                            }
                        }
                        if (strBldr2.Length > 0)
                        {
                            strBldr.AppendLine($"[{entity.InternalName}] is in CustomRegions: ").Append(strBldr2);
                            entCount++;
                        }
                    }
                    strBldr.Insert(0, $"Total {entCount} Entities [{EntityID}] are detected in {CustomRegionNames.Count} CustomRegion:");


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
