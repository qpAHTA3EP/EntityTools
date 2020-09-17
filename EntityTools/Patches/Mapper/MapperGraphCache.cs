using AStar;
using Astral;
using Astral.Classes;
using Astral.Logic.Classes.Map;
using EntityTools.Reflection;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;

namespace EntityTools.Patches.Mapper
{
#if PATCH_ASTRAL
    public class MapperGraphCache
    {
        public MapperGraphCache() { }
#if AstralMapper
        public MapperGraphCache(Astral.Forms.UserControls.Mapper m) 
#else
        public MapperGraphCache(MapperExt m) 
#endif
        {
            StartCache(m);
            RegenCache();
        }

    #region Дублирование интерфейса AStar.Graph
        /// <summary>
        /// Список кэшированных узлов
        /// </summary>
        public IList Nodes
        {
            get
            {
                if (EntityTools.PluginSettings.Mapper.CacheActive)
                {
                    if (NeedRegenCache)
                        RegenCache();
                    return LN;
                }
                return ((AStar.Graph)AstralAccessors.Quester.Core.Meshes).Nodes;
            }
        }

        /// <summary>
        /// Список кэшированных связей
        /// </summary>
        public IEnumerable<Arc> Arcs
        {
            get
            {
                if (EntityTools.PluginSettings.Mapper.CacheActive)
                {
                    if (NeedRegenCache)
                        RegenCache();
                    foreach (Node node in LN)
                        foreach (Arc arc in node.OutgoingArcs)
                            yield return arc;
                }
                else
                {
                    var arcs = ((AStar.Graph)AstralAccessors.Quester.Core.Meshes).Arcs;
                    foreach (Arc arc in arcs)
                        yield return arc;
                }
            }
        }

        /// <summary>
        /// Очистка кэша
        /// </summary>
        public void Clear()
        {
            LN.Clear();
        }

        /// <summary>
        /// Добавление узла
        /// </summary>
        /// <param name="newNode"></param>
        /// <returns></returns>
        public bool AddNode(Node newNode)
        {
            if (EntityTools.PluginSettings.Mapper.CacheActive)
            {
                if (((AStar.Graph)AstralAccessors.Quester.Core.Meshes).AddNode(newNode))
                {
                    LN.Add(newNode);
                    lastAddedNode = newNode;
                    return true;
                }
                return false;
            }
            return ((AStar.Graph)AstralAccessors.Quester.Core.Meshes).AddNode(newNode) == true;
        }

        /// <summary>
        /// Добавление узла
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public Node AddNode(float x, float y, float z)
        {
            if (EntityTools.PluginSettings.Mapper.CacheActive)
            {
                Node node = new Node((double)x, (double)y, (double)z);
                if (!this.AddNode(node))
                    return null;

                lastAddedNode = node;
                return node;
            }
            return lastAddedNode = ((AStar.Graph)AstralAccessors.Quester.Core.Meshes).AddNode(x, y, z);
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
            return ((AStar.Graph)AstralAccessors.Quester.Core.Meshes).AddArc(startNode, endNode, (float)weight);
        }

        /// <summary>
        /// Добавление связи
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <param name="Weight"></param>
        public void Add2Arcs(Node node1, Node node2, double weight)
        {
            ((AStar.Graph)AstralAccessors.Quester.Core.Meshes).Add2Arcs(node1, node2, (float)weight);
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
            try
            {
                if (((AStar.Graph)AstralAccessors.Quester.Core.Meshes).RemoveNode(NodeToRemove))
                {
                    LN.Remove(NodeToRemove);
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        /// <summary>
        /// Удаление связи
        /// </summary>
        /// <param name="ArcToRemove"></param>
        /// <returns></returns>
        public bool RemoveArc(Arc ArcToRemove)
        {
            if (ArcToRemove == null)
                return false;
            try
            {
                ((AStar.Graph)AstralAccessors.Quester.Core.Meshes).RemoveArc(ArcToRemove);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Поиск ближайшего узла
        /// </summary>
        /// <param name="PtX"></param>
        /// <param name="PtY"></param>
        /// <param name="PtZ"></param>
        /// <param name="Distance"></param>
        /// <param name="IgnorePassableProperty"></param>
        /// <returns></returns>
        public Node ClosestNode(double PtX, double PtY, double PtZ, out double Distance, bool IgnorePassableProperty = false)
        {
            if (EntityTools.PluginSettings.Mapper.CacheActive)
            {
                Node result = null;
                double num = -1.0;
                Point3D p = new Point3D(PtX, PtY, PtZ);
                foreach (Node node in LN)
                {
                    if (!IgnorePassableProperty || node.Passable)
                    {
                        double num2 = Point3D.DistanceBetween(node.Position, p);
                        if (num == -1.0 || num > num2)
                        {
                            num = num2;
                            result = node;
                        }
                    }
                }
                Distance = num;
                return result;
            }
            else return ((AStar.Graph)AstralAccessors.Quester.Core.Meshes).ClosestNode(PtX, PtY, PtZ, out Distance, IgnorePassableProperty);
        }

        /// <summary>
        /// Поиск ближайшего узла
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="Distance"></param>
        /// <param name="IgnorePassableProperty"></param>
        /// <returns></returns>
        public Node ClosestNode(Node pos, out double Distance, bool IgnorePassableProperty = false)
        {
            if (EntityTools.PluginSettings.Mapper.CacheActive)
            {
                Node result = null;
                double num = -1.0;
                foreach (Node node in LN)
                {
                    if (!IgnorePassableProperty || node.Passable)
                    {
                        double num2 = Point3D.DistanceBetween(node.Position, pos.Position);
                        if (num == -1.0 || num > num2)
                        {
                            num = num2;
                            result = node;
                        }
                    }
                }
                Distance = num;
                return result;
            }
            else return ((AStar.Graph)AstralAccessors.Quester.Core.Meshes).ClosestNode(pos.X, pos.Y, pos.Z, out Distance, IgnorePassableProperty);
        }

        /// <summary>
        /// Поиск ближайшего узла
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="Distance"></param>
        /// <param name="IgnorePassableProperty"></param>
        /// <returns></returns>
        public Node ClosestNode(Vector3 pos, out double Distance, bool IgnorePassableProperty = false)
        {
            if (EntityTools.PluginSettings.Mapper.CacheActive)
            {
                Node result = null;
                double num = -1.0;
                Point3D p = new Point3D(pos.X, pos.Y, pos.Z);
                foreach (Node node in LN)
                {
                    if (!IgnorePassableProperty || node.Passable)
                    {
                        double num2 = Point3D.DistanceBetween(node.Position, p);
                        if (num == -1.0 || num > num2)
                        {
                            num = num2;
                            result = node;
                        }
                    }
                }
                Distance = num;
                return result;
            }
            else return ((AStar.Graph)AstralAccessors.Quester.Core.Meshes).ClosestNode(pos.X, pos.Y, pos.Z, out Distance, IgnorePassableProperty);
        }

        /// <summary>
        /// Кэшированные узлы
        /// </summary>
        private List<Node> LN = new List<Node>();
#if false
        /// <summary>
        /// Кэшированные ребра
        /// </summary>
        private List<Arc> LA = new List<Arc>(); 
#endif
        #endregion

        /// <summary>
        /// Последняя добавленный узел
        /// </summary>
        internal Node LastAddedNode { get => lastAddedNode; set => lastAddedNode = value; }
        private Node lastAddedNode = null;

        /// <summary>
        /// Расстояние от cacheInitialPos в плоскости oXY, в пределах которого кэшируются узлы графа
        /// </summary>
        public double CacheXYDistance
        {
            get => cacheXY;
            set
            {
                if (value > 100)
                    cacheXY = value;
                else cacheXY = 100;
            }
        }
        private double cacheXY = 100;
        /// <summary>
        /// Расстояние от cacheInitialPos в плоскости oXY, в пределах которого кэшируются узлы графа
        /// </summary>
        public double CacheZDistance
        {
            get => cacheZ;
            set
            {
                if (value > 10)
                    cacheZ = value;
                else cacheZ = 10;
            }
        }
        private double cacheZ = 30;

        /// <summary>
        /// Флаг проверки 
        /// </summary>
        public bool NeedRegenCache
        {
            get
            {
                double z;
                return EntityManager.LocalPlayer.IsValid
                        && (cacheTimeout.IsTimedOut
                            || EntityManager.LocalPlayer.MapAndRegion != cachedMapAndRegion
                            || (EntityManager.LocalPlayer.Location.Distance2D(cacheInitialPos) > cacheXY * 0.75d
                            || (z = EntityManager.LocalPlayer.Location.Z - cacheInitialPos.Z) * Math.Sign(z) > cacheZ * 0.75d));
            }
        }

        /// <summary>
        /// Имя кэшированной карты и региона
        /// </summary>
        private string cachedMapAndRegion = string.Empty;

        /// <summary>
        /// обновление Кэша
        /// </summary>
        public bool RegenCache(Vector3 newIniPos = null)
        {
            LN.Clear();

            if (EntityTools.PluginSettings.Mapper.CacheActive)
            {
                if (newIniPos == null || !newIniPos.IsValid)
                    cacheInitialPos = EntityManager.LocalPlayer.Location.Clone();
                else cacheInitialPos = newIniPos.Clone();

                if (!AstralAccessors.Quester.Core.Meshes.IsValid())
                    return false;
                foreach (Node node in ((AStar.Graph)AstralAccessors.Quester.Core.Meshes).Nodes)
                    if (NeedCache(node))
                        LN.Add(node);

                cachedMapAndRegion = EntityManager.LocalPlayer.MapAndRegion;
                cacheTimeout.ChangeTime(EntityTools.PluginSettings.Mapper.CacheRegenTimeout);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Остановка кэширования
        /// </summary>
        public void StopCache()
        {
            if (mapper != null && !mapper.IsDisposed)
            {
                mapper.CustomDraw -= eventMapperDrawCache;
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
            lastAddedNode = null;
        }

        /// <summary>
        /// Старт кэширования 
        /// </summary>
        /// <param name="m"></param>
#if AstralMapper
        public void StartCache(Astral.Forms.UserControls.Mapper m) 
#else
        public void StartCache(MapperExt m)
#endif
        {
            mapper = m;
            cacheTimeout.ChangeTime(0);

            if (mapper != null && !mapper.IsDisposed)
            {
                mapper.CustomDraw += eventMapperDrawCache;
                object mapPictureObj = null;
                if (ReflectionHelper.GetFieldValue(mapper, "\u000E", out mapPictureObj, BindingFlags.Instance | BindingFlags.NonPublic)
                    && ReflectionHelper.SubscribeEvent(mapPictureObj, "MouseDoubleClick", this, "eventMapperDoubleClick", true, BindingFlags.Instance | BindingFlags.Public, BindingFlags.Instance | BindingFlags.NonPublic))
                    SubscribedMapperMouseDoubleClick = true;

                if (mapPictureObj != null
                    && ReflectionHelper.SubscribeEvent(mapPictureObj, "DoubleClick", this, "eventMapperDoubleClick", true, BindingFlags.Instance | BindingFlags.Public, BindingFlags.Instance | BindingFlags.NonPublic))
                    SubscribedMapperDoubleClick = true;
            }

            RegenCache();
        }

        /// <summary>
        /// Проверка попадания узла node в кэшируемый объем
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool NeedCache(Node node)
        {
            if (node.Passable)
            {
                double dx = node.X - cacheInitialPos.X;
                double dy = node.Y - cacheInitialPos.Y;
                double dz = node.Z - cacheInitialPos.Z;

                return dx > -cacheXY && dx < cacheXY
                    && dy > -cacheXY && dy < cacheXY
                    && dz > -cacheZ && dz < cacheZ;
            }
            else return false;
        }

        /// <summary>
        /// Полный граф
        /// </summary>
        public AStar.Graph FullGraph
        {
            get => AstralAccessors.Quester.Core.Meshes.Value;
        }
#if false
        private static readonly StaticPropertyAccessor<AStar.Graph> coreGraph = typeof(Astral.Quester.Core).GetStaticProperty<AStar.Graph>("Meshes"); 
#endif

        #region Mapper
#if AstralMapper
        private Astral.Forms.UserControls.Mapper mapper = null; 
#else
        MapperExt mapper = null;
#endif

        /// <summary>
        /// Обновление кэша при удалении вершин
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handler_MapperDoubleClick(object sender, EventArgs e)
        {
            // двойной клик означает удаление точек пути
            // следовательно нужно обновить кэш
            RegenCache();

#if DEBUG
            ETLogger.WriteLine(LogType.Debug, "MapperFormExt::eventMapperDoubleClick");
#endif
        }

        /// <summary>
        /// Отрисовка на Mapper'e кэшированных вершин
        /// </summary>
        /// <param name="g"></param>
        private void eventMapperDrawCache(GraphicsNW g)
        {
            if (LN != null)
            {
                lock (LN)
                {
                    foreach (var node in LN)
                        g.drawFillEllipse(new Vector3((float)node.X, (float)node.Y, (float)node.Z), new Size(7, 7), Brushes.Gold);
                }
            }
            if (lastAddedNode != null)
            {
                //g.drawFillEllipse(new Vector3((float)lastNodeDetail.Node.X, (float)lastNodeDetail.Node.Y, (float)lastNodeDetail.Node.Z), new Size(9, 9), Brushes.Cyan);
                MapperHelper_CustomRegion.DrawAnchor(g, new Vector3((float)lastAddedNode.X, (float)lastAddedNode.Y, (float)lastAddedNode.Z), MapperHelper_CustomRegion.DefaultAnchorSize, Brushes.Orange);
            }
        }
        private bool SubscribedMapperMouseDoubleClick = false;
        private bool SubscribedMapperDoubleClick = false;
    #endregion

        /// <summary>
        /// Центр кэшируемой области
        /// </summary>
        private Vector3 cacheInitialPos = new Vector3(0, 0, 0);

        private Timeout cacheTimeout = new Timeout(0);
    } 
#endif
}
