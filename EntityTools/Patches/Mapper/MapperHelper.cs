﻿using AStar;
using Astral.Logic.Classes.Map;
using EntityTools.Reflection;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using static EntityTools.Reflection.InstanceFieldAccessorFactory;

namespace EntityTools.Patches.Mapper
{
    public static class MapperHelper
    {
        public static readonly double DefaultAnchorSize = 16;

        public static readonly Size Square5 = new Size(5, 5);
        public static readonly Size Square8 = new Size(8, 8);
        public static readonly Size Square10 = new Size(10, 10);
        public static readonly Size Square12 = new Size(12, 12);
        public static readonly Size Square14 = new Size(14, 14);
        public static readonly Size Square16 = new Size(16, 16);

        public static readonly float cos30 = (float)Math.Cos(Math.PI / 6);
        public static readonly float cos60 = (float)Math.Cos(Math.PI / 3);
        public static readonly float tg30 = (float)Math.Tan(Math.PI / 6);
        public static readonly float tg60 = (float)Math.Tan(Math.PI / 3);
        public static readonly float sin30 = 0.5f;

        private static readonly Func<GraphicsNW, Graphics> getGraphicsFrom = GetInstanceFieldAccessor<GraphicsNW, Graphics>("g");

        /// <summary>
        /// Получение доступа к приватному члену <seealso cref="GraphicsNW.g"/> типа Graphics
        /// </summary>
        /// <param name="graphicsNW"></param>
        /// <returns></returns>
        public static Graphics GetGraphics(this GraphicsNW graphicsNW)
        {
            return getGraphicsFrom(graphicsNW);
        }

#if false
        /// <summary>
        /// Отрисовка заполненного равностороннего треугольника с основанием <paramref name="edge"/> внизу
        /// </summary>
        public static void drawFillTriangle(this GraphicsNW graphicsNW, Vector3 location, float edge, Brush brush)
        {
            float x = location.X,
                  y = location.Y,
                  r = edge / cos30,
                  dx = edge / 2,
                  dy = dx * tg30;

            List<Vector3> coords = new List<Vector3>() {
                    new Vector3(x,      y + r,  0),
                    new Vector3(x - dx, y - dy, 0),
                    new Vector3(x + dx, y - dy, 0)
                };
            graphicsNW.drawFillPolygon(coords, brush);
        }
        /// <summary>
        /// Отрисовка перевернутого заполненного равностороннего треугольника с основанием <paramref name="edge"/> вверху
        /// </summary>
        public static void drawFillUpsideTriangle(this GraphicsNW graphicsNW, Vector3 location, float edge, Brush brush)
        {
            float x = location.X,
                  y = location.Y,
                  r = edge / cos30,
                  dx = edge / 2,
                  dy = dx * tg30;

            List<Vector3> coords = new List<Vector3>() {
                    new Vector3(x,      y - r,  0),
                    new Vector3(x - dx, y + dy, 0),
                    new Vector3(x + dx, y + dy, 0)
                };
            graphicsNW.drawFillPolygon(coords, brush);
        }
        /// <summary>
        /// Отрисовка заполненного квадрата со стороной <paramref name="edge"/>
        /// </summary>
        public static void drawFillSquare(this GraphicsNW graphicsNW, Vector3 location, float edge, Brush brush)
        {
            float x = location.X,
                  y = location.Y,
                  halfEdge = edge / 2;

            List<Vector3> coords = new List<Vector3>() {
                    new Vector3(x - halfEdge, y - halfEdge, 0),
                    new Vector3(x - halfEdge, y + halfEdge, 0),
                    new Vector3(x + halfEdge, y + halfEdge, 0),
                    new Vector3(x + halfEdge, y - halfEdge, 0)
                };
            graphicsNW.drawFillPolygon(coords, brush);
        }
        /// <summary>
        /// Отрисовка заполненого ромба с диагоналями <paramref name="diagonalX"/> и <paramref name="diagonalY"/>
        /// </summary>
        /// <param name="graphicsNW"></param>
        /// <param name="location"></param>
        /// <param name="diagonalX"></param>
        /// <param name="diagonalY"></param>
        /// <param name="brush"></param>
        public static void drawFillRhomb(this GraphicsNW graphicsNW, Vector3 location, float diagonalX, float diagonalY, Brush brush)
        {
            float x = location.X,
                  y = location.Y,
                  dx = diagonalX / 2,
                  dy = diagonalY / 2;

            List<Vector3> coords = new List<Vector3>() {
                    new Vector3(x + dx, y,      0),
                    new Vector3(x,      y + dy, 0),
                    new Vector3(x - dx, y,      0),
                    new Vector3(x,      y - dy, 0)
                };
            graphicsNW.drawFillPolygon(coords, brush);
        }

        /// <summary>
        /// Преобразование <seealso cref="AStar.Point3D"/> в <seealso cref="Vector3"/>
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3 AsVector3(this AStar.Point3D point)
        {
            return new Vector3((float)point.X, (float)point.Y, (float)point.Z);
        } 
#endif

        /// <summary>
        /// Вращение изображения
        /// </summary>
        public static Bitmap RotateImage(Image image, float angle)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
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
                    destPoints = new Point[]
                    {
                        new Point((int)num7, 0),
                        new Point(num8, (int)num5),
                        new Point(0, (int)num6)
                    };
                }
                else if (num3 >= 1.5707963267948966 && num3 < 3.1415926535897931)
                {
                    destPoints = new Point[]
                    {
                        new Point(num8, (int)num5),
                        new Point((int)num4, num9),
                        new Point((int)num7, 0)
                    };
                }
                else if (num3 >= 3.1415926535897931 && num3 < 4.71238898038469)
                {
                    destPoints = new Point[]
                    {
                        new Point((int)num4, num9),
                        new Point(0, (int)num6),
                        new Point(num8, (int)num5)
                    };
                }
                else
                {
                    destPoints = new Point[]
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
        /// Из двух числе <paramref name="num1"/> и <paramref name="num2"/> определяе минимальное <paramref name="min"/> и максимальное <paramref name="max"/>
        /// </summary>
        public static void FixRange(double num1, double num2, out double min, out double max)
        {
            if(num1 > num2)
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
            if (distance < 0)
                distance = double.MaxValue;
            else distance *= distance;

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

        public static void ClosestNodeOxyProjection(this IGraph graph, double x, double y, out Node node, out int hash,
            double distance = -1)
        {
            lock (graph.SyncRoot)
            {
                node = graph.ClosestNodeOxyProjection(x, y, distance);
                hash = graph.GetHashCode();
            }
        }

#if false
        /// <summary>
        /// Проверка наличия элемента <paramref name="element"/> в коллекции <paramref name="collection"/>
        /// </summary>
        public static bool Contains<T>(this IEnumerable<T> collection, T element)
        {
            foreach (T elem in collection)
                if (Equals(elem, element))
                    return true;
            return false;
        } 
#endif

        /// <summary>
        /// Применение действия <paramref name="action"/> для каждого элемента в коллекции <paramref name="list"/>
        /// </summary>
        public static void ForEach<T>(this LinkedList<T> list, Action<T> action)
        {
            foreach (T elem in list)
                action(elem);
        }

        /// <summary>
        /// Применение действия <paramref name="action"/> для каждого элемента в коллекции <paramref name="collection"/>
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (T elem in collection)
                action(elem);
        }

#if false
        /// <summary>
        /// Вычисление размера (стороны квадрата) якоря для прямоугольника, 
        /// заданного верхней левой точкой с координатами <paramref name="leftX"/>, <paramref name="topY"/> 
        /// и правой нижней точкой с координатами <paramref name="rightX"/>, <paramref name="downY"/>.
        /// </summary>
        public static double AnchorWorldSize(double leftX, double topY, double rightX, double downY)
        {
            double width = rightX - leftX,
                   height = topY - downY,
                   anchorSize = Math.Max(2, Math.Min(Math.Min(width / 3, height / 3), DefaultAnchorSize));
            return anchorSize;
        } 
#endif
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
        /// Тип реоигна задается флагом <paramref name="isElliptical"/>
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
                graphics.FillSquareCentered(brushCR, leftX, topY, anchorSize, true);
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
    }
}
