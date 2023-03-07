using AStar;
using Astral.Quester.Classes;
using EntityTools.Quester.Mapper;
using System;
using System.Drawing;
using System.Windows.Forms;
// ReSharper disable InconsistentNaming

namespace EntityTools.Quester.Mapper.Tools
{
    public abstract class CustomRegionToolBase : IMapperTool//ICustomRegionTool
    {
        #region данные
        // координаты начальной точки CustomRegion'a
        protected double leftX;
        protected double topY;

        // координаты конечной точки CustomRegion'a
        protected double rightX;
        protected double bottomY;

        /// <summary>
        /// Сконструированный(измененный) CustomRegion (для выполнения Undo)
        /// </summary>
        protected CustomRegion customRegion;
        #endregion

        // Режим трансформации CustomRegion'a
        protected RegionTransformMode transformMode = RegionTransformMode.None;

        /// <summary>
        /// Имя CustomRegion'a
        /// </summary>
        protected string CustomRegionName => toolForm.CustomRegionName;

        #region Взаимодействие с пользователем
        /// <summary>
        /// Вспомогательное окно редактора CustomRegion'a
        /// </summary>
        protected CustomRegionToolForm toolForm
        {
            get
            {
                if (_crToolForm is null || _crToolForm.IsDisposed)
                {
                    _crToolForm = new CustomRegionToolForm();
                }
                

                switch (EditMode)
                {
                    case MapperEditMode.AddCustomRegion:
                        _crToolForm.Mode = CustomRegionToolForm.ViewMode.Add;
                        break;
                    case MapperEditMode.EditCustomRegion:
                        _crToolForm.Mode = CustomRegionToolForm.ViewMode.Edit;
                        break;
                    default:
                        _crToolForm.Mode = CustomRegionToolForm.ViewMode.Undefined;
                        break;
                }
                _crToolForm.SetEventHandlers(OnAcceptChanges, 
                                             OnCancelChanges,
                                             OnSizeChanged,
                                             OnSelectedCustomRegionChanged);
                return _crToolForm;
            }
        }
        private static CustomRegionToolForm _crToolForm = new CustomRegionToolForm(); 

        protected abstract void OnCancelChanges(CustomRegionToolForm sender, EventArgs e, object value);

        protected abstract void OnAcceptChanges(CustomRegionToolForm sender, EventArgs e, object value);

        protected virtual void OnSizeChanged(CustomRegionToolForm sender,
            CustomRegionToolForm.SizeAttribute sizeAttribute, int value)
        {
            switch (sizeAttribute)
            {
                case CustomRegionToolForm.SizeAttribute.X:
                    //var dx = value - leftX;
                    leftX = value;
                    //rightX += dx;
                    break;
                case CustomRegionToolForm.SizeAttribute.Y:
                    //var dy = value - topY;
                    topY = value;
                    //bottomY += dy;
                    break;
                case CustomRegionToolForm.SizeAttribute.Width:
                    //rightX = leftX + value;
                    MapperHelper.FixRange(leftX, leftX + value, out leftX, out rightX);
                    break;
                case CustomRegionToolForm.SizeAttribute.Height:
                    //bottomY = topY + value;
                    // Экранная ось Oy по отношению к игровой инвертирована
                    // Игровая ось ориентирована Снизу вверх
                    MapperHelper.FixRange(topY, topY - value, out bottomY, out topY);
                    break;
            }
        }

        protected abstract void OnSelectedCustomRegionChanged(CustomRegionToolForm sender, EventArgs e, object value);
        #endregion

        /// <summary>
        /// Флаг, указывающий элиптическую форму CustomRegion'a
        /// </summary>
        public virtual bool IsElliptical
        {
            get => toolForm.IsElliptical;
            set => toolForm.IsElliptical = value;
        }

        /// <summary>
        /// Координата <see cref="X"/> верхнего левого угла CustomRegion'a в игровых координата
        /// </summary>
        public virtual int X => (int)leftX;

        /// <summary>
        /// Координата <see cref="Y"/> верхнего левого угла CustomRegion'a в игровых координата
        /// </summary>
        public virtual int Y => (int)topY;

        /// <summary>
        /// Ширина CustomRegion'a в игровых координата
        /// </summary>
        public virtual int Widths =>
            !(rightX == 0 && bottomY == 0)
                ? (int)Math.Abs(rightX - leftX)
                : 0;

        /// <summary>
        /// Высота CustomRegion'a в игровых координата
        /// </summary>
        public virtual int Height =>
            !(rightX == 0 && bottomY == 0)
                ? (int)Math.Abs(topY - bottomY)
                : 0;

        /// <summary>
        /// Режим редактирования
        /// </summary>
        public abstract MapperEditMode EditMode { get; }

        /// <summary>
        /// Введенные данные корректны
        /// </summary>
        public virtual bool IsReady => !(leftX == 0 && topY == 0) 
                                         && !(rightX == 0 && bottomY == 0) 
                                         && Math.Abs(leftX - rightX) >= 1 
                                         && Math.Abs(topY - bottomY) >= 1
                                         && !string.IsNullOrEmpty(CustomRegionName);

        #region IMapperTool
        /// <summary>
        /// Использование механизма выделения вершин
        /// </summary>
        public virtual bool AllowNodeSelection => false;


        public virtual bool HandleMouseClick => true;
        /// <summary>
        /// Обработка нажатия кнопки мыши
        /// </summary>
        public abstract void OnMouseClick(IGraph graph, NodeSelectTool nodes, MapperMouseEventArgs e, out IMapperTool undo);


        public virtual bool HandleKeyUp => true;
        /// <summary>
        /// Обратка нажатия кнопки клавиатуры
        /// </summary>
        public abstract void OnKeyUp(IGraph graph, NodeSelectTool nodes, KeyEventArgs e, double worldMouseX, double worldMouseY, out IMapperTool undo);


        public virtual bool HandleCustomDraw => !(leftX == 0 && topY == 0) && !(rightX == 0 && bottomY == 0);
        /// <summary>
        /// Отрисовка формируемого(изменяемого) CustomRegion'a
        /// </summary>
        public virtual void OnCustomDraw(MapperGraphics graphics, NodeSelectTool nodes, double worldMouseX, double worldMouseY)
        {
            if (leftX == 0 && topY == 0) return;
            if (rightX != 0 || bottomY != 0)
            {
                if (transformMode == RegionTransformMode.None || transformMode == RegionTransformMode.Disabled)
                {
                    // Отрисовываем регион в зафиксированном состоянии
                    graphics.DrawCustomRegionEditable(leftX, topY, rightX, bottomY, IsElliptical);
                }
                else
                {
                    // отрисовываем регион в режиме трансформации
                    double left = leftX,
                        right = rightX,
                        top = topY,
                        down = bottomY;
                    MapperHelper.TransformRegion(ref left, ref top, ref right, ref down, Math.Round(worldMouseX), Math.Round(worldMouseY), transformMode);
                    graphics.DrawCustomRegionEditable(left, top, right, down, IsElliptical);
                }
            }
            else
            {
                // Отрисовываем прямоугольник будущего региона
                MapperHelper.FixRange(leftX, Math.Round(worldMouseX), out double left, out double right);
                MapperHelper.FixRange(topY, Math.Round(worldMouseY), out double down, out double top);
                graphics.DrawCustomRegionEditable(left, top, right, down, IsElliptical);
            }
        }


        /// <summary>
        /// Параметры специального курсора мыши
        /// </summary>
        /// <returns>False, т.е. специальный курсор по умолчанию не используется</returns>
        public virtual bool CustomMouseCursor(double worldMouseX, double worldMouseY, out string text, out Alignment textAlignment, out Font font, out Brush brush)
        {
            text = string.Empty;
            textAlignment = Alignment.None;
            font = Control.DefaultFont;
            brush = Brushes.White;

            return false;
        }


        public abstract bool Applied { get; }

        /// <summary>
        /// Откат изменений, внесенных инструментом
        /// </summary>
        public abstract void Undo(); 
        #endregion

        /// <summary>
        /// Сброс всех изменений и закрытие инструмента
        /// </summary>
        public virtual void Close()
        {
            customRegion = null;
            leftX = 0;
            topY = 0;
            rightX = 0;
            bottomY = 0;
            toolForm.Mode = CustomRegionToolForm.ViewMode.Undefined;
            toolForm.Hide();
        }
    }
}
