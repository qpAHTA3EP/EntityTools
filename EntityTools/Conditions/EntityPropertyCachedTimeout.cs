using Astral.Classes;
using Astral.Classes.ItemFilter;
using Astral.Quester.Classes;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Tools;
using EntityTools.Tools.Entities;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using System.Xml.Serialization;

namespace EntityTools.Conditions
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
                    Comparer = new EntityComparerToPattern(entityId, entityIdType, entityNameType);
                }
            }
        }

        [Description("Type of and EntityID:\n" +
            "Simple: Simple test string with a wildcard at the beginning or at the end (char '*' means any symbols)\n" +
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
                    Comparer = new EntityComparerToPattern(entityId, entityIdType, entityNameType);
                }
            }
        }

        [Description("Check Entity's Region:\n" +
            "True: Search an Entity only if it located in the same Region as Player\n" +
            "False: Does not consider the region when searching Entities")]
        [Category("Entity optional checks")]
        public bool RegionCheck { get; set; } = false;

        [Description("Check if Entity's health greater than zero:\n" +
            "True: Only Entities with nonzero health are detected\n" +
            "False: Entity's health does not checked during search")]
        [Category("Entity optional checks")]
        public bool HealthCheck { get; set; } = true;

        [Description("A subset of entities that are searched for a target\n" +
            "Contacts: Only interactable Entities\n" +
            "Complete: All possible Entities")]
        [Editor(typeof(MultiCustomRegionSelectEditor), typeof(UITypeEditor))]
        [Category("Entity optional checks")]
        public EntitySetType EntitySetType { get; set; } = EntitySetType.Complete;

        [Description("The maximum distance from the character within which the Entity is searched\n" +
            "The default value is 0, which disables distance checking")]
        [Category("Entity optional checks")]
        public float ReactionRange { get; set; } = 0;

        [Description("CustomRegion names collection")]
        [Editor(typeof(MultiCustomRegionSelectEditor), typeof(UITypeEditor))]
        [Category("Optional")]
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


        [XmlIgnore]
        Entity closestEntity = null;
        [XmlIgnore]
        private Timeout timeout = new Timeout(100);
        [XmlIgnore]
        private EntityComparerToPattern Comparer = null;
        [XmlIgnore]
        private string entityId = string.Empty;
        [XmlIgnore]
        private EntityNameType entityNameType = EntityNameType.NameUntranslated;
        [XmlIgnore]
        private ItemFilterStringType entityIdType = ItemFilterStringType.Simple;
        [XmlIgnore]
        private List<string> customRegionNames = null;
        [XmlIgnore]
        private List<CustomRegion> customRegions = null;


        public override bool IsValid
        {
            get
            {
                if (timeout.IsTimedOut)
                {
                    if (!string.IsNullOrEmpty(EntityID))
                        closestEntity = SearchCached.FindClosestEntity(entityId, entityIdType, entityNameType, EntitySetType,
                                                                HealthCheck, ReactionRange, RegionCheck, customRegions);

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
                                    return result = (closestEntity != null) && closestEntity.IsValid && (closestEntity.Location.Z == Value);
                                case Relation.NotEqual:
                                    return result = (closestEntity != null) && closestEntity.IsValid && (closestEntity.Location.Z != Value);
                                case Relation.Inferior:
                                    return result = (closestEntity != null) && closestEntity.IsValid && (closestEntity.Location.Z < Value);
                                case Relation.Superior:
                                    return result = (closestEntity != null) && closestEntity.IsValid && (closestEntity.Location.Z > Value);
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
                closestEntity = SearchCached.FindClosestEntity(entityId, entityIdType, entityNameType, EntitySetType,
                                                        HealthCheck, 0, RegionCheck, customRegions);

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
                    sb.Append(" [").Append(EntityID).Append(']').AppendLine();
                    if (PropertyType == EntityPropertyType.Distance)
                        sb.AppendLine("The distance to the missing Entity is considered equal to infinity.");
                    return sb.ToString();
                }
            }
        }

        private bool Validate(Entity e)
        {
            return e != null && Comparer.Check(e);
        }


        public override void Reset() { }
        public override string ToString()
        {
            return $"Entity [{entityId}] {PropertyType} {Sign} to {Value}";
        }

        public EntityProperty() { }
    }
}