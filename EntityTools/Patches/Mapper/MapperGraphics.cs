//#define USE_GRAPH_CACHE
using AStar.Tools;
using Astral.Logic.Classes.Map;
using EntityTools.Settings;
using MyNW.Classes;
using System;
using System.Drawing;
using System.Threading;

namespace EntityTools.Patches.Mapper
{
    /// <summary>
    /// Переопределение класса <seealso cref="Astral.Logic.Classes.Map.GraphicsNW"/>
    /// </summary>
    public partial class MapperGraphics : GraphicsNW
    {
        public MapperGraphics(int width, int height
#if USE_GRAPH_CACHE
        , MapperGraphCache graphCache = null 
#endif
            ) : base(width, height)
        {
            var mapperFormConfig = EntityTools.Config.Mapper.MapperForm;
            backColor = mapperFormConfig.BackgroundColor;

            mapperFormConfig.PropertyChanged += handler_PropertyChanged;
#if USE_GRAPH_CACHE
            GetWorldPosition(0, 0, out double left, out double top);
            GetWorldPosition(width, height, out double right, out double down);
            if (graphCache is null)
                cache = new MapperGraphCache(() => ACTP0Tools.AstralAccessors.Quester.Core.Meshes,
                    EntityTools.Config.Mapper.CacheActive);
            else cache = graphCache;
            cache.SetCacheArea(left, top, right, down); 
#endif
        }

        private void handler_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(sender is MapperFormSettings mapperFormSettings
                && e.PropertyName == nameof(MapperFormSettings.BackgroundColor))
            {
                backColor = mapperFormSettings.BackgroundColor;
            }
        }

        #region Reflection
        public override int ImageHeight
        {
            get => base.ImageHeight;
            set
            {
                base.ImageHeight = value;
#if USE_GRAPH_CACHE
                cache.CacheDistanceY = value / 1.8d / Zoom; 
#endif
            }
        }
        public override int ImageWidth
        {
            get => base.ImageWidth;
            set
            {
                base.ImageWidth = value;
#if USE_GRAPH_CACHE
                cache.CacheDistanceX = value / 1.8d / Zoom; 
#endif
            }
        }


        public override double Zoom
        {
            get => base.Zoom;
            set
            {
                if (Math.Abs(base.Zoom - value) > 0.05f)
                {
                    base.Zoom = value;
#if USE_GRAPH_CACHE
                    if (cache != null)
                    {
                        cache.CacheDistanceX = base.ImageWidth / 1.8d / value;
                        cache.CacheDistanceY = base.ImageHeight / 1.8d / value;
                    } 
#endif
                }
            }
        }


        public override Vector3 CenterPosition
        {
            get => base.CenterPosition.Clone();
            set
            {
                base.CenterPosition = value;
#if USE_GRAPH_CACHE
                cache?.SetCacheInitialPosition(value); 
#endif
            }
        }
        /// <summary>
        /// Перемещение центра изображения на величины <paramref name="dx"/> и <paramref name="dy"/>
        /// </summary>
        public void MoveCenterPosition(double dx, double dy, double dz)
        {
            base.CenterPosition.X += (float)dx;
            base.CenterPosition.Y += (float)dy;
            base.CenterPosition.Z += (float)dz;
#if USE_GRAPH_CACHE
            cache.MoveCenterPosition(dx, dy, dz); 
#endif
        }

        /// <summary>
        /// Задание геометрических атрибутов изображения с переносом центра в точку <paramref name="centerX"/>, <paramref name="centerY"/>, <paramref name="centerZ"/>
        /// </summary>
        public void Reinitialize(double centerX, double centerY, double centerZ, int width, int height, double zoom)
        {
            Reinitialize(centerX, centerY, centerZ, width, height, zoom, out _, out _, out _, out _);
        }

        /// <summary>
        /// Задание геометрических атрибутов изображения с переносом центра в <paramref name="centerPosition"/>
        /// </summary>
        public void Reinitialize(Vector3 centerPosition, int width, int height, double zoom)
        {
            Reinitialize(centerPosition.X, centerPosition.Y, centerPosition.Z, width, height, zoom, out _, out _, out _, out _);
        }
        /// <summary>
        /// Задание геометрических атрибутов изображения с переносом центра в <paramref name="centerPosition"/>
        /// </summary>
        public void Reinitialize(Vector3 centerPosition, int width, int height, double zoom, out double x1, out double y1, out double x2, out double y2)
        {
            Reinitialize(centerPosition.X, centerPosition.Y, centerPosition.Z, width, height, zoom, out x1, out y1, out x2, out y2);
        }
        /// <summary>
        /// Задание геометрических атрибутов изображения без переноса центра
        /// </summary>
        public void Reinitialize(int width, int height, double zoom, out double x1, out double y1, out double x2, out double y2)
        {
            int imgWidth = base.ImageHeight,
                imgHeight = base.ImageHeight;

            double dx = width / 2d / zoom;
            double dy = height / 2d / zoom;

            var centerPos = base.CenterPosition;
            x1 = centerPos.X - dx;
            x2 = centerPos.X + dx;
            y1 = centerPos.Y + dy;
            y2 = centerPos.Y - dy;

            if (imgWidth != width || imgHeight != height || !base.Zoom.Equals(zoom))
            {
#if USE_GRAPH_CACHE
                if (cache.CacheDistanceX > dx * 1.25
                            || cache.CacheDistanceY > dy * 1.25
                            || !cache.InCacheArea(x1, y1)
                            || !cache.InCacheArea(x2, y2))
                {
                    cache.SetCacheArea(x1, y1, x2, y2);
                } 
#endif

                base.ImageHeight = height;
                base.ImageWidth = width;
                base.Zoom = zoom;
            }
            resetImage();
        }

        /// <summary>
        /// Задание геометрических атрибутов изображения и вычисление игровых координат верхнего левого угла <paramref name="x1"/>, <paramref name="y1"/>
        /// и нижнего правого угла <paramref name="x2"/>, <paramref name="y2"/>
        /// </summary>
        public void Reinitialize(double centerX, double centerY, double centerZ, int width, int height, double zoom, out double x1, out double y1, out double x2, out double y2)
        {
            int imgWidth = base.ImageHeight,
                imgHeight = base.ImageHeight;

            double dx = width / 2d / zoom;
            double dy = height / 2d / zoom;

            x1 = centerX - dx;
            x2 = centerX + dx;
            y1 = centerY + dy;
            y2 = centerY - dy;

            var pos = base.CenterPosition;
            if (imgWidth != width || imgHeight != height || !base.Zoom.Equals(zoom)
                || Math.Abs(pos.X - centerX) > 0.1
                || Math.Abs(pos.Y - centerY) > 0.1
                || Math.Abs(pos.Z - centerZ) > 0.1)
            {
#if USE_GRAPH_CACHE
                if (cache.CacheDistanceX > dx * 1.25
                            || cache.CacheDistanceY > dy * 1.25
                            || !cache.InCacheArea(x1, y1)
                            || !cache.InCacheArea(x2, y2))
                    cache.SetCacheArea(x1, y1, centerZ + cache.CacheDistanceZ, x2, y2, centerZ - cache.CacheDistanceZ); 
#endif

                pos.X = (float)centerX;
                pos.Y = (float)centerY;
                pos.Z = (float)centerZ;

                base.ImageHeight = height;
                base.ImageWidth = width;
                base.Zoom = zoom;
            }
        }

        #endregion

        #region ReaderWriterLocker
        /// <summary>
        /// Объект синхронизации доступа к объекту <see cref="GraphicsNW"/>
        /// </summary>
        private readonly ReaderWriterLockSlim @lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

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

#if false
        /// <summary>
        /// Отображаемый подграф (часть карты путей, на которой находится персонаж)
        /// </summary>
        public MapperGraphCache GraphCache => cache;
        private readonly MapperGraphCache cache; 
#endif

        public MapperDrawingTools DrawingTools { get; } = new MapperDrawingTools();

        #region Перевод координат
        /// <summary>
        /// Перевод мировых координат <param name="worldPos"/> в координаты изображения
        /// </summary>
        /// <typeparam name="TPoint"></typeparam>
        /// <param name="worldPos"></param>
        /// <returns></returns>
        public PointF GetImagePosition<TPoint>(TPoint worldPos)
        {
            double scale = Zoom;
            PointHelper.GetXY(worldPos, out double worldX, out double worldY);

            double x = ImageWidth / 2.0 - (CenterPosition.X - worldY) * scale;// - 0.5f;
            double y = ImageHeight / 2.0 - (worldX - CenterPosition.Y) * scale;// - 0.5f;

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
            double scale = Zoom;
            PointHelper.GetXY(worldPos, out double worldX, out double worldY);

            x = ImageWidth / 2.0 - (CenterPosition.X - worldX) * scale;// - 0.5;
            y = ImageHeight / 2.0 - (worldY - CenterPosition.Y) * scale;// - 0.5;
        }
        /// <summary>
        /// Перевод игровых координат <param name="worldX"/>, <param name="worldY"/> в координаты изображения
        /// </summary>
        public void GetImagePosition(double worldX, double worldY, out double x, out double y)
        {
            double scale = Zoom;

            x = ImageWidth / 2.0 - (CenterPosition.X - worldX) * scale;// - 0.5;
            y = ImageHeight / 2.0 - (worldY - CenterPosition.Y) * scale;// - 0.5;
        }

        /// <summary>
        /// Перевод координат изображения <paramref name="imgPos"/> 
        /// в игровые координаты <param name="x"/> и <param name="y"/>
        /// </summary>
        public void GetWorldPosition(Point imgPos, out double x, out double y)
        {
#if false
        public static Vector3 getWorldPos(Point imgPoint, int boxWidth, int boxHeight, Vector3 centerPos, double Zoom)
		{
			Point point = imgPoint;
			point.X = boxWidth - point.X;
			point.Y = boxHeight - point.Y;
			Vector3 vector = Vector3.Empty;
			int num = Convert.ToInt32((double)boxWidth + 0.5);
			int num2 = Convert.ToInt32((double)boxHeight + 0.5);
			Point point2 = new Point(Convert.ToInt32((double)num / 2.0 + 0.5), Convert.ToInt32((double)num2 / 2.0 + 0.5));
			vector.X = (double)Convert.ToInt32((double)(point2.X - point.X) * 1.0 / Zoom + (double)centerPos.X);
			vector.Y = (double)Convert.ToInt32((double)(point.Y - point2.Y) * 1.0 / Zoom + (double)centerPos.Y);
			return vector;
		}
#endif
            double scale = Zoom;

            x = CenterPosition.X - (ImageWidth / 2.0 - imgPos.X /*- 0.75*/) / scale;
            y = CenterPosition.Y + (ImageHeight / 2.0 - imgPos.Y /*- 0.75*/) / scale;
        }
        /// <summary>
        /// Перевод координат изображения <paramref name="imgX"/>, <paramref name="imgY"/> 
        /// в игровые координаты <param name="x"/> и <param name="y"/>
        /// </summary>
        public void GetWorldPosition(double imgX, double imgY, out double x, out double y)
        {
            double scale = Zoom;

            x = CenterPosition.X - (ImageWidth / 2.0 - imgX /*- 0.75*/) / scale;
            y = CenterPosition.Y + (ImageHeight / 2.0 - imgY /*- 0.75*/) / scale;
        }
        #endregion

        /// <summary>
        /// Нарисовать закрашенный круг с центром в точке <paramref name="worldCenterPoint"/>, заданной в координатах игрового мира
        /// </summary>
        public void FillCircleCentered<TPoint>(Brush brush, TPoint worldCenterPoint, double diameter = 8, bool useScale = false)
        {
            PointHelper.GetXY(worldCenterPoint, out double x, out double y);
            FillCircleCentered(brush, x, y, (float) diameter, useScale);
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
            g.FillEllipse(brush, (float)(x - radius), (float)(y - radius), (float)diameter, (float)diameter);

        }
        /// <summary>
        /// Нарисовать круг с центром в точке <paramref name="worldCenterPoint"/>, заданной в координатах игрового мира
        /// </summary>
        public void DrawCircleCentered<TPoint>(Pen pen, TPoint worldCenterPoint, double diameter = 8, bool useScale = false)
        {
            PointHelper.GetXY(worldCenterPoint, out double x, out double y);
            DrawCircleCentered(pen, x, y, diameter, useScale);
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
            g.DrawEllipse(pen, (float)(x - radius), (float)(y - radius), (float)diameter, (float)diameter);
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

            g.FillEllipse(brush, (float)x, (float)y, (float)width, (float)height);
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

            g.DrawEllipse(pen, (float)x, (float)y, (float)width, (float)height);
        }

        /// <summary>
        /// Нарисовать закрашенный прямоугольник со сторонами <paramref name="width"/> и <paramref name="height"/>, 
        /// верхняя правая вершина которого задана точкой игрового мира <paramref name="point"/>
        /// </summary>
        public void FillRectangle<TPoint>(Brush brush, TPoint point, double width, double height, bool useScale = false)
        {
            FillRectangle(brush, PointHelper.GetX(point), PointHelper.GetY(point), width, height, useScale);
        }
        /// <summary>
        /// Нарисовать закрашенный прямоугольник со сторонами <paramref name="width"/> и <paramref name="height"/>, 
        /// верхняя правая вершина которого задана координатами игрового мира <paramref name="worldX"/>, <paramref name="worldY"/>
        /// </summary>
        public void FillRectangle(Brush brush, double worldX, double worldY, double width, double height, bool useScale)
        {
            GetImagePosition(worldX, worldY, out double x, out double y);
            if (useScale)
            {
                double scale = Zoom;
                width *= scale;
                height *= scale;
            }
            g.FillRectangle(brush, (float)x, (float)y, (float)width, (float)height);
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

            g.FillRectangle(brush, (float)x1, (float)y1, width, height);
        }
        /// <summary>
        /// Нарисовать прямоугольник со сторонами <paramref name="width"/> и <paramref name="height"/>, 
        /// верхняя правая вершина которого задана точкой игрового мира <paramref name="point"/>
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
        public void DrawRectangle(Pen pen, double worldX, double worldY, double width, double height, bool useScale)
        {
            GetImagePosition(worldX, worldY, out double x, out double y);
            if (useScale)
            {
                double scale = Zoom;
                width *= scale;
                height *= scale;
            }
            g.DrawRectangle(pen, (float)x, (float)y, (float)width, (float)height);
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

            g.DrawRectangle(pen, (float)x1, (float)y1, width, height);
        }

        /// <summary>
        /// Отрисовка заполненного равностороннего треугольника с основанием <paramref name="edge"/> внизу
        /// с центром в точке <paramref name="worldCenterPoint"/>, заданной в координатах игрового мира
        /// </summary>
        public void FillTriangleCentered<TPoint>(Brush brush, TPoint worldCenterPoint, double edge, bool useScale = false)
        {
            GetImagePosition(worldCenterPoint, out double x, out double y);
            if (useScale)
                edge /= Zoom;
            float dx = (float)edge / 2,
                  dy = dx * MapperHelper.tg30,
                  r = dx / MapperHelper.cos30;

            PointF[] points = {
                    new PointF((float)x,      (float)y - r),
                    new PointF((float)x - dx, (float)y + dy),
                    new PointF((float)x + dx, (float)y + dy)
                };
            g.FillPolygon(brush, points);
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

            PointF[] points = {
                    new PointF((float)x,      (float)y + r),
                    new PointF((float)x - dx, (float)y - dy),
                    new PointF((float)x + dx, (float)y - dy)
                };
            g.FillPolygon(brush, points);
        }
        /// <summary>
        /// Отрисовка заполненного квадрата со стороной <paramref name="edge"/>
        /// с центром в точке <paramref name="worldCenterPoint"/>, заданной в координатах игрового мира
        /// </summary>
        public void FillSquareCentered<TPoint>(Brush brush, TPoint worldCenterPoint, double edge, bool useScale = false)
        {
            PointHelper.GetXY(worldCenterPoint, out double x, out double y);
            FillSquareCentered(brush, x, y, edge, useScale);
        }
        /// <summary>
        /// Отрисовка заполненного квадрата со стороной <paramref name="edge"/>
        /// с центром в точке с координатами <paramref name="worldCenterX"/> и <paramref name="worldCenterY"/>, заданной в координатах игрового мира
        /// </summary>
        public void FillSquareCentered(Brush brush, double worldCenterX, double worldCenterY, double edge, bool useScale = false)
        {
            GetImagePosition(worldCenterX, worldCenterY, out double x, out double y);
            if (useScale)
                edge *= (float)Zoom;
            float halfEdge = (float)edge / 2, 
                x1 = (float)x - halfEdge, 
                x2 = (float)x + halfEdge, 
                y1 = (float)y - halfEdge,
                y2 = (float)y + halfEdge;

            PointF[] points = {
                    new PointF(x1, y1),
                    new PointF(x1, y2),
                    new PointF(x2, y2),
                    new PointF(x2, y1)
                };
            g.FillPolygon(brush, points);
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

            PointF[] points = {
                    new PointF((float)x + dx, (float)y),
                    new PointF((float)x,      (float)y + dy),
                    new PointF((float)x - dx, (float)y),
                    new PointF((float)x,      (float)y - dy)
                };
            g.FillPolygon(brush, points);
        }


        /// <summary>
        /// Отрисовка линии из точки <paramref name="p1"/> в точку <paramref name="p2"/>
        /// </summary>
        public void DrawLine<TPoint1, TPoint2>(Pen pen, TPoint1 p1, TPoint2 p2)
        {
            GetImagePosition(p1, out double x1, out double y1);
            GetImagePosition(p2, out double x2, out double y2);
            g.DrawLine(pen, (float)x1, (float)y1, (float)x2, (float)y2);
        }
        /// <summary>
        /// Отрисовка линии из точки c координатами <paramref name="worldX1"/>, <paramref name="worldY1"/> 
        /// в точку c координатами <paramref name="worldX2"/>, <paramref name="worldY2"/> 
        /// </summary>
        public void DrawLine(Pen pen, double worldX1, double worldY1, double worldX2, double worldY2)
        {
            GetImagePosition(worldX1, worldY1, out double x1, out double y1);
            GetImagePosition(worldX2, worldY2, out double x2, out double y2);
            g.DrawLine(pen, (float)x1, (float)y1, (float)x2, (float)y2);
        }

        /// <summary>
        /// Отрисовка текста<paramref name="text"/> в точке  <paramref name="worldPoint"/>,
        /// положение которой в прямоугольнике, охватывающем текс задано параметром <paramref name="align"/>
        /// </summary>
        public void DrawText<TPoint>(string text, TPoint worldPoint, Alignment align = Alignment.TopLeft, Font font = null, Brush brush = null)
        {
            if (align != Alignment.None)
            {
                PointHelper.GetXY(worldPoint, out double x, out double y);
                DrawText(text, x, y, align, font, brush);
            }
        }

        public void DrawText(string text, double worldX, double worldY, Alignment align = Alignment.MiddleCenter, Font font = null, Brush brush = null)
        {
            if (font is null)
                font = SystemFonts.DefaultFont;
            if (brush is null)
                brush = Brushes.Black;
            if (align != Alignment.None)
            {
                var size = g.MeasureString(text, font);

                GetImagePosition(worldX, worldY, out double x, out double y);

                switch (align)
                {
                    case Alignment.TopLeft:
                        x += 0.5f;
                        y += 0.5f;
                        break;
                    case Alignment.TopCenter:
                        x -= size.Width / 2 - 0.5f;
                        y += 0.5f;
                        break;
                    case Alignment.TopRight:
                        x -= size.Width - 0.5f;
                        y += 0.5f;
                        break;
                    case Alignment.MiddleLeft:
                        x += 0.5f;
                        y -= size.Height / 2 - 0.5f;
                        break;
                    case Alignment.MiddleCenter:
                        x -= size.Width / 2 - 0.5f;
                        y -= size.Height / 2 - 0.5f;
                        break;
                    case Alignment.MiddleRight:
                        x -= size.Width - 0.5f;
                        y -= size.Height / 2 - 0.5f;
                        break;
                    case Alignment.BottomLeft:
                        x += 0.5f;
                        y -= size.Height - 0.5f;
                        break;
                    case Alignment.BottomCenter:
                        x -= size.Width / 2;
                        y -= size.Height - 0.5f;
                        break;
                    case Alignment.BottomRight:
                        x -= size.Width - 0.5f;
                        y -= size.Height - 0.5f;
                        break;
                }

                g.DrawString(text, font, brush, (float)x, (float)y);
            }
        }
    }
}
