﻿using Astral.Classes;
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
        /// Поиск Entity из коллекции entities, у которого поле NameUntranslated соответствует шаблону entPattern, 
        /// и расположенного наиболее близко к персонажу 
        /// </summary>
        /// <param name="entities">Коллекция объектов Entity</param>
        /// <param name="entPattern">Строка-шаблон, которому должно соответствовать NameUntranslated у искомого Entity</param>
        /// <returns></returns>
        public static Entity FindClosestEntityRegex(List<Entity> entities, string entPattern, TempBlackList<IntPtr> bkList = null)
        {
            Entity closestEntity = new Entity(IntPtr.Zero);
            if (!string.IsNullOrEmpty(entPattern))
            {
                if (bkList == null)
                {
                    bkList = new TempBlackList<IntPtr>();
                }
                foreach (Entity entity in entities)
                {
                    if (Regex.IsMatch(entity.NameUntranslated, entPattern) 
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
        public static Entity FindClosestEntity(List<Entity> entities, string entPattern, ItemFilterStringType type, TempBlackList<IntPtr> bkList = null)
        {
            Entity closestEntity = new Entity(IntPtr.Zero);
            if (!string.IsNullOrEmpty(entPattern))
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
                        return FindClosestEntityRegex(entities, entPattern,bkList);
                }
            }
            return closestEntity;
        }
        /// <summary>
        /// Поиск Entity из коллекции entities, у которого поле NameUntranslated соответствует шаблону entPattern, 
        /// и расположенного наиболее близко к персонажу 
        /// </summary>
        /// <param name="entities">Коллекция объектов Entity</param>
        /// <param name="entPattern">Строка-шаблон, которому должно соответствовать NameUntranslated у искомого Entity</param>
        /// <param name="bkList">Список исключений Entity, которые запрещено выбирать в качестве цели</param>
        /// <returns></returns>
        public static Entity FindClosestInteractableEntityRegex(List<Entity> entities, string entPattern, TempBlackList<IntPtr> bkList = null)
        {
            Entity closestEntity = new Entity(IntPtr.Zero);
            if (!string.IsNullOrEmpty(entPattern))
            {
                if (bkList == null)
                {
                    bkList = new TempBlackList<IntPtr>();
                }
                foreach (Entity entity in entities)
                {
                    if (entity.Critter.IsInteractable
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
        public static Entity FindClosestInteractableEntity(List<Entity> entities, string entPattern, ItemFilterStringType type, TempBlackList<IntPtr> bkList = null)
        {
            Entity closestEntity = new Entity(IntPtr.Zero);
            if (!string.IsNullOrEmpty(entPattern))
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
                        return FindClosestInteractableEntityRegex(entities, entPattern, bkList);
                }
            }
            return closestEntity;
        }

        ///// <summary>
        ///// Поиск Entity из коллекции entities, у которого поле NameUntranslated соответствует шаблону entPattern, 
        ///// и расположенного наиболее близко к персонажу 
        ///// </summary>
        ///// <param name="entities">Коллекция объектов Entity</param>
        ///// <param name="entPattern">Строка-шаблон, которому должно соответствовать NameUntranslated у искомого Entity</param>
        ///// <returns></returns>
        //public static Entity FindClosestUninteractableEntity(List<Entity> entities, string entPattern)
        //{
        //    Entity closestEntity = new Entity(IntPtr.Zero);
        //    if (!string.IsNullOrEmpty(entPattern))
        //    {
        //        foreach (Entity entity in entities)
        //        {
        //            if (Regex.IsMatch(entity.NameUntranslated, entPattern) && !entity.InteractOption.IsValid/* && !entity.InteractOption.CanInteract()*/)
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
        /// Проверка нахождения объекта entity в границах региона region
        /// </summary>
        /// <param name="entity">Объект Entity, местонахождение которого проверяется</param>
        /// <param name="region">CustomRegion </param>
        /// <returns></returns>
        public static bool IsInCustomRegion(Entity entity, CustomRegion region)
        {
            if (region.Eliptic)
            {
                float x = region.Position.X;
                float y = region.Position.Y;
                float x2 = entity.X;
                float y2 = entity.Y;
                return (2f * (x2 - x) / (float)region.Width - 1f) * (2f * (x2 - x) / (float)region.Width - 1f) + (2f * (y2 - y) / (float)region.Height - 1f) * (2f * (y2 - y) / (float)region.Height - 1f) < 1f;
            }
            else
            {
                int num = region.Width;
                int num2 = region.Height;
                Vector3 vector = region.Position.Clone();
                if (num < 0)
                {
                    vector.X += (float)num;
                    num *= -1;
                }
                if (num2 < 0)
                {
                    vector.Y += (float)num2;
                    num2 *= -1;
                }
                Vector3 vector2 = new Vector3(vector.X + (float)num, vector.Y + (float)num2, 0f);
                Vector3 location = entity.Location;

                return location.X > vector.X && location.X < vector2.X && location.Y > vector.Y && location.Y < vector2.Y;
            }
        }

        /// <summary>
        /// Перемещение персонажа на заданный инстанс
        /// </summary>
        /// <param name="instNum">Номер инстанса (экземпляра карты) на который нужно переместиться</param>
        /// <returns></returns>
        public static Instances.ChangeInstanceResult ChangeInstance(uint instNum = 0)
        {
            if (!MapTransfer.CanChangeInstance)
            {
                Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(5000);
                while (!MapTransfer.CanChangeInstance)
                {
                    if (EntityManager.LocalPlayer.InCombat)
                    {
                        return Instances.ChangeInstanceResult.Combat;
                    }
                    if (timeout.IsTimedOut)
                    {
                        return Instances.ChangeInstanceResult.CantChange;
                    }
                    Thread.Sleep(200);
                }
            }
            if (EntityManager.LocalPlayer.InCombat)
            {
                return Instances.ChangeInstanceResult.Combat;
            }
            if (!MapTransfer.IsMapTransferFrameVisible())
            {
                MapTransfer.OpenMapTransferFrame();
                Thread.Sleep(3000);
            }

            PossibleMapChoice mapInstance = MapTransfer.PossibleMapChoices.Find(pmc => pmc.InstanceIndex == instNum);

            if (mapInstance != null && mapInstance.IsValid)
            {
                if (mapInstance.IsCurrent)
                    return Instances.ChangeInstanceResult.Success;

                if (!EntityManager.LocalPlayer.InCombat)
                {
                    Astral.Logger.WriteLine($"Change to instance {mapInstance.InstanceIndex} ...");
                    mapInstance.Transfer();
                    Thread.Sleep(7500);
                    while (EntityManager.LocalPlayer.IsLoading)
                    {
                        Thread.Sleep(500);
                    }
                    if (MapTransfer.IsMapTransferFrameVisible())
                    {
                        MapTransfer.CloseMapTransferFrame();
                    }
                    return Instances.ChangeInstanceResult.Success;
                }
            }
            MapTransfer.CloseMapTransferFrame();
            Thread.Sleep(500);
            
            return Instances.ChangeInstanceResult.NoValidChoice;
        }
    }
}
