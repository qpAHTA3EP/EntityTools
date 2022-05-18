using AStar;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace EntityTools.Patches.Mapper
{
    public static class MapperHelper
    {
        public static readonly double DefaultAnchorSize = 16;

        public static readonly Size Size_4x4 = new Size(4, 4);
        public static readonly Size Size_5x5 = new Size(5, 5);
        public static readonly Size Size_8x8 = new Size(8, 8);
        public static readonly Size Size_10x10 = new Size(10, 10);
        public static readonly Size Size_12x12 = new Size(12, 12);
        public static readonly Size Size_14x14 = new Size(14, 14);
        public static readonly Size Size_16x16 = new Size(16, 16);

        public static readonly float cos30 = (float)Math.Cos(Math.PI / 6);
        public static readonly float cos60 = (float)Math.Cos(Math.PI / 3);
        public static readonly float tg30 = (float)Math.Tan(Math.PI / 6);
        public static readonly float tg60 = (float)Math.Tan(Math.PI / 3);
        public static readonly float sin30 = 0.5f;

        /// <summary>
        /// Вращение изображения
        /// </summary>
        public static Bitmap RotateImage(Image image, float angle)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }
            float w = image.Width;
            float h = image.Height;
            double num3;
            for (num3 = angle * Math.PI / 180.0; num3 < 0.0; num3 += Math.PI * 2)
            {
            }
            double num4;
            double num5;
            double num6;
            double num7;
            if ((num3 >= 0.0 && num3 < 1.5707963267948966) || (num3 >= 3.1415926535897931 && num3 < 4.71238898038469))
            {
                num4 = Math.Abs(Math.Cos(num3)) * w;
                num5 = Math.Abs(Math.Sin(num3)) * w;
                num6 = Math.Abs(Math.Cos(num3)) * h;
                num7 = Math.Abs(Math.Sin(num3)) * h;
            }
            else
            {
                num4 = Math.Abs(Math.Sin(num3)) * h;
                num5 = Math.Abs(Math.Cos(num3)) * h;
                num6 = Math.Abs(Math.Sin(num3)) * w;
                num7 = Math.Abs(Math.Cos(num3)) * w;
            }
            double a = num4 + num7;
            double a2 = num6 + num5;
            int num8 = (int)Math.Ceiling(a);
            int num9 = (int)Math.Ceiling(a2);
            Bitmap bitmap = new Bitmap(num8, num9);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                Point[] destPoints;
                if (num3 >= 0.0 && num3 < 1.5707963267948966)
                {
                    destPoints = new[]
                    {
                        new Point((int)num7, 0),
                        new Point(num8, (int)num5),
                        new Point(0, (int)num6)
                    };
                }
                else if (num3 >= 1.5707963267948966 && num3 < 3.1415926535897931)
                {
                    destPoints = new[]
                    {
                        new Point(num8, (int)num5),
                        new Point((int)num4, num9),
                        new Point((int)num7, 0)
                    };
                }
                else if (num3 >= 3.1415926535897931 && num3 < 4.71238898038469)
                {
                    destPoints = new[]
                    {
                        new Point((int)num4, num9),
                        new Point(0, (int)num6),
                        new Point(num8, (int)num5)
                    };
                }
                else
                {
                    destPoints = new[]
                    {
                        new Point(0, (int)num6),
                        new Point((int)num7, 0),
                        new Point((int)num4, num9)
                    };
                }
                graphics.DrawImage(image, destPoints);
            }
            return bitmap;
        }

        /// <summary>
        /// Из двух числе <paramref name="num1"/> и <paramref name="num2"/> определяем минимальное <paramref name="min"/> и максимальное <paramref name="max"/>
        /// </summary>
        public static void FixRange(double num1, double num2, out double min, out double max)
        {
            if (num1 > num2)
            {
                min = num2;
                max = num1;
            }
            else
            {
                min = num1;
                max = num2;
            }
        }

        /// <summary>
        /// Вычисление смещения текстовой метки относительно пары точек с координатами <paramref name="x1"/>, <paramref name="y1"/> и <paramref name="x2"/>, <paramref name="y2"/>
        /// </summary>
        public static void GetLabelAlignment(double x1, double y1, double x2, double y2, out Alignment alignment1, out Alignment alignment2)
        {
            double dx = Math.Abs(x1 - x2),
                   dy = Math.Abs(y1 - y2);

            if (dx > 20)
            {
                if (dy > 20)
                {
                    if (x1 > x2)
                    {
                        // Point1 - справа
                        // Point2 - слева
                        if (y1 > y2)
                        {
                            // Point1 - выше
                            // Point2 - ниже
                            alignment1 = Alignment.BottomLeft;
                            alignment2 = Alignment.TopRight;
                        }
                        else
                        {
                            // Point1 - ниже
                            // Point2 - выше
                            alignment1 = Alignment.TopLeft;
                            alignment2 = Alignment.BottomRight;
                        }
                    }
                    else
                    {
                        // Point1 - слева
                        // Point2 - справа
                        if (y1 > y2)
                        {
                            // Point1 - выше
                            // Point2 - ниже
                            alignment1 = Alignment.BottomRight;
                            alignment2 = Alignment.TopLeft;
                        }
                        else
                        {
                            // Point1 - ниже
                            // Point2 - выше
                            alignment1 = Alignment.TopRight;
                            alignment2 = Alignment.BottomLeft;
                        }
                    }
                }
                else
                {
                    // Point1 и Point2 приблизительно на одной горизонтальной линии
                    if (x1 > x2)
                    {
                        // Point1 - справа
                        // Point2 - слева
                        alignment1 = Alignment.MiddleLeft;
                        alignment2 = Alignment.MiddleRight;
                    }
                    else
                    {
                        // Point1 - слева
                        // Point2 - справа
                        alignment1 = Alignment.MiddleRight;
                        alignment2 = Alignment.MiddleLeft;
                    }
                }
            }
            else
            {
                // Point1 и Point2 приблизительно на одной вертикальной линии
                if (y1 > y2)
                {
                    // Point1 - выше
                    // Point2 - ниже
                    alignment1 = Alignment.BottomCenter;
                    alignment2 = Alignment.TopCenter;
                }
                else
                {
                    // Point1 - ниже
                    // Point2 - выше
                    alignment1 = Alignment.TopCenter;
                    alignment2 = Alignment.BottomCenter;
                }
            }
        }

        /// <summary>
        /// Расстояние между точками в плоскости Оху
        /// </summary>
        public static double Distance2D(double x1, double y1, double x2, double y2)
        {
            double dx = x2 - x1;
            double dy = y2 - y1;

            return Math.Sqrt(dx * dx + dy * dy);
        }
        /// <summary>
        /// Квадрат расстояния между точками в плоскости Oxy
        /// </summary>
        public static double SquareDistance2D(double x1, double y1, double x2, double y2)
        {
            double dx = x2 - x1;
            double dy = y2 - y1;

            return dx * dx + dy * dy;
        }
        /// <summary>
        /// Квадрат расстояния между точками в плоскости Oxy
        /// </summary>
        public static double SquareDistance2D(Vector2 p1, Vector2 p2)
        {

            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;

            return dx * dx + dy * dy;
        }
        /// <summary>
        /// Квадрат расстояния между точками в 3D
        /// </summary>
        public static double SquareDistance3D(double x1, double y1, double z1, double x2, double y2, double z2)
        {

            double dx = x2 - x1;
            double dy = z2 - y1;
            double dz = z2 - z1;

            return dx * dx + dy * dy + dz * dz;
        }
        /// <summary>
        /// Квадрат расстояния между точками в 3D
        /// </summary>
        public static double SquareDistance3D(Vector3 p1, Vector3 p2)
        {

            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            double dz = p2.Z - p1.Z;

            return dx * dx + dy * dy + dz * dz;
        }
        /// <summary>
        /// Расстояние между проекциями точек <paramref name="p1"/> и <paramref name="p2"/> на плоскость Оху
        /// </summary>
        public static double Distance2D(Point3D p1, Point3D p2)
        {
            return Distance2D(p1.X, p1.Y, p2.X, p2.Y);
        }
        /// <summary>
        /// Расстояние между проекциями точек <paramref name="p1"/> и <paramref name="p2"/> на плоскость Оху
        /// </summary>
        public static double Distance2D(PointF p1, PointF p2)
        {
            return Distance2D(p1.X, p1.Y, p2.X, p2.Y);
        }
        /// <summary>
        /// Расстояние между проекциями точек <paramref name="p1"/> и <paramref name="p2"/> на плоскость Оху
        /// </summary>
        public static double Distance2D(Vector3 p1, Vector3 p2)
        {
            return Distance2D(p1.X, p1.Y, p2.X, p2.Y);
        }
        /// <summary>
        /// Расстояние между проекциями точек <paramref name="p1"/> и <paramref name="p2"/> на плоскость Оху
        /// </summary>
        public static double Distance2D<PointT1, PointT2>(PointT1 p1, PointT2 p2)
        {
            MapperGraphics.PointHelper.GetXY(p1, out double x1, out double y1);
            MapperGraphics.PointHelper.GetXY(p2, out double x2, out double y2);
            return Distance2D(x1, y1, x2, y2);
        }

        /// <summary>
        /// Поиск вершины графа <paramref name="graph"/>, проекция которой на плоскость Oxy от точки с координатами <paramref name="x"/>, <paramref name="y"/>
        /// находится не дальше <paramref name="distance"/>
        /// </summary>
        public static Node ClosestNodeOxyProjection(this IGraph graph, double x, double y, double distance = -1)
        {
            if (distance <= 0)
                distance = double.MaxValue;
            else if(distance < double.MaxValue)
                distance *= distance;


            Node result = null;
            foreach(Node node in graph.NodesCollection)
            {
                if(node.Passable)
                {
                    double sqDist = SquareDistance2D(x, y, node.X, node.Y);
                    if(distance > sqDist)
                    {
                        result = node;
                        distance = sqDist;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Поиск элемента коллекции <paramref name="enumerable"/>, проекция которого на плоскость Oxy от точки с координатами <paramref name="x"/>, <paramref name="y"/>
        /// находится не дальше <paramref name="distance"/>. Координаты извлекаются функтором <paramref name="selector"/>
        /// Координаты элемента возвращаются через параметры <paramref name="pX"/>, <paramref name="pY"/>, <paramref name="pZ"/>
        /// </summary>
        public static T ClosestNodeOxyProjection<T, TPoint>(this IEnumerable<T> enumerable, double x, double y, Func<T, TPoint> selector, out double pX, out double pY, out double pZ, double distance = -1)
        {
            if (distance <= 0)
                distance = double.MaxValue;
            else if (distance < double.MaxValue)
                distance *= distance;

            foreach (T element in enumerable)
            {
                TPoint point = selector(element);

                if (point != null)
                {
                    MapperGraphics.PointHelper.GetXYZ(point, out pX, out pY, out pZ);
                    double sqDist = SquareDistance2D(x, y, pX, pY);

                    if (distance > sqDist)
                    {
                        distance = sqDist;
                        return element;
                    }
                }
            }
            pX = 0; pY = 0;  pZ = 0;
            return default(T);
        }

        public static void ClosestNodeOxyProjection(this IGraph graph, double x, double y, out Node node, out int hash,
            double distance = -1)
        {
            node = graph.ClosestNodeOxyProjection(x, y, distance);
            hash = graph.GetHashCode();
        }

        /// <summary>
        /// Вычисление размера (стороны квадрата) якоря для прямоугольника со сторонами <paramref name="width"/> и <paramref name="height"/>
        /// </summary>
        public static double AnchorWorldSize(double width, double height)
        {
            //return DefaultAnchorSize;
            return Math.Max(2, Math.Min(Math.Min(width / 2, height / 2), DefaultAnchorSize));
            //return Math.Max(2, Math.Min(width / 3, height / 3));
        }

        /// <summary>
        /// Выбор якоря прямоугольника, заданного верхней левой точкой с координатами <paramref name="leftX"/>, <paramref name="topY"/> 
        /// и правой нижней точкой с координатами <paramref name="rightX"/>, <paramref name="downY"/>.
        /// Якорь квадратной формы определяется попаданием точки с координатами <paramref name="x"/>, <paramref name="y"/> в квадрат со стороной <paramref name="anchorSize"/>
        /// </summary>
        public static bool SelectAnchor(double leftX, double topY, double rightX, double downY, double x, double y, double anchorSize, out RegionTransformMode mode)
        {
            double hulfAnchorEdgeSize = anchorSize / 2;

            // TopLeft
            if (CheckAnchorSelection(leftX, topY, x, y, hulfAnchorEdgeSize))
            {
                mode = RegionTransformMode.TopLeft;
                return true;
            }

            // TopCenter
            double centerX = (rightX + leftX) / 2;
            if (CheckAnchorSelection(centerX, topY, x, y, hulfAnchorEdgeSize))
            {
                mode = RegionTransformMode.TopCenter;
                return true;
            }

            // TopRight
            if (CheckAnchorSelection(rightX, topY, x, y, hulfAnchorEdgeSize))
            {
                mode = RegionTransformMode.TopRight;
                return true;
            }

            // Left
            double centerY = (topY + downY) / 2;
            if (CheckAnchorSelection(leftX, centerY, x, y, hulfAnchorEdgeSize))
            {
                mode = RegionTransformMode.Left;
                return true;
            }

            // Center
            if (CheckAnchorSelection(centerX, centerY, x, y, hulfAnchorEdgeSize))
            {
                mode = RegionTransformMode.Center;
                return true;
            }

            // Right
            if (CheckAnchorSelection(rightX, centerY, x, y, hulfAnchorEdgeSize))
            {
                mode = RegionTransformMode.Right;
                return true;
            }

            // DownLeft
            if (CheckAnchorSelection(leftX, downY, x, y, hulfAnchorEdgeSize))
            {
                mode = RegionTransformMode.DownLeft;
                return true;
            }

            // DownCenter
            if (CheckAnchorSelection(centerX, downY, x, y, hulfAnchorEdgeSize))
            {
                mode = RegionTransformMode.DownCenter;
                return true;
            }

            // DownRight
            if (CheckAnchorSelection(rightX, downY, x, y, hulfAnchorEdgeSize))
            {
                mode = RegionTransformMode.DownRight;
                return true;
            }

            mode = RegionTransformMode.None;
            return false;
        }

        private static bool CheckAnchorSelection(double anchorX, double anchorY, double x, double y, double halfAnchorSize)
        {
            return anchorX - halfAnchorSize <= x && x <= anchorX + halfAnchorSize
                && anchorY - halfAnchorSize <= y && y <= anchorY + halfAnchorSize;
        }

        /// <summary>
        /// Изменение координат прямоугольника, заданного верхней левой точкой с координатами <paramref name="leftX"/>, <paramref name="topY"/> 
        /// и правой нижней точкой с координатами <paramref name="rightX"/>, <paramref name="downY"/>, в соответствии с режимом трансформации <paramref name="mode"/>
        /// </summary>
        public static void TransformRegion(ref double leftX, ref double topY, ref double rightX, ref double downY, double x, double y, RegionTransformMode mode)
        {
            switch (mode)
            {
                case RegionTransformMode.TopLeft:
                    leftX = x;
                    topY = y;
                    break;
                case RegionTransformMode.TopCenter:
                    topY = y;
                    break;
                case RegionTransformMode.TopRight:
                    rightX = x;
                    topY = y;
                    break;
                case RegionTransformMode.Left:
                    leftX = x;
                    break;
                case RegionTransformMode.Center:
                    double dx = (rightX + leftX) / 2 - x,
                           dy = (topY + downY) / 2 - y;
                    leftX -= dx;
                    rightX -= dx;
                    topY -= dy;
                    downY -= dy;
                    return;
                case RegionTransformMode.Right:
                    rightX = x;
                    break;
                case RegionTransformMode.DownLeft:
                    leftX = x;
                    downY = y;
                    break;
                case RegionTransformMode.DownCenter:
                    downY = y;
                    break;
                case RegionTransformMode.DownRight:
                    rightX = x;
                    downY = y;
                    break;
                default: return;
            }
            FixRange(leftX, rightX, out leftX, out rightX);
            FixRange(downY, topY, out downY, out topY);
        }

        /// <summary>
        /// Отрисовка региона, ограниченного прямоугольником с верхней левой точкой с координатами <paramref name="leftX"/>, <paramref name="topY"/> 
        /// и правой нижней точкой с координатами <paramref name="rightX"/>, <paramref name="downY"/>.
        /// Тип региона задается флагом <paramref name="isElliptical"/>
        /// </summary>
        public static void DrawCustomRegion(this MapperGraphics graphics, double leftX, double topY, double rightX, double downY, bool isElliptical, bool drawAnchors = true)
        {
            // Ось Oy в игровых координатах инвертирована по сравнению с координатами экрана windows (MapPicture)
            // Поэтому координата верхнего правого в экранных координатах имеет минимальное значение

            double width = Math.Abs(rightX - leftX),
                   height = Math.Abs(topY - downY),
                   centerX = (leftX + rightX) / 2,
                   centerY = (downY + topY) / 2,
                   anchorSize = AnchorWorldSize(width, height);

            Pen penRect = (isElliptical) ? Pens.DarkOliveGreen : Pens.LimeGreen;
            Pen penCR = Pens.LimeGreen;
            Brush brushRect = (isElliptical) ? Brushes.DarkOliveGreen : Brushes.LimeGreen;
            Brush brushCR = Brushes.LimeGreen;
            Brush mainAnchor = Brushes.DarkTurquoise;
            bool drawEdgeAnchors = drawAnchors && height > anchorSize * 3 && width > anchorSize * 3;
            bool drawCornerAnchors = drawAnchors && height > anchorSize && width > anchorSize;

            // Отрисовака опорного прямоугольника якоря topLeft
            graphics.DrawRectangle(penRect, leftX, topY, rightX, downY);
            if (drawEdgeAnchors)
            {
                graphics.DrawLine(penRect, centerX, topY, centerX, downY);
                graphics.DrawLine(penRect, leftX, centerY, rightX, centerY);

                // Отрисовака якоря top
                graphics.FillSquareCentered(brushCR, centerX, topY, anchorSize, true);
                // Отрисовака якоря left
                graphics.FillSquareCentered(brushCR, leftX, centerY, anchorSize, true);
                // Отрисовака якоря right
                graphics.FillSquareCentered(brushCR, rightX, centerY, anchorSize, true);
                // Отрисовака якоря down
                graphics.FillSquareCentered(brushCR, centerX, downY, anchorSize, true);
            }

            if (drawCornerAnchors)
            {
                // Отрисовка якоря TopLeft
                // Является основным якорем, координаты которого задат координаты CustomRegion'a
                graphics.FillSquareCentered(mainAnchor, leftX, topY, anchorSize, true);
                // Отрисовка якоря TopRight
                graphics.FillSquareCentered(brushCR, rightX, topY, anchorSize, true);
                // Отрисовка якоря Center
                graphics.FillSquareCentered(brushCR, centerX, centerY, anchorSize, true);
                // Отрисовка якоря DownLeft
                graphics.FillSquareCentered(brushCR, leftX, downY, anchorSize, true);
                // Отрисовка якоря DownRight
                graphics.FillSquareCentered(brushCR, rightX, downY, anchorSize, true);
            }

            // Отрисовка Эллипса
            if (isElliptical)
                graphics.DrawEllipse(penCR, leftX, Math.Max(topY, downY), width, height, true);
        }


        /// <summary>
        /// Поиск объекта, проекция которого на Oxy расположена не далее <paramref name="distance"/> 
        /// от точки, заданной координатами <paramref name="worldX"/>, <paramref name="worldY"/>
        /// </summary>
        public  static object SelectObjectByPosition(double worldX, double worldY, out double x, out double y, out double z, double distance = -1, IGraph graph = null)
        {
            if (distance <= 0)
                distance = double.MaxValue;

            if (graph is null)
                graph = AcTp0Tools.AstralAccessors.Quester.Core.Meshes;

            Node node = graph?.ClosestNodeOxyProjection(worldX, worldY, distance);
            if (node != null)
            {
                var pos = node.Position;
                x = pos.X;
                y = pos.Y;
                z = pos.Z;
                return node;
            }

            var entities = EntityManager.GetEntities();
            var ent = entities.ClosestNodeOxyProjection(worldX, worldY, (e) => e.Location, out x, out y, out z, distance);
            if (ent != null)
            {
                return ent;
            }

            var targetableNodes = EntityManager.LocalPlayer.Player.InteractStatus.TargetableNodes;
            var targetNode = targetableNodes.ClosestNodeOxyProjection(worldX, worldY, (nd) => nd.WorldInteractionNode.Location, out x, out y, out z, distance);
            if (targetNode != null)
                return targetNode;

            x = 0; y = 0; z = 0;
            return null;
        }
    }
}
