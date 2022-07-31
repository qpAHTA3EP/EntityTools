using Astral.Classes;
using Astral.Logic.UCC.Actions;
using Astral.Logic.UCC.Classes;
using EntityCore.Entities;
using EntityTools;
using EntityTools.Core.Interfaces;
using EntityTools.UCC.Actions;
using MyNW.Classes;
using System;

namespace EntityCore.UCC.Actions
{
    class DodgeFromEntityEngine : IUccActionEngine
    {
        #region Данные
        private DodgeFromEntity @this;

        private Dodge dodge = new Dodge();

        private Entity entity = null;
        private Timeout timeout = new Timeout(0);
        private string _label = string.Empty;
        private string _idStr;
        #endregion

        internal DodgeFromEntityEngine(DodgeFromEntity dfe)
        {
            InternalRebase(dfe);
            ETLogger.WriteLine(LogType.Debug, $"{_idStr} initialized: {Label()}");
        }
        ~DodgeFromEntityEngine()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (@this != null)
            {
                @this.PropertyChanged -= PropertyChanged;
                @this.Engine = null;
                @this = null;
            }
            _key = null;
            _specialCheck = null;
            _label = string.Empty;
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (ReferenceEquals(sender, @this))
            {
                _key = null;
                _specialCheck = null;
                _label = string.Empty;

                entity = null;
                timeout.ChangeTime(0);
            }
        }

        public bool Rebase(UCCAction action)
        {
            if (action is null)
                return false;
            if (ReferenceEquals(action, @this))
                return true;
            if (action is DodgeFromEntity ettApproach)
            {
                if (InternalRebase(ettApproach))
                {
                    ETLogger.WriteLine(LogType.Debug, $"{_idStr} reinitialized");
                    return true;
                }
                ETLogger.WriteLine(LogType.Debug, $"{_idStr} rebase failed");
                return false;
            }

            string debugStr = string.Concat("Rebase failed. ", action.GetType().Name, '[', action.GetHashCode().ToString("X2"), "] can't be casted to '" + nameof(DodgeFromEntity) + '\'');
            ETLogger.WriteLine(LogType.Error, debugStr);
            throw new InvalidCastException(debugStr);
        }

        private bool InternalRebase(DodgeFromEntity dfe)
        {
            // Убираем привязку к старому условию
            if (@this != null)
            {
                @this.PropertyChanged -= PropertyChanged;
                @this.Engine = null;
            }

            @this = dfe;
            @this.PropertyChanged += PropertyChanged;

            _key = null;
            _specialCheck = null;
            _label = string.Empty;

            _idStr = string.Concat(@this.GetType().Name, '[', @this.GetHashCode().ToString("X2"), ']');

            @this.Engine = this;

            return true;
        }

        #region IUCCActionEngine
        public bool NeedToRun
        {
            get
            {
                if (!string.IsNullOrEmpty(@this._entityId))
                {
                    var entityKey = EntityKey;

                    if (timeout.IsTimedOut)
                    {
                        entity = SearchCached.FindClosestEntity(entityKey, SpecialCheck);

                        timeout.ChangeTime(EntityTools.EntityTools.Config.EntityCache.CombatCacheTime);
                    }

                    return entityKey.Validate(entity) && entity.Location.Distance3DFromPlayer <= @this._entityRadius;
                }
                return false;
            }
        }

        public bool Run()
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
                MyNW.Internals.Movements.StopNavTo();

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
                    if (!string.IsNullOrEmpty(@this._entityId))
                    {
                        entity = SearchCached.FindClosestEntity(entityKey, 
                                                                SpecialCheck);
                        return entity;
                    }
                }
                return null;
            }
        }

        public string Label()
        {
            if (string.IsNullOrEmpty(_label))
            {
                if (string.IsNullOrEmpty(@this._entityId))
                    _label = @this.GetType().Name;
                else _label = $"{@this.GetType().Name} [{@this._entityId}]";
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
        public EntityCacheRecordKey EntityKey
        {
            get
            {
                if (_key is null)
                    _key = new EntityCacheRecordKey(@this._entityId, @this._entityIdType, @this._entityNameType);
                return _key;
            }
        }
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
                    _specialCheck = SearchHelper.Construct_EntityAttributePredicate(@this._healthCheck,
                                                            @this._reactionRange,
                                                            @this._reactionZRange > 0 ? @this._reactionZRange : Astral.Controllers.Settings.Get.MaxElevationDifference,
                                                            @this._regionCheck,
                                                            @this._aura.IsMatch);
                return _specialCheck;
            }
        }
        private Predicate<Entity> _specialCheck;
        #endregion

        #region Копия кода dodge
        //private bool havewaitdel(Func<bool> del)
        //{
        //    return del != null && del();
        //}

        //private bool pathtToLocIsInAOE(Vector3 start, Vector3 loc)
        //{

        //    Vector3 vector3_ = Vector3.Empty;
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
        //    Vector3 vector = Vector3.Empty;
        //    List<Astral.Logic.NW.Movements.DodgeLosTestResult> list2 = new List<Astral.Logic.NW.Movements.DodgeLosTestResult>();
        //    if (EntityManager.CurrentSettings.UsePathfinding3)
        //    {
        //        using (List<Vector3>.Enumerator enumerator = list.GetEnumerator())
        //        {
        //            while (enumerator.MoveNext())
        //            {
        //                Vector3 vector2 = enumerator.Current;
        //                Vector3 collidePos = Vector3.Empty;
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
        #endregion
    }
}
