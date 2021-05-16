using MyNW.Classes;
using System;
using AStar;

namespace EntityTools.Tools.Navigation
{
    public static class NavigationHelper
    {
        /// <summary>
        /// Вычисление косинуса угла образованного тремя точками с вершиной в точке <param name="angularPoint"/>
        /// cos(15)  =  0,9659258262890682867497431997289
        /// cos(30)  =  0,86602540378443864676372317075294
        /// cos(45)  =  0,70710678118654752440084436210485
        /// cos(60)  =  0,5
        /// cos(75)  =  0,25881904510252076234889883762405
        /// cos(90)  =  0,0
        /// cos(105) = -0,25881904510252076234889883762405
        /// cos(165) = -0,9659258262890682867497431997289
        /// </summary>
        /// <param name="squareDistance0">Квадрат расстояния между точками <param name="angularPoint"/> и <param name="point0"/></param>
        /// <param name="squareDistance1">Квадрат расстояния между точками <param name="angularPoint"/> и <param name="point1"/></param>
        public static double Cosine(Vector3 angularPoint, Vector3 point0, Vector3 point1, out float squareDistance0, out float squareDistance1)
        {
            float x0 = point0.X - angularPoint.X,
                  y0 = point0.Y - angularPoint.Y,
                  z0 = point0.Z - angularPoint.Z,
                  x1 = point1.X - angularPoint.X,
                  y1 = point1.Y - angularPoint.Y,
                  z1 = point1.Z - angularPoint.Z;

            squareDistance0 = x0 * x0 + y0 * y0 + z0 * z0; 
            squareDistance1 = x1 * x1 + y1 * y1 + z1 * z1; 

            return (x0 * x1 + y0 * y1 + z0 * z1) / Math.Sqrt(squareDistance0 * squareDistance1); 
        }

        /// <summary>
        /// Вычисление косинуса угла образованного тремя точками с вершиной в точке <param name="angularPoint"/>
        /// cos(15)  =  0,9659258262890682867497431997289
        /// cos(30)  =  0,86602540378443864676372317075294
        /// cos(45)  =  0,70710678118654752440084436210485
        /// cos(60)  =  0,5
        /// cos(75)  =  0,25881904510252076234889883762405
        /// cos(90)  =  0,0
        /// cos(105) = -0,25881904510252076234889883762405
        /// cos(165) = -0,9659258262890682867497431997289
        /// </summary>
        /// <param name="squareDistance0">Квадрат расстояния между точками <param name="angularPoint"/> и <param name="point0"/></param>
        /// <param name="squareDistance1">Квадрат расстояния между точками <param name="angularPoint"/> и <param name="point1"/></param>
        public static double Cosine(Point3D angularPoint, Point3D point0, Point3D point1, out double squareDistance0, out double squareDistance1)
        {
            double x0 = point0.X - angularPoint.X,
                y0 = point0.Y - angularPoint.Y,
                z0 = point0.Z - angularPoint.Z,
                x1 = point1.X - angularPoint.X,
                y1 = point1.Y - angularPoint.Y,
                z1 = point1.Z - angularPoint.Z;

            squareDistance0 = x0 * x0 + y0 * y0 + z0 * z0;
            squareDistance1 = x1 * x1 + y1 * y1 + z1 * z1;

            return (x0 * x1 + y0 * y1 + z0 * z1) / Math.Sqrt(squareDistance0 * squareDistance1);
        }

        /// <summary>
        /// Вычисление косинуса угла образованного тремя точками с вершиной в точке <param name="angularPointX"/>, <param name="angularPointY"/>, <param name="angularPointZ"/>)
        /// cos(15)  =  0,9659258262890682867497431997289
        /// cos(30)  =  0,86602540378443864676372317075294
        /// cos(45)  =  0,70710678118654752440084436210485
        /// cos(60)  =  0,5
        /// cos(75)  =  0,25881904510252076234889883762405
        /// cos(90)  =  0,0
        /// cos(105) = -0,25881904510252076234889883762405
        /// cos(165) = -0,9659258262890682867497431997289
        /// </summary>
        /// <param name="squareDistance0">Квадрат расстояния между точками с координатами (<param name="angularPointX"/>, <param name="angularPointY"/>, <param name="angularPointZ"/>) и (<param name="x0"/>, <param name="y0"/>, <param name="z0"/>)</param>
        /// <param name="squareDistance1">Квадрат расстояния между точками с координатами (<param name="angularPointX"/>, <param name="angularPointY"/>, <param name="angularPointZ"/>) и (<param name="x1"/>, <param name="y1"/>, <param name="z1"/>)</param>
        public static double Cosine(double angularPointX, double angularPointY, double angularPointZ,
                                    double x0, double y0, double z0,
                                    double x1, double y1, double z1,
                                    out double squareDistance0, out double squareDistance1)
        {
            double dx0 = x0 - angularPointX,
                   dy0 = y0 - angularPointY,
                   dz0 = z0 - angularPointZ,
                   dx1 = x1 - angularPointX,
                   dy1 = y1 - angularPointY,
                   dz1 = z1 - angularPointZ;

            squareDistance0 = dx0 * dx0 + dy0 * dy0 + dz0 * dz0;
            squareDistance1 = dx1 * dx1 + dy1 * dy1 + dz1 * dz1;

            return (dx0 * dx1 + dy0 * dy1 + dz0 * dz1) / Math.Sqrt(squareDistance0 * squareDistance1);
        }
    }
}
