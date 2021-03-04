using Astral.Classes;
using Astral.Classes.ItemFilter;
using EntityTools.Enums;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityCore.Entities
{
    /// <summary>
    /// Запись Кэша
    /// </summary>
    public class EntityCacheRecord
    {
#if DEBUG && PROFILING
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
        public EntityCacheRecord() { }
        public EntityCacheRecord(string p, ItemFilterStringType mp = ItemFilterStringType.Simple, EntityNameType nt = EntityNameType.NameUntranslated, EntitySetType est = EntitySetType.Complete)
        {
            Key = new EntityCacheRecordKey(p, mp, nt, est);
        }

        public EntityCacheRecordKey Key { get; }

        public Timeout Timer { get; private set; } = new Timeout(0);

        private LinkedList<Entity> entities = new LinkedList<Entity>();
        public LinkedList<Entity> Entities
        {
            get
            {
                if (Timer.IsTimedOut)
                    Regen();
                return entities;
            }
        }

        /// <summary>
        /// Обновление кэша
        /// </summary>
        public void Regen()
        {
#if DEBUG && PROFILING
            RegenCount++;
#endif
            LinkedList<Entity> entts;
            if (Key.EntitySetType == EntitySetType.Contacts)
                entts = SearchDirect.GetContactEntities(Key);
            else entts = SearchDirect.GetEntities(Key);

            if (entts != null)
                entities = entts;
            else entities.Clear();
#if DEBUG && PROFILING
            EntitiesCount += entities.Count;
#endif
            if (EntityManager.LocalPlayer.InCombat
                && !Astral.Quester.API.IgnoreCombat)
                Timer = new Timeout(EntityTools.EntityTools.Config.EntityCache.GlobalCacheTime);
            else Timer = new Timeout(EntityTools.EntityTools.Config.EntityCache.CombatCacheTime);
        }
        public void Regen(Action<Entity> action)
        {
#if DEBUG && PROFILING
            RegenCount++;
#endif
            LinkedList<Entity> entts;
            if (Key.EntitySetType == EntitySetType.Contacts)
                entts = SearchDirect.GetContactEntities(Key, action);
            else entts = SearchDirect.GetEntities(Key, action);

            if (entts != null)
                entities = entts;
            else entities.Clear();
#if DEBUG && PROFILING
            EntitiesCount += entities.Count;
#endif
            if (EntityManager.LocalPlayer.InCombat
                && !Astral.Quester.API.IgnoreCombat)
                Timer = new Timeout(EntityTools.EntityTools.Config.EntityCache.GlobalCacheTime);
            else Timer = new Timeout(EntityTools.EntityTools.Config.EntityCache.CombatCacheTime);
        }

        /// <summary>
        /// Обработка (сканирование) Entities
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
                    if (Key.Comparer(eNode.Value))
                        action(eNode.Value);
                    else entities.Remove(eNode);

                    eNode = eNode.Next;
                } while (eNode != null);
            }
        }

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
