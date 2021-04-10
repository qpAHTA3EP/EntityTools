using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using AStar;
using AcTp0Tools.Reflection;
using MyNW.Classes;

namespace EntityTools.Patches.Navmesh
{
    internal class Patch_Astral_Logic_Navmesh_GetPath : Patch
    {
        internal Patch_Astral_Logic_Navmesh_GetPath()
        {
            if (!NeedInjecttion) return;

            MethodInfo mi = typeof(Astral.Logic.Navmesh).GetMethod("getPath", ReflectionHelper.DefaultFlags);
            if (mi != null)
            {
                methodToReplace = mi;
            }
            else throw new Exception("Patch_Astral_Logic_Navmesh_GetPath: fail to initialize 'methodToReplace'");

            methodToInject = GetType().GetMethod(nameof(GetPath), ReflectionHelper.DefaultFlags, null, new Type[] { typeof(Graph), typeof(Vector3), typeof(Vector3) }, null);
        }

        public sealed override bool NeedInjecttion => EntityTools.Config.Patches.Navigation;

#if PATCH_LOG
        private static StringBuilder stringBuilder = new StringBuilder();
        private static Stopwatch stopwatch = new Stopwatch(); 
#endif

#if false
    Astral.Logic.Navmesh
    	private static List<Vector3> GetPathAndCorrect(Graph graph, Vector3 Start, Vector3 End)
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

                GetPathAndCorrect(graph, startNode, start, endNode, end, ref waypoints);
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
                    var wp0 = wpe.Current;
                    if (wpe.MoveNext())
                    {
                        var wp1 = wpe.Current;
                        if(start!= null && start.IsValid)
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
                    else if(wp0 != null)
                        waypoints.Add(wp0.Position);
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
    }
}
