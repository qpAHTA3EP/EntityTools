using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;
using Astral.Classes;
using Astral.Classes.ItemFilter;
using Astral.Controllers;
using Astral.Logic.NW;
using Astral.Logic.UCC.Actions;
using Astral.Logic.UCC.Classes;
using Astral.Quester.FSM.States;
using EntityTools;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Tools;
using EntityTools.Tools.Entities;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityTools.UCC
{
    [Serializable]
    public class ApproachEntity : UCCAction
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

        [Category("Entity")]
        public float EntityRadius { get; set; } = 12;

        [Description("Check Entity's Ingame Region (Not CustomRegion):\n" +
            "True: Only Entities located in the same Region as Player are detected\n" +
            "False: Entity's Region does not checked during search")]
        //[Category("Entity")]
        [Category("Optional")]
        public bool RegionCheck { get; set; } = true;

        [Description("Check if Entity's health greater than zero:\n" +
            "True: Only Entities with nonzero health are detected\n" +
            "False: Entity's health does not checked during search")]
        //[Category("Entity")]
        [Category("Optional")]
        public bool HealthCheck { get; set; } = true;

        [Description("Aura which checked on the Entity")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        //[Category("Entity")]
        [Category("Optional")]
        public AuraOption Aura { get; set; } = new AuraOption();

        [Description("The maximum distance from the character within which the Entity is searched\n" +
            "The default value is 0, which disables distance checking")]
        //[Category("Entity")]
        [Category("Optional")]
        public float ReactionRange { get; set; } = 30;

        [Description("The maximum ZAxis difference from the withing which the Entity is searched\n" +
            "The default value is 0, which disables ZAxis checking")]
        [Category("Optional")]
        public float ReactionZRange { get; set; } = 0;

        [XmlIgnore]
        [Editor(typeof(EntityTestEditor), typeof(UITypeEditor))]
        [Description("Нажми на кнопку '...' чтобы увидеть тестовую информацию")]
        //[Category("Entity")]
        public string TestInfo { get; } = "Нажми '...' =>";

        public override bool NeedToRun
        {
            get
            {
                if (!string.IsNullOrEmpty(EntityID))
                {
                    if (Comparer == null && !string.IsNullOrEmpty(entityId))
                        Comparer = new EntityComparerToPattern(entityId, entityIdType, entityNameType);

                    //entity = EntitySelectionTools.FindClosestEntity(EntityManager.GetEntities(), EntityID, EntityIdType, EntityNameType, HealthCheck, Range, RegionCheck);
                    entity = SearchCached.FindClosestEntity(EntityID, EntityIdType, EntityNameType, EntitySetType.Complete, 
                                                            HealthCheck, ReactionRange, ReactionZRange, RegionCheck, null, Aura.Checker);
                    return Validate(entity) && !(HealthCheck && entity.IsDead) && entity.CombatDistance > EntityRadius;
                }
                return false;
            }
        }

        public override bool Run()
        {
            return Approach.EntityByDistance(entity, EntityRadius);
        }

        [XmlIgnore]
        [Browsable(false)]
        internal EntityComparerToPattern Comparer { get; private set; } = null;

        [XmlIgnore]
        [Browsable(false)]
        public Entity UnitRef
        {
            get
            {
                if (Validate(entity))
                    return entity;
                else
                {
                    if (!string.IsNullOrEmpty(EntityID))
                    {
                        entity = SearchCached.FindClosestEntity(EntityID, EntityIdType, EntityNameType, EntitySetType.Complete,
                                                                HealthCheck, ReactionRange, ReactionRange, RegionCheck, null, Aura.Checker);
                        return entity;
                    }
                }
                return null;
            }
        }

        private bool Validate(Entity e)
        {
            return e != null && e.IsValid && Comparer.Check(e);
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(EntityID))
                return GetType().Name;
            else return GetType().Name + " [" + EntityID + ']';
        }

        public ApproachEntity()
        {
            Target = Astral.Logic.UCC.Ressources.Enums.Unit.Player;
        }
        public override UCCAction Clone()
        {
            return base.BaseClone(new ApproachEntity
            {
                entityId = this.entityId,
                entityIdType = this.entityIdType,
                entityNameType = this.entityNameType,
                RegionCheck = this.RegionCheck,
                HealthCheck = this.HealthCheck,
                EntityRadius = this.EntityRadius,
                Aura = new AuraOption
                {
                    AuraName = this.Aura.AuraName,
                    AuraNameType = this.Aura.AuraNameType,
                    Sign = this.Aura.Sign,
                    Stacks = this.Aura.Stacks
                }
            });
        }

        [NonSerialized]
        private string entityId = string.Empty;
        [NonSerialized]
        private ItemFilterStringType entityIdType = ItemFilterStringType.Simple;
        [NonSerialized]
        private EntityNameType entityNameType = EntityNameType.NameUntranslated;
        [NonSerialized]
        protected Entity entity = new Entity(IntPtr.Zero);

        #region Hide Inherited Properties
        [XmlIgnore]
        [Browsable(false)]
        public new Astral.Logic.UCC.Ressources.Enums.Unit Target { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public new string ActionName { get; set; } = string.Empty;
        #endregion
    }
}
