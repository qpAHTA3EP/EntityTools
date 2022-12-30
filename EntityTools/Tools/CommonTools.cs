using System;
using System.Threading;
using System.Windows.Forms;
using Astral;
using Astral.Logic.NW;
using Infrastructure;
using MyNW.Classes;
using MyNW.Internals;
using Timeout = Astral.Classes.Timeout;

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
                Timeout timeout = new Timeout(5000);
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
                    Logger.WriteLine($"Change to instance {mapInstance.InstanceIndex} ...");
                    ETLogger.WriteLine($"Change to instance {mapInstance.InstanceIndex} ...");
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

        public static void FocusForm(Type t)
        {
            foreach (Form f in Application.OpenForms)
                if (f.GetType() == t)
                {
                    Astral.Controllers.Forms.InvokeOnMainThread(()=> f.Focus());
                    return;
                }                   
        }
    }
}
