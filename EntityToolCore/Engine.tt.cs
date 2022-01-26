using System;
namespace EntityCore
{
    public partial class Engine
    {
        /// <summary>
        /// Правая часть неравенства представляет собой дату, через 87 дней после даты компиляции
        /// 87 дней фактический лимит действия сборки
        /// </summary>
        private static readonly bool isValid = DateTime.Now.Ticks <= 637858263010010697L;
    }
}