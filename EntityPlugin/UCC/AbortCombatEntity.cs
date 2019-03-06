using System;
using System.ComponentModel;
using System.Drawing.Design;
using Astral.Classes;
using Astral.Controllers;
using Astral.Logic.NW;
using Astral.Logic.UCC.Classes;
using Astral.Quester.FSM.States;
using EntityPlugin;
using EntityPlugin.Editors;
using MyNW.Classes;
using MyNW.Internals;
using ns1;

namespace Astral.Logic.UCC.Actions
{
    public class AbordCombatEntity : AbordCombat
    {
        public AbordCombatEntity()
        {
            EntityID = string.Empty;
            Distance = 0;
            Sign = Ressources.Enums.Sign.Superior;
            IgnoreCombatTime = 0;
            IgnoreCombatMinHP = 25;
        }

        public override UCCAction Clone()
        {
            return base.BaseClone(new AbordCombatEntity
            {
                IgnoreCombatMinHP = this.IgnoreCombatMinHP,
                IgnoreCombatTime = this.IgnoreCombatTime,
                EntityID = this.EntityID,
                Distance = this.Distance,
                Sign = this.Sign
            });
        }

        public override bool NeedToRun
        {
            get
            {
                if (base.NeedToRun)
                {
                    if (string.IsNullOrEmpty(EntityID))
                        entity = new Entity(IntPtr.Zero);
                    else
                        entity = Tools.FindClosestEntity(EntityManager.GetEntities(), EntityID);

                    return (entity.IsValid && entity.Location.Distance3DFromPlayer >= Distance);
                }
                return false;
            }
        }

        public override bool Run()
        {
            if (!entity.IsValid)
            {
                switch (Sign)
                {
                    case Ressources.Enums.Sign.Equal:
                        if(entity.Location.Distance3DFromPlayer == Distance)
                            return base.Run();
                        break;
                    case Ressources.Enums.Sign.NotEqual:
                        if (entity.Location.Distance3DFromPlayer != Distance)
                            return base.Run();
                        break;
                    case Ressources.Enums.Sign.Inferior:
                        if (entity.Location.Distance3DFromPlayer < Distance)
                            return base.Run();
                        break;
                    case Ressources.Enums.Sign.Superior:
                        if (entity.Location.Distance3DFromPlayer > Distance)
                            return base.Run();
                        break;
                }
            }
            return true;
        }

        public override string ToString() => GetType().Name;

        [Browsable(false)]
        public new string ActionName { get; set; }

        //[Description("How many time ignore combat in seconds (0 for infinite)")]
        //public int IgnoreCombatTime { get; set; }

        //[Description("Minimum health percent to enable combat again")]
        //public int IgnoreCombatMinHP { get; set; }

        [Description("ID (an untranslated name) of the Entity for the search (regex)")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        [Category("Entity")]
        public string EntityID { get; set; }

        [Category("Entity")]
        public float Distance { get; set; }

        [Description("Distance comparison type to the closest Entity")]
        [Category("Entity")]
        public Ressources.Enums.Sign Sign { get; set; }

        [NonSerialized]
        protected Entity entity;
    }
}
