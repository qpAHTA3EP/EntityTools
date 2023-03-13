using System;
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
        /// Наиболее выносливый (сильный) дамагер (c наибольшим значением максимум ХР).
        /// Урон, и хп персонажа сейчас считаются от ОУП'a с разными коэффициентами,
        /// поэтому можно принять MaxHP за приблизительную оценку DPS
        /// </summary>
        SturdiestDD,
        /// <summary>
        /// Слабейший член группы (c наименьшим значением максимума ХР)
        /// </summary>
        Weakest,
        /// <summary>
        /// Слабейший дамагер (c наименьшим значением максимума ХР)
        /// Урон, и хп персонажа сейчас считаются от ОУП'a с разными коэффициентами,
        /// поэтому можно принять MaxHP за приблизительную оценку DPS
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

    [Flags]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum PlayerParagonType : uint
    {
        /// <summary>
        /// Неизвестный тип
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Следопыт
        /// </summary>
        Ranger = 0b00_00_00_00_00_00_00_00_11,

        /// <summary>
        /// Следопыт (Охотник)
        /// </summary>
        Ranger_Hunter = 0b00_00_00_00_00_00_00_00_01,

        /// <summary>
        /// Следопыт (Хранитель)
        /// </summary>
        Ranger_Warden = 0b00_00_00_00_00_00_00_00_10,

        /// <summary>
        /// Волшебник
        /// </summary>
        Wizard = 0b00_00_00_00_00_00_00_11_00,

        /// <summary>
        /// Волшебник (Чудотворец)
        /// </summary>            
        Wizard_Arcanist = 0b00_00_00_00_00_00_00_01_00,

        /// <summary>
        /// Волшебник (Арканист)
        /// </summary>
        Wizard_Thaumaturge = 0b00_00_00_00_00_00_00_10_00,

        /// <summary>
        /// Клирик      
        /// </summary> 
        Cleric = 0b00_00_00_00_00_00_11_00_00,

        /// <summary>
        /// Клирик (Благочестивец)
        /// </summary>
        Cleric_Devout = 0b00_00_00_00_00_00_01_00_00,

        /// <summary>
        /// Клирик (Судья)
        /// </summary>
        Cleric_Arbiter = 0b00_00_00_00_00_00_10_00_00,

        /// <summary>  
        /// Варвар
        /// </summary>
        Barbarian = 0b00_00_00_00_00_11_00_00_00,

        /// <summary>
        /// Варвар (Страж)
        /// </summary>
        Barbarian_Sentinel = 0b00_00_00_00_00_01_00_00_00,

        /// <summary>
        /// Варвар (Мастер клинка)
        /// </summary>
        Barbarian_BladeMaster = 0b00_00_00_00_00_10_00_00_00,

        /// <summary>
        /// Воин (бывший Страж)
        /// </summary>
        Fighter = 0b00_00_00_00_11_00_00_00_00,

        /// <summary>
        /// Воин (Авангард)
        /// </summary>
        Fighter_Vanguard = 0b00_00_00_00_01_00_00_00_00,

        /// <summary>
        /// Воин (Сорвиголова)
        /// </summary>
        Fighter_Dreadnought = 0b00_00_00_00_10_00_00_00_00,

        /// <summary>
        /// Паладин
        /// </summary>
        Paladin = 0b00_00_00_11_00_00_00_00_00,

        /// <summary>
        /// Паладин (Клятвохранитель)
        /// </summary>
        Paladin_OathKeeper = 0b00_00_00_01_00_00_00_00_00,

        /// <summary>
        /// Паладин (Юстициар)
        /// </summary>
        Paladin_Justicar = 0b00_00_00_10_00_00_00_00_00,

        /// <summary>
        /// Чернокнижник
        /// </summary>
        Warlock = 0b00_00_11_00_00_00_00_00_00,

        /// <summary>
        /// Чернокнижник (Вестник ада)
        /// </summary>
        Warlock_HellBringer = 0b00_00_01_00_00_00_00_00_00,

        /// <summary>
        /// Чернокнижник (Ткач душ)
        /// </summary>
        Warlock_SoulWeaver = 0b00_00_10_00_00_00_00_00_00,

        /// <summary>
        /// Плут
        /// </summary>
        Rogue = 0b00_11_00_00_00_00_00_00_00,

        /// <summary>
        /// Плут (Убийца)
        /// </summary>
        Rogue_Assassin = 0b00_01_00_00_00_00_00_00_00,

        /// <summary>
        /// Плут (Шепчущий нож)
        /// </summary>
        Rogue_WhisperKnife = 0b00_10_00_00_00_00_00_00_00,

        /// <summary>
        /// Бард
        /// </summary>
        Bard = 0b11_00_00_00_00_00_00_00_00,

        /// <summary>
        /// Бард (хил)
        /// </summary>
        Bard_Minstrel = 0b01_00_00_00_00_00_00_00_00,

        /// <summary>
        /// Бард (ДД)
        /// </summary>
        Bard_Swashbuckler = 0b10_00_00_00_00_00_00_00_00
    }
}