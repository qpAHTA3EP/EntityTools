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
using EntityTools.Logger;

namespace EntityCore.UCC.Conditions
{
    public class UCCTargetMatchEntityEngine : IUCCConditionEngine
    {
        #region Данные
        private UCCTargetMatchEntity @this;

        private Predicate<Entity> checkEntity = null;

        private string label = string.Empty;
        #endregion

        internal UCCTargetMatchEntityEngine(UCCTargetMatchEntity tarMatch)
        {
            @this = tarMatch;
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
            }
        }

        #region MyRegion
        public bool IsOK(UCCAction refAction)
        {
            Entity target = refAction?.GetTarget();

            switch (@this._match)
            {
                case MatchType.Match:
                    return ValidateEntity(target);
                case MatchType.Mismatch:
                    return !ValidateEntity(target);
            }
            return false;
        }

        public string TestInfos(UCCAction refAction)
        {
            Entity target = refAction?.GetTarget();

            StringBuilder sb = new StringBuilder("Target ");
            if (target != null)
            {
                if (@this._entityNameType == EntityNameType.InternalName)
                    sb.Append('[').Append(target.InternalName).Append(']');
                else sb.Append('[').Append(target.NameUntranslated).Append(']');
                if (ValidateEntity(target))
                    sb.Append(" match");
                else sb.Append(" does not match");
                sb.Append(" EntityID [").Append(@this._entityId).Append(']');
            }
            else sb.Append("is NULL");

            return sb.ToString();

            return "Condition options is invalid!";
        }

        public string Label()
        {
            if (string.IsNullOrEmpty(label))
                label = $"{@this.GetType().Name} [{@this._entityId}]";
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
    }
}
