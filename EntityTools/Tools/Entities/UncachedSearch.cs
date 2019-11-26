using Astral.Classes;
using Astral.Classes.ItemFilter;
using EntityTools.Enums;
using MyNW.Classes;
using MyNW.Internals;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EntityTools.Tools.Entities
{
    public static class UncachedSearch
    {
        /// <summary>
        /// Формирование списка Entities, соответствующих шаблону
        /// </summary>
        /// <param name="entPattern"></param>
        /// <param name="strMatchType"></param>
        /// <param name="nameType"></param>
        /// <returns></returns>
        public static List<Entity> GetEntities(string entPattern, ItemFilterStringType strMatchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated)
        {
            EntityComparerToPattern comparer = new EntityComparerToPattern(entPattern, strMatchType, nameType);

            return EntityManager.GetEntities()?.FindAll(comparer.Check);
        }

        /// <summary>
        /// Формирование списка Entities, способных к взаимодействию и соответствующих шаблону
        /// </summary>
        /// <param name="entPattern">Шаблон, которому должен соответствовать идентификатор (имя) Entity, заданные параметном </param>
        /// <param name="strMatchType">Способ сопоставления шаблона: Regex (регулярное выражение) или Simple (простой текст)</param>
        /// <param name="nameType">Идентификатор (имя) Entity, с которым сопостовляется entPattern</param>
        /// <returns></returns>
        public static List<Entity> GetContactEntities(string entPattern, ItemFilterStringType strMatchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated)
        {
            List<Entity> entities = new List<Entity>();
            EntityComparerToPattern comparer = new EntityComparerToPattern(entPattern, strMatchType, nameType);

            foreach (ContactInfo contact in EntityManager.LocalPlayer.Player.InteractInfo.NearbyContacts)
            {
                if (contact.Entity.IsValid && comparer.Check(contact.Entity))
                    entities.Add(contact.Entity);
            }
            foreach (ContactInfo contact in EntityManager.LocalPlayer.Player.InteractInfo.NearbyInteractCritterEnts)
            {
                if (contact.Entity.IsValid && comparer.Check(contact.Entity))
                    entities.Add(contact.Entity);
            }

            return entities;
        }

    }
}
