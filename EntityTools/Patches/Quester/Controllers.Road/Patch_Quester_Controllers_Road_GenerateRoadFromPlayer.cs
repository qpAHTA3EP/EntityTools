﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using AStar;
using Astral.Logic.Classes.FSM;
using Astral.Logic.NW;
using EntityTools.Patches.Navmesh;
using AcTp0Tools.Reflection;
using MyNW.Classes;
using MyNW.Internals;
using AcTp0Tools;

namespace EntityTools.Patches.Quester.Controllers.Road
{
    internal class Patch_Quester_Controllers_Road_GenerateRoadFromPlayer : Patch
    {
        internal Patch_Quester_Controllers_Road_GenerateRoadFromPlayer()
        {
            if (NeedInjection)
            {
                MethodInfo mi = typeof(Astral.Quester.Controllers.Road).GetMethod("GenerateRoadFromPlayer", ReflectionHelper.DefaultFlags);
                if (mi != null)
                {
                    methodToReplace = mi;
                }
                else throw new Exception("Patch_Quester_Controllers_Road_GenerateRoadFromPlayer: fail to initialize 'methodToReplace'");

                methodToInject = GetType().GetMethod(nameof(GenerateRoadFromPlayer), ReflectionHelper.DefaultFlags);
            }
        }

        public override bool NeedInjection => EntityTools.Config.Patches.Navigation;

#if false
class Astral.Quester.Controllers.Road
		internal static Road GenerateRoadFromPlayer(Vector3 end)
		{
			return Navmesh.GenerateRoadFromPlayer(Core.Meshes, end, !\u0001.CurrentSettings.PFOnlyForApproaches);
		}
#endif
        internal static Astral.Logic.Classes.FSM.Road GenerateRoadFromPlayer(Vector3 end)
        {
            if (end?.IsValid == true)
            {
                var graph = AstralAccessors.Controllers.Roles.CurrentRole.UsedMeshes;
                var playerLocation = EntityManager.LocalPlayer.Location;
                return Patch_Astral_Logic_Navmesh_GenerateRoad.GenerateRoad(graph,
                    playerLocation, end, !Astral.API.CurrentSettings.PFOnlyForApproaches);

            }
            return new Astral.Logic.Classes.FSM.Road();
        }
    }
}
