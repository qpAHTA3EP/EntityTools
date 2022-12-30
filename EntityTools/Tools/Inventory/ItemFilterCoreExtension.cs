using Infrastructure;
using Astral.Classes.ItemFilter;
using MyNW.Classes;

namespace EntityTools.Tools.Inventory
{
    public static class ItemFilterCoreExtension
    {
        public static bool IsMatch(this ItemFilterCore itemFilter, Item item)
        {
            return AstralAccessors.ItemFilter.IsMatch?.Invoke(itemFilter, item) ?? false;
        }

        public static bool IsMatch(this ItemFilterCore itemFilter, string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
                return false;

            if (itemFilter?.Entries.Count > 0)
            {
                foreach (var filterEntry in itemFilter.Entries)
                {
                    var predicate = filterEntry.Text.GetComparer(filterEntry.StringType);
                    if (filterEntry.Mode == ItemFilterMode.Include)
                    {
                        if (predicate(itemId))
                            return true;
                    }
                    else // filterEntry.Mode == ItemFilterMode.Exclude
                    {
                        if (predicate(itemId))
                            return false;
                    }
                }
            }
            return false;
        }
    }
}
