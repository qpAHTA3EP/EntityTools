//#define PROFILING
#define LOG
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using AStar;
using Astral;
using MyNW.Classes;
using EntityTools.Tools;
using System.Threading.Tasks;
using System.Diagnostics;
using EntityTools.Logger;
//using NodeDistPair = EntityTools.Tools.Pair<AStar.Node, double>;

namespace EntityTools.Patches.Mapper
{
    internal static class AddNavigationNodeAdvanced
    {
#if PROFILING && DEBUG
        internal static int WatchCounter = 0;
        internal static long totalWatchTiks = 0;
        internal static long totalWatchMs = 0;
        internal static Stopwatch stopwatch = new Stopwatch();
        internal static void ResetWatch()
        {
            WatchCounter = 0;
            totalWatchTiks = 0;
            totalWatchMs = 0;
        }

        internal static void LogWatch()
        {
            if (WatchCounter != 0)
            {
                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::WatchCounter: {WatchCounter}");
                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::TotalWatchTiks: {totalWatchTiks}");
                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::TotalWatchMs: {totalWatchMs}");
                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::Average: {(totalWatchMs / WatchCounter).ToString("N3")} ms; {(totalWatchTiks / WatchCounter).ToString("N0")} Ticks");
            }

        }
#endif
        public static NodeDetail LinkNearest3Side(Vector3 pos, MapperGraphCache Graph, NodeDetail lastND = null, bool uniDirection = false)
        {
            if (Graph == null || pos == null || !pos.IsValid)
                return null;

            if (lastND == null)
                return LinkNearest1(pos, Graph, null, uniDirection);

            string obj = "AddNavigationNode.LinkNearest3Side";
            lock (obj)
            {
#if PROFILING && DEBUG
                stopwatch.Restart();
#endif
                Node newNode = new Node(pos.X, pos.Y, pos.Z);

                //double minDistance = (double)EntityTools.PluginSettings.Mapper.WaypointDistance / 2d;
                double maxDistance = Math.Max(25d, (double)EntityTools.PluginSettings.Mapper.WaypointDistance * 1.25d);
                double maxZDifference = EntityTools.PluginSettings.Mapper.MaxElevationDifference;
                double equivalenceDistance = EntityTools.PluginSettings.Mapper.WaypointEquivalenceDistance;

                // Ближайшие узлы разделенные по направлению движения
                NodeDetail frontND = null; // впереди 
                NodeDetail leftND = null; // слева    
                NodeDetail rightND = null; // справа
                NodeDetail backND = null;
                // Косое произведение веторов A и B
                // D = Ax * By - Bx * Ay
                // Если D == 0 - вектора коллиенарны
                // Если D > 0 - точека B в верхней полуплоскости
                // Если D < 0 - точека B в нижней полуплоскости

                try
                {
                    // сканируем узлы графа, для определения ближайших к добавляемому узлу newNode
                    //double minNodeDistance = double.MaxValue; // расстояние до ближайшего узла
                    //int closestNodeInd = -1; // Индекс ближайшего узла в списке nearestNodeList
                    //bool isEquivalent = false; // флаг, указывающий, что newNode был заменена на эквивалентный существующий узел
                    NodeDetail equivalentND = null;
                    for (int i = 0; i < Graph.Nodes.Count; i++)
                    {
                        if (!(Graph.Nodes[i] is Node currentNode)
                            ||NodesEquals(lastND.Node, currentNode))
                            continue;

                        //Проверяем разницу высот
                        NodeDetail curNodeDet = new NodeDetail(currentNode, newNode);
                        if (curNodeDet.Vector.Z * Math.Sign(curNodeDet.Vector.Z) < maxZDifference)
                        {
                            // Разница высот в пределах допустимой величины,
                            // проверяем расстояние от добавляемой точки до текущей
                            if (curNodeDet.Distance < equivalenceDistance)
                            {
#if SWAP_NEW2EQUIVALENT
                                // В графе есть точка, расстояние до которой меньше equivalenceDistance
                                // подменяем newNode на currentNode
                                newNode = currentNode;

                                isEquivalent = true;
                                minNodeDistance = double.MaxValue;
                                closestNodeInd = -1;

#if LINK_EQUIVALENT_ToALL
                                // Вариант, при котором эквивалентный узел связывается с другими "близкими" узлами
                                // пересчитываем расстояния для узлов, добавленных в список nearestNodeList
                                // одновременно с этим производит повторный поиск ближайшего узла к newNode = currentNode;
                                for ( int ind = 0; ind < nearestNodeList.Count; ind++)
                                {
                                    nearestNodeList[ind].Rebase(newNode);
                                    if(nearestNodeList[ind].Distance < minNodeDistance)
                                    {
                                        minNodeDistance = nearestNodeList[ind].Distance;
                                        closestNodeInd = ind;
                                    }
                                }
#else
                                // Вариант при котором эквивалентный узел связывается ТОЛЬКО с lastNode
                                // Очищаем список ближайших узлов
                                nearestNodeList.Clear();
                                // прерываем поиск
                                break;
#endif
#else
                                // Запоминаем существующий узел, эквивалентный "новому узлу"
                                if(equivalentND == null || curNodeDet.Distance < equivalentND.Distance)
                                    equivalentND = curNodeDet;
#endif
                            }
                            else if (curNodeDet.Distance <= maxDistance)
                            {
                                // вычисляем коссинус угла меду векторами для определения четверти (впереди, сзади, слева, справа)
                                // в которой находится curNode
                                double cos = CosinusXY(lastND, curNodeDet);

                                if(cos  < -0.7071d)
                                {
                                    // Угол между векторами больше 135 градусов
                                    // Значит точка находится в переди
                                    if (frontND == null
                                        || frontND.Distance > curNodeDet.Distance)
                                        frontND = curNodeDet;
                                }
                                else if(cos < 0.7071d)
                                {
                                    // Угол между веторками в диапазоне 45~125 градусов

                                    // Вычисляем в какой полуплоскости находится curNode
                                    // относительно вектора lastNodeDetail.Vector
                                    // который направлен от NewNode к lastNode
                                    double D = lastND.Vector.X * curNodeDet.Vector.Y - curNodeDet.Vector.X * lastND.Vector.Y;
                                    
                                    if(D > 0)
                                    {
                                        if (rightND == null
                                            || rightND.Distance > curNodeDet.Distance)
                                            rightND = curNodeDet;
                                    }
                                    else
                                    {
                                        if (leftND == null
                                            || leftND.Distance > curNodeDet.Distance)
                                            leftND = curNodeDet;
                                    }
                                }
                                else if(lastND == null)
                                {
                                    if (backND == null
                                        || backND.Distance > curNodeDet.Distance)
                                        backND = curNodeDet;
                                }
                            }
                        }
                    }

                    // Формирование функтора добавление граней, в зависимости от типа пути (одно- или двунаправленный)
                    Action<Node, Node, double> AddArcFunc;
                    if (uniDirection)
                        AddArcFunc = (n1, n2, w) => Graph.AddArc(n1, n2, w);
                    else AddArcFunc = (n1, n2, w) => Graph.Add2Arcs(n1, n2, w);

                    if (equivalentND != null)
                    {
#if DEBUG && LOG
                        EntityToolsLogger.WriteLine(LogType.Debug, $"AddNavigationNodeStatic::LinkNearest3Side: Detect 'EQUIVALENT' Node <{equivalentND.Node.X.ToString("N3")}{equivalentND.Node.Y.ToString("N3")}{equivalentND.Node.Z.ToString("N3")}>");
#endif
                        // Добавляем связь от узла equivalentNodeDetail 
                        // к узлам front, left, roght
                        if (frontND != null)
                        {
                            frontND.Rebase(equivalentND.Node);
                            AddArcFunc(equivalentND.Node, frontND.Node, frontND.Distance);
#if DEBUG && LOG
                            if(uniDirection)
                                EntityToolsLogger.WriteLine(LogType.Debug, $"AddNavigationNodeStatic::LinkNearest3Side: UniLink {{equivalent -> front}} <{frontND.Node.X.ToString("N3")}{frontND.Node.Y.ToString("N3")}{frontND.Node.Z.ToString("N3")}>");
                            else EntityToolsLogger.WriteLine(LogType.Debug, $"AddNavigationNodeStatic::LinkNearest3Side: BiLink {{equivalent <-> front}} <{frontND.Node.X.ToString("N3")}{frontND.Node.Y.ToString("N3")}{frontND.Node.Z.ToString("N3")}>");
#endif
                        }
                        if(leftND != null)
                        {
                            leftND.Rebase(equivalentND.Node);
                            AddArcFunc(leftND.Node, equivalentND.Node, leftND.Distance);
#if DEBUG && LOG
                            if (uniDirection)
                                EntityToolsLogger.WriteLine(LogType.Debug, $"AddNavigationNodeStatic::LinkNearest3Side: UniLink {{left -> equivalent}} <{leftND.Node.X.ToString("N3")}{leftND.Node.Y.ToString("N3")}{leftND.Node.Z.ToString("N3")}>");
                            else EntityToolsLogger.WriteLine(LogType.Debug, $"AddNavigationNodeStatic::LinkNearest3Side: BiLink {{left <-> quivalent}} <{leftND.Node.X.ToString("N3")}{leftND.Node.Y.ToString("N3")}{leftND.Node.Z.ToString("N3")}>");
#endif
                        }
                        if (rightND != null)
                        {
                            rightND.Rebase(equivalentND.Node);
                            AddArcFunc(rightND.Node, equivalentND.Node, rightND.Distance);
#if DEBUG && LOG
                            if (uniDirection)
                                EntityToolsLogger.WriteLine(LogType.Debug, $"AddNavigationNodeStatic::LinkNearest3Side: UniLink {{right -> equivalent}} <{rightND.Node.X.ToString("N3")}{rightND.Node.Y.ToString("N3")}{rightND.Node.Z.ToString("N3")}>");
                            else EntityToolsLogger.WriteLine(LogType.Debug, $"AddNavigationNodeStatic::LinkNearest3Side: BiLink {{right <-> quivalent}} <{rightND.Node.X.ToString("N3")}{rightND.Node.Y.ToString("N3")}{rightND.Node.Z.ToString("N3")}>");
#endif
                        }
                        if (backND != null)
                        {
                            backND.Rebase(equivalentND.Node);
                            AddArcFunc(backND.Node, equivalentND.Node, rightND.Distance);
#if DEBUG && LOG
                            if (uniDirection)
                                EntityToolsLogger.WriteLine(LogType.Debug, $"AddNavigationNodeStatic::LinkNearest3Side: UniLink {{back -> equivalent}} <{backND.Node.X.ToString("N3")}{backND.Node.Y.ToString("N3")}{backND.Node.Z.ToString("N3")}>");
                            else EntityToolsLogger.WriteLine(LogType.Debug, $"AddNavigationNodeStatic::LinkNearest3Side: BiLink {{back <-> quivalent}} <{backND.Node.X.ToString("N3")}{backND.Node.Y.ToString("N3")}{backND.Node.Z.ToString("N3")}>");
#endif
                        }

                        // Если задан узел lastNodeDetail - добавляем связь с ним
                        if (lastND != null)
                        {
                            lastND.Rebase(equivalentND.Node);
                            AddArcFunc(lastND.Node, equivalentND.Node, lastND.Distance);
#if DEBUG && LOG
                            if (uniDirection)
                                EntityToolsLogger.WriteLine(LogType.Debug, $"AddNavigationNodeStatic::LinkNearest3Side: UniLink {{last -> equivalent}} <{lastND.Node.X.ToString("N3")}{lastND.Node.Y.ToString("N3")}{lastND.Node.Z.ToString("N3")}>");
                            else EntityToolsLogger.WriteLine(LogType.Debug, $"AddNavigationNodeStatic::LinkNearest3Side: BiLink {{last <-> equivalent}} <{lastND.Node.X.ToString("N3")}{lastND.Node.Y.ToString("N3")}{lastND.Node.Z.ToString("N3")}>");
#endif
                        }

                        return equivalentND;
                    }
                    else
                    {
                        if (Graph.AddNode(newNode))
                        {
#if DEBUG && LOG
                            EntityToolsLogger.WriteLine(LogType.Debug, $"AddNavigationNodeStatic::LinkNearest3Side: Add 'NEW' node <{newNode.X.ToString("N3")}{newNode.Y.ToString("N3")}{newNode.Z.ToString("N3")}>");
#endif
                            // Добавляем связь от узла equivalentNodeDetail 
                            // к узлам front, left, roght
                            if (frontND != null)
                            {
                                AddArcFunc(newNode, frontND.Node, frontND.Distance);
#if DEBUG && LOG
                                if (uniDirection)
                                    EntityToolsLogger.WriteLine(LogType.Debug, $"AddNavigationNodeStatic::LinkNearest3Side: UniLink {{new -> front}} <{frontND.Node.X.ToString("N3")}{frontND.Node.Y.ToString("N3")}{frontND.Node.Z.ToString("N3")}>");
                                else EntityToolsLogger.WriteLine(LogType.Debug, $"AddNavigationNodeStatic::LinkNearest3Side: BiLink {{new <-> front}} <{frontND.Node.X.ToString("N3")}{frontND.Node.Y.ToString("N3")}{frontND.Node.Z.ToString("N3")}>");
#endif
                            }
                            if (leftND != null)
                            {
                                AddArcFunc(leftND.Node, newNode, leftND.Distance);
#if DEBUG && LOG
                                if (uniDirection)
                                    EntityToolsLogger.WriteLine(LogType.Debug, $"AddNavigationNodeStatic::LinkNearest3Side: UniLink {{left -> new}} <{leftND.Node.X.ToString("N3")}{leftND.Node.Y.ToString("N3")}{leftND.Node.Z.ToString("N3")}>");
                                else EntityToolsLogger.WriteLine(LogType.Debug, $"AddNavigationNodeStatic::LinkNearest3Side: BiLink {{left <-> new}} <{leftND.Node.X.ToString("N3")}{leftND.Node.Y.ToString("N3")}{leftND.Node.Z.ToString("N3")}>");
#endif
                            }
                            if (rightND != null)
                            {
                                AddArcFunc(rightND.Node, newNode, rightND.Distance);
#if DEBUG && LOG
                                if (uniDirection)
                                    EntityToolsLogger.WriteLine(LogType.Debug, $"AddNavigationNodeStatic::LinkNearest3Side: UniLink {{(right -> new}} <{rightND.Node.X.ToString("N3")}{rightND.Node.Y.ToString("N3")}{rightND.Node.Z.ToString("N3")}>");
                                else EntityToolsLogger.WriteLine(LogType.Debug, $"AddNavigationNodeStatic::LinkNearest3Side: BiLink {{(right <-> new}} <{rightND.Node.X.ToString("N3")}{rightND.Node.Y.ToString("N3")}{rightND.Node.Z.ToString("N3")}>");
#endif
                            }
                            if (backND != null)
                            {
                                backND.Rebase(newNode);
                                AddArcFunc(backND.Node, newNode, rightND.Distance);
#if DEBUG && LOG
                                if (uniDirection)
                                    EntityToolsLogger.WriteLine(LogType.Debug, $"AddNavigationNodeStatic::LinkNearest3Side: UniLink {{back -> new}} <{backND.Node.X.ToString("N3")}{backND.Node.Y.ToString("N3")}{backND.Node.Z.ToString("N3")}>");
                                else EntityToolsLogger.WriteLine(LogType.Debug, $"AddNavigationNodeStatic::LinkNearest3Side: BiLink {{back <-> new}} <{backND.Node.X.ToString("N3")}{backND.Node.Y.ToString("N3")}{backND.Node.Z.ToString("N3")}>");
#endif
                            }

                            // Если задан узел lastNodeDetail - добавляем связь с ним
                            if (lastND != null)
                            {
                                AddArcFunc(lastND.Node, newNode, lastND.Distance);
#if DEBUG && LOG
                                if (uniDirection)
                                    EntityToolsLogger.WriteLine(LogType.Debug, $"AddNavigationNodeStatic::LinkNearest3Side: UniLink {{last -> new}} <{lastND.Node.X.ToString("N3")}{lastND.Node.Y.ToString("N3")}{lastND.Node.Z.ToString("N3")}>");
                                else EntityToolsLogger.WriteLine(LogType.Debug, $"AddNavigationNodeStatic::LinkNearest3Side: BiLink {{last <-> new}} <{lastND.Node.X.ToString("N3")}{lastND.Node.Y.ToString("N3")}{lastND.Node.Z.ToString("N3")}>");
#endif
                            }
                            return new NodeDetail(newNode);
                        }
                        else
                        {
#if DEBUG && LOG
                            EntityToolsLogger.WriteLine(LogType.Debug, $"AddNavigationNodeStatic::LinkNearest3Side: Fail to add Node <{newNode.X.ToString("N3")}, {newNode.Y.ToString("N3")}, {newNode.Z.ToString("N3")}>");
#endif
                            return lastND;
                        }
                    }
                }
                catch (Exception ex)
                {
#if DEBUG && LOG
                    EntityToolsLogger.WriteLine(LogType.Debug, ex.ToString());
#endif
                    return null;
                }
#if PROFILING && DEBUG
                finally
                {
                    stopwatch.Stop();
                    WatchCounter++;
                    totalWatchTiks += stopwatch.ElapsedTicks;
                    totalWatchMs += stopwatch.ElapsedMilliseconds;
#if DEBUG && LOG
                    EntityToolsLogger.WriteLine(LogType.Debug, $"AddNavigationNodeStatic::LinkNearest3Side: ElapsedTime is {stopwatch.ElapsedMilliseconds} ms ({stopwatch.ElapsedTicks})");
#endif
                }
#endif
            }
            return null;
        }

        public static NodeDetail LinkLast(Vector3 pos, MapperGraphCache Graph, NodeDetail lastND = null, bool uniDirection = false)
        {
            if (Graph == null || pos == null || !pos.IsValid)
                return null;

            string obj = "AddNavigationNodeStatic.LinkLast";
            lock (obj)
            {
#if PROFILING && DEBUG
                stopwatch.Restart();
#endif
                double minNodeDistance = EntityTools.PluginSettings.Mapper.WaypointDistance;
                double maxDistance = Math.Max(25d, (double)EntityTools.PluginSettings.Mapper.WaypointDistance * 1.25d);
                double maxZDifference = EntityTools.PluginSettings.Mapper.MaxElevationDifference;
                double equivalenceDistance = EntityTools.PluginSettings.Mapper.WaypointEquivalenceDistance;

                Node newNode = new Node(pos.X, pos.Y, pos.Z);

                NodeDetail equivalentND = null;

                try
                {
                    // сканируем узлы графа, для определения ближайшего
                    for (int i = 0; i < Graph.Nodes.Count; i++)
                    {
                        //Проверяем разницу высот
                        if (Graph.Nodes[i] is Node iNode)
                        {
                            NodeDetail currentND = new NodeDetail(iNode, newNode);
                            if (Math.Sign(currentND.Vector.Z) * currentND.Vector.Z < maxZDifference)
                            {
                                // Разница высот в пределах допустимой величины,
                                // проверяем расстояние от добавляемой точки до текущей
                                if (currentND.Distance < equivalenceDistance)
                                {
                                    // NewNode CurNode в пределах equivalenceDistance
                                    if (equivalentND == null
                                        || equivalentND.Distance > currentND.Distance)
                                        equivalentND = currentND;
                                }
                                /*else if (currentND.Distance <= minNodeDistance
                                         && (nearestND == null || currentND.Distance < nearestND.Distance))
                                {
                                    // curNodeDet - близажший существующий узел к NewNode
                                    nearestND = currentND;
                                }*/
                            }
                        }
                    }

                    if (equivalentND != null)
                    {
                        if (lastND != null && lastND.Node != null)
                        {
                            lastND.Rebase(equivalentND.Node);
#if DEBUG && LOG
                            EntityToolsLogger.WriteLine(LogType.Debug, $"AddNavigationNodeStatic::LinkLast: Detect 'EQUIVALENT' Node <{equivalentND.Node.X.ToString("N3")}{equivalentND.Node.Y.ToString("N3")}{equivalentND.Node.Z.ToString("N3")}>");
#endif
                            if (uniDirection)
                            {
                                Graph.AddArc(lastND.Node, equivalentND.Node, lastND.Distance);
#if DEBUG && LOG
                                EntityToolsLogger.WriteLine(LogType.Debug, $"AddNavigationNodeStatic::LinkLast: UniLink {{last -> quivalent}} <{lastND.Node.X.ToString("N3")}{lastND.Node.Y.ToString("N3")}{lastND.Node.Z.ToString("N3")}>");
#endif
                            }
                            else
                            {
                                Graph.Add2Arcs(lastND.Node, equivalentND.Node, lastND.Distance);
#if DEBUG && LOG
                                EntityToolsLogger.WriteLine(LogType.Debug, $"AddNavigationNodeStatic::LinkLast: BiLink {{last <-> quivalent}} <{lastND.Node.X.ToString("N3")}{lastND.Node.Y.ToString("N3")}{lastND.Node.Z.ToString("N3")}>");
#endif
                            }
                        }
                        else
                        {
                            // предыдущий узел "отсутствует"
                            // связывать несчем
#if DEBUG && LOG
                            EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::LinkLast: Nodthing to link to detected 'EQUIVALENT' Node <{equivalentND.Node.X.ToString("N3")}{equivalentND.Node.Y.ToString("N3")}{equivalentND.Node.Z.ToString("N3")}>");
#endif
                        }
                        return equivalentND;
                    }
                    else
                    {
                        if (lastND != null && lastND.Node != null)
                        {
                            // Вставляем узел и связываем его с предыдущим узлом lastNode
                            if (Graph.AddNode(newNode))
                            {
#if DEBUG && LOG
                                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::LinkLast: Add 'NEW' Node <{newNode.X.ToString("N3")}{newNode.Y.ToString("N3")}{newNode.Z.ToString("N3")}>");
#endif
                                if (uniDirection)
                                {
                                    Graph.AddArc(lastND.Node, newNode, lastND.Distance);
#if DEBUG && LOG
                                    EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::LinkLast: UniLink {{last -> ew}} <{lastND.Node.X.ToString("N3")}{lastND.Node.Y.ToString("N3")}{lastND.Node.Z.ToString("N3")}>");
#endif
                                }
                                else
                                {
                                    Graph.Add2Arcs(lastND.Node, newNode, lastND.Distance);
#if DEBUG && LOG
                                    EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::LinkLast: BiLink {{last <-> ew}} <{lastND.Node.X.ToString("N3")}{lastND.Node.Y.ToString("N3")}{lastND.Node.Z.ToString("N3")}>");
#endif
                                }
                                return new NodeDetail(newNode);
                            }
                            else
                            {
#if DEBUG && LOG
                                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::LinkLast: Fail to add Node <{newNode.X.ToString("N3")}, {newNode.Y.ToString("N3")}, {newNode.Z.ToString("N3")}>");
#endif
                                return lastND;
                            }
                        }
                        else
                        {
                            // предыдущий узел "отсутствует"
                            // связывать несчем
                            if (Graph.AddNode(newNode))
                            {
#if DEBUG && LOG
                                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::LinkLast: Add 'NEW' Node <{newNode.X.ToString("N3")}{newNode.Y.ToString("N3")}{newNode.Z.ToString("N3")}> without links");
#endif
                                return new NodeDetail(newNode);
                            }
                            else
                            {
#if DEBUG && LOG
                                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::LinkLast: Fail to add Node <{newNode.X.ToString("N3")}, {newNode.Y.ToString("N3")}, {newNode.Z.ToString("N3")}>");
#endif
                                return null;
                            }
                        }
                    }
                }
#if DEBUG
                catch (Exception ex)
                {
                    EntityToolsLogger.WriteLine(Logger.LogType.Debug, ex.ToString());
                    return null;
                }
#else
                catch { }
#endif
#if PROFILING && DEBUG
                finally
                {
                    stopwatch.Stop();
                    WatchCounter++;
                    totalWatchTiks += stopwatch.ElapsedTicks;
                    totalWatchMs += stopwatch.ElapsedMilliseconds;
#if DEBUG && LOG
                    Logger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::LinkLast: ElapsedTime is {stopwatch.ElapsedMilliseconds} ms ({stopwatch.ElapsedTicks})");
#endif
                }
#endif
            }
            return null;
        }

        public static NodeDetail LinkNearest1(Vector3 pos, MapperGraphCache Graph, NodeDetail lastND = null, bool uniDirection = false)
        {
            if (Graph == null || pos == null || !pos.IsValid)
                return null;

            string obj = "AddNavigationNodeStatic.LinkNearest1";
            lock (obj)
            {
#if PROFILING && DEBUG
                stopwatch.Restart();
#endif
                Node newNode = new Node(pos.X, pos.Y, pos.Z);

                double minNodeDistance = EntityTools.PluginSettings.Mapper.WaypointDistance;
                //double minDistance = EntityTools.PluginSettings.Mapper.WaypointDistance / 2d;
                //double maxDistance = Math.Max(25, EntityTools.PluginSettings.Mapper.WaypointDistance * 1.25d);
                double maxZDifference = EntityTools.PluginSettings.Mapper.MaxElevationDifference;
                double equivalenceDistance = EntityTools.PluginSettings.Mapper.WaypointEquivalenceDistance;
                //bool isEquivalent = false;
                NodeDetail nearestND = null; // ближайший узел
                NodeDetail equivalentND = null;

                try
                {
                    // сканируем узлы графа, для определения ближайшего
                    for (int i = 0; i < Graph.Nodes.Count; i++)
                    {
                        //Проверяем разницу высот
                        if (Graph.Nodes[i] is Node iNode)
                        {
                            NodeDetail currentND = new NodeDetail(iNode, newNode);
                            if (Math.Sign(currentND.Vector.Z) * currentND.Vector.Z < maxZDifference)
                            {
                                // Разница высот в пределах допустимой величины,
                                // проверяем расстояние от добавляемой точки до текущей
                                if (currentND.Distance < equivalenceDistance)
                                {
                                    // NewNode CurNode в пределах equivalenceDistance
                                    if (equivalentND == null
                                        || equivalentND.Distance > currentND.Distance)
                                        equivalentND = currentND;
                                }
                                else if (currentND.Distance <= minNodeDistance
                                         && (nearestND == null || currentND.Distance < nearestND.Distance))
                                {
                                    // curNodeDet - близажший существующий узел к NewNode
                                    nearestND = currentND;
                                }
                            }
                        }
                    }

                    if(equivalentND != null)
                    {
#if DEBUG && LOG
                        EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::LinkNearest1: Detect 'EQUIVALENT' node <{equivalentND.Node.X.ToString("N3")}{equivalentND.Node.Y.ToString("N3")}{equivalentND.Node.Z.ToString("N3")}>");
#endif
                        // Найден эквивалентный узел
                        // строим связи к equivalentND
                        if (lastND != null)
                        {
                            lastND.Rebase(equivalentND.Node);
                            if (uniDirection)
                            {
                                Graph.AddArc(lastND.Node, equivalentND.Node, lastND.Distance);
#if DEBUG && LOG
                                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::LinkNearest1: UniLink {{las  -> quivalent}} <{lastND.Node.X.ToString("N3")}{lastND.Node.Y.ToString("N3")}{lastND.Node.Z.ToString("N3")}>");
#endif
                            }
                            else
                            {
                                Graph.Add2Arcs(lastND.Node, equivalentND.Node, lastND.Distance);
#if DEBUG && LOG
                                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::LinkNearest1: BiLink {{las < -> quivalent}} <{lastND.Node.X.ToString("N3")}{lastND.Node.Y.ToString("N3")}{lastND.Node.Z.ToString("N3")}>");
#endif
                            }
                        }
                        if (nearestND != null)
                        {
                            nearestND.Rebase(equivalentND.Node);
                            if (uniDirection)
                            {
                                Graph.AddArc(nearestND.Node, equivalentND.Node, nearestND.Distance);
#if DEBUG && LOG
                                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::LinkNearest1: UniLink {{nearest -> equivalent}} <{nearestND.Node.X.ToString("N3")}{nearestND.Node.Y.ToString("N3")}{nearestND.Node.Z.ToString("N3")}>");
#endif
                            }
                            else
                            {
                                Graph.Add2Arcs(nearestND.Node, equivalentND.Node, nearestND.Distance);
#if DEBUG && LOG
                                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::LinkNearest1: BiLink {{nearest -> equivalent}} <{nearestND.Node.X.ToString("N3")}{nearestND.Node.Y.ToString("N3")}{nearestND.Node.Z.ToString("N3")}>");
#endif
                            }
                        }
                        return equivalentND;
                    }
                    // Вставляем в Граф узел newNode
                    // связываем его с предыдущим узлом lastNode и с ближайшим узлом
                    else if (Graph.AddNode(newNode))
                    {
#if DEBUG && LOG
                        EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::LinkNearest1: Add 'NEW' node <{newNode.X.ToString("N3")}, {newNode.Y.ToString("N3")}, {newNode.Z.ToString("N3")}>");
#endif
                        if (lastND != null)
                        {
                            if (uniDirection)
                            {
                                Graph.AddArc(lastND.Node, newNode, lastND.Distance);
#if DEBUG && LOG
                                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::LinkNearest1: Link {{last -> new}} <{lastND.Node.X.ToString("N3")}, {lastND.Node.Y.ToString("N3")}, {lastND.Node.Z.ToString("N3")}>");
#endif
                            }
                            else
                            {
                                Graph.Add2Arcs(lastND.Node, newNode, lastND.Distance);
#if DEBUG && LOG
                                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::LinkNearest1: Link {{las <->ew}} <{lastND.Node.X.ToString("N3")}, {lastND.Node.Y.ToString("N3")}, {lastND.Node.Z.ToString("N3")}>");
#endif
                            }
                        }
                        if (nearestND != null)
                        {
                            if (uniDirection)
                            {
                                Graph.AddArc(nearestND.Node, newNode, nearestND.Distance);
#if DEBUG && LOG
                                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::LinkNearest1: Link {{nearest -> new}} <{nearestND.Node.X.ToString("N3")}, {nearestND.Node.Y.ToString("N3")}, {nearestND.Node.Z.ToString("N3")}>");
#endif
                            }
                            else
                            {
                                Graph.Add2Arcs(nearestND.Node, newNode, nearestND.Distance);
#if DEBUG && LOG
                                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::LinkNearest1: Link {{nearestNod <->ewNode}} <{nearestND.Node.X.ToString("N3")}, {nearestND.Node.Y.ToString("N3")}, {nearestND.Node.Z.ToString("N3")}>");
#endif
                            }

                        }
#if DEBUG && LOG
                        EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::LinkNearest1: Link newNode <{newNode.X.ToString("N3")}, {newNode.Y.ToString("N3")}, {newNode.Z.ToString("N3")}>");
#endif
                        return new NodeDetail(newNode);
                    }
                    else
                    {
#if DEBUG && LOG
                        EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::LinkNearest1: Fail to add Node <{newNode.X.ToString("N3")}, {newNode.Y.ToString("N3")}, {newNode.Z.ToString("N3")}>");
#endif
                        return lastND;
                    }
                }
#if DEBUG
                catch (Exception ex)
                {
                    EntityToolsLogger.WriteLine(Logger.LogType.Debug, ex.ToString());
                    return null;
                }
#else
                catch { }
#endif
#if PROFILING && DEBUG
                finally
                {
                    stopwatch.Stop();
                    WatchCounter++;
                    totalWatchTiks += stopwatch.ElapsedTicks;
                    totalWatchMs += stopwatch.ElapsedMilliseconds;
#if DEBUG && LOG
                    EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::LinkNearest1: ElapsedTime is {stopwatch.ElapsedMilliseconds} ms ({stopwatch.ElapsedTicks})");
#endif
                }
#endif
            }

            return null;
        }

        internal static double CosinusXY(NodeDetail A, NodeDetail B)
        {
            // вычисляем Cos угла между векторами из формулы скалярного произведения векторов
            // a * b = |a| * |b| * cos (alpha) = xa * xb + ya * yb
            //double cos = (A.Vector.X * B.Vector.X + A.Vector.Y * B.Vector.Y + A.Vector.Z * B.Vector.Z)
            //             / B.Distance * A.Distance;

            double cos = (A.Vector.X * B.Vector.X + A.Vector.Y * B.Vector.Y)
                          / Math.Sqrt((A.Vector.X * A.Vector.X + A.Vector.Y * A.Vector.Y)
                                      * (B.Vector.X * B.Vector.X + B.Vector.Y * B.Vector.Y));

            return cos;
        }

        internal static double Distance(Node node1, Node node2)
        {
            return Math.Sqrt(Math.Pow(node1.X - node2.X, 2) + Math.Pow(node1.Y - node2.Y, 2) + Math.Pow(node1.Z - node2.Z, 2));
        }

        private static bool NodesEquals(Node node1, Node node2)
        {
            return node1.Position == node2.Position;
        }
        private static bool NodesEquals(NodeDetail nodeDet1, NodeDetail nodeDet2)
        {
            /*return nodeDet1.Node.X == nodeDet2.Node.X
                && nodeDet1.Node.Y == nodeDet2.Node.Y
                && nodeDet1.Node.Z == nodeDet2.Node.Z;*/
            return nodeDet1.Node.Position == nodeDet2.Node.Position;
        }

#region Методы фильтрации (проверка связей и расстояний) или без таковых и добавления узлов в список ближайших
        /// <summary>
        /// Добавление узла в список без проверки
        /// </summary>
        /// <param name="listNodes"></param>
        /// <param name="nodeDet"></param>
        /// <returns></returns>
        private static int SimpleAddNode(List<NodeDetail> listNodes, NodeDetail nodeDet)
        {
            listNodes.Add(nodeDet);
            return listNodes.Count - 1;
        }

        /// <summary>
        /// Добавление узла в список с проверкой наличия связей
        /// </summary>
        /// <param name="listNodes"></param>
        /// <param name="nodeDet">nodePair.First - добавляемый узел
        /// nodePair.Second - расстояние от узла до newNode</param>
        /// <returns></returns>
        private static int SmartAddNode1(List<NodeDetail> listNodes, NodeDetail nodeDet)
        {
            IList outArcs = nodeDet.Node.OutgoingArcs;

            // проверяем наличие связей узла nodePair.First с узлами из listNodes
            // при наличии такой связи nodePair.First в список не добавляется
            foreach (NodeDetail n in listNodes)
            {
                // проверяем исходящие связи узла nodePair.First
                foreach (Arc arc in nodeDet.Node.OutgoingArcs)
                {
                    if (NodesEquals(arc.EndNode, n.Node))
                    {
                        // из node есть вязь с узлом из listNodes
                        // поэтому добавлять node в список не требуется
                        return -1;
                    }
                }
            }

            listNodes.Add(nodeDet);
            return listNodes.Count - 1;
        }

        /// <summary>
        /// Добавление узла в список с проверкой наличия связей
        /// и расстояния до узла newNode
        /// </summary>
        /// <param name="listNodes"></param>
        /// <param name="nodeDet">nodePair.First - добавляемый узел
        /// nodePair.Second - расстояние от узла до newNode</param>
        /// <returns></returns>
        private static int SmartAddNode2(List<NodeDetail> listNodes, NodeDetail nodeDet)
        {
            IList outArcs = nodeDet.Node.OutgoingArcs;
            IList inArcs = nodeDet.Node.IncomingArcs;

            // проверяем наличие связей узла nodePair.First с узлами из listNodes
            // при наличии такой связи nodePair.First в список не добавляется
            foreach (NodeDetail curNodeDet in listNodes)
            {
                if (curNodeDet.Distance < nodeDet.Distance)
                {
                    // добавляемый узел nodePair.Second дальше от newNode
                    // чем узел curNodePair.First, имеющийся в списке listNodes 

                    // проверяем наличие связи между узлами nodePair.First и curNodePair.First
                    foreach (Arc arc in nodeDet.Node.OutgoingArcs)
                    {
                        if (NodesEquals(arc.EndNode, curNodeDet.Node))
                        {
                            // Из узла nodePair.First можно перейти в узел curNodePair.First
                            // при этом nodePair.First дальше от newNode
                            // поэтому нет смысла добавлять nodePair.First в список listNodes
                            return -1;
                        }
                    }
                }
            }

            listNodes.Add(nodeDet);
            return listNodes.Count - 1;
        }

        /// <summary>
        /// Добавление узла в список с проверкой наличия связей
        /// и расстояния до узла newNode
        /// </summary>
        /// <param name="listNodes"></param>
        /// <param name="nodeDet">nodePair.First - добавляемый узел
        /// nodePair.Second - расстояние от узла до newNode</param>
        /// <returns></returns>
        private static int SmartAddNode3(List<NodeDetail> listNodes, NodeDetail nodeDet)
        {
            // проверяем наличие связей узла nodePair.First с узлами из listNodes
            // при наличии такой связи nodePair.First в список не добавляется
            int curNodeInd = 0;
            while (curNodeInd < listNodes.Count)
            {
                NodeDetail curNodeDet = listNodes[curNodeInd];
                bool curNodeRemoved = false;

                if (curNodeDet.Distance < nodeDet.Distance)
                {
                    // добавляемый узел nodePair.Second дальше от newNode
                    // чем узел curNodePair.First, имеющийся в списке listNodes 

                    // проверяем наличие связи между узлами nodePair.First и curNodePair.First
                    foreach (Arc arc in nodeDet.Node.OutgoingArcs)
                    {
                        if (NodesEquals(arc.EndNode, curNodeDet.Node))
                        {
                            // Из узла nodePair.First можно перейти в узел curNodePair.First
                            // при этом nodePair.First дальше от newNode
                            // поэтому нет смысла добавлять nodePair.First в список listNodes
                            return -1;
                        }
                    }
                }
                else
                {
                    // добавляемый узел nodePair.Second ближе к newNode
                    // чем узел curNodePair.First, имеющийся в списке listNodes 

                    // проверяем наличие связи между узлами nodePair.First и curNodePair.First
                    foreach (Arc arc in nodeDet.Node.IncomingArcs)
                    {
                        if (NodesEquals(arc.StartNode, curNodeDet.Node))
                        {
                            // Из узла curNodePair.First можно перейти в узел nodePair.First
                            // при этом nodePair.First ближе к newNode
                            // поэтому curNodePair.First нужно исключить из списка listNodes
                            listNodes.RemoveAt(curNodeInd);
                            curNodeRemoved = true;
                            break;
                        }
                    }
                }
                if (!curNodeRemoved)
                    curNodeInd++;
            }

            listNodes.Add(nodeDet);
            return listNodes.Count - 1;
        }
#endregion
    }
}
