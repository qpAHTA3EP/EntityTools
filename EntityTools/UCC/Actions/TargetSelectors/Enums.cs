using System.Diagnostics.CodeAnalysis;

namespace EntityTools.Enums
{
    /// <summary>
    /// Тип члена группы, принимаемого под наблюдение
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum Teammates
    {
        /// <summary>
        /// Лидер группы
        /// </summary>
        Leader,
        /// <summary>
        /// Танк
        /// </summary>
        Tank,
        /// <summary>
        /// Целитель
        /// </summary>
        Healer,
        /// <summary>
        /// Наиболее выносливый член группы (c наибольшим значением максимум ХР)
        /// </summary>
        Sturdiest,
        /// <summary>
        /// Наиболее выносливый (сильный) дамагер (c наибольшим значением максимум ХР)
        /// дамаг, и хп персонажа сейчас считаются от ОУП'a с разными коэффициентами,
        /// поэтому можно принять хп за приблизительную оценку дамажности
        /// </summary>
        SturdiestDD,
        /// <summary>
        /// Слабейший член группы (c наименьшим значением максимума ХР)
        /// </summary>
        Weakest,
        /// <summary>
        /// Слабейший дамагер (c наименьшим значением максимума ХР)
        /// </summary>
        WeakestDD,
        /// <summary>
        /// Наиболее израненный член группы (c наименьшим значением ХР)
        /// </summary>
        MostInjured,
        /// <summary>
        /// Наиболее израненный дамагер (c наименьшим значением ХР)
        /// </summary>
        MostInjuredDD
    }

    /// <summary>
    /// Предпочитаемый противник
    /// </summary>
    public enum PreferredFoe
    {
        /// <summary>
        /// Цель заданного члена группы
        /// </summary>
        TeammatesTarget,
        /// <summary>
        /// Ближайший к игроку противник
        /// </summary>
        ClosestToPlayer,
        /// <summary>
        /// Противник, ближайший к поднадзорному члену группы
        /// </summary>
        ClosestToTeammate,
        /// <summary>
        /// Самый выносливый противник (c наибольшим значением максимума ХР)
        /// </summary>
        Sturdiest,
        /// <summary>
        /// Наименее выносливый противник (c наименьшим значением максимума ХР)
        /// </summary>
        Weakest,
        /// <summary>
        /// Наиболее раненый противник (с наименьшим НР)
        /// </summary>
        MostInjured
    }
}