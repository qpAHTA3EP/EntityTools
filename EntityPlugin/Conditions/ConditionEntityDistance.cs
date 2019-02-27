using Astral.Quester.Classes;
using EntityPlugin.Editors;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using static Astral.Quester.Classes.Condition;

namespace EntityPlugin.Conditions
{
    [Serializable]
    public class EntityDistance : Condition
    {
        public EntityDistance()
        {
            EntityID = "";
            Distance = 0;
            Sign = Relation.Inferior;
        }

        [Description("ID (an untranslated name) of the Entity for the search (regex)")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        public string EntityID { get; set; }

        public float Distance { get; set; }

        [Description("Distance comparison type to the closest Entity")]
        public Condition.Relation Sign { get; set; }

        public override bool IsValid
        {
            get
            {
                Entity closestEntity = Tools.FindClosestEntity(EntityManager.GetEntities(), EntityID);

                //if(entities.Count > 0)
                //    closestEntity = entities.FindLast((Entity x) => (Regex.IsMatch(x.NameUntranslated, EntityID) && (x.CombatDistance < Distance))); 

                //List<Entity> entityList = entities.Find((Entity x) => x.CombatDistance3 < Distance);

                //string mess = "";
                bool result = false;
                switch (Sign)
                {
                    case Relation.Equal:
                        return result = (closestEntity != null) && (closestEntity.Location.Distance3DFromPlayer == Distance);
                        //mess = (result) ? Relation.Equal.ToString() : Relation.NotEqual.ToString();
                        //break;
                    case Relation.NotEqual:
                        return result = (closestEntity != null) && (closestEntity.Location.Distance3DFromPlayer != Distance);
                        //mess = (result) ? Relation.NotEqual.ToString() : Relation.Equal.ToString();
                        //break;
                    case Relation.Inferior:
                        return result = (closestEntity != null) && (closestEntity.Location.Distance3DFromPlayer < Distance);
                        //mess = ((result) ? "" : "Not")+ Relation.Inferior.ToString();
                        //break;
                    case Relation.Superior:
                        return result = (closestEntity == null) || (closestEntity.Location.Distance3DFromPlayer > Distance);
                        //mess = (result) ? Relation.Superior.ToString() : Relation.Inferior.ToString(); ;
                        //break;
                }

                //if (Sign == Relation.Equal || Sign == Relation.NotEqual)
                //    mess = (closestEntity.CombatDistance == Distance) ? Relation.Equal.ToString(): Relation.NotEqual.ToString();
                //else if (Sign == Relation.Inferior || Sign == Relation.Superior)
                //    mess = (closestEntity.CombatDistance == Distance) ? Relation.Equal.ToString() : Relation.NotEqual.ToString();

                //Debug.WriteLine(string.Format("Entity [{1}] matched to [{2}] at the Distance = {3} that {4} to {5}", 
                //    closestEntity.NameUntranslated,                                
                //    EntityID, 
                //    closestEntity.CombatDistance,
                //    mess, Distance));

                return false;
            }
        }

        public override void Reset()
        {
        }

        public override string ToString()
        {
            return string.Format("Entity [{0}] Distance {1} to {2}",
                    EntityID,
                    Sign,
                    Distance);
        }

        public override string TestInfos
        {
            get
            {
                Entity closestEntity = Tools.FindClosestEntity(EntityManager.GetEntities(), EntityID);

                if (closestEntity.IsValid)
                {
                    return string.Format("Found closect Entity [{0}] at the Distance = {1}",
                            closestEntity.NameUntranslated,
                            closestEntity.Location.Distance3DFromPlayer);
                }
                else
                {
                    return string.Format("No one Entity matched to [{0}]", EntityID);
                }
            }
        }
    }
}
