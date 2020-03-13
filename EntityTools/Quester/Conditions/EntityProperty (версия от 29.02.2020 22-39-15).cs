using Astral.Classes;
using Astral.Classes.ItemFilter;
using Astral.Quester.Classes;
using EntityCore.Enums;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Tools;
using EntityCore.Entities;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using System.Xml.Serialization;
using EntityCore.Extentions;

namespace EntityTools.Quester.Conditions
{
    [Serializable]
    public class EntityProperty : Condition
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
                        Comparer = EntityToPatternComparer.Get(entityId, entityIdType, (coreEntityNameType)entityNameType);
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
                        Comparer = EntityToPatternComparer.Get(entityId, entityIdType, (coreEntityNameType)entityNameType);
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
                        Comparer = EntityToPatternComparer.Get(entityId, entityIdType, (coreEntityNameType)entityNameType);
                    else Comparer = null;
                }
            }
        }

        [Description("Check Entity's Region:\n" +
            "True: Search an Entity only if it located in the same Region as Player\n" +
            "False: Does not consider the region when searching Entities")]
        [Category("Optional")]
        public bool RegionCheck { get; set; } = false;

        [Description("Check if Entity's health greater than zero:\n" +
            "True: Only Entities with nonzero health are detected\n" +
            "False: Entity's health does not checked during search")]
        [Category("Optional")]
        public bool HealthCheck { get; set; } = true;

        [Description("A subset of entities that are searched for a target\n" +
            "Contacts: Only interactable Entities\n" +
            "Complete: All possible Entities")]
        [Editor(typeof(CustomRegionListEditor), typeof(UITypeEditor))]
        [Category("Optional")]
        public EntitySetType EntitySetType { get; set; } = EntitySetType.Complete;

        [Description("The maximum distance from the character within which the Entity is searched\n" +
            "The default value is 0, which disables distance checking")]
        [Category("Entity optional checks")]
        public float ReactionRange { get; set; } = 0;

        [Description("The maximum ZAxis difference from the withing which the Entity is searched\n" +
            "The default value is 0, which disables ZAxis checking")]
        [Category("Optional")]
        public float ReactionZRange { get; set; } = 0;

        [Description("CustomRegion names collection")]
        [Editor(typeof(CustomRegionListEditor), typeof(UITypeEditor))]
        [Category("Optional")]
        public List<string> CustomRegionNames
        {
            get => customRegionNames;
            set
            {
                if (customRegionNames != value)
                {
                    customRegions = CustomRegionExtentions.GetCustomRegions(value);
                    customRegionNames = value;
                }
            }
        }

        [Category("Tested")]
        public EntityPropertyType PropertyType { get; set; } = EntityPropertyType.Distance;

        [Category("Tested")]
        public float Value { get; set; } = 0;

        [Description("Value comparison type to the closest Entity")]
        [Category("Tested")]
        public Condition.Relation Sign { get; set; } = Relation.Superior;

        [Description("Time between searches of the Entity (ms)")]
        [Category("Optional")]
        public int SearchTimeInterval { get; set; } = 100;


        [NonSerialized]
        Entity closestEntity = null;
        [NonSerialized]
        private Timeout timeout = new Timeout(0);
        [NonSerialized]
        private Predicate<Entity> Comparer = null;
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
                if (timeout.IsTimedOut || (closestEntity!=null && !Validate(closestEntity)))
                {
                    if (customRegionNames != null && (customRegions == null || customRegions.Count != customRegionNames.Count))
                        customRegions = CustomRegionExtentions.GetCustomRegions(customRegionNames);

                    if (Comparer == null && !string.IsNullOrEmpty(entityId))
                        Comparer = EntityToPatternComparer.Get(entityId, entityIdType, (coreEntityNameType)entityNameType);

                    if (!string.IsNullOrEmpty(EntityID))
                        closestEntity = SearchCached.FindClosestEntity(entityId, entityIdType, (coreEntityNameType)entityNameType, (coreEntitySetType)EntitySetType,
                                                                HealthCheck, ReactionRange, ReactionZRange, RegionCheck, customRegions);

                    timeout.ChangeTime(SearchTimeInterval);
                }

                if (Validate(closestEntity))
                {
                    bool result = false;
                    switch (PropertyType)
                    {
                        case EntityPropertyType.Distance:
                            switch (Sign)
                            {
                                case Relation.Equal:
                                    return result = (closestEntity.Location.Distance3DFromPlayer == Value);
                                case Relation.NotEqual:
                                    return result = (closestEntity.Location.Distance3DFromPlayer != Value);
                                case Relation.Inferior:
                                    return result = (closestEntity.Location.Distance3DFromPlayer < Value);
                                case Relation.Superior:
                                    return result = (closestEntity.Location.Distance3DFromPlayer > Value);
                            }
                            break;
                        case EntityPropertyType.HealthPercent:
                            switch (Sign)
                            {
                                case Relation.Equal:
                                    return result = (closestEntity.Character?.AttribsBasic?.HealthPercent == Value);
                                case Relation.NotEqual:
                                    return result = (closestEntity.Character?.AttribsBasic?.HealthPercent != Value);
                                case Relation.Inferior:
                                    return result = (closestEntity.Character?.AttribsBasic?.HealthPercent < Value);
                                case Relation.Superior:
                                    return result = (closestEntity.Character?.AttribsBasic?.HealthPercent > Value);
                            }
                            break;
                        case EntityPropertyType.ZAxis:
                            switch (Sign)
                            {
                                case Relation.Equal:
                                    return result = (closestEntity.Location.Z == Value);
                                case Relation.NotEqual:
                                    return result = (closestEntity.Location.Z != Value);
                                case Relation.Inferior:
                                    return result = (closestEntity.Location.Z < Value);
                                case Relation.Superior:
                                    return result = (closestEntity.Location.Z > Value);
                            }
                            break;
                    }
                }
                else return (PropertyType == EntityPropertyType.Distance && Sign == Relation.Superior);

                return false;
            }
        }

        public override string TestInfos
        {
            get
            {
                if (customRegionNames != null && (customRegions == null || customRegions.Count != customRegionNames.Count))
                    customRegions = CustomRegionExtentions.GetCustomRegions(customRegionNames);

                closestEntity = SearchCached.FindClosestEntity(entityId, entityIdType, (coreEntityNameType)entityNameType, (coreEntitySetType)EntitySetType,
                                                        HealthCheck, ReactionRange, ReactionZRange, RegionCheck, customRegions);

                if (Validate(closestEntity))
                {
                    StringBuilder sb = new StringBuilder("Found closect Entity");
                    sb.Append(" [").Append(closestEntity.NameUntranslated).Append(']').Append(" which ").Append(PropertyType).Append(" = ");
                    switch (PropertyType)
                    {
                        case EntityPropertyType.Distance:
                            sb.Append(closestEntity.Location.Distance3DFromPlayer);
                            break;
                        case EntityPropertyType.ZAxis:
                            sb.Append(closestEntity.Location.Z);
                            break;
                        case EntityPropertyType.HealthPercent:
                            sb.Append(closestEntity.Character?.AttribsBasic?.HealthPercent);
                            break;
                    }
                    return sb.ToString();
                }
                else
                {
                    StringBuilder sb = new StringBuilder("No one Entity matched to");
                    sb.Append(" [").Append(entityId).Append(']').AppendLine();
                    if (PropertyType == EntityPropertyType.Distance)
                        sb.AppendLine("The distance to the missing Entity is considered equal to infinity.");
                    return sb.ToString();
                }
            }
        }

        private bool Validate(Entity e)
        {
            return e != null && e.IsValid && Comparer?.Invoke(e) == true;
        }


        public override void Reset() { }
        public override string ToString()
        {
            return $"Entity [{entityId}] {PropertyType} {Sign} to {Value}";
        }

        public EntityProperty() { }
    }
}