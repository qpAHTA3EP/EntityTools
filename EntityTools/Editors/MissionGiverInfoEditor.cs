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
            Entity entity = null;
            if (EntityTools.Core.GUIRequest_EntityToInteract(ref entity))
            {
                var player = EntityManager.LocalPlayer;
                var giver = new MissionGiverInfo
                {
                    Id = entity.CostumeRef.CostumeName,
                    Position = entity.Location.Clone(),
                    MapName = player.MapState.MapName,
                    RegionName = player.RegionInternalName
                };
                value = giver;
            }
#else
            MissionGiverInfo giver = null;
            if (SetInfos(ref giver))
            {
                return giver;
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

        public static bool SetInfos(ref MissionGiverInfo missionGiver, MissionGiverType giverType = MissionGiverType.None)
        {
            if (giverType == MissionGiverType.None)
                if (!EntityTools.Core.GUIRequest_Item(() => DisplayedGivers, ref giverType))
                    return false;

            switch (giverType)
            {
                case MissionGiverType.NPC:
                    Entity entity = null;
                    if (EntityTools.Core.GUIRequest_EntityToInteract(ref entity))
                    {
                        var player = EntityManager.LocalPlayer;
#if false
                        var giver = new MissionGiverInfo
                        {
                            Type = MissionGiverType.NPC,
                            Id = entity.CostumeRef.CostumeName,
                            Position = entity.Location.Clone(),
                            MapName = player.MapState.MapName,
                            RegionName = player.RegionInternalName
                        }; 
                        missionGiver = giver;
#else
                        missionGiver = new MissionGiverInfo(entity.CostumeRef.CostumeName,
                                                            entity.Location,
                                                            player.MapState.MapName,
                                                            player.RegionInternalName);
#endif
                        return true;
                    }
                    break;
                case MissionGiverType.Remote:
                    string contactName = GetAnId.GetARemoteContact();
                    if (!string.IsNullOrEmpty(contactName))
                    {
#if false
                        var giver = new MissionGiverInfo
                        {
                            Type = MissionGiverType.Remote,
                            Id = contactName
                        }; 
                        missionGiver = giver;
#else
                        missionGiver = new MissionGiverInfo(contactName);
#endif
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
            return false;
        }
    }
#endif
}
