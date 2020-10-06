using AStar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data.Filtering.Helpers;

namespace EntityTools.Patches.Mapper
{
    public partial class MapperFormExt
    {
        /// <summary>
        /// Инструмент для выделения вершин
        /// </summary>
        public class NodeSelectTool : IDisposable, IEnumerable<Node>
        {
            MapperFormExt mapper;

            /// <summary>
            /// Набор выделенных вершин
            /// </summary>
            private readonly HashSet<Node> selectedNodes = new HashSet<Node>();
            private int graphHash;

            // координаты начала области выделения вершин
            private double selectAreaStartX;
            private double selectAreaStartY;

            public NodeSelectTool(MapperFormExt form)
            {
                BindTo(form);
            }

            public void BindTo(MapperFormExt form)
            {
                if (!ReferenceEquals(mapper, form))
                {
                    Unbind();
                    mapper = form;
                    if (mapper != null)
                    {
                        mapper.OnMapperMouseClick += handler_RightMouseClick;
                        mapper.OnMapperKeyUp += handler_KeyUp;
                        mapper.OnMapperDraw += CustomDraw;
                    }
                }
            }

            public void Unbind()
            {
                selectedNodes.Clear();
                if (mapper != null)
                {
                    mapper.OnMapperMouseClick -= handler_RightMouseClick;
                    mapper.OnMapperKeyUp -= handler_KeyUp;
                    mapper.OnMapperDraw -= CustomDraw;
                    mapper = null;
                }
            }

            public void Dispose()
            {
                Unbind();
            }

            private void CustomDraw(MapperFormExt form, MapperGraphics graphics)
            {
                if (form.CurrentTool != null && selectAreaStartX != 0 && selectAreaStartY != 0)
                {
                    if (Control.ModifierKeys == Keys.Shift)
                    {
                        // Получаем игровые координаты, соответствующие положению курсора мыши
                        graphics.GetWorldPosition(form.RelativeMousePosition, out double worldX, out double worldY);
                        graphics.DrawRectangle(Pens.Gainsboro, selectAreaStartX, selectAreaStartY, worldX, worldY);
                    }
                    else
                    {
                        selectAreaStartX = 0;
                        selectAreaStartY = 0;
                    }
                }

#if false
                if (selectedNodes.Count > 0)
                {
                    foreach (Node node in selectedNodes)
                        graphics.FillCircleCentered(Brushes.Yellow, node);
                } 
#endif
            }

            /// <summary>
            /// Клик на Mapper'e используемый для выбора вершины и её позиционирования
            /// </summary>
            private void handler_RightMouseClick(MapperFormExt form, MapperGraphics graphics, MouseEventArgs e)
            {
                if (form.CurrentTool != null && e.Button == MouseButtons.Right)
                {
                    // Получаем игровые координаты, соответствующие координатам клика
                    double selectAreaEndX, selectAreaEndY;
                    
                    using (graphics.ReadLock())
                        graphics.GetWorldPosition(e.Location, out selectAreaEndX, out selectAreaEndY);

                    if (Control.ModifierKeys == Keys.Shift)
                    {
                        // Нажата клавиша Shift - выделяем область
                        if (selectAreaStartX == 0 && selectAreaStartY == 0)
                        {
                            // Задаем начальную точку области выделения
                            selectAreaStartX = selectAreaEndX;
                            selectAreaStartY = selectAreaEndY;
                        }
                        else
                        { 
                            MapperGraphicsHelper.FixRange(selectAreaStartX, selectAreaEndX, out double left, out double right);
                            MapperGraphicsHelper.FixRange(selectAreaStartY, selectAreaEndY, out double down, out double top);

                            // Выделяем все вершины, охваченные областью выделения и добавляем в группу 
                            using (graphics.ReadLock())
                            {
                                var graph = graphics.VisibleGraph;
                                int hash = graph.GetHashCode();
                                if(graphHash != hash)
                                    selectedNodes.Clear();
                                foreach (Node nd in graph.NodesCollection)
                                {
                                    if (nd.Passable
                                        && left <= nd.X && nd.X <= right
                                        && down <= nd.Y && nd.Y <= top)
                                        selectedNodes.Add(nd);
                                }

                                graphHash = hash;
                            }

                            // Сбрасываем
                            selectAreaStartX = 0;
                            selectAreaStartY = 0;
                        }

                    }
                    else 
                    {
                        double minDistance = EntityTools.PluginSettings.Mapper.WaypointEquivalenceDistance;
                        graphics.VisibleGraph.ClosestNodeOxyProjection(selectAreaEndX, selectAreaEndY, out Node node, out int hash, minDistance);

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
                                    selectedNodes.Add(node);
                            }
                            else
                            {
                                // сбрасываем выделение и выделяем одну вершину
                                selectedNodes.Clear();
                                selectedNodes.Add(node);
                                graphHash = hash;
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// Отмена выделения вершины при нажатии клавиши 'Escape'
            /// </summary>
            public void handler_KeyUp(MapperFormExt form, MapperGraphics graphics, KeyEventArgs e)
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

            #region Интерфейс HashSet
            #region IEnumerable
            public IEnumerator<Node> GetEnumerator()
            {
                int hash = mapper.Graphics.VisibleGraph.GetHashCode();
                if (graphHash != hash)
                {
                    selectedNodes.Clear();
                    return EmptyEnumerable<Node>.Instance;
                }
                return selectedNodes.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
            #endregion

            public int Count => selectedNodes.Count;

            public bool Add(Node node)
            {
                int hash = mapper.Graphics.VisibleGraph.GetHashCode();
                if (graphHash == hash)
                    return selectedNodes.Add(node);
                else
                {
                    selectedNodes.Clear();
                    if (selectedNodes.Add(node))
                    {
                        graphHash = hash;
                        return true;
                    }
                }

                return false;
            }

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
}
