using System;
using EntityTools.Enums;
using MyNW;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityTools.Servises.SlideMonitor
{
    public class MountCostumeDef : NativeObject
    {
        public MountCostumeDef(IntPtr pointer) : base(pointer) { }

        public string InternalName
        {
            get
            {
                return Memory.MMemory.ReadString(Memory.MMemory.Read<IntPtr>(Pointer));
            }
        }

        public string Category
        {
            get
            {
                return Memory.MMemory.ReadString(Memory.MMemory.Read<IntPtr>(Memory.MMemory.Read<IntPtr>(Pointer + 24)));
            }
        }

        public MountType Type
        {
            get
            {
                if (!IsValid)
                {
                    if (EntityManager.LocalPlayer.CostumeRef.CostumeName == "Infernal_Machine_Car_Becritter_01")
                    {
                        return EntityTools.Servises.SlideMonitor.MountCostumeDef.MountType.InfernalCar;
                    }
                    return EntityTools.Servises.SlideMonitor.MountCostumeDef.MountType.None;
                }
                else
                {
                    if (Category != "Nw_Boat_Mount")
                    {
                        return EntityTools.Servises.SlideMonitor.MountCostumeDef.MountType.Mount;
                    }
                    string internalName = InternalName;
                    if (internalName == "M_Boat_Test_03")
                    {
                        return EntityTools.Servises.SlideMonitor.MountCostumeDef.MountType.BoatPurple;
                    }
                    if (internalName == "M_Boat_Test_02")
                    {
                        return EntityTools.Servises.SlideMonitor.MountCostumeDef.MountType.BoatGreen;
                    }
                    if (!(internalName == "M_Boat_Test_01"))
                    {
                        return EntityTools.Servises.SlideMonitor.MountCostumeDef.MountType.None;
                    }
                    return EntityTools.Servises.SlideMonitor.MountCostumeDef.MountType.BoatWhite;
                }
            }
        }
    }
}
