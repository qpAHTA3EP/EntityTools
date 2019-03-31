using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Astral.Forms;

namespace EntityPlugin
{
    public class EntityPlugin : Astral.Addons.Plugin
    {
        public static bool DebugInfoEnabled { get; set; }

        public override string Name => GetType().Name;

        public override string Author => "MichaelProg";

        public override System.Drawing.Image Icon => null;

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

        }

        public override void OnUnload()
        {

        }
    }
}
