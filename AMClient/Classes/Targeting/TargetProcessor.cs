using System;
using MyNW.Classes;

namespace AcTp0Tools.Classes.Targeting
{
    /// <summary>
    /// Интерфейс внешнего обработчика для <seealso cref="TargetSelector"/> 
    /// </summary>
    public abstract class TargetProcessor : IDisposable
    {

        /// <summary>
        /// Проверка <param name="target"/> на соответствие параметрам ассоциированного <seealso cref="TargetSelector"/> 
        /// </summary>
        public abstract bool IsMatch(Entity target);

        /// <summary>
        /// Проверка <param name="target"/> на соответствие параметрам ассоциированного <seealso cref="TargetSelector"/>
        /// и возможности/необходимости его замены на соответствующую <seealso cref="Entity"/>
        /// </summary>
        public abstract bool IsTargetMismatchedAndCanBeChanged(Entity target);

        /// <summary>
        /// Получить цель типа <seealso cref="Entity"/>, соответствующую параметрам ассоциированного <seealso cref="TargetSelector"/> 
        /// </summary>
        /// <returns></returns>
        public abstract Entity GetTarget();

        /// <summary>
        /// Сброс внутреннего состояния
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Дополнительная проверка <seealso cref="Entity"/>, например, расстояние до игрока
        /// </summary>
        public abstract Predicate<Entity> SpecialCheck { get; set; }

        /// <summary>
        /// Строковое представление ассоциированного <seealso cref="TargetSelector"/> 
        /// </summary>
        public abstract string Label();

        public abstract void Dispose();
    }
}
