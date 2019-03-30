#define ShowDebugMsg

using Astral.Logic.NW;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using static Astral.Logic.NW.Approach;
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
        public static bool InteractNPC(Entity target, int interactTime = 1000, List<string> dialogs = null)
        {
            if (target != null && target.IsValid)
            {
                bool result = false;
                if (target.Critter.IsInteractable && target.InteractOption.IsValid/* && Approach.EntityByDistance(target, target.Critter.InteractDistance, null)/*Approach.EntityForInteraction(target, null)*/)
                {
                    target.Location.Face();
                    target.Location.FaceYaw();

                    MyNW.Internals.Movements.StopNavTo();
                    //Thread.Sleep(500);
                    target.Interact();
                    Thread.Sleep(interactTime);
                    Interact.WaitForInteraction();
                    if (dialogs != null && dialogs.Count > 0)
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
        /// Взаимодействие с Entity 
        /// </summary>
        /// <param name="target">цель взаимодействия</param>
        /// <param name="interactTime">время взаимодействия</param>
        /// <param name="distance">дистанция до Entity на которой необходимо выполнять взаимодействие</param>
        /// <param name="dialogs">пункты диалога</param>
        /// <returns>Повторное взаимодействие c target возможно</returns>
        public static bool FollowAndInteractNPC(Entity target, int interactTime = 1000, float distance = 5, List<string> dialogs = null, Func<BreakInfos> breakFunc = null)
        {
            //Astral.Classes.Timeout interactTimeout = new Astral.Classes.Timeout(interactTime);
            while (target != null && target.IsValid/* && !interactTimeout.IsTimedOut*//* && target.InteractOption.IsValid*/)
            {
#if ShowDebugMsg
                Astral.Logger.WriteLine($"<{nameof(FollowAndInteractNPC)}>: Call '{nameof(Approach.EntityByDistance)}'");
#endif
                if (Approach.EntityByDistance(target, distance, breakFunc))
                {

                    target.Location.Face();
                    target.Location.FaceYaw();
#if ShowDebugMsg
                    Astral.Logger.WriteLine($"<{nameof(FollowAndInteractNPC)}>: Call '{nameof(MyNW.Internals.Movements.StopNavTo)}'");
#endif

                    MyNW.Internals.Movements.StopNavTo();
                    //Thread.Sleep(500);
#if ShowDebugMsg
                    Astral.Logger.WriteLine($"<{nameof(FollowAndInteractNPC)}>: Call '{nameof(target.Interact)}'");
#endif
                    target.Interact();
                    Thread.Sleep(1000);//Thread.Sleep(interactTime);
#if ShowDebugMsg
                    Astral.Logger.WriteLine($"<{nameof(FollowAndInteractNPC)}>: Call '{nameof(Interact.WaitForInteraction)}'");
#endif
                    Interact.WaitForInteraction();
                    if (dialogs != null && dialogs.Count > 0)
                    {
#if ShowDebugMsg
                        Astral.Logger.WriteLine($"<{nameof(FollowAndInteractNPC)}>: Wait for Dialog's window appears");
#endif
                        Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(interactTime);
                        while (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Options.Count == 0)
                        {
                            if (timeout.IsTimedOut)
                            {
                                break;
                            }
                            Thread.Sleep(100);
                        }
                        Thread.Sleep(500);
#if ShowDebugMsg
                        Astral.Logger.WriteLine($"<{nameof(FollowAndInteractNPC)}>: Processing Dialogs");
#endif
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

#if ShowDebugMsg
                    bool dist = false,
                        tarIsIntearctable = false,
                        tarInteractOptionIsValid = false,
                        tarCanInteract = false;

                    if (target.IsValid)
                    {
                        dist = target.Location.Distance3DFromPlayer <= distance;
                        Astral.Logger.WriteLine($"<{nameof(FollowAndInteractNPC)}>: Check Distance({target.Location.Distance3DFromPlayer})<'{distance}': {dist}");
                        tarIsIntearctable = target.Critter.IsInteractable;
                        Astral.Logger.WriteLine($"<{nameof(FollowAndInteractNPC)}>: Check Target is Interactable: {tarIsIntearctable}");
                        tarInteractOptionIsValid = target.InteractOption.IsValid;
                        Astral.Logger.WriteLine($"<{nameof(FollowAndInteractNPC)}>: Check Target InteractOption is valid: {tarInteractOptionIsValid}");
                        tarCanInteract = target.InteractOption.CanInteract();
                        Astral.Logger.WriteLine($"<{nameof(FollowAndInteractNPC)}>: Check Target CanInteract: {tarCanInteract}");

                        if (dist && !(tarIsIntearctable && tarInteractOptionIsValid && tarCanInteract))
                        {
                            Astral.Logger.WriteLine($"<{nameof(FollowAndInteractNPC)}>: Target cann't be interacted. Return 'false'");
                            return false;
                        }
                    }
                    else
                    {
                        Astral.Logger.WriteLine($"<{nameof(FollowAndInteractNPC)}>: Target is not valid. Return 'false'");
                        return false;
                    }

#else
                    if (!target.IsValid 
                        || (target.Location.Distance3DFromPlayer <= distance 
                        && !(target.Critter.IsInteractable && target.InteractOption.IsValid && target.InteractOption.CanInteract())))
                    {
                        return false;
                    }
#endif
                }
                else
                {
#if ShowDebugMsg
                    Astral.Logger.WriteLine($"<{nameof(FollowAndSimulateFKey)}>: Approach to Target [{target.Pointer}] fail.");
#endif
                    break;
                }
            }
#if ShowDebugMsg
            Astral.Logger.WriteLine($"<{nameof(FollowAndInteractNPC)}>: Return 'false'");
#endif
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
        public static bool InteractGeneric(Entity target, int interactTime = 1000, List<string> dialogs = null)
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
                            if (dialogs!=null && dialogs.Count > 0)
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
        public static bool SimulateFKey(Entity target, int interactTime = 1000, List<string> dialogs = null)
        {
            if (target != null && target.IsValid)
            {
                target.Location.Face();
/*                Thread.Sleep(300);
                GameCommands.SimulateFKey();*/
                target.Location.Face();
                target.Location.FaceYaw();
                Thread.Sleep(300);
                GameCommands.SimulateFKey();
                Thread.Sleep(interactTime);
                if (EntityManager.LocalPlayer.Player.InteractInfo.IsValid/*target.InteractOption.IsValid*/)
                {
                    /*if (target.CanInteract)
                    {
                        Thread.Sleep(interactTime);
                    }*/
                    if (dialogs != null && dialogs.Count > 0)
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
                return target.CanInteract;
            }
            return false;
        }


        /// <summary>
        /// Взаимодействие с Entity как с Node
        /// Функции перемещения не задействованы
        /// </summary>
        /// <param name="target">цель взаимодействия</param>
        /// <param name="interactTime">время взаимодействия</param>
        /// <param name="distance">дистанция до Entity на которой необходимо выполнять взаимодействие</param>
        /// <param name="dialogs">пункты диалога</param>
        /// <returns>Повторное взаимодействие c target возможно</returns>
        public static bool FollowAndSimulateFKey(Entity target, int interactTime = 1000, float distance = 5, List<string> dialogs = null, Func<BreakInfos> breakFunc = null)
        {
            while (target != null && target.IsValid)
            {
#if ShowDebugMsg
                Astral.Logger.WriteLine($"<{nameof(FollowAndSimulateFKey)}>: Call '{nameof(Approach.EntityByDistance)}'");
#endif
                if (Approach.EntityByDistance(target, distance, breakFunc))
                {
                    target.Location.Face();
                    target.Location.FaceYaw();
#if ShowDebugMsg
                    Astral.Logger.WriteLine($"<{nameof(FollowAndSimulateFKey)}>: Call '{nameof(GameCommands.SimulateFKey)}'");
#endif
                    GameCommands.SimulateFKey();
                    Thread.Sleep(interactTime);

                    //Approach.EntityByDistance(target, distance, breakFunc);
                    //GameCommands.SimulateFKey();
                    //Thread.Sleep(interactTime);

                    if (EntityManager.LocalPlayer.Player.InteractInfo.IsValid)
                    {

                        if (dialogs != null && dialogs.Count > 0)
                        {
#if ShowDebugMsg
                            Astral.Logger.WriteLine($"<{nameof(FollowAndInteractNPC)}>: Wait for Dialog's window appears");
#endif
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
#if ShowDebugMsg
                            Astral.Logger.WriteLine($"<{nameof(FollowAndInteractNPC)}>: Processing Dialogs");
#endif
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
#if ShowDebugMsg

                    bool dist = false,
                        //interactInfoIsValid = false,
                        tarCanInteract = false;

                    if (target.IsValid)
                    {
                        dist = target.Location.Distance3DFromPlayer <= distance;
                        Astral.Logger.WriteLine($"<{nameof(FollowAndInteractNPC)}>: Check Distance({target.Location.Distance3DFromPlayer})<'{distance}': {dist}");
                        //interactInfoIsValid = EntityManager.LocalPlayer.Player.InteractInfo.IsValid;
                        //Astral.Logger.WriteLine($"<{nameof(FollowAndInteractNPC)}>: Check {nameof(EntityManager.LocalPlayer.Player.InteractInfo)}: {interactInfoIsValid}");
                        tarCanInteract = target.CanInteract;
                        Astral.Logger.WriteLine($"<{nameof(FollowAndInteractNPC)}>: Check {nameof(target)}.{nameof(target.CanInteract)}: {tarCanInteract}");

                        if (dist && tarCanInteract/* && !interactInfoIsValid*/)
                        {
                            Astral.Logger.WriteLine($"<{nameof(FollowAndInteractNPC)}>: Target cann't be interacted. Return 'false'");
                            return false;
                        }
                    }
                    else
                    {
                        Astral.Logger.WriteLine($"<{nameof(FollowAndInteractNPC)}>: Target is not valid. Return 'false'");
                        return false;
                    }

#else
                    if (!target.IsValid || (target.Location.Distance3DFromPlayer <= distance && !EntityManager.LocalPlayer.Player.InteractInfo.IsValid))
                    {
                        return false;
                    }
#endif
                }
                else
                {
#if ShowDebugMsg
                    Astral.Logger.WriteLine($"<{nameof(FollowAndSimulateFKey)}>: Approach to Target [{target.Pointer}] fail.");
#endif
                    break;
                }
            }
#if ShowDebugMsg
            Astral.Logger.WriteLine($"<{nameof(FollowAndSimulateFKey)}>: Return 'false'");
#endif
            return false;
        }
    }
}

