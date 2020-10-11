using System;
using System.Drawing;
using AStar;
using MyNW.Classes;

namespace EntityTools.Patches.Mapper
{
    public partial class MapperGraphics
    {
        /// <summary>
        /// вспомогательные методы для извлечения координат из различных классов, определяющих координаты точки
        /// </summary>
        public static class PointHelper
        {
            public static void GetXY<TPoint>(TPoint point, out double x, out double y)
            {
                if (point is Vector3 v3)
                {
                    x = v3.X;
                    y = v3.Y;
                }
                else if (point is Vector2 v2)
                {
                    x = v2.X;
                    y = v2.Y;
                }
                else if (point is Point p)
                {
                    x = p.X;
                    y = p.Y;
                }
                else if (point is Point3D p3d)
                {
                    x = p3d.X;
                    y = p3d.Y;
                }
                else if(point is Node nd)
                {
                    var pos = nd.Position;
                    x = pos.X;
                    y = pos.Y;
                }
                else throw new NotImplementedException();
            }
            public static void GetXYZ<TPoint>(TPoint point, out double x, out double y, out double z)
            {
                if (point is Vector3 v3)
                {
                    x = v3.X;
                    y = v3.Y;
                    z = v3.Z;
                }
                else if (point is Point3D p3d)
                {
                    x = p3d.X;
                    y = p3d.Y;
                    z = p3d.Z;
                }
                else if (point is Node nd)
                {
                    var pos = nd.Position;
                    x = pos.X;
                    y = pos.Y;
                    z = pos.Z;
                }
                else throw new NotImplementedException();
            }

            public static double GetX<TPoint>(TPoint point)
            {
                if (point is Vector3 v3) return v3.X;
                if (point is Vector2 v2) return v2.X;
                if (point is Point p) return p.X;
                if (point is Point3D p3d) return p3d.X;
                if (point is Node nd) return nd.X;
                throw new NotImplementedException();
            }
            public static double GetY<TPoint>(TPoint point)
            {
                if (point is Vector3 v3) return v3.Y;
                if (point is Vector2 v2) return v2.Y;
                if (point is Point p) return p.Y;
                if (point is Point3D p3d) return p3d.Y;
                if (point is Node nd) return nd.Y;
                throw new NotImplementedException();
            }
            public static double GetZ<TPoint>(TPoint point)
            {
                if (point is Vector3 v3) return v3.Z;
                if (point is Point3D p3d) return p3d.Z;
                if (point is Node nd) return nd.Z;
                throw new NotImplementedException();
            }
            public static void SetX<TPoint>(TPoint point, double value)
            {
                if (point is Vector3 v3) v3.X = (float)value;
                else if (point is Vector2 v2) v2.X = (float)value;
                else if (point is Point p) p.X = (int)value;
                else if (point is Point3D p3d) p3d.X = value;
                else if (point is Node nd) nd.Position.X = value;
                else throw new NotImplementedException();
            }
            public static void SetY<TPoint>(TPoint point, double value)
            {
                if (point is Vector3 v3) v3.Y = (float)value;
                else if (point is Vector2 v2) v2.Y = (float)value;
                else if (point is Point p) p.Y = (int)value;
                else if (point is Point3D p3d) p3d.Y = value;
                else if (point is Node nd) nd.Position.Y = value;
                else throw new NotImplementedException();
            }
            public static void SetZ<TPoint>(TPoint point, double value)
            {
                if (point is Vector3 v3) v3.Z = (float)value;
                else if (point is Point3D p3d) p3d.Z = value;
                else if (point is Node nd) nd.Position.Z = value;
                else throw new NotImplementedException();
            }

#if false
            public static double GetX(Vector3 point) => point.X;
            public static double GetY(Vector3 point) => point.Y;
            public static double GetZ(Vector3 point) => point.Z;
            public static void SetX(Vector3 point, double value) => point.X = (float)value;
            public static void SetY(Vector3 point, double value) => point.Y = (float)value;
            public static void SetZ(Vector3 point, double value) => point.Z = (float)value;

            public static double GetX(Point3D point) => point.X;
            public static double GetY(Point3D point) => point.Y;
            public static double GetZ(Point3D point) => point.Z;
            public static void SetX(Point3D point, double value) => point.X = value;
            public static void SetY(Point3D point, double value) => point.Y = value;
            public static void SetZ(Point3D point, double value) => point.Z = value;

            public static int GetX(Point point) => point.X;
            public static int GetY(Point point) => point.Y;
            public static void SetX(Point point, int value) => point.X = value;
            public static void SetY(Point point, int value) => point.Y = value;

            public static double GetX(Vector2 point) => point.X;
            public static double GetY(Vector2 point) => point.Y;
            public static void SetX(Vector2 point, double value) => point.X = (float)value;
            public static void SetY(Vector2 point, double value) => point.Y = (float)value; 
#endif
        }
    }
}
