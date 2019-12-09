using Astral.Quester.Classes;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityTools.Tools
{
    public static class CustomRegionTools
    {
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
        /// Проверка нахождения объекта entity в границах региона customRegionName
        /// </summary>
        /// <param name="entity">Объект Entity, местонахождение которого проверяется</param>
        /// <param name="customRegionName">Наименование CustomRegion</param>
        /// <returns></returns>
        public static bool IsInCustomRegion(Entity entity, string customRegionName)
        {
            if (string.IsNullOrEmpty(customRegionName))
                return false;

            CustomRegion cReg = Astral.Quester.API.CurrentProfile.CustomRegions.Find(cr => cr.Name == customRegionName);

            return cReg != null || IsInCustomRegion(entity, cReg);
        }

        public static List<CustomRegion> GetCustomRegions(List<string> names)
        {
            if(names != null && names.Count > 0)
            return Astral.Quester.API.CurrentProfile.CustomRegions.FindAll((CustomRegion cr) =>
                                    names.Exists((string regName) => regName == cr.Name));
            else return null;
        }

        /// <summary>
        /// Методы расширения
        /// </summary>
        public static bool Within(this Entity entity, string customRegionName)
        {
            return IsInCustomRegion(entity, customRegionName);
        }
        public static bool Within(this Entity entity, CustomRegion customRegion)
        {
            return IsInCustomRegion(entity, customRegion);
        }
        public static bool Within(this CustomRegion customRegion, Entity entity)
        {
            return IsInCustomRegion(entity, customRegion);
        }
    }
}
