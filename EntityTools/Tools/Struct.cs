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
        public Pair(A a, B b)
        {
            First = a;
            Second = b;
        }

        public A First;
        public B Second;
    }

    public struct ReadonlyPair<A, B>
    {
        public ReadonlyPair(A a, B b)
        {
            First = a;
            Second = b;
        }

        public ReadonlyPair(ReadonlyPair<A, B> p)
        {
            First = p.First;
            Second = p.Second;
        }

        public ReadonlyPair(Pair<A, B> p)
        {
            First = p.First;
            Second = p.Second;
        }

        public readonly A First;
        public readonly B Second;
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
