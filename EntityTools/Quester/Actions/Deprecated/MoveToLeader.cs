using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using Astral.Logic.Classes.Map;
using Astral.Quester;
using Astral.Quester.UIEditors;
using DevExpress.XtraEditors;
using EntityTools.Quester.Mapper;
using MyNW.Classes;
using MyNW.Internals;
using Action = Astral.Quester.Classes.Action;

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class MoveToLeader : Action
    {
        #region Опции команды
#if true
        [Description("Distance to the Team Leader by which it is necessary to approach")]
        [Category("Interruptions")]
#if true
        public float Distance { get; set; } = 30;
#else
        public float Distance
        {
            get => _distance;
            set
            {
                if (value < 0)
                    value = 0;
                _distance = value;
                if (_distance > 0)
                    _squareDistance = _distance * _distance;
                else _squareDistance = float.MaxValue;
            }
        }
        private float _distance = 30;
        private float _squareDistance = 900; 
#endif

        [Description("Enable IgnoreCombat profile value while playing action")]
        [Category("Interruptions")]
        public bool IgnoreCombat { get; set; } = true;

        [Description("True: Complite an action when the Team Leader is closer than 'Distance'\n" +
                     "False: Follow an Team Leader regardless of its distance")]
        [Category("Interruptions")]
        public bool StopOnApproached { get; set; } = false;

        //[Description("Check player is the member of the Team:\n" +
        //    "True: If the player is not a team member then an action stops ('Position' is ignored).\n" +
        //    "False: If the player is not a team member the player move to the 'Position'")]
        //[Category("Entity")]
        //public bool PartyCheck { get; set; }  

        public override string Category => "Deprecated";
#endif

        [Editor(typeof(PositionEditor), typeof(UITypeEditor))]
        public Vector3 Position { get; set; } = Vector3.Empty; 
        #endregion

        public override string ActionLabel
        {
            get
            {
                var tmLeader = GetLeader();
                if (tmLeader != null)
                    return $"{GetType().Name}: {tmLeader.Entity.InternalName}";
                return GetType().Name;
            }
        }


        public override bool NeedToRun
        {
            get
            {
                if (InternalDestination.Distance3DFromPlayer > Distance)
                {
                    if(IgnoreCombat)
                        API.IgnoreCombat = true;
                    return false;
                }

                if (IgnoreCombat)
                    API.IgnoreCombat = false;
                return true;
            }
        }

        public override ActionResult Run()
        {
            if (InternalDestination.Distance3DFromPlayer <= Distance)
            {
                if (StopOnApproached)
                    return ActionResult.Completed;
                return ActionResult.Running;
            }

            if (IgnoreCombat)
                API.IgnoreCombat = true;
            return ActionResult.Running;
        }

        protected override bool IntenalConditions
        {
            get
            {
                var team = EntityManager.LocalPlayer.PlayerTeam;
                return Position.IsValid
                       || team.IsInTeam && !team.IsLeader;
            }
        }

        public override void OnMapDraw(GraphicsNW graph)
        {
            graph.drawFillEllipse(InternalDestination, MapperHelper.Size_10x10/*new Size(10, 10)*/, Brushes.Beige);
        }

        public override void InternalReset()
        {
            tmLeaderCache = null;
            tmPlayerCache = null;
        }
        public override string InternalDisplayName => string.Empty;
        public override bool UseHotSpots => false;
        protected override Vector3 InternalDestination
        {
            get
            {
                var tmLeader = GetLeader();
                if (tmLeader != null)
                    return tmLeader.Entity.Location.Clone();
                return Position.Clone();
            }
        }

        private TeamMember GetLeader()
        {
            // Сопоставление местонахождения tmPlayer и tm
            bool CheckTeamMember(TeamMember tmPlayer, TeamMember tm)
            {
                if (tmPlayer != null && tm != null
                                     && tmPlayer.MapInstanceNumber == tm.MapInstanceNumber
                                     && tmPlayer.MapName == tm.MapName)

                {
                    var playerEntity = tmPlayerCache.Entity;
                    var leaderEntity = tm.Entity;

                    return playerEntity.IAICombatTeamID == leaderEntity.IAICombatTeamID
                           && playerEntity.RegionInternalName == leaderEntity.RegionInternalName;
                }

                return false;
            }

            if (CheckTeamMember(tmPlayerCache, tmLeaderCache))
                return tmLeaderCache;

            tmPlayerCache = null;
            tmLeaderCache = null;

            var team = EntityManager.LocalPlayer.PlayerTeam;
            if (!team.IsInTeam || team.IsLeader)
                return null;

            var playerId = EntityManager.LocalPlayer.ContainerId;
            var leaderId = team.Team.Leader.EntityId;

            foreach (var member in team.Team.Members)
            {
                var ettId = member.EntityId;
                if (ettId == playerId)
                {
                    tmPlayerCache = member;
                    continue;
                }

                if (ettId == leaderId)
                {
                    tmLeaderCache = member;
                }
            }

            if (CheckTeamMember(tmPlayerCache, tmLeaderCache))
                return tmLeaderCache;

            return null;
        }
        private TeamMember tmPlayerCache = null;
        private TeamMember tmLeaderCache = null;

        protected override ActionValidity InternalValidity
        {
            get
            {
                if (Position.IsValid)
                    return new ActionValidity();
                return new ActionValidity($"Property '{nameof(Position)}' is not set.");
            }
        }
        public override void GatherInfos()
        {
            XtraMessageBox.Show("Place the character on the default waypoint and press OK");
            Position = EntityManager.LocalPlayer.Location.Clone();
        }
    }
}
