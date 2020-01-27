using Astral.Quester.Classes;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityTools.Tools.Entities
{
    /// <summary>
    /// Сопоставление свойств Entity
    /// </summary>
    public class EntityComparerToOptions
    {
        public Predicate<Entity> Check { get; private set; }

        private readonly float Range = 0;
        private readonly List<CustomRegion> CustomRegions;
        private readonly Predicate<Entity> SpecialCheck;

        public EntityComparerToOptions(float range = 0, bool healthCheck = false, bool regionCheck = false, List<CustomRegion> customRegions = null, 
            Predicate<Entity> specialCheck = null)
        {
            Range = range;
            CustomRegions = customRegions;
            SpecialCheck = specialCheck;
            if (customRegions == null)
                if (specialCheck == null)
                    Check = (Entity e) =>
                    {
                        return (!regionCheck || e.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                            && (!healthCheck || !e.IsDead);
                    };
                else Check = (Entity e) =>
                    {
                        return (!regionCheck || e.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                && (!healthCheck || !e.IsDead)
                                && specialCheck(e);
                    };
            else if (specialCheck == null)
                Check = (Entity e) =>
                {
                    return (!regionCheck || e.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                            && (!healthCheck || !e.IsDead)
                            && customRegions.Find((CustomRegion cr) => e.Within(cr)) != null;
                };
            else Check = (Entity e) =>
                {
                    return (!regionCheck || e.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                            && (!healthCheck || !e.IsDead)
                            && customRegions.Find((CustomRegion cr) => e.Within(cr)) != null
                            && specialCheck(e);
                };

        }

        private bool CkeckRangeHealthRegionCRegionSpecial(Entity e)
        {
            return e.CombatDistance2 <= Range
                    && !e.IsDead
                    && e.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName
                    && CustomRegions.Find((CustomRegion cr) => e.Within(cr)) != null
                    && SpecialCheck(e);
        }
    }

    public class EntityAuraChecker
    {

    }
}
