using MyNW.Classes;
using System;

namespace EntityTools.Tools.Export
{
    class PowerInfo : IEquatable<PowerInfo>, IComparable<PowerInfo>, IComparable
    {
        public PowerInfo(Power power) 
        {
            if (power is null || !power.IsValid)
                throw new ArgumentNullException(nameof(power));

            var powerDef = power.PowerDef;

            InternalName = powerDef.InternalName;
            DisplayName = powerDef.DisplayName;

            FullName = $"{DisplayName} [{InternalName}]";
        }

        public string InternalName { get; }
        public string DisplayName { get; }
        public string FullName { get; }

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

        public override int GetHashCode()
        {
            return InternalName != null ? InternalName.GetHashCode() : 0;
        }

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
}
