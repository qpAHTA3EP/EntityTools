using EntityTools.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace VariableTools.Classes
{
    [Serializable]
    public class VariableCollection
    {
        internal class InternalVariableCollection : KeyedCollection< Pair<string, string>, VariableContainer>
        {
            protected override Pair<string, string> GetKeyForItem(VariableContainer item)
            {
                return item.Key;
            }
        }

        internal InternalVariableCollection variables = new InternalVariableCollection();


    }
}
