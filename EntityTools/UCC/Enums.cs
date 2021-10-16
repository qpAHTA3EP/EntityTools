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
}