using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Threading;
using Astral;
using Astral.Logic.Classes.Map;
using Astral.Logic.NW;
using Astral.Quester.Classes;
using EntityPlugin.Editors;
using MyNW.Classes;
using MyNW.Internals;
using Action = Astral.Quester.Classes.Action;

namespace EntityPlugin.Actions
{
    public class InteractEntity : Astral.Quester.Classes.Action
    {
        public override string ActionLabel => $"[{Properties.Resources.CategoryDeprecated}] {GetType().Name} [{Name}]";

        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        public string Name { get; set; }

        public int Range { get; set; }

        public int InteractTime { get; set; }

        public int SkipTime { get; set; }

        public override string Category => Properties.Resources.CategoryDeprecated;

        public override void InternalReset()
        {
        }

        public override string InternalDisplayName
        {
            get
            {
                return string.Empty;
            }
        }

        public override void GatherInfos()
        {
        }

        protected override bool IntenalConditions => true;

        public InteractEntity()
        {
            this.Name = "";
            this.Range = 15;
            this.InteractTime = 4000;
            this.SkipTime = 1000;
        }

       protected override Astral.Quester.Classes.Action.ActionValidity InternalValidity
        {
            get
            {
                if (Name == string.Empty)
                {
                    return new Action.ActionValidity("Name property not set.");
                }
                return new Action.ActionValidity();
            }
        }

        public override bool NeedToRun => true;

        public override Astral.Quester.Classes.Action.ActionResult Run()
        {
            List<Entity> list = new List<Entity>();
            list.Clear();
            List<Entity> entities = EntityManager.GetEntities();
            foreach (Entity entity in entities)
            {
                bool flag = entity.InternalName.IndexOf(this.Name) >= 0;
                if (flag)
                {
                    list.Add(entity);
                }
            }
            Logger.WriteLine(list.Count + " items");
            for (; ; )
            {
                bool flag2 = !base.ConditionsAreOK && base.PlayWhileConditionsAreOk;
                if (flag2)
                {
                    break;
                }
                Approach.EntityByDistance(list[0], (float)this.Range, null);
                list[0].Location.Face();
                Thread.Sleep(300);
                GameCommands.SimulateFKey();
                list[0].Location.Face();
                Thread.Sleep(300);
                GameCommands.SimulateFKey();
                Thread.Sleep(this.SkipTime);
                bool flag3 = list[0].CombatDistance3 < (float)this.Range && list[0].InteractOption.CanInteract();
                if (flag3)
                {
                    Thread.Sleep(this.InteractTime);
                }
                bool flag4 = list[0].CombatDistance3 > (float)(this.Range + 7);
                if (!flag4)
                {
                    bool flag5 = list[0].CombatDistance3 < (float)(this.Range + 5) && !list[0].InteractOption.CanInteract();
                    if (flag5)
                    {
                        list.RemoveAt(0);
                    }
                    bool flag6 = list.Count != 0;
                    if (!flag6)
                    {
                        goto IL_1BD;
                    }
                }
            }
            return Astral.Quester.Classes.Action.ActionResult.Completed;
            IL_1BD:
            return Astral.Quester.Classes.Action.ActionResult.Completed;
        }

        protected override Vector3 InternalDestination => new Vector3();

        public override bool UseHotSpots => false;

        public override void OnMapDraw(GraphicsNW graph)
        {
        }

        public enum InteractionType
        {
            //Взаимодействие запрещено
            Forbidden,
            //Взаимодействовать если возможно
            IfPossible,
            //взаимодействие обязательно
            Obliganory
        }
    }
}
