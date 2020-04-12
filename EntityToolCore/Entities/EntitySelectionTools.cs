using Astral.Classes.ItemFilter;
using Astral.Quester.Classes;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EntityCore.Extensions;
using EntityTools.Enums;

namespace EntityCore.Entities
{
    public static class EntitySelectionTools
    {
        public delegate bool SpecialCheckDelegate(Entity entity);

#if DEBUG && PROFILING
        private static readonly long interval = 10000;

        private static Stopwatch stopwatch = new Stopwatch();
        private static Stopwatch cntStopwatch = new Stopwatch();
        private static int Count = 0;
        private static int WorseTryNumber = 0;
        private static TimeSpan MinTime = TimeSpan.MaxValue;
        private static TimeSpan MaxTime = TimeSpan.MinValue;

        private static int ContactCount = 0;
        private static int ContactWorseTryNumber = 0;
        private static TimeSpan ContactMinTime = TimeSpan.MaxValue;
        private static TimeSpan ContactMaxTime = TimeSpan.MinValue;

        private static Dictionary<long, long> frequencyDistribution = new Dictionary<long, long>();
        private static Dictionary<long, long> contactFrequencyDistribution = new Dictionary<long, long>();

        public static void ResetWatch()
        {
            ContactCount = 0;
            ContactWorseTryNumber = 0;
            Count = 0;
            WorseTryNumber = 0;
            MinTime = TimeSpan.MaxValue;
            MaxTime = TimeSpan.MinValue;
            ContactMinTime = TimeSpan.MaxValue;
            ContactMaxTime = TimeSpan.MinValue;
            stopwatch.Reset();
            cntStopwatch.Reset();
            frequencyDistribution.Clear();
            contactFrequencyDistribution.Clear();
            EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"UncachedSearch::ResetWatch()");
        }

        public static void LogWatch()
        {
            if (Count > 0)
            {
                double avrgTime = (double)stopwatch.ElapsedMilliseconds / (double)Count;
                double avrgTicks = (double)stopwatch.ElapsedTicks / (double)Count;
                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"EntitySelectionTools::FindClosestEntity():\tCount: {Count}");
                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"EntitySelectionTools::FindClosestEntity():\tWorseTryNumber: {WorseTryNumber}");
                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"EntitySelectionTools::FindClosestEntity():\tTotalTime: {stopwatch.Elapsed}({stopwatch.ElapsedMilliseconds.ToString("N0")} ms)");
                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"EntitySelectionTools::FindClosestEntity():\tMinTime: {MinTime.TotalMilliseconds.ToString("N3")} ms ({MinTime.Ticks.ToString("N0")} ticks)");
                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"EntitySelectionTools::FindClosestEntity():\tMaxTime: {MaxTime.TotalMilliseconds.ToString("N3")} ms ({MaxTime.Ticks.ToString("N0")} ticks)");
                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"EntitySelectionTools::FindClosestEntity():\tAverageTime: {avrgTime.ToString("N3")} ms ({avrgTicks.ToString("N0")} ticks)");
                if (frequencyDistribution.Count > 0)
                {
                    EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"EntitySelectionTools::FrequencyDistribution:");
                    var list = frequencyDistribution.ToList();
                    list.Sort((KeyValuePair<long, long> l, KeyValuePair<long, long> r) => { return (int)l.Key - (int)r.Key; });
                    foreach (var i in list)
                        EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"\t\t{((double)i.Key * (double)interval / 10000d).ToString("N3")} := {i.Value.ToString("N0")}");
                }
                //EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"EntitySelectionTools::FindClosestEntity(): Count: {Count}, TotalTime: {stopwatch.Elapsed}({stopwatch.ElapsedMilliseconds.ToString("N3")} ms), MinTime: {MinTime.TotalMilliseconds.ToString("N3")} ms ({MinTime.Ticks.ToString("N0")} ticks), MaxTime: {MaxTime.TotalMilliseconds.ToString("N3")} ms ({MaxTime.Ticks.ToString("N0")} ticks) , AverageTime: {avrgTime.ToString("N3")} ms ({avrgTicks.ToString("N0")} ticks)");
                //foreach (var i in frequencyDistribution)
                //    EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"FrequencyDistribution[{((double)i.Key * (double)interval / 10000d).ToString("N3")}] := {i.Value.ToString("N0")}");
            }
            else EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"EntitySelectionTools::FindClosestEntity(): Count: 0");
            if (ContactCount > 0)
            {
                double avrgTime = (double)cntStopwatch.ElapsedMilliseconds / (double)ContactCount;
                double avrgTicks = (double)cntStopwatch.ElapsedTicks / (double)ContactCount;
                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"EntitySelectionTools::FindClosestContactEntity():\tCount: {ContactCount}");
                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"EntitySelectionTools::FindClosestContactEntity():\tWorseTryNumber: {ContactWorseTryNumber}");
                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"EntitySelectionTools::FindClosestContactEntity():\tTotalTime: {cntStopwatch.Elapsed}({cntStopwatch.ElapsedMilliseconds.ToString("N0")} ms)");
                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"EntitySelectionTools::FindClosestContactEntity():\tMinTime: {ContactMinTime.TotalMilliseconds.ToString("N3")} ms ({ContactMinTime.Ticks.ToString("N0")} ticks)");
                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"EntitySelectionTools::FindClosestContactEntity():\tMaxTime: {ContactMaxTime.TotalMilliseconds.ToString("N3")} ms ({ContactMaxTime.Ticks.ToString("N0")} ticks)");
                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"EntitySelectionTools::FindClosestContactEntity():\tAverageTime: {avrgTime.ToString("N3")} ms ({avrgTicks.ToString("N0")} ticks)");
                if (contactFrequencyDistribution.Count > 0)
                {
                    EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"EntitySelectionTools::ContactFrequencyDistribution:");
                    var list = contactFrequencyDistribution.ToList();
                    list.Sort((KeyValuePair<long, long> l, KeyValuePair<long, long> r) => { return (int)l.Key - (int)r.Key; });
                    foreach (var i in list)
                        EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"\t\t{((double)i.Key * (double)interval / 10000d).ToString("N3")} := {i.Value.ToString("N0")}");
                }
                //EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"EntitySelectionTools::FindClosestContactEntity(): Count: {ContactCount}, TotalTime: {cntStopwatch.Elapsed}({cntStopwatch.ElapsedMilliseconds.ToString("N3")} ms), MinTime: {ContactMinTime.TotalMilliseconds.ToString("N3")} ms ({ContactMinTime.Ticks.ToString("N0")} ticks), MaxTime: {ContactMaxTime.TotalMilliseconds.ToString("N3")} ms ({ContactMaxTime.Ticks.ToString("N0")} ticks) , AverageTime: {avrgTime.ToString("N3")} ms ({avrgTicks.ToString("N0")} ticks)");
                //foreach (var i in contactFrequencyDistribution)
                //    EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"ContactFrequencyDistribution[{((double)i.Key * (double)interval / 10000d).ToString("N3")}] := {i.Value.ToString("N0")}");
            }
            else EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"EntitySelectionTools::FindClosestContactEntity(): Count: 0");
        }
#endif


        /// <summary>
        /// Поиск ближайшего Entity из entities, соответствующего условиям
        /// </summary>
        /// <param name="entities">Исходная коллекция Entity</param>
        /// <param name="entPattern">Шаблон, которому должен соответствовать идентификатор (имя) Entity, заданные параметном </param>
        /// <param name="strMatchType">Способ сопоставления шаблона: Regex (регулярное выражение) или Simple (простой текст)</param>
        /// <param name="nameType">Идентификатор (имя) Entity, с которым сопостовляется entPattern</param>
        /// <param name="healthCheck">Если True, искать только Entity с количеством здоровья (НР) больше 0 и без флага IsDead</param>
        /// <param name="range">Предельное расстояние поиска, дальше которого Утешен игнорируются</param>
        /// <param name="regionCheck">Если True, искать только те Entity, которые находятся в одном регионе с игроком</param>
        /// <param name="customRegionNames">Список CustomRegion'ов, в которых нужно искать Entity</param>
        /// <param name="specialCheck">Функтор дополнительной проверки Entity, например наличия в черном списке</param>
        /// <param name="interactable">Если True, искать только Entity с которыми можно взаимодействать</param>
        /// <returns>Найденное Entity</returns>
        public static Entity FindClosestEntity(List<Entity> entities, string entPattern, ItemFilterStringType strMatchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated, bool healthCheck = false, float range = 0, bool regionCheck = false, List<string> customRegionNames = null, SpecialCheckDelegate specialCheck = null, bool interactable = false)
        {
#if DEBUG && PROFILING
            Count++;
            TimeSpan StartTime = stopwatch.Elapsed;
            stopwatch.Start();
            try
            {
#endif
                Entity closestEntity = new Entity(IntPtr.Zero);
                if (!string.IsNullOrEmpty(entPattern) && entities != null)
                {
                    switch (nameType)
                    {

                        case EntityNameType.NameUntranslated:
                            switch (strMatchType)
                            {
                                case ItemFilterStringType.Simple:
                                    return FindClosestEntitySimpleNameUntranslated(entities, entPattern, healthCheck, range, regionCheck, customRegionNames, specialCheck, interactable);
                                case ItemFilterStringType.Regex:
                                    return FindClosestEntityRegexNameUntranslated(entities, entPattern, healthCheck, range, regionCheck, customRegionNames, specialCheck, interactable);
                            }
                            break;
                        case EntityNameType.InternalName:
                            switch (strMatchType)
                            {
                                case ItemFilterStringType.Simple:
                                    return FindClosestEntitySimpleNameInternal(entities, entPattern, healthCheck, range, regionCheck, customRegionNames, specialCheck, interactable);
                                case ItemFilterStringType.Regex:
                                    return FindClosestEntityRegexNameInternal(entities, entPattern, healthCheck, range, regionCheck, customRegionNames, specialCheck, interactable);
                            }
                            break;
                    }
                }
                return closestEntity;
#if DEBUG && PROFILING
            }
            finally
            {
                stopwatch.Stop();
                TimeSpan time = stopwatch.Elapsed.Subtract(StartTime);
                if (time > MaxTime)
                {
                    MaxTime = time;
                    WorseTryNumber = Count;
                }
                else if (time < MinTime)
                    MinTime = time;

                long i = Math.DivRem(time.Ticks, interval, out long rem);
                if (frequencyDistribution.ContainsKey(i))
                    frequencyDistribution[i] += 1;
                else frequencyDistribution.Add(i, 1);
            }
#endif
        }
        /// <summary>
        /// Поиск ближайшего Entity из entities, соответствующего условиям
        /// </summary>
        /// <param name="entPattern">Шаблон, которому должен соответствовать идентификатор (имя) Entity, заданные параметном </param>
        /// <param name="strMatchType">Способ сопоставления шаблона: Regex (регулярное выражение) или Simple (простой текст)</param>
        /// <param name="nameType">Идентификатор (имя) Entity, с которым сопостовляется entPattern</param>
        /// <param name="healthCheck">Если True, искать только Entity с количеством здоровья (НР) больше 0 и без флага IsDead</param>
        /// <param name="range">Предельное расстояние поиска, дальше которого Утешен игнорируются</param>
        /// <param name="regionCheck">Если True, искать только те Entity, которые находятся в одном регионе с игроком</param>
        /// <param name="customRegionNames">Список CustomRegion'ов, в которых нужно искать Entity</param>
        /// <param name="specialCheck">Функтор дополнительной проверки Entity, например наличия в черном списке</param>
        /// <returns>Найденное Entity</returns>
        public static Entity FindClosestContactEntity(string entPattern, ItemFilterStringType strMatchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated, bool healthCheck = false, float range = 0, bool regionCheck = false, List<string> customRegionNames = null, SpecialCheckDelegate specialCheck = null)
        {

#if DEBUG && PROFILING
            ContactCount++;
            TimeSpan StartTime = cntStopwatch.Elapsed;
            cntStopwatch.Start();
            try
            { 
#endif
                List<Entity> entities = new List<Entity>();

                foreach (ContactInfo contact in EntityManager.LocalPlayer.Player.InteractInfo.NearbyContacts)
                {
                    if (contact.Entity.IsValid)
                    {
                        if (specialCheck != null && !specialCheck(contact.Entity))
                            continue;

                        entities.Add(contact.Entity);
                    }
                }
                foreach (ContactInfo contact in EntityManager.LocalPlayer.Player.InteractInfo.NearbyInteractCritterEnts)
                {
                    if (contact.Entity.IsValid)
                    {
                        if (specialCheck != null && !specialCheck(contact.Entity))
                            continue;

                        entities.Add(contact.Entity);
                    }
                }

                return FindClosestEntity(entities, entPattern, strMatchType, nameType, healthCheck, range, regionCheck, customRegionNames, specialCheck, true);

#if DEBUG && PROFILING
            }
            finally
            {
                cntStopwatch.Stop();
                TimeSpan time = cntStopwatch.Elapsed.Subtract(StartTime);
                if (time > ContactMaxTime)
                {
                    ContactMaxTime = time;
                    ContactWorseTryNumber = ContactCount;
                }
                else if (time < ContactMinTime)
                    ContactMinTime = time;

                
                long i = Math.DivRem((long)time.Ticks, interval, out long rem);
                if (contactFrequencyDistribution.ContainsKey(i))
                    contactFrequencyDistribution[i] += 1;
                else contactFrequencyDistribution.Add(i, 1);
            }
#endif        
        }
        /// <summary>
        /// Поиск ближайшего Entity из entities, 
        /// у которого InternalName соответствует шаблону регулярного выражения
        /// и соответствующего остальным условиям
        /// </summary>
        /// <param name="entities">Исходная коллекция Entity</param>
        /// <param name="entPattern">Шаблон, соответсвие которому ищется</param>
        /// <param name="healthCheck">>Флаг проверки очков HP у Entity</param>
        /// <param name="range">Предельное расстояние поиска</param>
        /// <param name="regionCheck">Флаг проверки соответствия региона Entity и региона игрока</param>
        /// <param name="specialCheck">Функтор дополнительной проверки Entity, например наличия в черном списке</param>
        /// <returns>Найденное Entity</returns>
        private static Entity FindClosestEntityRegexNameInternal(List<Entity> entities, string entPattern, bool healthCheck = false, float range = 0, bool regionCheck = false, List<string> customRegionNames = null,SpecialCheckDelegate specialCheck = null, bool interactable = false)
        {
            Entity closestEntity = new Entity(IntPtr.Zero);

            List<CustomRegion> customRegions = (customRegionNames != null && customRegionNames.Count > 0) ? Astral.Quester.API.CurrentProfile.CustomRegions.FindAll((CustomRegion cr) =>
                                            customRegionNames.Exists((string regName) => regName == cr.Name)) : null;


            if (!string.IsNullOrEmpty(entPattern) && entities != null)
            {
                foreach (Entity entity in entities)
                {
                    if ((!regionCheck || entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                        && (!healthCheck || !entity.IsDead)
                        && (!interactable || entity.Critter.IsInteractable)
                        && (customRegions == null || customRegions.Count == 0 || customRegions.FindIndex(cr => entity.Within(cr)) > 0)
                        && (range == 0 || entity.Location.Distance3DFromPlayer < range)
                        && Regex.IsMatch(entity.InternalName, entPattern) 
                        && (specialCheck == null || specialCheck(entity)))
                    {
                        if (closestEntity.IsValid)
                        {
                            if (entity.Location.Distance3DFromPlayer < closestEntity.Location.Distance3DFromPlayer)
                                closestEntity = entity;
                        }
                        else closestEntity = entity;
                    }
                }
            }
            return closestEntity;
        }

        /// <summary>
        /// Поиск ближайшего Entity из entities, 
        /// у которого NameUntranslated соответствует шаблону регулярного выражения
        /// и соответствующего остальным условиям
        /// </summary>
        /// <param name="entities">Исходная коллекция Entity</param>
        /// <param name="entPattern">Шаблон, соответсвие которому ищется</param>
        /// <param name="healthCheck">>Флаг проверки очков HP у Entity</param>
        /// <param name="range">Предельное расстояние поиска</param>
        /// <param name="regionCheck">Флаг проверки соответствия региона Entity и региона игрока</param>
        /// <param name="specialCheck">Функтор дополнительной проверки Entity, например наличия в черном списке</param>
        /// <returns>Найденное Entity</returns>
        private static Entity FindClosestEntityRegexNameUntranslated(List<Entity> entities, string entPattern, bool healthCheck = false, float range = 0, bool regionCheck = false, List<string> customRegionNames = null,SpecialCheckDelegate specialCheck = null, bool interactable = false)
        {
            Entity closestEntity = new Entity(IntPtr.Zero);

            List<CustomRegion> customRegions = (customRegionNames != null && customRegionNames.Count > 0) ? Astral.Quester.API.CurrentProfile.CustomRegions.FindAll((CustomRegion cr) =>
                                            customRegionNames.Exists((string regName) => regName == cr.Name)) : null;


            if (!string.IsNullOrEmpty(entPattern) && entities != null)
            {
                foreach (Entity entity in entities)
                {
                    if ((!regionCheck || entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                        && (!healthCheck || !entity.IsDead)
                        && (!interactable || entity.Critter.IsInteractable)
                        && (customRegions == null || customRegions.Count == 0 || customRegions.FindIndex(cr => entity.Within(cr)) > 0)
                        && (range == 0 || entity.Location.Distance3DFromPlayer < range)
                        && Regex.IsMatch(entity.NameUntranslated, entPattern)
                        && (specialCheck == null || specialCheck(entity)))
                    {
                        if (closestEntity.IsValid)
                        {
                            if (entity.Location.Distance3DFromPlayer < closestEntity.Location.Distance3DFromPlayer)
                                closestEntity = entity;
                        }
                        else closestEntity = entity;
                    }
                }
            }
            return closestEntity;
        }

        /// <summary>
        /// Поиск ближайшего Entity из entities, соответствующего условиям. Поиск производится по полю InternalName
        /// </summary>
        /// <param name="entities">Исходная коллекция Entity</param>
        /// <param name="entPattern">Шаблон, соответсвие которому ищется</param>
        /// <param name="healthCheck">Флаг проверки очков HP у Entity</param>
        /// <param name="range">предельное расстояние поиска</param>
        /// <param name="regionCheck">Флаг проверки соответствия региона Entity и региона игрока</param>
        /// <param name="customRegionNames">Список CustomRegion'ов в которых нужно искать Entity</param>
        /// <param name="specialCheck">Функтор дополнительной проверки Entity, например наличия в черном списке</param>
        /// <returns>Найденное Entity</returns>
        private static Entity FindClosestEntitySimpleNameInternal(List<Entity> entities, string entPattern, bool healthCheck = false, float range = 0, bool regionCheck = false, List<string> customRegionNames = null,SpecialCheckDelegate specialCheck = null, bool interactable = false)
        {
            Entity closestEntity = new Entity(IntPtr.Zero);
            if (!string.IsNullOrEmpty(entPattern) && entities != null)
            {
                List<CustomRegion> customRegions = (customRegionNames != null && customRegionNames.Count > 0) ? Astral.Quester.API.CurrentProfile.CustomRegions.FindAll((CustomRegion cr) =>
                                                customRegionNames.Exists((string regName) => regName == cr.Name)) : null;

                if (entPattern[0] != '*')
                {
                    // Шаблона должен встречаться в начале EntityID
                    entPattern = entPattern.TrimEnd('*');
                    foreach (Entity entity in entities)
                    {
                        if ((range == 0 || entity.Location.Distance3DFromPlayer < range)
                            && (!healthCheck || !entity.IsDead)
                            && (!interactable || entity.Critter.IsInteractable)
                            && (customRegions == null || customRegions.Count == 0 || customRegions.FindIndex(cr => entity.Within(cr)) >= 0)
                            && entity.InternalName.StartsWith(entPattern)
                            && (!regionCheck || entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                            && (specialCheck == null || specialCheck(entity)))
                        {
                            if (closestEntity.IsValid)
                            {
                                if (entity.Location.Distance3DFromPlayer < closestEntity.Location.Distance3DFromPlayer)
                                    closestEntity = entity;
                            }
                            else closestEntity = entity;
                        }
                    }
                }
                else
                {
                    // Поиск любого вхождения шаблона
                    entPattern = entPattern.Trim('*');
                    foreach (Entity entity in entities)
                    {
                        if ((range == 0 || entity.Location.Distance3DFromPlayer < range)
                            && (!healthCheck || !entity.IsDead)
                            && (!interactable || entity.Critter.IsInteractable)
                            && (customRegions == null || customRegions.Count == 0 || customRegions.FindIndex(cr => entity.Within(cr)) >= 0)
                            && entity.InternalName.Contains(entPattern)
                            && (!regionCheck || entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                            && (specialCheck == null || specialCheck(entity)))
                        {
                            if (closestEntity.IsValid)
                            {
                                if (entity.Location.Distance3DFromPlayer < closestEntity.Location.Distance3DFromPlayer)
                                    closestEntity = entity;
                            }
                            else closestEntity = entity;
                        }
                    }
                }
            }
            return closestEntity;
        }

        /// <summary>
        /// Поиск ближайшего Entity из entities, соответствующего условиям. Поиск производится по полю NameUntranslated
        /// </summary>
        /// <param name="entities">Исходная коллекция Entity</param>
        /// <param name="entPattern">Шаблон, соответсвие которому ищется</param>
        /// <param name="strMatchType">Тип шаблона Regex или Simple (простой текст)</param>
        /// <param name="healthCheck">Флаг проверки очков HP у Entity</param>
        /// <param name="range">предельное расстояние поиска</param>
        /// <param name="regionCheck">Флаг проверки соответствия региона Entity и региона игрока</param>
        /// <param name="customRegionNames">Список CustomRegion'ов в которых нужно искать Entity</param>
        /// <param name="specialCheck">Функтор дополнительной проверки Entity, например наличия в черном списке</param>
        /// <returns>Найденное Entity</returns>
        private static Entity FindClosestEntitySimpleNameUntranslated(List<Entity> entities, string entPattern, bool healthCheck = false, float range = 0, bool regionCheck = false, List<string> customRegionNames = null,SpecialCheckDelegate specialCheck = null, bool interactable = false)
        {
            Entity closestEntity = new Entity(IntPtr.Zero);
            if (!string.IsNullOrEmpty(entPattern) && entities != null)
            {
                List<CustomRegion> customRegions = (customRegionNames != null && customRegionNames.Count > 0) ? Astral.Quester.API.CurrentProfile.CustomRegions.FindAll((CustomRegion cr) =>
                                                customRegionNames.Exists((string regName) => regName == cr.Name)) : null;

                if (entPattern[0] != '*')
                {
                    // Шаблона должен встречаться в начале EntityID
                    entPattern = entPattern.TrimEnd('*');
                    foreach (Entity entity in entities)
                    {
                        if ((range == 0 || entity.Location.Distance3DFromPlayer < range)
                            && (!healthCheck || !entity.IsDead)
                            && (!interactable || entity.Critter.IsInteractable)
                            && (customRegions == null || customRegions.Count == 0 || customRegions.FindIndex(cr => entity.Within(cr)) >= 0)
                            && entity.NameUntranslated.StartsWith(entPattern)
                            && (!regionCheck || entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                            && (specialCheck == null || specialCheck(entity)))
                        {
                            if (closestEntity.IsValid)
                            {
                                if (entity.Location.Distance3DFromPlayer < closestEntity.Location.Distance3DFromPlayer)
                                    closestEntity = entity;
                            }
                            else closestEntity = entity;
                        }
                    }
                }
                else
                {
                    // Поиск любого вхождения шаблона
                    entPattern = entPattern.Trim('*');
                    foreach (Entity entity in entities)
                    {
                        if ((range == 0 || entity.Location.Distance3DFromPlayer < range)
                            && (!healthCheck || !entity.IsDead)
                            && (!interactable || entity.Critter.IsInteractable)
                            && (customRegions == null || customRegions.Count == 0 || customRegions.FindIndex(cr => entity.Within(cr)) >= 0)
                            && entity.NameUntranslated.Contains(entPattern)
                            && (!regionCheck || entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                            && (specialCheck == null || specialCheck(entity)))
                        {
                            if (closestEntity.IsValid)
                            {
                                if (entity.Location.Distance3DFromPlayer < closestEntity.Location.Distance3DFromPlayer)
                                    closestEntity = entity;
                            }
                            else closestEntity = entity;
                        }
                    }
                }
            }
            return closestEntity;
        }

        /// <summary>
        /// Поиск всех Entity из entities, удовлетворяющих условиям
        /// </summary>
        /// <param name="entities">Исходная коллекция Entity</param>
        /// <param name="entPattern">Шаблон, которому должен соответствовать идентификатор (имя) Entity, заданные параметном </param>
        /// <param name="strMatchType">Способ сопоставления шаблона: Regex (регулярное выражение) или Simple (простой текст)</param>
        /// <param name="nameType">Идентификатор (имя) Entity, с которым сопостовляется entPattern</param>
        /// <param name="healthCheck">Если True, искать только Entity с количеством здоровья (НР) больше 0 и без флага IsDead</param>
        /// <param name="range">Предельное расстояние поиска, дальше которого Утешен игнорируются</param>
        /// <param name="regionCheck">Если True, искать только те Entity, которые находятся в одном регионе с игроком</param>
        /// <param name="customRegionNames">Список CustomRegion'ов, в которых нужно искать Entity</param>
        /// <param name="specialCheck">Функтор дополнительной проверки Entity, например наличия в черном списке</param>
        /// <param name="interactable">Если True, искать только Entity с которыми можно взаимодействать</param>
        /// <returns>Список найденных Entity</returns>
        public static List<Entity> FindAllEntities(List<Entity> entities, string entPattern, ItemFilterStringType strMatchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated, 
            bool healthCheck = false, bool regionCheck = false, List<string> customRegionNames = null,SpecialCheckDelegate specialCheck = null)
        {
            List<Entity> resultList = null;
            if (!string.IsNullOrEmpty(entPattern) && entities != null)
            {
                List<CustomRegion> customRegions = (customRegionNames != null && customRegionNames.Count > 0) ? Astral.Quester.API.CurrentProfile.CustomRegions.FindAll((CustomRegion cr) =>
                                                customRegionNames.Exists((string regName) => regName == cr.Name)) : null;

                switch (strMatchType)
                {
                    case ItemFilterStringType.Simple:
                        {

                            switch (nameType)
                            {
                                case EntityNameType.InternalName:
                                    return FindAllEntitiesSimpleNameInternal(entities, entPattern, healthCheck, regionCheck, customRegionNames, specialCheck);
                                case EntityNameType.NameUntranslated:
                                    return FindAllEntitiesSimpleNameUntranslated(entities, entPattern, healthCheck, regionCheck, customRegionNames, specialCheck);
                            }
                            break;
                        }
                    case ItemFilterStringType.Regex:
                        return FindAllEntitiesRegex(entities, entPattern, nameType, healthCheck, regionCheck, customRegionNames, specialCheck);
                }
            }
            return resultList;
        }

        /// <summary>
        /// Поиск всех Entity из entities, удовлетворяющих условиям
        /// </summary>
        /// <param name="entities">Исходная коллекция Entity</param>
        /// <param name="entPattern">Шаблон, которому должен соответствовать идентификатор (имя) Entity, заданные параметном </param>
        /// <param name="strMatchType">Способ сопоставления шаблона: Regex (регулярное выражение) или Simple (простой текст)</param>
        /// <param name="nameType">Идентификатор (имя) Entity, с которым сопостовляется entPattern</param>
        /// <param name="healthCheck">Если True, искать только Entity с количеством здоровья (НР) больше 0 и без флага IsDead</param>
        /// <param name="regionCheck">Если True, искать только те Entity, которые находятся в одном регионе с игроком</param>
        /// <param name="customRegionNames">Список CustomRegion'ов, в которых нужно искать Entity</param>
        /// <param name="specialCheck">Функтор дополнительной проверки Entity, например наличия в черном списке</param>
        /// <returns>Список всех найденных Entity</returns>
        public static List<Entity> FindAllContactEntities(string entPattern, ItemFilterStringType strMatchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated, 
            bool healthCheck = false, bool regionCheck = false, List<string> customRegionNames = null,SpecialCheckDelegate specialCheck = null)
        {
            List<Entity> entities = new List<Entity>();

            foreach(ContactInfo contact in EntityManager.LocalPlayer.Player.InteractInfo.NearbyContacts)
            {
                if(contact.Entity.IsValid)
                    entities.Add(contact.Entity);
            }
            foreach (ContactInfo contact in EntityManager.LocalPlayer.Player.InteractInfo.NearbyInteractCritterEnts)
            {
                if (contact.Entity.IsValid)
                    entities.Add(contact.Entity);
            }

            return FindAllEntities(entities, entPattern, strMatchType, nameType, healthCheck, regionCheck, customRegionNames, specialCheck);
        }

        /// <summary>
        /// Поиск всех Entity из entities, удовлетворяющих условиям
        /// у которого NameUntranslated соответствует шаблону типа Simple (Простой текст)
        /// </summary>
        /// <param name="entities">Исходная коллекция Entity</param>
        /// <param name="entPattern">Шаблон, соответсвие которому ищется</param>
        /// <param name="healthCheck">Флаг проверки очков HP у Entity</param>
        /// <param name="range">предельное расстояние поиска</param>
        /// <param name="customRegionNames">Список CustomRegion'ов в которых нужно искать Entity</param>
        /// <param name="specialCheck">Функтор дополнительной проверки Entity, например наличия в черном списке</param>
        /// <returns>Список найденных Entity</returns>
        private static List<Entity> FindAllEntitiesSimpleNameUntranslated(List<Entity> entities, string entPattern, bool healthCheck = false, bool regionCheck = false, List<string> customRegionNames = null,SpecialCheckDelegate specialCheck = null)
        {
            List<Entity> resultList = null;
            if (!string.IsNullOrEmpty(entPattern) && entities != null)
            {
                List<CustomRegion> customRegions = (customRegionNames != null && customRegionNames.Count > 0) ? Astral.Quester.API.CurrentProfile.CustomRegions.FindAll((CustomRegion cr) =>
                                                customRegionNames.Exists((string regName) => regName == cr.Name)) : null;

                if (entPattern[0] != '*')
                {
                    // Поиск шаблона в началае 
                    entPattern = entPattern.Trim('*');
                    resultList = entities.FindAll((Entity e) => (e.NameUntranslated.StartsWith(entPattern)
                                                                    && (!regionCheck || e.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                                    && (!healthCheck || !e.IsDead)
                                                                    && (customRegions == null || customRegions.Count == 0 || customRegions.FindIndex(cr => e.Within(cr)) >= 0)
                                                                    && (specialCheck == null || specialCheck(e))));
                }
                else
                {
                    // Поиск любого вхождения шаблона
                    entPattern = entPattern.Trim('*');
                    resultList = entities.FindAll((Entity e) => (e.NameUntranslated.Contains(entPattern)
                                                                    && (!regionCheck || e.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                                    && (!healthCheck || !e.IsDead)
                                                                    && (customRegions == null || customRegions.Count == 0 || customRegions.FindIndex(cr => e.Within(cr)) >= 0)
                                                                    && (specialCheck == null || specialCheck(e))));
                }
            }
            return resultList;
        }

        /// <summary>
        /// Поиск всех Entity из entities, удовлетворяющих условиям
        /// у которого InternalName соответствует шаблону <see cref="entPattern"/> типа Simple (Простой текст) 
        /// </summary>
        /// <param name="entities">Исходная коллекция Entity</param>
        /// <param name="entPattern">Шаблон, соответсвие которому ищется</param>
        /// <param name="healthCheck">Флаг проверки очков HP у Entity</param>
        /// <param name="range">предельное расстояние поиска</param>
        /// <param name="customRegionNames">Список CustomRegion'ов в которых нужно искать Entity</param>
        /// <param name="specialCheck">Функтор дополнительной проверки Entity, например наличия в черном списке</param>
        /// <returns>Список найденных Entity</returns>
        private static List<Entity> FindAllEntitiesSimpleNameInternal(List<Entity> entities, string entPattern, bool healthCheck = false, bool regionCheck = false, List<string> customRegionNames = null,SpecialCheckDelegate specialCheck = null)
        {
            List<Entity> resultList = null;
            if (!string.IsNullOrEmpty(entPattern) && entities != null)
            {
                List<CustomRegion> customRegions = (customRegionNames != null && customRegionNames.Count > 0) ? Astral.Quester.API.CurrentProfile.CustomRegions.FindAll((CustomRegion cr) =>
                                                customRegionNames.Exists((string regName) => regName == cr.Name)) : null;

                if (entPattern[0] != '*')
                {
                    // Поиск шаблона в началае 
                    entPattern = entPattern.Trim('*');
                    resultList = entities.FindAll((Entity e) => (e.InternalName.StartsWith(entPattern)
                                                                    && (!regionCheck || e.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                                    && (!healthCheck || !e.IsDead)
                                                                    && (customRegions == null || customRegions.Count == 0 || customRegions.FindIndex(cr => e.Within(cr)) >= 0)
                                                                    && (specialCheck == null || specialCheck(e))));
                }
                else
                {
                    // Поиск любого вхождения шаблона
                    entPattern = entPattern.Trim('*');
                    resultList = entities.FindAll((Entity e) => (e.InternalName.Contains(entPattern)
                                                                    && (!regionCheck || e.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                                    && (!healthCheck || !e.IsDead)
                                                                    && (customRegions == null || customRegions.Count == 0 || customRegions.FindIndex(cr => e.Within(cr)) >= 0)
                                                                    && (specialCheck == null || specialCheck(e))));
                }

            }
            return resultList;
        }

        /// <summary>
        /// Поиск всех Entity из entities, удовлетворяющих условиям
        /// у которого EntityID соответствует регулярному выражению <see cref="entPattern"/>>
        /// </summary>
        /// <param name="entities">Исходная коллекция Entity</param>
        /// <param name="entPattern">Шаблон, соответсвие которому ищется</param>
        /// <param name="nameType">Тип шаблона Regex или Simple (простой текст)</param>
        /// <param name="healthCheck">Флаг проверки очков HP у Entity</param>
        /// <param name="range">предельное расстояние поиска</param>
        /// <param name="customRegionNames">Список CustomRegion'ов в которых нужно искать Entity</param>
        /// <param name="specialCheck">Функтор дополнительной проверки Entity, например наличия в черном списке</param>
        /// <returns>Список найденных Entity</returns>
        private static List<Entity> FindAllEntitiesRegex(List<Entity> entities, string entPattern, EntityNameType nameType = EntityNameType.NameUntranslated, bool healthCheck = false, bool regionCheck = false, List<string> customRegionNames = null,SpecialCheckDelegate specialCheck = null)
        {
            List<Entity> resultList = null;
            if (!string.IsNullOrEmpty(entPattern) && entities != null)
            {
                List<CustomRegion> customRegions = (customRegionNames != null && customRegionNames.Count > 0) ? Astral.Quester.API.CurrentProfile.CustomRegions.FindAll((CustomRegion cr) =>
                                                customRegionNames.Exists((string regName) => regName == cr.Name)) : null;

                switch(nameType)
                {
                    case EntityNameType.InternalName: resultList = entities.FindAll((Entity e) => Regex.IsMatch(e.InternalName, entPattern)
                                                                                && (!regionCheck || e.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                                                && (!healthCheck || !e.IsDead)
                                                                                && (customRegions == null || customRegions.Count == 0 || customRegions.FindIndex(cr => e.Within(cr)) >= 0)
                                                                                && (specialCheck == null || specialCheck(e)));
                        break;
                    case EntityNameType.NameUntranslated: resultList = entities.FindAll((Entity e) => Regex.IsMatch(e.NameUntranslated, entPattern)
                                                                                && (!regionCheck || e.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                                                && (!healthCheck || !e.IsDead)
                                                                                && (customRegions == null || customRegions.Count == 0 || customRegions.FindIndex(cr => e.Within(cr)) >= 0)
                                                                                && (specialCheck == null || specialCheck(e)));
                        break;
                }
            }
            return resultList;
        }
    }
}
