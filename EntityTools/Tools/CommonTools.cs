using Astral.Logic.NW;
using Astral.Quester.Classes;
using EntityTools.Enums;
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
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static bool SimpleMaskTextComparer(string text, SimplePatternPos patternPos, string pattern)
        {
            if (string.IsNullOrEmpty(text))
                if (string.IsNullOrEmpty(pattern))
                    return true;
                else return false;
            else if (string.IsNullOrEmpty(pattern))
                return false;            

            switch (patternPos)
            {
                case SimplePatternPos.Start:
                    return text.StartsWith(pattern);
                case SimplePatternPos.Middle:
                    return text.Contains(pattern);
                case SimplePatternPos.End:
                    return text.EndsWith(pattern);
                case SimplePatternPos.Full:
                    return text == pattern;
                default:
                    return text == pattern;
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
