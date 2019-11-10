using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Xml.Serialization;
using Astral;
using Astral.Classes;
using Astral.Classes.ItemFilter;
using Astral.Logic.Classes.Map;
using EntityTools.Editors;
using EntityTools.Tools;
using MyNW.Classes;
using MyNW.Internals;
using Action = Astral.Quester.Classes.Action;

namespace EntityTools.Actions
{
    [Serializable]
    public class MoveToEntity : Astral.Quester.Classes.Action
    {
        [NonSerialized]
        internal Entity target = new Entity(IntPtr.Zero);

        //[NonSerialized]
        //[Description("Period of time until the closest entity is searched again")]
        //protected readonly int SearchTimeout = 1000;

        [Description("ID of the Entity for the search")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        [Category("Entity")]
        public string EntityID { get; set; } = string.Empty;

        [Description("Type of and EntityID:\n" +
            "Simple: Simple test string with a wildcard at the beginning or at the end (char '*' means any symbols)\n" +
            "Regex: Regular expression")]
        [Category("Entity")]
        public ItemFilterStringType EntityIdType { get; set; } = ItemFilterStringType.Simple;

        [Description("The switcher of the Entity filed which compared to the property EntityID")]
        [Category("Entity")]
        public EntityNameType EntityNameType { get; set; } = EntityNameType.NameUntranslated;

        [Description("Check Entity's Ingame Region (Not CustomRegion):\n" +
            "True: Only Entities located in the same Region as Player are detected\n" +
            "False: Entity's Region does not checked during search")]
        [Category("Entity optional checks")]
        public bool RegionCheck { get; set; } = false;

        [Description("Check if Entity's health greater than zero:\n" +
            "True: Only Entities with nonzero health are detected\n" +
            "False: Entity's health does not checked during search")]
        [Category("Entity optional checks")]
        public bool HealthCheck { get; set; } = true;

        [Description("True: Do not change the target Entity while it is alive or until the Bot within 'Distance' of it\n" +
                    "False: Constantly scan an area and target the nearest Entity")]
        [Category("Entity optional checks")]
        public bool HoldTargetEntity { get; set; } = true;

        [Description("The maximum distance from the character within which the Entity is searched\n" +
            "The default value is 0, which disables distance checking")]
        [Category("Entity optional checks")]
        public float ReactionRange { get; set; } = 0;

        [Description("CustomRegion names collection")]
        [Editor(typeof(MultiCustomRegionSelectEditor), typeof(UITypeEditor))]
        [Category("Entity optional checks")]
        public List<string> CustomRegionNames { get; set; } = new List<string>();

        [Description("Distance to the Entity by which it is necessary to approach")]
        [Category("Interruptions")]
        public float Distance { get; set; } = 30;

        [Description("Enable 'IgnoreCombat' profile value while playing action")]
        [Category("Interruptions")]
        public bool IgnoreCombat { get; set; } = true;

        //[Description("True: Clear the list the enemies (cache) attacking the player before engage combat")]
        //[Category("Interruptions")]
        //public bool ClearAttackersCache { get; set; } = true;

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
                // Поиск Entity по таймеру
                //if (string.IsNullOrEmpty(EntityID))
                //    target = new Entity(IntPtr.Zero);
                //else if (target == null || !target.IsValid || timer.IsTimedOut)
                //{
                //    target = SelectionTools.FindClosestEntity(EntityManager.GetEntities(), EntityID, EntityIdType, RegionCheck);
                //    timer.Reset();
                //}

                // Поиск Entity при каждой проверке
                //Команда работает с 2 - мя целями:
                //1 - я цель (target) определяет навигацию. Если она фиксированная(HoldTargetEntity), то не сбрасывается пока жива и не достигнута
                //2 - я ближайшая цель (closest) управляет флагом IgnoreCombat
                //Если HoldTargetEntity ВЫКЛЮЧЕН, то обе цели совпадают - это ближайшая цель 

                Entity closestEntity = EntitySelectionTools.FindClosestEntity(EntityManager.GetEntities(), EntityID, EntityIdType, EntityNameType, HealthCheck, ReactionRange, RegionCheck, CustomRegionNames);

                if (!HoldTargetEntity || target == null || !target.IsValid || (HealthCheck && target.IsDead))
                        target = closestEntity;

                if (IgnoreCombat && closestEntity != null && closestEntity.IsValid
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
                else if(IgnoreCombat)
                    Astral.Quester.API.IgnoreCombat = true;

                return (target != null && target.IsValid && (target.Location.Distance3DFromPlayer < Distance));
            }
        }

        public override ActionResult Run()
        {
            if (!target.IsValid)
            {
                Logger.WriteLine($"Entity [{EntityID}] not founded.");
                return ActionResult.Fail;
            }

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

            // Вариант реализации со сбросом флага IgnoreCombat в NeedToRun
            if (StopOnApproached)
                return ActionResult.Completed;
            else return ActionResult.Running;
        }

        public MoveToEntity() { }
        public override string ActionLabel => $"{GetType().Name} [{EntityID}]";
        public override void OnMapDraw(GraphicsNW graph)
        {
            if (target.IsValid && target.Location.IsValid)
            {
                Brush beige = Brushes.Beige;
                graph.drawFillEllipse(target.Location, new Size(10, 10), beige);
            }
        }
        public override void InternalReset() { }
        protected override bool IntenalConditions => !string.IsNullOrEmpty(EntityID);
        public override string InternalDisplayName => GetType().Name;
        public override bool UseHotSpots => true;
        protected override Vector3 InternalDestination
        {
            get
            {
                if (target != null && target.IsValid)
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
                if (string.IsNullOrEmpty(EntityID))
                {
                    return new ActionValidity($"Рroperty '{nameof(EntityID)}' not set.");
                }
                return new ActionValidity();
            }
        }
        public override void GatherInfos() { }
    }
}
