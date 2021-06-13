using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using AcTp0Tools.Reflection;

namespace EntityTools.Tools.Combats.IgnoredFoes
{
    //[HarmonyPatch(typeof(Astral.Quester.Core), nameof(Astral.Quester.Core.Load))]
    public static class IgnoredFoesCore
    {
        /// <summary>
        /// Коллекция списков игнорируемых врагов, ассоциированных с квестер-профилем
        /// </summary>
        private static readonly IgnoredFoesContainer BlContainer = new IgnoredFoesContainer();



#if Traverse_BLAttackersList
        /// <summary>
        /// Механизм доступа к функтору Astral.Logic.NW.Combats.BLAttackersList,
        /// который передает в боевую подсистему список игнорируемых врагов
        /// </summary>
        private static readonly Traverse<Func<List<string>>> BLAttackersList = Traverse.Create(typeof(Astral.Logic.NW.Combats)).Field<Func<List<string>>>("BLAttackersList");
#else
        private static readonly StaticFieldAccessor<Func<List<string>>> BLAttackersList = typeof(Astral.Logic.NW.Combats).GetStaticField<Func<List<string>>>("BLAttackersList");
#endif
        static IgnoredFoesCore()
        {
            //TODO Вместо патча Astral.Quester.Core.Load() использовать подписку на событие AstralAccessors.Quester.Core.OnProfileChanged

            var Quester_Core_Load = typeof(Astral.Quester.Core).GetMethod(nameof(Astral.Quester.Core.Load));
            var Quester_Core_Load_Postfix = typeof(IgnoredFoesCore).GetMethod(nameof(SetExtendedFoeBlackList));

            if (Quester_Core_Load != null && Quester_Core_Load_Postfix != null)
                AcTp0Tools.Patches.AcTp0Patcher.Harmony.Patch(Quester_Core_Load, null, new HarmonyMethod(Quester_Core_Load_Postfix));
        }

        /// <summary>
        /// Переопределение функтора, передающего в боевую подсистему список игнорируемых врагов.
        /// Новый функтор, включает "временно" игнорируемых врагов
        /// Метод, вызываемый после загрузки квестер-профиля.
        /// </summary>
        static void SetExtendedFoeBlackList()
        {
            var profName = Path.GetFullPath(Astral.API.CurrentSettings.LastQuesterProfile);

            if (!BlContainer.Contains(profName)) return;

            // Подменяем BLAttackersList
            var blEntry = BlContainer[profName];
            if (blEntry.Timeout.IsTimedOut)
                BlContainer.Remove(blEntry);
            else BLAttackersList.Value = () => blEntry.Foes;
        }

        /// <summary>
        /// Установка стандартного функтора, передающего в боевую подсистему список игнорируемых врагов из Profile.BlackList.
        /// </summary>
        static void SetDefaultFoeBlackList()
        {
            BLAttackersList.Value = () => Astral.Quester.API.CurrentProfile.BlackList; 
        }

        /// <summary>
        /// Добавление в список игнорируемых врагов, определенных в профиле,
        /// идентификаторов врагов <paramref name="foes"/>, игнорируемых в течение времени <paramref name="time"/>
        /// </summary>
        public static void Add(IEnumerable<string> foes, int time = -1)
        {
            var profName = Path.GetFullPath(Astral.API.CurrentSettings.LastQuesterProfile);
            var profBlackList = Astral.Quester.API.CurrentProfile.BlackList;

            IgnoredFoes blEntry = null;
            if (BlContainer.Contains(profName))
            {
                blEntry = BlContainer[profName];
                if (blEntry.Timeout.IsTimedOut)
                    blEntry.Clear();
                blEntry.AddRange(foes);
                blEntry.Timeout.ChangeTime(time > 0 ? time : int.MaxValue);
            }
            else
            {
                blEntry = new IgnoredFoes(profName, profBlackList, foes, time);
                BlContainer.Add(blEntry);
            }

            if (blEntry.Foes.Count > 0)
                SetExtendedFoeBlackList();
            else SetDefaultFoeBlackList();
        }

        /// <summary>
        /// Удаление из списока игнорируемых врагов, идентификаторов врагов <paramref name="foes"/>
        /// </summary>
        public static void Remove(IEnumerable<string> foes = null)
        {
            var profName = Path.GetFullPath(Astral.API.CurrentSettings.LastQuesterProfile);

            if (BlContainer.Contains(profName))
            {
                IgnoredFoes blEntry = BlContainer[profName];

                if (foes is null || blEntry.Timeout.IsTimedOut)
                {
                    BlContainer.Remove(blEntry);
                    SetDefaultFoeBlackList();
                }
                else
                {
                    blEntry.RemoveRange(foes);
                }
            }
        }

        /// <summary>
        /// Актуальный список игнорируемых врагов
        /// </summary>
        public static List<string> ActualIgnoredFoes
        {
            get
            {
                var profName = Path.GetFullPath(Astral.API.CurrentSettings.LastQuesterProfile);
                if (BlContainer.Contains(profName))
                {
                    var blEntry = BlContainer[profName];
                    return blEntry.Foes;
                }
                return Astral.Quester.API.CurrentProfile.BlackList;
            }
        }
    }
}
