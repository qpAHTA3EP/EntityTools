#define DEBUG_POWERCACHE
using System;
using EntityTools.Core.Interfaces;
using MyNW.Internals;

namespace EntityTools.Tools.Powers
{
    /// <summary>
    /// Класс, кэширующий <see cref="MyNW.Classes.Power"/>
    /// </summary>
    public class PowerCache : IPowerCache
    {
        public PowerCache(string powId)
        {
            powerIdPattern = powId;
            if (string.IsNullOrEmpty(powId))
            {
                initialized = false;
                checker = (p) => false;
            }
            else
            {
                checker = powId.GetComparer();
                initialized = true;
            }
        }

        /// <summary>
        /// Идентификатор игрового клиента для отслеживания актуальности кэша
        /// </summary>
        private int cachedAttachedGameProcessId;
        /// <summary>
        /// Идентификатор персонажа для отслеживания актуальности кэша
        /// </summary>
        private uint cachedCharacterContainerId;
        /// <summary>
        /// Идентификатор умения для отслеживания актуальности кэша
        /// </summary>
        private uint cachedPowerId;
        /// <summary>
        /// Кэшированное умение
        /// </summary>
        private MyNW.Classes.Power cachedPower;
        /// <summary>
        /// Функтор для проверки соответствия имени умения <see cref="PowerIdPattern"/>
        /// </summary>
        private Predicate<string> checker;

        /// <summary>
        /// Корректности инициализации параметров кэша.
        /// Эквивалентно проверке <code>string.IsNullOfEmpty(<see cref="PowerIdPattern"/>)</code>
        /// </summary>
        public bool IsInitialized => initialized;
        private bool initialized;

#if false
        /// <summary>
        /// Проверка актуальности кэша
        /// </summary>
        public bool IsValid
        {
            get
            {
                var player = EntityManager.LocalPlayer;
                var processId = Astral.API.AttachedGameProcess.Id;

                return !(attachedGameProcessId == processId
                         && characterContainerId == player.ContainerId
                         && power != null
                         && (power.PowerId == powerId
                             || checker(power.PowerDef.InternalName)
                             || checker(power.EffectivePowerDef().InternalName)));
            }
        } 
#endif

        /// <summary>
        /// Шаблон идентификатора кэшируемого умения
        /// Допускается использования символа подстановки '*' в начале и в конце.
        /// </summary>
        public string PowerIdPattern
        {
            get => powerIdPattern;
            set
            {
                if (powerIdPattern != value)
                {
                    powerIdPattern = value;
                    if (string.IsNullOrEmpty(powerIdPattern))
                    {
                        checker = (p) => false;
                        initialized = false;
                    }
                    else
                    {
                        checker = powerIdPattern.GetComparer();
                        initialized = true;
                    }
                }
            }
        }
        private string powerIdPattern;

        /// <summary>
        /// Кэшированное <see cref="MyNW.Classes.Power"/>
        /// </summary>
        public MyNW.Classes.Power Power => GetPower();

        public void Reset(string powerPattern = default) => PowerIdPattern = powerPattern;

        /// <summary>
        /// Кэшированное <see cref="MyNW.Classes.Power"/>
        /// </summary>
        public MyNW.Classes.Power GetPower()
        {
            if (!initialized)
            {
#if DEBUG_POWERCACHE
                ETLogger.WriteLine(LogType.Debug, $"{nameof(PowerCache)}: Not initialized.");
#endif
                return null;
            }

            var player = EntityManager.LocalPlayer;
            var processId = Astral.API.AttachedGameProcess.Id;

            if (!(cachedAttachedGameProcessId == processId
                  && cachedCharacterContainerId == player.ContainerId
                  && cachedPower != null
                  && (cachedPower.PowerId == cachedPowerId
                      || checker(cachedPower.PowerDef.InternalName)
                      || checker(cachedPower.EffectivePowerDef().InternalName))))
            {
#if DEBUG_POWERCACHE
                ETLogger.WriteLine(LogType.Debug, $"{nameof(PowerCache)}: Regen cache.");
#endif
                //power = Powers.GetPowerByInternalName(powId);
                cachedPower = SearchPower();
                if (cachedPower != null)
                {
#if DEBUG_POWERCACHE
                    ETLogger.WriteLine(LogType.Debug, $"{nameof(PowerCache)}: Found Power {cachedPower.PowerDef.FullDisplayName}[{cachedPower.Pointer:X8}].");
#endif
                    cachedPowerId = cachedPower.PowerId;
                    cachedAttachedGameProcessId = processId;
                    cachedCharacterContainerId = player.ContainerId;
                }
                else
                {
#if DEBUG_POWERCACHE
                    ETLogger.WriteLine(LogType.Debug, $"{nameof(PowerCache)}: No Power {powerIdPattern} was found.");
#endif
                    cachedPowerId = 0;
                    cachedAttachedGameProcessId = 0;
                    cachedCharacterContainerId = 0;
                }
            }
#if DEBUG_POWERCACHE
            else
                ETLogger.WriteLine(LogType.Debug, $"{nameof(PowerCache)}: Using cached Power {cachedPower.PowerDef.FullDisplayName}[{cachedPower.Pointer:X8}].");
#endif

            return cachedPower;
        }

        /// <summary>
        /// Оператор преобразования к типу <see cref="MyNW.Classes.Power"/>
        /// </summary>
        /// <param name="pwrCache"></param>
        public static explicit operator MyNW.Classes.Power(PowerCache pwrCache)
        {
            return pwrCache.GetPower();
        }

        /// <summary>
        /// Поиск умения, соответствующего <see cref="PowerIdPattern"/>
        /// </summary>
        /// <returns></returns>
        private MyNW.Classes.Power SearchPower()
        {
            foreach (var pwr in EntityManager.LocalPlayer.Character.Powers)
            {
                if (checker(pwr.PowerDef.InternalName))
                {
                    return new MyNW.Classes.Power(pwr.Pointer);
                }
            } 

            return null;
        }
    }
}
