using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityCore.Tools
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
    }
}
