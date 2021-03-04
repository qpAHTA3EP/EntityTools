using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityTools.Tools.Extensions
{
    public static class CollectionTExtentions
    {
#if false
        /// <summary>
        /// Проверка наличия элемента <paramref name="element"/> в коллекции <paramref name="collection"/>
        /// </summary>
        public static bool Contains<T>(this IEnumerable<T> collection, T element)
        {
            foreach (T elem in collection)
                if (Equals(elem, element))
                    return true;
            return false;
        } 
#endif

        /// <summary>
        /// Применение действия <paramref name="action"/> для каждого элемента в коллекции <paramref name="list"/>
        /// </summary>
        public static void ForEach<T>(this LinkedList<T> list, Action<T> action)
        {
            foreach (T elem in list)
                action(elem);
        }

        /// <summary>
        /// Применение действия <paramref name="action"/> для каждого элемента в коллекции <paramref name="collection"/>
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (T elem in collection)
                action(elem);
        }
    }
}
