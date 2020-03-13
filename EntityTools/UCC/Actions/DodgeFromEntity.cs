using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;
using Astral.Classes.ItemFilter;
using Astral.Logic.UCC.Actions;
using Astral.Logic.UCC.Classes;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Tools;
using MyNW.Classes;

namespace EntityTools.UCC.Actions
{
    [Serializable]
    public class DodgeFromEntity : UCCAction
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
            "The 0 (zero) value disables distance checking")]
        //[Category("Entity")]
        [Category("Optional")]
        public float ReactionRange { get; set; } = 30;

        [Description("The maximum ZAxis difference from the withing which the Entity is searched\n" +
            "The default value is 0, which disables ZAxis checking")]
        [Category("Optional")]
        public float ReactionZRange { get; set; } = 0;

        [DisplayName("Moving time")]
        [Category("Required")]
        public int MovingTime { get => dodge.MovingTime; set => dodge.MovingTime = value; }

        [DisplayName("Dodge Direction")]
        [Category("Required")]
        public Astral.Logic.UCC.Ressources.Enums.DodgeDirection Direction { get =>dodge.Direction; set => dodge.Direction = value; }

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
                        Comparer = EntityToPatternComparer.Get(entityId, entityIdType, entityNameType);
                    
                    //entity = EntitySelectionTools.FindClosestEntity(EntityManager.GetEntities(), EntityID, EntityIdType, EntityNameType, 
                    //                                                HealthCheck, Range, RegionCheck);

                    entity = SearchCached.FindClosestEntity(entityId, entityIdType, entityNameType, EntitySetType.Complete, 
                                                            HealthCheck, ReactionRange, ReactionZRange, RegionCheck);

                    return Validate(entity) && entity.Location.Distance3DFromPlayer <= EntityRadius;
                }
                return false;
            }
        }

        public override bool Run()
        {
            if (entity.Location.Distance3DFromPlayer < EntityRadius)
                return dodge.Run();
            return true; 
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

            //    // Выключаем все системы навигации
            //    //Astral.Logic.UCC.Controllers.Movements.Stop();
            //    ReflectionHelper.ExecStaticMethod(typeof(Astral.Logic.UCC.Controllers.Movements), "Stop", new object[0], out object res);
            //    //Astral.Logic.UCC.Controllers.Movements.RequireRange = 0;
            //    Astral.Quester.API.Engine.Navigation.Stop();
            //    MyNW.Internals.Movements.StopNavTo();

            //    bool flag2 = false;
            //    int num = 0;
            //    Vector3 location = EntityManager.LocalPlayer.Location;
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
            //        flag2 = (EntityManager.LocalPlayer.Location.Distance3D(location) > 5.0);
            //    }
            //    Astral.Logic.UCC.Core.CurrentTarget.Location.Face();
            //    Thread.Sleep(250);
            //    if (!Combats.ShouldDodge(true))
            //    {
            //        Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(2000);
            //        Astral.Classes.Timeout timeout2 = new Astral.Classes.Timeout(5000);
            //        if (Combats.IsMeleeChar && this.pathtToLocIsInAOE(EntityManager.LocalPlayer.Location, Astral.Logic.UCC.Core.CurrentTarget.Location))
            //        {
            //            while (this.pathtToLocIsInAOE(EntityManager.LocalPlayer.Location, Astral.Logic.UCC.Core.CurrentTarget.Location))
            //            {
            //                Vector3 location2 = EntityManager.LocalPlayer.Location;
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
            //                        Astral.Logic.UCC.Core.Get.queryTargetChange(entity, "near while aoe", 4000);
            //                        goto IL_32E;
            //                    }
            //                    if (entity.CombatDistance < 60f && !this.pathtToLocIsInAOE(location2, entity.Location))
            //                    {
            //                        if (!timeout.IsTimedOut)
            //                        {
            //                            if (EntityManager.CurrentSettings.UsePathfinding3 && PathFinding.CheckDirection(location2, entity.Location, ref vector))
            //                            {
            //                                continue;
            //                            }
            //                        }
            //                        else if (EntityManager.CurrentSettings.UsePathfinding3)
            //                        {
            //                            float num2 = 0f;
            //                            while ((double)num2 < 6.2831853071795862)
            //                            {
            //                                float num3 = (float)Math.Cos((double)num2) * 30f;
            //                                float num4 = (float)Math.Sin((double)num2) * 30f;
            //                                Vector3 vector2 = new Vector3(location2.X + num3, location2.Y + num4, location2.Z + 2f);
            //                                if (!vector2.IsInBackByRange(1.5f, 3.40282347E+38f) && !this.pathtToLocIsInAOE(location2, vector2) && !PathFinding.CheckDirection(location2, vector2, ref vector) && !this.pathtToLocIsInAOE(vector2, Astral.Logic.UCC.Core.CurrentTarget.Location) && !PathFinding.CheckDirection(vector2, Astral.Logic.UCC.Core.CurrentTarget.Location, ref vector))
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
            //                        Astral.Logic.UCC.Core.Get.queryTargetChange(entity, "outside aoe", 6000);
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

        /// <summary>
        /// Функтор, предназначенный для проверки соответствия Entity
        /// основным критериям: EntityID, EntityIdType, EntityNameType
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        internal Predicate<Entity> Comparer { get; private set; } = null;

        /// <summary>
        /// Ссылка на ближайший к персонажу Entity
        /// </summary>
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
                        entity = SearchCached.FindClosestEntity(entityId, entityIdType, entityNameType, EntitySetType.Complete,
                                                                HealthCheck, ReactionRange,
                                                                (ReactionZRange > 0) ? ReactionZRange : Astral.Controllers.Settings.Get.MaxElevationDifference, 
                                                                RegionCheck, null, Aura.Checker);
                        return entity;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Проверка валидности цели
        /// Флаг IsValid не гарантирует, что ранее найденный Entity ссылается на ту же сущность
        /// поскольку игровой клиент может её подменить
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool Validate(Entity e)
        {
            return e != null && e.IsValid && Comparer?.Invoke(e) == true;
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
            dodge.Direction = Astral.Logic.UCC.Ressources.Enums.DodgeDirection.DodgeSmart;
        }

        public override UCCAction Clone()
        {
            return base.BaseClone(new DodgeFromEntity
            {
                entityId = this.entityId,
                entityIdType = this.entityIdType,
                entityNameType = this.entityNameType,
                RegionCheck = this.RegionCheck,
                HealthCheck = this.HealthCheck,
                EntityRadius = this.EntityRadius,
                ReactionRange = this.ReactionRange,
                ReactionZRange = this.ReactionZRange,
                dodge = this.dodge.Clone() as Dodge,
                Aura = new AuraOption
                {
                    AuraName = this.Aura.AuraName,
                    AuraNameType = this.Aura.AuraNameType,
                    Sign = this.Aura.Sign,
                    Stacks = this.Aura.Stacks
                }
            });
        }

        //private bool havewaitdel(Func<bool> del)
        //{
        //    return del != null && del();
        //}

        //private bool pathtToLocIsInAOE(Vector3 start, Vector3 loc)
        //{
            
        //    Vector3 vector3_ = new Vector3();
        //    foreach (AOECheck.AOE aoe in AOECheck.List)
        //    {
        //        if (aoe.IsIn(loc))
        //        {
        //            return true;
        //        }
        //        if (aoe.Radius != 0f)
        //        {
        //            if (aoe.Source != null)
        //            {
        //                vector3_ = aoe.Source.Location;
        //            }
        //            if (aoe.Location != null)
        //            {
        //                vector3_ = aoe.Location();
        //            }
        //            if (Class81.smethod_5(vector3_, aoe.Radius, start, loc) > 0)
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}

        // Определение направления DodgeSmart
        // из Astral.Logic.NW.Movements
        //internal static Vector3 smart()
        //{
        //    Vector3 playerPos = EntityManager.LocalPlayer.Location;
        //    float num = 30f;
        //    List<Vector3> list = new List<Vector3>();
        //    Vector3 shouldDodgeSource = Combats.ShouldDodgeSource;
        //    int num2 = 0;
        //    if (shouldDodgeSource.IsValid)
        //    {
        //        if (Combats.ShouldFrontDodge)
        //        {
        //            num2 = 1;
        //        }
        //        else if (!Combats.AOEIsArc)
        //        {
        //            if (shouldDodgeSource.IsInYawFaceByRange(1.5f, 3.40282347E+38f) && shouldDodgeSource.Distance3D(playerPos) > 5.0 && Combats.InitialCombatLoc.Distance3D(playerPos) < 100.0)
        //            {
        //                num2 = 2;
        //            }
        //            else if ((Combats.IsMeleeChar && shouldDodgeSource.Distance3D(playerPos) < 5.0) || shouldDodgeSource.IsInBackByRange(1.5f, 3.40282347E+38f))
        //            {
        //                num2 = 1;
        //            }
        //        }
        //    }
        //    float num3 = 0f;
        //    while ((double)num3 < 6.2831853071795862)
        //    {
        //        float num4 = (float)Math.Cos((double)num3) * num;
        //        float num5 = (float)Math.Sin((double)num3) * num;
        //        Vector3 item = new Vector3(playerPos.X + num4, playerPos.Y + num5, playerPos.Z + 2f);
        //        list.Add(item);
        //        num3 += 0.314159274f;
        //    }
        //    list = (from i in list
        //            orderby Guid.NewGuid()
        //            select i).ToList<Vector3>();
        //    Vector3 vector = new Vector3();
        //    List<Astral.Logic.NW.Movements.DodgeLosTestResult> list2 = new List<Astral.Logic.NW.Movements.DodgeLosTestResult>();
        //    if (EntityManager.CurrentSettings.UsePathfinding3)
        //    {
        //        using (List<Vector3>.Enumerator enumerator = list.GetEnumerator())
        //        {
        //            while (enumerator.MoveNext())
        //            {
        //                Vector3 vector2 = enumerator.Current;
        //                Vector3 collidePos = new Vector3();
        //                bool collided = PathFinding.CheckDirection(playerPos, vector2, ref collidePos);
        //                list2.Add(new Astral.Logic.NW.Movements.DodgeLosTestResult(vector2, collided, collidePos));
        //            }
        //            goto IL_2E7;
        //        }
        //    }
        //    List<Injection.RayCastParams> list3 = new List<Injection.RayCastParams>();
        //    Vector3 from = new Vector3(playerPos.X, playerPos.Y, playerPos.Z + 2f);
        //    foreach (Vector3 vector3 in list)
        //    {
        //        list3.Add(new Injection.RayCastParams(from, vector3));
        //        Vector3 to = new Vector3(vector3.X, vector3.Y, vector3.Z - 5f);
        //        list3.Add(new Injection.RayCastParams(vector3, to));
        //    }
        //    Injection.RayCastResult[] array = Injection.MassPosRayCast(list3.ToArray(), 142u);
        //    for (int j = 0; j < list.Count; j++)
        //    {
        //        Injection.RayCastResult rayCastResult = array[j + j];
        //        if (array[j + j + 1].collided)
        //        {
        //            list2.Add(new Astral.Logic.NW.Movements.DodgeLosTestResult(list[j], rayCastResult.collided, rayCastResult.result));
        //        }
        //    }
        //    IL_2E7:
        //    Astral.Logic.NW.Movements.LastValidPoses = list2;
        //    Astral.Logic.NW.Movements.lastvlidposto.ChangeTime(2500);
        //    bool flag;
        //    if (!(flag = list2.Any((Astral.Logic.NW.Movements.DodgeLosTestResult r) => !r.Collided)))
        //    {
        //        list2 = (from r in list2
        //                 orderby r.CollidePos.Distance2D(playerPos) descending
        //                 select r).ToList<Astral.Logic.NW.Movements.DodgeLosTestResult>();
        //    }
        //    List<Entity> entities = EntityManager.GetEntities();
        //    float yaw = EntityManager.LocalPlayer.Yaw;
        //    using (List<Astral.Logic.NW.Movements.DodgeLosTestResult>.Enumerator enumerator2 = list2.GetEnumerator())
        //    {
        //        IL_490:
        //        while (enumerator2.MoveNext())
        //        {
        //            Astral.Logic.NW.Movements.DodgeLosTestResult dodgeLosTestResult = enumerator2.Current;
        //            Vector3 vector4 = dodgeLosTestResult.TestedPos;
        //            if (!flag || !dodgeLosTestResult.Collided)
        //            {
        //                if (dodgeLosTestResult.Collided)
        //                {
        //                    vector4 = dodgeLosTestResult.CollidePos;
        //                }
        //                if (!Combats.InitialCombatLoc.IsValid || Combats.InitialCombatLoc.Distance3D(playerPos) >= 150.0 || vector4.Distance3D(Combats.InitialCombatLoc) <= 70.0)
        //                {
        //                    if (!vector.IsValid)
        //                    {
        //                        vector = vector4;
        //                    }
        //                    if ((num2 != 2 || !vector4.IsInYawFaceByRange(1.5f, 3.40282347E+38f)) && (num2 != 1 || !vector4.IsInBackByRange(1.5f, 3.40282347E+38f)) && (num2 != 0 || (!vector4.IsInYawFaceByRange(0.5f, 3.40282347E+38f) && !vector4.IsInBackByRange(0.5f, 3.40282347E+38f))))
        //                    {
        //                        using (List<AOECheck.AOE>.Enumerator enumerator3 = AOECheck.List.GetEnumerator())
        //                        {
        //                            while (enumerator3.MoveNext())
        //                            {
        //                                if (enumerator3.Current.IsIn(vector4))
        //                                {
        //                                    goto IL_490;
        //                                }
        //                            }
        //                        }
        //                        if (!Astral.Logic.NW.Movements.ThereIsDangerousEntities(vector4, 70.0, entities))
        //                        {
        //                            return vector4;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    if (!vector.IsValid && list.Count > 0)
        //    {
        //        vector = list[0];
        //    }
        //    return vector;
        //}

        [NonSerialized]
        protected Entity entity = new Entity(IntPtr.Zero);

        //[NonSerialized]
        //private string spellNameCache = string.Empty;

        [NonSerialized]
        private string entityId = string.Empty;
        [NonSerialized]
        private ItemFilterStringType entityIdType = ItemFilterStringType.Simple;
        [NonSerialized]
        private EntityNameType entityNameType = EntityNameType.NameUntranslated;
        [NonSerialized]
        private Dodge dodge = new Dodge();

        #region Hide Inherited Properties
        [XmlIgnore]
        [Browsable(false)]
        public new Astral.Logic.UCC.Ressources.Enums.Unit Target { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public new int Timer { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public new string ActionName { get; set; } = string.Empty;
        #endregion
    }
}
