using Astral.Classes.ItemFilter;
using Astral.Quester.Classes;
using EntityPlugin.Editors;
using EntityPlugin.Tools;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace EntityPlugin.Conditions
{
    [Serializable]
    public class EntityDistance : Condition
    {
        public EntityDistance()
        {
            EntityID = string.Empty;
            Distance = 0;
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
        public float Distance { get; set; }

        [Description("Check Entity's Region:\n" +
            "True: Count Entity if it located in the same Region as Player\n" +
            "False: Does not consider the region when counting Entities")]
        [Category("Tested")]
        public bool RegionCheck { get; set; }

        [Description("Distance comparison type to the closest Entity")]
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
                    switch (Sign)
                    {
                        case Relation.Equal:
                            return result = (closestEntity != null) && closestEntity.IsValid && (closestEntity.Location.Distance3DFromPlayer == Distance);
                        case Relation.NotEqual:
                            return result = (closestEntity != null) && closestEntity.IsValid && (closestEntity.Location.Distance3DFromPlayer != Distance);
                        case Relation.Inferior:
                            return result = (closestEntity != null) && closestEntity.IsValid && (closestEntity.Location.Distance3DFromPlayer < Distance);
                        case Relation.Superior:
                            return result = (closestEntity == null) || !closestEntity.IsValid || (closestEntity.Location.Distance3DFromPlayer > Distance);
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
            return $"Entity [{EntityID}] VisibilityDistance {Sign} to {Distance}";
        }

        public override string TestInfos
        {
            get
            {
                Entity closestEntity = SelectionTools.FindClosestEntity(EntityManager.GetEntities(), EntityID, EntityIdType, RegionCheck);

                if (closestEntity.IsValid)
                {
                    return $"Found closect Entity [{closestEntity.NameUntranslated}] at the {nameof(Distance)} = {closestEntity.Location.Distance3DFromPlayer}";
                }
                else
                {
                    return $"No one Entity matched to [{EntityID}]";
                }
            }
        }
    }
}