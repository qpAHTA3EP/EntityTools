#define nodesLocker

using AStar;
using AStar.Tools;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Threading;
using Timeout = Astral.Classes.Timeout;

namespace EntityTools.Patches.Mapper
{
#if PATCH_ASTRAL
    public class MapperGraphCache :IGraph
    {
        public MapperGraphCache()
        {
            getGraph = () => ACTP0Tools.AstralAccessors.Quester.Core.Meshes;
            _active = EntityTools.Config.Mapper.CacheActive;
        }
        public MapperGraphCache(Func<IGraph> getGraph, bool activate = true, bool hold = true)
        {
            this.getGraph = getGraph;

            _holdPlayer = hold;
            _active = activate;
            if (_active)
                RegenerateCache();
        }

        #region Synchronization
        public object SyncRoot => getGraph().SyncRoot;

        private readonly ReaderWriterLockSlim @lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        /// <summary>
        /// Объект синхронизации для "чтения", допускающий одновременное чтение
        /// </summary>
        public RWLocker.ReadLockToken ReadLock() => new RWLocker.ReadLockToken(@lock);
        /// <summary>
        /// Объект синхронизации для "чтения", допускающий ужесточение блокировки до 
        /// </summary>
        public RWLocker.UpgradableReadToken UpgradableReadLock() => new RWLocker.UpgradableReadToken(@lock);
        /// <summary>
        /// Объект синхронизации для "записи".
        /// </summary>
        public RWLocker.WriteLockToken WriteLock() => new RWLocker.WriteLockToken(@lock);

        public bool IsReadLockHeld => @lock.IsReadLockHeld;
        public bool IsUpgradeableReadLockHeld => @lock.IsUpgradeableReadLockHeld;
        public bool IsWriteLockHeld => @lock.IsWriteLockHeld;
        #endregion

        /// <summary>
        /// Полный граф
        /// </summary>
        private readonly Func<IGraph> getGraph;

        public int Version { get; private set; }

        public override int GetHashCode()
        {
            return getGraph().GetHashCode();
        }

        #region IGraph
        /// <summary>
        /// Список кэшированных узлов
        /// </summary>
        public IEnumerable<Node> NodesCollection
        {
            get
            {
                if (_active)
                {
                    if (CacheRegenerationNeeded)
                        RegenerateCache();
                    using (_nodesLocker.ReadLock())
                        foreach (Node node in _nodes)
                            yield return node; 
                }
                else
                {
                    var graph = getGraph();
                    using (graph.ReadLock())
                        foreach (Node node in graph.NodesCollection)
                            yield return node; 
                }
            }
        }

        public int NodesCount
        {
            get
            {
                if (CacheRegenerationNeeded)
                    RegenerateCache();

                return _nodes.Count;
            }
        }

        /// <summary>
        /// Список кэшированных связей
        /// </summary>
        public IEnumerable<Arc> ArcsCollection
        {
            get
            {
                if (_active)
                {
                    if (CacheRegenerationNeeded)
                        RegenerateCache();
                    using (_nodesLocker.ReadLock()) 
                    {
                        foreach (Node node in _nodes)
                            foreach (Arc arc in node.OutgoingArcs)
                                yield return arc;
                    }
                }
                else
                {
                    var graph = getGraph();
                    using (graph.ReadLock())
                    {   foreach (Arc arc in graph.ArcsCollection)
                            yield return arc;
                    }
                }
            }
        }

        /// <summary>
        /// Применение <paramref name="action"/> к каждой кэшированной вершине
        /// </summary>
        public int ForEachNode(Action<Node> action, bool ignorePassableProperty = false)
        {
            int num = 0;
            var graph = getGraph();
            if (_active)
            {
                if (CacheRegenerationNeeded)
                {
                    using (_nodesLocker.WriteLock())
                    {
                        _nodes.Clear();
                        using (graph.ReadLock())
                        {
                            if (ignorePassableProperty)
                            {
                                foreach (Node node in graph.NodesCollection)
                                    if (NeedCache(node))
                                    {
                                        _nodes.AddLast(node);
                                        action(node);
                                        num++;
                                    }
                            }
                            else 
                            {
                                foreach (Node node in graph.NodesCollection)
                                    if (InCacheArea(node.Position))
                                    {
                                        _nodes.AddLast(node);
                                        if (node.Passable)
                                        {
                                            action(node);
                                            num++;
                                        }
                                    }
                            }
                        } 
                    }
                    _cachedMapAndRegion = EntityManager.LocalPlayer.MapAndRegion;
                    cacheTimeout.ChangeTime(EntityTools.Config.Mapper.CacheRegenTimeout);
                    Version++;
                }
                else
                {
                    using (_nodesLocker.ReadLock())
                    {
                        if (ignorePassableProperty)
                        {
                            foreach (Node node in _nodes)
                            {
                                action(node);
                                num++;
                            }
                        }
                        else foreach (Node node in _nodes)
                        {
                            if (node.Passable)
                            {
                                action(node);
                                num++;
                            }
                        }
                    }
                }
            }
            else
            {
                using(graph.ReadLock())
                    graph.ForEachNode(action, ignorePassableProperty);
            }
            return num;
        }

        /// <summary>
        /// Очистка кэша
        /// </summary>
        public void Clear()
        {
            using (_nodesLocker.WriteLock()) 
                _nodes.Clear();
        }

        /// <summary>
        /// Добавление узла
        /// </summary>
        /// <param name="newNode"></param>
        /// <returns></returns>
        public bool AddNode(Node newNode)
        {
            bool result;
            var graph = getGraph();
            if (_active)
            {
                using (graph.WriteLock())
                    result = graph.AddNode(newNode);
                if (result)
                {
                    using (_nodesLocker.WriteLock()) 
                    _nodes.AddLast(newNode);
#if LastAddedNode
                    lastAddedNode = newNode; 
#endif
                }
            }
            else
            {
                using (graph.WriteLock())
                    result = graph.AddNode(newNode);
#if LastAddedNode
                if (result)
                {
                    lastAddedNode = newNode; 
                }
#endif
            }
            return result;
        }

        /// <summary>
        /// Добавление узла
        /// </summary>
        public Node AddNode(float x, float y, float z)
        {
            var graph = getGraph();
            if (_active)
            {
                Node node = new Node(x, y, z);
                if (!AddNode(node))
                    return null;

#if LastAddedNode
                lastAddedNode = node; 
#endif
                return node;
            }
            using(graph.WriteLock())
                return graph.AddNode(x, y, z);
        }

        /// <summary>
        /// Добавление связи
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="endNode"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public Arc AddArc(Node startNode, Node endNode, double weight = 1)
        {
            var graph = getGraph();
            using (graph.WriteLock())
                return graph.AddArc(startNode, endNode, (float)weight);
        }

        /// <summary>
        /// Добавление связи
        /// </summary>
        public void Add2Arcs(Node node1, Node node2, double weight = 1)
        {
            var graph = getGraph();
            using (graph.WriteLock())
                graph.Add2Arcs(node1, node2, (float)weight);
        }

        /// <summary>
        /// Удаление узла
        /// </summary>
        public bool RemoveNode(Node nodeToRemove)
        {
            if (nodeToRemove == null)
                return false;
            bool removed = false;
            try
            {
                var graph = getGraph();
                using (graph.WriteLock())
                    removed = graph.RemoveNode(nodeToRemove);
                if(removed)
                {
                    using (_nodesLocker.WriteLock()) 
                        removed = _nodes.Remove(nodeToRemove);
                }
            }
            catch
            {
                return false;
            }
            return removed;
        }

        /// <summary>
        /// Удаление ребра
        /// </summary>
        public bool RemoveArc(Arc arcToRemove)
        {
            if (arcToRemove is null)
                return false;
            try
            {
                var graph = getGraph();
                using (graph.WriteLock())
                    return graph.RemoveArc(arcToRemove);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Поиск ближайшего узла
        /// </summary>
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
            var graph = getGraph();
            using(graph.ReadLock())
                return graph.ClosestNode(x, y, z, out distance, ignorePassableProperty);
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
            if (_active)
                return ClosestNode(pos.X, pos.Y, pos.Z, out distance, ignorePassableProperty);
            var graph = getGraph();
            using (graph.ReadLock())
                return graph.ClosestNode(pos.X, pos.Y, pos.Z, out distance, ignorePassableProperty);
        }

        /// <summary>
        /// Поиск ближайшего узла
        /// </summary>
        public Node ClosestNode(Vector3 pos, out double distance, bool ignorePassableProperty = false)
        {
            return ClosestNode(pos.X, pos.Y, pos.Z, out distance, ignorePassableProperty);
        }

        public Arc AddArc(Node startNode, Node endNode, float weight)
        {
            var graph = getGraph();
            using (graph.WriteLock())
                return graph.AddArc(startNode, endNode, weight);
        }

        public void Add2Arcs(Node node1, Node node2, float weight)
        {
            var graph = getGraph();
            using (graph.WriteLock())
                graph.Add2Arcs(node1, node2, weight);
        }

        /// <summary>
        /// Кэшированные узлы
        /// </summary>
        private readonly LinkedList<Node> _nodes = new LinkedList<Node>();
        //private List<Node> nodes = new List<Node>(300);

        private readonly RWLocker _nodesLocker = new RWLocker(); 
        #endregion

        /// <summary>
        /// Флаг активности кэша
        /// При неактивном кэше чтение-запись производится напрямую
        /// </summary>
        public bool Active
        {
            get => _active;// || EntityTools.Config.Mapper.CacheActive;
            set => _active = value;
        }
        bool _active;

        /// <summary>
        /// Флаг удержания персонажа в центре области кэширования
        /// </summary>
        public bool HoldPlayer
        {
            get => _holdPlayer;
            set => _holdPlayer = value;
        }
        bool _holdPlayer;

        /// <summary>
        /// Флаг проверки необходимости обновления кэша
        /// </summary>
        private bool CacheRegenerationNeeded
        {
            get
            {
                var player = EntityManager.LocalPlayer;
                if (_holdPlayer)
                {
                    var location = player.Location;
                    return player.IsValid && !player.IsLoading
                            && !(_nodesLocker.IsReadLockHeld || _nodesLocker.IsUpgradeableReadLockHeld || _nodesLocker.IsWriteLockHeld)
                            && (cacheTimeout.IsTimedOut
                                || Math.Abs(cacheX_0_75 - double.MaxValue) > 1 && Math.Abs(location.X - centerX) > cacheX_0_75
                                || Math.Abs(cacheY_0_75 - double.MaxValue) > 1 && Math.Abs(location.Y - centerY) > cacheY_0_75
                                || Math.Abs(cacheZ_0_75 - double.MaxValue) > 1 && Math.Abs(location.Z - centerZ) > cacheZ_0_75
                                || player.MapAndRegion != _cachedMapAndRegion);
                }

                return player.IsValid && !player.IsLoading && (cacheTimeout.IsTimedOut || player.MapAndRegion != _cachedMapAndRegion)
                    && !(_nodesLocker.IsReadLockHeld || _nodesLocker.IsUpgradeableReadLockHeld || _nodesLocker.IsWriteLockHeld);
            }
        }

        /// <summary>
        /// Имя кэшированной карты и региона
        /// </summary>
        private string _cachedMapAndRegion = string.Empty;

        /// <summary>
        /// Обновление кэша
        /// </summary>
        private void RegenerateCache()
        {
            if (_holdPlayer)
            {
                var location = EntityManager.LocalPlayer.Location;
                internal_SetCacheInitialPosition(location.X, location.Y, location.Z);
            }

            ScanNodes();
        }

        /// <summary>
        /// обновление Кэша вокруг заданной точки <paramref name="newIniPos"/>
        /// </summary>
        private void RegenerateCache(Vector3 newIniPos)
        {
            RegenerateCache(newIniPos.X, newIniPos.Y, newIniPos.Z);
        }

        /// <summary>
        /// обновление Кэша вокруг точки c координатами <paramref name="x"/>, <paramref name="y"/>, <paramref name="z"/>
        /// </summary>
        private void RegenerateCache(double x, double y, double z)
        {
            _holdPlayer = false;
            
            internal_SetCacheInitialPosition(x, y, z);
            ScanNodes();
        }

        /// <summary>
        /// Сканирование графа и заполнение кэша
        /// </summary>
        /// <returns></returns>
        private void ScanNodes()
        {
            using (_nodesLocker.WriteLock()) 
            {
                _nodes.Clear();
                var graph = getGraph();//FullGraph;

                if (graph != null)
                {
                    using (graph.ReadLock())
                    {
                        foreach (Node node in graph.NodesCollection)
                            if (NeedCache(node))
                                _nodes.AddLast(node);
                    } 
                }
            }
            _cachedMapAndRegion = EntityManager.LocalPlayer.MapAndRegion;
            cacheTimeout.ChangeTime(EntityTools.Config.Mapper.CacheRegenTimeout);
            Version++;
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
        public bool InCacheArea(double x, double y)
        {
            return minX <= x && x <= maxX
                && minY <= y && y <= maxY;
        }
        /// <summary>
        /// Размер кэшируемой области вдоль оси X
        /// </summary>
        public double CacheDistanceX
        {
            get => cacheX;
            set
            {
                if (value >= 10)
                {
                    cacheX = value;
                    minX = centerX - value;
                    maxX = centerX + value;
                    cacheX_0_75 = value * 0.75;
                }
                else
                {
                    cacheX = double.MaxValue;
                    minX = double.MaxValue;
                    maxX = double.MaxValue;
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
                if (value >= 10)
                {
                    cacheY = value;
                    minY = centerY - value;
                    maxY = centerY + value;
                    cacheY_0_75 = value * 0.75;
                }
                else
                {
                    cacheY = double.MaxValue;
                    minY = double.MaxValue;
                    maxY = double.MaxValue;
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
                if (value >= 10)
                {
                    cacheZ = value;
                    minZ = centerZ - value;
                    maxZ = centerZ + value;
                    cacheZ_0_75 = value * 0.75;
                }
                else
                {
                    cacheZ = double.MaxValue;
                    minZ = double.MaxValue;
                    maxZ = double.MaxValue;
                    cacheZ_0_75 = double.MaxValue;
                }
            }
        }
        private double cacheZ = 30, cacheZ_0_75;


        public void MoveCenterPosition(double dx, double dy, double dz)
        {
            _holdPlayer = false;

            //TODO: добавить проверку переполнения в MoveCenterPosition, в SetCacheArea и в SetCacheInitialPosition
            centerX += dx;
            
            if (!cacheX.Equals(double.MaxValue))
            {
                minX += dx;
                maxX += dx;
            }
            centerY += dy;
            if (!cacheY.Equals(double.MaxValue))
            {

                minY += dy;
                maxY += dy;
            }
            centerZ += dz;
            if (!cacheZ.Equals(double.MaxValue))
            {
                minZ += dz;
                maxZ += dz;
            }
        }

        /// <summary>
        /// Установка области кеширования координатами противолежащих точек параллелограмма с <param name="x1"/>, <param name="y1"/>, <param name="z1"/> и <param name="x2"/>, <param name="y2"/>, <param name="z2"/>
        /// </summary>
        public void SetCacheArea(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            _holdPlayer = false;
            checked
            {
                if (x1 != 0 && x2 != 0)
                {
                    MapperHelper.FixRange(x1, x2, out minX, out maxX);
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
                if (y1 != 0 && y2 != 0)
                {
                    MapperHelper.FixRange(y1, y2, out minY, out maxY);
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
                    MapperHelper.FixRange(z1, z2, out minZ, out maxZ);
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
        }
        public void SetCacheArea(double x1, double y1, double x2, double y2)
        {
            SetCacheArea(x1, y1, minZ, x2, y2, maxZ);
        }

        /// <summary>
        /// Задание нового центра кэширования
        /// </summary>
        /// <param name="pos"></param>
        public void SetCacheInitialPosition(Vector3 pos)
        {
            _holdPlayer = false;
            internal_SetCacheInitialPosition(pos.X, pos.Y, pos.Z);
        }
        /// <summary>
        /// Вспомогательный метод изменения позиции центра области "кэширования"
        /// </summary>
        private void internal_SetCacheInitialPosition(double x, double y, double z)
        {
            checked
            {
                centerX = x;
                centerY = y;
                centerZ = z;

                if (cacheX.Equals(double.MaxValue))
                {
                    minX = double.MinValue;
                    maxX = double.MaxValue;
                }
                else
                {
                    minX = centerX - cacheX;
                    maxX = centerX + cacheX;
                }
                if (cacheY.Equals(double.MaxValue))
                {
                    minY = double.MinValue;
                    maxY = double.MaxValue;
                }
                else
                {
                    minY = centerY - cacheY;
                    maxY = centerY + cacheY;
                }
                if (cacheZ.Equals(double.MaxValue))
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
        }

        private double centerX, centerY, centerZ;

        /// <summary>
        /// Границы кэшируемой области
        /// </summary>
        private double minX, maxX, minY, maxY, minZ, maxZ;

        private Timeout cacheTimeout = new Timeout(0);
    } 
#endif
}
