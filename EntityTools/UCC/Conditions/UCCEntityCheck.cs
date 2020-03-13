using Astral.Classes.ItemFilter;
using Astral.Logic.UCC.Classes;
using EntityTools.Editors;
using EntityTools.Tools;
using MyNW.Classes;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;
using EntityTools.Enums;
using System;
using EntityTools.UCC.Extensions;
using System.Text;

namespace EntityTools.UCC.Conditions
{
    public class UCCEntityCheck : UCCCondition, ICustomUCCCondition
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
                        Comparer = EntityToPatternComparer.Get(entityId, entityIdType, entityNameType);
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
                        Comparer = EntityToPatternComparer.Get(entityId, entityIdType, entityNameType);
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
                        Comparer = EntityToPatternComparer.Get(entityId, entityIdType, entityNameType);
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

        [Description("The maximum distance from the character within which the Entity is searched\n" +
            "The default value is 0, which disables distance checking")]
        [Category("Optional")]
        public float ReactionRange { get; set; } = 0;

        [Description("The maximum ZAxis difference from the withing which the Entity is searched\n" +
            "The default value is 0, which disables ZAxis checking")]
        [Category("Optional")]
        public float ReactionZRange { get; set; } = 0;

        [Description("Aura which checked on the Entity")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Optional")]
        public AuraOption Aura { get; set; } = new AuraOption();

        public EntityPropertyType PropertyType { get; set; } = EntityPropertyType.Distance;

        public float PropertyValue { get; set; } = 0;

        [XmlIgnore]
        [Editor(typeof(EntityTestEditor), typeof(UITypeEditor))]
        [Description("Нажми на кнопку '...' чтобы увидеть тестовую информацию")]
        //[Category("Entity")]
        public string TestInfo { get; } = "Нажми '...' =>";

        #region ICustomUCCCondition
        bool ICustomUCCCondition.IsOK(UCCAction refAction/* = null*/)
        {
            Entity closestEntity = refAction?.GetTarget();

            if (Comparer == null && !string.IsNullOrEmpty(entityId))
                Comparer = EntityToPatternComparer.Get(entityId, entityIdType, entityNameType);

            if (Comparer != null)//(!string.IsNullOrEmpty(EntityID))
            {
                //Entity closestEntity = EntitySelectionTools.FindClosestEntity(EntityManager.GetEntities(), EntityID, EntityIdType, EntityNameType, HealthCheck, ReactionRange, RegionCheck, null);
                if(!Validate(closestEntity))
                    closestEntity = SearchCached.FindClosestEntity(entityId, entityIdType, entityNameType, EntitySetType.Complete, 
                                                                   HealthCheck, ReactionRange,
                                                                   (ReactionZRange > 0) ? ReactionZRange : Astral.Controllers.Settings.Get.MaxElevationDifference, 
                                                                   RegionCheck, null, Aura.Checker);

                bool result = false;
                if (Validate(closestEntity))
                {
                    switch (PropertyType)
                    {
                        case EntityPropertyType.Distance:
                            switch (Sign)
                            {
                                case Astral.Logic.UCC.Ressources.Enums.Sign.Equal:
                                    return result = closestEntity.Location.Distance3DFromPlayer == PropertyValue;
                                case Astral.Logic.UCC.Ressources.Enums.Sign.NotEqual:
                                    return result = closestEntity.Location.Distance3DFromPlayer != PropertyValue;
                                case Astral.Logic.UCC.Ressources.Enums.Sign.Inferior:
                                    return result = closestEntity.Location.Distance3DFromPlayer < PropertyValue;
                                case Astral.Logic.UCC.Ressources.Enums.Sign.Superior:
                                    return result = closestEntity.Location.Distance3DFromPlayer > PropertyValue;
                            }
                            break;
                        case EntityPropertyType.HealthPercent:
                            switch (Sign)
                            {
                                case Astral.Logic.UCC.Ressources.Enums.Sign.Equal:
                                    return result = closestEntity.Character?.AttribsBasic?.HealthPercent == PropertyValue;
                                case Astral.Logic.UCC.Ressources.Enums.Sign.NotEqual:
                                    return result = closestEntity.Character?.AttribsBasic?.HealthPercent != PropertyValue;
                                case Astral.Logic.UCC.Ressources.Enums.Sign.Inferior:
                                    return result = closestEntity.Character?.AttribsBasic?.HealthPercent < PropertyValue;
                                case Astral.Logic.UCC.Ressources.Enums.Sign.Superior:
                                    return result = closestEntity.Character?.AttribsBasic?.HealthPercent > PropertyValue;
                            }
                            break;
                        case EntityPropertyType.ZAxis:
                            switch (Sign)
                            {
                                case Astral.Logic.UCC.Ressources.Enums.Sign.Equal:
                                    return result = closestEntity.Location.Z == PropertyValue;
                                case Astral.Logic.UCC.Ressources.Enums.Sign.NotEqual:
                                    return result = closestEntity.Location.Z != PropertyValue;
                                case Astral.Logic.UCC.Ressources.Enums.Sign.Inferior:
                                    return result = closestEntity.Location.Z < PropertyValue;
                                case Astral.Logic.UCC.Ressources.Enums.Sign.Superior:
                                    return result = closestEntity.Location.Z > PropertyValue;
                            }
                            break;
                    }
                }
                // Если Entity не найдено, условие будет истино в единственном случае:
                else return PropertyType == EntityPropertyType.Distance 
                            && Sign == Astral.Logic.UCC.Ressources.Enums.Sign.Superior;
            }

            return false;
        }

        bool ICustomUCCCondition.Loked { get => base.Locked; set => base.Locked = value; }

        string ICustomUCCCondition.TestInfos(UCCAction refAction)
        {
            Entity closestEntity = refAction?.GetTarget();

            if (Comparer == null && !string.IsNullOrEmpty(entityId))
                Comparer = EntityToPatternComparer.Get(entityId, entityIdType, entityNameType);

            if (Comparer != null)//(!string.IsNullOrEmpty(EntityID))
            {
                //Entity closestEntity = EntitySelectionTools.FindClosestEntity(EntityManager.GetEntities(), EntityID, EntityIdType, EntityNameType, HealthCheck, ReactionRange, RegionCheck, null);
                if (!Validate(closestEntity))
                    closestEntity = SearchCached.FindClosestEntity(entityId, entityIdType, entityNameType, EntitySetType.Complete,
                                                                   HealthCheck, ReactionRange, ReactionZRange, RegionCheck, null, Aura.Checker);

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
            return "Condition options is invalid!";
        }
        #endregion

        [XmlIgnore]
        [Browsable(false)]
        internal Predicate<Entity> Comparer { get; private set; } = null;

        private bool Validate(Entity e)
        {
            return e != null && e.IsValid && Comparer?.Invoke(e) == true;
        }

        public override string ToString()
        {
            return $"EntityCheck [{EntityID}]";
        }

        public UCCEntityCheck()
        {
            Sign = Astral.Logic.UCC.Ressources.Enums.Sign.Superior;
        }

        [NonSerialized]
        private string entityId = string.Empty;
        [NonSerialized]
        private ItemFilterStringType entityIdType = ItemFilterStringType.Simple;
        [NonSerialized]
        private EntityNameType entityNameType = EntityNameType.NameUntranslated;
        

        #region Hide Inherited Properties
        [XmlIgnore]
        [Browsable(false)]
        public new string Value { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public new Astral.Logic.UCC.Ressources.Enums.Unit Target { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public new Astral.Logic.UCC.Ressources.Enums.ActionCond Tested { get; set; }
        #endregion
    }
}
