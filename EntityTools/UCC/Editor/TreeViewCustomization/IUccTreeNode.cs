namespace EntityTools.UCC.Editor.TreeViewCustomization
{
    /// <summary>
    /// Вспомогательный интерфейс для отображения элементов UCC-профиля
    /// </summary>
    /// <typeparam name="T">Тип отображаемых элементом данных</typeparam>
    interface IUccTreeNode<T>
    {
        //UCCAction Data { get; }
        /// <summary>
        /// Допустимость дочерних узлов
        /// </summary>
        bool AllowChildren { get; }
        /// <summary>
        /// Обновление свойство узла в соответствии с отображаемыми данными
        /// </summary>
        void UpdateView();
        /// <summary>
        /// Реконструкция элемента UCC-профиля, с которым ассоциирован текущий узел 
        /// </summary>
        /// <returns></returns>
        T ReconstructInternal();
    }
}
