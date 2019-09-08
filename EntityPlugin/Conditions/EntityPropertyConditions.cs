using Astral.Classes.ItemFilter;
using Astral.Quester.Classes;
using EntityTools.Editors;
using EntityTools.Tools;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;

namespace EntityTools.Conditions
{
    [Serializable]
    public class EntityProperty : Condition
    {
        public EntityProperty()
        {
            EntityID = string.Empty;
            PropertyType = EntityPropertType.Distance;
            Value = 0;
            Sign = Relation.Superior;
            EntityIdType = ItemFilterStringType.Simple;
            RegionCheck = false;
        }

        [Description("Type of and EntityID:\n" +
            "Simple: Simple test string with a mask (char '*' means any chars)\n" +
            "Regex: Regular expression")]
        [Category("Entity")]
        public ItemFilterStringType EntityIdType { get; set; }

        [Description("ID (an untranslated name) of the Entity for the search")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        [Category("Entity")]
        public string EntityID { get; set; }

        [Category("Tested")]
        public EntityPropertType PropertyType { get; set; }

        [Category("Tested")]
        public float Value { get; set; }

        [Description("Check Entity's Region:\n" +
            "True: Search an Entity only if it located in the same Region as Player\n" +
            "False: Does not consider the region when searching Entities")]
        [Category("Tested")]
        public bool RegionCheck { get; set; }

        [Description("Value comparison type to the closest Entity")]
        [Category("Tested")]
        public Condition.Relation Sign { get; set; }

        public override bool IsValid
        {
            get
            {
                if (!string.IsNullOrEmpty(EntityID))
                {
                    Entity closestEntity = SelectionTools.FindClosestEntity(EntityManager.GetEntities(), EntityID, EntityIdType, RegionCheck);

                    bool result = false;
                    switch (PropertyType)
                    {
                        case EntityPropertType.Distance:
                            switch (Sign)
                            {
                                case Relation.Equal:
                                    return result = (closestEntity != null) && closestEntity.IsValid && (closestEntity.Location.Distance3DFromPlayer == Value);
                                case Relation.NotEqual:
                                    return result = (closestEntity != null) && closestEntity.IsValid && (closestEntity.Location.Distance3DFromPlayer != Value);
                                case Relation.Inferior:
                                    return result = (closestEntity != null) && closestEntity.IsValid && (closestEntity.Location.Distance3DFromPlayer < Value);
                                case Relation.Superior:
                                    return result = (closestEntity == null) || !closestEntity.IsValid || (closestEntity.Location.Distance3DFromPlayer > Value);
                            }
                            break;
                        case EntityPropertType.HealthPercent:
                            switch (Sign)
                            {
                                case Relation.Equal:
                                    return result = (closestEntity != null) && closestEntity.IsValid && (closestEntity.Character?.AttribsBasic?.HealthPercent == Value);
                                case Relation.NotEqual:
                                    return result = (closestEntity != null) && closestEntity.IsValid && (closestEntity.Character?.AttribsBasic?.HealthPercent != Value);
                                case Relation.Inferior:
                                    return result = (closestEntity != null) && closestEntity.IsValid && (closestEntity.Character?.AttribsBasic?.HealthPercent < Value);
                                case Relation.Superior:
                                    return result = (closestEntity != null) && closestEntity.IsValid && (closestEntity.Character?.AttribsBasic?.HealthPercent > Value);
                            }
                            break;
                        case EntityPropertType.ZAxis:
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

                return false;
            }
        }

        public override void Reset()
        {
        }

        public override string ToString()
        {
            return $"Entity [{EntityID}] {PropertyType} {Sign} to {Value}";
        }

        public override string TestInfos
        {
            get
            {
                Entity closestEntity = SelectionTools.FindClosestEntity(EntityManager.GetEntities(), EntityID, EntityIdType, RegionCheck);

                if (closestEntity.IsValid)
                {
                    StringBuilder sb = new StringBuilder("Found closect Entity");
                    sb.Append(" [").Append(closestEntity.NameUntranslated).Append(']').Append(" which ").Append(PropertyType).Append(" = ");
                    switch(PropertyType)
                    {
                        case EntityPropertType.Distance:
                            sb.Append(closestEntity.Location.Distance3DFromPlayer);
                            break;
                        case EntityPropertType.ZAxis:
                            sb.Append(closestEntity.Location.Z);
                            break;
                        case EntityPropertType.HealthPercent:
                            sb.Append(closestEntity.Character?.AttribsBasic?.HealthPercent);
                            break;
                    }
                    return sb.ToString();
                }
                else
                {
                    StringBuilder sb = new StringBuilder("No one Entity matched to");
                    sb.Append(" [").Append(EntityID).Append(']');
                    if (PropertyType == EntityPropertType.Distance)
                        sb.AppendLine("The distance to the missing Entity is considered equal to infinity.");
                    return sb.ToString();
                }
            }
        }
    }

    public enum EntityPropertType
        {
            Distance,
            ZAxis,
            HealthPercent
        }
}