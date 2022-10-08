using System;
using System.Collections.Generic;
using System.IO;
using ACTP0Tools;
using ACTP0Tools.Reflection;

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
#if false
            var Quester_Core_Load = typeof(Astral.Quester.Core).GetMethod(nameof(Astral.Quester.Core.Load));
            var Quester_Core_Load_Postfix = typeof(IgnoredFoesCore).GetMethod(nameof(SetExtendedFoeBlackList));

            if (Quester_Core_Load != null && Quester_Core_Load_Postfix != null)
                AcTp0Tools.Patches.ACTP0Patcher.Harmony.Patch(Quester_Core_Load, null, new HarmonyMethod(Quester_Core_Load_Postfix)); 
#else
            AstralAccessors.Quester.Core.OnProfileChanged += OnQuesterProfileChanged;
            Astral.Quester.API.BeforeStartEngine += API_BeforeStartEngine;
#endif
        }

        private static void API_BeforeStartEngine(object sender, Astral.Logic.Classes.FSM.BeforeEngineStart e)
        {
            var profName = Path.GetFullPath(Astral.API.CurrentSettings.LastQuesterProfile);

            if (!BlContainer.Contains(profName)) return;

            var blEntry = BlContainer[profName];

            if (blEntry.Timeout.IsTimedOut)
            {
                BlContainer.Remove(blEntry);
                BLAttackersList.Value = DefaultFoeBlackListGetter;
            }
            else SetExtendedFoeBlackList(blEntry);
        }

        private static void OnQuesterProfileChanged()
        {
            var profName = Path.GetFullPath(Astral.API.CurrentSettings.LastQuesterProfile);

            if (!BlContainer.Contains(profName)) return;

            var blEntry = BlContainer[profName];

            if (blEntry.Timeout.IsTimedOut)
            {
                BlContainer.Remove(blEntry);
                BLAttackersList.Value = DefaultFoeBlackListGetter;
            }
            else SetExtendedFoeBlackList(blEntry);
        }

        /// <summary>
        /// Переопределение функтора, передающего в боевую подсистему список игнорируемых врагов.
        /// Новый функтор, включает "временно" игнорируемых врагов
        /// </summary>
        static void SetExtendedFoeBlackList(IgnoredFoesEntry blEntry)
        {
            if(blEntry is null)
                return;

            List<string> ExtendedFoeListGetter()
            {
                if (blEntry.Timeout.IsTimedOut)
                {
                    BlContainer.Remove(blEntry);
                    BLAttackersList.Value = DefaultFoeBlackListGetter;
                    return DefaultFoeBlackListGetter();
                }

                return blEntry.Foes;
            }

            BLAttackersList.Value = ExtendedFoeListGetter;
        }

        /// <summary>
        /// Функтор, возвращающий стандартный список игнорируемых врагов текущего профиля
        /// </summary>
        private static readonly Func<List<string>> DefaultFoeBlackListGetter = () =>  Astral.Quester.API.CurrentProfile.BlackList;

        /// <summary>
        /// Добавление в список игнорируемых врагов, определенных в профиле,
        /// идентификаторов врагов <paramref name="foes"/>, игнорируемых в течение времени <paramref name="time"/>
        /// </summary>
        public static void Add(IEnumerable<string> foes, int time = -1)
        {
            var profName = Path.GetFullPath(Astral.API.CurrentSettings.LastQuesterProfile);
            IgnoredFoesEntry blEntry;
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
                var profBlackList = Astral.Quester.API.CurrentProfile.BlackList;
                blEntry = new IgnoredFoesEntry(profName, profBlackList, foes, time > 0 ? time : int.MaxValue);
                BlContainer.Add(blEntry);
            }

            if (blEntry.Foes.Count > 0)
                SetExtendedFoeBlackList(blEntry);
            else BLAttackersList.Value = DefaultFoeBlackListGetter;
        }

        /// <summary>
        /// Удаление из списка игнорируемых врагов, идентификаторов врагов <paramref name="foes"/>
        /// </summary>
        public static void Remove(IEnumerable<string> foes = null)
        {
            var profName = Path.GetFullPath(Astral.API.CurrentSettings.LastQuesterProfile);

            if (BlContainer.Contains(profName))
            {
                IgnoredFoesEntry blEntry = BlContainer[profName];

                if (foes is null || blEntry.Timeout.IsTimedOut)
                {
                    BlContainer.Remove(blEntry);
                    BLAttackersList.Value = DefaultFoeBlackListGetter;
                }
                else
                {
                    blEntry.RemoveRange(foes);
                }
            }
        }

#if false
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
#endif
    }
}
