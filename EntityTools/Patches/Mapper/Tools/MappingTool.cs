using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AStar;
using MyNW.Internals;

namespace EntityTools.Patches.Mapper.Tools
{
    public class MappingTool
    {
        public MappingTool(Func<IGraph> getGraph, MappingMode mode = Mapper.MappingMode.Stoped)
        {
            this.getGraph = getGraph;
            _mappingCache = new MapperGraphCache(getGraph, true, true)
            { 
                CacheDistanceX = EntityTools.PluginSettings.Mapper.CacheRadius,
                CacheDistanceY = EntityTools.PluginSettings.Mapper.CacheRadius,
                CacheDistanceZ = EntityTools.PluginSettings.Mapper.MaxElevationDifference
            };
            MappingMode = mode;
        }

        #region данные
        /// <summary>
        /// Task, прокладывающий путь
        /// </summary>
        private Task _mappingTask;
        /// <summary>
        /// Токен отмены MappingTask
        /// </summary>
        private CancellationTokenSource _mappingCanceler = null;

        /// <summary>
        /// Кэш вершин графа путей
        /// </summary>
        private readonly MapperGraphCache _mappingCache;

        private Func<IGraph> getGraph;

        /// <summary>
        /// Последняя добавленная вершина
        /// </summary>
        private NodeDetail _lastNodeDetail;

        /// <summary>
        /// Название карты и региона, на которой активировано прокладывание пути
        /// </summary>
        private string _mapAndRegion_whereMapping = null;

        private int _mapHash = 0;

        /// <summary>
        /// Тип пути
        /// </summary>
        public MappingMode MappingMode
        {
            get => _mode;
            set
            {
                if(value == MappingMode.Stoped)
                    StopMapping();
                else StartMapping();

                _mode = value;
            }
        }

        private MappingMode _mode = MappingMode.Stoped;

        /// <summary>
        /// Линейный путь (без боковых связей)
        /// </summary>
        public bool Linear
        {
            get => _linear;
            set => _linear = value;
        }
        private bool _linear;

        /// <summary>
        /// Принудительное связывание
        /// </summary>
        public bool ForceLink
        {
            get => _forceLink;
            set => _forceLink = value;
        }
        private bool _forceLink;
        #endregion

        /// <summary>
        /// Обработчик события отрисовки Mapper'a
        /// </summary>
        public void OnCustomDraw(MapperGraphics graphics)
        {
#if true
            using(_mappingCache.ReadLock())
                _mappingCache.ForEachNode(nd => graphics.FillCircleCentered(Brushes.Gold, nd, 7));

            if (_lastNodeDetail != null)
            {
                graphics.FillRhombCentered(Brushes.Gold, _lastNodeDetail, MapperHelper.DefaultAnchorSize, MapperHelper.DefaultAnchorSize);
                //graphics.FillSquareCentered(Brushes.Gold, _lastNodeDetail, MapperHelper.DefaultAnchorSize / 1.4142135623730950488016887242097);
                //graphics.FillSquareCentered(Brushes.ForestGreen, lastNodeDetail, MapperHelper.DefaultAnchorSize * 0.66);
            }

            var eqvDist = EntityTools.PluginSettings.Mapper.WaypointEquivalenceDistance;
            if (eqvDist >= 2)
            {
                var location = EntityManager.LocalPlayer.Location;
                graphics.DrawCircleCentered(Pens.Gold, location.X, location.Y, eqvDist * 2, true);
            }

#endif
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

            return false;
        }


        /// <summary>
        /// Запуск потока прокладывания маршрута
        /// </summary>
        private void StartMapping()
        {
            if (_mappingTask != null && !_mappingTask.IsCanceled && !_mappingTask.IsCompleted &&
                !_mappingTask.IsFaulted) return;

            _mapAndRegion_whereMapping = EntityManager.LocalPlayer.MapAndRegion;
            _mappingCanceler = new CancellationTokenSource();
            _mappingTask = Task.Factory.StartNew(() => work_Mapping(_mappingCanceler.Token), _mappingCanceler.Token);
            _mappingTask?.ContinueWith(t => StopMapping());
        }

        /// <summary>
        /// Событие остановки прокладывания маршрута
        /// </summary>
        private void StopMapping()
        {
            _mappingCanceler?.Cancel();
            _lastNodeDetail = null;
        }

        /// <summary>
        /// Прокладывание пути
        /// </summary>
        private void work_Mapping(CancellationToken token)
        {
            try
            {
#if PROFILING && DEBUG
                AddNavigationNodeChached.ResetWatch();
#endif
                if (_mappingCache.NodesCount != 0)
                {
                    if (_linear)
                        _lastNodeDetail = MappingToolHelper.LinkNearest_1(EntityManager.LocalPlayer.Location.Clone(), _mappingCache);
                    else _lastNodeDetail = MappingToolHelper.LinkNearest_8_Side(EntityManager.LocalPlayer.Location.Clone(), _mappingCache);
                }
                while (_mode != MappingMode.Stoped
                       && !token.IsCancellationRequested)
                {
                    // Проверяем расстояние только до предыдущего узла
                    _lastNodeDetail?.Rebase(EntityManager.LocalPlayer.Location);

                    if (_lastNodeDetail == null || _lastNodeDetail.Distance > EntityTools.PluginSettings.Mapper.WaypointDistance)
                    {
                        switch (MappingMode)
                        {
                            case MappingMode.Bidirectional:
                                if (_linear)
                                {
                                    if (_forceLink)
                                        _lastNodeDetail = MappingToolHelper.LinkLast(EntityManager.LocalPlayer.Location.Clone(), _mappingCache, _lastNodeDetail, false) ?? _lastNodeDetail;
                                    else _lastNodeDetail = MappingToolHelper.LinkLast(EntityManager.LocalPlayer.Location.Clone(), _mappingCache, null, false) ?? _lastNodeDetail;
                                }
                                else
                                {
                                    // Строим комплексный (многосвязный путь)
                                    if (_forceLink)
                                        _lastNodeDetail = MappingToolHelper.LinkNearest_8_Side(EntityManager.LocalPlayer.Location.Clone(), _mappingCache, _lastNodeDetail, false) ?? _lastNodeDetail;
                                    else _lastNodeDetail = MappingToolHelper.LinkNearest_8_Side(EntityManager.LocalPlayer.Location.Clone(), _mappingCache, null, false) ?? _lastNodeDetail;
                                }
                                break;
                            case MappingMode.Unidirectional:
                                {
                                    _lastNodeDetail = MappingToolHelper.LinkLast(EntityManager.LocalPlayer.Location.Clone(), _mappingCache, _lastNodeDetail, true) ?? _lastNodeDetail;
                                }
                                break;
                        }
#if LastAddedNode
                        _mappingCache.LastAddedNode = lastNodeDetail?.Node; 
#endif
                    }
                    Thread.Sleep(100);
                }
                if (token.IsCancellationRequested)
                {
                    // Инициировано прерывание 
                    // Связываем текущее местоположение с графом
                    if (_linear)
                        // Проверяется наличие вершины по курсу и связывается с найденной
                        MappingToolHelper.LinkLinear(EntityManager.LocalPlayer.Location.Clone(), _mappingCache, _lastNodeDetail, _mode == MappingMode.Unidirectional);
                    else MappingToolHelper.LinkNearest_8_Side(EntityManager.LocalPlayer.Location.Clone(), _mappingCache, _lastNodeDetail, _mode == MappingMode.Unidirectional);
                    _lastNodeDetail = null;
                }
            }
            catch (Exception ex)
            {
#if PROFILING && DEBUG
                AddNavigationNodeStatic.LogWatch();
#endif
#if LOG && DEBUG
                ETLogger.WriteLine(LogType.Debug, $"MapperExtWithCache:: Graph Nodes: {MappingCache.FullGraph.NodesCount}");
#endif
                ETLogger.WriteLine(LogType.Error, ex.ToString(), true);
            }
        }
    }
}
