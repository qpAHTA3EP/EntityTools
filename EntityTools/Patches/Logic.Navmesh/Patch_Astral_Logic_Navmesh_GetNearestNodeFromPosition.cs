using System;
using System.Reflection;
using AStar;
using AcTp0Tools.Reflection;
using MyNW.Classes;

namespace EntityTools.Patches.Navmesh
{
    internal class Patch_Astral_Logic_Navmesh_GetNearestNodeFromPosition : Patch
    {
        internal Patch_Astral_Logic_Navmesh_GetNearestNodeFromPosition()
        {
            if (NeedInjection)
            {
                MethodInfo mi = typeof(Astral.Logic.Navmesh).GetMethod("GetNearestNodeFromPosition", ReflectionHelper.DefaultFlags);
                if (mi != null)
                {
                    methodToReplace = mi;
                }
                else throw new Exception("Patch_Astral_Logic_Navmesh_GetNearestNodeFromPosition: fail to initialize 'methodToReplace'");

                methodToInject = GetType().GetMethod(nameof(GetNearestNodeFromPosition), ReflectionHelper.DefaultFlags);
            }
        }

        public sealed override bool NeedInjection => EntityTools.Config.Patches.Navigation;

#if false
    Astral.Logic.Navmesh
    public static Node GetNearestNodeFromPosition(Graph graph, Vector3 pos)
    {
	    double num = double.MaxValue;
	    Node result = new Node(0.0, 0.0, 0.0);
	    if (graph == null)
	    {
		    return new Node((double)pos.X, (double)pos.Y, (double)pos.Z);
	    }
	    foreach (object obj in graph.Nodes)
	    {
		    Node node = (Node)obj;
		    if (node.Passable)
		    {
			    Vector3 vector = new Vector3(pos.X, pos.Y, pos.Z);
			    Vector3 from = new Vector3((float)node.X, (float)node.Y, (float)node.Z);
			    double num2 = vector.Distance3D(from);
			    if (num2 < num)
			    {
				    num = num2;
				    result = node;
			    }
		    }
	    }
        return result;
    }
#endif
        internal static Node GetNearestNodeFromPosition(Graph graph, Vector3 pos)
        {
            if (graph?.NodesCount > 0 && pos != null && pos.IsValid)
            {
                var node = graph.ClosestNode(pos.X, pos.Y, pos.Z, out _, false);
                if (node != null)
                    return node;
            }

            return new Node(0,0,0);
        }
    }
}
