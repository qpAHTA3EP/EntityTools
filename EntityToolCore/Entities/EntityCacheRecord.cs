#define PROFILING

using Astral.Classes.ItemFilter;
using EntityTools;
using EntityTools.Enums;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EntityCore.Entities
{
    /// <summary>
    /// Запись Кэша
    /// </summary>
    public class EntityCacheRecord : IEnumerable<Entity>
    {
        public EntityCacheRecord(string p, ItemFilterStringType mp = ItemFilterStringType.Simple, EntityNameType nt = EntityNameType.NameUntranslated, EntitySetType est = EntitySetType.Complete)
        {
            Key = new EntityCacheRecordKey(p, mp, nt, est);
            _initTick = Environment.TickCount;
        }
        public EntityCacheRecord(EntityCacheRecordKey key)
        {
            Key = key;
            _initTick = Environment.TickCount;
        }

#if PROFILING
        public static int TotalRegenCount;
        public static int TotalCachedEntities;

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
        public int Capacity => entities.Count;

        public int LeftTime => Environment.TickCount - _lastAccessTick;

        /// <summary>
        /// Число обращений (считываний) к записи
        /// </summary>
        public int AccessCount => _accessCount;
        private int _accessCount;

        /// <summary>
        /// Число регенераций кэша
        /// </summary>
        public int RegenCount => _regenCount;
        private int _regenCount;

        /// <summary>
        /// Относительное Количество считываний в секунду с момента инициализации
        /// </summary>
        public double AccessPerSecond
        {
            get
            {
                var ticks = Environment.TickCount - _initTick;
                if (ticks > 0)
                    return _accessCount * 1000d / ticks;
                return _accessCount*1000d;
            }
        }

        /// <summary>
        /// Среднее время регенерации
        /// </summary>
        public double RegenTicksAvg => _regenCount == 0 ? _regenTiсksTotal / (double)_regenCount : _regenTiсksTotal;
        private int _regenTiсksTotal;
        public int RegenTicksMin => _regenTiсksMin;
        private int _regenTiсksMin = int.MaxValue;
        public int RegenTicksMax => _regenTiсksMax;
        private int _regenTiсksMax;

        /// <summary>
        /// Среднее время доступа
        /// </summary>
        public double AccessTicksAvg => _regenCount == 0 ? _accessTiсksTotal / (double) _regenCount : _accessTiсksTotal;
        private int _accessTiсksTotal;
        public int AccessTicksMax => _accessTiсksMax;
        private int _accessTiсksMax;
        public int AccessTicksMin => _accessTiсksMin;
        private int _accessTiсksMin = int.MaxValue;
#endif

        public string RecordKey => Key.ToString();
        public readonly EntityCacheRecordKey Key;

        //[Browsable(false)]
        //public Timeout Timer { get; private set; } = new Timeout(0);

        /// <summary>
        /// Отметка времени инициализации записи
        /// </summary>
        public int InitializationTick => _initTick;
        private int _initTick;

        /// <summary>
        /// Отметка времени последнего доступа
        /// </summary>
        public int LastAccessTick => _lastAccessTick;
        private int _lastAccessTick;

        /// <summary>
        /// Отметка времени последней регенации кэша
        /// </summary>
        public int LastRegenTick => _lastRegenTick;
        private int _lastRegenTick;

        /// <summary>
        /// Отметка времени, после которой должна быть произведена регенерация кэша
        /// </summary>
        private int _nextRegenTick;

        /// <summary>
        /// Кэшированные <see cref="Entity"/>
        /// </summary>
        private LinkedList<Entity> entities = new LinkedList<Entity>();

        public IEnumerator<Entity> GetEnumerator()
        {
            var ticks = Environment.TickCount;
            if (_nextRegenTick <= ticks)
            {
                Regen();
                ticks = Environment.TickCount;
            }
            _accessCount++;
            _lastAccessTick = ticks;
            return entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            var ticks = Environment.TickCount;
            if (_nextRegenTick <= ticks)
            {
                Regen();
                ticks = Environment.TickCount;
            }
            _accessCount++;
            _lastAccessTick = ticks;
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
            int ticks = Environment.TickCount;
#endif
            LinkedList<Entity> entts = Key.EntitySetType == EntitySetType.Contacts 
                ? SearchDirect.GetContactEntities(Key) 
                : SearchDirect.GetEntities(Key);

            if (entts != null)
                entities = entts;
            else entities.Clear();
            _lastRegenTick = Environment.TickCount;
            if (EntityManager.LocalPlayer.InCombat
                && !Astral.Quester.API.IgnoreCombat)
                _nextRegenTick = _lastRegenTick + EntityTools.EntityTools.Config.EntityCache.GlobalCacheTime;
            else _nextRegenTick = _lastRegenTick + EntityTools.EntityTools.Config.EntityCache.CombatCacheTime;
#if PROFILING
            ticks = Environment.TickCount - ticks;
            _regenTiсksTotal += ticks;
            if (_regenTiсksMin > ticks)
                _regenTiсksMin = ticks;
            if (_regenTiсksMax < ticks)
                _regenTiсksMax = ticks;
            TotalCachedEntities += entities.Count;
#endif
        }
        public void Regen(Action<Entity> action)
        {
#if PROFILING
            _regenCount++;
            TotalRegenCount++;
            int ticks = Environment.TickCount;
#endif
            LinkedList<Entity> entts = Key.EntitySetType == EntitySetType.Contacts 
                ? SearchDirect.GetContactEntities(Key, action) 
                : SearchDirect.GetEntities(Key, action);

            if (entts != null)
                entities = entts;
            else entities.Clear();
            _lastRegenTick = Environment.TickCount;
            if (EntityManager.LocalPlayer.InCombat
                && !Astral.Quester.API.IgnoreCombat)
                _nextRegenTick = _lastRegenTick + EntityTools.EntityTools.Config.EntityCache.GlobalCacheTime;
            else _nextRegenTick = _lastRegenTick + EntityTools.EntityTools.Config.EntityCache.CombatCacheTime;
#if PROFILING
            ticks = Environment.TickCount - ticks;
            _regenTiсksTotal += ticks;
            if (_regenTiсksMin > ticks)
                _regenTiсksMin = ticks;
            if (_regenTiсksMax < ticks)
                _regenTiсksMax = ticks;
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
            var ticks = Environment.TickCount;
#endif
            if (_nextRegenTick <= ticks)
            {
                Regen(action);
#if PROFILING
                _accessCount++;
                _lastAccessTick = Environment.TickCount;
                ticks = _lastAccessTick - ticks;
                _accessTiсksTotal += ticks;
                if (_accessTiсksMin > ticks)
                    _accessTiсksMin = ticks;
                if (_accessTiсksMax < ticks)
                    _accessTiсksMax = ticks;
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
                _lastAccessTick = ticks;
                ticks = _lastAccessTick - ticks;
                _accessTiсksTotal += ticks;
                if (_accessTiсksMin > ticks)
                    _accessTiсksMin = ticks;
                if (_accessTiсksMax < ticks)
                    _accessTiсksMax = ticks;
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
