using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityTools.Patches.Logic.Classes.FSM.Navigation
{
    public static class EntityEx
    {
        public static MountCostumeDef GetMountCostume(this Entity entity)
        {
            return new MountCostumeDef((IntPtr)entity.CostumeRef.pMountCostume);
        }
    }
}
