using Astral.Logic.NW;
using Astral.Quester.Classes;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace EntityTools.Tools
{
    public static class CommonTools
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

        /// <summary>
        /// Jпределяем местоположение простого шаблона matchText в идентификаторе pattern
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="matchText"></param>
        /// <returns></returns>
        public static SimplePatternPos GetSimplePatternPos(string pattern, out string matchText)
        {
            matchText = string.Empty;
            SimplePatternPos patternPos = SimplePatternPos.None;
            if(!string.IsNullOrEmpty(pattern))
                if (pattern[0] == '*')
                {
                    if (pattern[pattern.Length - 1] == '*')
                    {
                        patternPos = SimplePatternPos.Middle;
                        matchText = pattern.Trim('*');
                    }
                    else
                    {
                        patternPos = SimplePatternPos.End;
                        matchText = pattern.TrimStart('*');
                    }
                }
                else
                {
                    if (pattern[pattern.Length - 1] == '*')
                    {
                        patternPos = SimplePatternPos.Start;
                        matchText = pattern.TrimEnd('*');
                    }
                    else
                    {
                        patternPos = SimplePatternPos.Full;
                        matchText = pattern;
                    }
                }
            return patternPos;
        }

        /// <summary>
        /// Поиск вхождения подстроки строки с учетом заданного положения
        /// </summary>
        /// <param name="text"></param>
        /// <param name="patternPos"></param>
        /// <param name="trimedPattern"></param>
        /// <returns></returns>
        public static bool SimpleMaskTextComparer(string text, SimplePatternPos patternPos, string trimedPattern)
        {
            if (string.IsNullOrEmpty(text))
                if (string.IsNullOrEmpty(trimedPattern))
                    return true;
                else return false;
            else if (string.IsNullOrEmpty(trimedPattern))
                return false;            

            switch (patternPos)
            {
                case SimplePatternPos.Start:
                    return text.StartsWith(trimedPattern);
                case SimplePatternPos.Middle:
                    return text.Contains(trimedPattern);
                case SimplePatternPos.End:
                    return text.EndsWith(trimedPattern);
                case SimplePatternPos.Full:
                    return text == trimedPattern;
                default:
                    return text == trimedPattern;
            }
        }
        public static bool SimpleMaskTextComparer(string text, string pattern)
        {
            SimplePatternPos patternPos = GetSimplePatternPos(pattern, out string matchText);
            return SimpleMaskTextComparer(text, patternPos, matchText);
        }
        public static bool CompareToSimplePattern(this string @this, string pattern)
        {
            return SimpleMaskTextComparer(@this, pattern);
        }


        public static void FocusForm(Type t)
        {
            foreach (Form f in Application.OpenForms)
                if (f.GetType() == t)
                {
                    f.Focus();
                    return;
                }                   
        }
    }
}
