using AStar;
using Astral;
using Astral.Logic.Classes.Map;
using EntityTools.Reflection;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Threading;
using AStar.Tools;
using Timeout = Astral.Classes.Timeout;

namespace EntityTools.Patches.Mapper
{
#if PATCH_ASTRAL
    public class MapperGraphCache :IGraph
    {
        public object SyncRoot => FullGraph.SyncRoot;

        public MapperGraphCache(Func<IGraph> getGraph, bool activate = false)
        {
            this.getGraph = getGraph;
            _active = activate;
            if (activate)
            {
                StartCache();
                RegenerateCache();
            }
        }

        #region ReaderWriterLocker
        /// <summary>
        /// Объект синхронизации доступа к объекту <see cref="MapperGraphCache"/>
        /// </summary>
        private readonly ReaderWriterLockSlim @lock = new ReaderWriterLockSlim();

        /// <summary>
        /// Объект синхронизации для "чтения", допускающий одновременное чтение
        /// </summary>
        /// <returns></returns>
        public RWLocker.ReadLockToken ReadLock() => new RWLocker.ReadLockToken(@lock);
        /// <summary>
        /// Объект синхронизации для "записи".
        /// </summary>
        /// <returns></returns>
        public RWLocker.WriteLockToken WriteLock() => new RWLocker.WriteLockToken(@lock);
        #endregion

        /// <summary>
        /// Полный граф
        /// </summary>
        public IGraph FullGraph => getGraph();
        private Func<IGraph> getGraph;

        public override int GetHashCode()
        {
            return getGraph().GetHashCode();
        }

        #region Дублирование интерфейса AStar.Graph
        /// <summary>
        /// Список кэшированных узлов
        /// </summary>
        public IEnumerable<Node> NodesCollection
        {
            get
            {
                if (Active)
                {
                    if (CacheRegenerationNeeded)
                        RegenerateCache();
                    foreach (Node node in nodes)
                        yield return node;
                }
                else foreach (Node node in FullGraph.NodesCollection)
                        yield return node;
            }
        }
        public int NodesCount => nodes.Count;

        /// <summary>
        /// Список кэшированных связей
        /// </summary>
        public IEnumerable<Arc> ArcsCollection
        {
            get
            {
                if (Active)
                {
                    if (CacheRegenerationNeeded)
                        RegenerateCache();
                    foreach (Node node in nodes)
                        foreach (Arc arc in node.OutgoingArcs)
                            yield return arc;
                }
                else foreach (Arc arc in FullGraph.ArcsCollection)
                        yield return arc;
            }
        }

        /// <summary>
        /// Применение <paramref name="action"/> к каждой кэшированной вершине
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public int ForEachNode(Action<Node> action, bool ignorePassableProperty = false)
        {
            int num = 0;
            if (Active)
            {
                if (CacheRegenerationNeeded)
                {
                    var graph = FullGraph;
                    lock (graph.SyncRoot)
                    {
                        if (ignorePassableProperty)
                        {
                            foreach (Node node in graph.NodesCollection)
                                if (InCacheArea(node.Position))
                                {
                                    if (node.Passable)
                                        nodes.AddLast(node);
                                    action(node);
                                    num++;
                                }
                        }
                        else foreach (Node node in graph.NodesCollection)
                                if (NeedCache(node))
                                {
                                    nodes.AddLast(node);
                                    action(node);
                                    num++;
                                }
                    }
                    cachedMapAndRegion = EntityManager.LocalPlayer.MapAndRegion;
                    cacheTimeout.ChangeTime(EntityTools.PluginSettings.Mapper.CacheRegenTimeout);
                }
                else foreach (Node node in nodes)
                    {
                        action(node);
                        num++;
                    }
            }
            else
            {
                var graph = FullGraph;
                lock (graph.SyncRoot)
                    graph.ForEachNode(action, ignorePassableProperty);
            }
            return num;
        }

        /// <summary>
        /// Очистка кэша
        /// </summary>
        public void Clear()
        {
            nodes.Clear();
        }

        /// <summary>
        /// Добавление узла
        /// </summary>
        /// <param name="newNode"></param>
        /// <returns></returns>
        public bool AddNode(Node newNode)
        {
            if (Active)
            {
                var graph = FullGraph;
                lock (graph.SyncRoot)
                {
                    if (graph.AddNode(newNode))
                    {
                        nodes.AddLast(newNode);
                        lastAddedNode = newNode;
                        return true;
                    }
                }
            }
            else
            {
                var graph = FullGraph;
                lock (graph.SyncRoot)
                {
                    if (graph.AddNode(newNode))
                    {
                        lastAddedNode = newNode;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Добавление узла
        /// </summary>
        public Node AddNode(float x, float y, float z)
        {
            if (Active)
            {
                Node node = new Node(x, y, z);
                if (!AddNode(node))
                    return null;

                lastAddedNode = node;
                return node;
            }
            var graph = FullGraph;
            lock (graph.SyncRoot)
            {
                return lastAddedNode = graph.AddNode(x, y, z);
            }
        }

        /// <summary>
        /// Добавление связи
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="endNode"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public Arc AddArc(Node startNode, Node endNode, double weight)
        {
            var graph = FullGraph;
            lock (graph.SyncRoot)
            {
                return graph.AddArc(startNode, endNode, (float)weight);
            }
        }

        /// <summary>
        /// Добавление связи
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <param name="Weight"></param>
        public void Add2Arcs(Node node1, Node node2, double weight)
        {
            var graph = FullGraph;
            lock (graph.SyncRoot)
            {
                graph.Add2Arcs(node1, node2, (float)weight);
            }
        }

        /// <summary>
        /// Удаление узла
        /// </summary>
        /// <param name="NodeToRemove"></param>
        /// <returns></returns>
        public bool RemoveNode(Node NodeToRemove)
        {
            if (NodeToRemove == null)
                return false;
            bool removed = false;
            try
            {
                var graph = FullGraph;
                lock (graph.SyncRoot)
                {
                    removed = graph.RemoveNode(NodeToRemove);
                }
                if(removed)
                {
                    nodes.Remove(NodeToRemove);
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return removed;
        }

        /// <summary>
        /// Удаление связи
        /// </summary>
        /// <param name="ArcToRemove"></param>
        /// <returns></returns>
        public bool RemoveArc(Arc ArcToRemove)
        {
            if (ArcToRemove is null)
                return false;
            try
            {
                var graph = FullGraph;
                lock (graph.SyncRoot)
                {
                    return graph.RemoveArc(ArcToRemove);
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Поиск ближайшего узла
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="distance"></param>
        /// <param name="ignorePassableProperty"></param>
        /// <returns></returns>
        public Node ClosestNode(double x, double y, double z, out double distance, bool ignorePassableProperty = false)
        {
            if (Active && InCacheArea(x, y, z))
            {
                Node result = null;
                double minDist = double.MaxValue;
                Point3D p = new Point3D(x, y, z);
                foreach (Node node in NodesCollection)
                {
                    if (!ignorePassableProperty || node.Passable)
                    {
                        double dist = Point3D.DistanceBetween(node.Position, p);
                        if (minDist > dist)
                        {
                            minDist = dist;
                            result = node;
                        }
                    }
                }
                distance = minDist;
                return result;
            }
            var graph = FullGraph;
            lock (graph.SyncRoot)
            {
                return graph.ClosestNode(x, y, z, out distance, ignorePassableProperty);
            }
        }

        /// <summary>
        /// Поиск ближайшего узла
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="distance"></param>
        /// <param name="ignorePassableProperty"></param>
        /// <returns></returns>
        public Node ClosestNode(Point3D pos, out double distance, bool ignorePassableProperty = false)
        {
            if (Active)
                return ClosestNode(pos.X, pos.Y, pos.Z, out distance, ignorePassableProperty);
            var graph = FullGraph;
            lock (graph.SyncRoot)
            {
                return graph.ClosestNode(pos.X, pos.Y, pos.Z, out distance, ignorePassableProperty);
            }
        }

        /// <summary>
        /// Поиск ближайшего узла
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="distance"></param>
        /// <param name="ignorePassableProperty"></param>
        /// <returns></returns>
        public Node ClosestNode(Vector3 pos, out double distance, bool ignorePassableProperty = false)
        {
            return ClosestNode(pos.X, pos.Y, pos.Z, out distance, ignorePassableProperty);
        }

        public Arc AddArc(Node startNode, Node endNode, float weight)
        {
            var graph = FullGraph;
            lock (graph.SyncRoot)
            {
                return graph.AddArc(startNode, endNode, weight);
            }
        }

        public void Add2Arcs(Node Node1, Node Node2, float Weight)
        {
            var graph = FullGraph;
            lock (graph.SyncRoot)
            {
                graph.Add2Arcs(Node1, Node2, Weight);
            }
        }

        public int RemoveArcs(ArrayList ArcsToRemome)
        {
            var graph = FullGraph;
            lock (graph.SyncRoot)
            {
                return graph.RemoveArcs(ArcsToRemome);
            }
        }

        /// <summary>
        /// Кэшированные узлы
        /// </summary>
        private LinkedList<Node> nodes = new LinkedList<Node>();
#if false
        /// <summary>
        /// Кэшированные ребра
        /// </summary>
        private List<Arc> LA = new List<Arc>(); 
#endif
        #endregion

        /// <summary>
        /// Флаг активности кэша
        /// При неактивном кэше чтение-запись производится напрямую
        /// </summary>
        internal bool Active
        {
            get => _active;// || EntityTools.PluginSettings.Mapper.CacheActive;
            set
            {
                if (value)
                    StartCache();
                else StopCache();
                _active = value;
            }
        }
        bool _active;

        /// <summary>
        /// Флаг удержания персонажа в центре области кэширования
        /// </summary>
        internal bool HoldPlayer
        {
            get => _holdPlayer;
            set => _holdPlayer = value;
        }
        bool _holdPlayer;

        /// <summary>
        /// Последняя добавленная вершина (узел)
        /// </summary>
        internal Node LastAddedNode { get => lastAddedNode; set => lastAddedNode = value; }
        private Node lastAddedNode = null;

        /// <summary>
        /// Флаг проверки необходимости обновления кэша
        /// </summary>
        public bool CacheRegenerationNeeded
        {
            get
            {
                var player = EntityManager.LocalPlayer;
                if (_holdPlayer)
                {
                    var location = player.Location;
                    return player.IsValid && !player.IsLoading
                            && (cacheTimeout.IsTimedOut
                                || cacheX_0_75 != double.MaxValue && Math.Abs(location.X - centerX) > cacheX_0_75
                                || cacheY_0_75 != double.MaxValue && Math.Abs(location.Y - centerY) > cacheY_0_75
                                || cacheZ_0_75 != double.MaxValue && Math.Abs(location.Z - centerZ) > cacheZ_0_75
                                || player.MapAndRegion != cachedMapAndRegion );
                }
                else return player.IsValid && !player.IsLoading && (cacheTimeout.IsTimedOut || player.MapAndRegion != cachedMapAndRegion);
            }
        }

        /// <summary>
        /// Имя кэшированной карты и региона
        /// </summary>
        private string cachedMapAndRegion = string.Empty;

        /// <summary>
        /// Обновление кэша
        /// </summary>
        public bool RegenerateCache()
        {
            if (_holdPlayer)
            {
                var location = EntityManager.LocalPlayer.Location;
                return RegenerateCache(location.X, location.Y, location.Z);
            }

            nodes.Clear();
            return ScanNodes();
        }

        /// <summary>
        /// обновление Кэша вокруг заданной точки <paramref name="newIniPos"/>
        /// </summary>
        public bool RegenerateCache(Vector3 newIniPos)
        {
            return RegenerateCache(newIniPos.X, newIniPos.Y, newIniPos.Z);
        }

        /// <summary>
        /// обновление Кэша вокруг точки c координатами <paramref name="x"/>, <paramref name="y"/>, <paramref name="z"/>
        /// </summary>
        public bool RegenerateCache(double x, double y, double z)
        {
            _holdPlayer = false;
            nodes.Clear();

            if (Active)
            {
                SetCacheInitialPosition(x, y, z);
                return ScanNodes();
            }
            return false;
        }

        /// <summary>
        /// Сканирование графа и заполнение кэша
        /// </summary>
        /// <returns></returns>
        private bool ScanNodes()
        {
            var graph = FullGraph;
            lock (graph.SyncRoot)
            {
                foreach (Node node in graph.NodesCollection)
                    if (NeedCache(node))
                        nodes.AddLast(node);
            }
            cachedMapAndRegion = EntityManager.LocalPlayer.MapAndRegion;
            cacheTimeout.ChangeTime(EntityTools.PluginSettings.Mapper.CacheRegenTimeout);
            return true;
        }

        /// <summary>
        /// Остановка кэширования
        /// </summary>
        public void StopCache()
        {
            _active = false;
#if false
            if (mapper != null && !mapper.IsDisposed)
            {
                mapper.CustomDraw -= handler_MapperDrawCache;
                object mapPictureObj = null;
                if (SubscribedMapperMouseDoubleClick
                    && ReflectionHelper.GetFieldValue(mapper, "\u000E", out mapPictureObj, BindingFlags.Instance | BindingFlags.NonPublic)
                    && ReflectionHelper.UnsubscribeEvent(mapPictureObj, "MouseDoubleClick", this, "eventMapperDoubleClick", true, BindingFlags.Instance | BindingFlags.Public, BindingFlags.Instance | BindingFlags.NonPublic))
                    SubscribedMapperMouseDoubleClick = false;

                if (SubscribedMapperDoubleClick
                    && mapPictureObj != null
                    && ReflectionHelper.UnsubscribeEvent(mapPictureObj, "DoubleClick", this, "eventMapperDoubleClick", true, BindingFlags.Instance | BindingFlags.Public, BindingFlags.Instance | BindingFlags.NonPublic))
                    SubscribedMapperDoubleClick = false;
            } 
#endif
            lastAddedNode = null;
        }

        /// <summary>
        /// Старт кэширования 
        /// </summary>
        public void StartCache()
        {
            _active = true;
            cacheTimeout.ChangeTime(0);
#if false
            mapper = m;
            if (mapper != null && !mapper.IsDisposed)
            {
                mapper.CustomDraw += handler_MapperDrawCache;
                object mapPictureObj = null;
                if (ReflectionHelper.GetFieldValue(mapper, "\u000E", out mapPictureObj, BindingFlags.Instance | BindingFlags.NonPublic)
                    && ReflectionHelper.SubscribeEvent(mapPictureObj, "MouseDoubleClick", this, "eventMapperDoubleClick", true, BindingFlags.Instance | BindingFlags.Public, BindingFlags.Instance | BindingFlags.NonPublic))
                    SubscribedMapperMouseDoubleClick = true;

                if (mapPictureObj != null
                    && ReflectionHelper.SubscribeEvent(mapPictureObj, "DoubleClick", this, "eventMapperDoubleClick", true, BindingFlags.Instance | BindingFlags.Public, BindingFlags.Instance | BindingFlags.NonPublic))
                    SubscribedMapperDoubleClick = true;
            } 
#endif

            RegenerateCache();
        }

        /// <summary>
        /// Проверка попадания узла node в кэшируемый объем
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool NeedCache(Node node)
        {
            return node.Passable && InCacheArea(node.Position);
        }

        /// <summary>
        /// Проверка нахождения точки <param name="pos"/> в области кэширования
        /// </summary>
        public bool InCacheArea(Point3D pos)
        {
            return InCacheArea(pos.X, pos.Y, pos.Z);
        }
        public bool InCacheArea(double x, double y, double z)
        {
            return minX <= x && x <= maxX
                && minY <= y && y <= maxY
                && minZ <= z && z <= maxZ;
        }

        #region взаимодействие с Mapper'ом
#if false
        MapperExt mapper = null;

        /// <summary>
        /// Обновление кэша при удалении вершин
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handler_MapperDoubleClick(object sender, EventArgs e)
        {
            // двойной клик означает удаление точек пути
            // следовательно нужно обновить кэш
            RegenerateCache();

#if DEBUG
            ETLogger.WriteLine(LogType.Debug, "MapperFormExt::eventMapperDoubleClick");
#endif
        }

        /// <summary>
        /// Отрисовка на Mapper'e кэшированных вершин
        /// </summary>
        /// <param name="g"></param>
        private void handler_MapperDrawCache(GraphicsNW g)
        {
            if (nodes != null)
            {
                lock (nodes)
                {
                    foreach (var node in nodes)
                        g.drawFillEllipse(new Vector3((float)node.X, (float)node.Y, (float)node.Z), new Size(7, 7), Brushes.Gold);
                }
            }
            if (lastAddedNode != null)
            {
                //g.drawFillEllipse(new Vector3((float)lastNodeDetail.Node.X, (float)lastNodeDetail.Node.Y, (float)lastNodeDetail.Node.Z), new Size(9, 9), Brushes.Cyan);
                float anchorSize = (float)Math.Max(1, MapperHelper_CustomRegion.DefaultAnchorSize / mapper.Zoom);
                MapperHelper_CustomRegion.DrawAnchor(g, new Vector3((float)lastAddedNode.X, (float)lastAddedNode.Y, (float)lastAddedNode.Z), anchorSize, Brushes.Orange);
            }
        }
        private bool SubscribedMapperMouseDoubleClick = false;
        private bool SubscribedMapperDoubleClick = false; 
#endif
        #endregion

        /// <summary>
        /// Размер кэшируемой области вдоль оси X
        /// </summary>
        public double CacheDistanceX
        {
            get => cacheX;
            set
            {
                if (value >= 0)
                {
                    cacheX = value;
                    cacheX_0_75 = value * 0.75;
                }
                else
                {
                    cacheX = double.MaxValue;
                    cacheX_0_75 = double.MaxValue;
                }
            }
        }
        private double cacheX = 100, cacheX_0_75;

        /// <summary>
        /// Размер кэшируемой области вдоль оси Y
        /// </summary>
        public double CacheDistanceY
        {
            get => cacheY;
            set
            {
                if (value >= 0)
                {
                    cacheY = value;
                    cacheY_0_75 = value * 0.75;
                }
                else
                {
                    cacheY = double.MaxValue;
                    cacheY_0_75 = double.MaxValue;
                }
            }
        }
        private double cacheY = 100, cacheY_0_75;

        /// <summary>
        /// Размер кэшируемой области вдоль оси Z
        /// </summary>
        public double CacheDistanceZ
        {
            get => cacheZ;
            set
            {
                if (value >= 0)
                {
                    cacheZ = value;
                    cacheZ_0_75 = value * 0.75;
                }
                else
                {
                    cacheZ = double.MaxValue;
                    cacheZ_0_75 = double.MaxValue;
                }
            }
        }
        private double cacheZ = 30, cacheZ_0_75;
        
        /// <summary>
        /// Центр кэшированной области
        /// </summary>
        public Vector3 CacheCenter
        {
            get => cacheInitialPosition.Clone();
            set
            {
                if(!Equals(cacheInitialPosition, value))
                    SetCacheInitialPosition(value);
            }
        }

        /// <summary>
        /// Установка области кеширования координатами противолежащих точек параллелограмма с <param name="x1"/>, <param name="y1"/>, <param name="z1"/> и <param name="x2"/>, <param name="y2"/>, <param name="z2"/>
        /// </summary>
        public void SetCacheArea(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            if (x1 != 0 && x2 != 0)
            {
                MapperGraphicsHelper.FixRange(x1, x2, out minX, out maxX);
                cacheX = (maxX - minX) / 2;
                cacheX_0_75 = cacheX * 0.75;
                centerX = minX + cacheX;
            }
            else
            {
                minX = double.MinValue;
                maxX = double.MaxValue;
                cacheX = double.MaxValue;
                cacheX_0_75 = double.MaxValue;
                centerX = 0;
            }
            if (z1 != 0 && z2 != 0)
            {
                MapperGraphicsHelper.FixRange(y1, y2, out minY, out maxY);
                cacheY = (maxY - minY) / 2;
                cacheY_0_75 = cacheY * 0.75;
                centerY = minY + cacheY;
            }
            else
            {
                minY = double.MinValue;
                maxY = double.MaxValue;
                cacheY = double.MaxValue;
                cacheY_0_75 = double.MaxValue;
                centerY = 0;
            }
            if (z1 != 0 && z2 != 0)
            {
                MapperGraphicsHelper.FixRange(z1, z2, out minZ, out maxZ);
                cacheZ = (maxZ - minZ) / 2;
                cacheZ_0_75 = cacheZ * 0.75;
                centerZ = minZ + cacheZ;
            }
            else
            {
                minZ = double.MinValue;
                maxZ = double.MaxValue;
                cacheZ = double.MaxValue;
                cacheZ_0_75 = double.MaxValue;
                centerZ = 0;
            }
        }
        public void SetCacheArea(double x1, double y1, double x2, double y2)
        {
            SetCacheArea(x1, y1, 0, x2, y2, 0);
        }

        private void SetCacheInitialPosition(Vector3 pos)
        {
            _holdPlayer = false;

            SetCacheInitialPosition(pos.X, pos.Y, pos.Z);
        }
        private void SetCacheInitialPosition(double x, double y, double z)
        {
            centerX = x;
            centerY = y;
            centerZ = z;

            cacheInitialPosition.X = (float)x;
            cacheInitialPosition.Y = (float)y;
            cacheInitialPosition.Z = (float)z;

            if (cacheX == double.MaxValue)
            {
                minX = double.MinValue;
                maxX = double.MaxValue;
            }
            else
            {
                minX = centerX - cacheX;
                maxX = centerX + cacheX;
            }
            if (cacheY == double.MaxValue)
            {
                minY = double.MinValue;
                maxY = double.MaxValue;
            }
            else
            {
                minY = centerY - cacheY;
                maxY = centerY + cacheY;
            }
            if (cacheZ == double.MaxValue)
            {
                minZ = double.MinValue;
                maxZ = double.MaxValue;
            }
            else
            {
                minZ = centerZ - cacheZ;
                maxZ = centerZ + cacheZ;
            }
        }

        private Vector3 cacheInitialPosition = new Vector3(0, 0, 0);
        private double centerX, centerY, centerZ;

        /// <summary>
        /// Границы кэшируемой области
        /// </summary>
        private double minX, maxX, minY, maxY, minZ, maxZ;

        private Timeout cacheTimeout = new Timeout(0);
    } 
#endif
}
