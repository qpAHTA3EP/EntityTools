using DevExpress.XtraEditors.Controls;

namespace EntityTools.Extensions
{
    public static class ListBoxItemCollectionExtension
    {
        public static bool Contains<T>(this ListBoxItemCollection items, T item)
        {
            foreach (var i in items)
            {
                if (item.Equals(i))
                    return true;
            }
            return false;
        }
    }
}
