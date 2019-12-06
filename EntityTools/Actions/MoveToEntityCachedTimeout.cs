﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Xml.Serialization;
using Astral;
using Astral.Classes;
using Astral.Classes.ItemFilter;
using Astral.Logic.Classes.Map;
using Astral.Quester.Classes;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Tools;
using EntityTools.Tools.Entities;
using MyNW.Classes;
using MyNW.Internals;
using Action = Astral.Quester.Classes.Action;

namespace EntityTools.Actions
{
    [Serializable]
    public class MoveToEntity : Astral.Quester.Classes.Action
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
                    Comparer = new EntityComparerToPattern(entityId, entityIdType, entityNameType);
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
                if(entityNameType != value)
                { 
                    entityNameType = value;
                    Comparer = new EntityComparerToPattern(entityId, entityIdType, entityNameType);
                }
            }
        }

        [Description("Check Entity's Ingame Region (Not CustomRegion):\n" +
            "True: Only Entities located in the same Region as Player are detected\n" +
            "False: Entity's Region does not checked during search")]
        [Category("Optional")]
        public bool RegionCheck { get; set; } = false;

        [Description("Check if Entity's health greater than zero:\n" +
            "True: Only Entities with nonzero health are detected\n" +
            "False: Entity's health does not checked during search")]
        [Category("Optional")]
        public bool HealthCheck { get; set; } = true;

        [Description("True: Do not change the target Entity while it is alive or until the Bot within 'Distance' of it\n" +
                    "False: Constantly scan an area and target the nearest Entity")]
        [Category("Optional")]
        public bool HoldTargetEntity { get; set; } = true;

        [Description("The maximum distance from the character within which the Entity is searched\n" +
            "The default value is 0, which disables distance checking")]
        [Category("Optional")]
        public float ReactionRange { get; set; } = 0;

        [XmlIgnore]
        private List<string> customRegionNames = null;
        [XmlIgnore]
        private List<CustomRegion> customRegions = null;

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

        [Description("Time between searches of the Entity (ms)")]
        [Category("Optional")]
        public int SearchTimeInterval { get; set; } = 100;

        [Description("Distance to the Entity by which it is necessary to approach")]
        [Category("Interruptions")]
        public float Distance { get; set; } = 30;

        [Description("Enable 'IgnoreCombat' profile value while playing action")]
        [Category("Interruptions")]
        public bool IgnoreCombat { get; set; } = true;

        [Description("True: Complite an action when the object is closer than 'Distance'\n" +
                     "False: Follow an Entity regardless of its distance")]
        [Category("Interruptions")]
        public bool StopOnApproached { get; set; } = false;

        [Description("True: Clear the list of attackers and attack the target Entity when it is approached\n" +
            "This option is ignored if 'IgnoreCombat' does not set")]
        [Category("Interruptions")]
        public bool AttackTargetEntity { get; set; } = true;

        [XmlIgnore]
        [Editor(typeof(EntityTestEditor), typeof(UITypeEditor))]
        [Description("Нажми на кнопку '...' чтобы увидеть тестовую информацию")]
        public string TestInfo { get; } = "Нажми на кнопку '...' чтобы увидеть больше =>";

        public override bool NeedToRun
        {
            get
            {
                //Команда работает с 2 - мя целями:
                //1 - я цель (target) определяет навигацию. Если она фиксированная(HoldTargetEntity), то не сбрасывается пока жива и не достигнута
                //2 - я ближайшая цель (closest) управляет флагом IgnoreCombat
                //Если HoldTargetEntity ВЫКЛЮЧЕН, то обе цели совпадают - это ближайшая цель 

                Entity closestEntity = null;
                if (timeout.IsTimedOut || (target!= null && !Validate(target)))
                {
                    closestEntity = SearchCached.FindClosestEntity(entityId, entityIdType, entityNameType, EntitySetType.Complete,
                                                                HealthCheck, ReactionRange, RegionCheck, customRegions);
                    timeout.ChangeTime(SearchTimeInterval);
                }

                if (!HoldTargetEntity || !Validate(target) || (HealthCheck && target.IsDead))
                    target = closestEntity;

                if (IgnoreCombat && Validate(closestEntity)
                    && !(HealthCheck && closestEntity.IsDead)
                    && (closestEntity.Location.Distance3DFromPlayer <= Distance))
                {
                    Astral.Logic.NW.Attackers.List.Clear();
                    if (AttackTargetEntity)
                    {
                        Astral.Logic.NW.Attackers.List.Add(closestEntity);
                        Astral.Quester.API.IgnoreCombat = false;
                        Astral.Logic.NW.Combats.CombatUnit(closestEntity, null);
                    }
                    else Astral.Quester.API.IgnoreCombat = false;
                }
                else if (IgnoreCombat)
                    Astral.Quester.API.IgnoreCombat = true;

                return (Validate(target) && (target.Location.Distance3DFromPlayer < Distance));
            }
        }

        public override ActionResult Run()
        {
            //if (!Validate(target))
            //{
            //    Logger.WriteLine($"Entity [{EntityID}] not founded.");
            //    return ActionResult.Fail;
            //}

            // Вариант реализации со сбросом флага IgnoreCombat в Run()
            //if (target.Location.Distance3DFromPlayer < Distance)
            //{
            //    Astral.Quester.API.IgnoreCombat = false;

            //    if (StopOnApproached)
            //        return ActionResult.Completed;
            //    else return ActionResult.Running;
            //}
            //else
            //{
            //    if (IgnoreCombat)
            //        Astral.Quester.API.IgnoreCombat = IgnoreCombat;
            //    return ActionResult.Running;
            //}

            if (IgnoreCombat)
            {
                Astral.Logic.NW.Attackers.List.Clear();
                if (AttackTargetEntity)
                {
                    Astral.Logic.NW.Attackers.List.Add(target);
                    Astral.Quester.API.IgnoreCombat = false;
                    Astral.Logic.NW.Combats.CombatUnit(target, null);
                }
                else Astral.Quester.API.IgnoreCombat = false;
            }

            // Вариант реализации со сбросом флага IgnoreCombat в NeedToRun
            if (StopOnApproached)
            return ActionResult.Completed;
            else return ActionResult.Running;
        }

        private bool Validate(Entity e)
        {
            return e != null && Comparer.Check(e);
        }

        [XmlIgnore]
        private Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(0);
        [XmlIgnore]
        private string entityId = string.Empty;
        [XmlIgnore]
        private ItemFilterStringType entityIdType = ItemFilterStringType.Simple;
        [XmlIgnore]
        private EntityNameType entityNameType = EntityNameType.NameUntranslated;
        [XmlIgnore]
        internal Entity target = new Entity(IntPtr.Zero);
        [XmlIgnore]
        private EntityComparerToPattern Comparer = null;

        public MoveToEntity() { }
        public override string ActionLabel => $"{GetType().Name} [{entityId}]";
        public override void OnMapDraw(GraphicsNW graph)
        {
            if (Validate(target) && target.Location.IsValid)
            {
                Brush beige = Brushes.Beige;
                graph.drawFillEllipse(target.Location, new Size(10, 10), beige);
            }
        }
        public override void InternalReset() { }
        protected override bool IntenalConditions => !string.IsNullOrEmpty(entityId);
        public override string InternalDisplayName => string.Empty;
        public override bool UseHotSpots => true;
        protected override Vector3 InternalDestination
        {
            get
            {
                if (Validate(target))
                {
                    if (target.Location.Distance3DFromPlayer > Distance)
                        return target.Location.Clone();
                    else return EntityManager.LocalPlayer.Location.Clone();
                }
                return new Vector3();
            }
        }
        protected override ActionValidity InternalValidity
        {
            get
            {
                if (string.IsNullOrEmpty(entityId))
                {
                    return new ActionValidity($"EntityID property not set.");
                }
                return new ActionValidity();
            }
        }
        public override void GatherInfos() { }
    }
}
