using System;
using AStar;
using MyNW.Classes;

namespace EntityTools.Quester.Mapper.Tools
{
    internal class NodeDetail
    {
        public NodeDetail(Node n)
        {
            Node = n;
            //Vector = Node.Position;
            var pos = Node.Position;
            X = pos.X;
            Y = pos.Y;
            Z = pos.Z;
            Distance = 0; //Math.Sqrt(X * X + Y * Y + Z * Z);
        }
        public NodeDetail(Node n, Vector3 origin)
        {
            Node = n;
            //Vector = new Point3D(n.X - origin.X, n.Y - origin.Y, n.Z - origin.Z);
            var pos = Node.Position;
            X = pos.X - origin.X;
            Y = pos.Y - origin.Y;
            Z = pos.Z - origin.Z;
            Distance = Math.Sqrt(X * X + Y * Y + Z * Z);
        }
        public NodeDetail(Node n, Node origin)
        {
            Node = n;
            //Vector = new Point3D(n.X - origin.X, n.Y - origin.Y, n.Z - origin.Z);
            var pos = Node.Position;
            var origPos = origin.Position;
            X = pos.X - origPos.X;
            Y = pos.Y - origPos.Y;
            Z = pos.Z - origPos.Z;
            Distance = Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public void Rebase(Node origin)
        {
            //Vector = new Point3D(Node.X - origin.X, Node.Y - origin.Y, Node.Z - origin.Z);
            var pos = Node.Position;
            var origPos = origin.Position;
            X = pos.X - origPos.X;
            Y = pos.Y - origPos.Y;
            Z = pos.Z - origPos.Z;
            Distance = Math.Sqrt(X * X + Y * Y + Z * Z);
        }
        public void Rebase(Vector3 origin)
        {
            //Vector = new Point3D(Node.X - origin.X, Node.Y - origin.Y, Node.Z - origin.Z);
            var pos = Node.Position;
            X = pos.X - origin.X;
            Y = pos.Y - origin.Y;
            Z = pos.Z - origin.Z;
            Distance = Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public Node Node { get; }
        //public Point3D Vector { get; private set; }
        /// <summary>
        /// X, Y, Z задают координаты вершины Node относительно исследуемой вершины с координатами origin
        /// </summary>
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }
        /// <summary>
        /// Евклидово расстояние до исследуемой вершины с координатами origin
        /// </summary>
        public double Distance { get; private set; }
    }
}
