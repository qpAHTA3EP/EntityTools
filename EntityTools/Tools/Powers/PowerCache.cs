using System;
using System.Reflection;
using EntityTools.Core.Interfaces;
using Infrastructure;
using MyNW.Internals;

namespace EntityTools.Tools.Powers
{
    /// <summary>
    /// Класс, кэширующий <see cref="MyNW.Classes.Power"/>
    /// </summary>
    public class PowerCache : IPowerCache
    {
        public PowerCache(string powId, string ownerId = default, Func<bool> doLog = null)
        {
            if (doLog is null)
                this.doLog = () => false;
            else this.doLog = doLog;
            var extendedDebugInfo = this.doLog();

            powerIdPattern = powId;
            if (!string.IsNullOrEmpty(ownerId))
            {
                debugIdStr = string.Concat(ownerId, '.', nameof(PowerCache));
            }
            else
            {
                debugIdStr = nameof(PowerCache);
            }
            if (string.IsNullOrEmpty(powId))
            {
                //initialized = false;
                checker = (p) => false;

                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{debugIdStr}: Initialize {nameof(PowerIdPattern)} to 'Empty'.");
            }
            else
            {
                checker = powId.GetComparer();

                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{debugIdStr}: Initialize {nameof(PowerIdPattern)} to '{powerIdPattern}'.");
                //initialized = true;
            }
        }

        private readonly string debugIdStr;
        private Func<bool> doLog;

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
        public bool IsInitialized => !string.IsNullOrEmpty(powerIdPattern);//initialized;
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
                var extendedDebugInfo = doLog();
                string currentMethodName = extendedDebugInfo
                                         ? $"{debugIdStr}.{MethodBase.GetCurrentMethod()?.Name ?? nameof(GetPower)}"
                                         : string.Empty;
                if (powerIdPattern != value)
                {
                    powerIdPattern = value;
                    if (string.IsNullOrEmpty(powerIdPattern))
                    {


                        if (doLog())
                            ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Set {nameof(PowerIdPattern)} to 'Empty'.");
                        checker = (p) => false;
                        //initialized = false;
                    }
                    else
                    {
                        checker = powerIdPattern.GetComparer();
                        //initialized = true;
                        if (doLog())
                            ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Initialize {nameof(PowerIdPattern)} to '{powerIdPattern}'.");
                    }
                }
            }
        }
        private string powerIdPattern;

        /// <summary>
        /// Кэшированное <see cref="MyNW.Classes.Power"/>
        /// </summary>
        public MyNW.Classes.Power Power => GetPower();

        public void Reset(string powerPattern = default)
        {
            var extendedDebugInfo = doLog();
            string currentMethodName = extendedDebugInfo
                                     ? $"{debugIdStr}.{MethodBase.GetCurrentMethod()?.Name ?? nameof(GetPower)}"
                                     : string.Empty;

            if (extendedDebugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Reset {nameof(PowerIdPattern)} to '{powerPattern}'.");

            PowerIdPattern = powerPattern;
        }

        /// <summary>
        /// Кэшированное <see cref="MyNW.Classes.Power"/>
        /// </summary>
        public MyNW.Classes.Power GetPower()
        {
            var extendedDebugInfo = doLog();
            string currentMethodName = extendedDebugInfo
                                     ? $"{debugIdStr}.{MethodBase.GetCurrentMethod()?.Name ?? nameof(GetPower)}"
                                     : string.Empty;

            if (!IsInitialized)
            {
                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Not initialized.");

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
                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Regen cache.");

                //power = Powers.GetPowerByInternalName(powId);
                cachedPower = SearchPower();
                if (cachedPower != null)
                {
                    if (extendedDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Found Power {cachedPower.PowerDef.FullDisplayName}[{cachedPower.Pointer:X8}].");

                    cachedPowerId = cachedPower.PowerId;
                    cachedAttachedGameProcessId = processId;
                    cachedCharacterContainerId = player.ContainerId;
                }
                else
                {

                    if (extendedDebugInfo) 
                        ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: No Power {powerIdPattern} was found.");

                    cachedPowerId = 0;
                    cachedAttachedGameProcessId = 0;
                    cachedCharacterContainerId = 0;
                }
            }

            else if (extendedDebugInfo) 
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Using cached Power {cachedPower.PowerDef.FullDisplayName}[{cachedPower.Pointer:X8}].");

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
