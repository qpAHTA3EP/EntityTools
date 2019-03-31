using Astral.Classes;
using Astral.Classes.ItemFilter;
using Astral.Logic.NW;
using Astral.Quester.Classes;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace EntityPlugin.Tools
{
    public static class SelectionTools
    {
        /// <summary>
        /// Поиск ближайшего Entity из entities, 
        /// у которого NameUntranslated соответствует шаблону регулярного выражения
        /// и соответствующего остальным условиям
        /// </summary>
        /// <param name="entities">Исходная коллекция Entity</param>
        /// <param name="entPattern">Шаблон, соответсвие которому ищется</param>
        /// <param name="regionCheck">Флаг проверки соответствия региона Entity и региона игрока</param>
        /// <param name="bkList">Список указателей запрещенных Entity</param>
        /// <returns>Найденное Entity</returns>
        public static Entity FindClosestEntityRegex(List<Entity> entities, string entPattern, bool regionCheck = false, TempBlackList<IntPtr> bkList = null)
        {
            Entity closestEntity = new Entity(IntPtr.Zero);
            if (!string.IsNullOrEmpty(entPattern) && entities != null)
            {
                if (bkList == null)
                {
                    bkList = new TempBlackList<IntPtr>();
                }
                foreach (Entity entity in entities)
                {
                    if ((!regionCheck || entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
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
        /// Поиск ближайшего Entity из entities, соответствующего условиям
        /// </summary>
        /// <param name="entities">Исходная коллекция Entity</param>
        /// <param name="entPattern">Шаблон, соответсвие которому ищется</param>
        /// <param name="type">Тип шаблона</param>
        /// <param name="regionCheck">Флаг проверки соответствия региона Entity и региона игрока</param>
        /// <param name="bkList">Список указателей запрещенных Entity</param>
        /// <returns>Найденное Entity</returns>
        public static Entity FindClosestEntity(List<Entity> entities, string entPattern, ItemFilterStringType type = ItemFilterStringType.Simple, bool regionCheck = false, TempBlackList<IntPtr> bkList = null)
        {
            Entity closestEntity = new Entity(IntPtr.Zero);
            if (!string.IsNullOrEmpty(entPattern) && entities != null)
            {
                if (bkList == null)
                {
                    bkList = new TempBlackList<IntPtr>();
                }

                switch (type)
                {
                    case ItemFilterStringType.Simple:
                        {
                            if (entPattern[0]!='*')
                            {
                                // Поиск шаблона в началае 
                                entPattern = entPattern.Trim('*');
                                foreach (Entity entity in entities)
                                {
                                    if (entity.NameUntranslated.StartsWith(entPattern)
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
                                    if (entity.NameUntranslated.Contains(entPattern)
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
                            break;
                        }
                    case ItemFilterStringType.Regex:
                        return FindClosestEntityRegex(entities, entPattern, regionCheck, bkList);
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
        public static Entity FindClosestInteractableEntityRegex(List<Entity> entities, string entPattern, bool regionCheck = false, TempBlackList<IntPtr> bkList = null)
        {
            Entity closestEntity = new Entity(IntPtr.Zero);
            if (!string.IsNullOrEmpty(entPattern) && entities != null)
            {
                if (bkList == null)
                {
                    bkList = new TempBlackList<IntPtr>();
                }
                foreach (Entity entity in entities)
                {
                    if (entity.Critter.IsInteractable
                        && (!regionCheck || entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                        && Regex.IsMatch(entity.NameUntranslated, entPattern)
                        && !bkList.IsBlackList(entity.Pointer)
                        /* && entity.InteractOption.CanInteract()*/)
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
        public static Entity FindClosestInteractableEntity(List<Entity> entities, string entPattern, ItemFilterStringType type = ItemFilterStringType.Simple, bool regionCheck = false, TempBlackList<IntPtr> bkList = null)
        {
            Entity closestEntity = new Entity(IntPtr.Zero);
            if (!string.IsNullOrEmpty(entPattern) && entities != null)
            {
                if (bkList == null)
                {
                    bkList = new TempBlackList<IntPtr>();
                }

                switch (type)
                {
                    case ItemFilterStringType.Simple:
                        {
                            if (entPattern[0] != '*')
                            {
                                // Поиск шаблона в началае 
                                entPattern = entPattern.Trim('*');
                                foreach (Entity entity in entities)
                                {
                                    if (entity.Critter.IsInteractable
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
                                    if (entity.Critter.IsInteractable
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
                            break;
                        }
                    case ItemFilterStringType.Regex:
                        return FindClosestInteractableEntityRegex(entities, entPattern, regionCheck, bkList);
                }
            }
            return closestEntity;
        }

        /// <summary>
        /// Поиск всех Entity из entities, удовлетворяющих условиям
        /// </summary>
        /// <param name="entities">Исходная коллекция Entity</param>
        /// <param name="entPattern">Шаблон, соответсвие которому ищется</param>
        /// <param name="type">Тип шаблона</param>
        /// <param name="regionCheck">Флаг проверки соответствия региона Entity и региона игрока</param>
        /// <param name="bkList">Список указателей запрещенных Entity</param>
        /// <returns>Список найденных Entity</returns>
        public static List<Entity> FindEntities(List<Entity> entities, string entPattern, ItemFilterStringType type = ItemFilterStringType.Simple, bool regionCheck = false, TempBlackList<IntPtr> bkList = null)
        {
            List<Entity> resultList = null;
            if (!string.IsNullOrEmpty(entPattern) && entities != null)
            {
                if (bkList == null)
                {
                    bkList = new TempBlackList<IntPtr>();
                }

                switch (type)
                {
                    case ItemFilterStringType.Simple:
                        {
                            if (entPattern[0] != '*')
                            {
                                // Поиск шаблона в началае 
                                entPattern = entPattern.Trim('*');
                                resultList = entities.FindAll((Entity x) => (x.NameUntranslated.StartsWith(entPattern) 
                                                                             && (!regionCheck || x.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                                             && !bkList.Contains(x.Pointer)));
                            }
                            else
                            {
                                // Поиск любого вхождения шаблона
                                entPattern = entPattern.Trim('*');
                                resultList = entities.FindAll((Entity x) => x.NameUntranslated.Contains(entPattern)
                                                                             && (!regionCheck || x.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                                             && !bkList.Contains(x.Pointer));
                            }
                            break;
                        }
                    case ItemFilterStringType.Regex:
                        resultList = entities.FindAll((Entity x) => Regex.IsMatch(x.NameUntranslated, entPattern)
                                                                                  && (!regionCheck || x.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                                                  && !bkList.Contains(x.Pointer));
                        break;
                }
            }
            return resultList;
        }
    }
}
