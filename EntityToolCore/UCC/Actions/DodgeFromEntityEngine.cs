using Astral.Classes;
using Astral.Logic.UCC.Actions;
using EntityCore.Entities;
using EntityTools.Core.Interfaces;
using EntityTools.Enums;
using EntityTools.Logger;
using EntityTools.UCC.Actions;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityCore.UCC.Actions
{
    class DodgeFromEntityEngine : IEntityInfos
#if CORE_INTERFACES
        , IUCCActionEngine
#endif
    {
        #region Данные
        private DodgeFromEntity @this;

        private Dodge dodge = new Dodge();
        /// <summary>
        /// Функтор, предназначенный для проверки соответствия Entity
        /// основным критериям: EntityID, EntityIdType, EntityNameType
        /// </summary>
        private Predicate<Entity> checkEntity { get; set; } = null;
        private Entity entity = null;
        private Timeout timeout = new Timeout(0);
        private string label = string.Empty; 
        #endregion

        internal DodgeFromEntityEngine(DodgeFromEntity dfe)
        {
            @this = dfe;
#if CORE_INTERFACES
            @this.Engine = this;
#endif
            @this.PropertyChanged += PropertyChanged;

            checkEntity = internal_CheckEntity_Initializer;

            EntityToolsLogger.WriteLine(LogType.Debug, $"{@this.GetType().Name}[{@this.GetHashCode().ToString("X2")}] initialized");
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (object.ReferenceEquals(sender, @this))
            {
                switch (e.PropertyName)
                {
                    case "EntityID":
                        checkEntity = internal_CheckEntity_Initializer; //EntityToPatternComparer.Get(@this._entityId, @this._entityIdType, @this._entityNameType);
                        label = string.Empty;
                        break;
                    case "EntityIdType":
                        checkEntity = internal_CheckEntity_Initializer; //EntityToPatternComparer.Get(@this._entityId, @this._entityIdType, @this._entityNameType);
                        break;
                    case "EntityNameType":
                        checkEntity = internal_CheckEntity_Initializer; //EntityToPatternComparer.Get(@this._entityId, @this._entityIdType, @this._entityNameType);
                        break;
                }
                entity = null;
                timeout.ChangeTime(0);
            }
        }

        #region IUCCActionEngine
        public bool NeedToRun
        {
            get
            {
                if (!string.IsNullOrEmpty(@this._entityId))
                {
                    if (timeout.IsTimedOut)
                    {
                        entity = SearchCached.FindClosestEntity(@this._entityId, @this._entityIdType, @this._entityNameType, EntitySetType.Complete,
                                                                @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck);
                        timeout.ChangeTime(EntityTools.EntityTools.PluginSettings.EntityCache.CombatCacheTime);
                    }

                    return ValidateEntity(entity) && entity.Location.Distance3DFromPlayer <= @this._entityRadius;
                }
                return false;
            }
        }

        public bool Run()
        {
            if (entity.Location.Distance3DFromPlayer < @this._entityRadius)
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

        public Entity UnitRef
        {
            get
            {
                if (ValidateEntity(entity))
                    return entity;
                else
                {
                    if (!string.IsNullOrEmpty(@this._entityId))
                    {
                        entity = SearchCached.FindClosestEntity(@this._entityId, @this._entityIdType, @this._entityNameType, EntitySetType.Complete,
                                                                @this._healthCheck, @this._reactionRange,
                                                                (@this._reactionZRange > 0) ? @this._reactionZRange : Astral.Controllers.Settings.Get.MaxElevationDifference,
                                                                @this._regionCheck, null, @this._aura.Checker);
                        return entity;
                    }
                }
                return null;
            }
        }

        public string Label()
        {
            if (string.IsNullOrEmpty(label))
            {
                if (string.IsNullOrEmpty(@this._entityId))
                    label = @this.GetType().Name;
                else label = $"{@this.GetType().Name} [{@this._entityId}]";
            }
            return label;
        } 
        #endregion

        /// <summary>
        /// Проверка валидности цели
        /// Флаг IsValid не гарантирует, что ранее найденный Entity ссылается на ту же сущность
        /// поскольку игровой клиент может её подменить
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool ValidateEntity(Entity e)
        {
            return e != null && e.IsValid && checkEntity(e);
        }

        private bool internal_CheckEntity_Initializer(Entity e)
        {
            Predicate<Entity> predicate = EntityToPatternComparer.Get(@this._entityId, @this._entityIdType, @this._entityNameType);
            if (predicate != null)
            {
#if DEBUG
                EntityToolsLogger.WriteLine(LogType.Debug, $"{GetType().Name}[{this.GetHashCode().ToString("X2")}]: Comparer does not defined. Initialize.");
#endif
                checkEntity = predicate;
                return checkEntity(e);
            }
#if DEBUG
            else EntityToolsLogger.WriteLine(LogType.Error, $"{GetType().Name}[{this.GetHashCode().ToString("X2")}]: Fail to initialize the Comparer.");
#endif
            return false;
        }

        #region Копия кода dodge
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
        #endregion

        public bool EntityDiagnosticString(out string infoString)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("EntityID: ").AppendLine(@this._entityId);
            sb.Append("EntityIdType: ").AppendLine(@this._entityIdType.ToString());
            sb.Append("EntityNameType: ").AppendLine(@this._entityNameType.ToString());
            //sb.Append("EntitySetType: ").AppendLine(@this._entitySetType.ToString());
            sb.Append("HealthCheck: ").AppendLine(@this._healthCheck.ToString());
            sb.Append("ReactionRange: ").AppendLine(@this._reactionRange.ToString());
            sb.Append("ReactionZRange: ").AppendLine(@this._reactionZRange.ToString());
            sb.Append("RegionCheck: ").AppendLine(@this._regionCheck.ToString());
            sb.Append("Aura: ").AppendLine(@this._aura.ToString());
            //if (@this._customRegionNames != null && @this._customRegionNames.Count > 0)
            //{
            //    sb.Append("RegionCheck: {").Append(@this._customRegionNames[0]);
            //    for (int i = 1; i < @this._customRegionNames.Count; i++)
            //        sb.Append(", ").Append(@this._customRegionNames[i]);
            //    sb.AppendLine("}");
            //}
            sb.AppendLine();
            //sb.Append("NeedToRun: ").AppendLine(NeedToRun.ToString());
            //sb.AppendLine();

            // список всех Entity, удовлетворяющих условиям
            LinkedList<Entity> entities = SearchCached.FindAllEntity(@this._entityId, @this._entityIdType, @this._entityNameType, EntitySetType.Complete,
                                                                     @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck);


            // Количество Entity, удовлетворяющих условиям
            if (entities != null)
                sb.Append("Founded Entities: ").AppendLine(entities.Count.ToString());
            else sb.Append("Founded Entities: 0");
            sb.AppendLine();

            // Ближайшее Entity (найдено при вызове ie.NeedToRun, поэтому строка ниже закомментирована)
            entity = SearchCached.FindClosestEntity(@this._entityId, @this._entityIdType, @this._entityNameType, EntitySetType.Complete,
                                                    @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck);
            if (entity != null)
            {
                sb.Append("Target: ").AppendLine(entity.ToString());
                sb.Append("\tName: ").AppendLine(entity.Name);
                sb.Append("\tInternalName: ").AppendLine(entity.InternalName);
                sb.Append("\tNameUntranslated: ").AppendLine(entity.NameUntranslated);
                sb.Append("\tIsDead: ").AppendLine(entity.IsDead.ToString());
                sb.Append("\tRegion: '").Append(entity.RegionInternalName).AppendLine("'");
                sb.Append("\tLocation: ").AppendLine(entity.Location.ToString());
                sb.Append("\tDistance: ").AppendLine(entity.Location.Distance3DFromPlayer.ToString());
                sb.Append("\tZAxisDiff: ").AppendLine(Astral.Logic.General.ZAxisDiffFromPlayer(entity.Location).ToString());
            }
            else sb.AppendLine("Closest Entity not found!");

            infoString = sb.ToString();
            return true;
        }
    }
}
