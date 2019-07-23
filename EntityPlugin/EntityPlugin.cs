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
#if DEBUG
            // Вывод в лог всех States, загруженных в Engine, и их приоритетов
            foreach (Astral.Logic.Classes.FSM.State state in Astral.Quester.API.Engine.States)
            {
                Logger.WriteLine($"{state.DisplayName} {state.Priority}");
            }
#endif
        }

        public override void OnBotStop()
        {
            
        }

        public override void OnLoad()
        {

            States.SpellStuckMonitor.Activate = true;
            States.SlideMonitor.Activate = true;
#if CHANGE_WAYPOINT_DIST_SETTING
            States.SlideMonitor.DefaultChangeWaypointDist = Astral.API.CurrentSettings.ChangeWaypointDist;
#endif
        }

        public override void OnUnload()
        {
#if CHANGE_WAYPOINT_DIST_SETTING
           Astral.API.CurrentSettings.ChangeWaypointDist = States.SlideMonitor.DefaultChangeWaypointDist;
#endif
        }

    }
}
