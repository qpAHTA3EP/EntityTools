using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Threading;
using Astral;
using Astral.Logic.Classes.Map;
using Astral.Logic.NW;
using Astral.Quester.UIEditors;
using EntityPlugin.Editors;
using MyNW.Classes;
using MyNW.Internals;
using Action = Astral.Quester.Classes.Action;

namespace EntityPlugin.Actions
{
    /// <summary>
    /// Расширение команды MoveToEntity, позволяющее взаимодействовать с Entity
    /// </summary>
    public class InteractEntities : MoveToEntity
    {
        InteractEntities() : base()
        {
            InteractIfPossible = true;
            InteractTime = 2000;
            Dialogs = new List<string>();
        }

        [Description("Try interaction to Entity if possible")]
        public bool InteractIfPossible { get; set; }

        [Description("Time to interact (ms)")]
        public int InteractTime { get; set; }

        [Description("Answers in dialog while interact with Entity")]
        [Editor(typeof(DialogEditor), typeof(UITypeEditor))]
        public List<string> Dialogs { get; set; }

        public override string ActionLabel
        {
            get
            {
                return $"{GetType().Name} [{EntityID}]";
            }
        }

        protected override ActionResult InternalInteraction()
        {
            ActionResult actnReslt = ActionResult.Running;

            if (InteractIfPossible && target.IsValid && target.InteractOption.IsValid && Approach.EntityForInteraction(target, null))
            {
                MyNW.Internals.Movements.StopNavTo();
                Thread.Sleep(500);
                target.Interact();
                Thread.Sleep(InteractTime);
                Interact.WaitForInteraction();
                if (Dialogs.Count > 0)
                {
                    Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(5000);
                    while (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Options.Count == 0)
                    {
                        if (timeout.IsTimedOut)
                        {
                            actnReslt = ActionResult.Running;
                            break;
                        }
                        Thread.Sleep(100);
                    }
                    Thread.Sleep(500);
                    using (List<string>.Enumerator enumerator = Dialogs.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            string key = enumerator.Current;
                            EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.SelectOptionByKey(key, "");
                            Thread.Sleep(1000);
                        }
                    }
                }
                EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Close();
                target = new Entity(IntPtr.Zero);
            }
            return actnReslt;
        }
    }
}
