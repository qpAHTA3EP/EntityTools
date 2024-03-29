﻿using System.Collections.Generic;
using System.Text;

namespace EntityTools.Tools.Extensions
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder Append<T>(this StringBuilder stringBuilder, IEnumerable<T> collection, string collectionCaption = "", string emptyMessage = "")
        {
            if (collection != null)
            {
                using (var blEnumerator = collection.GetEnumerator())
                {
                    if (blEnumerator.MoveNext())
                    {
                        if (!string.IsNullOrEmpty(collectionCaption))
                            stringBuilder.AppendLine(collectionCaption);
                        stringBuilder.Append('\t').Append(blEnumerator.Current);
                        while (blEnumerator.MoveNext())
                        {
                            stringBuilder.Append(" ,\n\t").Append(blEnumerator.Current);
                        }
                        stringBuilder.AppendLine();
                    }
                    else if (string.IsNullOrEmpty(emptyMessage))
                        stringBuilder.AppendLine(emptyMessage);
                }
            }
            else if (string.IsNullOrEmpty(emptyMessage))
                stringBuilder.AppendLine(emptyMessage);
            return stringBuilder;
        }
    }
}
