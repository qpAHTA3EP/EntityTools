using System;
using System.Collections.Generic;
using System.Reflection;
using AcTp0Tools.Reflection;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityTools.Patches.Navmesh
{
    internal class Patch_Astral_Logic_Navmesh_TotalDistance : Patch
    {
        internal Patch_Astral_Logic_Navmesh_TotalDistance()
        {
            if (NeedInjection)
            {
                MethodInfo mi = typeof(Astral.Logic.Navmesh).GetMethod("TotalDistance", ReflectionHelper.DefaultFlags);
                if (mi != null)
                {
                    methodToReplace = mi;
                }
                else throw new Exception("Patch_Astral_Logic_Navmesh_TotalDistance: fail to initialize 'methodToReplace'");

                methodToInject = GetType().GetMethod(nameof(TotalDistance), ReflectionHelper.DefaultFlags);
            }
        }

        public sealed override bool NeedInjection => EntityTools.Config.Patches.Navigation;

#if false
public static double Astral.Logic.Navmesh.TotalDistance(List<Vector3> positions)
{
	if (positions.Count <= 1)
	{
		return 0.0;
	}
	double num = positions[0].Distance3D(positions[1]);
	for (int i = 1; i < positions.Count; i++)
	{
		num += positions[i - 1].Distance3D(positions[i]);
	}
	return num;
}
#endif
        internal static double TotalDistance(List<Vector3> positions)
        {
            double distance = 0;
            if (positions?.Count > 1)
            {
                using (var posEnumerator = positions.GetEnumerator())
                {
                    if (posEnumerator.MoveNext())
                    {
                        var pos1 = posEnumerator.Current;
                        while (posEnumerator.MoveNext())
                        {
                            var pos2 = posEnumerator.Current;
                            distance += MathHelper.Distance3D(pos1, pos2);
                            pos1 = pos2;
                        }
                    }

                }
            }

            return distance;
        }
    }
}
