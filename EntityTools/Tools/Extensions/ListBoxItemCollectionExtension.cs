using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityCore.Extensions
{
    public static class ListBoxItemCollectionExtension
    {
        public static bool Contains<T>(this ListBoxItemCollection items, T item)
        {
            foreach(var i in items)
            {
                if (item.Equals(i))
                    return true;
            }
            return false;
        }
    }
}
