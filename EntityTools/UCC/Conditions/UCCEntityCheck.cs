using Astral.Classes.ItemFilter;
using Astral.Logic.UCC.Classes;
using EntityTools.Editors;
using EntityTools.Tools;
using MyNW.Classes;
using MyNW.Internals;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;
using static Astral.Quester.Classes.Condition;
using Astral.Logic.UCC.Ressources;
using EntityTools.Enums;
using System;
using EntityTools.Tools.Entities;
using EntityTools.Tools.UCCExtensions;

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
                        Comparer = new EntityComparerToPattern(entityId, entityIdType, entityNameType);
                    else Comparer = null;
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
        bool ICustomUCCCondition.IsOk(UCCAction refAction = null)
        {
            Entity closestEntity = refAction?.GetTarget();            

            if (!string.IsNullOrEmpty(EntityID))
            {
                //Entity closestEntity = EntitySelectionTools.FindClosestEntity(EntityManager.GetEntities(), EntityID, EntityIdType, EntityNameType, HealthCheck, ReactionRange, RegionCheck, null);
                if(closestEntity != null)
                    closestEntity = SearchCached.FindClosestEntity(EntityID, EntityIdType, EntityNameType, EntitySetType.Complete, 
                                                                   HealthCheck, ReactionRange, RegionCheck, null, Aura.Checker);

                bool result = false;
                switch (PropertyType)
                {
                    case EntityPropertyType.Distance:
                        //if(float.TryParse(Value, out float distance))
                        switch (Sign)
                        {
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Equal:
                                return result = (closestEntity != null) && closestEntity.IsValid && (closestEntity.Location.Distance3DFromPlayer == PropertyValue);
                            case Astral.Logic.UCC.Ressources.Enums.Sign.NotEqual:
                                return result = (closestEntity != null) && closestEntity.IsValid && (closestEntity.Location.Distance3DFromPlayer != PropertyValue);
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Inferior:
                                return result = (closestEntity != null) && closestEntity.IsValid && (closestEntity.Location.Distance3DFromPlayer < PropertyValue);
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Superior:
                                return result = (closestEntity == null) || !closestEntity.IsValid || (closestEntity.Location.Distance3DFromPlayer > PropertyValue);
                        }
                        break;
                    case EntityPropertyType.HealthPercent:
                        //if (float.TryParse(Value, out float hp))
                        switch (Sign)
                        {
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Equal:
                                return result = (closestEntity != null) && closestEntity.IsValid && (closestEntity.Character?.AttribsBasic?.HealthPercent == PropertyValue);
                            case Astral.Logic.UCC.Ressources.Enums.Sign.NotEqual:
                                return result = (closestEntity != null) && closestEntity.IsValid && (closestEntity.Character?.AttribsBasic?.HealthPercent != PropertyValue);
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Inferior:
                                return result = (closestEntity != null) && closestEntity.IsValid && (closestEntity.Character?.AttribsBasic?.HealthPercent < PropertyValue);
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Superior:
                                return result = (closestEntity != null) && closestEntity.IsValid && (closestEntity.Character?.AttribsBasic?.HealthPercent > PropertyValue);
                        }
                        break;
                    case EntityPropertyType.ZAxis:
                        //if (float.TryParse(Value, out float z))
                        switch (Sign)
                        {
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Equal:
                                return result = (closestEntity != null) && closestEntity.IsValid && (closestEntity.Location.Z == PropertyValue);
                            case Astral.Logic.UCC.Ressources.Enums.Sign.NotEqual:
                                return result = (closestEntity != null) && closestEntity.IsValid && (closestEntity.Location.Z != PropertyValue);
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Inferior:
                                return result = (closestEntity != null) && closestEntity.IsValid && (closestEntity.Location.Z < PropertyValue);
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Superior:
                                return result = (closestEntity != null) && closestEntity.IsValid && (closestEntity.Location.Z > PropertyValue);
                        }
                        break;
                }
            }

            return false;
        }

        bool ICustomUCCCondition.Loked { get => base.Locked; set => base.Locked = value; }
        #endregion

        [XmlIgnore]
        [Browsable(false)]
        internal EntityComparerToPattern Comparer { get; private set; } = null;

        private bool Validate(Entity e)
        {
            return e != null && e.IsValid && Comparer.Check(e);
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
