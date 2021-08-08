using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Xml.Serialization;
using Astral;
using Astral.Logic.Classes.Map;
using Astral.Logic.NW;
using Astral.Quester.Classes;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityTools.Quester.Actions.Deprecated
{
    public class InteractEntity : Action
    {
        public override string ActionLabel => $"{GetType().Name} [{Name}]";

        public string Name { get; set; }

        public int Range { get; set; }

        public int InteractTime { get; set; }

        public int SkipTime { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public override string Category => "Deprecated";

        public override void InternalReset(){ }

        public override string InternalDisplayName => string.Empty;

        public override void GatherInfos() { }

        protected override bool IntenalConditions => false;

        public InteractEntity()
        {
            Name = "";
            Range = 15;
            InteractTime = 4000;
            SkipTime = 1000;
        }

       protected override ActionValidity InternalValidity
        {
            get
            {
                if (Name == string.Empty)
                {
                    return new ActionValidity("Name property not set.");
                }
                return new ActionValidity($"Action 'InteractEntity' is deprecated. Use action '{nameof(InteractEntities)}'");
            }
        }

        public override bool NeedToRun => true;

        public override ActionResult Run()
        {
            List<Entity> list = new List<Entity>();
            list.Clear();
            List<Entity> entities = EntityManager.GetEntities();
            foreach (Entity entity in entities)
            {
                bool flag = entity.InternalName.IndexOf(Name) >= 0;
                if (flag)
                {
                    list.Add(entity);
                }
            }
            Logger.WriteLine(list.Count + " items");
            for (; ; )
            {
                bool flag2 = !ConditionsAreOK && PlayWhileConditionsAreOk;
                if (flag2)
                {
                    break;
                }
                Approach.EntityByDistance(list[0], Range);
                list[0].Location.Face();
                Thread.Sleep(300);
                GameCommands.SimulateFKey();
                list[0].Location.Face();
                Thread.Sleep(300);
                GameCommands.SimulateFKey();
                Thread.Sleep(SkipTime);
                bool flag3 = list[0].CombatDistance3 < Range && list[0].InteractOption.CanInteract();
                if (flag3)
                {
                    Thread.Sleep(InteractTime);
                }
                bool flag4 = list[0].CombatDistance3 > Range + 7;
                if (!flag4)
                {
                    bool flag5 = list[0].CombatDistance3 < Range + 5 && !list[0].InteractOption.CanInteract();
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
            return ActionResult.Completed;
            IL_1BD:
            return ActionResult.Completed;
        }

        protected override Vector3 InternalDestination => Vector3.Empty;

        public override bool UseHotSpots => false;

        public override void OnMapDraw(GraphicsNW graph){ }
    }
}
