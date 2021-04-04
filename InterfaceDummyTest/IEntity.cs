using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceStubTest
{
    interface IEntity
    {
        bool IsUntargetable { get; }
        bool IsUnselectable { get; }
        bool DoNotDraw { get; }
        //EntityAttach Attach { get; }
        string Name { get; }
        string NameUntranslated { get; }
        string DebugName { get; }
        string RegionInternalName { get; }
        string InternalName { get; }
        bool CanInteract { get; }
        //EntityRelation RelationToPlayer { get; }
        float CombatDistance { get; }
        bool InCombat { get; }
        //SavedEntityData Saved { get; }
        //Character Character { get; }
        //Player Player { get; }
        //Critter Critter { get; }
        //EntityUI EntityUI { get; }
        //CostumeRef CostumeRef { get; }
        //PlayerTeam PlayerTeam { get; }
        //Inventory Inventory { get; }
        bool IsDead { get; }
        //InteractOption InteractOption { get; }
        bool IsPlayer { get; }
        float Yaw { get; }
        bool IsValid { get; }
        uint RefId { get; }
        bool FacingPlayer { get; }
        //Vector3 Location { get; }
        uint IAICombatTeamID { get; }
        uint ContainerId { get; }
        uint Flags { get; }
        uint OwnerRefId { get; }
        float X { get; }
        float Y { get; }
        float Z { get; }
        float Radius { get; }
        float CombatDistance2 { get; }
        float CombatDistance3 { get; }
        //GlobalType Type { get; }
        bool IsMounted { get; }

        bool FacingPlayerByRange(float range = 0.05F);
        string GetName(bool localized);
        void Interact();
        //bool IsEntityFlagActive(GlobalEntityFlags flag);
        bool IsLineOfSight();
        void SetYaw(float value);
    }
}
