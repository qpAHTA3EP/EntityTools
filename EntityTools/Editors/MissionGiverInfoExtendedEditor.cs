using System;
using System.ComponentModel;
using System.Drawing.Design;
using Astral.Quester.Forms;
using EntityTools.Enums;
using EntityTools.Forms;
using EntityTools.Tools;
using EntityTools.Tools.Missions;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityTools.Editors
{
    internal class MissionGiverInfoExtendedEditor : UITypeEditor
    {
        static MissionGiverInfoExtendedEditor(){ }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            MissionGiverBase giver = null;
            if (SetInfos(ref giver))
            {
                return giver;
            } 
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
            if (ItemSelectForm.GetAnItem(() => DisplayedGivers, ref giverType))
            {
                switch (giverType)
                {
                    case MissionGiverType.NPC:
                        Entity entity = TargetSelectHelper.GetEntityToInteract();
                        if (entity != null)
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
                            return true;
                        }
                        break;
                }
            }
            return false;
        }
    }
}
