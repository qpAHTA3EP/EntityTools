using MyNW.Classes;

namespace EntityTools.Enums
{
    /// <summary>
    /// Список 
    /// </summary>
    public enum UccTarget
    {
        Target,
        Player,
        MostInjuredAlly,
        StrongestAdd,
        StrongestTeamMember,
        //Entity,
        Auto
    }

    /// <summary>
    /// Тип (источник) дополнительного умения
    /// </summary>
    public enum PluggedSkillSource
    {
        /// <summary>
        /// Артефактное умение
        /// </summary>
        Artifact,
        /// <summary>
        /// Умение скакуна
        /// </summary>
        Mount
    }

    /// <summary>
    /// Тип проверки состояния умения <see cref="Power"/> 
    /// </summary>
    public enum PowerState
    {
        /// <summary>
        /// Персонаж обладает умением <see cref="Power"/>
        /// </summary>
        HasPower,
        /// <summary>
        /// Персонаж не обладает умением <see cref="Power"/>
        /// </summary>
        HasntPower,
        /// <summary>
        /// Умение временно не может быть использовано (кулдаун)
        /// </summary>
        //OnCooldown
    }
}