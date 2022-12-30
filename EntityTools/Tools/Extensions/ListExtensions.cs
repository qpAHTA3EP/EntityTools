using System.Collections.Generic;
using System.Linq;
using Infrastructure.Reflection;

namespace EntityTools.Tools.Extensions
{
    public static class ListExtensions
    {
        // /// <summary>
        // /// Удаление из списка <paramref name="list"/> элементов, заданных списоком индексов <paramref name="removedIndexes"/>.
        // /// Массив индексов <paramref name="removedIndexes"/> должен быть упорядочен по возрастанию.
        // /// </summary>
        // /// <typeparam name="T"></typeparam>
        // /// <param name="list"></param>
        // /// <param name="removedIndexes"></param>
        // /// <returns>Количество удаленных элементов</returns>
        // public static int RemoveAt<T>(this IList list, int[] removedIndexes)
        // {
        //     int len = removedIndexes?.Length ?? 0;
        //     int total = list?.Count ?? 0;
        //     if (len > 0
        //         && total > 0)
        //     {
        //         for (int i = 0; i < len; i++)
        //         {
        //             int ind1 = removedIndexes[i] - i;
        //             int ind2 = ++i < len ? removedIndexes[i] : total;
        //             for (int j = ind1 + 1; j < ind2; j++, ind1++)
        //             {
        //                 list[ind1] = list[j];
        //             }
        //         }
        //         list.
        //         return total - list.Count;
        //     }
        //
        //     return 0;
        // }
        
        
        //public static List<T> Clone<T>(this List<T> @this)
        //{
        //    if (@this != null)
        //    {
        //        List<T> listCopy = new List<T>(@this.Select(item => item.CreateDeepCopy()));
        //        return listCopy;
        //    }
        //    return null;
        //}
    }
}
