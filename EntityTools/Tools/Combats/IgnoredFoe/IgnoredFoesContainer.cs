using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityTools.Tools.Combats.IgnoredFoes
{
    public class IgnoredFoesContainer : KeyedCollection<string, IgnoredFoes>
    {
        protected override string GetKeyForItem(IgnoredFoes item)
        {
            return item.Profile;
        }
    }
}
