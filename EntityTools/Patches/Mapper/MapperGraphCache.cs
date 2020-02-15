using AStar;
using Astral.Classes;
using Astral.Logic.Classes.Map;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EntityTools.Patches.Mapper
{
    public class MapperGraphCache
    {
        public MapperGraphCache() { }
        public MapperGraphCache(AStar.Graph graph)
        {
            fullGraph = graph;
            RegenCache();
        }

        #region Дублирование интерфейса AStar.Graph

        public List<Node> Nodes
        {
            get
            {
                if (NeedRegenCache)
                    RegenCache();
                return LN;
            }
        }

        public List<Arc> Arcs
        {
            get
            {
                if (NeedRegenCache)
                    RegenCache();
                return LA;
            }
        }

        public void Clear()
        {
            LN.Clear();
            LA.Clear();
        }

        public bool AddNode(Node NewNode)
        {
            if (fullGraph.AddNode(NewNode))
            {
                LN.Add(NewNode);
                return true;
            }
            return false;
        }

        public Node AddNode(float x, float y, float z)
        {
            Node node = new Node((double)x, (double)y, (double)z);
            if (!this.AddNode(node))
            {
                return null;
            }
            return node;
        }

        public bool AddArc(Arc NewArc)
        {
            if (fullGraph.AddArc(NewArc))
            {
                LA.Add(NewArc);
                return true;
            }
            return false;
        }

        public Arc AddArc(Node StartNode, Node EndNode, double Weight)
        {
            Arc arc = new Arc(StartNode, EndNode);
            arc.Weight = Weight;
            if (!AddArc(arc))
            {
                return null;
            }
            return arc;
        }

        public void Add2Arcs(Node Node1, Node Node2, double Weight)
        {
            AddArc(Node1, Node2, Weight);
            AddArc(Node2, Node1, Weight);
        }

        public bool RemoveNode(Node NodeToRemove)
        {
            if (NodeToRemove == null)
            {
                return false;
            }
            try
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
                fullGraph.RemoveNode(NodeToRemove);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool RemoveArc(Arc ArcToRemove)
        {
            if (ArcToRemove == null)
            {
                return false;
            }
            try
            {
                LA.Remove(ArcToRemove);
                fullGraph.RemoveArc(ArcToRemove);
                ArcToRemove.StartNode.OutgoingArcs.Remove(ArcToRemove);
                ArcToRemove.EndNode.IncomingArcs.Remove(ArcToRemove);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public Node ClosestNode(double PtX, double PtY, double PtZ, out double Distance, bool IgnorePassableProperty = false)
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

        public Node ClosestNode(Node pos, out double Distance, bool IgnorePassableProperty = false)
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

        public Node ClosestNode(Vector3 pos, out double Distance, bool IgnorePassableProperty = false)
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

        public Arc ClosestArc(double PtX, double PtY, double PtZ, out double Distance, bool IgnorePassableProperty)
        {
            Arc result = null;
            double num = -1.0;
            Point3D point3D = new Point3D(PtX, PtY, PtZ);
            foreach (Arc arc in this.LA)
            {
                if (!IgnorePassableProperty || arc.Passable)
                {
                    Point3D p = Point3D.ProjectOnLine(point3D, arc.StartNode.Position, arc.EndNode.Position);
                    double num2 = Point3D.DistanceBetween(point3D, p);
                    if (num == -1.0 || num > num2)
                    {
                        num = num2;
                        result = arc;
                    }
                }
            }
            Distance = num;
            return result;
        }

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
                return cacheTimeout.IsTimedOut || 
                    (EntityManager.LocalPlayer.Location.Distance2D(cacheInitialPos) > cacheXY * 0.75d
                     && ((z = EntityManager.LocalPlayer.Location.Z - cacheInitialPos.Z) > cacheZ * 0.75d 
                        || z < -cacheZ * 0.75d));
            }
        }

        /// <summary>
        /// обновление Кэша
        /// </summary>
        public bool RegenCache(Vector3 newIniPos = null, bool force = false)
        {
            if (force || NeedRegenCache)
            {
                LN.Clear();
                LA.Clear();

                if (newIniPos == null || !newIniPos.IsValid)
                    cacheInitialPos = EntityManager.LocalPlayer.Location.Clone();
                else cacheInitialPos = newIniPos.Clone();

                if (fullGraph == null)
                    return false;
                foreach (Node node in fullGraph.Nodes)
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

                cacheTimeout.ChangeTime(RegenCacheTimeout);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Проверка попадания узла node в кэшируемый объем
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool NeedCache(Node node)
        {
            double dx = node.X - cacheInitialPos.X;
            double dy = node.Y - cacheInitialPos.Y;
            double dz = node.Z - cacheInitialPos.Z;

            return dx > -cacheXY && dx < cacheXY
                && dy > -cacheXY && dy < cacheXY
                && dz > -cacheZ && dz < cacheZ;
        }

        /// <summary>
        /// Полный граф
        /// </summary>
        public Graph FullGraph
        {
            get => fullGraph;
            set
            {
                if(value != fullGraph)
                {
                    fullGraph = value;
                    RegenCache();
                }
            }
        }
        private Graph fullGraph;

        /// <summary>
        /// Время обновления кэша
        /// </summary>
        public int RegenCacheTimeout
        {
            get => regenCacheTimeout;
            set
            {
                if (value > 1000)
                    regenCacheTimeout = value;
                else regenCacheTimeout = 1000;
            }
        }
        private int regenCacheTimeout = 5000;

        /// <summary>
        /// Центр кэшируемой области
        /// </summary>
        private Vector3 cacheInitialPos = new Vector3(0,0,0);

        private Timeout cacheTimeout = new Timeout(0);
    }
}
