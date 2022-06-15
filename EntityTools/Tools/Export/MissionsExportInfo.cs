using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using MyNW.Classes;
using MyNW.Patchables.Enums;

namespace EntityTools.Tools
{
    [Serializable]
    public class MissionInfo
    {
        [XmlAttribute]
        public string Name;
        public string DisplayName;
        public string Summary;
        public string UIStringMsg;
        public MissionState State;
        public uint MissionType;
        public bool CanRepeat;
        public List<MissionInfo> SubMissions;

        public MissionInfo()
        {
            Name = string.Empty;
            State = MissionState.Dropped;
            DisplayName = string.Empty;
            Summary = string.Empty;
            UIStringMsg = string.Empty;
            MissionType = 0;
            CanRepeat = false;
            SubMissions = new List<MissionInfo>();
        }

        public MissionInfo(Mission mission)
        {
            if(mission != null && mission.IsValid)
            {
                Name = mission.MissionName;
                State = mission.State;
                if (mission.MissionDef != null && mission.MissionDef.IsValid)
                {
                    DisplayName = mission.MissionDef.DisplayName;
                    Summary = mission.MissionDef.Summary;
                    UIStringMsg = mission.MissionDef.UIStringMsg;
                    MissionType = mission.MissionDef.MissionType;
                    CanRepeat = mission.MissionDef.CanRepeat;
                }
                else
                {
                    DisplayName = string.Empty;
                    Summary = string.Empty;
                    UIStringMsg = string.Empty;
                    MissionType = 0;
                    CanRepeat = false;
                }

                SubMissions = new List<MissionInfo>();
                foreach(Mission subMission in mission.Childrens)
                {
                    SubMissions.Add(new MissionInfo(subMission));
                }
            }
        }


        public MissionInfo(CompletedMission mission)
        {
            if (mission != null && mission.IsValid)
            {
                State = MissionState.TurnedIn;
                SubMissions = new List<MissionInfo>();
                if (mission.MissionDef != null && mission.MissionDef.IsValid)
                {
                    Name = mission.MissionDef.Name;
                    DisplayName = mission.MissionDef.DisplayName;
                    Summary = mission.MissionDef.Summary;
                    UIStringMsg = mission.MissionDef.UIStringMsg;
                    MissionType = mission.MissionDef.MissionType;
                    CanRepeat = mission.MissionDef.CanRepeat;
                    foreach (MissionDef subMission in mission.MissionDef.SubMissions)
                        SubMissions.Add(new MissionInfo(subMission));
                }
                else
                {
                    Name = string.Empty;
                    DisplayName = string.Empty;
                    Summary = string.Empty;
                    UIStringMsg = string.Empty;
                    MissionType = 0;
                    CanRepeat = false;
                }

            }
        }

        public MissionInfo(MissionDef mission)
        {
            SubMissions = new List<MissionInfo>();

            if (mission != null && mission.IsValid)
            {
                Name = mission.Name;
                DisplayName = mission.DisplayName;
                Summary = mission.Summary;
                UIStringMsg = mission.UIStringMsg;
                MissionType = mission.MissionType;
                CanRepeat = mission.CanRepeat;
                
                foreach (MissionDef subMission in mission.SubMissions)
                {
                    SubMissions.Add(new MissionInfo(subMission));
                }
            }
            else
            {
                Name = string.Empty;
                DisplayName = string.Empty;
                Summary = string.Empty;
                UIStringMsg = string.Empty;
                MissionType = 0;
                CanRepeat = false;
            }        
        }
    }

    [Serializable]
    public class MissionsWrapper
    {
        public List<MissionInfo> Missions;
        public List<MissionInfo> OpenMissions;
        public List<MissionInfo> CompletedMissions;

        public MissionsWrapper()
        {
            Missions = new List<MissionInfo>();
            OpenMissions = new List<MissionInfo>();
            CompletedMissions = new List<MissionInfo>();
        }

        public MissionsWrapper(MyNW.Classes.MissionInfo missionsInfo)
        {

            if(missionsInfo != null && missionsInfo.IsValid)
            {
                Missions = new List<MissionInfo>(missionsInfo.Missions.Count);
                OpenMissions = new List<MissionInfo>();
                CompletedMissions = new List<MissionInfo>(missionsInfo.CompletedMissions.Count);

                foreach(Mission mission in missionsInfo.Missions)
                    Missions.Add(new MissionInfo(mission));

                foreach (CompletedMission mission in missionsInfo.CompletedMissions)
                    CompletedMissions.Add(new MissionInfo(mission));
            }
        }

        public MissionsWrapper(LocalPlayerEntity player)
        {
            Missions = new List<MissionInfo>();
            OpenMissions = new List<MissionInfo>();
            CompletedMissions = new List<MissionInfo>();

            if (player != null && player.IsValid)
            {
                foreach (Mission mission in player.Player.MissionInfo.Missions)
                    Missions.Add(new MissionInfo(mission));

                foreach (CompletedMission mission in player.Player.MissionInfo.CompletedMissions)
                    CompletedMissions.Add(new MissionInfo(mission));

                foreach (OpenMission mission in player.MapState.OpenMissions)
                    if(mission.Mission.IsValid)
                        OpenMissions.Add(new MissionInfo(mission.Mission));
            }
        }
    }
}
