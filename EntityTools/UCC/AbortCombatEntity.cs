using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using Astral.Classes;
using Astral.Classes.ItemFilter;
using Astral.Controllers;
using Astral.Logic.NW;
using Astral.Logic.UCC.Actions;
using Astral.Logic.UCC.Classes;
using Astral.Quester.FSM.States;
using EntityTools;
using EntityTools.Editors;
using EntityTools.Tools;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityTools.UCC
{
    public class AbortCombatEntity : UCCAction
    {
        public AbortCombatEntity()
        {
            Target = Astral.Logic.UCC.Ressources.Enums.Unit.Player;
        }

        public override UCCAction Clone()
        {
            return base.BaseClone(new AbortCombatEntity
            {
                IgnoreCombatMinHP = this.IgnoreCombatMinHP,
                IgnoreCombatTime = this.IgnoreCombatTime,
                EntityID = this.EntityID,
                EntityIdType = this.EntityIdType,
                EntityNameType = this.EntityNameType,
                RegionCheck = this.RegionCheck,
                HealthCheck = this.HealthCheck,
                EntityDistance = this.EntityDistance,
                DistanceSign = this.DistanceSign
            });
        }

        public override bool NeedToRun
        {
            get
            {
                if (!string.IsNullOrEmpty(EntityID))
                {
                    entity = EntitySelectionTools.FindClosestEntity(EntityManager.GetEntities(), EntityID, EntityIdType, EntityNameType, HealthCheck, Range, RegionCheck);

                    switch (DistanceSign)
                    {
                        case Astral.Logic.UCC.Ressources.Enums.Sign.Equal:
                            return entity.Location.Distance3DFromPlayer == EntityDistance;
                        case Astral.Logic.UCC.Ressources.Enums.Sign.NotEqual:
                            return entity.Location.Distance3DFromPlayer != EntityDistance;
                        case Astral.Logic.UCC.Ressources.Enums.Sign.Inferior:
                            return entity.Location.Distance3DFromPlayer < EntityDistance;
                        case Astral.Logic.UCC.Ressources.Enums.Sign.Superior:
                            return entity.Location.Distance3DFromPlayer > EntityDistance;
                    }
                }
                return false;
            }
        }

        public override bool Run()
        {
            if (!entity.IsValid && !(HealthCheck && entity.IsDead))
                return abordCombat.Run();
            return true;
        }

        public override string ToString() => GetType().Name;

        [Description("How many time ignore combat in seconds (0 for infinite)")]
        [Category("Interruption")]
        public int IgnoreCombatTime { get => abordCombat.IgnoreCombatTime; set => abordCombat.IgnoreCombatTime = value; }

        [Description("Minimum health percent to enable combat again")]
        [Category("Interruption")]
        public int IgnoreCombatMinHP { get => abordCombat.IgnoreCombatMinHP; set => abordCombat.IgnoreCombatMinHP = value; }

        [Description("ID (an untranslated name) of the Entity for the search (regex)")]
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
        [Category("Entity")]
        public bool RegionCheck { get; set; } = false;

        [Description("Check if Entity's health greater than zero:\n" +
            "True: Only Entities with nonzero health are detected\n" +
            "False: Entity's health does not checked during search")]
        [Category("Entity")]
        public bool HealthCheck { get; set; } = true;

        [Description("Distance to the closest Entity")]
        [Category("Required")]
        public float EntityDistance { get; set; } = 0;

        [Description("Distance comparison type to the closest Entity")]
        [Category("Required")]
        public Astral.Logic.UCC.Ressources.Enums.Sign DistanceSign { get; set; } = Astral.Logic.UCC.Ressources.Enums.Sign.Inferior;

        [Browsable(false)]
        public new string ActionName { get; set; }

        [NonSerialized]
        protected Entity entity = new Entity(IntPtr.Zero);
        [NonSerialized]
        protected AbordCombat abordCombat = new AbordCombat();
    }
}
