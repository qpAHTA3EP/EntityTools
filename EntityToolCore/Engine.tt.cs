using System;
namespace EntityCore
{
    public partial class Engine
    {
        /// <summary>
        /// Правая часть неравенства представляет собой дату, через 87 дней после даты компиляции
        /// 87 дней фактический лимит действия сборки.
        /// Чтобы шаблон обрабатывался при каждом билде в Prebuild Event нужно добавить директиву:
        /// <code>"$(DevEnvDir)TextTransform.exe" -out "$(ProjectDir)Engine.tt.cs" "$(ProjectDir)Engine.tt"</code>
        /// </summary>
        private static readonly bool isValid = DateTime.Now.Ticks <= 638093246301130058L;
    }
}