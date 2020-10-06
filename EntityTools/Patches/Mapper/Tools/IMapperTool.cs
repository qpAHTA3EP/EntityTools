using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityTools.Patches.Mapper.Tools
{
    public interface IMapperTool : IDisposable
    {
        MapperEditMode EditMode { get; }

        /// <summary>
        /// Привязка инструмента к форме (в том числе к обработчикам событий формы)
        /// </summary>
        /// <param name="form"></param>
        void BindTo(MapperFormExt form);
        /// <summary>
        /// Отвязка от формы (обработчиков событий)
        /// </summary>
        void Unbind();

        /// <summary>
        /// Отрисовка инструмента на <paramref name="form"/> c помощью <paramref name="graphics"/>
        /// </summary>
        void CustomDraw(MapperFormExt form, MapperGraphics graphics);

        /// <summary>
        /// Проверка того, что инструмент был использован и его результаты можно "откатить"
        /// </summary>
        bool Applied { get; }

        bool Apply();
        
        /// <summary>
        /// Откат результатов применения инструмента
        /// </summary>
        void Undo(MapperFormExt mapper);
    }
}
