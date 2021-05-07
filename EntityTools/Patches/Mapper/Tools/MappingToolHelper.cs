//#define PROFILING
#define LOG

using System;
using System.Collections.Generic;
using System.Threading;
using AStar;
using MyNW.Classes;

//using NodeDistPair = EntityTools.Tools.Pair<AStar.Node, double>;

namespace EntityTools.Patches.Mapper.Tools
{
#if PATCH_ASTRAL
    internal static class MappingToolHelper
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
                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"WatchCounter: {WatchCounter}");
                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"TotalWatchTiks: {totalWatchTiks}");
                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"TotalWatchMs: {totalWatchMs}");
                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"Average: {(totalWatchMs / WatchCounter):N3} ms; {(totalWatchTiks / WatchCounter).ToString("N0")} Ticks");
            }

        }
#endif
        /// <summary>
        /// Добавление вершины c координатами <paramref name="pos"/> и соединение её с ближайшими вершинами впереди, слева, справа и позади неё
        /// </summary>
        public static NodeDetail LinkNearest_3_Side(Vector3 pos, MapperGraphCache graph, NodeDetail lastND = null, bool uniDirection = false)
        {
            if (graph is null || pos is null || !pos.IsValid)
                return null;

            if (lastND is null)
                return LinkNearest_1(pos, graph, null, uniDirection);


#if PROFILING && DEBUG
            stopwatch.Restart();
#endif
            Node newNode = new Node(pos.X, pos.Y, pos.Z);

            double maxDistance = Math.Max(10d, EntityTools.Config.Mapper.WaypointDistance * 1.25d);
            double maxZDifference = EntityTools.Config.Mapper.MaxElevationDifference;
            double equivalenceDistance = EntityTools.Config.Mapper.WaypointEquivalenceDistance;

            // Ближайшие узлы разделенные по направлению движения
            NodeDetail frontND = null; // впереди 
            NodeDetail leftND = null; // слева    
            NodeDetail rightND = null; // справа
            NodeDetail backND = null; 
            // Косое произведение веторов A и B
            // D = Ax * By - Bx * Ay
            // Если D == 0 - вектора коллиниарны
            // Если D > 0 - точка B в верхней полуплоскости
            // Если D < 0 - точка B в нижней полуплоскости

            try
            {
                // сканируем узлы графа, для определения ближайших к добавляемому узлу newNode
                NodeDetail equivalentND = null;
                using (graph.ReadLock())
                {
                    foreach (Node currentNode in graph.NodesCollection)
                    {
                        if (NodesEquals(lastND.Node, currentNode))
                            continue;

                        //Проверяем разницу высот
                        NodeDetail currentND = new NodeDetail(currentNode, newNode);
                        if (Math.Abs(currentND.Z) < maxZDifference)
                        {
                            // Разница высот в пределах допустимой величины,
                            // проверяем расстояние от добавляемой точки до текущей
                            if (currentND.Distance < equivalenceDistance)
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
                                if (equivalentND is null || currentND.Distance < equivalentND.Distance)
                                    equivalentND = currentND;
#endif
                            }
                            else if (currentND.Distance <= maxDistance)
                            {
                                // вычисляем косинус угла меду векторами для определения четверти (впереди, сзади, слева, справа)
                                // в которой находится curNode
                                double cos = CosineOxy(lastND, currentND);

                                if (cos < -0.7071d)
                                {
                                    // Угол между векторами больше 135 градусов
                                    // Значит точка находится впереди
                                    if (frontND is null
                                        || frontND.Distance > currentND.Distance)
                                        frontND = currentND;
                                }
                                else if (cos < 0.7071d)
                                {
                                    // Угол между векторами в диапазоне 45~125 градусов

                                    // Вычисляем в какой полуплоскости находится currentND
                                    // относительно вектора на lastND
                                    // который направлен от newNode к lastND
                                    double D = lastND.X * currentND.Y - currentND.X * lastND.Y;

                                    if (D > 0)
                                    {
                                        if (rightND is null
                                            || rightND.Distance > currentND.Distance)
                                            rightND = currentND;
                                    }
                                    else
                                    {
                                        if (leftND is null
                                            || leftND.Distance > currentND.Distance)
                                            leftND = currentND;
                                    }
                                }
#if false
                                // проверка (lastND is null) уже была выполнена в самом начале метода
                                else if (lastND is null)
                                {
                                    if (backND == null
                                        || backND.Distance > currentND.Distance)
                                        backND = currentND;
                                } 
#else
                                // узлом в задней четверти является lastND
                                else if (backND is null 
                                    || backND.Distance > currentND.Distance)
                                    backND = currentND;
#endif
                            }
                        }
                    }
                }

                // Формирование функтора добавление граней, в зависимости от типа пути (одно- или двунаправленный)
                Action<Node, Node> AddArcFunc;
                if (uniDirection)
                     AddArcFunc = (n1, n2) => {  using (graph.WriteLock())
                                                        graph.AddArc(n1, n2); };
                else AddArcFunc = (n1, n2) => {  using (graph.WriteLock())
                                                        graph.Add2Arcs(n1, n2); };

                if (equivalentND != null)
                {
#if DEBUG && LOG
                    ETLogger.WriteLine(LogType.Debug, $"LinkNearest3Side: Detect 'EQUIVALENT' Node <{equivalentND.Node.X:N3};{equivalentND.Node.Y:N3};{equivalentND.Node.Z:N3}>");
#endif
                    // Добавляем связь от узла equivalentNodeDetail 
                    // к узлам front, left, right
                    if (frontND != null)
                    {
                        frontND.Rebase(equivalentND.Node);
                        AddArcFunc(equivalentND.Node, frontND.Node);
#if DEBUG && LOG
                        ETLogger.WriteLine(LogType.Debug,
                            uniDirection
                                ? $"LinkNearest3Side: UniLink {{equivalent -> front}} <{frontND.Node.X:N3};{frontND.Node.Y:N3};{frontND.Node.Z:N3}>"
                                : $"LinkNearest3Side: BiLink {{equivalent <-> front}} <{frontND.Node.X:N3};{frontND.Node.Y:N3};{frontND.Node.Z:N3}>");
#endif
                    }
                    if (leftND != null)
                    {
                        leftND.Rebase(equivalentND.Node);
                        AddArcFunc(leftND.Node, equivalentND.Node);
#if DEBUG && LOG
                        ETLogger.WriteLine(LogType.Debug,
                            uniDirection
                                ? $"LinkNearest3Side: UniLink {{left -> equivalent}} <{leftND.Node.X:N3};{leftND.Node.Y:N3};{leftND.Node.Z:N3}>"
                                : $"LinkNearest3Side: BiLink {{left <-> equivalent}} <{leftND.Node.X:N3};{leftND.Node.Y:N3};{leftND.Node.Z:N3}>");
#endif
                    }
                    if (rightND != null)
                    {
                        rightND.Rebase(equivalentND.Node);
                        AddArcFunc(rightND.Node, equivalentND.Node);
#if DEBUG && LOG
                        ETLogger.WriteLine(LogType.Debug,
                            uniDirection
                                ? $"LinkNearest3Side: UniLink {{right -> equivalent}} <{rightND.Node.X:N3};{rightND.Node.Y:N3};{rightND.Node.Z:N3}>"
                                : $"LinkNearest3Side: BiLink {{right <-> equivalent}} <{rightND.Node.X:N3};{rightND.Node.Y:N3};{rightND.Node.Z:N3}>");
#endif
                    }
                    if (backND != null
                        && backND.Distance < lastND.Distance)
                    {
                        backND.Rebase(equivalentND.Node);
                        AddArcFunc(backND.Node, equivalentND.Node);
#if DEBUG && LOG
                        ETLogger.WriteLine(LogType.Debug,
                            uniDirection
                                ? $"LinkNearest3Side: UniLink {{back -> equivalent}} <{backND.Node.X:N3};{backND.Node.Y:N3};{backND.Node.Z:N3}>"
                                : $"LinkNearest3Side: BiLink {{back <-> equivalent}} <{backND.Node.X:N3};{backND.Node.Y:N3};{backND.Node.Z:N3}>");
#endif
                    } 

                    // Если задан узел lastNodeDetail - добавляем связь с ним
#if false
                    // Проверка (lastND != null) была выполнена в начале метода
                    if (lastND != null)
#endif
                    {
                        lastND.Rebase(equivalentND.Node);
                        AddArcFunc(lastND.Node, equivalentND.Node);
#if DEBUG && LOG
                        ETLogger.WriteLine(LogType.Debug,
                            uniDirection
                                ? $"LinkNearest3Side: UniLink {{last -> equivalent}} <{lastND.Node.X:N3};{lastND.Node.Y:N3};{lastND.Node.Z:N3}>"
                                : $"LinkNearest3Side: BiLink {{last <-> equivalent}} <{lastND.Node.X:N3};{lastND.Node.Y:N3};{lastND.Node.Z:N3}>");
#endif
                    }

                    return equivalentND;
                }

                bool addResult;
                using (graph.WriteLock())
                {
                    addResult = graph.AddNode(newNode);
                }
                if (addResult)
                {
#if DEBUG && LOG
                    ETLogger.WriteLine(LogType.Debug, $"LinkNearest3Side: Add 'NEW' node <{newNode.X:N3};{newNode.Y:N3};{newNode.Z:N3}>");
#endif
                    // Добавляем связь от узла equivalentNodeDetail 
                    // к узлам front, left, right
                    if (frontND != null)
                    {
                        AddArcFunc(newNode, frontND.Node);
#if DEBUG && LOG
                        ETLogger.WriteLine(LogType.Debug,
                            uniDirection
                                ? $"LinkNearest3Side: UniLink {{new -> front}} <{frontND.Node.X:N3};{frontND.Node.Y:N3};{frontND.Node.Z:N3}>"
                                : $"LinkNearest3Side: BiLink {{new <-> front}} <{frontND.Node.X:N3};{frontND.Node.Y:N3};{frontND.Node.Z:N3}>");
#endif
                    }
                    if (leftND != null)
                    {
                        AddArcFunc(leftND.Node, newNode);
#if DEBUG && LOG
                        ETLogger.WriteLine(LogType.Debug,
                            uniDirection
                                ? $"LinkNearest3Side: UniLink {{left -> new}} <{leftND.Node.X:N3};{leftND.Node.Y:N3};{leftND.Node.Z:N3}>"
                                : $"LinkNearest3Side: BiLink {{left <-> new}} <{leftND.Node.X:N3};{leftND.Node.Y:N3};{leftND.Node.Z:N3}>");
#endif
                    }
                    if (rightND != null)
                    {
                        AddArcFunc(rightND.Node, newNode);
#if DEBUG && LOG
                        ETLogger.WriteLine(LogType.Debug,
                            uniDirection
                                ? $"LinkNearest3Side: UniLink {{(right -> new}} <{rightND.Node.X:N3};{rightND.Node.Y:N3};{rightND.Node.Z:N3}>"
                                : $"LinkNearest3Side: BiLink {{(right <-> new}} <{rightND.Node.X:N3};{rightND.Node.Y:N3};{rightND.Node.Z:N3}>");
#endif
                    }
                    // узлом в задней четверти является lastND
                    if (backND != null
                        && backND.Distance < lastND.Distance)
                    {
                        backND.Rebase(newNode);
                        AddArcFunc(backND.Node, newNode);
#if DEBUG && LOG
                        ETLogger.WriteLine(LogType.Debug,
                            uniDirection
                                ? $"LinkNearest3Side: UniLink {{back -> new}} <{backND.Node.X:N3};{backND.Node.Y:N3};{backND.Node.Z:N3}>"
                                : $"LinkNearest3Side: BiLink {{back <-> new}} <{backND.Node.X:N3};{backND.Node.Y:N3};{backND.Node.Z:N3}>");
#endif
                    } 

                    // Если задан узел lastNodeDetail - добавляем связь с ним
#if false
                    // Проверка (lastND != null) была выполнена в начале метода
                    if (lastND != null) 
#endif
                    {
                        AddArcFunc(lastND.Node, newNode);
#if DEBUG && LOG
                        ETLogger.WriteLine(LogType.Debug,
                            uniDirection
                                ? $"LinkNearest3Side: UniLink {{last -> new}} <{lastND.Node.X:N3};{lastND.Node.Y:N3};{lastND.Node.Z:N3}>"
                                : $"LinkNearest3Side: BiLink {{last <-> new}} <{lastND.Node.X:N3};{lastND.Node.Y:N3};{lastND.Node.Z:N3}>");
#endif
                    }
                    return new NodeDetail(newNode);
                }
#if DEBUG && LOG
                ETLogger.WriteLine(LogType.Debug, $"LinkNearest3Side: Fail to add Node <{newNode.X:N3};{newNode.Y:N3};{newNode.Z:N3}>");
#endif
                return lastND;
            }
            catch (ThreadAbortException ex)
            {
#if DEBUG && LOG
                ETLogger.WriteLine(LogType.Debug, ex.ToString());
#endif
                throw;
            }
            catch (ThreadInterruptedException ex)
            {
#if DEBUG && LOG
                ETLogger.WriteLine(LogType.Debug, ex.ToString());
#endif
                throw;
            }
            catch (Exception ex)
            {
#if DEBUG && LOG
                ETLogger.WriteLine(LogType.Debug, ex.ToString());
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
                EntityToolsLogger.WriteLine(LogType.Debug, $"LinkNearest3Side: ElapsedTime is {stopwatch.ElapsedMilliseconds} ms ({stopwatch.ElapsedTicks})");
#endif
            }
#endif
            //return null;
        }

        /// <summary>
        /// Добавление вершины c координатами <paramref name="pos"/> и соединение её с ближайшими вершинами в 8 направлениях:
        /// впереди, впереди-слева, слева, впереди-справа, справа, позади-слева, позади-справа и позади неё
        /// </summary>
        public static NodeDetail LinkNearest_8_Side(Vector3 pos, MapperGraphCache graph, NodeDetail lastND = null, bool uniDirection = false)
        {
            if (graph is null || pos is null || !pos.IsValid)
                return null;

            //TODO: добавить связывание с несколькими ближайшими вершинами (по сторонам света)
            if (lastND is null)
                return LinkNearest_3_Side(pos, graph, null, uniDirection); 


#if PROFILING && DEBUG
            stopwatch.Restart();
#endif
            Node newNode = new Node(pos.X, pos.Y, pos.Z);

            double maxDistance = Math.Max(10d, EntityTools.Config.Mapper.WaypointDistance * 1.25d);
            double maxZDifference = EntityTools.Config.Mapper.MaxElevationDifference;
            double equivalenceDistance = EntityTools.Config.Mapper.WaypointEquivalenceDistance;

            // Ближайшие узлы разделенные по направлению движения
            NodeDetail frontND = null; // впереди (+/-15)
            NodeDetail frontLeftND = null; // впереди-слева (15~75)
            NodeDetail frontRightND = null; // впереди-слева -(15~75)
            NodeDetail leftND = null; // слева (75~105)
            NodeDetail leftBackND = null; // слева-сзади (105~165)
            NodeDetail rightND = null; // справа -(75~105)
            NodeDetail rightBackND = null; // справа-сзади -(105~165)
            NodeDetail backND = null; //сзади
            // Косое произведение веторов A и B
            // D = Ax * By - Bx * Ay
            // Если D == 0 - вектора коллиниарны
            // Если D > 0 - точка B в верхней полуплоскости
            // Если D < 0 - точка B в нижней полуплоскости

            try
            {
                // сканируем узлы графа, для определения ближайших к добавляемому узлу newNode
                NodeDetail equivalentND = null;
                using (graph.ReadLock())
                {
                    foreach (Node currentNode in graph.NodesCollection)
                    {
                        if (NodesEquals(lastND.Node, currentNode))
                            continue;

                        //Проверяем разницу высот
                        NodeDetail currentND = new NodeDetail(currentNode, newNode);
                        if (Math.Abs(currentND.Z) < maxZDifference)
                        {
                            // Разница высот в пределах допустимой величины,
                            // проверяем расстояние от добавляемой точки до текущей
                            if (currentND.Distance < equivalenceDistance)
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
                                if (equivalentND is null || currentND.Distance < equivalentND.Distance)
                                    equivalentND = currentND;
#endif
                            }
                            else if (currentND.Distance <= maxDistance)
                            {
                                // вычисляем косинус угла меду векторами для определения четверти (впереди, сзади, слева, справа)
                                // в которой находится curNode
                                double cos = CosineOxy(lastND, currentND);
                                // cos(165) = -0.9659258262890682867497431997289
                                // cos(105) = -0.25881904510252076234889883762405
                                // cos(75) = 0.25881904510252076234889883762405
                                // cos(15) = 0.9659258262890682867497431997289
                                if (cos < -0.9659258262890682867497431997289) // угол больше 165
                                {
                                    // Угол между векторами больше 165 градусов
                                    // Значит точка находится впереди
                                    if (frontND is null
                                        || frontND.Distance > currentND.Distance)
                                        frontND = currentND;
                                }
                                else 
                                {
                                    // Вычисляем в какой полуплоскости находится currentND
                                    // относительно вектора,
                                    // который направлен от newNode к lastND
                                    double D = lastND.X * currentND.Y - currentND.X * lastND.Y;

                                    if(cos < -0.25881904510252076234889883762405) // угол (105~165)
                                    {
                                        // Угол между векторами (105~165) градусов
                                        // Значит точка находится впереди-слева или впереди-справа
                                        if (D > 0)
                                        {
                                            if (frontRightND is null
                                                || frontRightND.Distance > currentND.Distance)
                                                frontRightND = currentND;
                                        }
                                        else
                                        {
                                            if (frontLeftND is null
                                                || frontLeftND.Distance > currentND.Distance)
                                                frontLeftND = currentND;
                                        }
                                    }
                                    else if (cos < 0.25881904510252076234889883762405) // угол (75~105)
                                    {
                                        // Угол между векторами (75~105) градусов
                                        // Значит точка находится впереди-слева или впереди-справа
                                        if (D > 0)
                                        {
                                            if (rightND is null
                                                || rightND.Distance > currentND.Distance)
                                                rightND = currentND;
                                        }
                                        else
                                        {
                                            if (leftND is null
                                                || leftND.Distance > currentND.Distance)
                                                leftND = currentND;
                                        }
                                    }
                                    else if (cos < 0.9659258262890682867497431997289) // угол (15~75)
                                    {
                                        // Угол между векторами (15~75) градусов
                                        // Значит точка находится впереди-слева или впереди-справа
                                        if (D > 0)
                                        {
                                            if (rightBackND is null
                                                || rightBackND.Distance > currentND.Distance)
                                                rightBackND = currentND;
                                        }
                                        else
                                        {
                                            if (leftBackND is null
                                                || leftBackND.Distance > currentND.Distance)
                                                leftBackND = currentND;
                                        }
                                    }
                                    else if (backND is null
                                             || backND.Distance > currentND.Distance)  // угол +/-15
                                        backND = currentND;
                                }
                            }
                        }
                    }
                }

                // Формирование функтора добавление граней, в зависимости от типа пути (одно- или двунаправленный)
                Action<Node, Node> AddArcFunc;
                if (uniDirection)
                    AddArcFunc = (n1, n2) => {
                        using (graph.WriteLock())
                            graph.AddArc(n1, n2);
                    };
                else AddArcFunc = (n1, n2) => {
                    using (graph.WriteLock())
                        graph.Add2Arcs(n1, n2);
                };

                if (equivalentND != null)
                {
#if DEBUG && LOG
                    ETLogger.WriteLine(LogType.Debug, $"LinkNearest8Side: Detect 'EQUIVALENT' Node <{equivalentND.Node.X:N3};{equivalentND.Node.Y:N3};{equivalentND.Node.Z:N3}>");
#endif
                    // Добавляем связь от узла equivalentND
                    // к узлам front, left, right и итд
                    if (frontND != null)
                    {
                        frontND.Rebase(equivalentND.Node);
                        AddArcFunc(equivalentND.Node, frontND.Node);
#if DEBUG && LOG
                        ETLogger.WriteLine(LogType.Debug,
                            uniDirection
                                ? $"LinkNearest8Side: UniLink {{equivalent -> front}} <{frontND.Node.X:N3};{frontND.Node.Y:N3};{frontND.Node.Z:N3}>"
                                : $"LinkNearest8Side: BiLink {{equivalent <-> front}} <{frontND.Node.X:N3};{frontND.Node.Y:N3};{frontND.Node.Z:N3}>");
#endif
                    }
                    if (frontLeftND != null)
                    {
                        frontLeftND.Rebase(equivalentND.Node);
                        AddArcFunc(frontLeftND.Node, equivalentND.Node);
#if DEBUG && LOG
                        ETLogger.WriteLine(LogType.Debug,
                            uniDirection
                                ? $"LinkNearest8Side: UniLink {{frontLeft -> equivalent}} <{frontLeftND.Node.X:N3};{frontLeftND.Node.Y:N3};{frontLeftND.Node.Z:N3}>"
                                : $"LinkNearest8Side: BiLink {{frontLeft <-> equivalent}} <{frontLeftND.Node.X:N3};{frontLeftND.Node.Y:N3};{frontLeftND.Node.Z:N3}>");
#endif
                    }
                    if (frontRightND != null)
                    {
                        frontRightND.Rebase(equivalentND.Node);
                        AddArcFunc(frontRightND.Node, equivalentND.Node);
#if DEBUG && LOG
                        ETLogger.WriteLine(LogType.Debug,
                            uniDirection
                                ? $"LinkNearest8Side: UniLink {{frontRight -> equivalent}} <{frontRightND.Node.X:N3};{frontRightND.Node.Y:N3};{frontRightND.Node.Z:N3}>"
                                : $"LinkNearest8Side: BiLink {{frontRight <-> equivalent}} <{frontRightND.Node.X:N3};{frontRightND.Node.Y:N3};{frontRightND.Node.Z:N3}>");
#endif
                    }
                    if (leftND != null)
                    {
                        leftND.Rebase(equivalentND.Node);
                        AddArcFunc(leftND.Node, equivalentND.Node);
#if DEBUG && LOG
                        ETLogger.WriteLine(LogType.Debug,
                            uniDirection
                                ? $"LinkNearest8Side: UniLink {{left -> equivalent}} <{leftND.Node.X:N3};{leftND.Node.Y:N3};{leftND.Node.Z:N3}>"
                                : $"LinkNearest8Side: BiLink {{left <-> equivalent}} <{leftND.Node.X:N3};{leftND.Node.Y:N3};{leftND.Node.Z:N3}>");
#endif
                    }
                    if (rightND != null)
                    {
                        rightND.Rebase(equivalentND.Node);
                        AddArcFunc(rightND.Node, equivalentND.Node);
#if DEBUG && LOG
                        ETLogger.WriteLine(LogType.Debug,
                            uniDirection
                                ? $"LinkNearest8Side: UniLink {{right -> equivalent}} <{rightND.Node.X:N3};{rightND.Node.Y:N3};{rightND.Node.Z:N3}>"
                                : $"LinkNearest8Side: BiLink {{right <-> equivalent}} <{rightND.Node.X:N3};{rightND.Node.Y:N3};{rightND.Node.Z:N3}>");
#endif
                    }
                    if (leftBackND != null)
                    {
                        leftBackND.Rebase(equivalentND.Node);
                        AddArcFunc(leftBackND.Node, equivalentND.Node);
#if DEBUG && LOG
                        ETLogger.WriteLine(LogType.Debug,
                            uniDirection
                                ? $"LinkNearest8Side: UniLink {{leftBack -> equivalent}} <{leftBackND.Node.X:N3};{leftBackND.Node.Y:N3};{leftBackND.Node.Z:N3}>"
                                : $"LinkNearest8Side: BiLink {{leftBack <-> equivalent}} <{leftBackND.Node.X:N3};{leftBackND.Node.Y:N3};{leftBackND.Node.Z:N3}>");
#endif
                    }
                    if (rightBackND != null)
                    {
                        rightBackND.Rebase(equivalentND.Node);
                        AddArcFunc(rightBackND.Node, equivalentND.Node);
#if DEBUG && LOG
                        ETLogger.WriteLine(LogType.Debug,
                            uniDirection
                                ? $"LinkNearest8Side: UniLink {{rightBack -> equivalent}} <{rightBackND.Node.X:N3};{rightBackND.Node.Y:N3};{rightBackND.Node.Z:N3}>"
                                : $"LinkNearest8Side: BiLink {{rightBack <-> equivalent}} <{rightBackND.Node.X:N3};{rightBackND.Node.Y:N3};{rightBackND.Node.Z:N3}>");
#endif
                    }
                    if (backND != null
                        && backND.Distance < lastND.Distance)
                    {
                        backND.Rebase(equivalentND.Node);
                        AddArcFunc(backND.Node, equivalentND.Node);
#if DEBUG && LOG && false
                        ETLogger.WriteLine(LogType.Debug,
                            uniDirection
                                ? $"LinkNearest8Side: UniLink {{back -> equivalent}} <{backND.Node.X:N3};{backND.Node.Y:N3};{backND.Node.Z:N3}>"
                                : $"LinkNearest8Side: BiLink {{back <-> equivalent}} <{backND.Node.X:N3};{backND.Node.Y:N3};{backND.Node.Z:N3}>");
#endif
                        AddArcFunc(lastND.Node, backND.Node);
#if DEBUG && LOG
                        ETLogger.WriteLine(LogType.Debug,
                            uniDirection
                                ? $"LinkNearest8Side: UniLink {{last -> back -> equivalent}} <{backND.Node.X:N3};{backND.Node.Y:N3};{backND.Node.Z:N3}>"
                                : $"LinkNearest8Side: BiLink {{last <-> back <-> equivalent}} <{backND.Node.X:N3};{backND.Node.Y:N3};{backND.Node.Z:N3}>");
#endif
                    }

                    // Если задан узел lastNodeDetail - добавляем связь с ним
#if false
                    // Проверка (lastND != null) была выполнена в начале метода
                    if (lastND != null)
#endif
                    {
                        lastND.Rebase(equivalentND.Node);
                        AddArcFunc(lastND.Node, equivalentND.Node);
#if DEBUG && LOG
                        ETLogger.WriteLine(LogType.Debug,
                            uniDirection
                                ? $"LinkNearest8Side: UniLink {{last -> equivalent}} <{lastND.Node.X:N3};{lastND.Node.Y:N3};{lastND.Node.Z:N3}>"
                                : $"LinkNearest8Side: BiLink {{last <-> equivalent}} <{lastND.Node.X:N3};{lastND.Node.Y:N3};{lastND.Node.Z:N3}>");
#endif
                    }

                    return equivalentND;
                }

                bool addResult;
                using (graph.WriteLock())
                {
                    addResult = graph.AddNode(newNode);
                }
                if (addResult)
                {
#if DEBUG && LOG
                    ETLogger.WriteLine(LogType.Debug, $"LinkNearest8Side: Add 'NEW' node <{newNode.X:N3};{newNode.Y:N3};{newNode.Z:N3}>");
#endif
                    // Добавляем связь от узла equivalentNodeDetail 
                    // к узлам front, left, right
                    if (frontND != null)
                    {
                        AddArcFunc(newNode, frontND.Node);
#if DEBUG && LOG
                        ETLogger.WriteLine(LogType.Debug,
                            uniDirection
                                ? $"LinkNearest8Side: UniLink {{new -> front}} <{frontND.Node.X:N3};{frontND.Node.Y:N3};{frontND.Node.Z:N3}>"
                                : $"LinkNearest8Side: BiLink {{new <-> front}} <{frontND.Node.X:N3};{frontND.Node.Y:N3};{frontND.Node.Z:N3}>");
#endif
                    }
                    if (frontLeftND != null)
                    {
                        AddArcFunc(newNode, frontLeftND.Node);
#if DEBUG && LOG
                        ETLogger.WriteLine(LogType.Debug,
                            uniDirection
                                ? $"LinkNearest8Side: UniLink {{new -> frontLeft}} <{frontLeftND.Node.X:N3};{frontLeftND.Node.Y:N3};{frontLeftND.Node.Z:N3}>"
                                : $"LinkNearest8Side: BiLink {{new <-> frontLeft}} <{frontLeftND.Node.X:N3};{frontLeftND.Node.Y:N3};{frontLeftND.Node.Z:N3}>");
#endif
                    }
                    if (frontRightND != null)
                    {
                        AddArcFunc(newNode, frontRightND.Node);
#if DEBUG && LOG
                        ETLogger.WriteLine(LogType.Debug,
                            uniDirection
                                ? $"LinkNearest8Side: UniLink {{new -> frontRight}} <{frontRightND.Node.X:N3};{frontRightND.Node.Y:N3};{frontRightND.Node.Z:N3}>"
                                : $"LinkNearest8Side: BiLink {{new <-> frontRight}} <{frontRightND.Node.X:N3};{frontRightND.Node.Y:N3};{frontRightND.Node.Z:N3}>");
#endif
                    }
                    if (leftND != null)
                    {
                        AddArcFunc(leftND.Node, newNode);
#if DEBUG && LOG
                        ETLogger.WriteLine(LogType.Debug,
                            uniDirection
                                ? $"LinkNearest8Side: UniLink {{left -> new}} <{leftND.Node.X:N3};{leftND.Node.Y:N3};{leftND.Node.Z:N3}>"
                                : $"LinkNearest8Side: BiLink {{left <-> new}} <{leftND.Node.X:N3};{leftND.Node.Y:N3};{leftND.Node.Z:N3}>");
#endif
                    }
                    if (rightND != null)
                    {
                        AddArcFunc(rightND.Node, newNode);
#if DEBUG && LOG
                        ETLogger.WriteLine(LogType.Debug,
                            uniDirection
                                ? $"LinkNearest8Side: UniLink {{(right -> new}} <{rightND.Node.X:N3};{rightND.Node.Y:N3};{rightND.Node.Z:N3}>"
                                : $"LinkNearest8Side: BiLink {{(right <-> new}} <{rightND.Node.X:N3};{rightND.Node.Y:N3};{rightND.Node.Z:N3}>");
#endif
                    }
                    if (leftBackND != null)
                    {
                        AddArcFunc(leftBackND.Node, newNode);
#if DEBUG && LOG
                        ETLogger.WriteLine(LogType.Debug,
                            uniDirection
                                ? $"LinkNearest8Side: UniLink {{leftBack -> new}} <{leftBackND.Node.X:N3};{leftBackND.Node.Y:N3};{leftBackND.Node.Z:N3}>"
                                : $"LinkNearest8Side: BiLink {{leftBack <-> new}} <{leftBackND.Node.X:N3};{leftBackND.Node.Y:N3};{leftBackND.Node.Z:N3}>");
#endif
                    }
                    if (rightBackND != null)
                    {
                        AddArcFunc(rightBackND.Node, newNode);
#if DEBUG && LOG
                        ETLogger.WriteLine(LogType.Debug,
                            uniDirection
                                ? $"LinkNearest8Side: UniLink {{(rightBack -> new}} <{rightBackND.Node.X:N3};{rightBackND.Node.Y:N3};{rightBackND.Node.Z:N3}>"
                                : $"LinkNearest8Side: BiLink {{(rightBack <-> new}} <{rightBackND.Node.X:N3};{rightBackND.Node.Y:N3};{rightBackND.Node.Z:N3}>");
#endif
                    }
                    // узлом в задней четверти является lastND
                    if (backND != null
                        && backND.Distance < lastND.Distance)
                    {
                        backND.Rebase(newNode);
                        AddArcFunc(backND.Node, newNode);
#if DEBUG && LOG && false
                        ETLogger.WriteLine(LogType.Debug,
                            uniDirection
                                ? $"LinkNearest8Side: UniLink {{back -> new}} <{backND.Node.X:N3};{backND.Node.Y:N3};{backND.Node.Z:N3}>"
                                : $"LinkNearest8Side: BiLink {{back <-> new}} <{backND.Node.X:N3};{backND.Node.Y:N3};{backND.Node.Z:N3}>");
#endif
                        AddArcFunc(lastND.Node, backND.Node);
#if DEBUG && LOG
                        ETLogger.WriteLine(LogType.Debug,
                            uniDirection
                                ? $"LinkNearest8Side: UniLink {{last -> back -> new}} <{backND.Node.X:N3};{backND.Node.Y:N3};{backND.Node.Z:N3}>"
                                : $"LinkNearest8Side: BiLink {{last <-> back <-> new}} <{backND.Node.X:N3};{backND.Node.Y:N3};{backND.Node.Z:N3}>");
#endif
                    }

                    // Если задан узел lastNodeDetail - добавляем связь с ним
#if false
                    // Проверка (lastND != null) была выполнена в начале метода
                    if (lastND != null) 
#endif
                    {
                        AddArcFunc(lastND.Node, newNode);
#if DEBUG && LOG
                        ETLogger.WriteLine(LogType.Debug,
                            uniDirection
                                ? $"LinkNearest8Side: UniLink {{last -> new}} <{lastND.Node.X:N3};{lastND.Node.Y:N3};{lastND.Node.Z:N3}>"
                                : $"LinkNearest8Side: BiLink {{last <-> new}} <{lastND.Node.X:N3};{lastND.Node.Y:N3};{lastND.Node.Z:N3}>");
#endif
                    }
                    return new NodeDetail(newNode);
                }
#if DEBUG && LOG
                ETLogger.WriteLine(LogType.Debug, $"LinkNearest8Side: Fail to add Node <{newNode.X:N3};{newNode.Y:N3};{newNode.Z:N3}>");
#endif
                return lastND;
            }
            catch (ThreadAbortException ex)
            {
#if DEBUG && LOG
                ETLogger.WriteLine(LogType.Debug, ex.ToString());
#endif
                throw;
            }
            catch (ThreadInterruptedException ex)
            {
#if DEBUG && LOG
                ETLogger.WriteLine(LogType.Debug, ex.ToString());
#endif
                throw;
            }
            catch (Exception ex)
            {
#if DEBUG && LOG
                ETLogger.WriteLine(LogType.Debug, ex.ToString());
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
                EntityToolsLogger.WriteLine(LogType.Debug, $"LinkNearest3Side: ElapsedTime is {stopwatch.ElapsedMilliseconds} ms ({stopwatch.ElapsedTicks})");
#endif
            }
#endif
            //return null;
        }

        /// <summary>
        /// Добавление вершины c координатами <paramref name="pos"/> и соединение её с последней добавленной вершиной <paramref name="lastND"/>
        /// </summary>
        public static NodeDetail LinkLast(Vector3 pos, MapperGraphCache graph, NodeDetail lastND = null, bool uniDirection = false)
        {
            if (graph is null || pos is null || !pos.IsValid)
                return null;


#if PROFILING && DEBUG
            stopwatch.Restart();
#endif
#if false
            double minNodeDistance = EntityTools.Config.Mapper.WaypointDistance;
            double maxDistance = Math.Max(10d, EntityTools.Config.Mapper.WaypointDistance * 1.25d);
#endif
            double maxZDifference = EntityTools.Config.Mapper.MaxElevationDifference;
            double equivalenceDistance = EntityTools.Config.Mapper.WaypointEquivalenceDistance;

            Node newNode = new Node(pos.X, pos.Y, pos.Z);

            NodeDetail equivalentND = null;

            try
            {
                // сканируем узлы графа, для определения эквивалентного узла
                using (graph.ReadLock())
                    foreach (Node node in graph.NodesCollection)
                    {
                        //Проверяем разницу высот
                        NodeDetail currentND = new NodeDetail(node, newNode);
                        if (Math.Abs(currentND.Z) < maxZDifference)
                        {
                            // Разница высот в пределах допустимой величины,
                            // проверяем расстояние от добавляемой точки до текущей
                            if (currentND.Distance < equivalenceDistance)
                            {
                                // NewNode CurNode в пределах equivalenceDistance
                                if (equivalentND is null
                                    || equivalentND.Distance > currentND.Distance)
                                    equivalentND = currentND;
                            }
                            /*else if (currentND.Distance <= minNodeDistance
                                        && (nearestND == null || currentND.Distance < nearestND.Distance))
                            {
                                // currentND - ближайший существующий узел к newNode
                                nearestND = currentND;
                            }*/
                        }
                    }

                if (equivalentND != null)
                {
                    if (lastND?.Node != null)
                    {
                        lastND.Rebase(equivalentND.Node);
#if DEBUG && LOG
                        ETLogger.WriteLine(LogType.Debug, $"LinkLast: Detect 'EQUIVALENT' Node <{equivalentND.Node.X:N3};{equivalentND.Node.Y:N3};{equivalentND.Node.Z:N3}>");
#endif
                        if (uniDirection)
                        {
                            using (graph.WriteLock())
                                graph.AddArc(lastND.Node, equivalentND.Node);
#if DEBUG && LOG
                            ETLogger.WriteLine(LogType.Debug, $"LinkLast: UniLink {{last -> equivalent}} <{lastND.Node.X:N3};{lastND.Node.Y:N3};{lastND.Node.Z:N3}>");
#endif
                        }
                        else
                        {
                            using (graph.WriteLock())
                                graph.Add2Arcs(lastND.Node, equivalentND.Node); 
#if DEBUG && LOG
                            ETLogger.WriteLine(LogType.Debug, $"LinkLast: BiLink {{last <-> equivalent}} <{lastND.Node.X:N3};{lastND.Node.Y:N3};{lastND.Node.Z:N3}>");
#endif
                        }
                    }
                    else
                    {
                        // предыдущий узел "отсутствует"
                        // связывать не с чем
#if DEBUG && LOG
                        ETLogger.WriteLine(LogType.Debug, $"LinkLast: Nothing to link to detected 'EQUIVALENT' Node <{equivalentND.Node.X:N3};{equivalentND.Node.Y:N3};{equivalentND.Node.Z:N3}>");
#endif
                    }
                    return equivalentND;
                }

                bool addResult;
                if (lastND?.Node != null)
                {
                    // Вставляем узел и связываем его с предыдущим узлом lastNode
                    using (graph.WriteLock())
                    {
                        addResult = graph.AddNode(newNode);
                    }
                    if (addResult)
                    {
#if DEBUG && LOG
                        ETLogger.WriteLine(LogType.Debug, $"LinkLast: Add 'NEW' Node <{newNode.X:N3};{newNode.Y:N3};{newNode.Z:N3}>");
#endif
                        if (uniDirection)
                        {
                            using (graph.WriteLock())
                                graph.AddArc(lastND.Node, newNode, lastND.Distance); 
#if DEBUG && LOG
                            ETLogger.WriteLine(LogType.Debug, $"LinkLast: UniLink {{last -> ew}} <{lastND.Node.X:N3};{lastND.Node.Y:N3};{lastND.Node.Z:N3}>");
#endif
                        }
                        else
                        {
                            using (graph.WriteLock())
                                graph.Add2Arcs(lastND.Node, newNode, lastND.Distance);
#if DEBUG && LOG
                            ETLogger.WriteLine(LogType.Debug, $"LinkLast: BiLink {{last <-> ew}} <{lastND.Node.X:N3};{lastND.Node.Y:N3};{lastND.Node.Z:N3}>");
#endif
                        }
                        return new NodeDetail(newNode);
                    }
#if DEBUG && LOG
                    ETLogger.WriteLine(LogType.Debug, $"LinkLast: Fail to add Node <{newNode.X:N3};{newNode.Y:N3};{newNode.Z:N3}>");
#endif
                    return lastND;
                }

                // предыдущий узел "отсутствует"
                // связывать не с чем
                using (graph.WriteLock())
                    addResult = graph.AddNode(newNode);
                if (addResult)
                {
#if DEBUG && LOG
                    ETLogger.WriteLine(LogType.Debug, $"LinkLast: Add 'NEW' Node <{newNode.X:N3};{newNode.Y:N3};{newNode.Z:N3}> without links");
#endif
                    return new NodeDetail(newNode);
                }
#if DEBUG && LOG
                ETLogger.WriteLine(LogType.Debug, $"LinkLast: Fail to add Node <{newNode.X:N3};{newNode.Y:N3};{newNode.Z:N3}>");
#endif
                return null;
            }
            catch (ThreadAbortException ex)
            {
#if DEBUG && LOG
                ETLogger.WriteLine(LogType.Debug, ex.ToString());
#endif
                throw;
            }
            catch (ThreadInterruptedException ex)
            {
#if DEBUG && LOG
                ETLogger.WriteLine(LogType.Debug, ex.ToString());
#endif
                throw;
            }
#if DEBUG
            catch (Exception ex)
            {
                ETLogger.WriteLine(LogType.Debug, ex.ToString());
                return null;
            }
#else
            catch { return null; }
#endif
#if PROFILING && DEBUG
            finally
            {
                stopwatch.Stop();
                WatchCounter++;
                totalWatchTiks += stopwatch.ElapsedTicks;
                totalWatchMs += stopwatch.ElapsedMilliseconds;
#if DEBUG && LOG
                Logger.WriteLine(Logger.LogType.Debug, $"LinkLast: ElapsedTime is {stopwatch.ElapsedMilliseconds} ms ({stopwatch.ElapsedTicks})");
#endif
            }
#endif
        }

        /// <summary>
        /// Добавление вершины c координатами <paramref name="pos"/> и соединение её с ближайшей вершиной
        /// </summary>
        public static NodeDetail LinkNearest_1(Vector3 pos, MapperGraphCache graph, NodeDetail lastND = null, bool uniDirection = false)
        {
            if (graph is null || pos is null || !pos.IsValid)
                return null;

#if PROFILING && DEBUG
            stopwatch.Restart();
#endif
            Node newNode = new Node(pos.X, pos.Y, pos.Z);

            double maxDistance = Math.Max(10d, EntityTools.Config.Mapper.WaypointDistance * 1.25d);
            double maxZDifference = EntityTools.Config.Mapper.MaxElevationDifference;
            double equivalenceDistance = EntityTools.Config.Mapper.WaypointEquivalenceDistance;
            NodeDetail nearestND = null; // ближайший узел
            NodeDetail equivalentND = null;// узел, считающийся "эквивалентным" newNode, в результате чего newNode не добавляется к графу

            try
            {
                // сканируем узлы графа, для определения ближайшего
                using (graph.ReadLock())
                    foreach (Node node in graph.NodesCollection)
                    {
                        //Проверяем разницу высот
                        NodeDetail currentND = new NodeDetail(node, newNode);
                        if (Math.Abs(currentND.Z) < maxZDifference)
                        {
                            // Разница высот в пределах допустимой величины,
                            // проверяем расстояние от добавляемой точки до текущей
                            if (currentND.Distance < equivalenceDistance)
                            {
                                // currentND в пределах equivalenceDistance от newNode
                                if (equivalentND is null
                                    || equivalentND.Distance > currentND.Distance)
                                    // currentND ближе к newNod, чем ранее найденная equivalentND
                                    equivalentND = currentND;
                            }
                            else if (currentND.Distance <= maxDistance
                                     && (nearestND is null || currentND.Distance < nearestND.Distance))
                            {
                                // currentND - ближайший существующий узел к newNode
                                nearestND = currentND;
                            }
                        }
                    }

                if (equivalentND != null)
                {
#if DEBUG && LOG
                    ETLogger.WriteLine(LogType.Debug,
                        $"LinkNearest1: Detect 'EQUIVALENT' node <{equivalentND.Node.X:N3};{equivalentND.Node.Y:N3};{equivalentND.Node.Z:N3}>");
#endif
                    // Найден эквивалентный узел
                    // строим связи к equivalentND
                    if (lastND != null)
                    {
                        lastND.Rebase(equivalentND.Node);
                        if (uniDirection)
                        {
                            using (graph.WriteLock())
                                graph.AddArc(lastND.Node, equivalentND.Node);
#if DEBUG && LOG
                            ETLogger.WriteLine(LogType.Debug,
                                $"LinkNearest1: UniLink {{last  -> equivalent}} <{lastND.Node.X:N3};{lastND.Node.Y:N3};{lastND.Node.Z:N3}>");
#endif
                        }
                        else
                        {
                            using (graph.WriteLock())
                                graph.Add2Arcs(lastND.Node, equivalentND.Node);
#if DEBUG && LOG
                            ETLogger.WriteLine(LogType.Debug,
                                $"LinkNearest1: BiLink {{last < -> equivalent}} <{lastND.Node.X:N3};{lastND.Node.Y:N3};{lastND.Node.Z:N3}>");
#endif
                        }
                    }

                    if (nearestND != null)
                    {
                        nearestND.Rebase(equivalentND.Node);
                        if (uniDirection)
                        {
                            using (graph.WriteLock())
                                graph.AddArc(nearestND.Node, equivalentND.Node);
#if DEBUG && LOG
                            ETLogger.WriteLine(LogType.Debug,
                                $"LinkNearest1: UniLink {{nearest -> equivalent}} <{nearestND.Node.X:N3};{nearestND.Node.Y:N3};{nearestND.Node.Z:N3}>");
#endif
                        }
                        else
                        {
                            using (graph.WriteLock())
                                graph.Add2Arcs(nearestND.Node, equivalentND.Node);
#if DEBUG && LOG
                            ETLogger.WriteLine(LogType.Debug,
                                $"LinkNearest1: BiLink {{nearest -> equivalent}} <{nearestND.Node.X:N3};{nearestND.Node.Y:N3};{nearestND.Node.Z:N3}>");
#endif
                        }
                    }

                    return equivalentND;
                }

                // Вставляем в Граф узел newNode
                // связываем его с предыдущим узлом lastNode и с ближайшим узлом
                bool addResult;
                using (graph.WriteLock())
                    addResult = graph.AddNode(newNode);
                if (addResult)
                {
                    // newNode успешно добавлен в граф
#if DEBUG && LOG
                    ETLogger.WriteLine(LogType.Debug,
                        $"LinkNearest1: Add 'NEW' node <{newNode.X:N3};{newNode.Y:N3};{newNode.Z:N3}>");
#endif
                    if (lastND != null)
                    {
                        if (uniDirection)
                        {
                            using (graph.WriteLock())
                                graph.AddArc(lastND.Node, newNode, lastND.Distance);
#if DEBUG && LOG
                            ETLogger.WriteLine(LogType.Debug,
                                $"LinkNearest1: Link {{last -> new}} <{lastND.Node.X:N3};{lastND.Node.Y:N3};{lastND.Node.Z:N3}>");
#endif
                        }
                        else
                        {
                            using (graph.WriteLock())
                                graph.Add2Arcs(lastND.Node, newNode, lastND.Distance);
#if DEBUG && LOG
                            ETLogger.WriteLine(LogType.Debug,
                                $"LinkNearest1: Link {{last <-> new}} <{lastND.Node.X:N3};{lastND.Node.Y:N3};{lastND.Node.Z:N3}>");
#endif
                        }
                    }

                    if (nearestND != null)
                    {
                        if (uniDirection)
                        {
                            using (graph.WriteLock())
                                graph.AddArc(nearestND.Node, newNode, nearestND.Distance);
#if DEBUG && LOG
                            ETLogger.WriteLine(LogType.Debug,
                                $"LinkNearest1: Link {{nearest -> new}} <{nearestND.Node.X:N3};{nearestND.Node.Y:N3};{nearestND.Node.Z:N3}>");
#endif
                        }
                        else
                        {
                            using (graph.WriteLock())
                                graph.Add2Arcs(nearestND.Node, newNode, nearestND.Distance);
#if DEBUG && LOG
                            ETLogger.WriteLine(LogType.Debug,
                                $"LinkNearest1: Link {{nearestNod <->ewNode}} <{nearestND.Node.X:N3};{nearestND.Node.Y:N3};{nearestND.Node.Z:N3}>");
#endif
                        }

                    }
#if DEBUG && LOG
                    ETLogger.WriteLine(LogType.Debug,
                        $"LinkNearest1: Link newNode <{newNode.X:N3};{newNode.Y:N3};{newNode.Z:N3}>");
#endif
                    return new NodeDetail(newNode);
                }
#if DEBUG && LOG
                ETLogger.WriteLine(LogType.Debug,
                    $"LinkNearest1: Fail to add Node <{newNode.X:N3};{newNode.Y:N3};{newNode.Z:N3}>");
#endif
                return lastND;
            }
            catch (ThreadAbortException ex)
            {
#if DEBUG && LOG
                ETLogger.WriteLine(LogType.Debug, ex.ToString());
#endif
                throw;
            }
            catch (ThreadInterruptedException ex)
            {
#if DEBUG && LOG
                ETLogger.WriteLine(LogType.Debug, ex.ToString());
#endif
                throw;
            }
#if DEBUG
            catch (Exception ex)
            {
                ETLogger.WriteLine(LogType.Debug, ex.ToString());
                return null;
            }
#else
            catch { return null; }
#endif
#if PROFILING && DEBUG
            finally
            {
                stopwatch.Stop();
                WatchCounter++;
                totalWatchTiks += stopwatch.ElapsedTicks;
                totalWatchMs += stopwatch.ElapsedMilliseconds;
#if DEBUG && LOG
                ETLogger.WriteLine(LogType.Debug, $"LinkNearest1: ElapsedTime is {stopwatch.ElapsedMilliseconds} ms ({stopwatch.ElapsedTicks})");
#endif
            }
#endif

            //return null;
        }

        /// <summary>
        /// Добавление вершины c координатами <paramref name="pos"/> и соединение её с последней <paramref name="lastND"/> и с ближайшей вершиной впереди
        /// </summary>
        public static NodeDetail LinkLinear(Vector3 pos, MapperGraphCache graph, NodeDetail lastND = null, bool uniDirection = false)
        {
            if (graph is null || pos is null || !pos.IsValid)
                return null;

#if PROFILING && DEBUG
            stopwatch.Restart();
#endif
            Node newNode = new Node(pos.X, pos.Y, pos.Z);

            double maxDistance = Math.Max(10d, EntityTools.Config.Mapper.WaypointDistance * 1.25d);
            double maxZDifference = EntityTools.Config.Mapper.MaxElevationDifference;
            double equivalenceDistance = EntityTools.Config.Mapper.WaypointEquivalenceDistance;
#if false
            NodeDetail nearestND = null; // ближайший узел  
#endif
            NodeDetail frontND = null; // впереди 
            NodeDetail equivalentND = null;// узел, считающийся "эквивалентным" newNode, в результате чего newNode не добавляется к графу

            try
            {
                // сканируем узлы графа, для определения ближайшего
                using (graph.ReadLock())
                    foreach (Node node in graph.NodesCollection)
                    {
                        //Проверяем разницу высот
                        NodeDetail currentND = new NodeDetail(node, newNode);
                        if (Math.Abs(currentND.Z) < maxZDifference)
                        {
                            // Разница высот в пределах допустимой величины,
                            // проверяем расстояние от добавляемой точки до текущей
                            if (currentND.Distance < equivalenceDistance)
                            {
                                // currentND в пределах equivalenceDistance от newNode
                                if (equivalentND is null
                                    || equivalentND.Distance > currentND.Distance)
                                    // currentND ближе к newNod, чем ранее найденная equivalentND
                                    equivalentND = currentND;
                            }
                            else if (currentND.Distance <= maxDistance)
                            {
                                // вычисляем косинус угла меду векторами для определения четверти (впереди, сзади, слева, справа)
                                // в которой находится curNode
                                double cos = CosineOxy(lastND, currentND);

                                if (cos < -0.7071d)
                                {
                                    // Угол между векторами больше 135 градусов
                                    // Значит точка находится впереди
                                    if (frontND is null
                                        || frontND.Distance > currentND.Distance)
                                        frontND = currentND;
                                }
#if false
                                else if (nearestND is null || currentND.Distance < nearestND.Distance)
                                {
                                    // currentND - ближайший существующий узел к newNode
                                    nearestND = currentND;
                                } 
#endif
                            }
                        }
                    }

                if (equivalentND != null)
                {
#if DEBUG && LOG
                    ETLogger.WriteLine(LogType.Debug,
                        $"LinkLinear: Detect 'EQUIVALENT' node <{equivalentND.Node.X:N3};{equivalentND.Node.Y:N3};{equivalentND.Node.Z:N3}>");
#endif
                    // Добавляем связь от узла equivalentNodeDetail 
                    // к узлам front, left, right
                    if (frontND != null)
                    {
                        frontND.Rebase(equivalentND.Node);
                        if (uniDirection)
                        {
                            using (graph.WriteLock())
                                graph.AddArc(equivalentND.Node, frontND.Node);
#if DEBUG && LOG
                            ETLogger.WriteLine(LogType.Debug,
                                $"LinkLinear: UniLink {{equivalent -> front}} <{frontND.Node.X:N3};{frontND.Node.Y:N3};{frontND.Node.Z:N3}>");
#endif
                        }
                        else
                        {
                            using (graph.WriteLock())
                                graph.Add2Arcs(equivalentND.Node, frontND.Node);
#if DEBUG && LOG
                            ETLogger.WriteLine(LogType.Debug,
                                $"LinkLinear: BiLink {{equivalent <-> front}} <{frontND.Node.X:N3};{frontND.Node.Y:N3};{frontND.Node.Z:N3}>");
#endif
                        }
                    }

                    // Найден эквивалентный узел
                    // строим связи к equivalentND
                    if (lastND != null)
                    {
                        lastND.Rebase(equivalentND.Node);
                        if (uniDirection)
                        {
                            using (graph.WriteLock())
                                graph.AddArc(lastND.Node, equivalentND.Node);
#if DEBUG && LOG
                            ETLogger.WriteLine(LogType.Debug,
                                $"LinkLinear: UniLink {{last  -> equivalent}} <{lastND.Node.X:N3};{lastND.Node.Y:N3};{lastND.Node.Z:N3}>");
#endif
                        }
                        else
                        {
                            using (graph.WriteLock())
                                graph.Add2Arcs(lastND.Node, equivalentND.Node);
#if DEBUG && LOG
                            ETLogger.WriteLine(LogType.Debug,
                                $"LinkLinear: BiLink {{last < -> equivalent}} <{lastND.Node.X:N3};{lastND.Node.Y:N3};{lastND.Node.Z:N3}>");
#endif
                        }
                    }

#if false
                    if (nearestND != null)
                    {
                        nearestND.Rebase(equivalentND.Node);
                        if (uniDirection)
                        {
                            using (graph.WriteLock())
                                graph.AddArc(nearestND.Node, equivalentND.Node);
#if DEBUG && LOG
                            ETLogger.WriteLine(LogType.Debug,
                                $"LinkLinear: UniLink {{nearest -> equivalent}} <{nearestND.Node.X:N3};{nearestND.Node.Y:N3};{nearestND.Node.Z:N3}>");
#endif
                        }
                        else
                        {
                            using (graph.WriteLock())
                                graph.Add2Arcs(nearestND.Node, equivalentND.Node);
#if DEBUG && LOG
                            ETLogger.WriteLine(LogType.Debug,
                                $"LinkLinear: BiLink {{nearest -> equivalent}} <{nearestND.Node.X:N3};{nearestND.Node.Y:N3};{nearestND.Node.Z:N3}>");
#endif
                        }
                    } 
#endif

                    return equivalentND;
                }

                // Вставляем в Граф узел newNode
                // связываем его с предыдущим узлом lastNode и с ближайшим узлом
                bool addResult;
                using (graph.WriteLock())
                    addResult = graph.AddNode(newNode);
                if (addResult)
                {
                    // newNode успешно добавлен в граф
#if DEBUG && LOG
                    ETLogger.WriteLine(LogType.Debug,
                        $"LinkLinear: Add 'NEW' node <{newNode.X:N3};{newNode.Y:N3};{newNode.Z:N3}>");
#endif
                    if (lastND != null)
                    {
                        if (uniDirection)
                        {
                            using (graph.WriteLock())
                                graph.AddArc(lastND.Node, newNode, lastND.Distance);
#if DEBUG && LOG
                            ETLogger.WriteLine(LogType.Debug,
                                $"LinkLinear: Link {{last -> new}} <{lastND.Node.X:N3};{lastND.Node.Y:N3};{lastND.Node.Z:N3}>");
#endif
                        }
                        else
                        {
                            using (graph.WriteLock())
                                graph.Add2Arcs(lastND.Node, newNode, lastND.Distance);
#if DEBUG && LOG
                            ETLogger.WriteLine(LogType.Debug,
                                $"LinkLinear: Link {{last <-> new}} <{lastND.Node.X:N3};{lastND.Node.Y:N3};{lastND.Node.Z:N3}>");
#endif
                        }
                    }

                    // Добавляем связь от узла newNode 
                    // к узлам front, left, right
                    if (frontND != null)
                    {
                        frontND.Rebase(newNode);
                        if (uniDirection)
                        {
                            using (graph.WriteLock())
                                graph.AddArc(newNode, frontND.Node);
#if DEBUG && LOG
                            ETLogger.WriteLine(LogType.Debug,
                                $"LinkLinear: UniLink {{new -> front}} <{frontND.Node.X:N3};{frontND.Node.Y:N3};{frontND.Node.Z:N3}>");
#endif
                        }
                        else
                        {
                            using (graph.WriteLock())
                                graph.Add2Arcs(newNode, frontND.Node);
#if DEBUG && LOG
                            ETLogger.WriteLine(LogType.Debug,
                                $"LinkLinear: BiLink {{new <-> front}} <{frontND.Node.X:N3};{frontND.Node.Y:N3};{frontND.Node.Z:N3}>");
#endif
                        }
                    }

#if false
                    if (nearestND != null)
                    {
                        if (uniDirection)
                        {
                            using (graph.WriteLock())
                                graph.AddArc(nearestND.Node, newNode, nearestND.Distance);
#if DEBUG && LOG
                            ETLogger.WriteLine(LogType.Debug,
                                $"LinkLinear: Link {{nearest -> new}} <{nearestND.Node.X:N3};{nearestND.Node.Y:N3};{nearestND.Node.Z:N3}>");
#endif
                        }
                        else
                        {
                            using (graph.WriteLock())
                                graph.Add2Arcs(nearestND.Node, newNode, nearestND.Distance);
#if DEBUG && LOG
                            ETLogger.WriteLine(LogType.Debug,
                                $"LinkLinear: Link {{nearestNod <->ewNode}} <{nearestND.Node.X:N3};{nearestND.Node.Y:N3};{nearestND.Node.Z:N3}>");
#endif
                        }

                    } 
#endif
#if DEBUG && LOG
                    ETLogger.WriteLine(LogType.Debug,
                        $"LinkLinear: Link newNode <{newNode.X:N3};{newNode.Y:N3};{newNode.Z:N3}>");
#endif
                    return new NodeDetail(newNode);
                }
#if DEBUG && LOG
                ETLogger.WriteLine(LogType.Debug,
                    $"LinkLinear: Fail to add Node <{newNode.X:N3};{newNode.Y:N3};{newNode.Z:N3}>");
#endif
                return lastND;
            }
            catch (ThreadAbortException ex)
            {
#if DEBUG && LOG
                ETLogger.WriteLine(LogType.Debug, ex.ToString());
#endif
                throw;
            }
            catch (ThreadInterruptedException ex)
            {
#if DEBUG && LOG
                ETLogger.WriteLine(LogType.Debug, ex.ToString());
#endif
                throw;
            }
#if DEBUG
            catch (Exception ex)
            {
                ETLogger.WriteLine(LogType.Debug, ex.ToString());
                return null;
            }
#else
            catch { return null; }
#endif
#if PROFILING && DEBUG
            finally
            {
                stopwatch.Stop();
                WatchCounter++;
                totalWatchTiks += stopwatch.ElapsedTicks;
                totalWatchMs += stopwatch.ElapsedMilliseconds;
#if DEBUG && LOG
                ETLogger.WriteLine(LogType.Debug, $"LinkNearest1: ElapsedTime is {stopwatch.ElapsedMilliseconds} ms ({stopwatch.ElapsedTicks})");
#endif
            }
#endif

            //return null;
        }

        /// <summary>
        /// Вычисление косинуса угла между проекциями веторов <paramref name="A"/> и <paramref name="B"/> на плоскость Oxy
        /// </summary>
        internal static double CosineOxy(NodeDetail A, NodeDetail B)
        {
            // вычисляем Cos угла между векторами из формулы скалярного произведения векторов
            // a * b = |a| * |b| * cos (alpha) = xa * xb + ya * yb

            double cos = (A.X * B.X + A.Y * B.Y)
                          / Math.Sqrt((A.X * A.X + A.Y * A.Y)
                                      * (B.X * B.X + B.Y * B.Y));

            return cos;
        }

        /// <summary>
        /// Вычисление косинуса угла между проекциями веторов <paramref name="A"/> и <paramref name="B"/> на плоскость Oxy
        /// </summary>
        internal static double CosineOxy(Vector3 Origin, Vector3 A, Vector3 B)
        {
            // вычисляем Cos угла между векторами из формулы скалярного произведения векторов
            // a * b = |a| * |b| * cos (alpha) = xa * xb + ya * yb

            double cos = (A.X * B.X + A.Y * B.Y)
                         / Math.Sqrt((A.X * A.X + A.Y * A.Y)
                                     * (B.X * B.X + B.Y * B.Y));

            return cos;
        }

        internal static double Distance(Node node1, Node node2)
        {
            var p1 = node1.Position;
            var p2 = node2.Position;
            double dx = p1.X - p2.X,
                   dy = p1.Y - p2.Y,
                   dz = p1.Z - p2.Z;


            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        private static bool NodesEquals(Node node1, Node node2)
        {
            return node1.Position.Equals(node2.Position);
        }

        private static bool NodesEquals(NodeDetail nodeDet1, NodeDetail nodeDet2)
        {
            return nodeDet1.Node.Position.Equals(nodeDet2.Node.Position);
        }

    #region Методы фильтрации (проверка связей и расстояний) или без таковых и добавления узлов в список ближайших
        /// <summary>
        /// Добавление узла в список без проверки
        /// </summary>
        private static int SimpleAddNode(List<NodeDetail> listNodes, NodeDetail nodeDet)
        {
            listNodes.Add(nodeDet);
            return listNodes.Count - 1;
        }

        /// <summary>
        /// Добавление узла в список с проверкой наличия связей
        /// </summary>
        private static int SmartAddNode1(List<NodeDetail> listNodes, NodeDetail nodeDet)
        {
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
        private static int SmartAddNode2(List<NodeDetail> listNodes, NodeDetail nodeDet)
        {
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
#endif
}
