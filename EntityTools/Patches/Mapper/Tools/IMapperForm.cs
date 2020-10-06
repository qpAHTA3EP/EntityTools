using AStar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityTools.Patches.Mapper.Tools
{
    public interface IMapperForm
    {
        LinkedList<Node> SelectedNodes { get; }
    }
}
