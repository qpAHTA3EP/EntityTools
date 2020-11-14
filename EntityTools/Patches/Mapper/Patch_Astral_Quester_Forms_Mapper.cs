using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AStar;
using EntityTools.Patches.Mapper;
using EntityTools.Reflection;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityTools.Patches.Mapper
{
    internal class Patch_Astral_Quester_Forms_Mapper : Patch
    {
        private static MapperFormExt @this;

        internal Patch_Astral_Quester_Forms_Mapper()
        {
            if (NeedInjecttion)
            {
                MethodInfo mi = typeof(Astral.Quester.Forms.MapperForm).GetMethod("Open", ReflectionHelper.DefaultFlags);
                if (mi != null)
                {
                    methodToReplace = mi;
                }
                else throw new Exception("Patch_Astral_Quester_Forms_Mapper: fail to initialize 'methodToReplace'");

                methodToInject = GetType().GetMethod(nameof(OpenMapper), ReflectionHelper.DefaultFlags);
            }
        }

        public override bool NeedInjecttion => EntityTools.Config.Patches.Navigation;

#if false
    Astral.Logic.Navmesh
public static double TotalDistance(List<Vector3> positions)
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
        internal static void OpenMapper()
        {
            if (EntityTools.Config.Mapper.Patch)
            {
                if (@this != null && !@this.IsDisposed)
                    @this.Show();//Focus();
                else
                {
                    @this = new MapperFormExt();
                    @this.Show();
                }
            }
            else Astral.Quester.Forms.MapperForm.Open();
        }

        internal static void CloseMapper()
        {
            if (@this != null && !@this.IsDisposed)
                @this.Close();
        }
    }
}
