using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using AStar;
using AStar.Tools;
using EntityTools.Quester.Mapper;

namespace EntityTools.Quester.Mapper.Tools
{
    /// <summary>
    /// Инструмент для выделения вершин
    /// </summary>
    public class NodeSelectTool : IEnumerable<Node>
    {
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
        /// Набор выделенных вершин
        /// </summary>
        private readonly LinkedList<Node> selectedNodes = new LinkedList<Node>();
        private int graphHash;

        // координаты начала области выделения вершин
        private double selectAreaStartX;
        private double selectAreaStartY;

        public Node Last
        {
            get
            {
                if(selectedNodes.Count > 0)
                    return selectedNodes.Last.Value;
                return null;
            }
        }
        public Node First
        {
            get
            {
                if (selectedNodes.Count > 0)
                    return selectedNodes.First.Value;
                return null;
            }
        }

        /// <summary>
        /// Отрисовка выделения на Mapper'e
        /// </summary>
        public void OnCustomDraw(MapperGraphics graphics, double worldMouseX, double worldMouseY)
        {
            if (selectAreaStartX != 0 && selectAreaStartY != 0)
            {
                if (Control.ModifierKeys == Keys.Shift)
                {
                    // Получаем игровые координаты, соответствующие положению курсора мыши
                    graphics.DrawRectangle(Pens.Gainsboro, selectAreaStartX, selectAreaStartY, worldMouseX, worldMouseY);
                }
            }
        }

        /// <summary>
        /// Клик на Mapper'e используемый для выбора вершины и её позиционирования
        /// </summary>
        public void OnMouseClick(IGraph graph, MapperMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            // Получаем игровые координаты, соответствующие координатам клика
            double mouseX = e.X;
            double mouseY = e.Y;

            switch (Control.ModifierKeys)
            {
                // Нажата клавиша Shift - выделяем область
                case Keys.Shift when selectAreaStartX == 0 && selectAreaStartY == 0:
                    // Задаем начальную точку области выделения
                    selectAreaStartX = mouseX;
                    selectAreaStartY = mouseY;
                    break;
                case Keys.Shift:
                {
                    MapperHelper.FixRange(selectAreaStartX, mouseX, out double left, out double right);
                    MapperHelper.FixRange(selectAreaStartY, mouseY, out double down, out double top);

                    // Выделяем все вершины, охваченные областью выделения и добавляем в группу 
                    int hash = graph.GetHashCode();
                    if(graphHash != hash)
                        selectedNodes.Clear();
                    foreach (Node nd in graph.NodesCollection)
                    {
                        if (nd.Passable
                            && left <= nd.X && nd.X <= right
                            && down <= nd.Y && nd.Y <= top)
                            selectedNodes.AddLast(nd);
                    }

                    graphHash = hash;

                    // Сбрасываем
                    selectAreaStartX = 0;
                    selectAreaStartY = 0;
                    break;
                }
                default:
                {
                    double minDistance = EntityTools.Config.Mapper.WaypointEquivalenceDistance;
                    graph.ClosestNodeOxyProjection(mouseX, mouseY, out Node node, out int hash, minDistance);

                    if (node != null)
                    {
                        if (Control.ModifierKeys == Keys.Control && graphHash == hash)
                        {   
                            // добавление и удаление вершины из(в) группу "перемещаемых"
                            if (selectedNodes.Contains(node))
                                // Вершина входит в группу "выделенных" и на неё кликнули повторно
                                // то есть она подлежит удалению из группы
                                selectedNodes.Remove(node);
                            else // Добавляем вершину в группу "выделенных"
                                selectedNodes.AddLast(node);
                        }
                        else
                        {
                            // сбрасываем выделение и выделяем одну вершину
                            selectedNodes.Clear();
                            selectedNodes.AddLast(node);
                            graphHash = hash;
                        }
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Отмена выделения вершины при нажатии клавиши 'Escape'
        /// </summary>
        public void OnKeyUp(IGraph graph, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                // Отключаем выделение регионом
                selectAreaStartX = 0;
                selectAreaStartY = 0;

                // Сбрасываем выделение
                selectedNodes.Clear();
                graphHash = 0;
            }
            else if (e.KeyCode == Keys.Shift)
            {
                selectAreaStartX = 0;
                selectAreaStartY = 0;
            }
        }

        #region Интерфейс коллекции
        #region IEnumerable
        public IEnumerator<Node> GetEnumerator()
        {
            return selectedNodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        public int Count => selectedNodes.Count;

        public bool Remove(Node node)
        {
            return selectedNodes.Remove(node);
        }

        public void Clear()
        {
            selectedNodes.Clear();
            graphHash = 0;
        } 
        #endregion
    }
}
