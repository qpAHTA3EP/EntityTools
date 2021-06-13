using MyNW.Classes;
using System;
using System.Linq;
using EntityCore.MountInsignias;

namespace EntityCore.Entities
{
    public class SimpleEntityWrapper
    {
        private Entity entity;
        public SimpleEntityWrapper(Entity entity)
        {
            this.entity = entity is null
                ? new Entity(IntPtr.Zero) 
                : entity;
        }
        public SimpleEntityWrapper(TeamMember teamMember)
        {
            entity = teamMember is null
                ? new Entity(IntPtr.Zero)
                : teamMember.Entity;
        }

        public uint ID => entity.ContainerId;
        public string InternalName => entity.InternalName;
        public string NameUntranslated => entity.NameUntranslated;
        public float Distance => entity.IsValid ? entity.CombatDistance3 : 0;
        public bool IsDead => entity.IsDead;

        public string Paragon =>
            entity.Character.CurrentPowerTreeBuild.SecondaryPaths.FirstOrDefault()?.Path.DisplayName ?? "Unknown";

        public float Health => entity.Character.AttribsBasic.Health;

        public override string ToString()
        {
            if (entity.IsValid)
                return $"{entity.InternalName} [{entity.ContainerId}]";

            return "Empty";
        }
    }
}
