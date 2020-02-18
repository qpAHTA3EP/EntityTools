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
        [Description("ID of the Entity for the search")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        [Category("Entity")]
        public string EntityID
        {
            get => entityId;
            set
            {
                if (entityId != value)
                {
                    entityId = value;
                    if (!string.IsNullOrEmpty(entityId))
                        Comparer = new EntityComparerToPattern(entityId, entityIdType, entityNameType);
                    else Comparer = null;
                }
            }
        }

        [Description("The switcher of the Entity filed which compared to the property EntityID")]
        [Category("Entity")]
        public EntityNameType EntityNameType
        {
            get => entityNameType;
            set
            {
                if (entityNameType != value)
                {
                    entityNameType = value;
                    if (!string.IsNullOrEmpty(entityId))
                        Comparer = new EntityComparerToPattern(entityId, entityIdType, entityNameType);
                    else Comparer = null;
                }
            }
        }

        [Description("Type of and EntityID:\n" +
            "Simple: Simple text string with a wildcard at the beginning or at the end (char '*' means any symbols)\n" +
            "Regex: Regular expression")]
        [Category("Entity")]
        public ItemFilterStringType EntityIdType
        {
            get => entityIdType;
            set
            {
                if (entityIdType != value)
                {
                    entityIdType = value;
                    if (!string.IsNullOrEmpty(entityId))
                        Comparer = new EntityComparerToPattern(entityId, entityIdType, entityNameType);
                    else Comparer = null;
                }
            }
        }
        [Description("A subset of entities that are searched for a target\n" +
            "Contacts: Only interactable Entities\n" +
            "Complete: All possible Entities")]
        [Editor(typeof(CustomRegionListEditor), typeof(UITypeEditor))]
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

        [Description("The list of the CustomRegions where Entities is counted")]
        [Editor(typeof(CustomRegionListEditor), typeof(UITypeEditor))]
        [Category("Location")]
        public List<string> CustomRegionNames
        {
            get => customRegionNames;
            set
            {
                if (customRegionNames != value)
                {
                    customRegions = CustomRegionTools.GetCustomRegions(value);
                    customRegionNames = value;
                }
            }
        }


        [Description("The maximum distance from the character within which the Entity is searched\n" +
            "The default value is 0, which disables distance checking")]
        [Category("Optional")]
        public float ReactionRange { get; set; } = 0;

        [Description("The maximum ZAxis difference from the withing which the Entity is searched\n" +
            "The default value is 0, which disables ZAxis checking")]
        [Category("Optional")]
        public float ReactionZRange { get; set; } = 0;

        //[Description("Time between searches of the Entity (ms)")]
        //[Category("Optional")]
        //public int SearchTimeInterval { get; set; } = 100;

        public override string ToString()
        {
            return $"{GetType().Name} {Sign} {Value}";
        }

        [NonSerialized]
        LinkedList<Entity> entities = null;
        [NonSerialized]
        private EntityComparerToPattern Comparer = null;
        //[XmlIgnore]
        //private Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(0);
        [NonSerialized]
        private string entityId = string.Empty;
        [NonSerialized]
        private EntityNameType entityNameType = EntityNameType.NameUntranslated;
        [NonSerialized]
        private ItemFilterStringType entityIdType = ItemFilterStringType.Simple;
        [NonSerialized]
        private List<string> customRegionNames = new List<string>();
        [NonSerialized]
        private List<CustomRegion> customRegions = new List<CustomRegion>();



        public override bool IsValid
        {
            get
            {
                if (!string.IsNullOrEmpty(EntityID))
                {
                    if (customRegionNames != null && (customRegions == null || customRegions.Count != customRegionNames.Count))
                        customRegions = CustomRegionTools.GetCustomRegions(customRegionNames);

                    if (Comparer == null)
                        Comparer = new EntityComparerToPattern(entityId, entityIdType, entityNameType);


                    //if (timeout.IsTimedOut)
                    {
                        entities = SearchCached.FindAllEntity(entityId, entityIdType, entityNameType, EntitySetType,
                           HealthCheck, ReactionRange, ReactionZRange, RegionCheck, customRegions);
                        //timeout.ChangeTime(SearchTimeInterval);
                    }

                    uint entCount = 0;

                    if (entities != null)
                    {
                        if (customRegions != null && customRegions.Count > 0)
                            foreach (Entity entity in entities)
                            {
                                bool match = false;
                                foreach (CustomRegion cr in customRegions)
                                {
                                    if (entity.Within(cr))
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
                        else entCount = (uint)entities.Count;
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
                    if (customRegionNames != null && (customRegions == null || customRegions.Count != customRegionNames.Count))
                        customRegions = CustomRegionTools.GetCustomRegions(customRegionNames);

                    entities = SearchCached.FindAllEntity(entityId, entityIdType, entityNameType, EntitySetType,
                           HealthCheck, ReactionRange, ReactionZRange, RegionCheck, customRegions);

                    StringBuilder strBldr = new StringBuilder();
                    uint entCount = 0;

                    if (entities != null)
                    {
                        if (customRegions != null && customRegions.Count > 0)
                        {
                            strBldr.AppendLine();
                            foreach (Entity entity in entities)
                            {
                                StringBuilder strBldr2 = new StringBuilder();
                                bool match = false;

                                foreach (CustomRegion customRegion in Astral.Quester.API.CurrentProfile.CustomRegions)
                                {
                                    if (entity.Within(customRegion))
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
                                strBldr.Insert(0, $"Total {entCount} Entities [{entityId}] are detected in {customRegions.Count} CustomRegion:");
                            if (Tested == Presence.NotEquel)
                                strBldr.Insert(0, $"Total {entCount} Entities [{entityId}] are detected out of {customRegions.Count} CustomRegion:");
                        }
                        else strBldr.Append($"Total {entities.Count} Entities [{entityId}] are detected");
                    }
                    else strBldr.Append($"No Entity [{entityId}] was found.");

                    return strBldr.ToString();
                }
                return $"Proeprty '{nameof(EntityID)}' does not set !";
            }
        }


        public override void Reset() { }
        public EntityCount() { }
    }
}
