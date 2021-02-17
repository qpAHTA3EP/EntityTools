using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using AStar;
using Astral.Logic.Classes.FSM;
using Astral.Logic.NW;
using EntityTools.Patches.Mapper;
using EntityTools.Reflection;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityTools.Patches.Navmesh
{
    internal class Patch_Quester_Controllers_Road_PathDistance : Patch
    {
        internal Patch_Quester_Controllers_Road_PathDistance()
        {
            if (NeedInjecttion)
            {
                MethodInfo mi = typeof(Astral.Quester.Controllers.Road).GetMethod("PathDistance", ReflectionHelper.DefaultFlags);
                if (mi != null)
                {
                    methodToReplace = mi;
                }
                else throw new Exception("Patch_Quester_Controllers_Road_PathDistance: fail to initialize 'methodToReplace'");

                methodToInject = GetType().GetMethod(nameof(PathDistance), ReflectionHelper.DefaultFlags); 
            }
        }

        public override bool NeedInjecttion => EntityTools.Config.Patches.Navigation;

#if false
        class Astral.Quester.Controllers.Road
		internal static double PathDistance(Vector3 pos)
		{
			Road road = Road.GenerateRoadFromPlayer(pos);
			road.Waypoints.Insert(0, \u0001.LocalPlayer.Location);
			return Navmesh.TotalDistance(road.Waypoints);
		}
#endif
        internal static double PathDistance(Vector3 pos)
        {
            if (pos != null && pos.IsValid)
            {
#if true
                var playerLocation = EntityManager.LocalPlayer.Location;
                var graph = AstralAccessors.Controllers.Roles.CurrentRole.UsedMeshes;

                if (graph is null)
                    return pos.Distance3D(playerLocation);

                var road = Patch_Astral_Logic_Navmesh_GenerateRoad.GenerateRoad(graph,
                    playerLocation, pos, !Astral.API.CurrentSettings.PFOnlyForApproaches);

                double distance = 0;
                if (road.Waypoints.Count > 1)
                {
                    distance = MathHelper.Distance3D(playerLocation, road.Waypoints[0]);
                    distance += Patch_Astral_Logic_Navmesh_TotalDistance.TotalDistance(road.Waypoints);
                }
                else distance = pos.Distance3DFromPlayer;

                return distance;
#endif
            }

            return double.MaxValue;
        }
    }
}
