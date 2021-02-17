using MyNW;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EntityTools.Servises.SlideMonitor.MountCostumeDef;

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
            if (pMountCostume > 0)
                return Memory.MMemory.ReadString(Memory.MMemory.Read<IntPtr>(new IntPtr(pMountCostume)));
            return string.Empty;
        }

#if false
        public static MountType GetMountType(this CostumeRef costumeRef)
        {
            var pMountCostume = costumeRef.pMountCostume;
            if (pMountCostume > 0)
            {
                string internalName = Memory.MMemory.ReadString(Memory.MMemory.Read<IntPtr>(new IntPtr(pMountCostume)));

            }
            return MountType.None;
        } 
#endif
    }
}
