using AcTp0Tools;
using AStar;
using Astral.Logic.Classes.FSM;
using Astral.Logic.NW;
using EntityTools.Tools.Navigation;
using HarmonyLib;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Reflection;

// ReSharper disable InconsistentNaming

namespace EntityTools.Patches
{
    //TODO Добавить проверку версии AStar
    internal static class ComplexPatch_Navigation
    { 
        public static bool PatchesWasApplied { get; private set; }

        private static MethodInfo original_Navmesh_GetPath;
        private static MethodInfo prefix_Navmesh_GetPath;

        private static MethodInfo original_Navmesh_GenerateRoad;
        private static MethodInfo prefix_Navmesh_GenerateRoad;

        private static MethodInfo original_Navmesh_GetNearestNodeFromPosition;
        private static MethodInfo prefix_Navmesh_GetNearestNodeFromPosition;

        private static MethodInfo original_Navmesh_FixPath;
        private static MethodInfo prefix_Navmesh_FixPath;

        private static MethodInfo original_Navmesh_TotalDistance;
        private static MethodInfo prefix_Navmesh_MethodInfo;

        private static MethodInfo original_Road_PathDistance;
        private static MethodInfo prefix_Road_PathDistance;

        private static MethodInfo original_Road_GenerateRoadFromPlayer;
        private static MethodInfo prefix_Road_GenerateRoadFromPlayer;

        internal static void Apply()
        {
            if (!EntityTools.Config.Patches.Navigation || PatchesWasApplied) return;

            var tNavmesh = typeof(Astral.Logic.Navmesh);
            var tRoad = typeof(Astral.Quester.Controllers.Road);
            var tPatch = typeof(ComplexPatch_Navigation);

            var Navmesh_GetPath = "getPath";
            original_Navmesh_GetPath = AccessTools.Method(tNavmesh, Navmesh_GetPath);
            if (original_Navmesh_GetPath is null)
            {
                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Navigation)}' failed. Method '{Navmesh_GetPath}' not found", true);
                return;
            }

            prefix_Navmesh_GetPath = AccessTools.Method(tPatch, nameof(PrefixGetPath));
            if (prefix_Navmesh_GetPath is null)
            {
                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Navigation)}' failed. Method '{nameof(PrefixGetPath)}' not found", true);
                return;
            }

            var Navmesh_GenerateRoad = "GenerateRoad";
            original_Navmesh_GenerateRoad = AccessTools.Method(tNavmesh, Navmesh_GenerateRoad);
            if (original_Navmesh_GenerateRoad is null)
            {
                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Navigation)}' failed. Method '{Navmesh_GenerateRoad}' not found", true);
                return;
            }
            prefix_Navmesh_GenerateRoad = AccessTools.Method(tPatch, nameof(PrefixGenerateRoad));
            if (prefix_Navmesh_GenerateRoad is null)
            {
                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Navigation)}' failed. Method '{nameof(PrefixGenerateRoad)}' not found", true);
                return;
            }

            var Navmesh_GetNearestNodeFromPosition = "GetNearestNodeFromPosition";
            original_Navmesh_GetNearestNodeFromPosition = AccessTools.Method(tNavmesh, "GetNearestNodeFromPosition");
            if (original_Navmesh_GetNearestNodeFromPosition is null)
            {
                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Navigation)}' failed. Method '{Navmesh_GetNearestNodeFromPosition}' not found", true);
                return;
            }
            prefix_Navmesh_GetNearestNodeFromPosition = AccessTools.Method(tPatch, nameof(PrefixGetNearestNodeFromPosition));
            if (prefix_Navmesh_GetNearestNodeFromPosition is null)
            {
                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Navigation)}' failed. Method '{nameof(PrefixGetNearestNodeFromPosition)}' not found", true);
                return;
            }

            var Navmesh_FixPath = "fixPath";
            original_Navmesh_FixPath = AccessTools.Method(tNavmesh, "fixPath");
            if (original_Navmesh_FixPath is null)
            {
                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Navigation)}' failed. Method '{Navmesh_FixPath}' not found", true);
                return;
            }
            prefix_Navmesh_FixPath = AccessTools.Method(tPatch, nameof(PrefixFixPath));
            if (prefix_Navmesh_FixPath is null)
            {
                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Navigation)}' failed. Method '{nameof(PrefixFixPath)}' not found", true);
                return;
            }

            var Navmesh_TotalDistance = "TotalDistance";
            original_Navmesh_TotalDistance = AccessTools.Method(tNavmesh, Navmesh_TotalDistance);
            if (original_Navmesh_TotalDistance is null)
            {
                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Navigation)}' failed. Method '{Navmesh_TotalDistance}' not found", true);
                return;
            }
            prefix_Navmesh_MethodInfo = AccessTools.Method(tPatch, nameof(PrefixTotalDistance));
            if (prefix_Navmesh_MethodInfo is null)
            {
                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Navigation)}' failed. Method '{nameof(PrefixTotalDistance)}' not found", true);
                return;
            }

            var Road_PathDistance = "PathDistance";
            original_Road_PathDistance = AccessTools.Method(tRoad, Road_PathDistance );
            if (original_Road_PathDistance is null)
            {
                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Navigation)}' failed. Method '{Road_PathDistance}' not found", true);
                return;
            }
            prefix_Road_PathDistance = AccessTools.Method(tPatch, nameof(PathDistance));
            if (prefix_Road_PathDistance is null)
            {
                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Navigation)}' failed. Method '{nameof(PathDistance)}' not found", true);
                return;
            }

            var Road_GenerateRoadFromPlayer  = "GenerateRoadFromPlayer";
            original_Road_GenerateRoadFromPlayer = AccessTools.Method(tRoad, Road_GenerateRoadFromPlayer);
            if (original_Road_GenerateRoadFromPlayer is null)
            {
                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Navigation)}' failed. Method '{Road_GenerateRoadFromPlayer}' not found", true);
                return;
            }
            prefix_Road_GenerateRoadFromPlayer = AccessTools.Method(tPatch, nameof(GenerateRoadFromPlayer));
            if (prefix_Road_GenerateRoadFromPlayer is null)
            {
                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Navigation)}' failed. Method '{nameof(GenerateRoadFromPlayer)}' not found", true);
                return;
            }

            Action unPatch = null;

            try
            {
                AcTp0Tools.Patches.AcTp0Patcher.Harmony.Patch(original_Navmesh_GetPath, new HarmonyMethod(prefix_Navmesh_GetPath));
                unPatch = () =>
                {
                    ETLogger.WriteLine(LogType.Debug, $@"Unpatch method '{original_Navmesh_GetPath}'.", true);
                    AcTp0Tools.Patches.AcTp0Patcher.Harmony.Unpatch(original_Navmesh_GetPath,
                            prefix_Navmesh_GetPath);
                };

                AcTp0Tools.Patches.AcTp0Patcher.Harmony.Patch(original_Navmesh_GenerateRoad, new HarmonyMethod(prefix_Navmesh_GenerateRoad));
                unPatch += () =>
                {
                    ETLogger.WriteLine(LogType.Debug, $@"Unpatch method '{original_Navmesh_GenerateRoad}'.", true);
                    AcTp0Tools.Patches.AcTp0Patcher.Harmony.Unpatch(original_Navmesh_GenerateRoad,
                            prefix_Navmesh_GenerateRoad);
                };

                AcTp0Tools.Patches.AcTp0Patcher.Harmony.Patch(original_Navmesh_GetNearestNodeFromPosition, new HarmonyMethod(prefix_Navmesh_GetNearestNodeFromPosition));
                unPatch += () =>
                {
                    ETLogger.WriteLine(LogType.Debug, $@"Unpatch method '{original_Navmesh_GetNearestNodeFromPosition}'.", true);
                    AcTp0Tools.Patches.AcTp0Patcher.Harmony.Unpatch(original_Navmesh_GetNearestNodeFromPosition,
                            prefix_Navmesh_GetNearestNodeFromPosition);
                };

                AcTp0Tools.Patches.AcTp0Patcher.Harmony.Patch(original_Navmesh_FixPath, new HarmonyMethod(prefix_Navmesh_FixPath));
                unPatch += () =>
                {
                    ETLogger.WriteLine(LogType.Debug, $@"Unpatch method '{original_Navmesh_FixPath}'.", true);
                    AcTp0Tools.Patches.AcTp0Patcher.Harmony.Unpatch(original_Navmesh_FixPath,
                            prefix_Navmesh_FixPath);
                };

                AcTp0Tools.Patches.AcTp0Patcher.Harmony.Patch(original_Navmesh_TotalDistance, new HarmonyMethod(prefix_Navmesh_MethodInfo));
                unPatch += () =>
                {
                    ETLogger.WriteLine(LogType.Debug, $@"Unpatch method '{original_Navmesh_TotalDistance}'.", true);
                    AcTp0Tools.Patches.AcTp0Patcher.Harmony.Unpatch(original_Navmesh_TotalDistance,
                            prefix_Navmesh_MethodInfo);
                };

                AcTp0Tools.Patches.AcTp0Patcher.Harmony.Patch(original_Road_PathDistance, new HarmonyMethod(prefix_Road_PathDistance));
                unPatch += () =>
                {
                    ETLogger.WriteLine(LogType.Debug, $@"Unpatch of the '{original_Road_PathDistance}'.", true);
                    AcTp0Tools.Patches.AcTp0Patcher.Harmony.Unpatch(original_Road_PathDistance,
                            prefix_Road_PathDistance);
                };

                AcTp0Tools.Patches.AcTp0Patcher.Harmony.Patch(original_Road_GenerateRoadFromPlayer, new HarmonyMethod(prefix_Road_GenerateRoadFromPlayer));
                //unPatch += () =>
                //{
                //    ETLogger.WriteLine(LogType.Debug, $@"Unpatch method '{original_Road_GenerateRoadFromPlayer}'.", true);
                //    AcTp0Tools.Patches.AcTp0Patcher.Harmony.Unpatch(original_Road_GenerateRoadFromPlayer,
                //            prefix_Road_GenerateRoadFromPlayer);
                //};

                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Navigation)}' succeeded", true);
            }
            catch (Exception e)
            {
                ETLogger.WriteLine(LogType.Error, $@"Patch '{nameof(ComplexPatch_Navigation)}' failed", true);
                unPatch?.Invoke();
                ETLogger.WriteLine(LogType.Error, e.ToString(), true);
            }

            PatchesWasApplied = true;
        }

        #region GetPath
        //private static List<Vector3> Astral.Logic.Navmesh.getPath(Graph graph, Vector3 Start, Vector3 End)
        //{
        //	List<Vector3> list = new List<Vector3>();
        //	if (graph == null)
        //	{
        //		list.Add(End.Clone());
        //	}
        //	else
        //	{
        //		if (Navmesh.GetNearestNodePosFromPosition(graph, End).Distance3D(End) < Start.Distance3D(End))
        //		{
        //			AStar astar = new AStar(graph);
        //			Node nearestNodeFromPosition = Navmesh.GetNearestNodeFromPosition(graph, Start);
        //			Node nearestNodeFromPosition2 = Navmesh.GetNearestNodeFromPosition(graph, End);
        //			astar.SearchPath(nearestNodeFromPosition, nearestNodeFromPosition2);
        //			if (astar.PathFound && astar.PathByNodes.Length != 0)
        //			{
        //				foreach (Node node in astar.PathByNodes)
        //				{
        //					list.Add(new Vector3((float)node.X, (float)node.Y, (float)node.Z));
        //				}
        //			}
        //		}
        //		list.Add(End);
        //	}
        //	return list;
        //}

        /// <summary>
        /// Патч метода <see cref="Astral.Logic.Navmesh.getPath(Graph,Vector3,Vector3)"/>
        /// </summary>
        private static bool PrefixGetPath(ref List<Vector3> __result, Graph graph, Vector3 Start, Vector3 End)
        {
            if (__result is null)
                __result = new List<Vector3>();
            if (graph != null)
            {
                GetPathAndCorrect(graph, Start.X, Start.Y, Start.Z, End.X, End.Y, End.Z, ref __result);
            }
            else __result.Add(End.Clone());
            
            return false;

        }

        /// <summary>
        /// Поиск пути в графе <paramref name="graph"/> 
        /// из вершины наиболее близкой к <paramref name="start"/> 
        /// к вершине наиболее близкой к <paramref name="end"/>
        /// </summary>
        internal static List<Vector3> GetPath(Graph graph, Vector3 start, Vector3 end)
        {
            List<Vector3> waypoints = null;
            PrefixGetPath(ref waypoints, graph, start, end);
            return waypoints;
        }

        /// <summary>
        /// Поиск пути <paramref name="waypoints"/> в графе <paramref name="graph"/> 
        /// из вершины <paramref name="startNode"/> к вершине <paramref name="endNode"/>
        /// </summary>
        internal static bool GetPath(Graph graph, Node startNode, Node endNode, ref List<Vector3> waypoints)
        {
            if (waypoints is null)
                waypoints = new List<Vector3>();

            if (graph != null)
            {
                AStar.AStar astar = new AStar.AStar(graph);

#if PATCH_LOG
                stopwatch.Restart(); 
#endif
                astar.SearchPath(startNode, endNode);
#if PATCH_LOG
                stopwatch.Stop(); 
#endif

                if (astar.PathFound)
                {
#if false
                    waypoints.AddRange(astar.PathNodes.Select(nd => (Vector3)nd.Position));
#else
                    foreach (Node node in astar.PathNodes)
                        waypoints.Add(node.Position);

#endif
#if PATCH_LOG
                    if (waypoints.Count == 0)
                    {
                        stringBuilder.Clear();
                        stringBuilder.AppendLine(nameof(Patch_Astral_Logic_Navmesh_GetPath));
                        stringBuilder.Append("\tFrom '").Append(startNode).Append("' to '").Append(endNode).AppendLine("'");
                        stringBuilder.Append("\tElapsedTime: ").Append(stopwatch.ElapsedMilliseconds).AppendLine(" ms");
                        stringBuilder.AppendLine(Environment.StackTrace);

                        ETLogger.WriteLine(LogType.Debug, stringBuilder.ToString());
                    } 
#endif

                }

            }
            return waypoints.Count > 1;
        }

        /// <summary>
        /// Построение пути <param name="waypoints"/> в графе <param name="graph"/> из точки c координатами (<param name="startX"/>, <param name="startY"/>, <param name="startZ"/>)
        /// к точке c координатами (<param name="endX"/>, <param name="endY"/>, <param name="endZ"/>)
        /// </summary>
        /// <returns>True, если путь найден</returns>
        internal static bool GetPathAndCorrect(Graph graph, 
                                               double startX, double startY, double startZ, 
                                               double endX, double endY, double endZ,
                                               ref List<Vector3> waypoints)
        {
            bool debugInfo = EntityTools.Config.Logger.DebugNavigation;
            var methodName = debugInfo ? MethodBase.GetCurrentMethod().Name : string.Empty;


            var player = EntityManager.LocalPlayer;

            if (!player.IsValid || player.IsLoading)
            {
                if (debugInfo)
                    ETLogger.WriteLine(LogType.Error, $"{methodName}: The Player is out of the Game. Stop");
                return false;
            }

            if (startX == 0 && startY == 0 && startZ == 0)
            {
                if (debugInfo)
                    ETLogger.WriteLine(LogType.Error, $"{methodName}: Start point is invalid. Stop\n{Environment.StackTrace}");
                return false;
            }
            if (endX == 0 && endY == 0 && endZ == 0)
            {
                if (debugInfo)
                    ETLogger.WriteLine(LogType.Error, $"{methodName}: End point is invalid. Stop\n{Environment.StackTrace}");
                return false;
            }

            if (graph is null || graph.NodesCount < 1)
            {
                if (debugInfo)
                    ETLogger.WriteLine(LogType.Error, $"{methodName}: Graph is Empty. Stop\n{Environment.StackTrace}");
                return false;
            }

            if (debugInfo)
                ETLogger.WriteLine(LogType.Error, $"{methodName}: Search Nodes in the Graph closest to the Start and the End point");

            // для краткости 'start' будем называть точку с координатами (startX, startY, startZ), не принадлежащую графу
            //               'end' - точку с координатами (endX, endY, endZ), не принадлежащую графу

            // Поиск вершин графа, наиболее близких к координатам (startX, startY, startZ) и (endX, endY, endZ)
            graph.ClosestNodes(startX, startY, startZ, out double distanceStart2StartNode, out Node startNode,
                endX, endY, endZ, out double distanceEnd2EndNode, out Node endNode);

            if (startNode is null)
            {
                if (debugInfo)
                    ETLogger.WriteLine(LogType.Error, $"{methodName}: Start Node not found in the Graph. Stop\n{Environment.StackTrace}");
                return false;
            }
            if (endNode is null)
            {
                if (debugInfo)
                    ETLogger.WriteLine(LogType.Error, $"{methodName}: End Node not found in the Graph. Stop\n{Environment.StackTrace}");
                return false;
            }
            double /*x0, y0, z0, x1, y1, z1,*/ distance0, distance1, cos;

            if (ReferenceEquals(startNode, endNode))
            {
                if (debugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{methodName}: Only one Node of the Graph is located near the Start and the End points. Check direction");

                // startNode и endNode совпадают, 
                // то есть вблизи точек start и end находится только одна точка
                var pos0 = endNode.Position;

                // Проверяем возможность продвинуться напрямую от start к end
                // pos0 - вершина угла, величину которого оцениваем
#if false
                x0 = startX - pos0.X;
                y0 = startY - pos0.Y;
                z0 = startZ - pos0.Z;
                x1 = endX - pos0.X;
                y1 = endY - pos0.Y;
                z1 = endZ - pos0.Z;
#if false
                d0 = x0 * x0 + y0 * y0 + z0 * z0; // равно distanceStart2StartNode^2
                d1 = x1 * x1 + y1 * y1 + z1 * z1; // равно distanceEnd2EndNode^2

                cos = (x0 * x1 + y0 * y1 + z0 * z1) / Math.Sqrt(d0 * d1); 
#else
                cos = (x0 * x1 + y0 * y1 + z0 * z1) / distanceStart2StartNode * distanceEnd2EndNode;
#endif 
#else
                cos = NavigationHelper.Cosine(pos0.X, pos0.Y, pos0.Z, 
                                              startX, startY, startZ, 
                                              endX, endY, endZ, 
                                              out distance0, out distance1);
#endif

                if (waypoints is null)
                    waypoints = new List<Vector3>(2);

                if (cos < 0
                    && distanceStart2StartNode > 5
                    && distanceEnd2EndNode > 5)
                {
                    if (debugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{methodName}: Insert found Node to the Path");
                    waypoints.Add(pos0);
                }
                else if (debugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{methodName}: Ignore found Node. Move directly to the End point");

                waypoints.Add(new Vector3((float)endX, (float)endY, (float)endZ));
                return true;
            }

            if (debugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{methodName}: Starting the search of the Path using AStar.dll");

            var pathFinder = new AStar.AStar(graph);

#if PATCH_LOG
                stopwatch.Restart(); 
#endif
            bool searchResult = pathFinder.SearchPath(startNode, endNode);
#if PATCH_LOG
                stopwatch.Stop(); 
#endif

            if (!searchResult || !pathFinder.PathFound)
            {
                if (debugInfo)
                    ETLogger.WriteLine(LogType.Error, $"{methodName}: Path not found");
                return false;
            }

            var nodesCount = pathFinder.PathNodesCount;

            if (debugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{methodName}: Found Path has {nodesCount} waypoints. Starting the checking of the direction and correction the Path if needed");

            if (waypoints is null)
                waypoints = new List<Vector3>(nodesCount + 1);

            using (var enumerator = pathFinder.PathNodes.GetEnumerator())
            {
                Node wp0;
                if (enumerator.MoveNext()
                    && (wp0 = enumerator.Current) != null)
                {
                    Node wp1;
                    if (enumerator.MoveNext()
                        && (wp1 = enumerator.Current) != null)
                    {
                        // Найденный путь содержит не менее 2 вершин
                        var pos0 = wp0.Position;
                        var pos1 = wp1.Position;

                        // Проверяем положение стартовой точки start относительно первых двух точек пути(pos0 и pos1 соответственно)
                        cos = NavigationHelper.Cosine(pos0.X, pos0.Y, pos0.Z,
                                                    startX, startY, startZ,
                                                    pos1.X, pos1.Y, pos1.Z,
                                                    out _, out _);
                        // cos(165) = -0,9659258262890682867497431997289
                        // cos(105) = -0,25881904510252076234889883762405
                        // cos(75)  =  0,25881904510252076234889883762405
                        // cos(15)  =  0,9659258262890682867497431997289
                        // cos(30)  =  0,86602540378443864676372317075294
                        // cos(45)  =  0,70710678118654752440084436210485
                        // cos(60)  =  0,5

                        if (cos < 0)
                        {
                            // угол между векторами (wp0 -> start) и (wp0 -> wp1) больше 90 градусов,
                            // путь должен проходить через wp0 и wp1
                            waypoints.Add(pos0);
                        }
                        else
                        {
                            // угол между векторами (wp0 -> start) и (wp0 -> wp1) меньше 90 градусов,
                            // то есть wp0 находится "позади" и её добавлять не нужно
                            // путь должен проходить только через wp1
                            if (debugInfo)
                                ETLogger.WriteLine(LogType.Debug, $"{methodName}: Skipping the first waypoint");
                        }

                        if (nodesCount > 2)
                        {
                            waypoints.Add(wp1.Position);

                            // уменьшаем nodesCount на количество обработанных (скопированных) переменных
                            nodesCount -= 2;

                            // копируем все точки пути, за исключением последней
                            while (nodesCount > 1 && enumerator.MoveNext())
                            {
                                var wpNext = enumerator.Current;
                                if (wpNext != null)
                                    waypoints.Add(wpNext.Position);
                                --nodesCount;
                            }

                            wp1 = enumerator.MoveNext() ? enumerator.Current : null;
                        }

                        // проводим такую же оценку взаимного положения двух последних точек пути wp0, wp1 и end
                        // wp0 - указывает на предпоследнюю точку найденного пути, и последнюю добавленную в waypoints
                        // wp1 - указывает на последнюю точку найденного пути, еще не добавленную в waypoints

                        if (wp1 != null)
                        {
                            pos0 = wp0.Position;
                            pos1 = wp1.Position;

                            // Проверяем положение конечной точки end относительно последних двух точек пути (pos0 и pos1 соответственно)
                            // pos1 - вершина угла, величину которого оцениваем

                            cos = NavigationHelper.Cosine(pos1.X, pos1.Y, pos1.Z,
                                endX, endY, endZ,
                                pos0.X, pos0.Y, pos0.Z,
                                out _, out _);

                            if (cos > 0)
                                waypoints.Add(pos1);
                        }
                    }
                    else
                    {
                        if (debugInfo)
                            ETLogger.WriteLine(LogType.Debug, $"{methodName}: The Path has only one waypoint. Check direction");

                        // Путь содержит только одну точку
                        //waypoints.Add(wp0.Position);
                        var pos0 = wp0.Position;

                        // Проверяем возможность продвинуться напрямую от start к end
                        // pos0 - вершина угла, величину которого оцениваем 
                        cos = NavigationHelper.Cosine(pos0.X, pos0.Y, pos0.Z,
                                                    startX, startY, startZ,
                                                    endX, endY, endZ, out distance0, out distance1);

                        // Добавляем wp0 лишь в том случае,
                        // если стартовая точка находится в задней полуплоскости относительно вектора (wp0 -> end),
                        // то есть угол между векторами (pw0 -> start) и (pw0 -> end) больше 90 градусов (при этом cos отрицательный)
                        if (cos < 0
                            && distance0 > 25
                            && distance1 > 25)
                            waypoints.Add(pos0);
                        else if (debugInfo)
                            ETLogger.WriteLine(LogType.Debug, $"{methodName}: Remove waypoint from the Path. Move directly to the End point");

                    }
                }
                if (debugInfo)
                    ETLogger.WriteLine(LogType.Error, $"{methodName}: First waypoint is invalid. Reset the Path");
            }

            waypoints.Add(new Vector3((float)endX, (float)endY, (float)endZ));
#if PATCH_LOG
                    if (waypoints.Count == 0)
                    {
                        stringBuilder.Clear();
                        stringBuilder.AppendLine(nameof(Patch_Astral_Logic_Navmesh_GetPath));
                        stringBuilder.Append("\tFrom '").Append(startNode).Append("' to '").Append(endNode).AppendLine("'");
                        stringBuilder.Append("\tElapsedTime: ").Append(stopwatch.ElapsedMilliseconds).AppendLine(" ms");
                        stringBuilder.AppendLine(Environment.StackTrace);

                        ETLogger.WriteLine(LogType.Debug, stringBuilder.ToString());
                    } 
#endif
            return true;//waypoints.Count > 1;
        }
        #endregion

        #region GenerateRoad
        //public static Road Astral.Logic.Navmesh.GenerateRoad(Graph graph, Vector3 Start, Vector3 End, bool allowPathFinding)
        //{
        //	string obj = "AddPointWork";
        //	Road result;
        //	lock (obj)
        //	{
        //		Road road = new Road();
        //		if (Class1.CurrentSettings.UsePathfinding3)
        //		{
        //			List<Vector3> list = new List<Vector3>();
        //			if (allowPathFinding || graph.Nodes.Count == 0)
        //			{
        //				list = PathFinding.GetPathAndCorrect(Start, End, Class1.CurrentSettings.AllowPartialPath);
        //			}
        //			if (list.Count > 0)
        //			{
        //				road.Type = Road.RoadGenType.PathFinding;
        //				road.Waypoints = list;
        //			}
        //			else
        //			{
        //				List<Vector3> list2 = new List<Vector3>();
        //				if (graph != null && graph.Nodes.Count > 0 && !BotClient.Client.tcpClient_0.Connected)
        //				{
        //					Vector3 nearestNodePosFromPosition = Navmesh.GetNearestNodePosFromPosition(graph, Start);
        //					if (nearestNodePosFromPosition.Distance3D(Start) < 130.0)
        //					{
        //						road.Type = Road.RoadGenType.StandardGraph;
        //						list2 = Navmesh.getPath(graph, nearestNodePosFromPosition, End);
        //					}
        //				}
        //				if (list2.Count == 0 && BotClient.Client.tcpClient_0.Connected)
        //				{
        //					road.Type = Road.RoadGenType.StandardGraph;
        //					list2 = BotClient.QueryPathToServer(Start, End);
        //				}
        //				if (list2.Count == 0)
        //				{
        //					road.Type = Road.RoadGenType.GoldenPathGraph;
        //					list2 = Navmesh.GetPathAndCorrect(GoldenPath.GetCurrentMapGraphFromCache(), Start, End);
        //				}
        //				if (list2.Count > 0)
        //				{
        //					Vector3 vector = Vector3.Empty;
        //					if (PathFinding.CheckDirection(Start, list2[0], ref vector))
        //					{
        //						List<Vector3> path = PathFinding.GetPathAndCorrect(Start, list2[0], true);
        //						road.Waypoints.AddRange(path);
        //					}
        //					road.Waypoints.AddRange(list2);
        //				}
        //				else
        //				{
        //					List<Vector3> path2 = PathFinding.GetPathAndCorrect(Start, End, true);
        //					road.Type = Road.RoadGenType.PartialPathFinding;
        //					road.Waypoints = path2;
        //				}
        //			}
        //		}
        //		else if (graph == null)
        //		{
        //			road.Waypoints.Add(End.Clone());
        //		}
        //		else
        //		{
        //			road.Waypoints = Navmesh.GetPathAndCorrect(graph, Start, End);
        //		}
        //		result = road;
        //	}
        //	return result;
        //}

        private static readonly object @lock = new object();

        internal static Road GenerateRoad(Graph graph, Vector3 start, Vector3 end, bool allowPathFinding)
        {
            PrefixGenerateRoad(out Road road, graph, start, end, allowPathFinding);
            return road;
        }
        private static bool PrefixGenerateRoad(out Road __result, Graph graph, Vector3 Start, Vector3 End, bool allowPathFinding)
        {
            bool debugInfo = EntityTools.Config.Logger.DebugNavigation;
            var methodName = debugInfo ? MethodBase.GetCurrentMethod().Name : string.Empty;

            //TODO Переработать корректировку пути (первую и последнюю точки)
            Road road = new Road();
#if PATCH_LOG
            long pathFindingOrEmptyGraph_Time = 0;
            long standardGraphSearching_Time = 0;
            long queryPathToServer_Time = 0;
            long goldenPathSearch_Time = 0;
            long partialPathSearch_Time = 0;
            long partialPathFinding_Time = 0;
            long directPathFinding_Time = 0;
            stringBuilder.Clear();
            stringBuilder.Append(nameof(Patch_Astral_Logic_Navmesh_GenerateRoad)).Append('.').AppendLine(nameof(GenerateRoad)); 
#endif

            if (End != null && End.IsValid)
            {
                if (Start == null || !Start.IsValid)
                {
                    var player = EntityManager.LocalPlayer;
                    if (!player.IsValid || player.IsLoading)
                    {
                        if (debugInfo)
                            ETLogger.WriteLine(LogType.Error, $"{methodName}: Start node is invalid. The Player is out of the Game. Stop\n{Environment.StackTrace}");
                        __result = road;
                        return false;
                    }

                    if (debugInfo)
                        ETLogger.WriteLine(LogType.Error, $"{methodName}: Start node is invalid. Using the Player's location\n{Environment.StackTrace}");

                    Start = player.Location.Clone();
                }

                bool graphOK = graph?.NodesCount > 0;

                if(debugInfo)
                {
                    if (graphOK)
                        ETLogger.WriteLine(LogType.Debug, $"{methodName}: Graph has {graph.NodesCount} nodes");
                    else ETLogger.WriteLine(LogType.Error, $"{methodName}: Graph is empty");
                }

                lock (@lock)
                {
#if PATCH_LOG
                    stopwatch.Start(); 
#endif
                    if (Astral.Controllers.Settings.Get.UsePathfinding3)
                    {
                        List<Vector3> waypoints = null;
                        if (allowPathFinding || !graphOK)
                        {
#if PATCH_LOG
                            pathFindingOrEmptyGraph_Time = stopwatch.ElapsedTicks; 
#endif
                            waypoints = PathFinding.GetPath(Start, End,
                                Astral.Controllers.Settings.Get.AllowPartialPath);
#if PATCH_LOG
                            pathFindingOrEmptyGraph_Time = stopwatch.ElapsedTicks - pathFindingOrEmptyGraph_Time;
                            stringBuilder.Append("Search PathFindingOrEmptyGraph {").Append(Start).Append("} => {").Append(End).AppendLine("}")
                                .Append("\tSearch time ").AppendLine(pathFindingOrEmptyGraph_Time.ToString()); 
#endif
                        }

                        var pathCount = waypoints?.Count ?? 0;

                        if (pathCount > 0)
                        {
                            if (debugInfo)
                                ETLogger.WriteLine(LogType.Debug, $"{methodName}: PathFinding found the path which has {pathCount} waypoints");

                            road.Type = Road.RoadGenType.PathFinding;
                            road.Waypoints = waypoints;
                        }
                        else
                        {
                            if (waypoints is null)
                                waypoints = new List<Vector3>();

                            if (debugInfo)
                                ETLogger.WriteLine(LogType.Debug, $"{methodName}: PathFinding did not found the path");

                            bool tcpClientConnected = AstralAccessors.Controllers.BotComs.BotClient.Client_TcpClient.Connected;
                            if (graphOK && !tcpClientConnected)
                            {
#if PATCH_LOG
                                standardGraphSearching_Time = stopwatch.ElapsedTicks; 
#endif
                                using (graph.ReadLock())
                                {
                                    road.Type = Road.RoadGenType.StandardGraph;
                                    GetPathAndCorrect(graph, Start.X, Start.Y, Start.Z, End.X, End.Y, End.Z, ref waypoints);
                                }
#if PATCH_LOG
                                standardGraphSearching_Time = stopwatch.ElapsedTicks - standardGraphSearching_Time;
                                stringBuilder.AppendLine("Search StandardGraph")
                                             .Append("\tStart pos {").Append(Start).Append("} to Node {")
                                                    .Append(startNode).Append("} at the distance ").AppendLine(dist1.ToString())
                                             .Append("\tEnd pos {").Append(End).Append("} to Node {")
                                             .Append(endNode).Append("} at the distance ").AppendLine(dist2.ToString())
                                             .Append("\tSearch time ").AppendLine(standardGraphSearching_Time.ToString()); 
#endif
                                if (debugInfo)
                                {
                                    pathCount = waypoints.Count;
                                    if (pathCount > 0)
                                        ETLogger.WriteLine(LogType.Debug, $"{methodName}: AStar found in StandardGraph the path which has {pathCount} waypoints");
                                    else
                                        ETLogger.WriteLine(LogType.Debug, $"{methodName}: AStar did not found the path in StandardGraph");
                                }
                            }

                            if (waypoints.Count == 0 && tcpClientConnected)
                            {
#if PATCH_LOG
                                queryPathToServer_Time = stopwatch.ElapsedTicks; 
#endif
                                road.Type = Road.RoadGenType.StandardGraph;
                                waypoints = Astral.Controllers.BotComs.BotClient.QueryPathToServer(Start, End);
#if PATCH_LOG
                                queryPathToServer_Time = stopwatch.ElapsedTicks - queryPathToServer_Time;
                                stringBuilder.Append("Search QueryPathToServer {").Append(Start).Append("} => {").Append(End).AppendLine("}")
                                    .Append("\tSearch time ").AppendLine(queryPathToServer_Time.ToString()); 
#endif
                                if (debugInfo)
                                {
                                    pathCount = waypoints.Count;
                                    if (pathCount > 0)
                                        ETLogger.WriteLine(LogType.Debug, $"{methodName}: Receive from Server the path which has {pathCount} waypoints");
                                    else
                                        ETLogger.WriteLine(LogType.Debug, $"{methodName}: Server did not send the path");
                                }
                            }

                            if (waypoints.Count == 0)
                            {
#if PATCH_LOG
                                goldenPathSearch_Time = stopwatch.ElapsedTicks; 
#endif
                                if (debugInfo)
                                    ETLogger.WriteLine(LogType.Debug, $"{methodName}: PathFinding and AStar did not found the path. Using GoldenPath");

                                road.Type = Road.RoadGenType.GoldenPathGraph;
                                var goldGraph = Astral.Logic.NW.GoldenPath.GetCurrentMapGraphFromCache();
                                using (goldGraph.ReadLock())
                                {
                                    GetPathAndCorrect(goldGraph, Start.X, Start.Y, Start.Z, End.X, End.Y, End.Z, ref waypoints);
                                }
#if PATCH_LOG
                                goldenPathSearch_Time = stopwatch.ElapsedTicks - goldenPathSearch_Time;
                                stringBuilder.Append("Search GoldenPathSearch {").Append(Start).Append("} => {").Append(End).AppendLine("}")
                                    .Append("\tSearch time ").AppendLine(goldenPathSearch_Time.ToString()); 
#endif
                                if (debugInfo)
                                {
                                    pathCount = waypoints.Count;
                                    if (pathCount > 0)
                                        ETLogger.WriteLine(LogType.Debug, $"{methodName}: AStar found in GoldenPath the path which has {pathCount} waypoints");
                                    else
                                        ETLogger.WriteLine(LogType.Debug, $"{methodName}: AStar did not found the path in GoldenPath");
                                }
                            }

                            pathCount = waypoints.Count;

                            if (pathCount > 0)
                            {
#if PATCH_LOG
                                partialPathSearch_Time = stopwatch.ElapsedTicks; 
#endif
                                var start2firstDistance = NavigationHelper.SquareDistance3D(Start, waypoints[0]);
                                if (start2firstDistance > 100)
                                {
                                    if (debugInfo)
                                        ETLogger.WriteLine(LogType.Debug,
                                            $"{methodName}: Path has {pathCount} waypoints. " +
                                            $"Distance to the first waypoint is {start2firstDistance:N1} therefore using the PartialPathfinding");
                                    
                                    var vector = Vector3.Empty;
                                    if (PathFinding.CheckDirection(Start, waypoints[0], ref vector))
                                    {
                                        List<Vector3> path = PathFinding.GetPath(Start, waypoints[0], true);

                                        waypoints.InsertRange(0, path);
                                    }
                                }

                                road.Waypoints = waypoints;
#if PATCH_LOG
                                partialPathSearch_Time = stopwatch.ElapsedTicks - partialPathSearch_Time;
                                stringBuilder.Append("Search PartialPath from {").Append(Start).Append("} => {").Append(waypoints[0]).AppendLine("}")
                                    .Append("\tSearch time ").AppendLine(partialPathSearch_Time.ToString()); 
#endif
                                if (debugInfo)
                                {
                                    pathCount = waypoints.Count;
                                    if (pathCount > 0)
                                        ETLogger.WriteLine(LogType.Debug, $"{methodName}: PathFinding found the path which has {pathCount} waypoints");
                                    else
                                        ETLogger.WriteLine(LogType.Debug, $"{methodName}: PathFinding did not found the path");
                                }
                            }
                            else
                            {
                                if (debugInfo)
                                    ETLogger.WriteLine(LogType.Debug, $"{methodName}: No path found. Using PathFinding");
#if PATCH_LOG
                                partialPathFinding_Time = stopwatch.ElapsedTicks; 
#endif
                                var path = PathFinding.GetPath(Start, End, true);
                                if (path?.Count > 0)
                                {
                                    road.Type = Road.RoadGenType.PartialPathFinding;
                                    road.Waypoints = path;
                                }
                                else road.Waypoints.Add(End.Clone());
#if PATCH_LOG
                                partialPathFinding_Time = stopwatch.ElapsedTicks - partialPathFinding_Time;
                                stringBuilder.Append("PartialPathFinding from {").Append(Start).Append("} => {").Append(End).AppendLine("}")
                                    .Append("\tSearch time ").AppendLine(partialPathFinding_Time.ToString()); 
#endif
                                if (debugInfo)
                                {
                                    pathCount = waypoints.Count;
                                    if (pathCount > 0)
                                        ETLogger.WriteLine(LogType.Debug, $"{methodName}: PathFinding found the path which has {pathCount} waypoints");
                                    else
                                        ETLogger.WriteLine(LogType.Debug, $"{methodName}: PathFinding did not found the path");
                                }
                            }
                        }
                    }
                    else if (graphOK)
                    {
#if PATCH_LOG
                        directPathFinding_Time = stopwatch.ElapsedTicks; 
#endif
                        using (graph.ReadLock())
                        {
                            var path = GetPath(graph, Start, End);
                            if (path?.Count > 0)
                                road.Waypoints = path;
                            else road.Waypoints.Add(End.Clone());
                        }
#if PATCH_LOG
                        directPathFinding_Time = stopwatch.ElapsedTicks - directPathFinding_Time;
                        stringBuilder.Append("DirectPathFinding from {").Append(Start).Append("} => {").Append(End).AppendLine("}")
                            .Append("\tSearch time ").AppendLine(directPathFinding_Time.ToString()); 
#endif
                        if (debugInfo)
                        {
                            var pathCount = road.Waypoints.Count;
                            if (pathCount > 0)
                                ETLogger.WriteLine(LogType.Debug, $"{methodName}: AStar found in StandardGraph the path which has {pathCount} waypoints");
                            else
                                ETLogger.WriteLine(LogType.Debug, $"{methodName}: AStar did not found the path in StandardGraph");
                        }
                    }
                    else
                    {
                        if (debugInfo)
                            ETLogger.WriteLine(LogType.Debug, $"{methodName}: PathFinding are disabled and Graph is invalid therefore moving to the End waypoint directly");

                        road.Waypoints.Add(End.Clone());
                    }

#if PATCH_LOG
                    stopwatch.Stop();

                    if (pathFindingOrEmptyGraph_Time > 500
                        || standardGraphSearching_Time > 500
                        || queryPathToServer_Time > 500
                        || goldenPathSearch_Time > 500
                        || partialPathSearch_Time > 500
                        || partialPathFinding_Time > 500
                        || directPathFinding_Time > 500)
                    {
                        long time = pathFindingOrEmptyGraph_Time + standardGraphSearching_Time +
                                    queryPathToServer_Time + partialPathSearch_Time + partialPathFinding_Time +
                                    directPathFinding_Time;
                        stringBuilder.Append("Total time: \t").Append((time)).Append(" ms (").Append(stopwatch.ElapsedTicks).AppendLine(")");
                        if (road?.Waypoints.Count > 0)
                            stringBuilder.Append("Road waypoints number: ").AppendLine(road.Waypoints.Count.ToString());

                        stringBuilder.AppendLine(Environment.StackTrace);


                        ETLogger.WriteLine(LogType.Debug, stringBuilder.ToString());
                    } 
#endif
                }
            }
            else
            {
                if (debugInfo)
                    ETLogger.WriteLine(LogType.Error, $"{methodName}: End node is invalid. Stop\n{Environment.StackTrace}");
            }
            __result = road;

            return false;
        }
        #endregion

        #region GetNearestNodeFromPosition
        //public static Node Astral.Logic.Navmesh.GetNearestNodeFromPosition(Graph graph, Vector3 pos)
        //{
        //	double num = double.MaxValue;
        //	Node result = new Node(0.0, 0.0, 0.0);
        //	if (graph == null)
        //	{
        //		return new Node((double)pos.X, (double)pos.Y, (double)pos.Z);
        //	}
        //	foreach (object obj in graph.Nodes)
        //	{
        //		Node node = (Node)obj;
        //		if (node.Passable)
        //		{
        //			Vector3 vector = new Vector3(pos.X, pos.Y, pos.Z);
        //			Vector3 from = new Vector3((float)node.X, (float)node.Y, (float)node.Z);
        //			double num2 = vector.Distance3D(from);
        //			if (num2 < num)
        //			{
        //				num = num2;
        //				result = node;
        //			}
        //		}
        //	}
        //    return result;
        //}

        internal static Node GetNearestNodeFromPosition(Graph graph, Vector3 pos) 
        {
            PrefixGetNearestNodeFromPosition(out Node node, graph, pos);
            return node;
        }
        private static bool PrefixGetNearestNodeFromPosition(out Node __result, Graph graph, Vector3 Pos)
        {
            Node node = null;
            if (graph?.NodesCount > 0 && Pos != null && Pos.IsValid)
                node = graph.ClosestNode(Pos.X, Pos.Y, Pos.Z, out _, false);

            __result = node ?? new Node(0, 0, 0);
            return false;
        }
        #endregion

        #region FixPath
        //private static void Astral.Logic.Navmesh.FixPath(List<Vector3> waypoints, Vector3 from)
        //{
        //	int num = 0;
        //	while (waypoints.Count > 1)
        //	{
        //		num++;
        //		if (num > 3)
        //		{
        //			return;
        //		}
        //		double num2 = waypoints[0].Distance3D(from) + waypoints[0].Distance3D(waypoints[1]);
        //		if (waypoints[1].Distance3D(from) * 1.1 <= num2)
        //		{
        //			waypoints.RemoveAt(0);
        //		}
        //	}
        //}

        // ReSharper disable UnusedParameter.Local
        private static void PrefixFixPath(List<Vector3> waypoints, Vector3 from) { }
        // ReSharper restore UnusedParameter.Local
        #endregion

        #region TotalDistance
        // public static double Astral.Logic.Navmesh.TotalDistance(List<Vector3> positions)
        //public static double Astral.Logic.Navmesh.TotalDistance(List<Vector3> positions)
        //{
        //	if (positions.Count <= 1)
        //	{
        //		return 0.0;
        //	}
        //	double num = positions[0].Distance3D(positions[1]);
        //	for (int i = 1; i < positions.Count; i++)
        //	{
        //		num += positions[i - 1].Distance3D(positions[i]);
        //	}
        //	return num;
        //}

        internal static double TotalDistance(List<Vector3> positions)
        {
            PrefixTotalDistance(out double distance, positions);
            return distance;
        }
        private static bool PrefixTotalDistance(out double __result, List<Vector3> positions)
        {
            __result = 0;
            if (positions?.Count > 1)
            {
                using (var posEnumerator = positions.GetEnumerator())
                {
                    if (posEnumerator.MoveNext())
                    {
                        var pos1 = posEnumerator.Current;
                        while (posEnumerator.MoveNext())
                        {
                            var pos2 = posEnumerator.Current;
                            __result += MathHelper.Distance3D(pos1, pos2);
                            pos1 = pos2;
                        }
                    }
                }
            }

            return false;
        }
        #endregion

        #region PathDistance
        //internal static double Astral.Quester.Controllers.Road.PathDistance(Vector3 pos)
        //{
        //	Road road = Road.GenerateRoadFromPlayer(pos);
        //	road.Waypoints.Insert(0, \u0001.LocalPlayer.Location);
        //	return Navmesh.TotalDistance(road.Waypoints);
        //}

        internal static bool PathDistance(out double __result, Vector3 pos)
        {
            bool debugInfo = EntityTools.Config.Logger.DebugNavigation;
            var methodName = debugInfo ? MethodBase.GetCurrentMethod().Name : string.Empty;

            __result = 0;

            var player = EntityManager.LocalPlayer;

            if (player.IsValid && !player.IsLoading)
            {   
                if (pos != null && pos.IsValid)
                {

                    var playerLocation = player.Location;
                    var graph = AstralAccessors.Controllers.Roles.CurrentRole.UsedMeshes;

                    if (graph?.NodesCount > 0)
                    {
                        if (debugInfo)
                            ETLogger.WriteLine(LogType.Debug, $"{methodName}: Starting Road generation");
                        var road = GenerateRoad(graph,
                            playerLocation, pos, !Astral.API.CurrentSettings.PFOnlyForApproaches);

                        if (road.Waypoints.Count > 1)
                        {
                            __result = MathHelper.Distance3D(playerLocation, road.Waypoints[0]);
                            __result += TotalDistance(road.Waypoints);

                            if (debugInfo)
                                ETLogger.WriteLine(LogType.Debug, $"{methodName}: Result is {__result:N1}");
                        }
                        else
                        {
                            __result = pos.Distance3D(playerLocation);
                            if (debugInfo)
                                ETLogger.WriteLine(LogType.Debug, $"{methodName}: Path has only one waypoint. Linear distance is {__result:N1}");
                        }
                    }
                    else
                    {
                        __result = pos.Distance3D(playerLocation);
                        if (debugInfo)
                            ETLogger.WriteLine(LogType.Debug, $"{methodName}: Graph is empty. Linear distance is {__result:N1}");
                    }
                }
                else if (debugInfo)
                    ETLogger.WriteLine(LogType.Error, $"{methodName}: End point is invalid. Stop");
            }
            else if (debugInfo)
                ETLogger.WriteLine(LogType.Error, $"{methodName}: The Player is out of the Game. Stop");

            return false;
        }
        #endregion

        #region GenerateRoadFromPlayer
		//internal static Road Astral.Quester.Controllers.Road.GenerateRoadFromPlayer(Vector3 end)
		//{
		//	return Navmesh.GenerateRoadFromPlayer(Core.Meshes, end, !\u0001.CurrentSettings.PFOnlyForApproaches);
		//}

        internal static bool GenerateRoadFromPlayer(out Road __result, Vector3 end)
        {
            bool debugInfo = EntityTools.Config.Logger.DebugNavigation;
            var methodName = debugInfo ? MethodBase.GetCurrentMethod().Name : string.Empty;

            var player = EntityManager.LocalPlayer;

            if (player.IsValid && !player.IsLoading)
            {
                if (end?.IsValid == true)
                {
                    if (debugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{methodName}: Starting Road generation");

                    var playerLocation = player.Location;
                    var graph = AstralAccessors.Controllers.Roles.CurrentRole.UsedMeshes;
                    __result = GenerateRoad(graph,
                        playerLocation, end, !Astral.API.CurrentSettings.PFOnlyForApproaches);
                    return false;
                }
                if (debugInfo)
                    ETLogger.WriteLine(LogType.Error, $"{methodName}: End node is invalid. Stop\n{Environment.StackTrace}");
            }
            else if (debugInfo)
                ETLogger.WriteLine(LogType.Error, $"{methodName}: The Player is out of the Game. Stop");

            __result = new Road();

            return false;
        } 
        #endregion
    }
}
