using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using AStar;
using Astral.Logic.Classes.FSM;
using Astral.Logic.NW;
using EntityTools.Patches.Mapper;
using EntityTools.Reflection;
using MyNW.Classes;

namespace EntityTools.Patches.Navmesh
{
    internal class Patch_Astral_Logic_Navmesh_GenerateRoad : Patch
    {
        internal Patch_Astral_Logic_Navmesh_GenerateRoad()
        {
            if (NeedInjecttion)
            {
                MethodInfo mi = typeof(Astral.Logic.Navmesh).GetMethod("GenerateRoad", ReflectionHelper.DefaultFlags);
                if (mi != null)
                {
                    methodToReplace = mi;
                }
                else throw new Exception("Patch_Astral_Logic_Navmesh_GenerateRoad: fail to initialize 'methodToReplace'");

                methodToInject = GetType().GetMethod(nameof(GenerateRoad), ReflectionHelper.DefaultFlags);
            }
        }

        public sealed override bool NeedInjecttion => EntityTools.Config.Patches.Navigation;

#if PATCH_LOG
        private static StringBuilder stringBuilder = new StringBuilder();

        private static Stopwatch stopwatch = new Stopwatch(); 
#endif

        private static readonly object @lock = new object();


#if false
// Astral.Logic.Navmesh
// Token: 0x060011DA RID: 4570 RVA: 0x0005CF50 File Offset: 0x0005B150
public static Road GenerateRoad(Graph graph, Vector3 Start, Vector3 End, bool allowPathFinding)
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
					Vector3 vector = new Vector3();
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
        internal static Road GenerateRoad(Graph graph, Vector3 start, Vector3 end, bool allowPathFinding)
        {
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

            if (start != null && start.IsValid 
                && end != null && end.IsValid)
            {
#if false
                string obj = "AddPointWork";
                lock (obj)
#elif true
                lock(@lock)
#endif
                {
#if PATCH_LOG
                    stopwatch.Start(); 
#endif
                    if (Astral.Controllers.Settings.Get.UsePathfinding3)
                    {
                        List<Vector3> waypoints = new List<Vector3>();
                        if (allowPathFinding || graph.NodesCount == 0)
                        {
#if PATCH_LOG
                            pathFindingOrEmptyGraph_Time = stopwatch.ElapsedTicks; 
#endif
                            waypoints = PathFinding.GetPath(start, end,
                                Astral.Controllers.Settings.Get.AllowPartialPath);
#if PATCH_LOG
                            pathFindingOrEmptyGraph_Time = stopwatch.ElapsedTicks - pathFindingOrEmptyGraph_Time;
                            stringBuilder.Append("Search PathFindingOrEmptyGraph {").Append(start).Append("} => {").Append(end).AppendLine("}")
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
                            if (graph?.NodesCount > 0 && !tcpClientConnected)
                            {
                                double dist1 = 0, dist2 = 0;
                                Node startNode = null, endNode = null;
#if PATCH_LOG
                                standardGraphSearching_Time = stopwatch.ElapsedTicks; 
#endif
                                using (graph.ReadLock())
                                {
                                    //Vector3 nearestNodePosFromPosition = Navmesh.GetNearestNodePosFromPosition(graph, start);
                                    graph.ClosestNodes(start.X, start.Y, start.Z, out dist1, out startNode,
                                        end.X, end.Y, end.Z, out dist2, out endNode);

                                    if (dist1 < 130.0 && !ReferenceEquals(startNode, endNode))
                                    {
                                        road.Type = Road.RoadGenType.StandardGraph;
                                        Patch_Astral_Logic_Navmesh_GetPath.GetPathAndCorrect(graph, startNode, start, endNode, end, ref waypoints);
                                    }
                                }
#if PATCH_LOG
                                standardGraphSearching_Time = stopwatch.ElapsedTicks - standardGraphSearching_Time;
                                stringBuilder.AppendLine("Search StandardGraph")
                                             .Append("\tStart pos {").Append(start).Append("} to Node {")
                                                    .Append(startNode).Append("} at the distance ").AppendLine(dist1.ToString())
                                             .Append("\tEnd pos {").Append(end).Append("} to Node {")
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
                                waypoints = Astral.Controllers.BotComs.BotClient.QueryPathToServer(start, end);
#if PATCH_LOG
                                queryPathToServer_Time = stopwatch.ElapsedTicks - queryPathToServer_Time;
                                stringBuilder.Append("Search QueryPathToServer {").Append(start).Append("} => {").Append(end).AppendLine("}")
                                    .Append("\tSearch time ").AppendLine(queryPathToServer_Time.ToString()); 
#endif
                            }

                            if (waypoints.Count == 0)
                            {
#if PATCH_LOG
                                goldenPathSearch_Time = stopwatch.ElapsedTicks; 
#endif
                                road.Type = Road.RoadGenType.GoldenPathGraph;
                                var goldGraph = GoldenPath.GetCurrentMapGraphFromCache();
                                using (goldGraph.ReadLock())
                                {
                                    goldGraph.ClosestNodes(start.X, start.Y, start.Z, out double _, out Node startNode,
                                                           end.X, end.Y, end.Z, out _, out Node endNode);
                                    Patch_Astral_Logic_Navmesh_GetPath.GetPathAndCorrect(goldGraph, startNode, start, endNode, end, ref waypoints);
                                }
#if PATCH_LOG
                                goldenPathSearch_Time = stopwatch.ElapsedTicks - goldenPathSearch_Time;
                                stringBuilder.Append("Search GoldenPathSearch {").Append(start).Append("} => {").Append(end).AppendLine("}")
                                    .Append("\tSearch time ").AppendLine(goldenPathSearch_Time.ToString()); 
#endif
                            }

                            if (waypoints.Count > 0)
                            {
#if PATCH_LOG
                                partialPathSearch_Time = stopwatch.ElapsedTicks; 
#endif
                                Vector3 vector = new Vector3();
                                if (PathFinding.CheckDirection(start, waypoints[0], ref vector))
                                {
                                    List<Vector3> path = PathFinding.GetPath(start, waypoints[0], true);
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
                                stringBuilder.Append("Search PartialPath from {").Append(start).Append("} => {").Append(waypoints[0]).AppendLine("}")
                                    .Append("\tSearch time ").AppendLine(partialPathSearch_Time.ToString()); 
#endif
                            }
                            else
                            {
#if PATCH_LOG
                                partialPathFinding_Time = stopwatch.ElapsedTicks; 
#endif
                                List<Vector3> path = PathFinding.GetPath(start, end, true);
                                road.Type = Road.RoadGenType.PartialPathFinding;
                                road.Waypoints = path;
#if PATCH_LOG
                                partialPathFinding_Time = stopwatch.ElapsedTicks - partialPathFinding_Time;
                                stringBuilder.Append("PartialPathFinding from {").Append(start).Append("} => {").Append(end).AppendLine("}")
                                    .Append("\tSearch time ").AppendLine(partialPathFinding_Time.ToString()); 
#endif
                            }
                        }
                    }
                    else if (graph is null)
                    {
                        road.Waypoints.Add(end.Clone());
                    }
                    else
                    {
#if PATCH_LOG
                        directPathFinding_Time = stopwatch.ElapsedTicks; 
#endif
                        using (graph.ReadLock())
                        {
                            road.Waypoints = Patch_Astral_Logic_Navmesh_GetPath.GetPath(graph, start, end);
                        }
#if PATCH_LOG
                        directPathFinding_Time = stopwatch.ElapsedTicks - directPathFinding_Time;
                        stringBuilder.Append("DirectPathFinding from {").Append(start).Append("} => {").Append(end).AppendLine("}")
                            .Append("\tSearch time ").AppendLine(directPathFinding_Time.ToString()); 
#endif
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

            return road;
        }
    }
}
