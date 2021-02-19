using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using Astral.Logic.NW;
using Astral.Quester.Classes;
using Astral.Quester.Forms;
using DevExpress.XtraEditors;
using EntityTools.Enums;
using EntityTools.Tools.BuySellItems;
using EntityTools.Tools.Missions;
using MyNW.Classes;
using MyNW.Internals;
//using static EntityTools.Reflection.PrivateConsructor;

namespace EntityTools.Editors
{
#if DEVELOPER
    internal class MissionGiverInfoExtendedEditor : UITypeEditor
    {
        static MissionGiverInfoExtendedEditor(){ }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
#if true
            MissionGiverBase giver = value as MissionGiverBase;
            if (SetInfos(ref giver))
            {
                return giver;
            } 
#else
            NPCInfos npc = null;
            if (EntityTools.Core.GUIRequest_NPCInfos(ref npc))
            {
                Entity betterEntityToInteract = Interact.GetBetterEntityToInteract();
                if (betterEntityToInteract.IsValid)
                {
                    var player = EntityManager.LocalPlayer;
                    value = new MissionGiverInfo
                    {
                        Id = betterEntityToInteract.CostumeRef.CostumeName,
                        Position = betterEntityToInteract.Location.Clone(),
                        MapName = player.MapState.MapName,
                        RegionName = player.RegionInternalName
                    };
                }
            }
#endif
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public static readonly MissionGiverType[] DisplayedGivers = { MissionGiverType.NPC,
                                                                      MissionGiverType.Remote }; 
        public static bool SetInfos(ref MissionGiverBase missionGiver)
        {
            missionGiver = null;
            MissionGiverType giverType = MissionGiverType.None;

            if (EntityTools.Core.GUIRequest_Item(() => DisplayedGivers, ref giverType))
            {
                switch (giverType)
                {
                    case MissionGiverType.NPC:
                        NPCInfos npc = null;
                        if (EntityTools.Core.GUIRequest_NPCInfos(ref npc))
                        {
                            Entity betterEntityToInteract = Interact.GetBetterEntityToInteract();
                            if (betterEntityToInteract.IsValid)
                            {
                                var player = EntityManager.LocalPlayer;
                                missionGiver = new MissionGiverNPC
                                {
                                    Id = betterEntityToInteract.CostumeRef.CostumeName,
                                    Position = betterEntityToInteract.Location.Clone(),
                                    MapName = player.MapState.MapName,
                                    RegionName = player.RegionInternalName
                                };
                                return true;
                            }
                        }
                        break;
                    case MissionGiverType.Remote:
                        string contactName = GetAnId.GetARemoteContact();
                        if (!string.IsNullOrEmpty(contactName))
                        {
                            missionGiver = new MissionGiverRemote()
                            {
                                Id = contactName
                            };
                            if (XtraMessageBox.Show("Call it now ?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                foreach (RemoteContact remoteContact in EntityManager.LocalPlayer.Player.InteractInfo.RemoteContacts)
                                {
                                    if (remoteContact.ContactDef != contactName) continue;

                                    missionGiver.Id = remoteContact.ContactDef;
                                    remoteContact.Start();
                                    break;
                                }
                            }
                            return true;
                        }
                        break;
                }
            }
            return false;
        }
#endif
    }
}
