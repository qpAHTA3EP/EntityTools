//#define PROFILING

using System;
using System.Collections.Generic;
using Astral.Classes.ItemFilter;
using EntityTools.Enums;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityTools.Tools.Entities
{
    public static class SearchDirect
    {
#if PROFILING
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
            ETLogger.WriteLine(LogType.Debug, $"UncachedSearch::ResetWatch()");
        }

        public static void LogWatch()
        {
            if (Count > 0)
            {
                double avrgTime = (double)stopwatch.ElapsedMilliseconds / (double)Count;
                double avrgTicks = (double)stopwatch.ElapsedTicks / (double)Count;
                //EntityToolsLogger.WriteLine(LogType.Debug, $"SearchDirect::GetEntities(): Count: {Count}, TotalTime: {stopwatch.Elapsed}({stopwatch.ElapsedMilliseconds.ToString("N3")} ms), MinTime: {MinTime.TotalMilliseconds.ToString("N3")} ms ({MinTime.Ticks.ToString("N0")} ticks), MaxTime: {MaxTime.TotalMilliseconds.ToString("N3")} ms ({MaxTime.Ticks.ToString("N0")} ticks) , AverageTime: {avrgTime.ToString("N3")} ms ({avrgTicks.ToString("N0")} ticks)");
                ETLogger.WriteLine(LogType.Debug, $"SearchDirect::GetEntities():\tCount: {Count}, TotalTime: {stopwatch.Elapsed}({stopwatch.ElapsedMilliseconds.ToString("N0")} ms)");
                ETLogger.WriteLine(LogType.Debug, $"SearchDirect::GetEntities():\tMinTime: {MinTime.TotalMilliseconds.ToString("N3")} ms ({MinTime.Ticks.ToString("N0")} ticks)");
                ETLogger.WriteLine(LogType.Debug, $"SearchDirect::GetEntities():\tMaxTime: {MaxTime.TotalMilliseconds.ToString("N3")} ms ({MaxTime.Ticks.ToString("N0")} ticks)");
                ETLogger.WriteLine(LogType.Debug, $"SearchDirect::GetEntities():\tAverageTime: {avrgTime.ToString("N3")} ms ({avrgTicks.ToString("N0")} ticks)");
            }
            else ETLogger.WriteLine(LogType.Debug, $"SearchDirect::GetEntities(): Count: 0");
            if (ContactCount > 0)
            {
                double avrgTime = (double)cntStopwatch.ElapsedMilliseconds / (double)ContactCount;
                double avrgTicks = (double)cntStopwatch.ElapsedTicks / (double)ContactCount;
                //EntityToolsLogger.WriteLine(LogType.Debug, $"SearchDirect::GetContactEntities(): Count: {ContactCount}, TotalTime: {cntStopwatch.Elapsed}({cntStopwatch.ElapsedMilliseconds.ToString("N3")} ms), MinTime: {ContactMinTime.TotalMilliseconds.ToString("N3")} ms ({ContactMinTime.Ticks.ToString("N0")} ticks), MaxTime: {ContactMaxTime.TotalMilliseconds.ToString("N3")} ms ({ContactMaxTime.Ticks.ToString("N0")} ticks) , AverageTime: {avrgTime.ToString("N3")} ms ({avrgTicks.ToString("N0")} ticks)");
                ETLogger.WriteLine(LogType.Debug, $"SearchDirect::GetContactEntities():\tCount: {ContactCount}, TotalTime: {cntStopwatch.Elapsed}({cntStopwatch.ElapsedMilliseconds.ToString("N0")} ms)");
                ETLogger.WriteLine(LogType.Debug, $"SearchDirect::GetContactEntities():\tMinTime: {ContactMinTime.TotalMilliseconds.ToString("N3")} ms ({ContactMinTime.Ticks.ToString("N0")} ticks)");
                ETLogger.WriteLine(LogType.Debug, $"SearchDirect::GetContactEntities():\tMaxTime: {ContactMaxTime.TotalMilliseconds.ToString("N3")} ms ({ContactMaxTime.Ticks.ToString("N0")} ticks)");
                ETLogger.WriteLine(LogType.Debug, $"SearchDirect::GetContactEntities():\tAverageTime: {avrgTime.ToString("N3")} ms ({avrgTicks.ToString("N0")} ticks)");
            }
            else ETLogger.WriteLine(LogType.Debug, $"SearchDirect::GetContactEntities(): Count: 0");
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
#if PROFILING
            Count++;
            TimeSpan StartTime = stopwatch.Elapsed;
            stopwatch.Start();
            try
            {
#endif
                LinkedList<Entity> entities = new LinkedList<Entity>();
                Predicate<Entity> comparer = EntityComparer.Get(entPattern, strMatchType, nameType);

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
#if PROFILING
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
        public static LinkedList<Entity> GetEntities(EntityCacheRecordKey key, Action<Entity> action = null)
        {
            if (key == null)
                return null;
#if PROFILING
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
                    if (key.IsMatch(e))
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
                    if (key.IsMatch(e))
                        entities.AddLast(e);
            }
            return entities;
#if PROFILING
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
        /// Формирование списка <seealso cref="Entity"/>, соответствующих <paramref name="key"/>.
        /// К каждому из отобранных <seealso cref="Entity"/> применяется <paramref name="processor"/>, а результат обработки возврадается в виде <paramref name="agregator"/>
        /// </summary>
        public static LinkedList<Entity> GetEntities<TAgregator>(EntityCacheRecordKey key, Func<TAgregator, Entity, TAgregator> processor, ref TAgregator agregator)
        {
            if (key == null)
                return null;
#if PROFILING
            Count++;
            TimeSpan StartTime = stopwatch.Elapsed;
            stopwatch.Start();
            try
            {
#endif
            LinkedList<Entity> entities = new LinkedList<Entity>();
            if (processor != null)
            {
                foreach (Entity e in EntityManager.GetEntities())
                {
                    if (key.IsMatch(e))
                    {
                        if (e.IsValid && key.IsMatch(e))
                        {
                            agregator = processor(agregator, e);
                            entities.AddFirst(e);
                        }
                        entities.AddLast(e);
                    }
                }
            }
            else
            {
                foreach (Entity e in EntityManager.GetEntities())
                    if (key.IsMatch(e))
                        entities.AddLast(e);
            }
            return entities;
#if PROFILING
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
        /// Формирование списка <seealso cref="Entity"/>, соответствующих <paramref name="key"/>.
        /// К каждому из отобранных <seealso cref="Entity"/> применяется <paramref name="processor"/>, а результат обработки возврадается в виде <paramref name="agregator"/>
        /// </summary>
        public static void AgregateEntities<TAgregator>(Func<TAgregator, Entity, TAgregator> processor, ref TAgregator agregator)
        {
#if PROFILING
            Count++;
            TimeSpan StartTime = stopwatch.Elapsed;
            stopwatch.Start();
            try
            {
#endif
            if (processor is null) return;
            
            foreach (Entity e in EntityManager.GetEntities())
                agregator = processor(agregator, e);
#if PROFILING
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
        /// Формирование списка <seealso cref="Entity"/>, способных к взаимодействию и соответствующих шаблону <paramref name="entPattern"/>
        /// К каждому из отобранных <seealso cref="Entity"/> применяется <paramref name="action"/>
        /// </summary>
        /// <param name="entPattern">Шаблон, которому должен соответствовать идентификатор (имя) Entity, заданные параметном </param>
        /// <param name="strMatchType">Способ сопоставления шаблона: Regex (регулярное выражение) или Simple (простой текст)</param>
        /// <param name="nameType">Идентификатор (имя) Entity, с которым сопостовляется entPattern</param>
        /// <param name="action">Функтор действия, которое нужно выполнить над Entity, удовлетворяющем условиям</param>
        /// <returns></returns>
        public static LinkedList<Entity> GetContactEntities(string entPattern, ItemFilterStringType strMatchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated, Action<Entity> action = null)
        {
#if PROFILING
            ContactCount++;
            TimeSpan StartTime = cntStopwatch.Elapsed;
            cntStopwatch.Start();
            try
            {
#endif
                LinkedList<Entity> entities = new LinkedList<Entity>();
                Predicate<Entity> comparer = EntityComparer.Get(entPattern, strMatchType, nameType);

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
#if PROFILING
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
        /// Формирование списка <seealso cref="Entity"/>, способных к взаимодействию и соответствующих <paramref name="key"/>.
        /// К каждому из отобранных <seealso cref="Entity"/> применяется <paramref name="action"/>
        /// </summary>
        public static LinkedList<Entity> GetContactEntities(EntityCacheRecordKey key, Action<Entity> action = null)
        {
            if (key == null)
                return null;
#if PROFILING
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
                        if (contact.Entity.IsValid && key.IsMatch(contact.Entity))
                        {
                            action(contact.Entity);
                            entities.AddFirst(contact.Entity);
                        }
                    }
                    foreach (ContactInfo contact in EntityManager.LocalPlayer.Player.InteractInfo.NearbyInteractCritterEnts)
                    {
                        if (contact.Entity.IsValid && key.IsMatch(contact.Entity))
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
                        if (contact.Entity.IsValid && key.IsMatch(contact.Entity))
                            entities.AddFirst(contact.Entity);
                    }
                    foreach (ContactInfo contact in EntityManager.LocalPlayer.Player.InteractInfo.NearbyInteractCritterEnts)
                    {
                        if (contact.Entity.IsValid && key.IsMatch(contact.Entity))
                            entities.AddFirst(contact.Entity);
                    }
                }
                return entities;
#if PROFILING
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
        /// Формирование списка <seealso cref="Entity"/>, спосо,ных к взаимодействию и соответствующих <paramref name="key"/>.
        /// К каждому из отобранных <seealso cref="Entity"/> применяется <paramref name="proseccor"/>, а результат обработки возврадается в виде <paramref name="agregator"/>
        /// </summary>
        public static LinkedList<Entity> GetContactEntities<TAgregator>(EntityCacheRecordKey key, Func<TAgregator, Entity, TAgregator> proseccor, ref TAgregator agregator)
        {
            if (key == null)
                return null;
#if PROFILING
            ContactCount++;
            TimeSpan StartTime = cntStopwatch.Elapsed;
            cntStopwatch.Start();
            try
            {
#endif
            LinkedList<Entity> entities = new LinkedList<Entity>();

            if (proseccor != null)
            {
                foreach (ContactInfo contact in EntityManager.LocalPlayer.Player.InteractInfo.NearbyContacts)
                {
                    var ett = contact.Entity;
                    if (ett.IsValid && key.IsMatch(ett))
                    {
                        agregator = proseccor(agregator, ett);
                        entities.AddLast(ett);
                    }
                }
                foreach (ContactInfo contact in EntityManager.LocalPlayer.Player.InteractInfo.NearbyInteractCritterEnts)
                {
                    var ett = contact.Entity;
                    if (ett.IsValid && key.IsMatch(ett))
                    {
                        agregator = proseccor(agregator, ett);
                        entities.AddLast(ett);
                    }
                }
            }
            else
            {
                foreach (ContactInfo contact in EntityManager.LocalPlayer.Player.InteractInfo.NearbyContacts)
                {
                    var ett = contact.Entity;
                    if (ett.IsValid && key.IsMatch(ett))
                        entities.AddFirst(ett);
                }
                foreach (ContactInfo contact in EntityManager.LocalPlayer.Player.InteractInfo.NearbyInteractCritterEnts)
                {
                    var ett = contact.Entity;
                    if (ett.IsValid && key.IsMatch(ett))
                        entities.AddFirst(ett);
                }
            }
            return entities;
#if PROFILING
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
        public static void AgregateContactEntities<TAgregator>(Func<TAgregator, Entity, TAgregator> proseccor, ref TAgregator agregator)
        {
#if PROFILING
            ContactCount++;
            TimeSpan StartTime = cntStopwatch.Elapsed;
            cntStopwatch.Start();
            try
            {
#endif
            if (proseccor is null) return;
            
            foreach (ContactInfo contact in EntityManager.LocalPlayer.Player.InteractInfo.NearbyContacts)
                agregator = proseccor(agregator, contact.Entity);
            foreach (ContactInfo contact in EntityManager.LocalPlayer.Player.InteractInfo.NearbyInteractCritterEnts)
                agregator = proseccor(agregator, contact.Entity);
#if PROFILING
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
