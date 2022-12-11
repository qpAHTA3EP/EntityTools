using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

using ACTP0Tools.Annotations;

using Astral.Classes.ItemFilter;
using Astral.Logic.UCC.Actions;
using Astral.Logic.UCC.Classes;

using EntityTools.Core.Interfaces;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Tools;
using EntityTools.Tools.Entities;

using MyNW.Classes;

using static Astral.Logic.UCC.Ressources.Enums;

using Timeout = Astral.Classes.Timeout;

namespace EntityTools.UCC.Actions
{
    [Serializable]
    public class DodgeFromEntity : UCCAction, IEntityDescriptor, INotifyPropertyChanged
    {
        #region Опции команды
        #region Entity
#if DEVELOPER
        [Description("ID of the Entity for the search")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        [Category("Entity")]
#else
        [Browsable(false)]
#endif
        public string EntityID
        {
            get => _entityId;
            set
            {
                if (_entityId != value)
                {
                    _entityId = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _entityId = string.Empty;

#if DEVELOPER
        [Description("Type of and EntityID:\n" +
            "Simple: Simple text string with a wildcard at the beginning or at the end (char '*' means any symbols)\n" +
            "Regex: Regular expression")]
        [Category("Entity")]
#else
        [Browsable(false)]
#endif
        public ItemFilterStringType EntityIdType
        {
            get => _entityIdType;
            set
            {
                if (_entityIdType != value)
                {
                    _entityIdType = value;
                    OnPropertyChanged();
                }
            }
        }
        private ItemFilterStringType _entityIdType = ItemFilterStringType.Simple;

#if DEVELOPER
        [Description("The switcher of the Entity filed which compared to the property EntityID")]
        [Category("Entity")]
#else
        [Browsable(false)]
#endif
        public EntityNameType EntityNameType
        {
            get => _entityNameType;
            set
            {
                if (_entityNameType != value)
                {
                    _entityNameType = value;
                    OnPropertyChanged();
                }
            }
        }
        private EntityNameType _entityNameType = EntityNameType.InternalName;

#if DEVELOPER
        [Category("Entity")]
#else
        [Browsable(false)]
#endif
        public float EntityRadius
        {
            get => _entityRadius;
            set
            {
                if (Math.Abs(_entityRadius - value) > 0.1)
                {
                    _entityRadius = value;
                    OnPropertyChanged();
                }
            }
        }
        private float _entityRadius = 12;
#if DEVELOPER
        [XmlIgnore]
        [Editor(typeof(EntityTestEditor), typeof(UITypeEditor))]
        [Description("Test the Entity searching.")]
        [Category("Entity")]
        public string EntityTestInfo => "Push button '...' =>";
#endif 
        #endregion

#if DEVELOPER
        [Description("Check Entity's Ingame Region (Not CustomRegion):\n" +
            "True: Only Entities located in the same Region as Player are detected\n" +
            "False: Entity's Region does not checked during search")]
        //[Category("Entity")]
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
        public bool RegionCheck
        {
            get => _regionCheck;
            set
            {
                if (_regionCheck != value)
                {
                    _regionCheck = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _regionCheck = true;

#if DEVELOPER
        [Description("Check if Entity's health greater than zero:\n" +
            "True: Only Entities with nonzero health are detected\n" +
            "False: Entity's health does not checked during search")]
        //[Category("Entity")]
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
        public bool HealthCheck
        {
            get => _healthCheck;
            set
            {
                if (_healthCheck != value)
                {
                    _healthCheck = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _healthCheck = true;

#if DEVELOPER
        [Description("Aura which checked on the Entity")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        //[Category("Entity")]
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
        public AuraOption Aura
        {
            get => _aura;
            set
            {
                if (_aura != value)
                {
                    _aura = value;
                    OnPropertyChanged();
                }
            }
        }
        private AuraOption _aura = new AuraOption();

#if DEVELOPER
        [Description("The maximum distance from the character within which the Entity is searched\n" +
            "The 0 (zero) value disables distance checking")]
        //[Category("Entity")]
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
        public float ReactionRange
        {
            get => _reactionRange;
            set
            {
                if (Math.Abs(_reactionRange - value) > 0.1)
                {
                    _reactionRange = value;
                    OnPropertyChanged();
                }
            }
        }
        private float _reactionRange = 30;

#if DEVELOPER
        [Description("The maximum ZAxis difference from the withing which the Entity is searched\n" +
            "The default value is 0, which disables ZAxis checking")]
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
        public float ReactionZRange
        {
            get => _reactionZRange;
            set
            {
                if (Math.Abs(_reactionZRange - value) > 0.1)
                {
                    _reactionZRange = value;
                    OnPropertyChanged();
                }
            }
        }
        private float _reactionZRange;

#if DEVELOPER
        [DisplayName("Moving time")]
        [Category("Required")]
#else
        [Browsable(false)]
#endif
        public int MovingTime
        {
            get => _movingTime;
            set
            {
                if (_movingTime != value)
                {
                    _movingTime = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _movingTime = 700;

#if DEVELOPER
        [DisplayName("Dodge Direction")]
        [Category("Required")]
#else
        [Browsable(false)]
#endif
        public DodgeDirection Direction
        {
            get => _dodgeDirection;
            set
            {
                if (_dodgeDirection != value)
                {
                    _dodgeDirection = value;
                    OnPropertyChanged();
                }
            }
        }
        private DodgeDirection _dodgeDirection = DodgeDirection.DodgeSmart;


        #region Hide Inherited Properties
        [XmlIgnore]
        [Browsable(false)]
        public new Unit Target { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public new int Timer { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public new string ActionName { get; set; } = string.Empty;
        #endregion
        #endregion

        #region Взаимодействие с INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = default)
        {
            privateResetOnPropertyChanged(propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void privateResetOnPropertyChanged([CallerMemberName] string propertyName = default)
        {
            _key = null;
            _specialCheck = null;
            _label = string.Empty;

            entity = null;
            timeout.ChangeTime(0);
        }
        #endregion




        public override UCCAction Clone()
        {
            return BaseClone(new DodgeFromEntity
            {
                _entityId = _entityId,
                _entityIdType = _entityIdType,
                _entityNameType = _entityNameType,
                _regionCheck = _regionCheck,
                _healthCheck = _healthCheck,
                _entityRadius = _entityRadius,
                _reactionRange = _reactionRange,
                _reactionZRange = _reactionZRange,
                _aura = new AuraOption
                {
                    AuraName = _aura.AuraName,
                    AuraNameType = _aura.AuraNameType,
                    Sign = _aura.Sign,
                    Stacks = _aura.Stacks
                }
            });
        }

        #region Данные
        private Dodge dodge = new Dodge();

        private Entity entity;
        private Timeout timeout = new Timeout(0);
        private string _label = string.Empty;
        #endregion
        


        
        #region IUCCActionEngine
        public override bool NeedToRun
        {
            get
            {
                if (!string.IsNullOrEmpty(_entityId))
                {
                    var entityKey = EntityKey;

                    if (timeout.IsTimedOut)
                    {
                        entity = SearchCached.FindClosestEntity(entityKey, SpecialCheck);

                        timeout.ChangeTime(EntityTools.Config.EntityCache.CombatCacheTime);
                    }

                    return entityKey.Validate(entity) && entity.Location.Distance3DFromPlayer <= _entityRadius;
                }
                return false;
            }
        }

        public override bool Run()
        {
            return dodge.Run();
#if false
            bool flag = false;
            for (; ; )
            {
                IL_326:
                if (flag)
                {
                    if (base.CoolDown > 0)
                    {
                        break;
                    }
                }

                // Выключаем все системы навигации
                //Astral.Logic.UCC.Controllers.Movements.Stop();
                ReflectionHelper.ExecStaticMethod(typeof(Astral.Logic.UCC.Controllers.Movements), "Stop", new object[0], out object res);
                //Astral.Logic.UCC.Controllers.Movements.RequireRange = 0;
                Astral.Quester.API.Engine.Navigation.Stop();
                MyNW.privates.Movements.StopNavTo();

                bool flag2 = false;
                int num = 0;
                Vector3 location = EntityManager.LocalPlayer.Location;
                while (!flag2)
                {
                    num++;
                    if (num > 2)
                    {
                        goto IL_32E;
                    }
                    flag = true;
                    Astral.Logic.NW.Movements.Dodge(this.Direction, this.MovingTime);
                    Thread.Sleep(250);
                    flag2 = (EntityManager.LocalPlayer.Location.Distance3D(location) > 5.0);
                }
                Astral.Logic.UCC.Core.CurrentTarget.Location.Face();
                Thread.Sleep(250);
                if (!Combats.ShouldDodge(true))
                {
                    Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(2000);
                    Astral.Classes.Timeout timeout2 = new Astral.Classes.Timeout(5000);
                    if (Combats.IsMeleeChar && this.pathtToLocIsInAOE(EntityManager.LocalPlayer.Location, Astral.Logic.UCC.Core.CurrentTarget.Location))
                    {
                        while (this.pathtToLocIsInAOE(EntityManager.LocalPlayer.Location, Astral.Logic.UCC.Core.CurrentTarget.Location))
                        {
                            Vector3 location2 = EntityManager.LocalPlayer.Location;
                            Vector3 vector = Vector3.Empty;
                            Thread.Sleep(150);
                            if (Combats.ShouldDodge(true))
                            {
                                goto IL_326;
                            }
                            if (timeout2.IsTimedOut)
                            {
                                break;
                            }
                            foreach (Entity entity in from a in Attackers.List
                                                      orderby a.CombatDistance
                                                      select a)
                            {
                                if (entity.CombatDistance < 10f)
                                {
                                    Astral.Logic.UCC.Core.Get.queryTargetChange(entity, "near while aoe", 4000);
                                    goto IL_32E;
                                }
                                if (entity.CombatDistance < 60f && !this.pathtToLocIsInAOE(location2, entity.Location))
                                {
                                    if (!timeout.IsTimedOut)
                                    {
                                        if (EntityManager.CurrentSettings.UsePathfinding3 && PathFinding.CheckDirection(location2, entity.Location, ref vector))
                                        {
                                            continue;
                                        }
                                    }
                                    else if (EntityManager.CurrentSettings.UsePathfinding3)
                                    {
                                        float num2 = 0f;
                                        while ((double)num2 < 6.2831853071795862)
                                        {
                                            float num3 = (float)Math.Cos((double)num2) * 30f;
                                            float num4 = (float)Math.Sin((double)num2) * 30f;
                                            Vector3 vector2 = new Vector3(location2.X + num3, location2.Y + num4, location2.Z + 2f);
                                            if (!vector2.IsInBackByRange(1.5f, 3.40282347E+38f) && !this.pathtToLocIsInAOE(location2, vector2) && !PathFinding.CheckDirection(location2, vector2, ref vector) && !this.pathtToLocIsInAOE(vector2, Astral.Logic.UCC.Core.CurrentTarget.Location) && !PathFinding.CheckDirection(vector2, Astral.Logic.UCC.Core.CurrentTarget.Location, ref vector))
                                            {
                                                vector2.Face();
                                                flag = true;
                                                Astral.Logic.NW.Movements.Dodge(Enums.DodgeDirection.DodgeFront, this.MovingTime);
                                                Thread.Sleep(250);
                                                goto IL_32E;
                                            }
                                            num2 += 0.314159274f;
                                        }
                                    }
                                    Astral.Logic.UCC.Core.Get.queryTargetChange(entity, "outside aoe", 6000);
                                    goto IL_32E;
                                }
                            }
                        }
                        break;
                    }
                    break;
                }
            }
            IL_32E:
            base.CurrentTimeout = new Astral.Classes.Timeout(base.CoolDown);
            return true;
#endif
        }

        public Entity UnitRef
        {
            get
            {
                var entityKey = EntityKey;
                if (entityKey.Validate(entity))
                    return entity;
                else
                {
                    if (!string.IsNullOrEmpty(_entityId))
                    {
                        entity = SearchCached.FindClosestEntity(entityKey,
                                                                SpecialCheck);
                        return entity;
                    }
                }
                return null;
            }
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(_label))
            {
                _label = string.IsNullOrEmpty(_entityId) 
                       ? GetType().Name 
                       : $"{GetType().Name} [{_entityId}]";
            }
            return _label;
        }
        #endregion

        #region Вспомогательные инструменты
        // TODO: Добавить постфиксный патч Astral.Controllers.AOECheck.Check()
        // который будет добавлять в список AOE Entity, от которых нужно уклоняться

        /// <summary>
        /// Комплексный (составной) идентификатор, используемый для поиска <see cref="Entity"/> в кэше
        /// </summary>
        public EntityCacheRecordKey EntityKey => _key ?? (_key = new EntityCacheRecordKey(_entityId, _entityIdType, _entityNameType));

        private EntityCacheRecordKey _key;

        /// <summary>
        /// Функтор дополнительной проверки <seealso cref="Entity"/> 
        /// на предмет нахождения наличия (отсутствия) ауры <see cref="DodgeFromEntity.Aura"/>
        /// Использовать самомодифицирующийся предиката, т.к. предикат передается в <seealso cref="SearchCached.FindClosestEntity(EntityCacheRecordKey, Predicate{Entity})"/>
        /// </summary>        
        private Predicate<Entity> SpecialCheck
        {
            get
            {
                if (_specialCheck is null)
                    _specialCheck = SearchHelper.Construct_EntityAttributePredicate(_healthCheck,
                                                            _reactionRange,
                                                            _reactionZRange > 0 ? _reactionZRange : Astral.Controllers.Settings.Get.MaxElevationDifference,
                                                            _regionCheck,
                                                            _aura.IsMatch);
                return _specialCheck;
            }
        }
        private Predicate<Entity> _specialCheck;
        #endregion


#if false //Копия кода dodge
        private bool havewaitdel(Func<bool> del)
        {
            return del != null && del();
        }

        private bool pathtToLocIsInAOE(Vector3 start, Vector3 loc)
        {

            Vector3 vector3_ = Vector3.Empty;
            foreach (AOECheck.AOE aoe in AOECheck.List)
            {
                if (aoe.IsIn(loc))
                {
                    return true;
                }
                if (aoe.Radius != 0f)
                {
                    if (aoe.Source != null)
                    {
                        vector3_ = aoe.Source.Location;
                    }
                    if (aoe.Location != null)
                    {
                        vector3_ = aoe.Location();
                    }
                    if (Class81.smethod_5(vector3_, aoe.Radius, start, loc) > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

         Определение направления DodgeSmart
         из Astral.Logic.NW.Movements
        private static Vector3 smart()
        {
            Vector3 playerPos = EntityManager.LocalPlayer.Location;
            float num = 30f;
            List<Vector3> list = new List<Vector3>();
            Vector3 shouldDodgeSource = Combats.ShouldDodgeSource;
            int num2 = 0;
            if (shouldDodgeSource.IsValid)
            {
                if (Combats.ShouldFrontDodge)
                {
                    num2 = 1;
                }
                else if (!Combats.AOEIsArc)
                {
                    if (shouldDodgeSource.IsInYawFaceByRange(1.5f, 3.40282347E+38f) && shouldDodgeSource.Distance3D(playerPos) > 5.0 && Combats.InitialCombatLoc.Distance3D(playerPos) < 100.0)
                    {
                        num2 = 2;
                    }
                    else if ((Combats.IsMeleeChar && shouldDodgeSource.Distance3D(playerPos) < 5.0) || shouldDodgeSource.IsInBackByRange(1.5f, 3.40282347E+38f))
                    {
                        num2 = 1;
                    }
                }
            }
            float num3 = 0f;
            while ((double)num3 < 6.2831853071795862)
            {
                float num4 = (float)Math.Cos((double)num3) * num;
                float num5 = (float)Math.Sin((double)num3) * num;
                Vector3 item = new Vector3(playerPos.X + num4, playerPos.Y + num5, playerPos.Z + 2f);
                list.Add(item);
                num3 += 0.314159274f;
            }
            list = (from i in list
                    orderby Guid.NewGuid()
                    select i).ToList<Vector3>();
            Vector3 vector = Vector3.Empty;
            List<Astral.Logic.NW.Movements.DodgeLosTestResult> list2 = new List<Astral.Logic.NW.Movements.DodgeLosTestResult>();
            if (EntityManager.CurrentSettings.UsePathfinding3)
            {
                using (List<Vector3>.Enumerator enumerator = list.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Vector3 vector2 = enumerator.Current;
                        Vector3 collidePos = Vector3.Empty;
                        bool collided = PathFinding.CheckDirection(playerPos, vector2, ref collidePos);
                        list2.Add(new Astral.Logic.NW.Movements.DodgeLosTestResult(vector2, collided, collidePos));
                    }
                    goto IL_2E7;
                }
            }
            List<Injection.RayCastParams> list3 = new List<Injection.RayCastParams>();
            Vector3 from = new Vector3(playerPos.X, playerPos.Y, playerPos.Z + 2f);
            foreach (Vector3 vector3 in list)
            {
                list3.Add(new Injection.RayCastParams(from, vector3));
                Vector3 to = new Vector3(vector3.X, vector3.Y, vector3.Z - 5f);
                list3.Add(new Injection.RayCastParams(vector3, to));
            }
            Injection.RayCastResult[] array = Injection.MassPosRayCast(list3.ToArray(), 142u);
            for (int j = 0; j < list.Count; j++)
            {
                Injection.RayCastResult rayCastResult = array[j + j];
                if (array[j + j + 1].collided)
                {
                    list2.Add(new Astral.Logic.NW.Movements.DodgeLosTestResult(list[j], rayCastResult.collided, rayCastResult.result));
                }
            }
            IL_2E7:
            Astral.Logic.NW.Movements.LastValidPoses = list2;
            Astral.Logic.NW.Movements.lastvlidposto.ChangeTime(2500);
            bool flag;
            if (!(flag = list2.Any((Astral.Logic.NW.Movements.DodgeLosTestResult r) => !r.Collided)))
            {
                list2 = (from r in list2
                         orderby r.CollidePos.Distance2D(playerPos) descending
                         select r).ToList<Astral.Logic.NW.Movements.DodgeLosTestResult>();
            }
            List<Entity> entities = EntityManager.GetEntities();
            float yaw = EntityManager.LocalPlayer.Yaw;
            using (List<Astral.Logic.NW.Movements.DodgeLosTestResult>.Enumerator enumerator2 = list2.GetEnumerator())
            {
                IL_490:
                while (enumerator2.MoveNext())
                {
                    Astral.Logic.NW.Movements.DodgeLosTestResult dodgeLosTestResult = enumerator2.Current;
                    Vector3 vector4 = dodgeLosTestResult.TestedPos;
                    if (!flag || !dodgeLosTestResult.Collided)
                    {
                        if (dodgeLosTestResult.Collided)
                        {
                            vector4 = dodgeLosTestResult.CollidePos;
                        }
                        if (!Combats.InitialCombatLoc.IsValid || Combats.InitialCombatLoc.Distance3D(playerPos) >= 150.0 || vector4.Distance3D(Combats.InitialCombatLoc) <= 70.0)
                        {
                            if (!vector.IsValid)
                            {
                                vector = vector4;
                            }
                            if ((num2 != 2 || !vector4.IsInYawFaceByRange(1.5f, 3.40282347E+38f)) && (num2 != 1 || !vector4.IsInBackByRange(1.5f, 3.40282347E+38f)) && (num2 != 0 || (!vector4.IsInYawFaceByRange(0.5f, 3.40282347E+38f) && !vector4.IsInBackByRange(0.5f, 3.40282347E+38f))))
                            {
                                using (List<AOECheck.AOE>.Enumerator enumerator3 = AOECheck.List.GetEnumerator())
                                {
                                    while (enumerator3.MoveNext())
                                    {
                                        if (enumerator3.Current.IsIn(vector4))
                                        {
                                            goto IL_490;
                                        }
                                    }
                                }
                                if (!Astral.Logic.NW.Movements.ThereIsDangerousEntities(vector4, 70.0, entities))
                                {
                                    return vector4;
                                }
                            }
                        }
                    }
                }
            }
            if (!vector.IsValid && list.Count > 0)
            {
                vector = list[0];
            }
            return vector;
        }   
#endif
    }
}
