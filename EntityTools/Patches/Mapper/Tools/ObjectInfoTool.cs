using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AStar;
using DevExpress.XtraEditors;

namespace EntityTools.Patches.Mapper.Tools
{
    /// <summary>
    /// Инструмент вывода информации о выбранном объекте
    /// </summary>
    public class ObjectInfoTool : IMapperTool
    {
        private object selectedObject;
        private double x, y;
        private StringBuilder infoBuilder = new StringBuilder();
        ObjectInfoForm infoForm = new ObjectInfoForm();

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
        public bool CustomMouseCusor(double worldMouseX, double worldMouseY, out string text, out Alignment textAlignment, out Font font, out Brush brush)
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

#if false
            if (selectedObject is Node nd)
            {
                x = nd.X;
                y = nd.Y;
                infoBuilder.Clear();
                infoBuilder.Append("Node at position ").AppendLine(nd.ToString());
                infoBuilder.AppendLine(nd.Passable ? "Passable; " : "Unpassable");
                infoBuilder.AppendFormat("{0} {1:N0}:\n\r", nameof(nd.IncomingArcs), nd.IncomingArcsCount);
                foreach (var arc in nd.IncomingArcs)
                    infoBuilder.AppendFormat("\t{0}\n\r", arc);
                infoBuilder.AppendFormat("{0} {1:N0}:\n\r", nameof(nd.OutgoingArcs), nd.OutgoingArcsCount);
                foreach (var arc in nd.OutgoingArcs)
                    infoBuilder.AppendFormat("\t{0}\n\r", arc); 
                XtraMessageBox.Show(infoBuilder.ToString());
            }
            else
            {
                x = 0;
                y = 0;
            }
#else

            ShowObject(selectedObject);
#endif
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
            if(infoForm == null || infoForm.IsDisposed)
                infoForm = new ObjectInfoForm();
            infoForm.Visible = true;
            infoForm.Show(obj);
        }

        private void Hide()
        {
            if (infoForm != null && !infoForm.IsDisposed)
                infoForm.Visible = false;
        }
    }
}