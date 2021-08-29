﻿using Astral.Quester.Forms;
using EntityTools.Enums;
using EntityTools.Tools.Missions;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.ComponentModel;
using System.Drawing.Design;
namespace EntityTools.Editors
{
#if DEVELOPER
    internal class MissionGiverInfoEditor : UITypeEditor
    {
        static MissionGiverInfoEditor(){ }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            MissionGiverInfo giver = null;
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

        public static bool SetInfos(ref MissionGiverInfo missionGiver, MissionGiverType giverType = MissionGiverType.None)
        {
            if (giverType == MissionGiverType.None)
                if (!EntityTools.Core.UserRequest_SelectItem(() => DisplayedGivers, ref giverType))
                    return false;

            switch (giverType)
            {
                case MissionGiverType.NPC:
                    Entity entity = null;
                    if (EntityTools.Core.UserRequest_GetEntityToInteract(ref entity))
                    {
                        var player = EntityManager.LocalPlayer;
                        missionGiver = new MissionGiverInfo(entity.CostumeRef.CostumeName,
                                                            entity.Location,
                                                            player.MapState.MapName,
                                                            player.RegionInternalName);
                        return true;
                    }
                    break;
                case MissionGiverType.Remote:
                    string contactName = GetAnId.GetARemoteContact();
                    if (!string.IsNullOrEmpty(contactName))
                    {
                        missionGiver = new MissionGiverInfo(contactName);
                        return true;
                    }
                    break;
            }
            return false;
        }
    }
#endif
}