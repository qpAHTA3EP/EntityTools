using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyNW.Classes;

namespace EntityTools.Tools.ItemFilter
{
    public interface IItemMatch
    {
        bool IsMatch(Item item);
    }
}
