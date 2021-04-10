using System;
using System.Reflection;
using Astral.Logic.NW;
using AcTp0Tools.Reflection;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityTools.Patches
{
    internal class Patch_VIP_ProfessionVendorEntity : Patch
    {
        private readonly Func<string, Entity> VIP_GetNearestEntityByCostume = typeof(VIP).GetStaticFunction<string, Entity>("GetNearestEntityByCostume");

        internal Patch_VIP_ProfessionVendorEntity()
        {
            if (NeedInjecttion)
            {
                PropertyInfo pi = typeof(VIP).GetProperty("ProfessionVendorEntity", ReflectionHelper.DefaultFlags);
                if (pi != null)
                {
                    MethodInfo getter = pi.GetGetMethod((ReflectionHelper.DefaultFlags & BindingFlags.NonPublic) == BindingFlags.NonPublic);
                    if (getter != null)
                    {
                        methodToReplace = getter;
                    }
                }

                if (methodToReplace == null)
                    throw new Exception("Patch_VIP_ProfessionVendorEntity: fail to initialize 'methodToReplace'");

                methodToInject = GetType().GetMethod(nameof(get_ProfessionVendorEntity), ReflectionHelper.DefaultFlags);
            }
        }

        public sealed override bool NeedInjecttion => EntityTools.Config.Patches.VipProfessionVendorEntity;

        internal static Entity get_ProfessionVendorEntity()
        {
            uint refId = EntityManager.LocalPlayer.RefId;
            Entity entity = null;
            double dist = double.MaxValue;
            foreach (var e in EntityManager.GetEntities())
            {
                double ettDist = e.Location.Distance3DFromPlayer;
                if(e.OwnerRefId == refId && e.InternalName == "Vip_Professions_Vendor" && ettDist < dist)
                {
                    entity = e;
                    dist = ettDist;
                }
            }
            return entity;
        }
    }
}
