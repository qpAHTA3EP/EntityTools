using AcTp0Tools;
using AStar;
using Astral.Logic.Classes.FSM;
using Astral.Logic.Classes.Map;
using Astral.Logic.NW;
using EntityTools.Patches.Mapper;
using HarmonyLib;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;

// ReSharper disable InconsistentNaming

namespace EntityTools.Patches
{
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

            original_Navmesh_GetPath = AccessTools.Method(tNavmesh, "getPath");
            if (original_Navmesh_GetPath is null)
            {
                ETLogger.WriteLine($@"{nameof(original_Navmesh_GetPath)} not found");
                return;
            }
            prefix_Navmesh_GetPath = AccessTools.Method(tPatch, nameof(PrefixGetPath));
            if (prefix_Navmesh_GetPath is null)
            {
                ETLogger.WriteLine($@"{nameof(prefix_Navmesh_GetPath)} not found");
                return;
            }            
            original_Navmesh_GenerateRoad = AccessTools.Method(tNavmesh, "GenerateRoad");
            if (original_Navmesh_GenerateRoad is null)
            {
                ETLogger.WriteLine($@"{nameof(original_Navmesh_GenerateRoad)} not found");
                return;
            }
            prefix_Navmesh_GenerateRoad = AccessTools.Method(tPatch, nameof(PrefixGenerateRoad));
            if (prefix_Navmesh_GenerateRoad is null)
            {
                ETLogger.WriteLine($@"{nameof(prefix_Navmesh_GenerateRoad)} not found");
                return;
            }
            original_Navmesh_GetNearestNodeFromPosition = AccessTools.Method(tNavmesh, "GetNearestNodeFromPosition");
            if (original_Navmesh_GetNearestNodeFromPosition is null)
            {
                ETLogger.WriteLine($@"{nameof(original_Navmesh_GetNearestNodeFromPosition)} not found");
                return;
            }
            prefix_Navmesh_GetNearestNodeFromPosition = AccessTools.Method(tPatch, nameof(PrefixGetNearestNodeFromPosition));
            if (prefix_Navmesh_GetNearestNodeFromPosition is null)
            {
                ETLogger.WriteLine($@"{nameof(prefix_Navmesh_GetNearestNodeFromPosition)} not found");
                return;
            }
            original_Navmesh_FixPath = AccessTools.Method(tNavmesh, "fixPath");
            if (original_Navmesh_FixPath is null)
            {
                ETLogger.WriteLine($@"{nameof(original_Navmesh_FixPath)} not found");
                return;
            }
            prefix_Navmesh_FixPath = AccessTools.Method(tPatch, nameof(PrefixFixPath));
            if (prefix_Navmesh_FixPath is null)
            {
                ETLogger.WriteLine($@"{nameof(prefix_Navmesh_FixPath)} not found");
                return;
            }
            original_Navmesh_TotalDistance = AccessTools.Method(tNavmesh, "TotalDistance");
            if (original_Navmesh_TotalDistance is null)
            {
                ETLogger.WriteLine($@"{nameof(original_Navmesh_TotalDistance)} not found");
                return;
            }
            prefix_Navmesh_MethodInfo = AccessTools.Method(tPatch, nameof(PrefixTotalDistance));
            if (prefix_Navmesh_MethodInfo is null)
            {
                ETLogger.WriteLine($@"{nameof(prefix_Navmesh_MethodInfo)} not found");
                return;
            }
            original_Road_PathDistance = AccessTools.Method(tRoad, "PathDistance");
            if (original_Road_PathDistance is null)
            {
                ETLogger.WriteLine($@"{nameof(original_Road_PathDistance)} not found");
                return;
            }
            prefix_Road_PathDistance = AccessTools.Method(tPatch, nameof(PathDistance));
            if (prefix_Road_PathDistance is null)
            {
                ETLogger.WriteLine($@"{nameof(prefix_Road_PathDistance)} not found");
                return;
            }
            original_Road_GenerateRoadFromPlayer = AccessTools.Method(tRoad, "GenerateRoadFromPlayer");
            if (original_Road_GenerateRoadFromPlayer is null)
            {
                ETLogger.WriteLine($@"{nameof(original_Road_GenerateRoadFromPlayer)} not found");
                return;
            }
            prefix_Road_GenerateRoadFromPlayer = AccessTools.Method(tPatch, nameof(GenerateRoadFromPlayer));
            if (prefix_Road_GenerateRoadFromPlayer is null)
            {
                ETLogger.WriteLine($@"{nameof(prefix_Road_GenerateRoadFromPlayer)} was not found");
                return;
            }

            Action unPatch = null;

            try
            {
                AcTp0Tools.Patches.AcTp0Patcher.Harmony.Patch(original_Navmesh_GetPath, new HarmonyMethod(prefix_Navmesh_GetPath));
                unPatch = () => AcTp0Tools.Patches.AcTp0Patcher.Harmony.Unpatch(original_Navmesh_GetPath, prefix_Navmesh_GetPath);

                AcTp0Tools.Patches.AcTp0Patcher.Harmony.Patch(original_Navmesh_GenerateRoad, new HarmonyMethod(prefix_Navmesh_GenerateRoad));
                unPatch += () => AcTp0Tools.Patches.AcTp0Patcher.Harmony.Unpatch(original_Navmesh_GenerateRoad, prefix_Navmesh_GenerateRoad);

                AcTp0Tools.Patches.AcTp0Patcher.Harmony.Patch(original_Navmesh_GetNearestNodeFromPosition, new HarmonyMethod(prefix_Navmesh_GetNearestNodeFromPosition));
                unPatch += () => AcTp0Tools.Patches.AcTp0Patcher.Harmony.Unpatch(original_Navmesh_GetNearestNodeFromPosition, prefix_Navmesh_GetNearestNodeFromPosition);

                AcTp0Tools.Patches.AcTp0Patcher.Harmony.Patch(original_Navmesh_FixPath, new HarmonyMethod(prefix_Navmesh_FixPath));
                unPatch += () => AcTp0Tools.Patches.AcTp0Patcher.Harmony.Unpatch(original_Navmesh_FixPath, prefix_Navmesh_FixPath);

                AcTp0Tools.Patches.AcTp0Patcher.Harmony.Patch(original_Navmesh_TotalDistance, new HarmonyMethod(prefix_Navmesh_MethodInfo));
                unPatch += () => AcTp0Tools.Patches.AcTp0Patcher.Harmony.Unpatch(original_Navmesh_TotalDistance, prefix_Navmesh_MethodInfo);

                AcTp0Tools.Patches.AcTp0Patcher.Harmony.Patch(original_Road_PathDistance, new HarmonyMethod(prefix_Road_PathDistance));
                unPatch += () => AcTp0Tools.Patches.AcTp0Patcher.Harmony.Unpatch(original_Road_PathDistance, prefix_Road_PathDistance);

                AcTp0Tools.Patches.AcTp0Patcher.Harmony.Patch(original_Road_GenerateRoadFromPlayer, new HarmonyMethod(prefix_Road_GenerateRoadFromPlayer));
                unPatch += () => AcTp0Tools.Patches.AcTp0Patcher.Harmony.Unpatch(original_Road_GenerateRoadFromPlayer, prefix_Road_GenerateRoadFromPlayer);
            }
            catch (Exception e)
            {
                unPatch?.Invoke();
                ETLogger.WriteLine(LogType.Error, $@"{nameof(ComplexPatch_Navigation)} failed");
                ETLogger.WriteLine(LogType.Error, e.ToString());
                throw;
            }

            PatchesWasApplied = true;
            ETLogger.WriteLine($@"{nameof(ComplexPatch_Navigation)} succeeded");
        }

        #region GetPath
#if false   // private static List<Vector3> Astral.Logic.Navmesh.getPath(Graph graph, Vector3 Start, Vector3 End)
private static List<Vector3> Astral.Logic.Navmesh.getPath(Graph graph, Vector3 Start, Vector3 End)
{
	List<Vector3> list = new List<Vector3>();
	if (graph == null)
	{
		list.Add(End.Clone());
	}
	else
	{
		if (Navmesh.GetNearestNodePosFromPosition(graph, End).Distance3D(End) < Start.Distance3D(End))
		{
			AStar astar = new AStar(graph);
			Node nearestNodeFromPosition = Navmesh.GetNearestNodeFromPosition(graph, Start);
			Node nearestNodeFromPosition2 = Navmesh.GetNearestNodeFromPosition(graph, End);
			astar.SearchPath(nearestNodeFromPosition, nearestNodeFromPosition2);
			if (astar.PathFound && astar.PathByNodes.Length != 0)
			{
				foreach (Node node in astar.PathByNodes)
				{
					list.Add(new Vector3((float)node.X, (float)node.Y, (float)node.Z));
				}
			}
		}
		list.Add(End);
	}
	return list;
}
#endif
        /// <summary>
        /// Патч метода <see cref="Astral.Logic.Navmesh.getPath(Graph,Vector3,Vector3)"/>
        /// </summary>
        private static bool PrefixGetPath(ref List<Vector3> __result, Graph graph, Vector3 Start, Vector3 End)
        {
            if (__result is null)
                __result = new List<Vector3>();
            if (graph != null)
            {
                graph.ClosestNodes(Start.X, Start.Y, Start.Z, out double _, out Node startNode,
                    End.X, End.Y, End.Z, out double _, out Node endNode);

                GetPathAndCorrect(graph, startNode, Start, endNode, End, ref __result);
            }
            __result.Add(End.Clone());
            
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

        internal static bool GetPathAndCorrect(Graph graph, Node startNode, Vector3 start, Node endNode, Vector3 end,
            ref List<Vector3> waypoints)
        {
            //TODO Переработать корректировку пути (первую и последнюю точки)
            if (waypoints is null)
                waypoints = new List<Vector3>();
            if (ReferenceEquals(startNode, endNode) || graph is null)
                return false;

            AStar.AStar astar = new AStar.AStar(graph);

#if PATCH_LOG
                stopwatch.Restart(); 
#endif
            bool searchResult = astar.SearchPath(startNode, endNode);
#if PATCH_LOG
                stopwatch.Stop(); 
#endif

            if (!searchResult || !astar.PathFound) return false;

            using (var wpe = astar.PathNodes.GetEnumerator())
            {
                if (wpe.MoveNext())
                {
                    var wp0 = wpe.Current ?? throw new NullReferenceException();
                    if (wpe.MoveNext())
                    {
                        var wp1 = wpe.Current;
                        if (start != null && start.IsValid)
                        {
                            var pos0 = wp0.Position;
                            var pos1 = wp1.Position;

                            // найденный путь содержит не менее 2 вершин
                            // проверяем направления и корректируем "возврат"
                            double x0 = pos0.X - start.X,
                                y0 = pos0.Y - start.Y,
                                z0 = pos0.Z - start.Z,
                                x1 = pos1.X - start.X,
                                y1 = pos1.Y - start.Y,
                                z1 = pos1.Z - start.Z,
                                d0 = x0 * x0 + y0 * y0 + z0 * z0,
                                d1 = x1 * x1 + y1 * y1 + z1 * z1;

                            // Вычисляем косинус угла между векторами (pos -> wp0) и (pos -> wp1)
                            // из формулы скалярного произведения векторов
                            double cos = (x0 * x1 + y0 * y1 + z0 * z1) / Math.Sqrt(d0 * d1);

                            if (cos > 0 /*|| d1 > d0 * 1.21*/ || (z1 > 0 && z1 * z1 / d1 > 0.25))
                            {
                                // угол между векторами (from -> wp0) и (from -> wp1) меньше 90 градусов,
                                // либо расстояние до обеих точек различается более чем на 10%,
                                // крутизна угла на wp1 больше 30 град. к горизонту Oxy (z1 *z1 / d1 определяет квадрат синуса угла между вектором (from -> wp1) и его проектцией на Oxy)
                                // поэтому добавляем в путь обе точки
                                waypoints.Add(wp0.Position);
                                waypoints.Add(wp1.Position);
                            }
                            else
                            {
                                // угол между направлениями из точки from на точки wp0 и wp1
                                // больше 90 градусов, поэтому точку wp0 нужно "пропустить" (чтобы не возвращаться "назад")
                                waypoints.Add(wp1.Position);
                            }
                        }
                        else
                        {
                            waypoints.Add(wp0.Position);
                            waypoints.Add(wp1.Position);
                        }

                        while (wpe.MoveNext())
                        {
                            waypoints.Add(wpe.Current.Position);
                        }
                    }
                    else waypoints.Add(wp0.Position);
                }
            }
            waypoints.Add(end);
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
            return waypoints.Count > 1;
        }
        #endregion

        #region GenerateRoad
#if false   // public static Road Astral.Logic.Navmesh.GenerateRoad(Graph graph, Vector3 Start, Vector3 End, bool allowPathFinding)
public static Road Astral.Logic.Navmesh.GenerateRoad(Graph graph, Vector3 Start, Vector3 End, bool allowPathFinding)
{
	string obj = "AddPointWork";
	Road result;
	lock (obj)
	{
		Road road = new Road();
		if (Class1.CurrentSettings.UsePathfinding3)
		{
			List<Vector3> list = new List<Vector3>();
			if (allowPathFinding || graph.Nodes.Count == 0)
			{
				list = PathFinding.GetPathAndCorrect(Start, End, Class1.CurrentSettings.AllowPartialPath);
			}
			if (list.Count > 0)
			{
				road.Type = Road.RoadGenType.PathFinding;
				road.Waypoints = list;
			}
			else
			{
				List<Vector3> list2 = new List<Vector3>();
				if (graph != null && graph.Nodes.Count > 0 && !BotClient.Client.tcpClient_0.Connected)
				{
					Vector3 nearestNodePosFromPosition = Navmesh.GetNearestNodePosFromPosition(graph, Start);
					if (nearestNodePosFromPosition.Distance3D(Start) < 130.0)
					{
						road.Type = Road.RoadGenType.StandardGraph;
						list2 = Navmesh.getPath(graph, nearestNodePosFromPosition, End);
					}
				}
				if (list2.Count == 0 && BotClient.Client.tcpClient_0.Connected)
				{
					road.Type = Road.RoadGenType.StandardGraph;
					list2 = BotClient.QueryPathToServer(Start, End);
				}
				if (list2.Count == 0)
				{
					road.Type = Road.RoadGenType.GoldenPathGraph;
					list2 = Navmesh.GetPathAndCorrect(GoldenPath.GetCurrentMapGraphFromCache(), Start, End);
				}
				if (list2.Count > 0)
				{
					Vector3 vector = Vector3.Empty;
					if (PathFinding.CheckDirection(Start, list2[0], ref vector))
					{
						List<Vector3> path = PathFinding.GetPathAndCorrect(Start, list2[0], true);
						road.Waypoints.AddRange(path);
					}
					road.Waypoints.AddRange(list2);
				}
				else
				{
					List<Vector3> path2 = PathFinding.GetPathAndCorrect(Start, End, true);
					road.Type = Road.RoadGenType.PartialPathFinding;
					road.Waypoints = path2;
				}
			}
		}
		else if (graph == null)
		{
			road.Waypoints.Add(End.Clone());
		}
		else
		{
			road.Waypoints = Navmesh.GetPathAndCorrect(graph, Start, End);
		}
		result = road;
	}
	return result;
}
#endif
        private static readonly object @lock = new object();

        internal static Road GenerateRoad(Graph graph, Vector3 start, Vector3 end, bool allowPathFinding)
        {
            PrefixGenerateRoad(out Road road, graph, start, end, allowPathFinding);
            return road;
        }
        private static bool PrefixGenerateRoad(out Road __result, Graph graph, Vector3 Start, Vector3 End, bool allowPathFinding)
        {
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

            if (Start != null && Start.IsValid
                && End != null && End.IsValid)
            {
#if false
                string obj = "AddPointWork";
                lock (obj)
#elif true

                double distanceStart2End = Start.Distance3D(End);
                double distanceStart2StartNode = 0, distanceEnd2EndNode = 0;
                Node startNode = null, endNode = null;

                bool graphOK = graph?.NodesCount > 0;
                if (graphOK)
                    graph.ClosestNodes(Start.X, Start.Y, Start.Z, out distanceStart2StartNode, out startNode,
                        End.X, End.Y, End.Z, out distanceEnd2EndNode, out endNode);

                lock (@lock)
#endif
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

                        if (waypoints?.Count > 0)
                        {
                            road.Type = Road.RoadGenType.PathFinding;
                            road.Waypoints = waypoints;
                        }
                        else
                        {
                            if (waypoints is null)
                                waypoints = new List<Vector3>();

                            bool tcpClientConnected = AstralAccessors.Controllers.BotComs.BotClient.Client_TcpClient.Connected;
                            if (graphOK && !tcpClientConnected)
                            {
#if PATCH_LOG
                                standardGraphSearching_Time = stopwatch.ElapsedTicks; 
#endif
#if false
                                if (dist1 < 130.0 && !ReferenceEquals(startNode, endNode)) 
#else
                                if (distanceStart2End > distanceStart2StartNode + distanceEnd2EndNode && !ReferenceEquals(startNode, endNode))
#endif
                                {
                                    using (graph.ReadLock())
                                    {
                                        road.Type = Road.RoadGenType.StandardGraph;
                                        GetPathAndCorrect(graph, startNode, Start, endNode, End, ref waypoints);
                                    }
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
                            }

                            if (waypoints.Count == 0)
                            {
#if PATCH_LOG
                                goldenPathSearch_Time = stopwatch.ElapsedTicks; 
#endif
                                road.Type = Road.RoadGenType.GoldenPathGraph;
                                var goldGraph = Astral.Logic.NW.GoldenPath.GetCurrentMapGraphFromCache();
                                using (goldGraph.ReadLock())
                                {
                                    goldGraph.ClosestNodes(Start.X, Start.Y, Start.Z, out double _, out startNode,
                                                           End.X, End.Y, End.Z, out _, out endNode);
                                    GetPathAndCorrect(goldGraph, startNode, Start, endNode, End, ref waypoints);
                                }
#if PATCH_LOG
                                goldenPathSearch_Time = stopwatch.ElapsedTicks - goldenPathSearch_Time;
                                stringBuilder.Append("Search GoldenPathSearch {").Append(Start).Append("} => {").Append(End).AppendLine("}")
                                    .Append("\tSearch time ").AppendLine(goldenPathSearch_Time.ToString()); 
#endif
                            }

                            if (waypoints.Count > 0)
                            {
#if PATCH_LOG
                                partialPathSearch_Time = stopwatch.ElapsedTicks; 
#endif
                                var vector = Vector3.Empty;
                                if (PathFinding.CheckDirection(Start, waypoints[0], ref vector))
                                {
                                    List<Vector3> path = PathFinding.GetPath(Start, waypoints[0], true);
#if false
                                    road.Waypoints.AddRange(path);
                                }

                                road.Waypoints.AddRange(waypoints);  
#else
                                    waypoints.InsertRange(0, path);
                                }
                                road.Waypoints = waypoints;
#endif
#if PATCH_LOG
                                partialPathSearch_Time = stopwatch.ElapsedTicks - partialPathSearch_Time;
                                stringBuilder.Append("Search PartialPath from {").Append(Start).Append("} => {").Append(waypoints[0]).AppendLine("}")
                                    .Append("\tSearch time ").AppendLine(partialPathSearch_Time.ToString()); 
#endif
                            }
                            else
                            {
#if PATCH_LOG
                                partialPathFinding_Time = stopwatch.ElapsedTicks; 
#endif
                                var path = PathFinding.GetPath(Start, End, true);
                                road.Type = Road.RoadGenType.PartialPathFinding;
                                road.Waypoints = path;
#if PATCH_LOG
                                partialPathFinding_Time = stopwatch.ElapsedTicks - partialPathFinding_Time;
                                stringBuilder.Append("PartialPathFinding from {").Append(Start).Append("} => {").Append(End).AppendLine("}")
                                    .Append("\tSearch time ").AppendLine(partialPathFinding_Time.ToString()); 
#endif
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
                            road.Waypoints = GetPath(graph, Start, End);
                        }
#if PATCH_LOG
                        directPathFinding_Time = stopwatch.ElapsedTicks - directPathFinding_Time;
                        stringBuilder.Append("DirectPathFinding from {").Append(Start).Append("} => {").Append(End).AppendLine("}")
                            .Append("\tSearch time ").AppendLine(directPathFinding_Time.ToString()); 
#endif
                    }
                    else
                    {
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
            __result = road;

            return false;
        }
        #endregion

        #region GetNearestNodeFromPosition
#if false   // public static Node Astral.Logic.Navmesh.GetNearestNodeFromPosition(Graph graph, Vector3 pos)
public static Node Astral.Logic.Navmesh.GetNearestNodeFromPosition(Graph graph, Vector3 pos)
{
	double num = double.MaxValue;
	Node result = new Node(0.0, 0.0, 0.0);
	if (graph == null)
	{
		return new Node((double)pos.X, (double)pos.Y, (double)pos.Z);
	}
	foreach (object obj in graph.Nodes)
	{
		Node node = (Node)obj;
		if (node.Passable)
		{
			Vector3 vector = new Vector3(pos.X, pos.Y, pos.Z);
			Vector3 from = new Vector3((float)node.X, (float)node.Y, (float)node.Z);
			double num2 = vector.Distance3D(from);
			if (num2 < num)
			{
				num = num2;
				result = node;
			}
		}
	}
    return result;
}
#endif
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
#if false   // private static void Astral.Logic.Navmesh.FixPath(List<Vector3> waypoints, Vector3 from)
private static void Astral.Logic.Navmesh.FixPath(List<Vector3> waypoints, Vector3 from)
{
	int num = 0;
	while (waypoints.Count > 1)
	{
		num++;
		if (num > 3)
		{
			return;
		}
		double num2 = waypoints[0].Distance3D(from) + waypoints[0].Distance3D(waypoints[1]);
		if (waypoints[1].Distance3D(from) * 1.1 <= num2)
		{
			waypoints.RemoveAt(0);
		}
	}
}
#endif
        // ReSharper disable once UnusedParameter.Local
        private static void PrefixFixPath(List<Vector3> waypoints, Vector3 from) { }
        #endregion

        #region TotalDistance
#if false   // public static double Astral.Logic.Navmesh.TotalDistance(List<Vector3> positions)
public static double Astral.Logic.Navmesh.TotalDistance(List<Vector3> positions)
{
	if (positions.Count <= 1)
	{
		return 0.0;
	}
	double num = positions[0].Distance3D(positions[1]);
	for (int i = 1; i < positions.Count; i++)
	{
		num += positions[i - 1].Distance3D(positions[i]);
	}
	return num;
}
#endif
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
#if false //Astral.Quester.Controllers.Road.PathDistance(Vector3 pos)
internal static double Astral.Quester.Controllers.Road.PathDistance(Vector3 pos)
{
	Road road = Road.GenerateRoadFromPlayer(pos);
	road.Waypoints.Insert(0, \u0001.LocalPlayer.Location);
	return Navmesh.TotalDistance(road.Waypoints);
}
#endif
        internal static bool PathDistance(out double __result, Vector3 pos)
        {
            if (pos != null && pos.IsValid)
            {
#if true
                var playerLocation = EntityManager.LocalPlayer.Location;
                var graph = AstralAccessors.Controllers.Roles.CurrentRole.UsedMeshes;

                if (graph?.NodesCount > 0)
                {
                    var road = GenerateRoad(graph,
                        playerLocation, pos, !Astral.API.CurrentSettings.PFOnlyForApproaches);

                    if (road.Waypoints.Count > 1)
                    {
                        __result = MathHelper.Distance3D(playerLocation, road.Waypoints[0]);
                        __result += TotalDistance(road.Waypoints);
                    }
                    else __result = pos.Distance3DFromPlayer;
                }
                else __result = pos.Distance3D(playerLocation);
#endif
            }
            else __result = 0;//double.MaxValue;

            return false;
        }
        #endregion

        #region GenerateRoadFromPlayer
#if false
		internal static Road Astral.Quester.Controllers.Road.GenerateRoadFromPlayer(Vector3 end)
		{
			return Navmesh.GenerateRoadFromPlayer(Core.Meshes, end, !\u0001.CurrentSettings.PFOnlyForApproaches);
		}
#endif
        internal static bool GenerateRoadFromPlayer(out Astral.Logic.Classes.FSM.Road __result, Vector3 end)
        {
            if (end?.IsValid == true)
            {
                var graph = AstralAccessors.Controllers.Roles.CurrentRole.UsedMeshes;
                var playerLocation = EntityManager.LocalPlayer.Location;
                __result = GenerateRoad(graph,
                    playerLocation, end, !Astral.API.CurrentSettings.PFOnlyForApproaches);

            }
            else __result = new Astral.Logic.Classes.FSM.Road();

            return false;
        } 
        #endregion
    }
}
