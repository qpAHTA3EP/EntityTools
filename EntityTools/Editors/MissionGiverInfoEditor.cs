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
    internal class MissionGiverInfoEditor : UITypeEditor
    {
        static MissionGiverInfoEditor(){ }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
#if false
            if (SetInfos(out MissionGiverInfo giver))
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

#if false
        public static readonly MissionGiverType[] DisplayedGivers = { MissionGiverType.NPC,
                                                                      MissionGiverType.Remote,
                                                                      MissionGiverType.Item }; 
        public static bool SetInfos(out MissionGiverInfo missionGiver)
        {
            missionGiver = null;
            MissionGiverType giverType = MissionGiverType.None;

            if (EntityTools.Core.GUIRequest_Item(() => DisplayedGivers, ref giverType))
            {
                switch (giverType)
                {
                    case MissionGiverType.None:
                        return false;
                    case MissionGiverType.NPC:
                        NPCInfos npc = null;
                        if (EntityTools.Core.GUIRequest_NPCInfos(ref npc))
                        {
                            Entity betterEntityToInteract = Interact.GetBetterEntityToInteract();
                            if (betterEntityToInteract.IsValid)
                            {
                                var player = EntityManager.LocalPlayer;
                                missionGiver = new MissionGiverInfo
                                {
                                    GiverType = giverType,
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
                            missionGiver = new MissionGiverInfo()
                            {
                                GiverType = giverType,
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
                        }
                        break;
                    case MissionGiverType.Item:
                        GetAnItem.ListItem listItem = GetAnItem.Show();
                        if (listItem != null && !string.IsNullOrEmpty(listItem.ItemId))
                        {
                            missionGiver = new MissionGiverInfo
                            {
                                GiverType = giverType,
                                Id = listItem.ItemId
                            };
                        }
                        break;
                }
            }
            return false;
        }
#endif
    }
#endif
}
