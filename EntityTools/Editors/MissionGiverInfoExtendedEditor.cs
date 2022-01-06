﻿using Astral.Quester.Forms;
using EntityTools.Enums;
using EntityTools.Tools.Missions;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.ComponentModel;
using System.Drawing.Design;
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
            MissionGiverBase giver = null;
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
            MissionGiverType giverType = MissionGiverType.None;
            if (EntityTools.Core.UserRequest_SelectItem(() => DisplayedGivers, ref giverType))
            {
                switch (giverType)
                {
                    case MissionGiverType.NPC:
                        Entity entity = null;
                        if (EntityTools.Core.UserRequest_GetEntityToInteract(ref entity))
                        {
                            var player = EntityManager.LocalPlayer;
                            var giver = new MissionGiverNPC
                            {
                                Id = entity.CostumeRef.CostumeName,
                                Position = entity.Location.Clone(),
                                MapName = player.MapState.MapName,
                                RegionName = player.RegionInternalName
                            };
                            missionGiver = giver;
                            return true;
                        }
                        break;
                    case MissionGiverType.Remote:
                        string contactName = GetAnId.GetARemoteContact();
                        if (!string.IsNullOrEmpty(contactName))
                        {
                            var giver = new MissionGiverRemote
                            {
                                Id = contactName
                            };
                            missionGiver = giver;
#if false
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
#endif
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
