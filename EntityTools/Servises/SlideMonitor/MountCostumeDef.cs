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

        public string InternalName => Memory.MMemory.ReadString(Memory.MMemory.Read<IntPtr>(Pointer));

        public string Category => Memory.MMemory.ReadString(Memory.MMemory.Read<IntPtr>(Memory.MMemory.Read<IntPtr>(Pointer + 24)));

        public MountType Type
        {
            get
            {
                if (!IsValid)
                {
                    if (EntityManager.LocalPlayer.CostumeRef.CostumeName == "Infernal_Machine_Car_Becritter_01")
                        return MountType.InfernalCar;
                    return MountType.None;
                }

                if (Category != "Nw_Boat_Mount")
                    return MountType.Mount;
                string internalName = InternalName;
                if (internalName == "M_Boat_Test_03")
                    return MountType.BoatPurple;
                if (internalName == "M_Boat_Test_02")
                    return MountType.BoatGreen;
                if (internalName == "M_Boat_Test_01")
                    return MountType.BoatWhite;
                return MountType.None;
            }
        }
    }
}
