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
#if DEVELOPER
    public class MapperGraphCache
    {
        public MapperGraphCache() { }
        public MapperGraphCache(Astral.Forms.UserControls.Mapper m)
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
                else return ((AStar.Graph)coreGraph).Nodes;
            }
        }

        /// <summary>
        /// Список кэшированных связей
        /// </summary>
        public IList Arcs
        {
            get
            {
                if (EntityTools.PluginSettings.Mapper.CacheActive)
                {
                    if (NeedRegenCache)
                        RegenCache();
                    return LA;
                }
                else return ((AStar.Graph)coreGraph).Arcs;
            }
        }

        /// <summary>
        /// Очистка кэша
        /// </summary>
        public void Clear()
        {
            LN.Clear();
            LA.Clear();
        }

        /// <summary>
        /// Добавление узла
        /// </summary>
        /// <param name="NewNode"></param>
        /// <returns></returns>
        public bool AddNode(Node NewNode)
        {
            if (EntityTools.PluginSettings.Mapper.CacheActive)
            {
                //if (NeedRegenCache)
                //    RegenCache();
                if (((AStar.Graph)coreGraph).AddNode(NewNode))
                {
                    LN.Add(NewNode);
                    lastAddedNode = NewNode;
                    return true;
                }
                return false;
            }
            else return ((AStar.Graph)coreGraph).AddNode(NewNode) == true;
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
                //if (NeedRegenCache)
                //    RegenCache();
                Node node = new Node((double)x, (double)y, (double)z);
                if (!this.AddNode(node))
                    return null;

                lastAddedNode = node;
                return node;
            }
            else return lastAddedNode = ((AStar.Graph)coreGraph).AddNode(x, y, z);
        }

        /// <summary>
        /// Добавление связи
        /// </summary>
        /// <param name="NewArc"></param>
        /// <returns></returns>
        public bool AddArc(Arc NewArc)
        {
            if (EntityTools.PluginSettings.Mapper.CacheActive)
            {
                //if (NeedRegenCache)
                //    RegenCache();
                if (((AStar.Graph)coreGraph).AddArc(NewArc))
                {
                    LA.Add(NewArc);
                    return true;
                }
                return false;
            }
            else return ((AStar.Graph)coreGraph).AddArc(NewArc) == true;
        }

        /// <summary>
        /// Добавление связи
        /// </summary>
        /// <param name="StartNode"></param>
        /// <param name="EndNode"></param>
        /// <param name="Weight"></param>
        /// <returns></returns>
        public Arc AddArc(Node StartNode, Node EndNode, double Weight)
        {
            if (EntityTools.PluginSettings.Mapper.CacheActive)
            {
                //if (NeedRegenCache)
                //    RegenCache();
                Arc arc = new Arc(StartNode, EndNode) { Weight = Weight };
                if (!AddArc(arc))
                {
                    return null;
                }
                return arc;
            }
            else return ((AStar.Graph)coreGraph).AddArc(StartNode, EndNode, (float)Weight);
        }

        /// <summary>
        /// Добавление связи
        /// </summary>
        /// <param name="Node1"></param>
        /// <param name="Node2"></param>
        /// <param name="Weight"></param>
        public void Add2Arcs(Node Node1, Node Node2, double Weight)
        {
            AddArc(Node1, Node2, Weight);
            AddArc(Node2, Node1, Weight);
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
                if (EntityTools.PluginSettings.Mapper.CacheActive)
                {

                    foreach (Arc inArc in NodeToRemove.IncomingArcs)
                    {
                        inArc.StartNode.OutgoingArcs.Remove(inArc);
                        LA.Remove(inArc);
                    }
                    foreach (Arc outArc in NodeToRemove.OutgoingArcs)
                    {
                        outArc.EndNode.IncomingArcs.Remove(outArc);
                        LA.Remove(outArc);
                    }
                    LN.Remove(NodeToRemove);
                }
                ((AStar.Graph)coreGraph).RemoveNode(NodeToRemove);
            }
            catch
            {
                return false;
            }
            return true;
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
                if (EntityTools.PluginSettings.Mapper.CacheActive)
                    LA.Remove(ArcToRemove);
                ((AStar.Graph)coreGraph).RemoveArc(ArcToRemove);
                ArcToRemove.StartNode.OutgoingArcs.Remove(ArcToRemove);
                ArcToRemove.EndNode.IncomingArcs.Remove(ArcToRemove);
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
            else return ((AStar.Graph)coreGraph).ClosestNode(PtX, PtY, PtZ, out Distance, IgnorePassableProperty);
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
            else return ((AStar.Graph)coreGraph).ClosestNode(pos.X, pos.Y, pos.Z, out Distance, IgnorePassableProperty);
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
            else return ((AStar.Graph)coreGraph).ClosestNode(pos.X, pos.Y, pos.Z, out Distance, IgnorePassableProperty);
        }

        //public Arc ClosestArc(double PtX, double PtY, double PtZ, out double Distance, bool IgnorePassableProperty)
        //{
        //    if (EntityTools.PluginSettings.Mapper.UseCache)
        //    {
        //        Arc result = null;
        //        double num = -1.0;
        //        Point3D point3D = new Point3D(PtX, PtY, PtZ);
        //        foreach (Arc arc in this.LA)
        //        {
        //            if (!IgnorePassableProperty || arc.Passable)
        //            {
        //                Point3D p = Point3D.ProjectOnLine(point3D, arc.StartNode.Position, arc.EndNode.Position);
        //                double num2 = Point3D.DistanceBetween(point3D, p);
        //                if (num == -1.0 || num > num2)
        //                {
        //                    num = num2;
        //                    result = arc;
        //                }
        //            }
        //        }
        //        Distance = num;
        //        return result;
        //    }
        //    else return fullGraph.AddArc(PtX, PtY, );
        //}

        /// <summary>
        /// Кэшированные узлы
        /// </summary>
        private List<Node> LN = new List<Node>();
        /// <summary>
        /// Кэшированные ребра
        /// </summary>
        private List<Arc> LA = new List<Arc>();
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
            LA.Clear();

            if (EntityTools.PluginSettings.Mapper.CacheActive)
            {
                if (newIniPos == null || !newIniPos.IsValid)
                    cacheInitialPos = EntityManager.LocalPlayer.Location.Clone();
                else cacheInitialPos = newIniPos.Clone();

                if (coreGraph == null)
                    return false;
                foreach (Node node in ((AStar.Graph)coreGraph).Nodes)
                {
                    if (NeedCache(node))
                    {
                        LN.Add(node);
                        foreach (Arc arc in node.IncomingArcs)
                            //if (NeedCache(arc.StartNode))
                            LA.Add(arc);
                        foreach (Arc arc in node.OutgoingArcs)
                            //if (NeedCache(arc.EndNode))
                            LA.Add(arc);
                    }
                }

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
        public void StartCache(Astral.Forms.UserControls.Mapper m)
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
            get => coreGraph.Value;
        }
        private static readonly StaticPropertyAccessor<AStar.Graph> coreGraph = typeof(Astral.Quester.Core).GetStaticProperty<AStar.Graph>("Meshes");

    #region Mapper
        private Astral.Forms.UserControls.Mapper mapper = null;

        /// <summary>
        /// Обновление кэша при удалении вершин
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eventMapperDoubleClick(object sender, EventArgs e)
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
                    for (int i = 0; i < LN.Count; i++)
                    {
                        Node node = LN[i];
                        g.drawFillEllipse(new Vector3((float)node.X, (float)node.Y, (float)node.Z), new Size(7, 7), Brushes.Gold);
                    }
                }
            }
            if (lastAddedNode != null)
            {
                //g.drawFillEllipse(new Vector3((float)lastNodeDetail.Node.X, (float)lastNodeDetail.Node.Y, (float)lastNodeDetail.Node.Z), new Size(9, 9), Brushes.Cyan);
                CustomRegionHelper.DrawAnchor(g, new Vector3((float)lastAddedNode.X, (float)lastAddedNode.Y, (float)lastAddedNode.Z), Brushes.Orange);
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
