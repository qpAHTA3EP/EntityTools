using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityTools.Tools
{
    /// <summar>
    /// Вспомогательная структура "Пара"
    /// </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    public struct Pair<A, B>
    {
        public A First;
        public B Second;
    }

    /// <summar>
    /// Вспомогательная структура "Пара"
    /// </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    /// <typeparam name="C"></typeparam>
    public struct Triple<A, B, C>
    {
        public A First;
        public B Second;
        public C Third;
    }
}
