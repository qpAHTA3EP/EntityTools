using Astral.Logic.NW;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using static Astral.Quester.Classes.Action;

namespace EntityPlugin.Tools
{
    public static class InteractionTools
    {
        /// <summary>
        /// Взаимодействие с Entity как с NPC
        /// Функции перемещения не задействованы
        /// </summary>
        /// <param name="target">цель взаимодействия</param>
        /// <param name="interactTime">время взаимодействия</param>
        /// <param name="dialogs">пункты диалога</param>
        /// <returns>Повторное взаимодействие c target возможно</returns>
        public static bool InteractNPC(Entity target, int interactTime, List<string> dialogs)
        {
            if (target != null && target.IsValid)
            {
                bool result = false;
                if (target.Critter.IsInteractable && target.InteractOption.IsValid && Approach.EntityForInteraction(target, null))
                {
                    MyNW.Internals.Movements.StopNavTo();
                    Thread.Sleep(500);
                    target.Interact();
                    Thread.Sleep(interactTime);
                    Interact.WaitForInteraction();
                    if (dialogs.Count > 0)
                    {
                        Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(interactTime);
                        while (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Options.Count == 0)
                        {
                            if (timeout.IsTimedOut)
                            {
                                result = target.Critter.IsInteractable && target.InteractOption.IsValid && target.InteractOption.CanInteract();
                                break;
                            }
                            Thread.Sleep(100);
                        }
                        Thread.Sleep(500);
                        using (List<string>.Enumerator enumerator = dialogs.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                string key = enumerator.Current;
                                EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.SelectOptionByKey(key, "");
                                Thread.Sleep(1000);
                            }
                        }
                    }
                    EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Close();
                    result = target.Critter.IsInteractable && target.InteractOption.IsValid && target.InteractOption.CanInteract();
                }
                return result;
            }
            return false;
        }

        /// <summary>
        /// Взаимодействие с Entity как с Node
        /// Функции перемещения не задействованы
        /// </summary>
        /// <param name="target">цель взаимодействия</param>
        /// <param name="interactTime">время взаимодействия</param>
        /// <param name="dialogs">пункты диалога</param>
        /// <returns></returns>
        //public static ActionResult InteractNode(Entity target, int interactTime, List<string> dialogs)
        //{
        //    target.
        //    if (target != null && target.IsValid)
        //    {
        //        ActionResult actnReslt = ActionResult.Running;

        //        if (!target.currentNode.Node.IsValid)
        //        {
        //            return Action.ActionResult.Running;
        //        }
        //        Interact.DynaNode dynaNode = this.currentNode;
        //        this.currentNode = null;
        //        Astral.Logic.NW.General.CloseContactDialog();
        //        Approach.NodeForInteraction(dynaNode, null);
        //        if (Attackers.InCombat)
        //        {
        //            return Action.ActionResult.Running;
        //        }
        //        Class1.MainEngine.Navigation.Stop();
        //        Thread.Sleep(500);
        //        if (!dynaNode.Node.WorldInteractionNode.Interact())
        //        {
        //            return Action.ActionResult.Running;
        //        }
        //        int num = this.InteractTime;
        //        if (num < 1000)
        //        {
        //            num = 1000;
        //        }
        //        Thread.Sleep(num);
        //        Interact.WaitForInteraction();
        //        if (this.Dialogs.Count > 0)
        //        {
        //            Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(5000);
        //            while (Class1.LocalPlayer.Player.InteractInfo.ContactDialog.Options.Count == 0)
        //            {
        //                if (timeout.IsTimedOut)
        //                {
        //                    return Action.ActionResult.Fail;
        //                }
        //                Thread.Sleep(100);
        //            }
        //            foreach (string key in this.Dialogs)
        //            {
        //                Class1.LocalPlayer.Player.InteractInfo.ContactDialog.SelectOptionByKey(key, this.RewardItemChoose);
        //                Thread.Sleep(1500);
        //            }
        //            Class1.LocalPlayer.Player.InteractInfo.ContactDialog.Close();
        //        }
        //        if (Game.IsLootFrameVisible())
        //        {
        //            GameCommands.SimulateFKey();
        //            Astral.Classes.Timeout timeout2 = new Astral.Classes.Timeout(5000);
        //            while (Game.IsLootFrameVisible())
        //            {
        //                if (timeout2.IsTimedOut)
        //                {
        //                    Game.CloseLootFrame();
        //                    return Action.ActionResult.Fail;
        //                }
        //                Thread.Sleep(200);
        //            }
        //        }
        //        foreach (ClientDialogBox clientDialogBox in UIManager.ClientDialogBoxes)
        //        {
        //            clientDialogBox.PerformOK();
        //            Thread.Sleep(500);
        //        }
        //        if (Class1.CurrentSettings.UsePathfinding3 && Class1.LocalPlayer.CurrentZoneMapInfo.MapType == ZoneMapType.Mission)
        //        {
        //            Thread.Sleep(2500);
        //            if (Class1.LocalPlayer.IsValid && !Class1.LocalPlayer.IsLoading)
        //            {
        //                Logger.WriteLine("Refresh tiles after specific node interaction ...");
        //                PathFinding.RegenTilesNearPlayer();
        //            }
        //        }
        //    }
        //    return ActionResult.Fail;
        //}

        /// <summary>
        /// Взаимодействие с Entity как с Node
        /// Функции перемещения не задействованы
        /// </summary>
        /// <param name="target">цель взаимодействия</param>
        /// <param name="interactTime">время взаимодействия</param>
        /// <param name="dialogs">пункты диалога</param>
        /// <returns>Повторное взаимодействие c target возможно</returns>
        public static bool InteractGeneric(Entity target, int interactTime, List<string> dialogs)
        {
            if (target != null && target.IsValid)
            {
                target.Location.Face();
                target.Location.FaceYaw();

                if (EntityManager.LocalPlayer.Player.InteractStatus.InteractOptions.Count > 0)
                {
                    bool result = false;
                    using (List<InteractOption>.Enumerator enumerator = EntityManager.LocalPlayer.Player.InteractStatus.InteractOptions.GetEnumerator())
                    {
                        if (!enumerator.MoveNext())
                        {
                            return false;
                        }
                        InteractOption interactOption = enumerator.Current;
                        if (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.IsValid)
                        {
                            Astral.Logic.NW.General.CloseContactDialog();
                            Thread.Sleep(1000);
                        }
                        if (interactOption.Entity.Pointer.Equals(target.Pointer))
                        {
                           
                            interactOption.Interact();
                            int num = interactTime;
                            if (num < 1000)
                            {
                                num = 1000;
                            }
                            Thread.Sleep(num);
                            Interact.WaitForInteraction();
                            if (dialogs.Count > 0)
                            {
                                Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(num);
                                while (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Options.Count == 0)
                                {
                                    if (timeout.IsTimedOut)
                                    {
                                        break;
                                    }
                                    Thread.Sleep(100);
                                }
                                foreach (string key in dialogs)
                                {
                                    EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.SelectOptionByKey(key, "");
                                    Thread.Sleep(1500);
                                }
                                EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Close();
                            }
                            if (Game.IsLootFrameVisible())
                            {
                                GameCommands.SimulateFKey();
                                Astral.Classes.Timeout timeout2 = new Astral.Classes.Timeout(5000);
                                while (Game.IsLootFrameVisible())
                                {
                                    if (timeout2.IsTimedOut)
                                    {
                                        Game.CloseLootFrame();
                                        break;
                                    }
                                    Thread.Sleep(200);
                                }
                            }
                            foreach (ClientDialogBox clientDialogBox in UIManager.ClientDialogBoxes)
                            {
                                clientDialogBox.PerformOK();
                                Thread.Sleep(500);
                            }
                            result = interactOption.IsValid && interactOption.CanInteract();                            
                        }
                    }
                    return result;
                }
                return false;
            }
            return false;
        }

        /// <summary>
        /// Взаимодействие с Entity как с Node
        /// Функции перемещения не задействованы
        /// </summary>
        /// <param name="target">цель взаимодействия</param>
        /// <param name="interactTime">время взаимодействия</param>
        /// <param name="dialogs">пункты диалога</param>
        /// <returns>Повторное взаимодействие c target возможно</returns>
        public static bool SimulateFKey(Entity target, int interactTime, List<string> dialogs)
        {
            if (target != null && target.IsValid)
            {
                target.Location.Face();
                Thread.Sleep(300);
                GameCommands.SimulateFKey();
                target.Location.Face();
                target.Location.FaceYaw();
                Thread.Sleep(300);
                GameCommands.SimulateFKey();
                Thread.Sleep(interactTime);
                if (target.InteractOption.IsValid)
                {
                    if (target.InteractOption.CanInteract())
                    {
                        Thread.Sleep(interactTime);
                    }
                    if (dialogs.Count > 0)
                    {
                        Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(5000);
                        while (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Options.Count == 0)
                        {
                            if (timeout.IsTimedOut)
                            {
                                break;
                            }
                            Thread.Sleep(100);
                        }
                        Thread.Sleep(500);
                        using (List<string>.Enumerator enumerator = dialogs.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                string key = enumerator.Current;
                                EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.SelectOptionByKey(key, "");
                                Thread.Sleep(1000);
                            }
                        }
                    }
                }
                return target.InteractOption.CanInteract();
            }
            return false;
        }
    }
}

