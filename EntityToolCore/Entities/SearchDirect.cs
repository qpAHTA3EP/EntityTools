#define PROFILING
using Astral.Classes.ItemFilter;
using EntityTools.Enums;
using EntityTools.Logger;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace EntityCore.Entities
{
    public static class SearchDirect
    {
#if DEBUG && PROFILING
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

        public static void ResetWatch()
        {
            ContactCount = 0;
            Count = 0;
            WorseTryNumber = 0;
            ContactWorseTryNumber = 0;
            MinTime = TimeSpan.MaxValue;
            MaxTime = TimeSpan.MinValue;
            ContactMinTime = TimeSpan.MaxValue;
            ContactMaxTime = TimeSpan.MinValue;
            stopwatch.Reset();
            cntStopwatch.Reset();
            EntityToolsLogger.WriteLine(LogType.Debug, $"UncachedSearch::ResetWatch()");
        }

        public static void LogWatch()
        {
            if (Count > 0)
            {
                double avrgTime = (double)stopwatch.ElapsedMilliseconds / (double)Count;
                double avrgTicks = (double)stopwatch.ElapsedTicks / (double)Count;
                //EntityToolsLogger.WriteLine(LogType.Debug, $"SearchDirect::GetEntities(): Count: {Count}, TotalTime: {stopwatch.Elapsed}({stopwatch.ElapsedMilliseconds.ToString("N3")} ms), MinTime: {MinTime.TotalMilliseconds.ToString("N3")} ms ({MinTime.Ticks.ToString("N0")} ticks), MaxTime: {MaxTime.TotalMilliseconds.ToString("N3")} ms ({MaxTime.Ticks.ToString("N0")} ticks) , AverageTime: {avrgTime.ToString("N3")} ms ({avrgTicks.ToString("N0")} ticks)");
                EntityToolsLogger.WriteLine(LogType.Debug, $"SearchDirect::GetEntities():\tCount: {Count}, TotalTime: {stopwatch.Elapsed}({stopwatch.ElapsedMilliseconds.ToString("N0")} ms)");
                EntityToolsLogger.WriteLine(LogType.Debug, $"SearchDirect::GetEntities():\tMinTime: {MinTime.TotalMilliseconds.ToString("N3")} ms ({MinTime.Ticks.ToString("N0")} ticks)");
                EntityToolsLogger.WriteLine(LogType.Debug, $"SearchDirect::GetEntities():\tMaxTime: {MaxTime.TotalMilliseconds.ToString("N3")} ms ({MaxTime.Ticks.ToString("N0")} ticks)");
                EntityToolsLogger.WriteLine(LogType.Debug, $"SearchDirect::GetEntities():\tAverageTime: {avrgTime.ToString("N3")} ms ({avrgTicks.ToString("N0")} ticks)");
            }
            else EntityToolsLogger.WriteLine(LogType.Debug, $"SearchDirect::GetEntities(): Count: 0");
            if (ContactCount > 0)
            {
                double avrgTime = (double)cntStopwatch.ElapsedMilliseconds / (double)ContactCount;
                double avrgTicks = (double)cntStopwatch.ElapsedTicks / (double)ContactCount;
                //EntityToolsLogger.WriteLine(LogType.Debug, $"SearchDirect::GetContactEntities(): Count: {ContactCount}, TotalTime: {cntStopwatch.Elapsed}({cntStopwatch.ElapsedMilliseconds.ToString("N3")} ms), MinTime: {ContactMinTime.TotalMilliseconds.ToString("N3")} ms ({ContactMinTime.Ticks.ToString("N0")} ticks), MaxTime: {ContactMaxTime.TotalMilliseconds.ToString("N3")} ms ({ContactMaxTime.Ticks.ToString("N0")} ticks) , AverageTime: {avrgTime.ToString("N3")} ms ({avrgTicks.ToString("N0")} ticks)");
                EntityToolsLogger.WriteLine(LogType.Debug, $"SearchDirect::GetContactEntities():\tCount: {ContactCount}, TotalTime: {cntStopwatch.Elapsed}({cntStopwatch.ElapsedMilliseconds.ToString("N0")} ms)");
                EntityToolsLogger.WriteLine(LogType.Debug, $"SearchDirect::GetContactEntities():\tMinTime: {ContactMinTime.TotalMilliseconds.ToString("N3")} ms ({ContactMinTime.Ticks.ToString("N0")} ticks)");
                EntityToolsLogger.WriteLine(LogType.Debug, $"SearchDirect::GetContactEntities():\tMaxTime: {ContactMaxTime.TotalMilliseconds.ToString("N3")} ms ({ContactMaxTime.Ticks.ToString("N0")} ticks)");
                EntityToolsLogger.WriteLine(LogType.Debug, $"SearchDirect::GetContactEntities():\tAverageTime: {avrgTime.ToString("N3")} ms ({avrgTicks.ToString("N0")} ticks)");
            }
            else EntityToolsLogger.WriteLine(LogType.Debug, $"SearchDirect::GetContactEntities(): Count: 0");
        }
#endif

        /// <summary>
        /// Формирование списка Entities, соответствующих шаблону
        /// </summary>
        /// <param name="entPattern"></param>
        /// <param name="strMatchType"></param>
        /// <param name="nameType"></param>
        /// <param name="action">Функтор действия, которое нужно выполнить над Entity, удовлетворяющем условиям</param>
        /// <returns></returns>
        public static LinkedList<Entity> GetEntities(string entPattern, ItemFilterStringType strMatchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated, Action<Entity> action = null)
        {
#if DEBUG && PROFILING
            Count++;
            TimeSpan StartTime = stopwatch.Elapsed;
            stopwatch.Start();
            try
            {
#endif
                LinkedList<Entity> entities = new LinkedList<Entity>();
                Predicate<Entity> comparer = EntityToPatternComparer.Get(entPattern, strMatchType, nameType);

                if (action != null)
                {
                    foreach (Entity e in EntityManager.GetEntities())
                    {
                        if (comparer(e))
                        {
                            action(e);
                            entities.AddLast(e);
                        }
                    }
                    //return EntityManager.GetEntities()?.FindAll((Entity e) =>
                    //        {
                    //            if (comparer.Check(e))
                    //            {
                    //                action(e);
                    //                return true;
                    //            }
                    //            else return false;
                    //        });
                }
                else
                {
                    foreach (Entity e in EntityManager.GetEntities())
                        if (comparer(e))
                            entities.AddLast(e);

                    //return EntityManager.GetEntities()?.FindAll(comparer.Check);
                }
                return entities;
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

            }
#endif
        }

        /// <summary>
        /// Формирование списка Entities, соответствующих ключу
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action">Функтор действия, которое нужно выполнить над Entity, удовлетворяющем условиям</param>
        /// <returns></returns>
        public static LinkedList<Entity> GetEntities(CacheRecordKey key, Action<Entity> action = null)
        {
            if (key == null)
                return null;
#if DEBUG && PROFILING
            Count++;
            TimeSpan StartTime = stopwatch.Elapsed;
            stopwatch.Start();
            try
            {
#endif
            LinkedList<Entity> entities = new LinkedList<Entity>();
            if (action != null)
            {
                foreach (Entity e in EntityManager.GetEntities())
                {
                    if (key.Comparer(e))
                    {
                        action(e);
                        entities.AddLast(e);
                    }
                }
                //return EntityManager.GetEntities()?.FindAll((Entity e) =>
                //      {
                //          if (key.Comparer.Check(e))
                //          {
                //              action(e);
                //              return true;
                //          }
                //          else return false;
                //      });
            }
            else
            {
                //return EntityManager.GetEntities()?.FindAll(key.Comparer.Check);
                foreach (Entity e in EntityManager.GetEntities())
                    if (key.Comparer(e))
                        entities.AddLast(e);
            }
            return entities;
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

            }
#endif
        }

        /// <summary>
        /// Формирование списка Entities, способных к взаимодействию и соответствующих шаблону
        /// </summary>
        /// <param name="entPattern">Шаблон, которому должен соответствовать идентификатор (имя) Entity, заданные параметном </param>
        /// <param name="strMatchType">Способ сопоставления шаблона: Regex (регулярное выражение) или Simple (простой текст)</param>
        /// <param name="nameType">Идентификатор (имя) Entity, с которым сопостовляется entPattern</param>
        /// <param name="action">Функтор действия, которое нужно выполнить над Entity, удовлетворяющем условиям</param>
        /// <returns></returns>
        public static LinkedList<Entity> GetContactEntities(string entPattern, ItemFilterStringType strMatchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated, Action<Entity> action = null)
        {
#if DEBUG && PROFILING
            ContactCount++;
            TimeSpan StartTime = cntStopwatch.Elapsed;
            cntStopwatch.Start();
            try
            {
#endif
                LinkedList<Entity> entities = new LinkedList<Entity>();
                Predicate<Entity> comparer = EntityToPatternComparer.Get(entPattern, strMatchType, nameType);

                if (action != null)
                {
                    foreach (ContactInfo contact in EntityManager.LocalPlayer.Player.InteractInfo.NearbyContacts)
                    {
                        if (contact.Entity.IsValid && comparer(contact.Entity))
                        {
                            action(contact.Entity);
                            entities.AddLast(contact.Entity);
                        }
                    }
                    foreach (ContactInfo contact in EntityManager.LocalPlayer.Player.InteractInfo.NearbyInteractCritterEnts)
                    {
                        if (contact.Entity.IsValid && comparer(contact.Entity))
                        {
                            action(contact.Entity);
                            entities.AddLast(contact.Entity);
                        }
                    }
                }
                else
                {
                    foreach (ContactInfo contact in EntityManager.LocalPlayer.Player.InteractInfo.NearbyContacts)
                    {
                        if (contact.Entity.IsValid && comparer(contact.Entity))
                            entities.AddLast(contact.Entity);
                    }
                    foreach (ContactInfo contact in EntityManager.LocalPlayer.Player.InteractInfo.NearbyInteractCritterEnts)
                    {
                        if (contact.Entity.IsValid && comparer(contact.Entity))
                            entities.AddLast(contact.Entity);
                    }
                }
                return entities;
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
            }
#endif
        }

        /// <summary>
        /// Формирование списка Entities, способных к взаимодействию и соответствующих ключу
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action">Функтор действия, которое нужно выполнить над Entity, удовлетворяющем условиям</param>
        /// <returns></returns>
        public static LinkedList<Entity> GetContactEntities(CacheRecordKey key, Action<Entity> action = null)
        {
            if (key == null)
                return null;
#if DEBUG && PROFILING
            ContactCount++;
            TimeSpan StartTime = cntStopwatch.Elapsed;
            cntStopwatch.Start();
            try
            {
#endif
            LinkedList<Entity> entities = new LinkedList<Entity>();

                if (action != null)
                {
                    foreach (ContactInfo contact in EntityManager.LocalPlayer.Player.InteractInfo.NearbyContacts)
                    {
                        if (contact.Entity.IsValid && key.Comparer(contact.Entity))
                        {
                            action(contact.Entity);
                            entities.AddFirst(contact.Entity);
                        }
                    }
                    foreach (ContactInfo contact in EntityManager.LocalPlayer.Player.InteractInfo.NearbyInteractCritterEnts)
                    {
                        if (contact.Entity.IsValid && key.Comparer(contact.Entity))
                        {
                            action(contact.Entity);
                            entities.AddFirst(contact.Entity);
                        }
                    }
                }
                else
                {
                    foreach (ContactInfo contact in EntityManager.LocalPlayer.Player.InteractInfo.NearbyContacts)
                    {
                        if (contact.Entity.IsValid && key.Comparer(contact.Entity))
                            entities.AddFirst(contact.Entity);
                    }
                    foreach (ContactInfo contact in EntityManager.LocalPlayer.Player.InteractInfo.NearbyInteractCritterEnts)
                    {
                        if (contact.Entity.IsValid && key.Comparer(contact.Entity))
                            entities.AddFirst(contact.Entity);
                    }
                }
                return entities;
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
            }
#endif
        }
    }
}
