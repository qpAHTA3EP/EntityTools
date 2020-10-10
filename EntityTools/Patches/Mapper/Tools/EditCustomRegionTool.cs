using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using AStar;
using EntityTools.Patches.Mapper.Tools;
using HarmonyLib;
using Astral.Quester.Classes;
using MyNW.Classes;

namespace EntityTools.Patches.Mapper.Tools
{
    /// <summary>
    /// Инструмент для удаления вершин
    /// </summary>
    public class EditCustomRegionTool : IMapperTool
    {
        public EditCustomRegionTool() { }

        public EditCustomRegionTool(CustomRegion cr)
        {
            AttachTo(cr);
        }

        public void AttachTo(CustomRegion cr)
        {
            customRegion = cr;
            crName = Name = cr.Name;
            crElliptic = IsElliptical = cr.Eliptic;
            crX = (float)(startX = cr.Position.X);
            crY = (float)(startY = cr.Position.Y);
            crWidth = cr.Width;
            endX = startX + crWidth;
            crHeight = cr.Height;
            endY = startY + crHeight;

            changed = false;
            transformMode = RegionTransformMode.None;
        }

        #region данные
        #region исходные данные CustomRegion'a для отката изменений
        private string crName;
        private float crX, crY;
        private int crWidth, crHeight;
        private bool crElliptic;
        #endregion

        // координаты начальной точки CustomRegion'a
        private double startX;
        private double startY;

        // координаты конечной точки CustomRegion'a
        private double endX;
        private double endY;

        // Режим трансформации CustomRegion'a
        private RegionTransformMode transformMode = RegionTransformMode.None;

        /// <summary>
        /// Сконструированный CustomRegion (для выполнения Undo)
        /// </summary>
        private CustomRegion customRegion;

        private bool changed = false;

        /// <summary>
        /// Флаг, указывающий является ли CustomRegion элиптическим
        /// </summary>
        public bool IsElliptical { get; set; } = false;

        /// <summary>
        /// Имя CustomRegion'a
        /// </summary>
        public string Name{ get; set; }
        #endregion

        /// <summary>
        /// Воведенные данные корректны
        /// </summary>
        public bool IsCorrect => customRegion!= null && !(startX == 0 && startY == 0) && !(endX == 0 && endY == 0) && Math.Abs(startX - endX) >= 1 && Math.Abs(startY - endY) >= 1;

        /// <summary>
        /// Использование механизма выделения вершин
        /// </summary>
        public bool AllowNodeSelection => false;

        /// <summary>
        /// Режим редактирования
        /// </summary>
        public MapperEditMode EditMode => MapperEditMode.EditCustomRegion;

        public bool HandleCustomDraw => !(startX == 0 && startY == 0) && !(endX == 0 && endY == 0);
        /// <summary>
        /// Отрисовка выделенной вершины
        /// </summary>
        public void OnCustomDraw(MapperGraphics graphics, NodeSelectTool nodes, double worldMouseX, double worldMouseY)
        {
            if (!(startX == 0 && startY == 0))
            {
                if (!(endX == 0 && endY == 0))
                {
                    if (transformMode == RegionTransformMode.None && transformMode == RegionTransformMode.Disabled)
                    {
                        // Отрисовываем регион в зафиксированном состоянии
                        graphics.DrawCustomRegion(startX, startY, endX, endY, IsElliptical);
                    }
                    else
                    {
                        // отрисовываем регион в режиме трансформации
                        double left = startX,
                               right = endX,
                               top = startY,
                               down = endY;
                        MapperHelper.TransformRegion(ref left, ref top, ref right, ref down, Math.Round(worldMouseX), Math.Round(worldMouseY), transformMode);
                        graphics.DrawCustomRegion(left, top, right, down, IsElliptical);
                    }
                }
                else
                {
                    // Отрисовываем прямоугольник будущего региона
                    MapperHelper.FixRange(startX, Math.Round(worldMouseX), out double left, out double right);
                    MapperHelper.FixRange(startY, Math.Round(worldMouseY), out double down, out double top);
                    graphics.DrawCustomRegion(left, top, right, down, IsElliptical);
                }
            }
        }

        public bool HandleKeyUp => true;
        public void OnKeyUp(IGraph graph, NodeSelectTool nodes, KeyEventArgs e, double worldMouseX, double worldMouseY, out IMapperTool undo)
        {
            undo = null;
            if (e.KeyCode == Keys.Escape)
            {
                if (transformMode == RegionTransformMode.None && transformMode == RegionTransformMode.Disabled)
                    // Сбрасываем режим трансформации
                    transformMode = RegionTransformMode.None;
                else
                {
                    // Сбрасываем выделение
                    IsElliptical = customRegion.Eliptic;
                    startX = customRegion.Position.X;
                    startY = customRegion.Position.Y;
                    endX = startX + customRegion.Width;
                    endY = startY + customRegion.Height;
                }
            }
            else if (e.KeyCode == Keys.Enter)
                // Иммитируем нажатие правой кнопки мыши
                OnMouseClick(graph, nodes, new MapperMouseEventArgs(MouseButtons.Right, 1, worldMouseX, worldMouseY), out undo);
        }

        public bool HandleMouseClick => true;
        public void OnMouseClick(IGraph graph, NodeSelectTool nodes, MapperMouseEventArgs e, out IMapperTool undo)
        {
            undo = null;
            if (e.Button == MouseButtons.Right)
            {
#if false
                if (startX == 0 && startY == 0)
                {
                    // Отмечаем начальную точку CustomRegion'a
                    startX = Math.Round(e.X);
                    startY = Math.Round(e.Y);
                    transformMode = RegionTransformMode.None;
                }
                else if (endX == 0 && endY == 0)
                {
                    // Отмечаем конечную точку CustomRegion'a
                    MapperHelper.FixRange(startX, Math.Round(e.X), out startX, out endX);
                    MapperHelper.FixRange(startY, Math.Round(e.Y), out startY, out endY);
                    transformMode = RegionTransformMode.None;
                }
                else  
#endif
                if(transformMode == RegionTransformMode.None && transformMode != RegionTransformMode.Disabled)
                {
                    // проверяем выбор якоря и режима трансформации
                    double width = Math.Abs(endX - startX),
                           height = Math.Abs(endY - startY),
                           anchorSize = MapperHelper.AnchorWorldSize(width, height);

                    MapperHelper.SelectAnchor(startX, startY, endX, endY, e.X, e.Y, anchorSize, out transformMode);
                }
                else
                {
                    // преобразование CustomRegion'a
                    MapperHelper.TransformRegion(ref startX, ref startY, ref endX, ref endY, Math.Round(e.X), Math.Round(e.Y), transformMode);
                    transformMode = RegionTransformMode.None;
                }
            }
        }

        /// <summary>
        /// Указывает, что инструмент был применен
        /// </summary>
        public bool Modified
        {
            get
            {
                MapperHelper.FixRange(startX, endX, out double x1, out double x2);
                MapperHelper.FixRange(startY, endY, out double y2, out double y1);
                return customRegion != null
                    && (crX != (float) x1 || crY != (float) y1
                         || crElliptic != IsElliptical
                         || crWidth != (x2 - x1) || crHeight != (y2 - y1)
                         || Name != crName);
            }
        }

        public bool Applied => _applied;
        private bool _applied = false;

        public CustomRegion Apply(string name = "")
        {
#if false
            var crList = Astral.Quester.API.CurrentProfile.CustomRegions;

            // Запрет повторяющихся имен
            if (crList.Count > 0
                && crList.Find(cr => cr.Name == name) != null)
                return null; 
#endif
            if(!(startX == 0 && startY == 0)
                && !(endX == 0 && endY == 0)
                && Math.Abs(startX - endX) >= 1  && Math.Abs(startY - endY) >= 1)
            {
                MapperHelper.FixRange(startX, endX, out startX, out endX);
                // Ось Oy в игровых координатах инвертирована по сравнению с координатами экрана windows (MapPicture)
                // Поэтому координата верхнего правого угла CustomRegion'e должна иметь максимальную Y
                MapperHelper.FixRange(startY, endY, out endY, out startY);
                int width = (int)Math.Round(endX - startX),
                    // в Astra'e высота CustomRegion'a должна быть отрицательной в связи с инверсией оси
                    height = (int)Math.Round(endY - startY);


#if false
                if (customRegion is null)
                    return customRegion = new CustomRegion()
                    {
                        Position = new Vector3((float)startX, (float)startY, 0),
                        Eliptic = IsElliptical,
                        Height = height,
                        Width = width,
                        Name = name
                    };
                else 
                {
#else 
                if (customRegion != null)
                { 
#endif
                    if (!string.IsNullOrEmpty(name))
                        customRegion.Name = name;

                    customRegion.Position.X = (float)Math.Round(startX);
                    customRegion.Position.Y = (float)Math.Round(startY);
                    customRegion.Eliptic = IsElliptical;
                    customRegion.Height = height;
                    customRegion.Width = width;

                    _applied = true;

                    return customRegion;
                }
            }
            return null;
        }

        /// <summary>
        /// Откат изменений, внесенных инструментом
        /// </summary>
        public void Undo()
        {
            if (customRegion != null)
            {
                customRegion.Name = crName;
                customRegion.Position.X = crX;
                customRegion.Position.Y = crY;
                customRegion.Eliptic = IsElliptical;
                customRegion.Height = crHeight;
                customRegion.Width = crWidth;

                _applied = false;
                customRegion = null;
            }
        }
    }
}