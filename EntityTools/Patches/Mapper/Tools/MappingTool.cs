using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AStar;

namespace EntityTools.Patches.Mapper.Tools
{
    public class MappingTool
    {
        public MappingTool(Func<IGraph> getGraph, MappingMode mode = Mapper.MappingMode.Stoped)
        {
            this.getGraph = getGraph;
            _mappingCache = new MapperGraphCache(getGraph);
            MappingMode = mode;
        }

        #region данные
        /// <summary>
        /// Кэш вершин графа путей
        /// </summary>
        private readonly MapperGraphCache _mappingCache;

        private Func<IGraph> getGraph;

        /// <summary>
        /// Тип пути
        /// </summary>
        public MappingMode MappingMode
        {
            get => _mode;
            set => _mode = value;
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

        }

    }
}
