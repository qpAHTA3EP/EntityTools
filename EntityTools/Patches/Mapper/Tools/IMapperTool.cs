using AStar;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EntityTools.Patches.Mapper.Tools
{
    public interface IMapperTool// : IDisposable
    {
        MapperEditMode EditMode { get; }

#if false
        /// <summary>
        /// Привязка инструмента к форме (в том числе к обработчикам событий формы)
        /// </summary>
        /// <param name="form"></param>
        void BindTo(MapperFormExt form);
        /// <summary>
        /// Отвязка от формы (обработчиков событий)
        /// </summary>
        void Unbind(); 
#endif

        /// <summary>
        /// Обработчик события MouseClick
        /// </summary>
        void OnMouseClick(IGraph graph, NodeSelectTool nodes, MapperMouseEventArgs e, out IMapperTool undo);
        bool HandleMouseClick { get; }

        /// <summary>
        /// Обработчик события KeyUp
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="nodes"></param>
        /// <param name="e"></param>
        /// <param name="undo"></param>
        void OnKeyUp(IGraph graph, NodeSelectTool nodes, KeyEventArgs e, double worldMouseX, double worldMouseY, out IMapperTool undo);
        bool HandleKeyUp { get; }

        /// <summary>
        /// Обработчик события отрисовки Mapper'a
        /// </summary>
        void OnCustomDraw(MapperGraphics graphics, NodeSelectTool nodes, double worldMouseX, double worldMouseY);
        bool HandleCustomDraw { get; }

        /// <summary>
        /// Проверка того, что инструмент был использован и его результаты можно "откатить"
        /// </summary>
        bool Applied { get; }

        /// <summary>
        /// Откат результатов применения инструмента
        /// </summary>
        void Undo();
    }
}
