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
    public class Pair<A, B>
    {
        public Pair() { }
        public Pair(A a, B b)
        {
            First = a;
            Second = b;
        }

        public Pair<A, B> Clone()
        {
            return new Pair<A, B>()
            {
                First = this.First,
                Second = this.Second
            };
        }

        public A First;
        public B Second;
    }

    public class ReadonlyPair<A, B>
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
    public class Triple<A, B, C>
    {
        public A First;
        public B Second;
        public C Third;

        public Triple() { }
        public Triple(A a, B b, C c)
        {
            First = a;
            Second = b;
            Third = c;
        }
        public Triple<A, B, C> Clone()
        {
            return new Triple<A, B, C>()
            {
                First = this.First,
               Second = this.Second,
                Third = this.Third
            };
        }
    }
}
