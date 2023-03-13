using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using Astral.Logic.UCC.Classes;
using EntityTools.Tools.Entities;
using MyNW.Classes;

namespace EntityTools.Tools.Targeting
{
    public class EntityPriority : TargetPriorityEntry
    {
        //[Editor(typeof(Class38), typeof(UITypeEditor))]
        public List<EntityKey> Entities { get; set; } = new List<EntityKey>();

        public override ChooseTargetInfos HaveValidTarget(List<Entity> entities)
        {
            foreach (Entity entity in entities)
            {
                if (entity.CombatDistance <= MaxRange && (MinRange <= 0 || entity.CombatDistance >= MinRange))
                {
                    foreach (var entityKey in Entities)
                    {
                        if (entityKey.IsMatch(entity))
                        {
                            return new ChooseTargetInfos
                            {
                                Entity = entity,
                                Reason = "Entity",
                                Selected = true
                            };
                        }
                    }
                }
            }

            return new ChooseTargetInfos();
        }

        public override bool Unique => false;

        public override string ToString()
        {
            return $"Entity {Entities.Count}";
        }
    }
}
