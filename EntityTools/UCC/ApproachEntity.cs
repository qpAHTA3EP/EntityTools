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
using EntityTools.Tools;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityTools.UCC
{
    [Serializable]
    public class ApproachEntity : UCCAction
    {
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
        public bool RegionCheck { get; set; } = true;

        [Description("Check if Entity's health greater than zero:\n" +
            "True: Only Entities with nonzero health are detected\n" +
            "False: Entity's health does not checked during search")]
        [Category("Entity")]
        public bool HealthCheck { get; set; } = true;

        [Category("Entity")]
        public float EntityRadius { get; set; } = 12;

        [Description("The maximum distance from the character within which the Entity is searched\n" +
            "The default value is 0, which disables distance checking")]
        //[Category("Entity")]
        public float ReactionRange { get; set; } = 30;

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
                    entity = EntitySelectionTools.FindClosestEntity(EntityManager.GetEntities(), EntityID, EntityIdType, EntityNameType, HealthCheck, Range, RegionCheck);
                    return entity != null && entity.IsValid && !(HealthCheck && entity.IsDead) && entity.CombatDistance > EntityRadius;
                }
                return false;
            }
        }

        public override bool Run()
        {
            return Approach.EntityByDistance(entity, EntityRadius);
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
                EntityID = this.EntityID,
                EntityIdType = this.EntityIdType,
                EntityNameType = this.EntityNameType,
                RegionCheck = this.RegionCheck,
                HealthCheck = this.HealthCheck,
                EntityRadius = this.EntityRadius,
            });
        }

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
