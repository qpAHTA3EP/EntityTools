using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Linq;

namespace EntityTools.Tools.Missions
{
    /// <summary>
    /// Класс, инкапсулирующий идентификатор задания (квеста, миссии) с разделением на подзадачи
    /// </summary>
    public class MissionIdNode
    {
        public MissionIdNode(string missionId, MissionIdNode root = null)
        {
#if false
            if (root is null)
                throw new ArgumentNullException(nameof(root)); 
#endif
            _rootMission = root;
            _checkMissionId = (m) => m.MissionName == _id;

            if (!string.IsNullOrEmpty(missionId))
            {
                int ind = missionId.IndexOf('/');
                if (ind > 0)
                {
                    var missId = missionId.Substring(0, ind);
                    _id = missId;
                    var missIdTail = ++ind < missionId.Length ? missionId.Substring(ind) : string.Empty;

                    if (root is null)
                        _mission = EntityManager.LocalPlayer.Player.MissionInfo.Missions.FirstOrDefault(_checkMissionId);
                    else _mission = root.Mission?.Childrens.FirstOrDefault(_checkMissionId);

                    if (!string.IsNullOrEmpty(missIdTail))
                        _subMission = new MissionIdNode(missIdTail, this);
                }
            }
        }

        public string MissionId { get => _id;}
        readonly string _id;

        /// <summary>
        /// Ассоциированная игровая задача (миссия)
        /// </summary>
        public Mission Mission
        {
            get
            {
                if(_mission is null)
                {
                    if(_rootMission is null)
                        _mission = EntityManager.LocalPlayer.Player.MissionInfo.Missions.FirstOrDefault(_checkMissionId);
                    else _mission = _rootMission.Mission?.Childrens.FirstOrDefault(_checkMissionId); 
                }
                return _mission;
            }
        }
        Mission _mission;

        /// <summary>
        /// Подзадача
        /// </summary>
        public MissionIdNode SubMission { get => _subMission; }
        MissionIdNode _subMission;

        /// <summary>
        /// Главнная задача
        /// </summary>
        public MissionIdNode RootMission { get => _rootMission; }
        readonly MissionIdNode _rootMission;

        /// <summary>
        /// Идентификатор Главной задачи
        /// </summary>
        public string RootMissionId => _rootMission?._id;

        public override string ToString()
        {
            return _id;
        }

        readonly Func<Mission, bool> _checkMissionId;
    }
}
