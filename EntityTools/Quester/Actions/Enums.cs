namespace EntityTools.Enums
{
    public enum ContactHaveMissionCheckType
    {
        /// <summary>
        /// Не выполнять проверку наличия миссий
        /// </summary>
        Disabled,
        /// <summary>
        /// Проверять наличие повторяемых миссий
        /// </summary>
        RepeatablesOnly,
        /// <summary>
        /// Проверять наличие любых миссий
        /// </summary>
        Any
    }
}