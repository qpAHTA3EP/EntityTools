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
        private int attachedGameProcessId;
        /// <summary>
        /// Идентификатор персонажа для отслеживания актуальности кэша
        /// </summary>
        private uint characterContainerId;
        /// <summary>
        /// Идентификатор умения для отслеживания актуальности кэша
        /// </summary>
        private uint powerId;
        /// <summary>
        /// Кэшированное умение
        /// </summary>
        private MyNW.Classes.Power power;
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

        /// <summary>
        /// Кэшированное <see cref="MyNW.Classes.Power"/>
        /// </summary>

        public MyNW.Classes.Power GetPower()
        {
            if (!initialized)
                return null;

            var player = EntityManager.LocalPlayer;
            var processId = Astral.API.AttachedGameProcess.Id;

            if (!(attachedGameProcessId == processId
                  && characterContainerId == player.ContainerId
                  && power != null
                  && (power.PowerId == powerId
                      || checker(power.PowerDef.InternalName)
                      || checker(power.EffectivePowerDef().InternalName))))
            {
                //power = Powers.GetPowerByInternalName(powId);
                power = SearchPower();
                if (power != null)
                {
                    powerId = power.PowerId;
                    attachedGameProcessId = processId;
                    characterContainerId = player.ContainerId;
                }
                else
                {
                    powerId = 0;
                    attachedGameProcessId = 0;
                    characterContainerId = 0;
                }
            }
            return power;
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
