using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityTools.Enums
{
    public enum BuyEntryType
    {
        /// <summary>
        /// Предмет задан идентификатором
        /// </summary>
        Identifier,
        /// <summary>
        /// Предмет задан категорией
        /// </summary>
        Category
    }

    public enum VendorType
    {
        None,
        /// <summary>
        /// Автоматический выбор вендора
        /// </summary>
        Auto,
        /// <summary>
        /// Конкретный торговец на карте
        /// </summary>
        Normal,
        /// <summary>
        /// Призываемый торговец (актефактом Каталог Аврора для всех миров)
        /// </summary>
        ArtifactVendor,
        /// <summary>
        /// Игровой магазин кампаний
        /// </summary>
        RemouteVendor,
        /// <summary>
        /// ВИП-магазин печатей
        /// </summary>
        VIPSummonSealTrader,
        /// <summary>
        /// ВИП-магазин магазина профессий
        /// </summary>
        VIPProfessionVendor
    }

    /// <summary>
    /// Результаты торговли
    /// </summary>
    public enum BuyItemResult
    {
        /// <summary>
        /// Покупка успешна
        /// </summary>
        Succeeded,
        /// <summary>
        /// Операция завершена без ошибок, покупок не совершено
        /// </summary>
        Completed,
        /// <summary>
        /// Предмет найден, но валюты для покупки недостаточно
        /// </summary>
        NotEnoughCurrency,
        /// <summary>
        /// Полная сумка, поэтому покупка не состоялась
        /// </summary>
        FullBag,
        /// <summary>
        /// Предмет не подлежит покупке
        /// </summary>
        Skiped,
        /// <summary>
        /// Ошибка во время сделки
        /// </summary>
        Error
    }
}
