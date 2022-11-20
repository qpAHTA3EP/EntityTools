using MyNW;
using MyNW.Classes;
using System;

namespace EntityTools.Servises.SlideMonitor
{
    public static class EntityEx
    {
        public static MountCostumeDef GetMountCostume(this Entity entity)
        {
            return new MountCostumeDef((IntPtr)entity.CostumeRef.pMountCostume);
        }

        public static string GetMountCostumeInternalName(this CostumeRef costumeRef)
        {
            var pMountCostume = costumeRef.pMountCostume;
            return pMountCostume > 0 
                 ? Memory.MMemory.ReadString(Memory.MMemory.Read<IntPtr>(new IntPtr(pMountCostume))) 
                 : string.Empty;
        }
    }
}
