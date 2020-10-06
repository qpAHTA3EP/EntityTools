namespace EntityTools.Patches.Mapper
{
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
    }
}