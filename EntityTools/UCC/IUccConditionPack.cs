using Astral.Logic.UCC.Classes;
using EntityTools.Enums;
using ConditionList = System.Collections.Generic.List<Astral.Logic.UCC.Classes.UCCCondition>;

namespace EntityTools.UCC.Actions
{
    public interface IUccConditionPack
    {
        /// <summary>
        /// Отрицание результатов проверки
        /// </summary>
        bool Not { get; set; }

        /// <summary>
        /// Правил проверки списка условий(И / ИЛИ)
        /// <see cref="Enums.LogicRule.Conjunction"/> - Логическое И
        /// <see cref="Enums.LogicRule.Disjunction"/> - Логическое ИЛИ
        /// </summary>
        LogicRule LogicRule { get; set; }

        /// <summary>
        /// Список условий
        /// </summary>
        ConditionList Conditions { get; set; }

        /// <summary>
        /// Проверка условий
        /// </summary>
        bool IsOK(UCCAction refAction);
    }
}
