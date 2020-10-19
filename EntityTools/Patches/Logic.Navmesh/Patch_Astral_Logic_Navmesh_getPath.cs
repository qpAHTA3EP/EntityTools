using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using AStar;
using EntityTools.Reflection;
using MyNW.Classes;

namespace EntityTools.Patches.Navmesh
{
    internal class Patch_Astral_Logic_Navmesh_getPath : Patch
    {
        internal Patch_Astral_Logic_Navmesh_getPath()
        {
            MethodInfo mi = typeof(Astral.Logic.Navmesh).GetMethod("getPath", ReflectionHelper.DefaultFlags);
            if (mi != null)
            {
                methodToReplace = mi;
            }
            else throw new Exception("Patch_Astral_Logic_Navmesh_getPath: fail to initialize 'methodToReplace'");

            methodToInject = GetType().GetMethod(nameof(GetPath), ReflectionHelper.DefaultFlags, null, new Type[]{ typeof(Graph), typeof(Vector3), typeof(Vector3)}, null);
        }

#if PATCH_LOG
        private static StringBuilder stringBuilder = new StringBuilder();
        private static Stopwatch stopwatch = new Stopwatch(); 
#endif

#if false
    Astral.Logic.Navmesh
    	private static List<Vector3> GetPath(Graph graph, Vector3 Start, Vector3 End)
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

        internal static List<Vector3> GetPath(Graph graph, Vector3 start, Vector3 end)
        {
            List<Vector3> waypoints = new List<Vector3>();
            if (graph != null)
            {
                graph.ClosestNodes(start.X, start.Y, start.Z, out double dist1, out Node startNode,
                    end.X, end.Y, end.Z, out double dist2, out Node endNode);

                GetPath(graph, startNode, endNode, ref waypoints);
            }
            return waypoints;
        }

        internal static bool GetPath(Graph graph, Node startNode, Node endNode, ref List<Vector3> waypoints)
        {
            if(waypoints is null)
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
                        stringBuilder.AppendLine(nameof(Patch_Astral_Logic_Navmesh_getPath));
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
    }
}
