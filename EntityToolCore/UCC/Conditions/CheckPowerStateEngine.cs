using Astral.Logic.UCC.Classes;
using EntityCore.Tools.Powers;
using EntityTools;
using EntityTools.Core.Interfaces;
using EntityTools.Enums;
using EntityTools.UCC.Conditions;
using System;

namespace EntityCore.UCC.Conditions
{
    public class CheckPowerStateEngine : IUccConditionEngine
    {
        #region Данные
        private CheckPowerState @this;
        private readonly PowerCache powerCache = new PowerCache(string.Empty);

#if false
        private Predicate<Entity> checkEntity = null; 
#endif

        private string label = string.Empty;
        private string _idStr;
        #endregion

        internal CheckPowerStateEngine(CheckPowerState pwrState)
        {
            InternalRebase(pwrState);
            ETLogger.WriteLine(LogType.Debug, $"{_idStr} initialized: {Label()}");
        }
        ~CheckPowerStateEngine()
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
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (ReferenceEquals(sender, @this))
            {
#if false
                switch (e.PropertyName)
                {
                    case nameof(@this.EntityID):
                        checkEntity = initialize_CheckEntity;
                        label = string.Empty;
                        break;
                    case nameof(@this.EntityIdType):
                        checkEntity = initialize_CheckEntity;
                        break;
                    case nameof(@this.EntityNameType):
                        checkEntity = initialize_CheckEntity;
                        break;
                } 
#else
                if(e.PropertyName == nameof(CheckPowerState.PowerId))
                    powerCache.PowerIdPattern = @this.PowerId;
#endif
            }
        }

        public bool Rebase(UCCCondition condition)
        {
            if (condition is null)
                return false;
            if (ReferenceEquals(condition, @this))
                return true;
            if (condition is CheckPowerState pwrState)
            {
                if (InternalRebase(pwrState))
                {
                    ETLogger.WriteLine(LogType.Debug, $"{_idStr} reinitialized");
                    return true;
                }
                ETLogger.WriteLine(LogType.Debug, $"{_idStr} rebase failed");
                return false;
            }

            string debugStr = string.Concat("Rebase failed. ", condition.GetType().Name, '[', condition.GetHashCode().ToString("X2"), "] can't be casted to '" + nameof(CheckPowerState) + '\'');
            ETLogger.WriteLine(LogType.Error, debugStr);
            throw new InvalidCastException(debugStr);
        }

        private bool InternalRebase(CheckPowerState pwrState)
        {
            // Убираем привязку к старому условию
            if (@this != null)
            {
                @this.PropertyChanged -= PropertyChanged;
                @this.Engine = null;
            }

            @this = pwrState;
            @this.PropertyChanged += PropertyChanged;

            _idStr = string.Concat(@this.GetType().Name, '[', @this.GetHashCode().ToString("X2"), ']');

            @this.Engine = this;

            return true;
        }

        #region Вспомогательные методы
        public bool IsOK(UCCAction refAction)
        {
            var pwr = powerCache.GetPower();

            switch (@this.CheckState)
            {
                case PowerState.HasPower:
                    return pwr != null && pwr.IsValid;
                case PowerState.HasntPower:
                    return pwr == null || !pwr.IsValid;
            }

            return false;
        }

        public string TestInfos(UCCAction refAction)
        {
            var pwr = powerCache.GetPower();

            if (pwr != null && pwr.IsValid)
            {
                var pwrDef = pwr.PowerDef;
                return $"Character has Power {pwrDef.DisplayName}[{pwrDef.InternalName}].";
            }

            return $"No Power [{@this.PowerId}] was found.";
        }

        public string Label()
        {
            return $"CheckPowerState : {@this.CheckState} [{@this.PowerId}]";
        }
        #endregion
    }
}
