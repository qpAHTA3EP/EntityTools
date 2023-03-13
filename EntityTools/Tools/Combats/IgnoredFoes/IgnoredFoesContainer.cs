using System.Collections.ObjectModel;

namespace EntityTools.Tools.Combats.IgnoredFoes
{
    public class IgnoredFoesContainer : KeyedCollection<string, IgnoredFoesEntry>
    {
        protected override string GetKeyForItem(IgnoredFoesEntry item)
        {
            return item.Profile;
        }
    }
}
