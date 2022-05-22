using EntityTools.Tools;
using MyNW.Internals;
using System;

namespace EntityCore.Tools.Powers
{
    /// <summary>
    /// Класс, кэширующий <see cref="MyNW.Classes.Power"/>
    /// </summary>
    public class PowerCache
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

        public void Reset()
        {
            powerIdPattern = string.Empty;
            checker = (p) => false;
            initialized = false;
        }

        /// <summary>
        /// Кэшированное <see cref="MyNW.Classes.Power"/>
        /// </summary>
        public MyNW.Classes.Power GetPower()
        {
            if (!initialized)
                return null;

            var player = EntityManager.LocalPlayer;
            var processId = Astral.API.AttachedGameProcess.Id;

            if (!(cachedAttachedGameProcessId == processId
                  && cachedCharacterContainerId == player.ContainerId
                  && cachedPower != null
                  && (cachedPower.PowerId == cachedPowerId
                      || checker(cachedPower.PowerDef.InternalName)
                      || checker(cachedPower.EffectivePowerDef().InternalName))))
            {
                //power = Powers.GetPowerByInternalName(powId);
                cachedPower = SearchPower();
                if (cachedPower != null)
                {
                    cachedPowerId = cachedPower.PowerId;
                    cachedAttachedGameProcessId = processId;
                    cachedCharacterContainerId = player.ContainerId;
                }
                else
                {
                    cachedPowerId = 0;
                    cachedAttachedGameProcessId = 0;
                    cachedCharacterContainerId = 0;
                }
            }
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
