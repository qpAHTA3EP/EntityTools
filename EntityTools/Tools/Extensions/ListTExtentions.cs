using EntityTools.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace EntityTools.Extensions
{
    public static class ListTExtentions
    {
        public static List<T> Clone<T>(this List<T> @this)
        {
            if(@this != null)
            {
                List<T> listCopy = new List<T>(@this.Count);
                foreach(T t in @this)
                {
                    listCopy.Add(CopyHelper.CreateDeepCopy(t));
                }

                return listCopy;
            }
            return null;
        }

        /// <summary>
        /// Проверка содержет ли список хотя бы один list элемент из elements
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static bool ContainsAny<T>(this IEnumerable<T> list, IEnumerable<T> elements)
        {
            var elem = list.First((t) => elements.Contains(t));
            if (elem != null)
                return true;
            return false;
        }
    }
}
