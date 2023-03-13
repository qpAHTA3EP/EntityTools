//#define PROFILING
//#define LOG
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
//using NodeDistPair = EntityTools.Tools.Pair<AStar.Node, double>;

namespace EntityTools.Patches.Mapper
{
    internal static class AddNavigationNodeStatic
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
                Logger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::WatchCounter: {WatchCounter}");
                Logger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::TotalWatchTiks: {totalWatchTiks}");
                Logger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::TotalWatchMs: {totalWatchMs}");
                Logger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::Average: {(totalWatchMs / WatchCounter).ToString("N3")} ms; {(totalWatchTiks / WatchCounter).ToString("N0")} Ticks");
            }

        }
#endif
        public static NodeDetail LinkComplex(Vector3 pos, MapperGraphCache Graph, NodeDetail lastNodeDetail = null, bool uniDirection = false)
        {
            if (Graph == null || pos == null || !pos.IsValid)
                return null;

            string obj = "AddNavigationNode.LinkComplex";
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
                // список узлов, ближайших к добавляемому узлу newNode
                List<NodeDetail> nearestNodeList = new List<NodeDetail>();

                try
                {
                    // сканируем узлы графа, для определения ближайших к добавляемому узлу newNode
                    double minNodeDistance = double.MaxValue; // расстояние до ближайшего узла
                    int closestNodeInd = -1; // Индекс ближайшего узла в списке nearestNodeList
                    bool isEquivalent = false; // флаг, указывающий, что newNode был заменена на эквивалентный существующий узел
                    NodeDetail equivalentNodeDetail = null;
                    for (int i = 0; i < Graph.Nodes.Count; i++)
                    {
                        if (!(Graph.Nodes[i] is Node currentNode)
                            || (lastNodeDetail != null
                                && NodesEquals(lastNodeDetail.Node, currentNode)))
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
                                if(equivalentNodeDetail == null || curNodeDet.Distance < equivalentNodeDetail.Distance)
                                    equivalentNodeDetail = curNodeDet;
#endif
                            }
                            else if (curNodeDet.Distance <= maxDistance)
                            {
                                // добавляет узел currentNode в список нодов
                                int ind = SimpleAddNode(nearestNodeList, curNodeDet);
                                if(curNodeDet.Distance < minNodeDistance)
                                {
                                    // сохраняем индекс узла, ближайшего к newNode
                                    minNodeDistance = curNodeDet.Distance;
                                    closestNodeInd = ind;
                                }
                            }
                        }
                    }

#if !SWAP_NEW2EQUIVALENT
                    if (equivalentNodeDetail != null)
                    {
                        // Формирование функтора добавление граней, в зависимости от типа пути (одно- или двунаправленный)
                        Action<Node, Node, double> AddArcFunc;
                        if (uniDirection)
                            AddArcFunc = (n1, n2, w) => Graph.AddArc(n1, n2, w);
                        else AddArcFunc = (n1, n2, w) => Graph.Add2Arcs(n1, n2, w);

                        //int arcsNum = equivalentNodeDetail.Node.IncomingArcs.Count + equivalentNodeDetail.Node.IncomingArcs.Count;
                        // Если задан узел lastNodeDetail - добавляем связь с ним
                        if (lastNodeDetail != null)
                        {
                            lastNodeDetail.Rebase(equivalentNodeDetail.Node);
                            AddArcFunc(lastNodeDetail.Node, equivalentNodeDetail.Node, lastNodeDetail.Distance);

                            return equivalentNodeDetail;
                        }

                        // Добавляем связь между узлами newNode и CurrentNode
                        for (int ind = 0; ind < nearestNodeList.Count; ind++)
                        {
                            NodeDetail curNodeDet = nearestNodeList[ind];
                            curNodeDet.Rebase(equivalentNodeDetail.Node);
                            AddArcFunc(curNodeDet.Node, equivalentNodeDetail.Node, curNodeDet.Distance);
                        }

                    }
#endif

                    if (isEquivalent || Graph.AddNode(newNode))
                    {
                        // Формирование функтора добавление граней, в зависимости от типа пути (одно- или двунаправленный)
                        Action<Node, Node, double> AddArcFunc;
                        if (uniDirection)
                            AddArcFunc = (n1, n2, w) => Graph.AddArc(n1, n2, w);
                        else AddArcFunc = (n1, n2, w) => Graph.Add2Arcs(n1, n2, w);

                        try
                        {
                            if (lastNodeDetail != null)
                            {
                                if (closestNodeInd >= 0 && closestNodeInd < nearestNodeList.Count)
                                {
                                    NodeDetail closestNodeDetail = nearestNodeList[closestNodeInd];

                                    // анализируем взаимное расположение cosestNode и lastNode
                                    // Если угол между направлениями newNode на cosestNode и lastNode будет меньше 20 градусов, 
                                    // cos(10) == 0,9848
                                    // cos(20) == 0,9397
                                    // cos(45) == 0,5
                                    // тогда можно будет соединить только currentNode и lastNode, 
                                    // а текущих узел newNode отбросить

                                    // вычисляем Cos угла между векторами из формулы скалярного произведения векторов
                                    // a * b = |a| * |b| * cos (alpha) = xa * xb + ya * yb
                                    double cos = Cosinus(closestNodeDetail, lastNodeDetail);
                                    if (cos >= 0.5)
                                    {
                                        // направление на closestNode и lastNode близко, поэтому
                                        // добавляем путь от lastNode до closestNode
                                        AddArcFunc(lastNodeDetail.Node, closestNodeDetail.Node, Distance(lastNodeDetail.Node, closestNodeDetail.Node));
                                        // добавляем путь от closestNode до newNode
                                        AddArcFunc(closestNodeDetail.Node, newNode, Distance(closestNodeDetail.Node, newNode));
                                    }
                                    else
                                    {
                                        // направления на closestNode и lastNode сильно отличаются, поэтому
                                        // строить путь от lastNode к closestNode не следует

                                        // строим путь от lastNode к newNode
                                        AddArcFunc(lastNodeDetail.Node, newNode, lastNodeDetail.Distance);
                                        closestNodeInd = -1;

                                        // строим путь от closestNode к newNode 
                                        AddArcFunc(closestNodeDetail.Node, newNode, closestNodeDetail.Distance);
                                    }

                                    nearestNodeList.RemoveAt(0);
                                }
                                else AddArcFunc(lastNodeDetail.Node, newNode, lastNodeDetail.Distance);
                            }
                            else if(closestNodeInd >= 0 && closestNodeInd < nearestNodeList.Count)
                            {
                                NodeDetail closestNodeDet = nearestNodeList[closestNodeInd];

                                AddArcFunc(closestNodeDet.Node, newNode, closestNodeDet.Distance);
                            }

                            // Добавляем связь между узлами newNode и CurrentNode
                            for (int ind = 0; ind < closestNodeInd; ind++)
                            {
                                NodeDetail curNodeDet = nearestNodeList[ind];

                                AddArcFunc(curNodeDet.Node, newNode, curNodeDet.Distance);
                            }
                            // пропускаем closestNodeInd и добавляем связи с следующим узлам
                            for (int ind = closestNodeInd + 1; ind < nearestNodeList.Count; ind++)
                            {
                                NodeDetail curNodeDet = nearestNodeList[ind];

                                AddArcFunc(curNodeDet.Node, newNode, curNodeDet.Distance);
                            }
                        }
                        catch (Exception ex)
                        {
#if DEBUG
                            Logger.WriteLine(Logger.LogType.Debug, ex.ToString());
#endif
                            return null;
                        }
                        return new NodeDetail(newNode);
                    }
#if DEBUG && LOG
                    else Logger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::LinkComplex: Fail to add Node <{newNode.X.ToString("N3")}, {newNode.Y.ToString("N3")}, {newNode.Z.ToString("N3")}>");
#endif

                }
                catch (Exception ex)
                {
#if DEBUG
                    Logger.WriteLine(Logger.LogType.Debug, ex.ToString());
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
                    Logger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::LinkComplex: ElapsedTime is {stopwatch.ElapsedMilliseconds} ms ({stopwatch.ElapsedTicks})");
#endif
                }
#endif
            }
            return null;
        }

        public static NodeDetail LinkLast(Vector3 pos, MapperGraphCache Graph, NodeDetail lastNodeDetail = null, bool uniDirection = false)
        {
            if (Graph == null || pos == null || !pos.IsValid)
                return null;

            string obj = "AddNavigationNodeStatic.LinkLast";
            lock (obj)
            {
#if PROFILING && DEBUG
                stopwatch.Restart();
#endif
                Node newNode = new Node(pos.X, pos.Y, pos.Z);

                try
                {
                    if (lastNodeDetail != null && lastNodeDetail.Node != null)
                    {
                        // Вставляем узел и связываем его с предыдущим узлом lastNode
                        if (Graph.AddNode(newNode))
                        {
                            if (uniDirection)
                                Graph.AddArc(lastNodeDetail.Node, newNode, lastNodeDetail.Distance);
                            else Graph.Add2Arcs(lastNodeDetail.Node, newNode, lastNodeDetail.Distance);
                            return new NodeDetail(newNode);
                        }
                        else
                        {
#if DEBUG && LOG
                            Logger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::LinkLast: Fail to add Node <{newNode.X.ToString("N3")}, {newNode.Y.ToString("N3")}, {newNode.Z.ToString("N3")}>");
#endif
                            return null;
                        }
                    }
                    else
                    {
                        // предыдущий узел "отсутствует"
                        // связывать несчем
                        if (Graph.AddNode(newNode))
                            return new NodeDetail(newNode);
                        else
                        {
#if DEBUG && LOG
                            Logger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::LinkLast: Fail to add Node <{newNode.X.ToString("N3")}, {newNode.Y.ToString("N3")}, {newNode.Z.ToString("N3")}>");
#endif
                            return null;
                        }
                    }
                }
#if DEBUG
                catch (Exception ex)
                {
                    Logger.WriteLine(Logger.LogType.Debug, ex.ToString());
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
                return null;
            }
        }

        public static NodeDetail LinkNearest(Vector3 pos, MapperGraphCache Graph, NodeDetail lastNodeDetail = null, bool uniDirection = false)
        {
            if (Graph == null || pos == null || !pos.IsValid)
                return null;

            string obj = "AddNavigationNodeStatic.LinkNearest";
            lock (obj)
            {
#if PROFILING && DEBUG
                stopwatch.Restart();
#endif
                Node newNode = new Node(pos.X, pos.Y, pos.Z);

                double minNodeDistance = EntityTools.PluginSettings.Mapper.WaypointDistance;
                double minDistance = EntityTools.PluginSettings.Mapper.WaypointDistance / 2;
                double maxDistance = Math.Max(25, EntityTools.PluginSettings.Mapper.WaypointDistance * 1.25);
                double maxZDifference = EntityTools.PluginSettings.Mapper.MaxElevationDifference;
                double equivalenceDistance = EntityTools.PluginSettings.Mapper.WaypointEquivalenceDistance;
                bool isEquivalent = false;
                NodeDetail nearestNodeDet = null; // ближайший узел

                try
                {
                    // сканируем узлы графа, для определения ближайшего
                    for (int i = 0; i < Graph.Nodes.Count; i++)
                    {
                        //Проверяем разницу высот
                        if (Graph.Nodes[i] is Node iNode)
                        {
                            NodeDetail curNodeDet = new NodeDetail(iNode, newNode);
                            if (Math.Sign(curNodeDet.Vector.Z) * curNodeDet.Vector.Z < maxZDifference)
                            {
                                // Разница высот в пределах допустимой величины,
                                // проверяем расстояние от добавляемой точки до текущей
                                if (curNodeDet.Distance < equivalenceDistance)
                                {
                                    // В графе есть точка, расстояние до которой меньше equivalenceDistance
                                    // подменяем newNode на currentNode
                                    newNode = curNodeDet.Node;
                                    isEquivalent = true;
                                    if (nearestNodeDet != null)
                                        nearestNodeDet.Rebase(newNode);
                                }
                                else if (nearestNodeDet == null || curNodeDet.Distance <= minNodeDistance)
                                {
                                    // добавляет узел currentNode в список нодов
                                    if (nearestNodeDet == null || curNodeDet.Distance <= nearestNodeDet.Distance)
                                    {
                                        nearestNodeDet = curNodeDet;
                                    }
                                }
                            }
                        }
                    }


                    // Вставляем узел 
                    // связываем его с предыдущим узлом lastNode и с ближайшим узлом
                    if (isEquivalent || Graph.AddNode(newNode))
                    {
                        if(lastNodeDetail != null)
                        {
                            if (uniDirection)
                                Graph.AddArc(lastNodeDetail.Node, newNode, lastNodeDetail.Distance);
                            else Graph.Add2Arcs(lastNodeDetail.Node, newNode, lastNodeDetail.Distance);
                        }
                        if (nearestNodeDet != null)
                        {
                            if (uniDirection)
                                Graph.AddArc(nearestNodeDet.Node, newNode, nearestNodeDet.Distance);
                            else Graph.Add2Arcs(nearestNodeDet.Node, newNode, nearestNodeDet.Distance);
                        }
                        return new NodeDetail(newNode);
                    }
                    else
                    {
#if DEBUG && LOG
                        Logger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::LinkNearest: Fail to add Node <{newNode.X.ToString("N3")}, {newNode.Y.ToString("N3")}, {newNode.Z.ToString("N3")}>");
#endif
                        return null;
                    }
                }
#if DEBUG
                catch (Exception ex)
                {
                    Logger.WriteLine(Logger.LogType.Debug, ex.ToString());
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
                    Logger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeStatic::LinkNearest: ElapsedTime is {stopwatch.ElapsedMilliseconds} ms ({stopwatch.ElapsedTicks})");
#endif
                }
#endif
            }

            return null;
        }

        internal static double Cosinus(NodeDetail A, NodeDetail B)
        {
            // вычисляем Cos угла между векторами из формулы скалярного произведения векторов
            // a * b = |a| * |b| * cos (alpha) = xa * xb + ya * yb
            double cos = (A.Vector.X * B.Vector.X + A.Vector.Y * B.Vector.Y + A.Vector.Z * B.Vector.Z)
                         / B.Distance * A.Distance;

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
