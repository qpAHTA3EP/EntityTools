using MyNW.Classes;
using System;

namespace AcTp0Tools.Classes.UCC
{
    /// <summary>
    /// Интерфейс внешнего обработчика для <seealso cref="UccTargetSelector"/> 
    /// </summary>
    public abstract class UccTargetProcessor : IDisposable
    {
#if false
        /// <summary>
        /// установка связи с обрабатываемым <param name="selector"/>
        /// </summary>
        void BindTo(UccTargetSelector selector); 
#endif

        /// <summary>
        /// Проверка <param name="target"/> на соответствие параметрам ассоциированного <seealso cref="UccTargetSelector"/> 
        /// </summary>
        public abstract bool IsMatch(Entity target);

        /// <summary>
        /// Проверка <param name="target"/> на соответствие параметрам ассоциированного <seealso cref="UccTargetSelector"/>
        /// и возможности/необходимости его замены на соответствующую <seealso cref="Entity"/>
        /// </summary>
        public abstract bool IsTargetMismatchedAndCanBeChanged(Entity target);

        /// <summary>
        /// Получить цель типа <seealso cref="Entity"/>, соответствующую параметрам ассоциированного <seealso cref="UccTargetSelector"/> 
        /// </summary>
        /// <returns></returns>
        public abstract Entity GetTarget();

        /// <summary>
        /// Сброс внутреннего состояния
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Строковое представление ассоциированного <seealso cref="UccTargetSelector"/> 
        /// </summary>
        public abstract string Label();

        public abstract void Dispose();
    }
}
