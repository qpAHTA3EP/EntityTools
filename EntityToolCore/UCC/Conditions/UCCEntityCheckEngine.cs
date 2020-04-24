using Astral.Logic.UCC.Classes;
using EntityCore.Entities;
using EntityTools.Core.Interfaces;
using EntityTools.Enums;
using EntityTools.UCC.Conditions;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using EntityTools.UCC.Extensions;
using Astral.Classes;
using EntityTools;

namespace EntityCore.UCC.Conditions
{
    public class UCCEntityCheckEngine : IUCCConditionEngine
    {
        #region Данные
        private UCCEntityCheck @this;

        private Predicate<Entity> checkEntity { get; set; } = null;
        private Entity entity = null;
        private Timeout timeout = new Timeout(0);
        private string label = string.Empty;
        #endregion

        internal UCCEntityCheckEngine(UCCEntityCheck eck)
        {
            @this = eck;
#if CORE_INTERFACES
            @this.Engine = this;
#endif
            @this.PropertyChanged += PropertyChanged;

            checkEntity = internal_CheckEntity_Initializer;

            ETLogger.WriteLine(LogType.Debug, $"{@this.GetType().Name}[{@this.GetHashCode().ToString("X2")}] initialized: {Label()}");
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
            }
        }

        #region MyRegion
        public bool IsOK(UCCAction refAction)
        {
            Entity targetEntity = refAction?.GetTarget();

            if (ValidateEntity(targetEntity))
                entity = targetEntity;
            else if (timeout.IsTimedOut)
            {
                entity = SearchCached.FindClosestEntity(@this._entityId, @this._entityIdType, @this._entityNameType, EntitySetType.Complete,
                                                        @this._healthCheck, @this._reactionRange,
                                                        (@this._reactionZRange > 0) ? @this._reactionZRange : Astral.Controllers.Settings.Get.MaxElevationDifference,
                                                        @this._regionCheck, null, @this._aura.Checker);
            }


            bool result = false;
            if (ValidateEntity(entity))
            {
                switch (@this._propertyType)
                {
                    case EntityPropertyType.Distance:
                        switch (@this.Sign)
                        {
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Equal:
                                return result = entity.Location.Distance3DFromPlayer == @this._propertyValue;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.NotEqual:
                                return result = entity.Location.Distance3DFromPlayer != @this._propertyValue;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Inferior:
                                return result = entity.Location.Distance3DFromPlayer < @this._propertyValue;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Superior:
                                return result = entity.Location.Distance3DFromPlayer > @this._propertyValue;
                        }
                        break;
                    case EntityPropertyType.HealthPercent:
                        switch (@this.Sign)
                        {
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Equal:
                                return result = entity.Character?.AttribsBasic?.HealthPercent == @this._propertyValue;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.NotEqual:
                                return result = entity.Character?.AttribsBasic?.HealthPercent != @this._propertyValue;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Inferior:
                                return result = entity.Character?.AttribsBasic?.HealthPercent < @this._propertyValue;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Superior:
                                return result = entity.Character?.AttribsBasic?.HealthPercent > @this._propertyValue;
                        }
                        break;
                    case EntityPropertyType.ZAxis:
                        switch (@this.Sign)
                        {
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Equal:
                                return result = entity.Location.Z == @this._propertyValue;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.NotEqual:
                                return result = entity.Location.Z != @this._propertyValue;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Inferior:
                                return result = entity.Location.Z < @this._propertyValue;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Superior:
                                return result = entity.Location.Z > @this._propertyValue;
                        }
                        break;
                }
                return result;
            }
            // Если Entity не найдено, условие будет истино в единственном случае:
            else return @this._propertyType == EntityPropertyType.Distance
                        && @this.Sign == Astral.Logic.UCC.Ressources.Enums.Sign.Superior;
        }

        public string TestInfos(UCCAction refAction)
        {
            Entity closestEntity = refAction?.GetTarget();

            if (!ValidateEntity(closestEntity))
                closestEntity = SearchCached.FindClosestEntity(@this._entityId, @this._entityIdType, @this._entityNameType, EntitySetType.Complete,
                                                               @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck, null, @this._aura.Checker);

            if (ValidateEntity(closestEntity))
            {
                StringBuilder sb = new StringBuilder("Found closect Entity");
                sb.Append(" [").Append(closestEntity.NameUntranslated).Append(']').Append(" which ").Append(@this._propertyType).Append(" = ");
                switch (@this._propertyType)
                {
                    case EntityPropertyType.Distance:
                        sb.Append(closestEntity.Location.Distance3DFromPlayer);
                        break;
                    case EntityPropertyType.ZAxis:
                        sb.Append(closestEntity.Location.Z);
                        break;
                    case EntityPropertyType.HealthPercent:
                        sb.Append(closestEntity.Character?.AttribsBasic?.HealthPercent);
                        break;
                }
                return sb.ToString();
            }
            else
            {
                StringBuilder sb = new StringBuilder("No one Entity matched to");
                sb.Append(" [").Append(@this._entityId).Append(']').AppendLine();
                if (@this._propertyType == EntityPropertyType.Distance)
                    sb.AppendLine("The distance to the missing Entity is considered equal to infinity.");
                return sb.ToString();
                //return string.Concat("No one Entity matched to [", @this._entityId, ']',Environment.NewLine, (@this._propertyType == EntityPropertyType.Distance)? "The distance to the missing Entity is considered equal to infinity." : string.Empty, "The distance to the missing Entity is considered equal to infinity.");
            }
        }

        public string Label()
        {
            if (string.IsNullOrEmpty(label))
            {
                label = $"{@this.GetType().Name} [{@this._entityId}]";
            }
            else label = @this.GetType().Name;

            return label;
        }
        #endregion

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
                ETLogger.WriteLine(LogType.Debug, $"{GetType().Name}[{this.GetHashCode().ToString("X2")}]: Comparer does not defined. Initialize.");
#endif
                checkEntity = predicate;
                return checkEntity(e);
            }
#if DEBUG
            else ETLogger.WriteLine(LogType.Error, $"{GetType().Name}[{this.GetHashCode().ToString("X2")}]: Fail to initialize the Comparer.");
#endif
            return false;
        }
    }
}
