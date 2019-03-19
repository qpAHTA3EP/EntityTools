using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Astral.Forms;

namespace EntityPlugin
{
    public class EntityPlugin : Astral.Addons.Plugin
    {
        public override string Name => GetType().Name;

        public override string Author => "MichaelProg";

        public override System.Drawing.Image Icon => null;

        public override BasePanel Settings => new Forms.MainPanel();

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
