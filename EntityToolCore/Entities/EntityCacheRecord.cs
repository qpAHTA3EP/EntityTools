//#define PROFILING

using Astral.Classes;
using Astral.Classes.ItemFilter;
using EntityTools.Enums;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections;
using System.Collections.Generic;
using EntityTools;

namespace EntityCore.Entities
{
    /// <summary>
    /// Запись Кэша
    /// </summary>
    public class EntityCacheRecord : IEnumerable<Entity>
    {
#if PROFILING
        private static int RegenCount = 0;
        private static int EntitiesCount = 0;
        public static void ResetWatch()
        {
            RegenCount = 0;
            EntitiesCount = 0;
            ETLogger.WriteLine(LogType.Debug, $"EntityCacheRecord::ResetWatch()");
        }

        public static void LogWatch()
        {
            ETLogger.WriteLine(LogType.Debug, $"EntityCacheRecord: RegenCount: {RegenCount}");
        }
#endif
        public EntityCacheRecord(string p, ItemFilterStringType mp = ItemFilterStringType.Simple, EntityNameType nt = EntityNameType.NameUntranslated, EntitySetType est = EntitySetType.Complete)
        {
            Key = new EntityCacheRecordKey(p, mp, nt, est);
        }
        public EntityCacheRecord(EntityCacheRecordKey key)
        {
            Key = key;
        }

        public readonly EntityCacheRecordKey Key;

        public Timeout Timer { get; private set; } = new Timeout(0);

        private LinkedList<Entity> entities = new LinkedList<Entity>();

        public IEnumerator<Entity> GetEnumerator()
        {
            if (Timer.IsTimedOut)
                Regen();
            return entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (Timer.IsTimedOut)
                Regen();
            return entities.GetEnumerator();
        }

        /// <summary>
        /// Обновление кэша
        /// </summary>
        public void Regen()
        {
#if PROFILING
            RegenCount++;
#endif
            LinkedList<Entity> entts = Key.EntitySetType == EntitySetType.Contacts 
                ? SearchDirect.GetContactEntities(Key) 
                : SearchDirect.GetEntities(Key);

            if (entts != null)
                entities = entts;
            else entities.Clear();
#if PROFILING
            EntitiesCount += entities.Count;
#endif
            if (EntityManager.LocalPlayer.InCombat
                && !Astral.Quester.API.IgnoreCombat)
                Timer.ChangeTime(EntityTools.EntityTools.Config.EntityCache.GlobalCacheTime);
            else Timer.ChangeTime(EntityTools.EntityTools.Config.EntityCache.CombatCacheTime);
        }
        public void Regen(Action<Entity> action)
        {
#if PROFILING
            RegenCount++;
#endif
            LinkedList<Entity> entts = Key.EntitySetType == EntitySetType.Contacts 
                ? SearchDirect.GetContactEntities(Key, action) 
                : SearchDirect.GetEntities(Key, action);

            if (entts != null)
                entities = entts;
            else entities.Clear();
#if PROFILING
            EntitiesCount += entities.Count;
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
        public void Processing(Action<Entity> action)
        {
            if (Timer.IsTimedOut)
                Regen(action);
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
            }
        }


#if false
        public void Regen<TAgregator>(Func<TAgregator, Entity, TAgregator> proseccor, ref TAgregator agregator)
        {
#if PROFILING
            RegenCount++;
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
            EntitiesCount += entities.Count;
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
            return Key.Pattern.GetHashCode();
        }
    }

}
