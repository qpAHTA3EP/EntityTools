using Astral.Quester.Classes;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EntityPlugin
{
    public static class Tools
    {
        /// <summary>
        /// Поиск Entity из коллекции entities, у которого поле NameUntranslated соответствует шаблону entPattern, 
        /// и расположенного наиболее близко к персонажу 
        /// </summary>
        /// <param name="entities">Коллекция объектов Entity</param>
        /// <param name="entPattern">Строка-шаблон, которому должно соответствовать NameUntranslated у искомого Entity</param>
        /// <returns></returns>
        public static Entity FindClosestEntity(List<Entity> entities, string entPattern)
        {
            Entity closestEntity = new Entity(IntPtr.Zero);
            foreach (Entity entity in entities)
            {
                if (Regex.IsMatch(entity.NameUntranslated, entPattern))
                {
                    if (closestEntity.IsValid)
                    {
                        if (entity.Location.Distance3DFromPlayer < closestEntity.Location.Distance3DFromPlayer)
                            closestEntity = entity;
                    }
                    else closestEntity = entity;
                }
            }
            return closestEntity;
        }

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
    }
}
