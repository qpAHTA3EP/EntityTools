namespace EntityCore.Enums
{
    public enum MissionInteractResult
    {
        /// <summary>
        /// Ошибка во время попытки взаимодействия с НПС при принятии миссии
        /// </summary>
        Error,
        /// <summary>
        /// НПС начинает диалог с автоматического предложения
        /// </summary>
        MissionOffer,
        /// <summary>
        /// Целвая миссия принята
        /// </summary>
        Succeed,
        /// <summary>
        /// Целевая миссия не найдена
        /// </summary>
        MissionNotFound
    }

    public enum MissionProcessingResult
    {
        /// <summary>
        /// Ошибка во время попытки взаимодействия с НПС при принятии миссии
        /// </summary>
        Error,
        /// <summary>
        /// Целевая миссия не найдена
        /// </summary>
        MissionNotFound,
        /// <summary>
        /// Награда за целевую миссию не содержит обязательного предмета
        /// </summary>
        MissionRequiredRewardMissing,
        /// <summary>
        /// Целвая миссия принята
        /// </summary>
        MissionAccepted,
        /// <summary>
        /// Принята миссия, предложенная НПС
        /// </summary>
        MissionOfferAccepted,
#if false
        /// <summary>
        /// Награда за предложенную миссию не содержит обязательного предмета
        /// </summary>
        OfferMissionRequiredRewardNotFound, 

        /// <summary>
        /// Отказ от принятия предложенной миссии
        /// </summary>
        MissionOfferAborted
#endif
        /// <summary>
        /// Миссия завершена
        /// </summary>
        MissionTurnedIn
    }
}
