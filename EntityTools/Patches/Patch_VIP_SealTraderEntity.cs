using System;
using System.Reflection;
using Astral.Logic.NW;
using EntityTools.Reflection;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityTools.Patches
{
    internal class Patch_VIP_SealTraderEntity : Patch
    {
        private readonly Func<string, Entity> VIP_GetNearestEntityByCostume = typeof(VIP).GetStaticFunction<string, Entity>("GetNearestEntityByCostume");

        internal Patch_VIP_SealTraderEntity()
        {
            if (NeedInjecttion)
            {
                PropertyInfo pi = typeof(VIP).GetProperty("SealTraderEntity", ReflectionHelper.DefaultFlags);
                if (pi != null)
                {
                    MethodInfo getter = pi.GetGetMethod((ReflectionHelper.DefaultFlags & BindingFlags.NonPublic) == BindingFlags.NonPublic);
                    if (getter != null)
                    {
                        methodToReplace = getter;
                    }
                }

                if (methodToReplace == null)
                    throw new Exception("Patch_VIP_SealTraderEntity: fail to initialize 'methodToReplace'");

                methodToInject = GetType().GetMethod(nameof(get_SealTraderEntity), ReflectionHelper.DefaultFlags);
            }
        }

        public sealed override bool NeedInjecttion => EntityTools.Config.Patches.VipSealTraderEntity;

        internal static Entity get_SealTraderEntity()
        {
            uint refId = EntityManager.LocalPlayer.RefId;
            Entity entity = null;
            double dist = double.MaxValue;
            foreach (var e in EntityManager.GetEntities())
            {
                double ettDist = e.Location.Distance3DFromPlayer;
                if(e.OwnerRefId == refId && e.InternalName == "Vip_Seal_Trader" && ettDist < dist)
                {
                    entity = e;
                    dist = ettDist;
                }
            }
            return entity;
        }
    }
}
