using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using MyNW.Internals;
using MyNW.Patchables.Enums;

namespace EntityTools.Tools.Export
{
    [Serializable]
    public class PowersWrapper
    {
        public List<PowerInfo> Powers { get; set; }

        public PowersWrapper()
        {
            Powers = EntityManager.LocalPlayer.Character.Powers.Select(pwr => new PowerInfo(pwr)).ToList();
        }
    }

    [Serializable]
    public class PowerInfo : IEquatable<PowerInfo>, IComparable<PowerInfo>, IComparable
    {
        public PowerInfo(){ }

        public PowerInfo(Power power) 
        {
            if (power is null || !power.IsValid)
                throw new ArgumentNullException(nameof(power));

            var powerDef = power.PowerDef;

            InternalName = powerDef.InternalName;
            DisplayName = powerDef.DisplayName;
            FullName = $"{DisplayName} [{InternalName}]";
            hash = string.IsNullOrEmpty(InternalName) ? 0 : InternalName.GetHashCode();

            Description = powerDef.Description;
            GroundTargeted = powerDef.GroundTargeted;
            Cost = powerDef.Cost;
            Radius = powerDef.Radius;
            StartingRadius = powerDef.StartingRadius;
            RangeMax = powerDef.RangeMax;
            RangeMin = powerDef.RangeMin;
            RangeSecondary = powerDef.RangeSecondary;
            Arc = powerDef.Arc;

            Categories = powerDef.Categories;
            TargetMain = new PowerTargetInfo(powerDef.TargetMain);
            TargetAffected = new PowerTargetInfo(powerDef.TargetAffected);

            TraySlot = power.TraySlot;
            PowerId = power.PowerId;

            IsOnCooldown = power.IsOnCooldown();
            CanExec = power.CanExec();
            IsInTray = power.IsInTray;
        }

        public string InternalName { get; set; }
        public string DisplayName { get; set; }
        public string FullName { get; set; }
        private readonly int hash;

        public string Description { get; set; }

        public uint TraySlot { get; set; }
        public uint PowerId { get; set; }

        public uint Cost { get; set; }
        public uint Radius { get; set; }
        public float StartingRadius { get; set; }
        public float RangeMax { get; set; }
        public float RangeMin { get; set; }
        public float RangeSecondary { get; set; }

        public uint Arc { get; set; }

        public List<PowerCategory> Categories { get; set; }

        public PowerTargetInfo TargetMain { get; set; }
        public PowerTargetInfo TargetAffected { get; set; }
        public bool GroundTargeted { get; set; }
        public bool IsOnCooldown { get; set; }
        public bool CanExec { get; set; }
        public bool IsInTray { get; set; }


        public override string ToString() => FullName;

        public bool Equals(PowerInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return InternalName == other.InternalName;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PowerInfo) obj);
        }

        public override int GetHashCode() => hash;

        public static bool operator ==(PowerInfo left, PowerInfo right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PowerInfo left, PowerInfo right)
        {
            return !Equals(left, right);
        }

        public int CompareTo(PowerInfo other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return string.Compare(InternalName, other.InternalName, StringComparison.Ordinal);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            if (ReferenceEquals(this, obj)) return 0;
            return obj is PowerInfo other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(PowerInfo)}");
        }
    }

    [Serializable]
    public class PowerTargetInfo
    {
        public PowerTargetInfo() { }
        public PowerTargetInfo(PowerTarget powTar)
        {
            AffectFriend = powTar.AffectFriend;
            AffectFoe = powTar.AffectFoe;
            Self = powTar.Self;
            Name = powTar.Name;
        }

        public bool AffectFriend { get; set; }

        public bool AffectFoe { get; set; }

        public bool Self { get; set; }

        public string Name { get; set; }
    }
}
