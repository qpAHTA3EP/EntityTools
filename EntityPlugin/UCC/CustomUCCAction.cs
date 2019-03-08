using System;
using System.ComponentModel;
using Astral.Classes;
using Astral.Controllers;
using Astral.Logic.NW;
using Astral.Logic.UCC.Classes;
using Astral.Quester.FSM.States;
using ns1;

namespace Astral.Logic.UCC.Actions
{
    [Serializable]
    public class CustomUCCAction : UCCAction
    {
        public override UCCAction Clone()
        {
            return base.BaseClone(new CustomUCCAction());
        }

        public override bool NeedToRun
        {
            get
            {
                return true;
            }
        }

        public override bool Run()
        {
            return true;
        }

        public override string ToString()
        {
            return GetType().Name;
        }

        [Browsable(false)]
        public new string ActionName { get; set; }

        public CustomUCCAction()
        {
        }
    }
}
