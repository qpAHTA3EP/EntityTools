#define PROFILING

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Astral.Classes.ItemFilter;
using EntityTools.Enums;
using Infrastructure;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityTools.Tools.Entities
{
    /// <summary>
    /// Запись Кэша
    /// </summary>
    public class EntityCacheRecord : IEnumerable<Entity>
    {
        public EntityCacheRecord(string p, ItemFilterStringType mp = ItemFilterStringType.Simple, EntityNameType nt = EntityNameType.NameUntranslated, EntitySetType est = EntitySetType.Complete)
        {
            Key = new EntityCacheRecordKey(p, mp, nt, est);
            _initTime = DateTime.Now;
        }
        public EntityCacheRecord(EntityCacheRecordKey key)
        {
            Key = key;
            _initTime = DateTime.Now;
        }

#if PROFILING
        public static int TotalRegenCount;
        public static int TotalCachedEntities;

        private Stopwatch regenSw = new Stopwatch();
        private Stopwatch accessWs = new Stopwatch();

        public static void ResetWatch()
        {
            TotalRegenCount = 0;
            TotalCachedEntities = 0;
            ETLogger.WriteLine(LogType.Debug, $"EntityCacheRecord::ResetWatch()");
        }

        public static void LogWatch()
        {
            ETLogger.WriteLine(LogType.Debug, $"EntityCacheRecord: TotalRegenCount: {TotalRegenCount}");
        }
        
        /// <summary>
        /// Количество кэшированных <see cref="Entity"/>
        /// </summary>
        public uint Capacity => (uint)entities.Count;

        /// <summary>
        /// Время, прошедшее с момента последнего чтения записи
        /// </summary>
        public TimeSpan ElapsedTime => DateTime.Now - _lastAccessTime;

        public double Rank => _accessCount == 0 ? (DateTime.Now - _initTime).Ticks / (double)_accessCount : (DateTime.Now - _initTime).Ticks;

        /// <summary>
        /// Число обращений (считываний) к записи
        /// </summary>
        public int AccessCount => _accessCount;
        private int _accessCount;

        /// <summary>
        /// Число регенераций кэша
        /// </summary>
        public uint RegenCount => _regenCount;
        private uint _regenCount;

        /// <summary>
        /// Относительное Количество считываний в секунду с момента инициализации
        /// </summary>
        public double AccessPerSecond
        {
            get
            {
                var interval = DateTime.Now - _initTime;
                if (interval.Ticks > 0)
                    return _accessCount / interval.TotalSeconds;
                return _accessCount;
            }
        }

        /// <summary>
        /// Среднее время регенерации
        /// </summary>
        public TimeSpan RegenTimeAvg => _regenCount > 0 ? new TimeSpan(_regenTimeTotal.Ticks / _regenCount) : _regenTimeTotal;
        public TimeSpan RegenTimeTotal => _regenTimeTotal;
        private TimeSpan _regenTimeTotal;
        public TimeSpan RegenTimeMin => regenTimeMin;
        private TimeSpan regenTimeMin = TimeSpan.MaxValue;
        public TimeSpan RegenTimeMax => _regenTimeMax;
        private TimeSpan _regenTimeMax;

        /// <summary>
        /// Среднее время доступа
        /// </summary>
        public TimeSpan AccessTimeAvg => _accessCount > 0 ? new TimeSpan(_accessTimeTotal.Ticks / _accessCount) : _accessTimeTotal;
        public TimeSpan AccessTimeTotal => _accessTimeTotal;
        private TimeSpan _accessTimeTotal;
        public TimeSpan AccessTimeMax => _accessTimeMax;
        private TimeSpan _accessTimeMax;
        public TimeSpan AccessTimeMin => _accessTimeMin;
        private TimeSpan _accessTimeMin = TimeSpan.MaxValue;
#endif

        public string RecordKey => Key.ToString();
        public readonly EntityCacheRecordKey Key;

        //[Browsable(false)]
        //public Timeout Timer { get; private set; } = new Timeout(0);

        /// <summary>
        /// Отметка времени инициализации записи
        /// </summary>
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{HH.mm.ss.fffffff}")]
        public DateTime InitializationTime => _initTime;
        private DateTime _initTime;

        /// <summary>
        /// Отметка времени последнего доступа
        /// </summary>
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{HH.mm.ss.fffffff}")]
        public DateTime LastAccessTime => _lastAccessTime;
        private DateTime _lastAccessTime;

        /// <summary>
        /// Отметка времени последней регенации кэша
        /// </summary>
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{HH.mm.ss.fffffff}")]
        public DateTime LastRegenTime => _lastRegenTime;
        private DateTime _lastRegenTime;

        /// <summary>
        /// Отметка времени, после которой должна быть произведена регенерация кэша
        /// </summary>
        private DateTime _nextRegenTime;

        /// <summary>
        /// Кэшированные <see cref="Entity"/>
        /// </summary>
        private LinkedList<Entity> entities = new LinkedList<Entity>();

        public IEnumerator<Entity> GetEnumerator()
        {
            var now = DateTime.Now;
            if (_nextRegenTime <= now)
            {
                Regen();
                now = DateTime.Now;
            }
            _accessCount++;
            _lastAccessTime = now;
            return entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            var time = DateTime.Now;
            if (_nextRegenTime <= time)
            {
                Regen();
                time = DateTime.Now;
            }
            _accessCount++;
            _lastAccessTime = time;
            return entities.GetEnumerator();
        }

        /// <summary>
        /// Обновление кэша
        /// </summary>
        public void Regen()
        {
#if PROFILING
            _regenCount++;
            TotalRegenCount++;
            regenSw.Restart();
#endif
            LinkedList<Entity> entts = Key.EntitySetType == EntitySetType.Contacts 
                ? SearchDirect.GetContactEntities(Key) 
                : SearchDirect.GetEntities(Key);

            if (entts != null)
                entities = entts;
            else entities.Clear();
            _lastRegenTime = DateTime.Now;
            if (EntityManager.LocalPlayer.InCombat
                && !Astral.Quester.API.IgnoreCombat)
                _nextRegenTime = _lastRegenTime.AddMilliseconds(global::EntityTools.EntityTools.Config.EntityCache.GlobalCacheTime);
            else _nextRegenTime = _lastRegenTime.AddMilliseconds(global::EntityTools.EntityTools.Config.EntityCache.CombatCacheTime);
#if PROFILING
            regenSw.Stop();
            var interval = regenSw.Elapsed;
            _regenTimeTotal += interval;
            if (regenTimeMin > interval)
                regenTimeMin = interval;
            if (_regenTimeMax < interval)
                _regenTimeMax = interval;

            TotalCachedEntities += entities.Count;
#endif
        }
        public void Regen(Action<Entity> action)
        {
#if PROFILING
            _regenCount++;
            TotalRegenCount++;
            regenSw.Restart();
#endif
            LinkedList<Entity> entts = Key.EntitySetType == EntitySetType.Contacts 
                ? SearchDirect.GetContactEntities(Key, action) 
                : SearchDirect.GetEntities(Key, action);

            if (entts != null)
                entities = entts;
            else entities.Clear();
            _lastRegenTime = DateTime.Now;
            if (EntityManager.LocalPlayer.InCombat
                && !Astral.Quester.API.IgnoreCombat)
                _nextRegenTime = _lastRegenTime.AddMilliseconds(global::EntityTools.EntityTools.Config.EntityCache.GlobalCacheTime);
            else _nextRegenTime = _lastRegenTime.AddMilliseconds(global::EntityTools.EntityTools.Config.EntityCache.CombatCacheTime);
#if PROFILING
            regenSw.Stop();
            var time = regenSw.Elapsed;
            _regenTimeTotal += time;
            if (_regenTimeMax < time)
                _regenTimeMax = time; 
            else if (regenTimeMin > time)
                regenTimeMin = time;
            
            TotalCachedEntities += entities.Count;
#endif
        }

        /// <summary>
        /// Обработка (сканирование) кэшированных <see cref="Entity"/>
        /// </summary>
        /// <param name="action"></param>
        public void Processing(Action<Entity> action)
        {
#if PROFILING
            var start = DateTime.Now;
            accessWs.Restart();
#endif
            if (_nextRegenTime <= start)
            {
                Regen(action);
#if PROFILING
                accessWs.Stop();
                _accessCount++;
                _lastAccessTime = DateTime.Now;
                var time = accessWs.Elapsed;
                _accessTimeTotal += time;
                if (_accessTimeMax < time)
                    _accessTimeMax = time;
                else if (_accessTimeMin > time)
                    _accessTimeMin = time;
#endif
            }
            else if (entities != null && entities.Count > 0)
            {
                // Если Entity валидна - оно передается для обработки в action
                // в противном случае - удаляется из коллекции                
                LinkedListNode<Entity> eNode = entities.First;

                do
                {
                    if (Key.IsMatch(eNode.Value))
                        action(eNode.Value);
                    else entities.Remove(eNode);

                    eNode = eNode.Next;
                } while (eNode != null);
#if PROFILING
                _accessCount++;
                _lastAccessTime = DateTime.Now;
                var time = _lastAccessTime - start;
                _accessTimeTotal += time;
                if (_accessTimeMax < time)
                    _accessTimeMax = time;
                else if (_accessTimeMin > time)
                    _accessTimeMin = time;
#endif
            }
        }


#if false
        public void Regen<TAgregator>(Func<TAgregator, Entity, TAgregator> proseccor, ref TAgregator agregator)
        {
#if PROFILING
            TotalRegenCount++;
#endif
            LinkedList<Entity> entts;
            if (Key.EntitySetType == EntitySetType.Contacts)
                entts = SearchDirect.GetContactEntities(Key, proseccor, ref agregator);
            else entts = SearchDirect.GetEntities(Key, proseccor, ref agregator);

            if (entts != null)
                entities = entts;
            else
            {
                entities.Clear();
                agregator = default;
            }
#if PROFILING
            TotalCachedEntities += entities.Count;
#endif
            if (EntityManager.LocalPlayer.InCombat
                && !Astral.Quester.API.IgnoreCombat)
                Timer = new Timeout(EntityTools.EntityTools.Config.EntityCache.GlobalCacheTime);
            else Timer = new Timeout(EntityTools.EntityTools.Config.EntityCache.CombatCacheTime);
        }
        /// <summary>
        /// Обработка (сканирование) кэшированных <see cref="Entity"/>
        /// </summary>
        /// <param name="action"></param>
        public void Processing<TAgregator>(Func<TAgregator, Entity, TAgregator> processor, ref TAgregator agregator)
        {
            if (Timer.IsTimedOut)
                Regen(processor, ref agregator);
            else if (entities?.Count > 0)
            {
                // Если Entity валидна - оно передается для обработки в action
                // в противном случае - удаляется из коллекции                
                LinkedListNode<Entity> eNode = entities.First;
                do
                {
                    var ett = eNode.Value;
                    if (Key.IsMatch(ett))
                        agregator = processor(agregator, ett);
                    else entities.Remove(eNode);

                    eNode = eNode.Next;
                } while (eNode != null);
            }
        } 
#endif

#if false
        /// <summary>
        /// Поиск <see cref="Entity"/> с минимальной величиной, 
        /// </summary>
        public Entity Min<T>(Func<Entity, T> selector) where T : IComparable
        {
            Entity minEntity = null;
            if (Timer.IsTimedOut)
                minEntity = RegenMin(selector);
            else if (entities != null && entities.Count > 0)
            {
                // Если Entity валидна - оно передается для обработки в action
                // в противном случае - удаляется из коллекции                
                LinkedListNode<Entity> eNode = entities.First;
                LinkedListNode<Entity> next;
                while(!Key.IsMatch(eNode.Value))
                {
                    next = eNode.Next;
                    entities.Remove(eNode);
                    eNode = next;
                }

                if (eNode != null)
                {
                    next = eNode.Next;
                    var curEntity = eNode.Value;
                    T min = selector(curEntity);
                    do
                    {
                        if (Key.IsMatch(curEntity))
                        {
                            T val = selector(curEntity);
                            if (min.CompareTo(val) > 0)
                            {
                                minEntity = curEntity;
                                min = val;
                            }
                        }
                        else entities.Remove(eNode);

                        eNode = next;
                    } while (eNode != null);
                }
            }
            return minEntity;
        } 
#endif

        public bool Equals(EntityCacheRecord other)
        {
            return ReferenceEquals(this, other) || Key.Equals(other.Key);
        }

        public override int GetHashCode()
        {
            return Key.ToString().GetHashCode();
        }
    }

}
