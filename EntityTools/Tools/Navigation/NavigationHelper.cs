using System;
using System.Linq;
using System.Threading;
using ACTP0Tools;
using AStar;
using MyNW.Classes;
using MyNW.Internals;
using Timeout = Astral.Classes.Timeout;

namespace EntityTools.Tools.Navigation
{
    public static class NavigationHelper
    {
        /// <summary>
        /// Вычисление косинуса угла образованного тремя точками с вершиной в точке <param name="angularPoint"/>
        /// cos(15)  =  0,9659258262890682867497431997289
        /// cos(30)  =  0,86602540378443864676372317075294
        /// cos(45)  =  0,70710678118654752440084436210485
        /// cos(60)  =  0,5
        /// cos(75)  =  0,25881904510252076234889883762405
        /// cos(90)  =  0,0
        /// cos(105) = -0,25881904510252076234889883762405
        /// cos(165) = -0,9659258262890682867497431997289
        /// </summary>
        /// <param name="squareDistance0">Квадрат расстояния между точками <param name="angularPoint"/> и <param name="point0"/></param>
        /// <param name="squareDistance1">Квадрат расстояния между точками <param name="angularPoint"/> и <param name="point1"/></param>
        public static double Cosine(Vector3 angularPoint, Vector3 point0, Vector3 point1, out float squareDistance0, out float squareDistance1)
        {
            float x0 = point0.X - angularPoint.X,
                  y0 = point0.Y - angularPoint.Y,
                  z0 = point0.Z - angularPoint.Z,
                  x1 = point1.X - angularPoint.X,
                  y1 = point1.Y - angularPoint.Y,
                  z1 = point1.Z - angularPoint.Z;

            squareDistance0 = x0 * x0 + y0 * y0 + z0 * z0;
            squareDistance1 = x1 * x1 + y1 * y1 + z1 * z1;

            return (x0 * x1 + y0 * y1 + z0 * z1) / Math.Sqrt(squareDistance0 * squareDistance1);
        }

        /// <summary>
        /// Вычисление косинуса угла образованного тремя точками с вершиной в точке <param name="angularPoint"/>
        /// cos(15)  =  0,9659258262890682867497431997289
        /// cos(30)  =  0,86602540378443864676372317075294
        /// cos(45)  =  0,70710678118654752440084436210485
        /// cos(60)  =  0,5
        /// cos(75)  =  0,25881904510252076234889883762405
        /// cos(90)  =  0,0
        /// cos(105) = -0,25881904510252076234889883762405
        /// cos(165) = -0,9659258262890682867497431997289
        /// </summary>
        /// <param name="squareDistance0">Квадрат расстояния между точками <param name="angularPoint"/> и <param name="point0"/></param>
        /// <param name="squareDistance1">Квадрат расстояния между точками <param name="angularPoint"/> и <param name="point1"/></param>
        public static double Cosine(Point3D angularPoint, Point3D point0, Point3D point1, out double squareDistance0, out double squareDistance1)
        {
            double x0 = point0.X - angularPoint.X,
                y0 = point0.Y - angularPoint.Y,
                z0 = point0.Z - angularPoint.Z,
                x1 = point1.X - angularPoint.X,
                y1 = point1.Y - angularPoint.Y,
                z1 = point1.Z - angularPoint.Z;

            squareDistance0 = x0 * x0 + y0 * y0 + z0 * z0;
            squareDistance1 = x1 * x1 + y1 * y1 + z1 * z1;

            return (x0 * x1 + y0 * y1 + z0 * z1) / Math.Sqrt(squareDistance0 * squareDistance1);
        }

        /// <summary>
        /// Вычисление косинуса угла образованного тремя точками с вершиной в точке <param name="angularPointX"/>, <param name="angularPointY"/>, <param name="angularPointZ"/>)
        /// cos(15)  =  0,9659258262890682867497431997289
        /// cos(30)  =  0,86602540378443864676372317075294
        /// cos(45)  =  0,70710678118654752440084436210485
        /// cos(60)  =  0,5
        /// cos(75)  =  0,25881904510252076234889883762405
        /// cos(90)  =  0,0
        /// cos(105) = -0,25881904510252076234889883762405
        /// cos(165) = -0,9659258262890682867497431997289
        /// </summary>
        /// <param name="squareDistance0">Квадрат расстояния между точками с координатами (<param name="angularPointX"/>, <param name="angularPointY"/>, <param name="angularPointZ"/>) и (<param name="x0"/>, <param name="y0"/>, <param name="z0"/>)</param>
        /// <param name="squareDistance1">Квадрат расстояния между точками с координатами (<param name="angularPointX"/>, <param name="angularPointY"/>, <param name="angularPointZ"/>) и (<param name="x1"/>, <param name="y1"/>, <param name="z1"/>)</param>
        public static double Cosine(double angularPointX, double angularPointY, double angularPointZ,
                                    double x0, double y0, double z0,
                                    double x1, double y1, double z1,
                                    out double squareDistance0, out double squareDistance1)
        {
            double dx0 = x0 - angularPointX,
                   dy0 = y0 - angularPointY,
                   dz0 = z0 - angularPointZ,
                   dx1 = x1 - angularPointX,
                   dy1 = y1 - angularPointY,
                   dz1 = z1 - angularPointZ;

            squareDistance0 = dx0 * dx0 + dy0 * dy0 + dz0 * dz0;
            squareDistance1 = dx1 * dx1 + dy1 * dy1 + dz1 * dz1;

            return (dx0 * dx1 + dy0 * dy1 + dz0 * dz1) / Math.Sqrt(squareDistance0 * squareDistance1);
        }

        /// <summary>
        /// Вычисление квадрата расстояния между точкой с координатами {<param name="x0"/>, <param name="y0"/>, <param name="z0"/>} и точкой с координатами {<param name="x1"/>, <param name="y1"/>, <param name="z1"/>}
        /// </summary>
        public static double SquareDistance3D(double x0, double y0, double z0, double x1, double y1, double z1)
        {
            double dx = x0 - x1;
            double dy = y0 - y1;
            double dz = z0 - z1;

            return dx * dx + dy * dy + dz * dz;
        }

        /// <summary>
        /// Вычисление квадрата расстояния между точками <param name="point0"/> и <param name="point1"/>
        /// </summary>

        public static double SquareDistance3D(Vector3 point0, Vector3 point1)
        {
            return SquareDistance3D(point0.X, point0.Y, point0.Z, point1.X, point1.Y, point1.Z);
        }

#if disabled_2021_04_19
        public static int REGENERATE_PATH_TIME = 3000;
        public static int TARGET_DISTANCE_MEASURE_TIME = 500;

        /// <summary>
        /// Перемещение к <paramref name="entity"/> на расстояние <paramref name="distance"/>
        /// </summary>
        public static bool Approach(this Entity entity, double distance = 7f, int timeout = 5000)
        {
            if (entity is null || !entity.IsValid)
                return false;

            var location = entity.Location;

            if (location.Distance3DFromPlayer < distance)
                return true;

            var task = ApproachAsync(location, distance, timeout);
            var timer = new Timeout(timeout);
            while (timer.IsTimedOut)
            {
                if (task.IsCompleted)
                {
                    if (task.Status == TaskStatus.RanToCompletion)
                        return task.Result;
                    return false;
                }
                Thread.Sleep(50);
            }

            return false;
        }
        /// <summary>
        /// Перемещение к <paramref name="entity"/> на расстояние <paramref name="distance"/>
        /// </summary>
        public static bool Approach(this Vector3 position, double distance = 7f, int timeout = 5000)
        {
            if (position is null || !position.IsValid)
                return false;

            if (position.Distance3DFromPlayer < distance)
                return true;

            var task = ApproachAsync(position, distance, timeout);
            var timer = new Timeout(timeout);
            while (timer.IsTimedOut)
            {
                if (task.IsCompleted)
                {
                    if (task.Status == TaskStatus.RanToCompletion)
                        return task.Result;
                    return false;
                }
                Thread.Sleep(50);
            }

            return false;
        }
        /// <summary>
        /// Перемещение к <paramref name="entity"/> на расстояние <paramref name="distance"/>
        /// </summary>
        public static async Task<bool> ApproachAsync(this Entity entity, double distance = 7f, int timeout = 0)
        {
            if (!entity.IsValid)
                return false;

            var location = entity.Location;

            if (location.Distance3DFromPlayer < distance)
                return true;

            return await ApproachAsync(location, distance, timeout);
        }

        /// <summary>
        /// Перемещение к <paramref name="targetPosition"/> на расстояние <paramref name="targetDistance"/>
        /// </summary>
        public static async Task<bool> ApproachAsync(Vector3 targetPosition, double targetDistance = 7f, int timeout = 0, Func<bool> breakingFunction = null)
        {
            if (!targetPosition.IsValid)
                return false;

            if (targetDistance <= 3)
                targetDistance = 3f;


            bool result = false;
            var player = EntityManager.LocalPlayer;

            var currentPosition = player.Location.Clone();
            var startDistance = currentPosition.Distance3D(targetPosition);

            var regeneratePathTimer = new Timeout(REGENERATE_PATH_TIME);
            var targetDistanceMeasureTimer = new Timeout(TARGET_DISTANCE_MEASURE_TIME);

#if false
            var mainEngineNavigation = AstralAccessors.Controllers.Engine.MainEngine.Navigation; 
            if (mainEngineNavigation is null)
                return  false;
#else
            var customNavigator = new CustomNavigator();
#endif
#if true
            //var timer = new Timeout(timeout > 0 ? timeout : int.MaxValue);

            Func<bool> continueFunction = null;
            if (timeout > 0)
            {
                var timer = new Timeout(timeout);
                if (breakingFunction is null)
                    continueFunction = () => !timer.IsTimedOut;
                else continueFunction = () => !(timer.IsTimedOut || breakingFunction());
            }
            else if (breakingFunction is null)
                continueFunction = () => true;
            else continueFunction = () => !breakingFunction();

            while (continueFunction())
#else
            while(true)
#endif
            {
                currentPosition = player.Location;
                var currentDistance = currentPosition.Distance3D(targetPosition);
                if (currentDistance <= targetDistance)
                {
                    result = true;
                    break;
                }

                if (player.IsDead || player.Character.IsNearDeath)
                    break;

                if (targetDistanceMeasureTimer.IsTimedOut)
                {
                    if (Math.Abs(currentDistance - startDistance) < 2)
                        // Персонаж застрял
                        break;
                    startDistance = currentDistance;
                    targetDistanceMeasureTimer.Reset();
                }
                else
                {
                    int ind = customNavigator.CurrentWaypointIndex;
                    var waypoints = customNavigator.Road.Waypoints;
                    var wp0 = waypoints[ind];
                    if (++ind < waypoints.Count)
                    {
                        var wp1 = waypoints[ind];
                        if (Cos(currentPosition, wp0, wp1, out double cos, out double sqrtDist0, out double sqrtDis1) < 0
                            || sqrtDis1 < sqrtDist0)
                        {
                            // 
                            customNavigator.SetNextWaypoint();
                            startDistance = currentDistance;
                            targetDistanceMeasureTimer.Reset();
                        }
                    }
                    else break;
                }

                if (regeneratePathTimer.IsTimedOut)
                {
                    var graph = AstralAccessors.Controllers.Roles.CurrentRole.UsedMeshes;
                    if (graph is null)
                        break;
#if true
                    var pathRoad = await Task.Run(() => Navmesh.GenerateRoad(graph, currentPosition, targetPosition, false));
#else
                    var pathRoad = Patches.Navmesh.Patch_Astral_Logic_Navmesh_GenerateRoad.GenerateRoad(graph, currentPosition, targetPosition, false);
#endif
#if false
                    mainEngineNavigation.Stop(); 
#endif
                    customNavigator.Reset();
                    customNavigator.Road = pathRoad;
                    customNavigator.Start();
                }


#if true
                Thread.Sleep(50);
#else
                await Task.Delay(50);
#endif
            }

            customNavigator.Stop();
#if false
            Navigator.StopNavigator();
            customNavigator.Reset(); 
#endif
            return result;
        }

        /// <summary>
        /// Повторение функционала MainApproach без "взаимодействия" в целью
        /// </summary>
        public static async Task<bool> SimpleApproachAsync(Vector3 targetPosition, float targetDistance = 7f, int time = 5000)
        {
            if (!targetPosition.IsValid)
                return false;

            if (targetDistance <= 3)
                targetDistance = 3f;


            bool result = false;
            var player = EntityManager.LocalPlayer;

            var currentPosition = player.Location.Clone();
            var startDistance = currentPosition.Distance3D(targetPosition);

            var regeneratePathTimer = new Timeout(REGENERATE_PATH_TIME);
            var targetDistanceMeasureTimer = new Timeout(TARGET_DISTANCE_MEASURE_TIME);

#if false
            var mainEngineNavigation = AstralAccessors.Controllers.Engine.MainEngine.Navigation; 
            if (mainEngineNavigation is null)
                return  false;
#else
            var mainEngineNavigation = new CustomNavigator();
#endif

            var timer = new Timeout(time);

            while (!timer.IsTimedOut)
            {
                currentPosition = player.Location;
                var currentDistance = currentPosition.Distance3D(targetPosition);
                if (currentDistance <= targetDistance)
                {
                    result = true;
                    break;
                }

                if (player.IsDead || player.Character.IsNearDeath)
                    break;

                if (targetDistanceMeasureTimer.IsTimedOut)
                {
                    if (Math.Abs(currentDistance - startDistance) < 2)
                        // Персонаж застрял
                        break;
                    startDistance = currentDistance;
                    targetDistanceMeasureTimer.Reset();
                }
                else
                {
                    int ind = mainEngineNavigation.CurrentWaypointIndex;
                    var waypoints = mainEngineNavigation.Road.Waypoints;
                    var wp0 = waypoints[ind];
                    if (++ind < waypoints.Count)
                    {
                        var wp1 = waypoints[ind];
                        if (Cos(currentPosition, wp0, wp1, out double cos, out double sqrtDist0, out double sqrtDis1) < 0
                            || sqrtDis1 < sqrtDist0)
                        {
                            // 
                            mainEngineNavigation.SetNextWaypoint();
                            startDistance = currentDistance;
                            targetDistanceMeasureTimer.Reset();
                        }
                    }
                    else break;
                }

                if (regeneratePathTimer.IsTimedOut)
                {
                    var graph = AstralAccessors.Controllers.Roles.CurrentRole.UsedMeshes;
                    if (graph is null)
                        break;
#if true
                    var pathRoad = await Task.Run(() => Navmesh.GenerateRoad(graph, currentPosition, targetPosition, false));
#else
                    var pathRoad = Patches.Navmesh.Patch_Astral_Logic_Navmesh_GenerateRoad.GenerateRoad(graph, currentPosition, targetPosition, false);
#endif
#if false
                    mainEngineNavigation.Stop(); 
#endif
                    mainEngineNavigation.Reset();
                    mainEngineNavigation.Road = pathRoad;
                    mainEngineNavigation.Start();
                }


#if true
                Thread.Sleep(50);
#else
                await Task.Delay(50);
#endif
            }

            mainEngineNavigation.Stop();

            return result;
        }
#if false   // public static bool MainApproach(Vector3 targetPosition, Func<InteractOption> InteractOption, float distance, bool interactApproach, Func<ApproachAsync.BreakInfos> breakFunc, string targetName = "", Func<float> specialDist = null)
        public static bool MainApproach(Vector3 targetPosition, Func<InteractOption> InteractOption, float distance, bool interactApproach, Func<ApproachAsync.BreakInfos> breakFunc, string targetName = "", Func<float> specialDist = null)
        {
            if (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.IsValid)
            {
                EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Close();
            }
            Func<float> currentDist = delegate ()
            {
                if (specialDist != null)
                {
                    return specialDist();
                }
                return (float)targetPosition.Distance3DFromPlayer;
            };
            if (!interactApproach)
            {
                if (distance < 1f)
                {
                    distance = 1f;
                }
                if (currentDist() < distance)
                {
                    return true;
                }
            }
            else if (InteractOption().IsValid)
            {
                if (InteractOption().hNode != IntPtr.Zero && InteractOption().CanInteract())
                {
                    return true;
                }
                if (InteractOption().EntityRefId > 0U)
                {
                    return true;
                }
            }
            Graph meshes = AstralAccessors.Controllers.Roles.CurrentRole.UsedMeshes;
            Vector3 from = targetPosition.Clone();
            double num = (double)currentDist();
            Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(5000);
            Func<Road> func = delegate ()
            {
                Road road = new Road();
                if (EntityManager.CurrentSettings.UsePathfinding3)
                {
                    road = Navmesh.GenerateRoadFromPlayer(meshes, targetPosition(), true);
                }
                else if (Navmesh.GetNearestNodePosFromPosition(meshes, targetPosition()).Distance3D(targetPosition()) < (double)currentDist())
                {
                    road = Navmesh.GenerateRoadFromPlayer(meshes, targetPosition(), true);
                }
                else
                {
                    road.Waypoints.Add(targetPosition());
                }
                return road;
            };
            if (targetName.Length > 0)
            {
                Logger.WriteLine("ApproachAsync " + targetName + " ...");
            }
            AstralAccessors.Controllers.Engine.MainEngine.Navigation.Reset();
            AstralAccessors.Controllers.Engine.MainEngine.Navigation.road = func();
            AstralAccessors.Controllers.Engine.MainEngine.Navigation.Start();
            if (distance < 3f)
            {
                distance = 3f;
            }
            Astral.Classes.Timeout timeout2 = new Astral.Classes.Timeout(3000);
            bool result = false;
            for (; ; )
            {
                Vector3 vector = targetPosition.Clone();
                if (!vector.IsValid)
                {
                    goto IL_45A;
                }
                if (interactApproach && InteractOption().IsValid)
                {
                    if (InteractOption().IsValid && InteractOption().EntityRefId > 0U)
                    {
                        goto IL_30E;
                    }
                    if (InteractOption().IsValid && InteractOption().hNode != IntPtr.Zero)
                    {
                        if (!InteractOption().CanInteract())
                        {
                            goto IL_31E;
                        }
                        AstralAccessors.Controllers.Engine.MainEngine.Navigation.Stop();
                        Thread.Sleep(1000);
                        if (InteractOption().CanInteract())
                        {
                            goto IL_316;
                        }
                    }
                }
                AstralAccessors.Controllers.Engine.MainEngine.Navigation.Start();
                if (!interactApproach && currentDist() < distance)
                {
                    goto IL_41A;
                }
                if (breakFunc != null)
                {
                    ApproachAsync.BreakInfos breakInfos = breakFunc();
                    if (breakInfos == ApproachAsync.BreakInfos.ApproachFail)
                    {
                        goto IL_45A;
                    }
                    if (breakInfos == ApproachAsync.BreakInfos.ApproachOK)
                    {
                        break;
                    }
                }
                if (EntityManager.LocalPlayer.IsDead || EntityManager.LocalPlayer.Character.IsNearDeath)
                {
                    goto IL_45A;
                }
                if (timeout2.IsTimedOut || vector.Distance3D(from) > 2.0)
                {
                    from = vector.Clone();
                    AstralAccessors.Controllers.Engine.MainEngine.Navigation.Reset();
                    AstralAccessors.Controllers.Engine.MainEngine.Navigation.road = func();
                    timeout2.Reset();
                }
                if ((double)currentDist() < num)
                {
                    timeout.Reset();
                    num = (double)currentDist();
                }
                if (timeout.IsTimedOut)
                {
                    goto IL_424;
                }
                Thread.Sleep(50);
            }
            result = true;
            goto IL_45A;
            IL_30E:
            result = true;
            goto IL_45A;
            IL_316:
            result = true;
            goto IL_45A;
            IL_31E:
            int num2 = 0;
            do
            {
                num2++;
                if (num2 >= 6)
                {
                    goto IL_45A;
                }
                if (num2 > 1)
                {
                    AstralAccessors.Controllers.Engine.MainEngine.Navigation.Stop();
                }
                switch (num2)
                {
                    case 1:
                        {
                            Astral.Classes.Timeout timeout3 = new Astral.Classes.Timeout(2000);
                            while (!InteractOption().CanInteract())
                            {
                                if (timeout3.IsTimedOut)
                                {
                                    break;
                                }
                                Thread.Sleep(10);
                            }
                            break;
                        }
                    case 2:
                        GfxCameraController.SetPitch(-0.66f);
                        Thread.Sleep(300);
                        break;
                    case 3:
                        Movements.SetMovementType(Movements.MovementsType.StrafeRight);
                        Thread.Sleep(200);
                        Movements.UnsetMovementType(Movements.MovementsType.StrafeRight);
                        break;
                    case 4:
                        Movements.SetMovementType(Movements.MovementsType.StrafeLeft);
                        Thread.Sleep(400);
                        Movements.UnsetMovementType(Movements.MovementsType.StrafeLeft);
                        break;
                    case 5:
                        targetPosition().FaceYaw();
                        Movements.SetMovementType(Movements.MovementsType.MoveForward);
                        Thread.Sleep(800);
                        Movements.UnsetMovementType(Movements.MovementsType.MoveForward);
                        break;
                }
            }
            while (!InteractOption().CanInteract());
            result = true;
            goto IL_45A;
            IL_41A:
            result = true;
            goto IL_45A;
            IL_424:
            string text = "Timeout while approaching ";
            if (targetName.Length > 0)
            {
                text = text + targetName + " ";
            }
            text += "...";
            Logger.WriteLine(text);
            IL_45A:
            AstralAccessors.Controllers.Engine.MainEngine.Navigation.Stop();
            Navigator.StopNavigator();
            AstralAccessors.Controllers.Engine.MainEngine.Navigation.Reset();
            return result;
        } 
#endif

        /// <summary>
        /// Остановка штатной навигации Astral'a
        /// </summary>
        public static void AstralNavigationStop()
        {
            AstralAccessors.Controllers.Engine.MainEngine.Navigation.Stop();
            Astral.Logic.NW.Navigator.StopNavigator();
            Astral.Logic.NW.Movements.StopMovements();
        }

        private static double Cos(Vector3 originPosition, Vector3 wp0, Vector3 wp1, out double cos, out double sqrtDist0, out double sqrtDist1)
        {
            // cos(165) = -0.9659258262890682867497431997289
            // cos(150) = -0.86602540378443864676372317075294
            // cos(135) = -0.70710678118654752440084436210485
            // cos(105) = -0.25881904510252076234889883762405
            // cos(75) = 0.25881904510252076234889883762405
            // cos(15) = 0.9659258262890682867497431997289

            // вычисляем Cos угла между векторами из формулы скалярного произведения векторов
            // a * b = |a| * |b| * cos (alpha) = xa * xb + ya * yb

            double x0 = wp0.X - originPosition.X,
                   y0 = wp0.Y - originPosition.Y,
                   z0 = wp0.Z - originPosition.Z,
                   x1 = wp1.X - originPosition.X,
                   y1 = wp1.Y - originPosition.Y,
                   z1 = wp1.Z - originPosition.Z;

            sqrtDist0 = x0 * x0 + y0 * y0 + z0 * z0;
            sqrtDist1 = x1 * x1 + y1 * y1 + z1 * z1;
            cos = (x0 * x1 + y0 * y1 + z0 * z1)
                         / Math.Sqrt(sqrtDist0 * sqrtDist1);

            return cos;
        }

        private static double SquaredDistance3D(Vector3 wp1, Vector3 wp2)
        {
            double dx = wp1.X - wp2.X,
                dy = wp1.Y - wp2.Y,
                dz = wp1.Z - wp2.Z,
                sqrtDist = dx * dx + dy * dy + dz * dz;
            return sqrtDist;
        } 
#endif



        /// <summary>
        /// Взаимодействие с <paramref name="entity"/> предусматривающее, в случае необходимости ориентацию в направлении <paramref name="entity"/>
        /// </summary>
        public static bool SmartInteract(this Entity entity, float interactDistance = 5, int interactTime = 2000)
        {
            //TODO: Разворачивать камеру вместе с персонажем

            // Полная остановка навигации
            StopNavigationCompletely();
            
            uint refId = entity.RefId;
            interactTime = Math.Max(interactTime, 500);
            var player = EntityManager.LocalPlayer.Player;
            InteractOption interactOption;

            // Установка ориентации на entity
            entity.Location.FaceYaw();
            Thread.Sleep(500);


            // Попытка взаимодействия в текущем положении персонажа
#if true
            var interactionWaitTimer = new Timeout(interactTime);
            do
            {
                interactOption = player.InteractStatus.InteractOptions.FirstOrDefault(o => o.EntityRefId == refId);
                if (interactOption != null 
                    && interactOption.CanInteract()
                    && interactOption.Interact())
                {
                    interactionWaitTimer.Reset();
                    do
                    {
                        Thread.Sleep(250);
                        if (player.InteractInfo.ContactDialog.IsValid)
                            return true;
                    }
                    while (!interactionWaitTimer.IsTimedOut);
                    break;
                }
                Thread.Sleep(250);
            }
            while (!interactionWaitTimer.IsTimedOut);
#else
            Interact.ForContactDialog(entity);
#endif

            //TODO: Попробовать отключить, т.к. находясь вплотную к NPC персонаж часто не может с ним взаимодействовать (нужно покрутить камеру)
#if false
            if (entity.Location.Distance3DFromPlayer > interactDistance) 
#endif
            {
                // Изменение угла камеры к горизонту и попытка взаимодействия
                GfxCameraController.SetPitch(-0.66f);
                Thread.Sleep(300);
                interactOption = player.InteractStatus.InteractOptions.FirstOrDefault(o => o.EntityRefId == refId);
                if (interactOption != null && interactOption.CanInteract()//IsValid
                    && interactOption.Interact())
                {
                    interactionWaitTimer.Reset();
                    do
                    {
                        Thread.Sleep(250);
                        if (player.InteractInfo.ContactDialog.IsValid)
                            return true;
                    }
                    while (!interactionWaitTimer.IsTimedOut);
                }

                // Шаг вправо и попытка взаимодействия
                MyNW.Internals.Movements.SetMovementType(MyNW.Internals.Movements.MovementsType.StrafeRight);
                Thread.Sleep(200);
                MyNW.Internals.Movements.UnsetMovementType(MyNW.Internals.Movements.MovementsType.StrafeRight);
                interactOption = player.InteractStatus.InteractOptions.FirstOrDefault(o => o.EntityRefId == refId);
                if (interactOption != null && interactOption.CanInteract()//isValid
                    && interactOption.Interact())
                {
                    interactionWaitTimer.Reset();
                    do
                    {
                        Thread.Sleep(250);
                        if (player.InteractInfo.ContactDialog.IsValid)
                            return true;
                    }
                    while (!interactionWaitTimer.IsTimedOut);
                }

                // Два шага влево и попытка взаимодействия
                MyNW.Internals.Movements.SetMovementType(MyNW.Internals.Movements.MovementsType.StrafeLeft);
                Thread.Sleep(200);
                MyNW.Internals.Movements.UnsetMovementType(MyNW.Internals.Movements.MovementsType.StrafeLeft);
                interactOption = player.InteractStatus.InteractOptions.FirstOrDefault(o => o.EntityRefId == refId);
                if (interactOption != null && interactOption.CanInteract()//isValid
                    && interactOption.Interact())
                {
                    interactionWaitTimer.Reset();
                    do
                    {
                        Thread.Sleep(250);
                        if (player.InteractInfo.ContactDialog.IsValid)
                            return true;
                    }
                    while (!interactionWaitTimer.IsTimedOut);
                }

                // Ориентация на целб шаг назад и попытка взаимодействия
                Thread.Sleep(250);
                MyNW.Internals.Movements.SetMovementType(MyNW.Internals.Movements.MovementsType.MoveBackward);
                Thread.Sleep(200);
                entity.Location.FaceYaw();
                MyNW.Internals.Movements.UnsetMovementType(MyNW.Internals.Movements.MovementsType.MoveBackward);
                interactOption = player.InteractStatus.InteractOptions.FirstOrDefault(o => o.EntityRefId == refId);
                if (interactOption != null && interactOption.CanInteract()//IsValid
                    && interactOption.Interact())
                {
                    interactionWaitTimer.Reset();
                    do
                    {
                        Thread.Sleep(250);
                        if (player.InteractInfo.ContactDialog.IsValid)
                            return true;
                    }
                    while (!interactionWaitTimer.IsTimedOut);
                }

                // Два шага вперед и попытка взаимодействия
                MyNW.Internals.Movements.SetMovementType(MyNW.Internals.Movements.MovementsType.MoveForward);
                Thread.Sleep(400);
                MyNW.Internals.Movements.UnsetMovementType(MyNW.Internals.Movements.MovementsType.MoveForward);
                entity.Location.FaceYaw();
                interactOption = player.InteractStatus.InteractOptions.FirstOrDefault(o => o.EntityRefId == refId);
                if (interactOption != null && interactOption.CanInteract()//IsValid
                    && interactOption.Interact())
                    Thread.Sleep(interactTime); 
            }

            return player.InteractInfo.ContactDialog.IsValid;
        }

        /// <summary>
        /// Полная остановка навигации (перемещения)
        /// </summary>
        public static void StopNavigationCompletely()
        {
            // Остановка движка навигации "активной роли"
            var mainEngineNavigation = AstralAccessors.Controllers.Engine.MainEngine.Navigation;
            mainEngineNavigation.Stop();
            mainEngineNavigation.Reset();
            // Остановка общего движка перемещения
            Astral.Logic.NW.Navigator.StopNavigator();
            // Сброс всех направлений перемещения
#if false
            Astral.Logic.NW.Movements.StopMovements(); 
#else
            MyNW.Internals.Movements.UnsetMovementType(MyNW.Internals.Movements.MovementsType.MoveForward);
            MyNW.Internals.Movements.UnsetMovementType(MyNW.Internals.Movements.MovementsType.Jump);
            MyNW.Internals.Movements.UnsetMovementType(MyNW.Internals.Movements.MovementsType.MoveBackward);
            MyNW.Internals.Movements.UnsetMovementType(MyNW.Internals.Movements.MovementsType.StrafeLeft);
            MyNW.Internals.Movements.UnsetMovementType(MyNW.Internals.Movements.MovementsType.StrafeRight);
#endif

            MyNW.Internals.Movements.StopNavTo();
        }
    }
}
