//#define ShowDebugMsg

using Astral;
using Astral.Logic;
using Astral.Logic.Classes.FSM;
using Astral.Logic.NW;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
//using static Astral.Logic.NW.Approach;
using static Astral.Quester.Classes.Action;

namespace EntityTools.Tools
{
    public static class EntityInteractionTools
    {
        //private static bool MainApproach(Func<Vector3> Position, Func<InteractOption> InteractOption, float distance, bool interactApproach, Func<Approach.BreakInfos> breakFunc, string targetName = "")
        //{
        //    if (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.IsValid)
        //    {
        //        EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Close();
        //    }
        //    if (!interactApproach)
        //    {
        //        if (distance < 1f)
        //        {
        //            distance = 1f;
        //        }
        //        if (Position().Distance3DFromPlayer < (double)distance)
        //        {
        //            return true;
        //        }
        //    }
        //    else if (InteractOption().IsValid)
        //    {
        //        if (InteractOption().hNode != IntPtr.Zero && InteractOption().CanInteract())
        //        {
        //            return true;
        //        }
        //        if (InteractOption().EntityRefId > 0u)
        //        {
        //            return true;
        //        }
        //    }
        //    Graph meshes = Approach.UsedMeshes;
        //    Vector3 vector = Position().Clone();
        //    double distance3DFromPlayer = vector.Distance3DFromPlayer;
        //    Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(5000);
        //    Func<Road> func = delegate ()
        //    {
        //        Road road = new Road();
        //        if (Astral.API.CurrentSettings.UsePathfinding3)
        //        {
        //            road = Navmesh.GenerateRoadFromPlayer(meshes, Position(), true);
        //        }
        //        else if (Navmesh.GetNearestNodePosFromPosition(meshes, Position()).Distance3D(Position()) < Position().Distance3DFromPlayer)
        //        {
        //            road = Navmesh.GenerateRoadFromPlayer(meshes, Position(), true);
        //        }
        //        else
        //        {
        //            road.Waypoints.Add(Position());
        //        }
        //        return road;
        //    };
        //    if (targetName.Length > 0)
        //    {
        //        Logger.WriteLine("Approach " + targetName + " ...");
        //    }

        //    Astral.Quester.API.Engine.Navigation.Reset();
        //    Astral.Quester.API.Engine.Navigation.road = func();
        //    Astral.Quester.API.Engine.Navigation.Start();
        //    if (distance < 3f)
        //    {
        //        distance = 3f;
        //    }
        //    Astral.Classes.Timeout timeout2 = new Astral.Classes.Timeout(3000);
        //    bool result = false;
        //    for (; ; )
        //    {
        //        Vector3 vector2 = Position();
        //        if (!vector2.IsValid)
        //        {
        //            goto IL_416;
        //        }
        //        if (interactApproach && InteractOption().IsValid)
        //        {
        //            if (InteractOption().IsValid && InteractOption().EntityRefId > 0u)
        //            {
        //                goto IL_2E7;
        //            }
        //            if (InteractOption().IsValid && InteractOption().hNode != IntPtr.Zero)
        //            {
        //                if (!InteractOption().CanInteract())
        //                {
        //                    goto IL_2F7;
        //                }
        //                Astral.Quester.API.Engine.Navigation.Stop();
        //                Thread.Sleep(1000);
        //                if (InteractOption().CanInteract())
        //                {
        //                    goto IL_2EF;
        //                }
        //            }
        //        }
        //        Astral.Quester.API.Engine.Navigation.Start();
        //        if (!interactApproach && vector2.Distance3DFromPlayer < (double)distance)
        //        {
        //            goto IL_3D6;
        //        }
        //        if (breakFunc != null)
        //        {
        //            Approach.BreakInfos breakInfos = breakFunc();
        //            if (breakInfos == Approach.BreakInfos.ApproachFail)
        //            {
        //                goto IL_416;
        //            }
        //            if (breakInfos == Approach.BreakInfos.ApproachOK)
        //            {
        //                break;
        //            }
        //        }
        //        if (EntityManager.LocalPlayer.IsDead || EntityManager.LocalPlayer.Character.IsNearDeath)
        //        {
        //            goto IL_416;
        //        }
        //        if (timeout2.IsTimedOut || vector2.Distance3D(vector) > 2.0)
        //        {
        //            vector = vector2.Clone();
        //            Astral.Quester.API.Engine.Navigation.Reset();
        //            Astral.Quester.API.Engine.Navigation.road = func();
        //            timeout2.Reset();
        //        }
        //        if (vector2.Distance3DFromPlayer < distance3DFromPlayer)
        //        {
        //            timeout.Reset();
        //            distance3DFromPlayer = vector2.Distance3DFromPlayer;
        //        }
        //        if (timeout.IsTimedOut)
        //        {
        //            goto IL_3E0;
        //        }
        //        Thread.Sleep(50);
        //    }
        //    result = true;
        //    goto IL_416;
        //    IL_2E7:
        //    result = true;
        //    goto IL_416;
        //    IL_2EF:
        //    result = true;
        //    goto IL_416;
        //    IL_2F7:
        //    int num = 0;
        //    do
        //    {
        //        num++;
        //        if (num >= 5)
        //        {
        //            goto IL_416;
        //        }
        //        if (num > 1)
        //        {
        //            Astral.Quester.API.Engine.Navigation.Stop();
        //        }
        //        switch (num)
        //        {
        //            case 1:
        //                {
        //                    Astral.Classes.Timeout timeout3 = new Astral.Classes.Timeout(2000);
        //                    while (!InteractOption().CanInteract())
        //                    {
        //                        if (timeout3.IsTimedOut)
        //                        {
        //                            break;
        //                        }
        //                        Thread.Sleep(10);
        //                    }
        //                    break;
        //                }
        //            case 2:
        //                MyNW.Internals.Movements.SetMovementType(MyNW.Internals.Movements.MovementsType.StrafeRight);
        //                Thread.Sleep(200);
        //                MyNW.Internals.Movements.UnsetMovementType(MyNW.Internals.Movements.MovementsType.StrafeRight);
        //                break;
        //            case 3:
        //                MyNW.Internals.Movements.SetMovementType(MyNW.Internals.Movements.MovementsType.StrafeLeft);
        //                Thread.Sleep(400);
        //                MyNW.Internals.Movements.UnsetMovementType(MyNW.Internals.Movements.MovementsType.StrafeLeft);
        //                break;
        //            case 4:
        //                Position().FaceYaw();
        //                MyNW.Internals.Movements.SetMovementType(MyNW.Internals.Movements.MovementsType.MoveForward);
        //                Thread.Sleep(800);
        //                MyNW.Internals.Movements.UnsetMovementType(MyNW.Internals.Movements.MovementsType.MoveForward);
        //                break;
        //        }
        //    }
        //    while (!InteractOption().CanInteract());
        //    result = true;
        //    goto IL_416;
        //    IL_3D6:
        //    result = true;
        //    goto IL_416;
        //    IL_3E0:
        //    string text = "Timeout while approaching ";
        //    if (targetName.Length > 0)
        //    {
        //        text = text + targetName + " ";
        //    }
        //    text += "...";
        //    Logger.WriteLine(text);
        //    IL_416:
        //    Astral.Quester.API.Engine.Navigation.Stop();
        //    Navigator.StopNavigator();            
        //    Astral.Quester.API.Engine.Navigation.Reset();
        //    return result;
        //}

        private static Approach.BreakInfos CheckCombat()
        {
            if (Attackers.InCombat)
            {
                return Approach.BreakInfos.ApproachFail;
            }
            return Approach.BreakInfos.Continue;
        }

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
            if (target != null && target.IsValid /*&& Approach.EntityForInteraction(target, new Func<Approach.BreakInfos>(CheckCombat))*/)
            {
                target.Interact();
                Thread.Sleep(interactTime);
                Interact.WaitForInteraction();
                if (dialogs != null && dialogs.Count > 0)
                {
                    Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(5000);
                    while (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Options.Count == 0)
                    {
                        if (timeout.IsTimedOut)
                        {
                            return false;
                        }
                        Thread.Sleep(100);
                    }
                    Thread.Sleep(500);
                    using (List<string>.Enumerator enumerator = dialogs.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            string key = enumerator.Current;
                            EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.SelectOptionByKey(key);
                            Thread.Sleep(1000);
                        }
                    }
                }
                EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Close();
                target = new Entity(IntPtr.Zero);
                return false;
            }
            return true;
        }
        #region InteractNPC_2
        //public static bool InteractNPC(Entity target, int interactTime = 1000, List<string> dialogs = null)
        //{
        //    if (target != null && target.IsValid)
        //    {
        //        bool result = false;
        //        if (target.Critter.IsInteractable && target.InteractOption.IsValid/* && Approach.EntityByDistance(target, target.Critter.InteractDistance, null)/*Approach.EntityForInteraction(target, null)*/)
        //        {
        //            MyNW.Internals.Movements.StopNavTo();

        //            Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(interactTime + 2000);

        //            while (!timeout.IsTimedOut)
        //            {
        //                target.Location.Face();
        //                target.Location.FaceYaw();

        //                Thread.Sleep(500);
        //                target.Interact();
        //                Thread.Sleep(500);//Thread.Sleep(interactTime);
        //                Interact.WaitForInteraction();
        //                if (dialogs != null && dialogs.Count > 0)
        //                {
        //                    if (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Options.Count > 0)
        //                    {
        //                        using (List<string>.Enumerator enumerator = dialogs.GetEnumerator())
        //                        {
        //                            while (enumerator.MoveNext())
        //                            {
        //                                string key = enumerator.Current;
        //                                EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.SelectOptionByKey(key);
        //                                Thread.Sleep(1000);
        //                            }
        //                        }
        //                    }
        //                }

        //                Approach.EntityForInteraction(target, null);
        //            }

        //            //EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Close();
        //            result = target.Critter.IsInteractable && target.InteractOption.IsValid && target.InteractOption.CanInteract();
        //        }
        //        return result;
        //    }
        //    return false;
        //}
        #endregion
        #region InteractNPC_1
        //public static bool InteractNPC(Entity target, int interactTime = 1000, List<string> dialogs = null)
        //{
        //    if (target != null && target.IsValid)
        //    {
        //        bool result = false;
        //        if (target.Critter.IsInteractable && target.InteractOption.IsValid/* && Approach.EntityByDistance(target, target.Critter.InteractDistance, null)/*Approach.EntityForInteraction(target, null)*/)
        //        {
        //            MyNW.Internals.Movements.StopNavTo();

        //            target.Location.Face();
        //            target.Location.FaceYaw();

        //            Thread.Sleep(500);
        //            target.Interact();
        //            Thread.Sleep(interactTime);
        //            Interact.WaitForInteraction();
        //            if (dialogs != null && dialogs.Count > 0)
        //            {
        //                Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(interactTime);
        //                while (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Options.Count == 0)
        //                {
        //                    if (timeout.IsTimedOut)
        //                    {
        //                        result = target.Critter.IsInteractable && target.InteractOption.IsValid && target.InteractOption.CanInteract();
        //                        break;
        //                    }
        //                    Thread.Sleep(100);
        //                }
        //                Thread.Sleep(500);
        //                using (List<string>.Enumerator enumerator = dialogs.GetEnumerator())
        //                {
        //                    while (enumerator.MoveNext())
        //                    {
        //                        string key = enumerator.Current;
        //                        EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.SelectOptionByKey(key, "");
        //                        Thread.Sleep(1000);
        //                    }
        //                }
        //            }
        //            EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Close();                    
        //            result = target.Critter.IsInteractable && target.InteractOption.IsValid && target.InteractOption.CanInteract();
        //        }
        //        return result;
        //    }
        //    return false;
        //}
        #endregion

        /// <summary>
        /// Взаимодействие с Entity 
        /// </summary>
        /// <param name="target">цель взаимодействия</param>
        /// <param name="interactTime">время взаимодействия</param>
        /// <param name="distance">дистанция до Entity на которой необходимо выполнять взаимодействие</param>
        /// <param name="dialogs">пункты диалога</param>
        /// <returns>Повторное взаимодействие c target возможно</returns>
        public static bool FollowAndInteractNPC(Entity target, int interactTime = 1000, float distance = 5, List<string> dialogs = null, Func<Approach.BreakInfos> breakFunc = null)
        {
            //Astral.Classes.Timeout interactTimeout = new Astral.Classes.Timeout(interactTime);
            while (target != null && target.IsValid/* && !interactTimeout.IsTimedOut*//* && target.InteractOption.IsValid*/)
            {
                if (Approach.EntityByDistance(target, distance, breakFunc))
                {

                    target.Location.Face();
                    target.Location.FaceYaw();

                    MyNW.Internals.Movements.StopNavTo();
                    //Thread.Sleep(500);
                    target.Interact();
                    Thread.Sleep(1000);//Thread.Sleep(interactTime);
                    Interact.WaitForInteraction();
                    if (dialogs != null && dialogs.Count > 0)
                    {
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

                    if (!target.IsValid
                        || (target.Location.Distance3DFromPlayer <= distance
                        && !(target.Critter.IsInteractable && target.InteractOption.IsValid && target.InteractOption.CanInteract())))
                    {
                        return false;
                    }
                }
                else
                {
                    break;
                }
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
        //        EntityManager.MainEngine.Navigation.Stop();
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
        //            Astral.Classes.Timeout SearchTimeout = new Astral.Classes.Timeout(5000);
        //            while (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Options.Count == 0)
        //            {
        //                if (SearchTimeout.IsTimedOut)
        //                {
        //                    return Action.ActionResult.Fail;
        //                }
        //                Thread.Sleep(100);
        //            }
        //            foreach (string key in this.Dialogs)
        //            {
        //                EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.SelectOptionByKey(key, this.RewardItemChoose);
        //                Thread.Sleep(1500);
        //            }
        //            EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Close();
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
        //        if (EntityManager.CurrentSettings.UsePathfinding3 && EntityManager.LocalPlayer.CurrentZoneMapInfo.MapType == ZoneMapType.Mission)
        //        {
        //            Thread.Sleep(2500);
        //            if (EntityManager.LocalPlayer.IsValid && !EntityManager.LocalPlayer.IsLoading)
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
        public static bool FollowAndSimulateFKey(Entity target, int interactTime = 1000, float distance = 5, List<string> dialogs = null, Func<Approach.BreakInfos> breakFunc = null)
        {
            while (target != null && target.IsValid)
            {
                if (Approach.EntityByDistance(target, distance, breakFunc))
                {
                    target.Location.Face();
                    target.Location.FaceYaw();
                    GameCommands.SimulateFKey();
                    Thread.Sleep(interactTime);

                    //Approach.EntityByDistance(target, distance, breakFunc);
                    //GameCommands.SimulateFKey();
                    //Thread.Sleep(interactTime);

                    if (EntityManager.LocalPlayer.Player.InteractInfo.IsValid)
                    {

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
                    if (!target.IsValid || (target.Location.Distance3DFromPlayer <= distance && !EntityManager.LocalPlayer.Player.InteractInfo.IsValid))
                        return false;
                }
                else break;
            }
            return false;
        }
    }
}

