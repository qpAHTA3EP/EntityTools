using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Astral.Classes.ItemFilter;
using Astral.Quester;
using Astral.Quester.Classes;
using Astral.Quester.UIEditors;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Tools;
using EntityTools.Tools.Entities;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityTools.Conditions
{
    /// <summary>
    /// Проверка наличия хотя бы одного объекта Entity, подпадающих под шаблон EntityID,
    /// в регионе CustomRegion, заданным в CustomRegionNames
    /// </summary>

    [Serializable]
    public class EntityCount : Condition
    {
        [Description("ID of the Entity for the search (regex)")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        [Category("Entity")]
        public string EntityID { get; set; } = string.Empty;

        [Description("Type of the EntityID:\n" +
            "Simple: Simple test string with a wildcard at the beginning or at the end (char '*' means any symbols)\n" +
            "Regex: Regular expression")]
        [Category("Entity")]
        public ItemFilterStringType EntityIdType { get; set; } = ItemFilterStringType.Simple;

        [Description("The switcher of the Entity field which compared to the property EntityID")]
        [Category("Entity")]
        public EntityNameType EntityNameType { get; set; } = EntityNameType.NameUntranslated;

        [Description("A subset of entities that are searched for a target\n" +
            "Contacts: Only interactable Entities\n" +
            "Complete: All possible Entities")]
        [Editor(typeof(MultiCustomRegionSelectEditor), typeof(UITypeEditor))]
        [Category("Optional")]
        public EntitySetType EntitySetType { get; set; } = EntitySetType.Complete;

        [Description("Check Entity's Region:\n" +
            "True: Count Entity if it located in the same Region as Player\n" +
            "False: Does not consider the region when counting Entities")]
        [Category("Optional")]
        public bool RegionCheck { get; set; } = false;

        [Description("Check if Entity's health greater than zero:\n" +
            "True: Only Entities with nonzero health are detected\n" +
            "False: Entity's health does not checked during search")]
        [Category("Optional")]
        public bool HealthCheck { get; set; } = true;

        [Description("Threshold value to compare by 'Sign' with the number of the Entities")]
        [Category("Tested")]
        public uint Value { get; set; } = 0;

        [Description("The comparison type for the number of the Entities with 'Value'")]
        [Category("Tested")]
        public Relation Sign { get; set; } = Relation.Superior;

        [Category("Location")]
        public Condition.Presence Tested { get; set; } = Condition.Presence.Equal;

        [XmlIgnore]
        private List<string> customRegionNames = null;
        [XmlIgnore]
        private List<CustomRegion> customRegions = null;

        [Description("The list of the CustomRegions where Entities is counted")]
        [Editor(typeof(MultiCustomRegionSelectEditor), typeof(UITypeEditor))]
        [Category("Location")]
        public List<string> CustomRegionNames
        {
            get => customRegionNames;
            set
            {
                if (customRegionNames != value)
                {
                    if (value != null
                        && value.Count > 0)
                        customRegions = Astral.Quester.API.CurrentProfile.CustomRegions.FindAll((CustomRegion cr) =>
                                    value.Exists((string regName) => regName == cr.Name));
                    else customRegions = null;
                    customRegionNames = value;
                }
            }
        }

        [XmlIgnore]
        private Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(500);

        [Description("The maximum distance from the character within which the Entity is searched\n" +
            "The default value is 0, which disables distance checking")]
        [Category("Optional")]
        public float ReactionRange { get; set; } = 0;

        [Description("Time between searches of the Entity (ms)")]
        [Category("Optional")]
        public int SearchTimeInterval { get; set; } = 500;

        public override string ToString()
        {
            return $"{GetType().Name} {Sign} {Value}";
        }

        [XmlIgnore]
        List<Entity> entities = null;

        public override bool IsValid
        {
            get
            {
                if (!string.IsNullOrEmpty(EntityID))
                {
                    if (timeout.IsTimedOut)
                    {
                        entities = SearchCached.FindAllEntity(EntityID, EntityIdType, EntityNameType, EntitySetType,
                           HealthCheck, ReactionRange, RegionCheck, customRegions);
                        timeout.ChangeTime(SearchTimeInterval);
                    }

                    uint entCount = 0;

                    if(entities != null)
                        foreach (Entity entity in entities)
                        {
                            bool match = false;

                            foreach (CustomRegion cr in customRegions)
                            {
                                if (CommonTools.IsInCustomRegion(entity, cr))
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
                if (!string.IsNullOrEmpty(EntityID))
                {
                    List<Entity> entities = SearchCached.FindAllEntity(EntityID, EntityIdType, EntityNameType, EntitySetType,
                           HealthCheck, ReactionRange, RegionCheck, customRegions);

                    StringBuilder strBldr = new StringBuilder();
                    strBldr.AppendLine();
                    uint entCount = 0;

                    foreach (Entity entity in entities)
                    {
                        StringBuilder strBldr2 = new StringBuilder();
                        bool match = false;

                        foreach (CustomRegion customRegion in Astral.Quester.API.CurrentProfile.CustomRegions)
                        {
                            if (CommonTools.IsInCustomRegion(entity, customRegion))
                            {
                                match = true;
                                if (strBldr2.Length > 0)
                                    strBldr2.Append(", ");
                                strBldr2.Append($"[{customRegion.Name}]");
                            }
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
                return $"'{nameof(EntityID)}' or '{nameof(CustomRegionNames)}' properties are not set";
            }
        }


        public EntityCount() { }
        public override void Reset() { }
    }
}
