using Astral.Classes;
using Astral.Classes.ItemFilter;
using Astral.Quester.Classes;
using MyNW.Classes;
using MyNW.Internals;
using System;

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EntityTools.Tools
{
    public static class EntitySelectionTools
    {
        public delegate bool MatchPatternDelegate(Entity entity, string str);

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
        /// <param name="bkList">Список указателей запрещенных (игнорируемых) Entity</param>
        /// <param name="interactable">Если True, искать только Entity с которыми можно взаимодействать</param>
        /// <returns>Найденное Entity</returns>
        public static Entity FindClosestEntity(List<Entity> entities, string entPattern, ItemFilterStringType strMatchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated, bool healthCheck = false, float range = 0, bool regionCheck = false, List<string> customRegionNames = null, TempBlackList<IntPtr> bkList = null, bool interactable = false)
        {
            Entity closestEntity = new Entity(IntPtr.Zero);
            if (!string.IsNullOrEmpty(entPattern) && entities != null)
            {
                if (bkList == null)
                    bkList = new TempBlackList<IntPtr>();

                switch(nameType)
                {
                    case EntityNameType.NameUntranslated:
                        switch (strMatchType)
                        {
                            case ItemFilterStringType.Simple:
                                return FindClosestEntitySimpleNameUntranslated(entities, entPattern, healthCheck, range, regionCheck, customRegionNames, bkList, interactable);
                            case ItemFilterStringType.Regex:
                                return FindClosestEntityRegexNameUntranslated(entities, entPattern, healthCheck, range, regionCheck, customRegionNames, bkList, interactable);
                        }
                        break;
                    case EntityNameType.InternalName:
                        switch(strMatchType)
                        {
                            case ItemFilterStringType.Simple:
                                return FindClosestEntitySimpleNameInternal(entities, entPattern, healthCheck, range, regionCheck, customRegionNames, bkList, interactable);
                            case ItemFilterStringType.Regex:
                                return FindClosestEntityRegexNameInternal(entities, entPattern, healthCheck, range, regionCheck, customRegionNames, bkList, interactable);
                        }
                        break;
                }
            }
            return closestEntity;
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
        /// <param name="bkList">Список указателей запрещенных (игнорируемых) Entity</param>
        /// <returns>Найденное Entity</returns>
        public static Entity FindClosestContactEntity(string entPattern, ItemFilterStringType strMatchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated, bool healthCheck = false, float range = 0, bool regionCheck = false, List<string> customRegionNames = null, TempBlackList<IntPtr> bkList = null)
        {
            List<Entity> entities = new List<Entity>();

            foreach (ContactInfo contact in EntityManager.LocalPlayer.Player.InteractInfo.NearbyContacts)
            {
                if (contact.Entity.IsValid)
                {
                    if (bkList != null && bkList.Contains(contact.Entity.Pointer))
                        continue;

                    entities.Add(contact.Entity);
                }
            }
            foreach (ContactInfo contact in EntityManager.LocalPlayer.Player.InteractInfo.NearbyInteractCritterEnts)
            {
                if (contact.Entity.IsValid)
                {
                    if (bkList != null && bkList.Contains(contact.Entity.Pointer))
                        continue;

                    entities.Add(contact.Entity);
                }
            }
            return FindClosestEntity(entities, entPattern, strMatchType, nameType, healthCheck, range, regionCheck, customRegionNames, bkList, true);
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
        /// <param name="bkList">Список указателей запрещенных Entity</param>
        /// <returns>Найденное Entity</returns>
        private static Entity FindClosestEntityRegexNameInternal(List<Entity> entities, string entPattern, bool healthCheck = false, float range = 0, bool regionCheck = false, List<string> customRegionNames = null, TempBlackList<IntPtr> bkList = null, bool interactable = false)
        {
            Entity closestEntity = new Entity(IntPtr.Zero);

            List<CustomRegion> customRegions = (customRegionNames != null && customRegionNames.Count > 0) ? Astral.Quester.API.CurrentProfile.CustomRegions.FindAll((CustomRegion cr) =>
                                            customRegionNames.Exists((string regName) => regName == cr.Name)) : null;


            if (!string.IsNullOrEmpty(entPattern) && entities != null)
            {
                if (bkList == null)
                    bkList = new TempBlackList<IntPtr>();

                foreach (Entity entity in entities)
                {
                    if ((!regionCheck || entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                        && (!healthCheck || !entity.IsDead)
                        && (!interactable || entity.Critter.IsInteractable)
                        && (customRegions == null || customRegions.Count == 0 || customRegions.FindIndex(cr => CommonTools.IsInCustomRegion(entity, cr)) > 0)
                        && (range == 0 || entity.Location.Distance3DFromPlayer < range)
                        && Regex.IsMatch(entity.InternalName, entPattern) 
                        && !bkList.Contains(entity.Pointer))
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
        /// <param name="bkList">Список указателей запрещенных Entity</param>
        /// <returns>Найденное Entity</returns>
        private static Entity FindClosestEntityRegexNameUntranslated(List<Entity> entities, string entPattern, bool healthCheck = false, float range = 0, bool regionCheck = false, List<string> customRegionNames = null, TempBlackList<IntPtr> bkList = null, bool interactable = false)
        {
            Entity closestEntity = new Entity(IntPtr.Zero);

            List<CustomRegion> customRegions = (customRegionNames != null && customRegionNames.Count > 0) ? Astral.Quester.API.CurrentProfile.CustomRegions.FindAll((CustomRegion cr) =>
                                            customRegionNames.Exists((string regName) => regName == cr.Name)) : null;


            if (!string.IsNullOrEmpty(entPattern) && entities != null)
            {
                if (bkList == null)
                    bkList = new TempBlackList<IntPtr>();

                foreach (Entity entity in entities)
                {
                    if ((!regionCheck || entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                        && (!healthCheck || !entity.IsDead)
                        && (!interactable || entity.Critter.IsInteractable)
                        && (customRegions == null || customRegions.Count == 0 || customRegions.FindIndex(cr => CommonTools.IsInCustomRegion(entity, cr)) > 0)
                        && (range == 0 || entity.Location.Distance3DFromPlayer < range)
                        && Regex.IsMatch(entity.NameUntranslated, entPattern)
                        && !bkList.Contains(entity.Pointer))
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
        /// <param name="bkList">Список указателей запрещенных Entity</param>
        /// <returns>Найденное Entity</returns>
        private static Entity FindClosestEntitySimpleNameInternal(List<Entity> entities, string entPattern, bool healthCheck = false, float range = 0, bool regionCheck = false, List<string> customRegionNames = null, TempBlackList<IntPtr> bkList = null, bool interactable = false)
        {
            Entity closestEntity = new Entity(IntPtr.Zero);
            if (!string.IsNullOrEmpty(entPattern) && entities != null)
            {
                if (bkList == null)
                {
                    bkList = new TempBlackList<IntPtr>();
                }

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
                            && (customRegions == null || customRegions.Count == 0 || customRegions.FindIndex(cr => CommonTools.IsInCustomRegion(entity, cr)) >= 0)
                            && entity.InternalName.StartsWith(entPattern)
                            && (!regionCheck || entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                            && !bkList.Contains(entity.Pointer))
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
                            && (customRegions == null || customRegions.Count == 0 || customRegions.FindIndex(cr => CommonTools.IsInCustomRegion(entity, cr)) >= 0)
                            && entity.InternalName.Contains(entPattern)
                            && (!regionCheck || entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                            && !bkList.Contains(entity.Pointer))
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
        /// <param name="bkList">Список указателей запрещенных Entity</param>
        /// <returns>Найденное Entity</returns>
        private static Entity FindClosestEntitySimpleNameUntranslated(List<Entity> entities, string entPattern, bool healthCheck = false, float range = 0, bool regionCheck = false, List<string> customRegionNames = null, TempBlackList<IntPtr> bkList = null, bool interactable = false)
        {
            Entity closestEntity = new Entity(IntPtr.Zero);
            if (!string.IsNullOrEmpty(entPattern) && entities != null)
            {
                if (bkList == null)
                {
                    bkList = new TempBlackList<IntPtr>();
                }

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
                            && (customRegions == null || customRegions.Count == 0 || customRegions.FindIndex(cr => CommonTools.IsInCustomRegion(entity, cr)) >= 0)
                            && entity.NameUntranslated.StartsWith(entPattern)
                            && (!regionCheck || entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                            && !bkList.Contains(entity.Pointer))
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
                            && (customRegions == null || customRegions.Count == 0 || customRegions.FindIndex(cr => CommonTools.IsInCustomRegion(entity, cr)) >= 0)
                            && entity.NameUntranslated.Contains(entPattern)
                            && (!regionCheck || entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                            && !bkList.Contains(entity.Pointer))
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
        /// Поиск ближайшего Entity из entities, с которым можно взаимодействовать, 
        /// у которого NameUntranslated соответствует шаблону регулярного выражения
        /// и соответствующего остальным условиям
        /// </summary>
        /// <param name="entities">Исходная коллекция Entity</param>
        /// <param name="entPattern">Шаблон, соответсвие которому ищется</param>
        /// <param name="regionCheck">Флаг проверки соответствия региона Entity и региона игрока</param>
        /// <param name="bkList">Список указателей запрещенных Entity</param>
        /// <returns>Найденное Entity</returns>
        //private static Entity FindClosestInteractableEntityRegex(List<Entity> entities, string entPattern, bool regionCheck = false, TempBlackList<IntPtr> bkList = null)
        //{
        //    Entity closestEntity = new Entity(IntPtr.Zero);
        //    if (!string.IsNullOrEmpty(entPattern) && entities != null)
        //    {
        //        if (bkList == null)
        //        {
        //            bkList = new TempBlackList<IntPtr>();
        //        }
        //        foreach (Entity entity in entities)
        //        {
        //            if (entity.Critter.IsInteractable
        //                && (!regionCheck || entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
        //                && Regex.IsMatch(entity.NameUntranslated, entPattern)
        //                && !bkList.IsBlackList(entity.Pointer)
        //                /* && entity.InteractOption.CanInteract()*/)
        //            {
        //                if (closestEntity.IsValid)
        //                {
        //                    if (entity.Location.Distance3DFromPlayer < closestEntity.Location.Distance3DFromPlayer)
        //                        closestEntity = entity;
        //                }
        //                else closestEntity = entity;
        //            }
        //        }
        //    }
        //    return closestEntity;
        //}

        /// <summary>
        /// Поиск ближайшего Entity из entities, с которым можно взаимодействовать, 
        /// у которого NameUntranslated соответствует шаблону регулярного выражения
        /// и соответствующего остальным условиям
        /// </summary>
        /// <param name="entities">Исходная коллекция Entity</param>
        /// <param name="entPattern">Шаблон, соответсвие которому ищется</param>
        /// <param name="type">Тип шаблона</param>
        /// <param name="regionCheck">Флаг проверки соответствия региона Entity и региона игрока</param>
        /// <param name="bkList">Список указателей запрещенных Entity</param>
        /// <returns>Найденное Entity</returns>
        //public static Entity FindClosestInteractableEntity(List<Entity> entities, string entPattern, ItemFilterStringType type = ItemFilterStringType.Simple, bool regionCheck = false, TempBlackList<IntPtr> bkList = null)
        //{
        //    Entity closestEntity = new Entity(IntPtr.Zero);
        //    if (!string.IsNullOrEmpty(entPattern) && entities != null)
        //    {
        //        if (bkList == null)
        //        {
        //            bkList = new TempBlackList<IntPtr>();
        //        }

        //        switch (type)
        //        {
        //            case ItemFilterStringType.Simple:
        //                {
        //                    if (entPattern[0] != '*')
        //                    {
        //                        // Поиск шаблона в началае 
        //                        entPattern = entPattern.Trim('*');
        //                        foreach (Entity entity in entities)
        //                        {
        //                            if (entity.CanInteract //entity.Critter.IsInteractable
        //                                && entity.NameUntranslated.StartsWith(entPattern)
        //                                && (!regionCheck || entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
        //                                && !bkList.Contains(entity.Pointer))
        //                            {
        //                                if (closestEntity.IsValid)
        //                                {
        //                                    if (entity.Location.Distance3DFromPlayer < closestEntity.Location.Distance3DFromPlayer)
        //                                        closestEntity = entity;
        //                                }
        //                                else closestEntity = entity;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        // Поиск любого вхождения шаблона
        //                        entPattern = entPattern.Trim('*');
        //                        foreach (Entity entity in entities)
        //                        {
        //                            if (entity.CanInteract // entity.Critter.IsInteractable
        //                                && entity.NameUntranslated.Contains(entPattern)
        //                                && (!regionCheck || entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
        //                                && !bkList.Contains(entity.Pointer))
        //                            {
        //                                if (closestEntity.IsValid)
        //                                {
        //                                    if (entity.Location.Distance3DFromPlayer < closestEntity.Location.Distance3DFromPlayer)
        //                                        closestEntity = entity;
        //                                }
        //                                else closestEntity = entity;
        //                            }
        //                        }
        //                    }
        //                    break;
        //                }
        //            case ItemFilterStringType.Regex:
        //                return FindClosestInteractableEntityRegex(entities, entPattern, regionCheck, bkList);
        //        }
        //    }
        //    return closestEntity;
        //}

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
        /// <param name="bkList">Список указателей запрещенных (игнорируемых) Entity</param>
        /// <param name="interactable">Если True, искать только Entity с которыми можно взаимодействать</param>
        /// <returns>Список найденных Entity</returns>
        public static List<Entity> FindAllEntities(List<Entity> entities, string entPattern, ItemFilterStringType strMatchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated, 
            bool healthCheck = false, bool regionCheck = false, List<string> customRegionNames = null, TempBlackList<IntPtr> bkList = null)
        {
            List<Entity> resultList = null;
            if (!string.IsNullOrEmpty(entPattern) && entities != null)
            {
                if (bkList == null)
                {
                    bkList = new TempBlackList<IntPtr>();
                }

                List<CustomRegion> customRegions = (customRegionNames != null && customRegionNames.Count > 0) ? Astral.Quester.API.CurrentProfile.CustomRegions.FindAll((CustomRegion cr) =>
                                                customRegionNames.Exists((string regName) => regName == cr.Name)) : null;

                switch (strMatchType)
                {
                    case ItemFilterStringType.Simple:
                        {

                            switch (nameType)
                            {
                                case EntityNameType.InternalName:
                                    return FindAllEntitiesSimpleNameInternal(entities, entPattern, healthCheck, regionCheck, customRegionNames, bkList);
                                case EntityNameType.NameUntranslated:
                                    return FindAllEntitiesSimpleNameUntranslated(entities, entPattern, healthCheck, regionCheck, customRegionNames, bkList);
                            }
                            break;
                        }
                    case ItemFilterStringType.Regex:
                        return FindAllEntitiesRegex(entities, entPattern, nameType, healthCheck, regionCheck, customRegionNames, bkList);
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
        /// <param name="bkList">Список указателей запрещенных (игнорируемых) Entity</param>
        /// <returns>Список всех найденных Entity</returns>
        public static List<Entity> FindAllContactEntities(string entPattern, ItemFilterStringType strMatchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated, 
            bool healthCheck = false, bool regionCheck = false, List<string> customRegionNames = null, TempBlackList<IntPtr> bkList = null)
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

            return FindAllEntities(entities, entPattern, strMatchType, nameType, healthCheck, regionCheck, customRegionNames, bkList);
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
        /// <param name="bkList">Список указателей запрещенных Entity</param>
        /// <returns>Список найденных Entity</returns>
        private static List<Entity> FindAllEntitiesSimpleNameUntranslated(List<Entity> entities, string entPattern, bool healthCheck = false, bool regionCheck = false, List<string> customRegionNames = null, TempBlackList<IntPtr> bkList = null)
        {
            List<Entity> resultList = null;
            if (!string.IsNullOrEmpty(entPattern) && entities != null)
            {
                if (bkList == null)
                {
                    bkList = new TempBlackList<IntPtr>();
                }

                List<CustomRegion> customRegions = (customRegionNames != null && customRegionNames.Count > 0) ? Astral.Quester.API.CurrentProfile.CustomRegions.FindAll((CustomRegion cr) =>
                                                customRegionNames.Exists((string regName) => regName == cr.Name)) : null;

                if (entPattern[0] != '*')
                {
                    // Поиск шаблона в началае 
                    entPattern = entPattern.Trim('*');
                    resultList = entities.FindAll((Entity x) => (x.NameUntranslated.StartsWith(entPattern)
                                                                    && (!regionCheck || x.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                                    && (!healthCheck || !x.IsDead)
                                                                    && (customRegions == null || customRegions.Count == 0 || customRegions.FindIndex(cr => CommonTools.IsInCustomRegion(x, cr)) >= 0)
                                                                    && !bkList.Contains(x.Pointer)));
                }
                else
                {
                    // Поиск любого вхождения шаблона
                    entPattern = entPattern.Trim('*');
                    resultList = entities.FindAll((Entity x) => x.NameUntranslated.Contains(entPattern)
                                                                    && (!regionCheck || x.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                                    && (!healthCheck || !x.IsDead)
                                                                    && (customRegions == null || customRegions.Count == 0 || customRegions.FindIndex(cr => CommonTools.IsInCustomRegion(x, cr)) >= 0)
                                                                    && !bkList.Contains(x.Pointer));
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
        /// <param name="bkList">Список указателей запрещенных Entity</param>
        /// <returns>Список найденных Entity</returns>
        private static List<Entity> FindAllEntitiesSimpleNameInternal(List<Entity> entities, string entPattern, bool healthCheck = false, bool regionCheck = false, List<string> customRegionNames = null, TempBlackList<IntPtr> bkList = null)
        {
            List<Entity> resultList = null;
            if (!string.IsNullOrEmpty(entPattern) && entities != null)
            {
                if (bkList == null)
                {
                    bkList = new TempBlackList<IntPtr>();
                }

                List<CustomRegion> customRegions = (customRegionNames != null && customRegionNames.Count > 0) ? Astral.Quester.API.CurrentProfile.CustomRegions.FindAll((CustomRegion cr) =>
                                                customRegionNames.Exists((string regName) => regName == cr.Name)) : null;

                if (entPattern[0] != '*')
                {
                    // Поиск шаблона в началае 
                    entPattern = entPattern.Trim('*');
                    resultList = entities.FindAll((Entity x) => (x.InternalName.StartsWith(entPattern)
                                                                    && (!regionCheck || x.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                                    && (!healthCheck || !x.IsDead)
                                                                    && (customRegions == null || customRegions.Count == 0 || customRegions.FindIndex(cr => CommonTools.IsInCustomRegion(x, cr)) >= 0)
                                                                    && !bkList.Contains(x.Pointer)));
                }
                else
                {
                    // Поиск любого вхождения шаблона
                    entPattern = entPattern.Trim('*');
                    resultList = entities.FindAll((Entity x) => x.InternalName.Contains(entPattern)
                                                                    && (!regionCheck || x.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                                    && (!healthCheck || !x.IsDead)
                                                                    && (customRegions == null || customRegions.Count == 0 || customRegions.FindIndex(cr => CommonTools.IsInCustomRegion(x, cr)) >= 0)
                                                                    && !bkList.Contains(x.Pointer));
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
        /// <param name="bkList">Список указателей запрещенных Entity</param>
        /// <returns>Список найденных Entity</returns>
        private static List<Entity> FindAllEntitiesRegex(List<Entity> entities, string entPattern, EntityNameType nameType = EntityNameType.NameUntranslated, bool healthCheck = false, bool regionCheck = false, List<string> customRegionNames = null, TempBlackList<IntPtr> bkList = null)
        {
            List<Entity> resultList = null;
            if (!string.IsNullOrEmpty(entPattern) && entities != null)
            {
                if (bkList == null)
                {
                    bkList = new TempBlackList<IntPtr>();
                }

                List<CustomRegion> customRegions = (customRegionNames != null && customRegionNames.Count > 0) ? Astral.Quester.API.CurrentProfile.CustomRegions.FindAll((CustomRegion cr) =>
                                                customRegionNames.Exists((string regName) => regName == cr.Name)) : null;

                switch(nameType)
                {
                    case EntityNameType.InternalName: resultList = entities.FindAll((Entity x) => Regex.IsMatch(x.InternalName, entPattern)
                                                                                && (!regionCheck || x.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                                                && (!healthCheck || !x.IsDead)
                                                                                && (customRegions == null || customRegions.Count == 0 || customRegions.FindIndex(cr => CommonTools.IsInCustomRegion(x, cr)) >= 0)
                                                                                && !bkList.Contains(x.Pointer));
                        break;
                    case EntityNameType.NameUntranslated: resultList = entities.FindAll((Entity x) => Regex.IsMatch(x.NameUntranslated, entPattern)
                                                                                && (!regionCheck || x.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                                                && (!healthCheck || !x.IsDead)
                                                                                && (customRegions == null || customRegions.Count == 0 || customRegions.FindIndex(cr => CommonTools.IsInCustomRegion(x, cr)) >= 0)
                                                                                && !bkList.Contains(x.Pointer));
                        break;
                }
            }
            return resultList;
        }
    }
}
