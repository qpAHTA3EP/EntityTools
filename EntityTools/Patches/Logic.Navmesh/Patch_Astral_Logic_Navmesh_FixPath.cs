using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AStar;
using DevExpress.XtraNavBar.ViewInfo;
using EntityTools.Reflection;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityTools.Patches.Navmesh
{
    internal class Patch_Astral_Logic_Navmesh_FixPath : Patch
    {
        internal Patch_Astral_Logic_Navmesh_FixPath()
        {
            MethodInfo mi = typeof(Astral.Logic.Navmesh).GetMethod("fixPath", ReflectionHelper.DefaultFlags);
            if (mi != null)
            {
                methodToReplace = mi;
            }
            else throw new Exception("Patch_Astral_Logic_Navmesh_FixPath: fail to initialize 'methodToReplace'");

            methodToInject = GetType().GetMethod(nameof(FixPath), ReflectionHelper.DefaultFlags);
        }


#if false
    Astral.Logic.Navmesh
private static void FixPath(List<Vector3> waypoints, Vector3 from)
{
	int num = 0;
	while (waypoints.Count > 1)
	{
		num++;
		if (num > 3)
		{
			return;
		}
		double num2 = waypoints[0].Distance3D(from) + waypoints[0].Distance3D(waypoints[1]);
		if (waypoints[1].Distance3D(from) * 1.1 <= num2)
		{
			waypoints.RemoveAt(0);
		}
	}
}
#endif
        private static void FixPath(List<Vector3> waypoints, Vector3 from)
        {
#if false   // Корректировка пути реализована в Patch_Astral_Logic_Navmesh_GetPath.GetPathAndCorrect()

            // cos(165) = -0.9659258262890682867497431997289
            // cos(150) = -0.86602540378443864676372317075294
            // cos(135) = -0.70710678118654752440084436210485
            // cos(105) = -0.25881904510252076234889883762405
            // cos(75) = 0.25881904510252076234889883762405
            // cos(15) = 0.9659258262890682867497431997289

            // вычисляем Cos угла между векторами из формулы скалярного произведения векторов
            // a * b = |a| * |b| * cos (alpha) = xa * xb + ya * yb


            if (waypoints?.Count > 1)
            {
                var wp0 = waypoints[0];
                var wp1 = waypoints[1];

                double x0 = wp0.X - from.X,
                    y0 = wp0.Y - from.Y,
                    z0 = wp0.Z - from.Z,
                    x1 = wp1.X - from.X,
                    y1 = wp1.Y - from.Y,
                    z1 = wp1.Z - from.Z;



                double cos = (x0 * x1 + y0 * y1 + z0 * z1)
                             / Math.Sqrt((x0 * x0 + y0 * y0 + z0 * z0)
                                         * (x1 * x1 + y1 * y1 + z1 * z1));

                if (cos < 0)
                {
                    // угол между направлениями из точки from на точки wp0 и wp1
                    // больше 90 градусов, поэтому точку wp0 нужно "удалить" (чтобы не возвращаться "назад")
                    waypoints.RemoveAt(0);
                }
            } 
#endif
        }
    }
}
