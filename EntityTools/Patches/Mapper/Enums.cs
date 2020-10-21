﻿namespace EntityTools.Patches.Mapper
{
#if PATCH_ASTRAL
    /// <summary>
    /// Режим редактирования графа
    /// </summary>
    public enum MapperEditMode
    {
        None,
        /// <summary>
        /// Прогладывание путей
        /// </summary>
        Mapping,
        /// <summary>
        /// Ручное перемещение вершин
        /// </summary>
        RelocateNodes,
        /// <summary>
        /// Удаление вершин
        /// </summary>
        DeleteNodes,
        /// <summary>
        /// Ручное изменение ребер
        /// </summary>
        EditEdges,
        /// <summary>
        /// Добавление CustomRegion'a
        /// </summary>
        AddCustomRegion,
        /// <summary>
        /// Редактирование CustomRegion'ов
        /// </summary>
        EditCustomRegion,
        /// <summary>
        /// Измерение расстояниямежду точками
        /// </summary>
        DistanceMeasurement
    }

    public enum MappingMode
    {
        Bidirectional,
        Unidirectional,
        Stoped
    }
    /// <summary>
    /// Выравнивание (точки в прямоугольнике)
    /// </summary>
    public enum Alignment
    {
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottomCenter,
        BottomRight,
    }

    /// <summary>
    /// Режим перетаскивания якорей CustomRegion'a
    /// </summary>
    public enum RegionTransformMode
    {
        None, // перетаскивание якорей разрешено, но не производится
        TopLeft,
        TopCenter,
        TopRight,
        Left,
        Center,
        Right,
        DownLeft,
        DownCenter,
        DownRight,
        Disabled // перетаскивание якорей запрещено
    } 
#endif
}