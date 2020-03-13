using Astral;
using Astral.Classes;
using Astral.Quester.Classes;
using EntityCore.Entities;
using EntityTools.Extentions;
using EntityTools.Enums;
using EntityTools.Quester.Actions;
using MyNW.Classes;
using MyNW.Patchables.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using static Astral.Quester.Classes.Action;

namespace EntityCore.Quester.Action
{
    public class MoveToEntityEngine : IQuesterActionEngine, IEntityInfos
    {
        private MoveToEntity @this;
        internal Predicate<Entity> Comparer { get; private set; } = null;
        private List<CustomRegion> customRegions = new List<CustomRegion>();
        private Entity target;
        private Timeout timeout = new Timeout(0);

        internal MoveToEntityEngine(MoveToEntity m2e)
        {
            @this = m2e;
            @this.coreNeedToRun = NeedToRun;
            @this.coreRun = Run;
            @this.coreValidate = Validate;
            @this.coreReset = Reset;
            @this.coreGatherInfos = GatherInfos;
            @this.getString = GetString;
            @this.getTarget = Target;

            @this.PropertyChanged += PropertyChanged;
        }

        internal void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(object.ReferenceEquals(sender, @this))
            {
                switch(e.PropertyName)
                {
                    case "EntityID":
                        Comparer = EntityToPatternComparer.Get(@this.EntityID, @this.EntityIdType, @this.EntityNameType);
                        break;
                    case "EntityIdType":
                        Comparer = EntityToPatternComparer.Get(@this.EntityID, @this.EntityIdType, @this.EntityNameType);
                        break;
                    case "EntityNameType":
                        Comparer = EntityToPatternComparer.Get(@this.EntityID, @this.EntityIdType, @this.EntityNameType);
                        break;
                    case "CustomRegionNames":
                        customRegions = CustomRegionExtentions.GetCustomRegions(@this.CustomRegionNames);
                        break;
                }
            }
        }

        internal bool NeedToRun()
        {
            //Команда работает с 2 - мя целями:
            //1 - я цель (target) определяет навигацию. Если она зафиксированная(HoldTargetEntity), то не сбрасывается пока жива и не достигнута
            //2 - я ближайшая цель (closest) управляет флагом IgnoreCombat
            //Если HoldTargetEntity ВЫКЛЮЧЕН, то обе цели совпадают - это ближайшая цель 

            if (customRegions == null)
                customRegions = CustomRegionExtentions.GetCustomRegions(@this.CustomRegionNames);

            if (Comparer == null && !string.IsNullOrEmpty(@this.EntityID))
            {
#if DEBUG
                Logger.WriteLine(Logger.LogType.Debug, "MoveToEntityEngine::NeedToRun: Comparer is null. Initialize.");
#endif
                Comparer = EntityToPatternComparer.Get(@this.EntityID, @this.EntityIdType, @this.EntityNameType);
            }


            Entity closestEntity = null;
            if (timeout.IsTimedOut/* || (target != null && (!Validate(target) || (HealthCheck && target.IsDead)))*/)
            {
                closestEntity = SearchCached.FindClosestEntity(@this.EntityID, @this.EntityIdType, @this.EntityNameType, EntitySetType.Complete,
                                                            @this.HealthCheck, @this.ReactionRange, @this.ReactionZRange, @this.RegionCheck, customRegions);
                timeout.ChangeTime(@this.SearchTimeInterval);
            }

            if (!@this.HoldTargetEntity || !Validate(target) || (@this.HealthCheck && target.IsDead))
                target = closestEntity;

            if (Validate(target)
                && !(@this.HealthCheck && target.IsDead)
                && (target.Location.Distance3DFromPlayer <= @this.Distance))
            {
                if (@this.AttackTargetEntity)
                {
                    Astral.Logic.NW.Attackers.List.Clear();
                    if (@this.AttackTargetEntity && target.RelationToPlayer == EntityRelation.Foe)
                    {
                        Astral.Logic.NW.Attackers.List.Add(target);
                        if (@this.IgnoreCombat)
                            Astral.Quester.API.IgnoreCombat = false;
                        Astral.Logic.NW.Combats.CombatUnit(target, null);
                    }
                }
                else if (@this.IgnoreCombat)
                    Astral.Quester.API.IgnoreCombat = false;
            }
            else if (Validate(closestEntity)
                     && !(@this.HealthCheck && closestEntity.IsDead)
                     && (closestEntity.Location.Distance3DFromPlayer <= @this.Distance))
            {
                if (@this.AttackTargetEntity)
                {
                    Astral.Logic.NW.Attackers.List.Clear();
                    if (@this.AttackTargetEntity && closestEntity.RelationToPlayer != EntityRelation.Friend)
                    {
                        Astral.Logic.NW.Attackers.List.Add(closestEntity);
                        if (@this.IgnoreCombat)
                            Astral.Quester.API.IgnoreCombat = false;
                        Astral.Logic.NW.Combats.CombatUnit(closestEntity, null);
                    }
                }
                else if (@this.IgnoreCombat)
                    Astral.Quester.API.IgnoreCombat = false;
            }
            else if (@this.IgnoreCombat)
                Astral.Quester.API.IgnoreCombat = true;

            return (Validate(target) && (target.Location.Distance3DFromPlayer < @this.Distance));
        }

        internal ActionResult Run()
        {
            if (@this.AttackTargetEntity)
            {
                Astral.Logic.NW.Attackers.List.Clear();
                if (@this.AttackTargetEntity)
                {
                    Astral.Logic.NW.Attackers.List.Add(target);
                    if (@this.IgnoreCombat)
                        Astral.Quester.API.IgnoreCombat = false;
                    Astral.Logic.NW.Combats.CombatUnit(target, null);
                }
                else Astral.Quester.API.IgnoreCombat = false;
            }

            if (@this.IgnoreCombat)
                Astral.Quester.API.IgnoreCombat = false;

            if (@this.StopOnApproached)
                return ActionResult.Completed;
            else return ActionResult.Running;
        }

        internal Entity Target()
        {
            return target;
        }

        internal bool Validate()
        {
            return Validate(target);
        }

        internal bool Validate(Entity e)
        {
            return e != null && e.IsValid && (Comparer?.Invoke(e) == true);
        }

        public bool EntityDiagnosticString(out string infoString)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("EntityID: ").AppendLine(@this.EntityID);
            sb.Append("EntityIdType: ").AppendLine(@this.EntityIdType.ToString());
            sb.Append("EntityNameType: ").AppendLine(@this.EntityNameType.ToString());
            sb.Append("HealthCheck: ").AppendLine(@this.HealthCheck.ToString());
            sb.Append("ReactionRange: ").AppendLine(@this.ReactionRange.ToString());
            sb.Append("ReactionZRange: ").AppendLine(@this.ReactionZRange.ToString());
            sb.Append("RegionCheck: ").AppendLine(@this.RegionCheck.ToString());
            if (@this.CustomRegionNames != null && @this.CustomRegionNames.Count > 0)
            {
                sb.Append("RegionCheck: {").Append(@this.CustomRegionNames[0]);
                for (int i = 1; i < @this.CustomRegionNames.Count; i++)
                    sb.Append(", ").Append(@this.CustomRegionNames[i]);
                sb.AppendLine("}");
            }
            sb.AppendLine();
            sb.Append("NeedToRun: ").AppendLine(NeedToRun.ToString());
            sb.AppendLine();
            // список всех Entity, удовлетворяющих условиям
            LinkedList<Entity> entities = SearchCached.FindAllEntity(@this.EntityID, @this.EntityIdType, @this.EntityNameType, EntitySetType.Complete,
                @this.HealthCheck, @this.ReactionRange, @this.ReactionZRange, @this.RegionCheck, customRegions);

            // Количество Entity, удовлетворяющих условиям
            if (entities != null)
                sb.Append("Founded Entities: ").AppendLine(entities.Count.ToString());
            else sb.Append("Founded Entities: 0");
            sb.AppendLine();

            /// Ближайшее Entity (найдено при вызове mte.NeedToRun, поэтому строка ниже закомментирована)
            //target = EntitySelectionTools.FindClosestEntity(entities, EntityId, EntityIdType, NameType, HealthCheck, ReactionRange, RegionCheck, CustomRegionNames);
            if (target != null && target.IsValid)
            {
                //sb.Append("ClosectEntity: ").AppendLine(mte.target.ToString());
                //sb.Append("\tName: ").AppendLine(mte.target.Name);
                //sb.Append("\tInternalName: ").AppendLine(mte.target.InternalName);
                //sb.Append("\tNameUntranslated: ").AppendLine(mte.target.NameUntranslated);
                //sb.Append("\t[").Append(!(mte.HealthCheck && mte.target.IsDead) ? "+" : "-")
                //    .Append("]IsDead: ").AppendLine(mte.target.IsDead.ToString());
                //sb.Append("\t[").Append((!mte.RegionCheck || mte.target.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName) ? "+" : "-")
                //    .Append("]Region: '").Append(mte.target.RegionInternalName).AppendLine("'");
                //sb.Append("\tLocation: ").AppendLine(mte.target.Location.ToString());
                //sb.Append("\t[").Append((mte.ReactionRange == 0 || mte.target.Location.Distance3DFromPlayer < mte.ReactionRange) ? "+" : "-")
                //    .Append("]Distance: ").AppendLine(mte.target.Location.Distance3DFromPlayer.ToString());
                sb.Append("Target: ").AppendLine(target.ToString());
                sb.Append("\tName: ").AppendLine(target.Name);
                sb.Append("\tInternalName: ").AppendLine(target.InternalName);
                sb.Append("\tNameUntranslated: ").AppendLine(target.NameUntranslated);
                sb.Append("\tIsDead: ").AppendLine(target.IsDead.ToString());
                sb.Append("\tRegion: '").Append(target.RegionInternalName).AppendLine("'");
                sb.Append("\tLocation: ").AppendLine(target.Location.ToString());
                sb.Append("\tDistance: ").AppendLine(target.Location.Distance3DFromPlayer.ToString());
                sb.Append("\tZAxisDiff: ").AppendLine(Astral.Logic.General.ZAxisDiffFromPlayer(target.Location).ToString());
            }
            else sb.AppendLine("Closest Entity not found!");

            infoString = sb.ToString();
            return true;
        }
    }
}
