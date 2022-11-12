using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Astral.Logic.NW;
using EntityCore.Forms;
using EntityTools.Forms;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityTools.Tools
{
    internal static class TargetSelectHelper
    {
        public static Entity GetEntityToInteract()
        {
            while (TargetSelectForm.GUIRequest("Get NPC", "Target the NPC and press ok.") == DialogResult.OK)
            {
                Entity betterEntityToInteract = Interact.GetBetterEntityToInteract();
                if (betterEntityToInteract.IsValid)
                    return betterEntityToInteract;
            }

            return null;
        }

        public static Vector3 GetNodeLocation(string caption, string message = "")
        {
            var localPlayer = EntityManager.LocalPlayer;
            while (localPlayer.IsValid && !localPlayer.IsLoading
                   && TargetSelectForm.GUIRequest(caption, message) == DialogResult.OK)
            {
                var playerInteractStatus = localPlayer.Player.InteractStatus;
                if (playerInteractStatus.pMouseOverNode != IntPtr.Zero)
                {
                    var node = playerInteractStatus.PreferredTargetNode;
                    if (node != null && node.IsValid && node.IsMouseOver)
                        return node.Location.Clone();
                }
            }
            return Vector3.Empty;
        }
    }
}
