namespace EntityTools.Quester.Editor
{
    public interface IQEdit
    {
        /// <summary>
        /// Применениие команды редактирования
        /// </summary>
        void Apply();

        /// <summary>
        /// Отмены команды редактирования
        /// </summary>
        void Undo();

        /// <summary>
        /// Признак применения команды 
        /// </summary>
        bool Applied { get; }

        /// <summary>
        /// Текстовая метка командым редактирования
        /// </summary>
        string Label { get; }

        /// <summary>
        /// Текстовая метка отмены команды редактирования
        /// </summary>
        string UndoLabel { get; }
    }
}
