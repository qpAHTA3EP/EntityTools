using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EntityPlugin
{
    public static class Tools
    {
        public static Entity FindClosestEntity(List<Entity> entities, string entPattern)
        {
            Entity closestEntity = new Entity(IntPtr.Zero);
            foreach (Entity entity in entities)
            {
                if (Regex.IsMatch(entity.NameUntranslated, entPattern))
                {
                    if (closestEntity.IsValid)
                    {
                        if (entity.Location.Distance3DFromPlayer < closestEntity.Location.Distance3DFromPlayer)
                            closestEntity = entity;
                    }
                    else closestEntity = entity;
                }
            }
            return closestEntity;
        }
    }
}
