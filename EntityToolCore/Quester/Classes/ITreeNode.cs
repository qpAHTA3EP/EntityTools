using QuesterAction = Astral.Quester.Classes.Action;
using QuesterCondition = Astral.Quester.Classes.Condition;

namespace EntityCore.Quester.Classes
{
    /// <summary>
    /// Вспомогательный интерфейс для отображения элементов quester-профиля
    /// </summary>
    /// <typeparam name="T">Тип отображаемых элементом данных</typeparam>
    interface ITreeNode<T>
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
        /// Реконструкция элемента quester-профиля, с которым ассоциирован текущий узел 
        /// </summary>
        /// <returns></returns>
        T ReconstructInternal();
    }
}
