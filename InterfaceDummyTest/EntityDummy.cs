using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceStubTest
{
    public class EntityStub //: IEntity
    {
        public bool IsUntargetable => false;
        public bool IsUnselectable => false;
        public bool DoNotDraw => false;
        //public EntityAttach Attach => new EntityAttach(IntPtr.Zero);
        public string Name => GetType().Name;
        public string NameUntranslated => GetType().Name;
        public string DebugName => GetType().Name;
        public string RegionInternalName => GetType().Name;
        public string InternalName => GetType().Name;
        public bool CanInteract => false;
        //public EntityRelation RelationToPlayer => EntityRelation.Unknown;
        public float CombatDistance => 0;
        public bool InCombat => false;
        //public SavedEntityData Saved { get; }
        //public Character Character { get; }
        //public Player Player { get; }
        //public Critter Critter { get; }
        //public EntityUI EntityUI { get; }
        //public CostumeRef CostumeRef { get; }
        //public PlayerTeam PlayerTeam { get; }
        //public Inventory Inventory { get; }
        public bool IsDead => false;
        //public InteractOption InteractOption { get; }
        public bool IsPlayer => false;
        public float Yaw => 0;
        public bool IsValid => true;
        public uint RefId => uint.MaxValue;
        public bool FacingPlayer => false;
        //public Vector3 Location => Vector3.Empty;
        public uint IAICombatTeamID => uint.MaxValue / 2;
        public uint ContainerId => uint.MaxValue;
        public uint Flags => uint.MaxValue;
        public uint OwnerRefId => uint.MaxValue;
        public float X => 0;
        public float Y { get; }
        public float Z { get; }
        public float Radius { get; }
        public float CombatDistance2 { get; }
        public float CombatDistance3 { get; }
        //public GlobalType Type { get; }
        public bool IsMounted { get; }

        public bool FacingPlayerByRange(float range = 0.05F) => false;
        public string GetName(bool localized) => GetType().Name;
        public void Interact() { }
        //public bool IsEntityFlagActive(GlobalEntityFlags flag) => false;
        public bool IsLineOfSight() => false;
        public void SetYaw(float value) { }
        public override string ToString() => GetType().Name;
    }
}
