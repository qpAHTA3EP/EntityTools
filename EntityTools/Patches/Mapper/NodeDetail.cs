using AStar;
using MyNW.Classes;
using System;

namespace EntityTools.Patches.Mapper
{
#if PATCH_ASTRAL
    internal class NodeDetail
    {
        public NodeDetail()
        {
            Node = new Node();
            Vector = new Point3D(0, 0, 0);
            Distance = 0;
        }
        public NodeDetail(Node n)
        {
            Node = n;
            Vector = Node.Position;
            Distance = Math.Sqrt(Vector.X * Vector.X + Vector.Y * Vector.Y + Vector.Z * Vector.Z);
        }
        public NodeDetail(Node n, Vector3 origin)
        {
            Node = n;
            Vector = new Point3D(n.X - origin.X, n.Y - origin.Y, n.Z - origin.Z);
            Distance = Math.Sqrt(Vector.X * Vector.X + Vector.Y * Vector.Y + Vector.Z * Vector.Z);
        }
        public NodeDetail(Node n, Node origin)
        {
            Node = n;
            Vector = new Point3D(n.X - origin.X, n.Y - origin.Y, n.Z - origin.Z);
            Distance = Math.Sqrt(Vector.X * Vector.X + Vector.Y * Vector.Y + Vector.Z * Vector.Z);
        }

        public void Rebase(Node origin)
        {
            Vector = new Point3D(Node.X - origin.X, Node.Y - origin.Y, Node.Z - origin.Z);
            Distance = Math.Sqrt(Vector.X * Vector.X + Vector.Y * Vector.Y + Vector.Z * Vector.Z);
        }
        public void Rebase(Vector3 origin)
        {
            Vector = new Point3D(Node.X - origin.X, Node.Y - origin.Y, Node.Z - origin.Z);
            Distance = Math.Sqrt(Vector.X * Vector.X + Vector.Y * Vector.Y + Vector.Z * Vector.Z);
        }

        public Node Node { get; }
        public Point3D Vector { get; private set; }
        public double Distance { get; private set; }
    }
#endif
}
