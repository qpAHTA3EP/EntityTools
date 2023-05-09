namespace EntityTools.Quester.Editor
{
    public interface IEditAct
    {
        /// <summary>
        /// Подготовка команды
        /// </summary>
        /// <param name="editorForm"></param>
        bool Prepare(QuesterEditor editorForm);

        /// <summary>
        /// Применениие команды редактирования
        /// </summary>
        /// <param name="editorForm"></param>
        void Apply(QuesterEditor editorForm);

        /// <summary>
        /// Отмены команды редактирования
        /// </summary>
        /// <param name="editorForm"></param>
        void Undo(QuesterEditor editorForm);

        /// <summary>
        /// Признак готовности команды к применению
        /// </summary>
        bool IsReady { get; }

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
