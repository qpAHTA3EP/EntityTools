#define PROFILING
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

namespace EntityTools.Patches.Mapper
{
    internal class AddNavigationNodeDirect
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
                Logger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeDirect::WatchCounter: {WatchCounter}");
                Logger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeDirect::TotalWatchTiks: {totalWatchTiks}");
                Logger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeDirect::TotalWatchMs: {totalWatchMs}");
                Logger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeDirect::Average: {(totalWatchMs/WatchCounter).ToString("N3")} ms; {(totalWatchTiks/WatchCounter).ToString("N0")} Ticks");
            }

        }
#endif
        public AddNavigationNodeDirect(float X, float Y, float Z, Graph Graph)
        {
            newNode = new Node((double)X, (double)Y, (double)Z);
            this.Graph = Graph;
            //new Thread(new ThreadStart(this.Work)).Start();
            Task.Factory.StartNew(Work);
        }

        public AddNavigationNodeDirect(Node node, Graph graph, bool uniDirection = false/*, bool showLogs = false*/)
        {
            this.newNode = node;
            this.Graph = graph;
            this.uniDirection = uniDirection;
            //this.showLogs = showLogs;
            this.Work();
        }

        public void Work()
        {
            string obj = "AddNavigationNodeDirect";
            lock (obj)
            {
#if PROFILING && DEBUG
                stopwatch.Restart();
#endif
                //double minDistance = EntityTools.PluginSettings.Mapper.WaipointDistance;
                double maxDistance = Math.Max(25, EntityTools.PluginSettings.Mapper.WaypointDistance * 1.25);
                double maxZDifference = EntityTools.PluginSettings.Mapper.MaxElevationDifference;

                // список узлов, ближайших к добавляемому узлу newNode
                // Pair.Fist - узел
                // Pair.Second - расстояние от узла, до добавляемого узла newNode
                List<Pair<Node, double>> nearestNodeList = new List<Pair<Node, double>>(10);

                bool needAddNode = true;

                try
                {
                    double distance;
                    double zDifference;
                    // сканируем узлы графа, для определения ближайших к добавляемому узлу newNode
                    foreach (Node currentNode in Graph.Nodes)
                    {
                        //Проверяем разницу высот
                        zDifference = currentNode.Z - newNode.Z;
                        zDifference *= Math.Sign(zDifference);
                        if (zDifference < maxZDifference)
                        {
                            // Разница высот в пределах допустимой величины,
                            // вычисляем расстояние от добавляемой точки до текущей
                            distance = Distance(currentNode, newNode);

                            if (distance < 2)
                            {
                                // В графе есть точка, расстояние до которой меньше 2
                                // добавлять текущую точку нет смысла
                                needAddNode = false;
                                return;
                            }
                            else if (/*distance >= minDistance
                                     && */distance <= maxDistance)
                            {
                                // добавляет узел currentNode в список нодов
                                SimpleAddNode(nearestNodeList, new Pair<Node, double>(currentNode, distance));                                
                            }
                        }
                    }

                    if (needAddNode)
                    {
                        int newNodeInd = Graph.Nodes.Add(newNode);
                        nearestNodeList.Sort((n1, n2) => (int)(n1.Second - n2.Second));

                        while (nearestNodeList.Count > 0)
                        {
                            Pair<Node, double> currentNodePair = nearestNodeList[0];
                            // Добавляем связь между узлами newNode и CurrentNode
                            try
                            {
                                if (uniDirection)
                                {
                                    Graph.AddArc(currentNodePair.First, newNode, (float)nearestNodeList[0].Second);
                                }
                                else
                                {
                                    Graph.Add2Arcs(newNode, currentNodePair.First, (float)nearestNodeList[0].Second);
                                }
                                //if (showLogs)
                                //{
                                //    Logger.WriteLine(Logger.LogType.Debug, string.Concat(new object[]
                                //    {
                                //                " Node #",
                                //                currentNode.GetHashCode().ToString("8X"),
                                //                " linked with node #",
                                //                newNode.GetHashCode().ToString("8X"),
                                //                " Weight : ",
                                //                Math.Truncate(nearestNodeList[0].Second)
                                //    }));
                                //}
                                nearestNodeList.RemoveAt(0);
                                //////// просматриваем список ближайших узлов nearestNodeList
                                //////// и удаляем из нех те, которые связаны с currentNode
                                //////IList inArcs = currentNodePair.First.IncomingArcs;
                                //////nearestNodeList.RemoveAll((Pair<Node, double> n) =>
                                //////            {
                                //////                bool linked = false;
                                //////                foreach (Arc arc in inArcs)
                                //////                    if (NodesEquals(arc.StartNode, n.First))
                                //////                        linked = true;
                                //////                return linked;
                                //////            });
                            }
#if DEBUG
                            catch (Exception ex)
                            {
                                Logger.WriteLine(Logger.LogType.Debug, ex.Message);
                            }
#else
                            catch { }
#endif
                        }
                    }

                }
#if DEBUG
                catch (Exception ex)
                {
                    Logger.WriteLine(Logger.LogType.Debug, ex.Message);
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
                    Logger.WriteLine(Logger.LogType.Debug, $"AddNavigationNodeDirect:: Time is {stopwatch.ElapsedMilliseconds} ms ({stopwatch.ElapsedTicks})");
                }
#endif
            }
        }

        private double Distance(Node node1, Node node2)
        {
            return Math.Sqrt(Math.Pow(node1.X - node2.X, 2) + Math.Pow(node1.Y - node2.Y, 2) + Math.Pow(node1.Z - node2.Z, 2));
        }

        private bool NodesEquals(Node node1, Node node2)
        {
            return node1.X == node2.X && node1.Y == node2.Y && node1.Z == node2.Z;
        }

        #region Методы фильтрации (проверка связей и расстояний) или без таковых и добавления узлов в список ближайших
        /// <summary>
        /// Добавление узла в список без проверки
        /// </summary>
        /// <param name="listNodes"></param>
        /// <param name="nodePair"></param>
        /// <returns></returns>
        private int SimpleAddNode(List<Pair<Node, double>> listNodes, Pair<Node, double> nodePair)
        {
            listNodes.Add(nodePair);
            return listNodes.Count - 1;
        }

        /// <summary>
        /// Добавление узла в список с проверкой наличия связей
        /// </summary>
        /// <param name="listNodes"></param>
        /// <param name="nodePair">nodePair.First - добавляемый узел
        /// nodePair.Second - расстояние от узла до newNode</param>
        /// <returns></returns>
        private int SmartAddNode1(List<Pair<Node, double>> listNodes, Pair<Node, double> nodePair)
        {
            IList outArcs = nodePair.First.OutgoingArcs;

            // проверяем наличие связей узла nodePair.First с узлами из listNodes
            // при наличии такой связи nodePair.First в список не добавляется
            foreach (Pair<Node, double> n in listNodes)
            {
                // проверяем исходящие связи узла nodePair.First
                foreach (Arc arc in nodePair.First.OutgoingArcs)
                {
                    if (NodesEquals(arc.EndNode, n.First))
                    {
                        // из node есть вязь с узлом из listNodes
                        // поэтому добавлять node в список не требуется
                        return -1;
                    }
                }
            }

            listNodes.Add(nodePair);
            return listNodes.Count - 1;
        }

        /// <summary>
        /// Добавление узла в список с проверкой наличия связей
        /// и расстояния до узла newNode
        /// </summary>
        /// <param name="listNodes"></param>
        /// <param name="nodePair">nodePair.First - добавляемый узел
        /// nodePair.Second - расстояние от узла до newNode</param>
        /// <returns></returns>
        private int SmartAddNode2(List<Pair<Node, double>> listNodes, Pair<Node, double> nodePair)
        {
            IList outArcs = nodePair.First.OutgoingArcs;
            IList inArcs = nodePair.First.IncomingArcs;

            // проверяем наличие связей узла nodePair.First с узлами из listNodes
            // при наличии такой связи nodePair.First в список не добавляется
            foreach (Pair<Node, double> curNodePair in listNodes)
            {
                if (curNodePair.Second < nodePair.Second)
                {
                    // добавляемый узел nodePair.Second дальше от newNode
                    // чем узел curNodePair.First, имеющийся в списке listNodes 

                    // проверяем наличие связи между узлами nodePair.First и curNodePair.First
                    foreach (Arc arc in nodePair.First.OutgoingArcs)
                    {
                        if (NodesEquals(arc.EndNode, curNodePair.First))
                        {
                            // Из узла nodePair.First можно перейти в узел curNodePair.First
                            // при этом nodePair.First дальше от newNode
                            // поэтому нет смысла добавлять nodePair.First в список listNodes
                            return -1;
                        }
                    }
                }
            }

            listNodes.Add(nodePair);
            return listNodes.Count - 1;
        }

        /// <summary>
        /// Добавление узла в список с проверкой наличия связей
        /// и расстояния до узла newNode
        /// </summary>
        /// <param name="listNodes"></param>
        /// <param name="nodePair">nodePair.First - добавляемый узел
        /// nodePair.Second - расстояние от узла до newNode</param>
        /// <returns></returns>
        private int SmartAddNode3(List<Pair<Node, double>> listNodes, Pair<Node, double> nodePair)
        {
            // проверяем наличие связей узла nodePair.First с узлами из listNodes
            // при наличии такой связи nodePair.First в список не добавляется
            int curNodeInd = 0;
            while (curNodeInd < listNodes.Count)
            {
                Pair<Node, double> curNodePair = listNodes[curNodeInd];
                bool curNodeRemoved = false;

                if (curNodePair.Second < nodePair.Second)
                {
                    // добавляемый узел nodePair.Second дальше от newNode
                    // чем узел curNodePair.First, имеющийся в списке listNodes 

                    // проверяем наличие связи между узлами nodePair.First и curNodePair.First
                    foreach (Arc arc in nodePair.First.OutgoingArcs)
                    {
                        if (NodesEquals(arc.EndNode, curNodePair.First))
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
                    foreach (Arc arc in nodePair.First.IncomingArcs)
                    {
                        if (NodesEquals(arc.StartNode, curNodePair.First))
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

            listNodes.Add(nodePair);
            return listNodes.Count - 1;
        } 
        #endregion

        public Graph Graph;

        public Node newNode;

        public bool uniDirection;

        //public bool showLogs = false;
    }
}
