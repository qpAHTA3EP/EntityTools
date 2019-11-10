using System;
using System.ComponentModel;
using System.Drawing.Design;
using Astral.Classes.ItemFilter;
using Astral.Controllers;
using Astral.Logic.NW;
using Astral.Logic.UCC.Actions;
using Astral.Logic.UCC.Classes;
using Astral.Logic.UCC.Ressources;
using EntityTools.Editors;
using EntityTools.Tools;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityTools.UCC
{
    [Serializable]
    public class DodgeFromEntity : UCCAction
    {
        [Description("ID of the Entity for the search (regex)")]
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
        public bool RegionCheck { get; set; } = true;

        [Description("Check if Entity's health greater than zero:\n" +
            "True: Only Entities with nonzero health are detected\n" +
            "False: Entity's health does not checked during search")]
        [Category("Entity optional checks")]
        public bool HealthCheck { get; set; } = true;

        [Description("The maximum distance from the character within which the Entity is searched\n" +
            "The default value is 0, which disables distance checking")]
        [Category("Entity optional checks")]
        public float ReactionRange { get; set; } = 30;

        [Category("Required")]
        public float EntityRadius { get; set; } = 12;

        [DisplayName("Moving time")]
        [Category("Required")]
        public int MovingTime { get; set; }

        [DisplayName("Dodge Direction")]
        [Category("Required")]
        public Enums.DodgeDirection Direction { get; set; }

        [Browsable(false)]
        public new string ActionName { get; set; } = string.Empty;

        public override bool NeedToRun
        {
            get
            {
                if (!string.IsNullOrEmpty(EntityID))
                {
                    entity = EntitySelectionTools.FindClosestEntity(EntityManager.GetEntities(), EntityID, EntityIdType, EntityNameType, HealthCheck, Range, RegionCheck);

                    return entity != null && entity.IsValid && entity.Location.Distance3DFromPlayer <= EntityRadius;//!Dodge.DisableDodge;
                }
                return false;
            }
        }

        public override bool Run()
        {
            return false;
            //bool flag = false;
            //for (; ; )
            //{
            //    IL_326:
            //    if (flag)
            //    {
            //        if (base.CoolDown > 0)
            //        {
            //            break;
            //        }
            //    }
            //    Astral.Logic.UCC.Controllers.Movements.Stop();
            //    Astral.Logic.UCC.Controllers.Movements.RequireRange = 0;
            //    Class1.MainEngine.Navigation.Stop();
            //    MyNW.Internals.Movements.StopNavTo();
            //    bool flag2 = false;
            //    int num = 0;
            //    Vector3 location = Class1.LocalPlayer.Location;
            //    while (!flag2)
            //    {
            //        num++;
            //        if (num > 2)
            //        {
            //            goto IL_32E;
            //        }
            //        flag = true;
            //        Astral.Logic.NW.Movements.Dodge(this.Direction, this.MovingTime);
            //        Thread.Sleep(250);
            //        flag2 = (Class1.LocalPlayer.Location.Distance3D(location) > 5.0);
            //    }
            //    Core.CurrentTarget.Location.Face();
            //    Thread.Sleep(250);
            //    if (!Combats.ShouldDodge(true))
            //    {
            //        Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(2000);
            //        Astral.Classes.Timeout timeout2 = new Astral.Classes.Timeout(5000);
            //        if (Combats.IsMeleeChar && this.pathtToLocIsInAOE(Class1.LocalPlayer.Location, Core.CurrentTarget.Location))
            //        {
            //            while (this.pathtToLocIsInAOE(Class1.LocalPlayer.Location, Core.CurrentTarget.Location))
            //            {
            //                Vector3 location2 = Class1.LocalPlayer.Location;
            //                Vector3 vector = new Vector3();
            //                Thread.Sleep(150);
            //                if (Combats.ShouldDodge(true))
            //                {
            //                    goto IL_326;
            //                }
            //                if (timeout2.IsTimedOut)
            //                {
            //                    break;
            //                }
            //                foreach (Entity entity in from a in Attackers.List
            //                                          orderby a.CombatDistance
            //                                          select a)
            //                {
            //                    if (entity.CombatDistance < 10f)
            //                    {
            //                        Core.Get.queryTargetChange(entity, "near while aoe", 4000);
            //                        goto IL_32E;
            //                    }
            //                    if (entity.CombatDistance < 60f && !this.pathtToLocIsInAOE(location2, entity.Location))
            //                    {
            //                        if (!timeout.IsTimedOut)
            //                        {
            //                            if (Class1.CurrentSettings.UsePathfinding3 && PathFinding.CheckDirection(location2, entity.Location, ref vector))
            //                            {
            //                                continue;
            //                            }
            //                        }
            //                        else if (Class1.CurrentSettings.UsePathfinding3)
            //                        {
            //                            float num2 = 0f;
            //                            while ((double)num2 < 6.2831853071795862)
            //                            {
            //                                float num3 = (float)Math.Cos((double)num2) * 30f;
            //                                float num4 = (float)Math.Sin((double)num2) * 30f;
            //                                Vector3 vector2 = new Vector3(location2.X + num3, location2.Y + num4, location2.Z + 2f);
            //                                if (!vector2.IsInBackByRange(1.5f, 3.40282347E+38f) && !this.pathtToLocIsInAOE(location2, vector2) && !PathFinding.CheckDirection(location2, vector2, ref vector) && !this.pathtToLocIsInAOE(vector2, Core.CurrentTarget.Location) && !PathFinding.CheckDirection(vector2, Core.CurrentTarget.Location, ref vector))
            //                                {
            //                                    vector2.Face();
            //                                    flag = true;
            //                                    Astral.Logic.NW.Movements.Dodge(Enums.DodgeDirection.DodgeFront, this.MovingTime);
            //                                    Thread.Sleep(250);
            //                                    goto IL_32E;
            //                                }
            //                                num2 += 0.314159274f;
            //                            }
            //                        }
            //                        Core.Get.queryTargetChange(entity, "outside aoe", 6000);
            //                        goto IL_32E;
            //                    }
            //                }
            //            }
            //            break;
            //        }
            //        break;
            //    }
            //}
            //IL_32E:
            //base.CurrentTimeout = new Astral.Classes.Timeout(base.CoolDown);
            //return true;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(EntityID))
                return GetType().Name;
            else return GetType().Name + " [" + EntityID + ']';
        }

        public DodgeFromEntity()
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

        private bool havewaitdel(Func<bool> del)
        {
            return del != null && del();
        }

        private bool pathtToLocIsInAOE(Vector3 start, Vector3 loc)
        {
            //Vector3 vector3_ = new Vector3();
            //foreach (AOECheck.AOE aoe in AOECheck.List)
            //{
            //    if (aoe.IsIn(loc))
            //    {
            //        return true;
            //    }
            //    if (aoe.Radius != 0f)
            //    {
            //        if (aoe.Source != null)
            //        {
            //            vector3_ = aoe.Source.Location;
            //        }
            //        if (aoe.Location != null)
            //        {
            //            vector3_ = aoe.Location();
            //        }
            //        if (Class81.smethod_5(vector3_, aoe.Radius, start, loc) > 0)
            //        {
            //            return true;
            //        }
            //    }
            //}
            return false;
        }
        [NonSerialized]
        protected Entity entity = new Entity(IntPtr.Zero);

        [NonSerialized]
        private string spellNameCache = string.Empty;
    }
}
