﻿using Astral.Quester.Classes;
using MyNW.Classes;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EntityCore.Extensions
{
    public static class CustomRegionExtentions
    {
        /// <summary>
        /// Проверка нахождения объекта entity в границах региона region
        /// </summary>
        /// <param name="entity">Объект Entity, местонахождение которого проверяется</param>
        /// <param name="region">CustomRegion </param>
        /// <returns></returns>
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
#else
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
#endif
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

#if false
        /// <summary>
        /// Делегат, заполняющий DataGridView списком итемов
        /// </summary>
        /// <param name="dgv"></param>
        internal static void CustomRegionList2DataGridView(List<string> regions, DataGridView dgv)
        {
            int indSelect = dgv.Columns.Contains("clmnSelect") ? dgv.Columns["clmnSelect"].DisplayIndex : -1;
            int indItems = dgv.Columns.Contains("clmnItems") ? dgv.Columns["clmnItems"].DisplayIndex : -1;
            if (indSelect == -1 || indItems == -1)
                return;

            dgv.Rows.Clear();
            foreach (CustomRegion cr in Astral.Quester.API.CurrentProfile.CustomRegions)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgv);
                row.Cells[indItems].Value = cr.Name;
                row.Cells[indSelect].Value = (regions != null && regions.Contains(cr.Name));
                dgv.Rows.Add(row);
            }
        }

        /// <summary>
        /// Делегат, формирующий список выбранных итемов из DataGridView
        /// </summary>
        /// <param name="dgv"></param>
        internal static void DataGridView2CustomRegionList(DataGridView dgv, ref List<string> regions)
        {
            int indSelect = dgv.Columns.Contains("clmnSelect") ? dgv.Columns["clmnSelect"].DisplayIndex : -1;
            int indItems = dgv.Columns.Contains("clmnItems") ? dgv.Columns["clmnItems"].DisplayIndex : -1;
            if (indSelect == -1 || indItems == -1)
                return;

            regions = new List<string>(dgv.Rows.Count);

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.Cells[indSelect].Value.Equals(true))
                {
                    regions.Add(row.Cells[indItems].Value.ToString());
                }
            }
#endif
    }
}
