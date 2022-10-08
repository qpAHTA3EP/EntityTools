using Astral.Quester.Classes;
using EntityTools.Tools.CustomRegions;
using MyNW.Classes;
using System.Collections.Generic;

namespace EntityTools.Extensions
{
    public static class CustomRegionExtensions
    {
        /// <summary>
        /// Проверка нахождения объекта <paramref name="position"/> в границах региона <paramref name="region"/>
        /// </summary>
        public static bool IsInCustomRegion(Vector3 position, CustomRegion region)
        {
            float x = position.X;
            float y = position.Y;
            var regPos = region.Position;
            float x1 = regPos.X;
            float y1 = regPos.Y;
            float width = region.Width;
            float height = region.Height;

            if (width == 0 || height == 0)
                return false;

            if (region.Eliptic)
            {
                float dx = 2f * (x - x1) / width - 1f;
                float dy = 2f * (y - y1) / height - 1f;
                return dx * dx + dy * dy <= 1f;
            }
            else
            {
                float x2 = x1;
                float y2 = y1;
                if (width < 0)
                    x1 += width;
                else x2 = x1 + width;
                if (height < 0)
                    y1 += height;
                else y2 = y1 + height;

                return x > x1 && x < x2 && y > y1 && y < y2;
            }
        }
        public static bool IsInCustomRegion(Entity entity, CustomRegion region)
        {
#if false
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
#elif false
            float x = entity.X;
            float y = entity.Y;
            var regPos = region.Position;
            float x1 = regPos.X;
            float y1 = regPos.Y;
            float width = region.Width;
            float height = region.Height;

            if (width == 0 || height == 0)
                return false;

            if (region.Eliptic)
            {
                float dx = 2f * (x - x1) / width - 1f;
                float dy = 2f * (y - y1) / height - 1f;
                return dx * dx + dy * dy <= 1f;
            }
            else
            {
                float x2 = x1;
                float y2 = y1;
                if (width < 0)
                    x1 += width;
                else x2 = x1 + width;
                if (height < 0)
                    y1 += height;
                else y2 = y1 + height;

                return x > x1 && x < x2 && y > y1 && y < y2;
            }
#else
            return IsInCustomRegion(entity.Location, region);
#endif
        }

        /// <summary>
        /// Проверка нахождения объекта <paramref name="entity"/> в границах региона <paramref name="customRegionName"/>
        /// </summary>
        public static bool IsInCustomRegion(Entity entity, string customRegionName)
        {
            if (string.IsNullOrEmpty(customRegionName))
                return false;

            CustomRegion cReg = Astral.Quester.API.CurrentProfile.CustomRegions.Find(cr => cr.Name == customRegionName);

            return cReg != null && IsInCustomRegion(entity, cReg);
        }
        public static bool IsInCustomRegion(Vector3 position, string customRegionName)
        {
            if (string.IsNullOrEmpty(customRegionName))
                return false;

            CustomRegion cReg = Astral.Quester.API.CurrentProfile.CustomRegions.Find(cr => cr.Name == customRegionName);

            return cReg != null && IsInCustomRegion(position, cReg);
        }
        public static List<CustomRegion> GetCustomRegions(List<string> names)
        {
            if (names != null && names.Count > 0)
                return Astral.Quester.API.CurrentProfile.CustomRegions.FindAll(cr =>
                                        names.Exists(regName => regName == cr.Name));
            return null;
        }

        /// <summary>
        /// Проверка нахождения <paramref name="entity"/> в пределах <seealso cref="CustomRegion"/> c именем <paramref name="customRegionName"/>
        /// </summary>
        public static bool Within(this Entity entity, string customRegionName)
        {
            return IsInCustomRegion(entity, customRegionName);
        }
        /// <summary>
        /// Проверка нахождения <paramref name="entity"/> в пределах <paramref name="customRegion"/>
        /// </summary>
        public static bool Within(this Entity entity, CustomRegion customRegion)
        {
            return IsInCustomRegion(entity, customRegion);
        }
        /// <summary>
        /// Проверка нахождения <paramref name="entity"/> в пределах <paramref name="crEntry"/>
        /// </summary>
        public static bool Within(this Entity entity, CustomRegionEntry crEntry)
        {
            var cr = crEntry.CustomRegion;
            return cr is null ? false : IsInCustomRegion(entity, cr);
        }
        /// <summary>
        /// Проверка нахождения <paramref name="position"/> в пределах <seealso cref="CustomRegion"/> c именем <paramref name="customRegionName"/>
        /// </summary>
        public static bool Within(this Vector3 position, string customRegionName)
        {
            return IsInCustomRegion(position, customRegionName);
        }
        /// <summary>
        /// Проверка нахождения <paramref name="position"/> в пределах <paramref name="customRegion"/>
        /// </summary>
        public static bool Within(this Vector3 position, CustomRegion customRegion)
        {
            return IsInCustomRegion(position, customRegion);
        }
        /// <summary>
        /// Проверка нахождения <paramref name="position"/> в пределах <paramref name="crEntry"/>
        /// </summary>
        public static bool Within(this Vector3 position, CustomRegionEntry crEntry)
        {
            var cr = crEntry.CustomRegion;
            return cr is null ? false : IsInCustomRegion(position, cr);
        }
        /// <summary>
        /// Проверка нахождения <paramref name="entity"/> в пределах <paramref name="customRegion"/>
        /// </summary>
        public static bool Cover(this CustomRegion customRegion, Entity entity)
        {
            return IsInCustomRegion(entity, customRegion);
        }
        /// <summary>
        /// Проверка нахождения <paramref name="position"/> в пределах <paramref name="customRegion"/>
        /// </summary>
        public static bool Cover(this CustomRegion customRegion, Vector3 position)
        {
            return IsInCustomRegion(position, customRegion);
        }
        /// <summary>
        /// Проверка нахождения <paramref name="entity"/> в пределах <paramref name="crEntry"/>
        /// </summary>
        public static bool Cover(this CustomRegionEntry crEntry, Entity entity)
        {
            var cr = crEntry.CustomRegion;
            return cr is null ? false : IsInCustomRegion(entity, cr);
        }
        /// <summary>
        /// Проверка нахождения <paramref name="position"/> в пределах <paramref name="crEntry"/>
        /// </summary>
        public static bool Cover(this CustomRegionEntry crEntry, Vector3 position)
        {
            var cr = crEntry.CustomRegion;
            return cr is null ? false : IsInCustomRegion(position, cr);
        }
        /// <summary>
        /// Проверка нахождения <paramref name="entity"/> в пределах любого <seealso cref="CustomRegion"/> из списка <paramref name="customRegions"/>
        /// </summary>
        public static bool Cover(this IEnumerable<CustomRegion> customRegions, Entity entity)
        {
#if false
            return customRegions.Any((CustomRegion cr) => IsInCustomRegion(entity, cr)); 
#else
            foreach (var cr in customRegions)
                if (IsInCustomRegion(entity, cr))
                    return true;
            return false;
#endif
        }
        /// <summary>
        /// Проверка нахождения <paramref name="position"/> в пределах любого <seealso cref="CustomRegion"/> из списка <paramref name="customRegions"/>
        /// </summary>
        public static bool Cover(this IEnumerable<CustomRegion> customRegions, Vector3 position)
        {
#if false
            return customRegions.Any((CustomRegion cr) => IsInCustomRegion(position, cr)); 
#else
            foreach (var cr in customRegions)
                if (IsInCustomRegion(position, cr))
                    return true;
            return false;
#endif
        }
    }
}
