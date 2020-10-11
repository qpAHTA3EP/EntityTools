using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using MyNW.Classes;
using MyNW.Patchables.Enums;

namespace EntityTools.Tools
{
    [Serializable]
    public class MissionNode
    {
        [XmlAttribute]
        public string Name;
        public string DisplayName;
        public string Summary;
        public string UIStringMsg;
        public MissionState State;
        public uint MissionType;
        public bool CanRepeat;
        public List<MissionNode> SubMissions;

        public MissionNode()
        {
            Name = string.Empty;
            State = MissionState.Dropped;
            DisplayName = string.Empty;
            Summary = string.Empty;
            UIStringMsg = string.Empty;
            MissionType = 0;
            CanRepeat = false;
            SubMissions = new List<MissionNode>();
        }

        public MissionNode(Mission mission)
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

                SubMissions = new List<MissionNode>();
                foreach(Mission subMission in mission.Childrens)
                {
                    SubMissions.Add(new MissionNode(subMission));
                }
            }
        }


        public MissionNode(CompletedMission mission)
        {
            if (mission != null && mission.IsValid)
            {
                State = MissionState.TurnedIn;
                if (mission.MissionDef != null && mission.MissionDef.IsValid)
                {
                    Name = mission.MissionDef.Name;
                    DisplayName = mission.MissionDef.DisplayName;
                    Summary = mission.MissionDef.Summary;
                    UIStringMsg = mission.MissionDef.UIStringMsg;
                    MissionType = mission.MissionDef.MissionType;
                    CanRepeat = mission.MissionDef.CanRepeat;
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

                SubMissions = new List<MissionNode>();
                foreach (MissionDef subMission in mission.MissionDef.SubMissions)
                {
                    SubMissions.Add(new MissionNode(subMission, MissionState.TurnedIn));
                }
            }
        }

        public MissionNode(MissionDef mission, MissionState state = MissionState.Dropped)
        {
            SubMissions = new List<MissionNode>();

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
                    SubMissions.Add(new MissionNode(subMission, MissionState.TurnedIn));
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
        public List<MissionNode> Missions;
        public List<MissionNode> OpenMissions;
        public List<MissionNode> CompletedMissions;

        public MissionsWrapper()
        {
            Missions = new List<MissionNode>();
            OpenMissions = new List<MissionNode>();
            CompletedMissions = new List<MissionNode>();
        }

        public MissionsWrapper(MissionInfo missionsInfo)
        {
            Missions = new List<MissionNode>();
            OpenMissions = new List<MissionNode>();
            CompletedMissions = new List<MissionNode>();

            if(missionsInfo != null && missionsInfo.IsValid)
            {
                foreach(Mission mission in missionsInfo.Missions)
                    Missions.Add(new MissionNode(mission));

                foreach (CompletedMission mission in missionsInfo.CompletedMissions)
                    CompletedMissions.Add(new MissionNode(mission));
            }
        }

        public MissionsWrapper(LocalPlayerEntity player)
        {
            Missions = new List<MissionNode>();
            OpenMissions = new List<MissionNode>();
            CompletedMissions = new List<MissionNode>();

            if (player != null && player.IsValid)
            {
                foreach (Mission mission in player.Player?.MissionInfo?.Missions)
                    Missions.Add(new MissionNode(mission));

                foreach (CompletedMission mission in player.Player?.MissionInfo?.CompletedMissions)
                    CompletedMissions.Add(new MissionNode(mission));

                foreach (OpenMission mission in player?.MapState?.OpenMissions)
                    if(mission.Mission.IsValid)
                        OpenMissions.Add(new MissionNode(mission.Mission));
            }
        }
    }
}
