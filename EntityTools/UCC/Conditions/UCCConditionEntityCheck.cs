using Astral.Classes.ItemFilter;
using Astral.Logic.UCC.Classes;
using EntityTools.Editors;
using EntityTools.Tools;
using MyNW.Classes;
using MyNW.Internals;
using System.ComponentModel;
using System.Drawing.Design;
using static Astral.Quester.Classes.Condition;

namespace EntityTools.UCC.Conditions
{
    public class UCCConditionEntityCheck : CustomUCCCondition
    {
        [Description("ID (an untranslated name) of the Entity for the search")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        [Category("Entity")]
        public string EntityID { get; set; } = string.Empty;

        [Description("The switcher of the Entity filed which compared to the property EntityID")]
        [Category("Entity")]
        public EntityNameType EntityNameType { get; set; } = EntityNameType.NameUntranslated;

        [Description("Type of and EntityID:\n" +
            "Simple: Simple test string with a wildcard at the beginning or at the end (char '*' means any symbols)\n" +
            "Regex: Regular expression")]
        [Category("Entity")]
        public ItemFilterStringType EntityIdType { get; set; } = ItemFilterStringType.Simple;

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

        [Description("The maximum distance from the character within which the Entity is searched\n" +
            "The default value is 0, which disables distance checking")]
        [Category("Entity optional checks")]
        public float ReactionRange { get; set; } = 0;

        //[Description("CustomRegion names collection")]
        //[Editor(typeof(MultiCustomRegionSelectEditor), typeof(UITypeEditor))]
        //[Category("Entity optional checks")]
        //public List<string> CustomRegionNames { get; set; } = new List<string>();

        [Category("Tested")]
        public EntityPropertyType PropertyType { get; set; } = EntityPropertyType.Distance;

        [Category("Tested")]
        public float Value { get; set; } = 0;

        [Description("Value comparison type to the closest Entity")]
        [Category("Tested")]
        public Relation Sign { get; set; } = Relation.Superior;

        [Browsable(false)]
        public override bool IsOK(UCCAction refAction = null)
        {
            if (!string.IsNullOrEmpty(EntityID))
            {
                Entity closestEntity = EntitySelectionTools.FindClosestEntity(EntityManager.GetEntities(), EntityID, EntityIdType, EntityNameType, HealthCheck, ReactionRange, RegionCheck, null);

                bool result = false;
                switch (PropertyType)
                {
                    case EntityPropertyType.Distance:
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
                    case EntityPropertyType.HealthPercent:
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

            return false;
        }

        public override string ToString()
        {
            return $"EntityCheck [{EntityID}]";
        }
    }
}
