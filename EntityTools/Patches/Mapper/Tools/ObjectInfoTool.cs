using AStar;
using EntityTools.Forms;
using System.Drawing;
using System.Windows.Forms;

namespace EntityTools.Patches.Mapper.Tools
{
    /// <summary>
    /// Инструмент вывода информации о выбранном объекте
    /// </summary>
    public class ObjectInfoTool : IMapperTool
    {
        private object selectedObject;
        private double x, y;
        ObjectInfoForm infoForm;

        /// <summary>
        /// Использование механизма выделения вершин
        /// </summary>
        public bool AllowNodeSelection => false;

        /// <summary>
        /// Режим редактирования
        /// </summary>
        public MapperEditMode EditMode => MapperEditMode.Information;

        public bool HandleCustomDraw => true;//selectedObject != null;
        /// <summary>
        /// Отрисовка выделенной вершины
        /// </summary>
        public void OnCustomDraw(MapperGraphics graphics, NodeSelectTool nodes, double worldMouseX, double worldMouseY)
        {
            if (selectedObject != null)
            {
                if(!(x == 0 && y == 0))
                    MapperGraphics.PointHelper.GetXY(selectedObject, out x, out y);

                graphics.FillCircleCentered(Brushes.Red, x, y, 14);
                graphics.FillCircleCentered(Brushes.Orange, x, y, 8);
            }

            graphics.DrawCircleCentered(Pens.Orange, worldMouseX, worldMouseY, EntityTools.Config.Mapper.WaypointEquivalenceDistance * 2, true);
        }

        /// <summary>
        /// Специальный курсор мыши
        /// </summary>
        public bool CustomMouseCursor(double worldMouseX, double worldMouseY, out string text, out Alignment textAlignment, out Font font, out Brush brush)
        {
            text = string.Empty;
            textAlignment = Alignment.None;
            font = Control.DefaultFont;
            brush = Brushes.White;

            return true;
        }


        public bool HandleKeyUp => true;
        public void OnKeyUp(IGraph graph, NodeSelectTool nodes, KeyEventArgs e, double worldMouseX, double worldMouseY, out IMapperTool undo)
        {
            undo = null;
            if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Enter)
            {
                selectedObject = null;
                x = 0;
                y = 0;
                Hide();
            }
        }

        public bool HandleMouseClick => true;
        public void OnMouseClick(IGraph graph, NodeSelectTool nodes, MapperMouseEventArgs e, out IMapperTool undo)
        {
            undo = null;
            if (e.Button != MouseButtons.Right) return;

            var obj = MapperHelper.SelectObjectByPosition(e.X, e.Y, out x, out y, out _,
                EntityTools.Config.Mapper.WaypointEquivalenceDistance, graph);

            if (obj is null) return;

            selectedObject = obj;

            ShowObject(selectedObject);
        }

        /// <summary>
        /// Указывает, что инструмент был применен
        /// </summary>
        public bool Applied => false;

        /// <summary>
        /// Откат изменений, внесенных инструментом
        /// </summary>
        public void Undo() { }

        private void ShowObject(object obj)
        {
            infoForm = ObjectInfoForm.Show(obj);
        }

        private void Hide()
        {
            infoForm.Close();
        }
    }
}