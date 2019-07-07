using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Astral;
using Astral.Forms;

namespace EntityPlugin
{
    public class EntityPlugin : Astral.Addons.Plugin
    {
        public static bool DebugInfoEnabled { get; set; }

        public override string Name => "Entity Tools";

        public override string Author => "MichaelProg";

        public override System.Drawing.Image Icon => Properties.Resources.EntityIcon;

        public override BasePanel Settings => new Forms.MainPanel();

        public EntityPlugin() : base ()
        {
            DebugInfoEnabled = false;
        }

        public override void OnBotStart()
        {
            
        }

        public override void OnBotStop()
        {
            
        }

        public override void OnLoad()
        {
            Astral.Quester.API.BeforeStartEngine += API_BeforeStartEngine;
        }

        public override void OnUnload()
        {

        }

        private void API_BeforeStartEngine(object sender, Astral.Logic.Classes.FSM.BeforeEngineStart e)
        {
            States.SpellStuckMonitor.Activate();
        }

    }
}
