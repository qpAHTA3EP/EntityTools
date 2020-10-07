using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using AStar.Tools;
using Astral.Logic.Classes.Map;
using EntityTools.Reflection;
using MyNW.Classes;
using static EntityTools.Reflection.InstanceFieldAccessorFactory;

namespace EntityTools.Patches.Mapper
{
    /// <summary>
    /// Переопределение класса <seealso cref="Astral.Logic.Classes.Map.GraphicsNW"/>
    /// </summary>
    public partial class MapperGraphics : GraphicsNW
    {
        public MapperGraphics(int width, int height) : base(width, height)
        {
            GetWorldPosition(0, 0, out double left, out double top);
            GetWorldPosition(width, height, out double right, out double down);
            _cache = new MapperGraphCache(() => AstralAccessors.Quester.Core.Meshes.Value, true);
            _cache.SetCacheArea(left, top, right, down);
        }

        #region Reflection
        /// <summary>
        /// Получение к приватному члену <seealso cref="GraphicsNW.g"/> типа Graphics
        /// </summary>
        private Graphics graphics => getGraphicsFrom(this);
        private static readonly Func<GraphicsNW, Graphics> getGraphicsFrom = GetInstanceFieldAccessor<GraphicsNW, Graphics>("g");

        /// <summary>
        /// Получение к приватному члену <seealso cref="GraphicsNW.g"/> типа Graphics
        /// </summary>
        private Image image => getImageFrom(this);
        private static readonly Func<GraphicsNW, Image> getImageFrom = GetInstanceFieldAccessor<GraphicsNW, Image>("img");

        public new int ImageHeight
        {
            get => base.ImageHeight;
            set
            {
                base.ImageHeight = value;
                _cache.CacheDistanceY = value / 2.0 / Zoom;
            }
        }
        public new int ImageWidth
        {
            get => base.ImageWidth;
            set
            {
                base.ImageWidth = value;
                _cache.CacheDistanceX = value / 2.0 / Zoom;
            }
        }
        public new Vector3 CenterPosition
        {
            get => base.CenterPosition;
            set
            {
                base.CenterPosition = value;
                _cache.CacheCenter = value;
            }
        }
        #endregion

        #region ReaderWriterLocker
        /// <summary>
        /// Объект синхронизации доступа к объекту <see cref="graphicsNW"/>
        /// </summary>
        private readonly ReaderWriterLockSlim @lock = new ReaderWriterLockSlim();

        /// <summary>
        /// Объект синхронизации для "чтения", допускающий одновременное чтение
        /// </summary>
        /// <returns></returns>
        public RWLocker.ReadLockToken ReadLock() => new RWLocker.ReadLockToken(@lock);
        /// <summary>
        /// Объект синхронизации для "записи".
        /// </summary>
        /// <returns></returns>
        public RWLocker.WriteLockToken WriteLock() => new RWLocker.WriteLockToken(@lock);
        #endregion

        /// <summary>
        /// Отображаемый подграф (часть карты путей, на которой находится персонаж)
        /// </summary>
        public MapperGraphCache VisibleGraph => _cache;
        protected MapperGraphCache _cache = null;

        #region Перевод координат
        /// <summary>
        /// Перевод мировых координат <param name="worldPos"/> в координаты изображения
        /// </summary>
        /// <typeparam name="TPoint"></typeparam>
        /// <param name="worldPos"></param>
        /// <returns></returns>
        public PointF GetImagePosition<TPoint>(TPoint worldPos)
        {
            var img = image;
            double scale = Zoom;
            PointHelper.GetXY(worldPos, out double worldX, out double worldY);

            double x = img.Width / 2.0 - (CenterPosition.X - worldY) * scale;// - 0.5f;
            double y = img.Height / 2.0 - (worldX - CenterPosition.Y) * scale;// - 0.5f;

            return new PointF((float)x, (float)y);
#if false
        public static Point getImgPos(Vector3 worldPost, int imgWidth, int imgHeight, Vector3 centerPos, double Zoom)
		{
			int num = Convert.ToInt32((double)worldPost.X + 0.5);
			double num2 = (double)Convert.ToInt32((double)worldPost.Y + 0.5);
			double num3 = (double)Convert.ToInt32((double)centerPos.X + 0.5);
			int num4 = Convert.ToInt32((double)centerPos.Y + 0.5);
			Point point = new Point(Convert.ToInt32((double)imgWidth / 2.0 + 0.5), Convert.ToInt32((double)imgHeight / 2.0 + 0.5));
			int num5 = Convert.ToInt32((num3 - (double)num) * Zoom + (double)point.X);
			int num6 = Convert.ToInt32((num2 - (double)num4) * Zoom + (double)point.Y);
			num5 = imgWidth - num5;
			num6 = imgHeight - num6;
			return new Point(num5, num6);
		}
#endif
        }
        /// <summary>
        /// Перевод мировых координат <param name="worldPos"/> в координаты изображения
        /// </summary>
        public void GetImagePosition<TPoint>(TPoint worldPos, out double x, out double y)
        {
            var img = image;
            double scale = Zoom;
            PointHelper.GetXY(worldPos, out double worldX, out double worldY);

            x = img.Width / 2.0 - (CenterPosition.X - worldX) * scale;// - 0.5;
            y = img.Height / 2.0 - (worldY - CenterPosition.Y) * scale;// - 0.5;
        }
        /// <summary>
        /// Перевод игровых координат <param name="worldX"/>, <param name="worldY"/> в координаты изображения
        /// </summary>
        public void GetImagePosition(double worldX, double worldY, out double x, out double y)
        {
            var img = image;
            double scale = Zoom;

            x = img.Width / 2.0 - (CenterPosition.X - worldX) * scale;// - 0.5;
            y = img.Height / 2.0 - (worldY - CenterPosition.Y) * scale;// - 0.5;
        }

        /// <summary>
        /// Перевод координат изображения <paramref name="imgPos"/> 
        /// в игровые координаты <param name="x"/> и <param name="y"/>
        /// </summary>
        public void GetWorldPosition(Point imgPos, out double x, out double y)
        {
            var img = image;
            double scale = Zoom;

            x = CenterPosition.X - (img.Width / 2.0 - imgPos.X /*- 0.75*/) / scale;
            y = CenterPosition.Y + (img.Height / 2.0 - imgPos.Y /*- 0.75*/) / scale;
#if false
        public static Vector3 getWorldPos(Point imgPoint, int boxWidth, int boxHeight, Vector3 centerPos, double Zoom)
		{
			Point point = imgPoint;
			point.X = boxWidth - point.X;
			point.Y = boxHeight - point.Y;
			Vector3 vector = new Vector3();
			int num = Convert.ToInt32((double)boxWidth + 0.5);
			int num2 = Convert.ToInt32((double)boxHeight + 0.5);
			Point point2 = new Point(Convert.ToInt32((double)num / 2.0 + 0.5), Convert.ToInt32((double)num2 / 2.0 + 0.5));
			vector.X = (double)Convert.ToInt32((double)(point2.X - point.X) * 1.0 / Zoom + (double)centerPos.X);
			vector.Y = (double)Convert.ToInt32((double)(point.Y - point2.Y) * 1.0 / Zoom + (double)centerPos.Y);
			return vector;
		}
#endif
        }
        /// <summary>
        /// Перевод координат изображения <paramref name="imgX"/>, <paramref name="imgY"/> 
        /// в игровые координаты <param name="x"/> и <param name="y"/>
        /// </summary>
        public void GetWorldPosition(double imgX, double imgY, out double x, out double y)
        {
            var img = image;
            double scale = Zoom;

            x = CenterPosition.X - (img.Width / 2.0 - imgX /*- 0.75*/) / scale;
            y = CenterPosition.Y + (img.Height / 2.0 - imgY /*- 0.75*/) / scale;
        }

#if false
        /// <summary>
        /// Перевод координат курсора мыши в мировые координаты <param name="x"/> и <param name="y"/>
        /// </summary>
        public void GetMouseCursorWorldPosition(out double x, out double y)
        {
            var cersorPos = Control.MousePosition;// Cursor.Position;
            var img = image;
            double scale = Zoom;

            x = CenterPosition.X - (img.Width / 2 - cersorPos.X /*- 0.75*/) / scale;
            y = CenterPosition.Y + (img.Height / 2 - cersorPos.Y /*- 0.75*/) / scale;
        }  
#endif
        #endregion

        /// <summary>
        /// Нарисовать закрашенный круг с центром в точке <paramref name="worldCenterPoint"/>, заданной в координатах игрового мира
        /// </summary>
        public void FillCircleCentered<TPoint>(Brush brush, TPoint worldCenterPoint, double diameter = 8, bool useScale = false)
        {
#if true
            PointHelper.GetXY(worldCenterPoint, out double x, out double y);
            FillCircleCentered(brush, x, y, (float) diameter, useScale);
#else
            GetImagePosition(worldCenterPoint, out double x, out double y);
            if (useScale)
                diameter *= Zoom;

            double radius = diameter / 2;
            graphics.FillEllipse(brush, (float)(x - radius), (float)(y - radius), (float)diameter, (float)diameter);
#endif

        }
        /// <summary>
        /// Нарисовать закрашенный круг с центром в точке c игровыми координатами <paramref name="worldX"/>, <paramref name="worldY"/>
        /// </summary>
        public void FillCircleCentered(Brush brush, double worldX, double worldY, double diameter = 8, bool useScale = false)
        {
            GetImagePosition(worldX, worldY, out double x, out double y);
            if (useScale)
                diameter *= Zoom;

            double radius = diameter / 2;
            graphics.FillEllipse(brush, (float)(x - radius), (float)(y - radius), (float)diameter, (float)diameter);

        }
        /// <summary>
        /// Нарисовать круг с центром в точке <paramref name="worldCenterPoint"/>, заданной в координатах игрового мира
        /// </summary>
        public void DrawCircleCentered<TPoint>(Pen pen, TPoint worldCenterPoint, double diameter = 8, bool useScale = false)
        {
#if true
            PointHelper.GetXY(worldCenterPoint, out double x, out double y);
            DrawCircleCentered(pen, x, y, diameter, useScale);
#else
            GetImagePosition(worldCenterPoint, out double x, out double y);
            if (useScale)
                diameter *= Zoom;

            double radius = diameter / 2;
            graphics.DrawEllipse(pen, (float)(x - radius), (float)(y - radius), (float)diameter, (float)diameter);
#endif
        }
        /// <summary>
        /// Нарисовать круг с центром в точке c игровыми координатами <paramref name="worldX"/>, <paramref name="worldY"/>
        /// </summary>
        public void DrawCircleCentered(Pen pen, double worldX, double worldY, double diameter = 8, bool useScale = false)
        {
            GetImagePosition(worldX, worldY, out double x, out double y);
            if (useScale)
                diameter *= Zoom;

            double radius = diameter / 2;
            graphics.DrawEllipse(pen, (float)(x - radius), (float)(y - radius), (float)diameter, (float)diameter);
        }

        /// <summary>
        /// Нарисовать закрашенный эллипс вписанный в прямоугольник со сторонами <paramref name="width"/> и <paramref name="height"/>, 
        /// верхняя правая вершина которого задана координатами игрового мира <paramref name="worldX"/>, <paramref name="worldY"/>
        /// </summary>
        public void FillEllipse(Brush brush, double worldX, double worldY, double width, double height, bool useScale = false)
        {
            GetImagePosition(worldX, worldY, out double x, out double y);
            if (useScale)
            {
                double scale = Zoom;
                width *= scale;
                height *= scale;
            }

            graphics.FillEllipse(brush, (float)x, (float)y, (float)width, (float)height);
        }

        /// <summary>
        /// Нарисовать эллипс вписанный в прямоугольник со сторонами <paramref name="width"/> и <paramref name="height"/>, 
        /// верхняя правая вершина которого задана координатами игрового мира <paramref name="worldX"/>, <paramref name="worldY"/>
        /// </summary>
        public void DrawEllipse(Pen pen, double worldX, double worldY, double width, double height, bool useScale = false)
        {
            GetImagePosition(worldX, worldY, out double x, out double y);
            if (useScale)
            {
                double scale = Zoom;
                width *= scale;
                height *= scale;
            }

            graphics.DrawEllipse(pen, (float)x, (float)y, (float)width, (float)height);
        }

        /// <summary>
        /// Нарисовать закрашенный прямоугольник со сторонами <paramref name="width"/> и <paramref name="height"/>, 
        /// верхняя правая вершина которого задана координатами игрового мира <paramref name="worldCenterX"/>, <paramref name="worldCenterY"/>
        /// </summary>
        public void FillRectangle<TPoint>(Brush brush, TPoint point, double width, double height, bool useScale = false)
        {
            FillRectangle(brush, PointHelper.GetX(point), PointHelper.GetY(point), width, height, useScale);
        }
        /// <summary>
        /// Нарисовать закрашенный прямоугольник со сторонами <paramref name="width"/> и <paramref name="height"/>, 
        /// верхняя правая вершина которого задана координатами игрового мира <paramref name="x1"/>, <paramref name="x2"/>
        /// </summary>
        public void FillRectangle(Brush brush, double worldX, double worldY, double width, double height, bool useScale = false)
        {
            GetImagePosition(worldX, worldY, out double x, out double y);
            if (useScale)
            {
                double scale = Zoom;
                width *= scale;
                height *= scale;
            }
            graphics.FillRectangle(brush, (float)x, (float)y, (float)width, (float)height);
        }
        /// <summary>
        /// Нарисовать закрашенный прямоугольник
        /// верхняя правая вершина которого задана координатами игрового мира <paramref name="worldX1"/>, <paramref name="worldY1"/>,
        /// а нижняя левая вершина которого задана координатами игрового мира <paramref name="worldX2"/>, <paramref name="worldY2"/>
        /// </summary>
        public void FillRectangle(Brush brush, double worldX1, double worldY1, double worldX2, double worldY2)
        {
            GetImagePosition(worldX1, worldY1, out double x1, out double y1);
            GetImagePosition(worldX2, worldY2, out double x2, out double y2);
            float width, height;

            if (x1 > x2)
            {
                width = (float)(x1 - x2);
                x1 = x2;
            }
            else width = (float)(x2 - x1);

            if (y1 > y2)
            {
                height = (float)(y1 - y2);
                y1 = y2;
            }
            else height = (float)(y2 - y1);

            graphics.FillRectangle(brush, (float)x1, (float)y1, width, height);
        }
        /// <summary>
        /// Нарисовать прямоугольник со сторонами <paramref name="width"/> и <paramref name="height"/>, 
        /// верхняя правая вершина которого задана координатами игрового мира <paramref name="worldCenterX"/>, <paramref name="worldCenterY"/>
        /// </summary>
        /// <typeparam name="TPoint"></typeparam>
        /// <param name="point"></param>
        /// <param name="pen"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="useScale"></param>
        public void DrawRectangle<TPoint>(Pen pen, TPoint point, double width, double height, bool useScale = false)
        {
            DrawRectangle(pen, PointHelper.GetX(point), PointHelper.GetY(point), width, height, useScale);
        }
        /// <summary>
        /// Нарисовать прямоугольник со сторонами <paramref name="width"/> и <paramref name="height"/>, 
        /// верхняя правая вершина которого задана координатами игрового мира <paramref name="worldX"/>, <paramref name="worldY"/>
        /// </summary>
        public void DrawRectangle(Pen pen, double worldX, double worldY, double width, double height, bool useScale = false)
        {
            GetImagePosition(worldX, worldY, out double x, out double y);
            if (useScale)
            {
                double scale = Zoom;
                width *= scale;
                height *= scale;
            }
            graphics.DrawRectangle(pen, (float)x, (float)y, (float)width, (float)height);
        }
        /// <summary>
        /// Нарисовать прямоугольник со сторонами,
        /// верхняя правая вершина которого задана координатами игрового мира <paramref name="worldX1"/>, <paramref name="worldY1"/>
        /// а нижняя левая вершина - координатами<paramref name="worldX2"/>, <paramref name="worldY2"/>
        /// </summary>
        public void DrawRectangle(Pen pen, double worldX1, double worldY1, double worldX2, double worldY2)
        {
            GetImagePosition(worldX1, worldY1, out double x1, out double y1);
            GetImagePosition(worldX2, worldY2, out double x2, out double y2);

            float width, height;

            if (x1 > x2)
            {
                width = (float)(x1 - x2);
                x1 = x2;
            }
            else width = (float)(x2 - x1);
            
            if (y1 > y2)
            {
                height = (float)(y1 - y2);
                y1 = y2;
            }
            else height = (float)(y2 - y1);

            graphics.DrawRectangle(pen, (float)x1, (float)y1, width, height);
        }

        /// <summary>
        /// Отрисовка заполненного равностороннего треугольника с основанием <paramref name="edge"/> внизу
        /// с центром в точке <paramref name="worldCenterPoint"/>, заданной в координатах игрового мира
        /// </summary>
        public void FillTriangleCentered<TPoint>(Brush brush, TPoint worldCenterPoint, double edge, bool useScale = false)
        {
            GetImagePosition(worldCenterPoint, out double x, out double y);
            if (useScale)
                edge *= Zoom;
            float dx = (float)edge / 2,
                  dy = dx * MapperHelper.tg30,
                  r = dx / MapperHelper.cos30;

            PointF[] points = new PointF[] {
                    new PointF((float)x,      (float)y - r),
                    new PointF((float)x - dx, (float)y + dy),
                    new PointF((float)x + dx, (float)y + dy)
                };
            graphics.FillPolygon(brush, points);
        }
        /// <summary>
        /// Отрисовка перевернутого заполненного равностороннего треугольника с основанием <paramref name="edge"/> вверху
        /// с центром в точке <paramref name="worldCenterPoint"/>, заданной в координатах игрового мира
        /// </summary>
        public void FillUpsideTriangleCentered<TPoint>(Brush brush, TPoint worldCenterPoint, double edge, bool useScale = false)
        {
            GetImagePosition(worldCenterPoint, out double x, out double y);
            if (useScale)
                edge *= (float)Zoom;
            float dx = (float)edge / 2,
                  dy = dx * MapperHelper.tg30,
                  r = dx / MapperHelper.cos30;

            PointF[] points = new PointF[] {
                    new PointF((float)x,      (float)y + r),
                    new PointF((float)x - dx, (float)y - dy),
                    new PointF((float)x + dx, (float)y - dy)
                };
            graphics.FillPolygon(brush, points);
        }
        /// <summary>
        /// Отрисовка заполненного квадрата со стороной <paramref name="edge"/>
        /// с центром в точке <paramref name="worldCenterPoint"/>, заданной в координатах игрового мира
        /// </summary>
        public void FillSquareCentered<TPoint>(Brush brush, TPoint worldCenterPoint, double edge, bool useScale = false)
        {
            GetImagePosition(worldCenterPoint, out double x, out double y);
            if (useScale)
                edge *= (float)Zoom;
            float halfEdge = (float)edge / 2;

            PointF[] points = new PointF[] {
                    new PointF((float)x - halfEdge, (float)y - halfEdge),
                    new PointF((float)x - halfEdge, (float)y + halfEdge),
                    new PointF((float)x + halfEdge, (float)y + halfEdge),
                    new PointF((float)x + halfEdge, (float)y - halfEdge)
                };
            graphics.FillPolygon(brush, points);
        }
        /// <summary>
        /// Отрисовка заполненого ромба с диагоналями <paramref name="width"/> и <paramref name="height"/>
        /// с центром в точке <paramref name="worldCenterPoint"/>, заданной в координатах игрового мира
        /// </summary>
        public void FillRhombCentered<TPoint>(Brush brush, TPoint worldCenterPoint, double width, double height, bool useScale = false)
        {
            GetImagePosition(worldCenterPoint, out double x, out double y);
            if (useScale)
            {
                float scale = (float)Zoom;
                width *= scale;
                height *= scale;
            }
            float dx = (float)width / 2,
                  dy = (float)height / 2;

            PointF[] points = new PointF[] {
                    new PointF((float)x + dx, (float)y),
                    new PointF((float)x,      (float)y + dy),
                    new PointF((float)x - dx, (float)y),
                    new PointF((float)x,      (float)y - dy)
                };
            graphics.FillPolygon(brush, points);
        }
#if false
        /// <summary>
        /// Отрисовка изображения с центром в точке <paramref name="worldCenterPoint"/>, заданной в координатах игрового мира
        /// </summary>
        public void DrawImageCentered<TPoint>(TPoint worldCenterPoint, Image image, double angle)
        {
            GetImagePosition(worldCenterPoint, out double x, out double y);
            float imgWidth = image.Width,
                  imgHeight = image.Height;
            if (angle != 0)
            {
                try
                {
                    graphics.RotateTransform((float)(angle/* - Math.PI / 2*/));
                    graphics.DrawImage(image, (float)x - imgWidth / 2, (float)y - imgHeight / 2);
                }
                finally
                {
                    graphics.ResetTransform();
                }
            }
            else graphics.DrawImage(image, (float)x, (float)y);
        } 
#endif
        /// <summary>
        /// Отрисовка линии из точки <paramref name="p1"/> в точку <paramref name="p2"/>
        /// </summary>
        public void DrawLine<TPoint1, TPoint2>(Pen pen, TPoint1 p1, TPoint2 p2)
        {
            GetImagePosition(p1, out double x1, out double y1);
            GetImagePosition(p2, out double x2, out double y2);
            graphics.DrawLine(pen, (float)x1, (float)y1, (float)x2, (float)y2);
        }
        /// <summary>
        /// Отрисовка линии из точки c координатами <paramref name="worldX1"/>, <paramref name="worldY1"/> 
        /// в точку c координатами <paramref name="worldX2"/>, <paramref name="worldY2"/> 
        /// </summary>
        public void DrawLine(Pen pen, double worldX1, double worldY1, double worldX2, double worldY2)
        {
            GetImagePosition(worldX1, worldY1, out double x1, out double y1);
            GetImagePosition(worldX2, worldY2, out double x2, out double y2);
            graphics.DrawLine(pen, (float)x1, (float)y1, (float)x2, (float)y2);
        }
    }
}
