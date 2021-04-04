namespace EntityCore.Enums
{
    public enum EntityPreprocessingResult
    {
        /// <summary>
        /// Некорректная сущность, обработка провалена
        /// </summary>
        Failed,
        /// <summary>
        /// Сущность в пределах заданной дистации и "обработана" в соответствии с настройками
        /// </summary>
        Succeeded,
        /// <summary>
        /// Суoнщсть не достигнута (находится за пределами заданой дистанции)
        /// </summary>
        Faraway
    }
}