using Astral.Logic.Classes.Map;
using EntityTools.Enums;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Astral;
using Action = Astral.Quester.Classes.Action;

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class PromoteTeammate : Action
    {
        [Description("Способ выбора члена группы, которому будет передано лидерство.")]
        public PromotionType NewLeader { get; set; } = PromotionType.NextOne;

        [Description("Зацикливание порядка передачи лидерства при последовательных способах выбора нового лидера (" + nameof(PromotionType.NextOne) + " или " + nameof(PromotionType.NextAlphabetical) + "). \n" +
                     "True: Передача лидерства от последнего члена группы первому.\n" +
                     "False : Смена лидера, являющегося последним членом группы, не производится.")]
        public bool Cycling { get; set; }

        [Description("Количество попыток передачи лидерства")]
        public int AttemptsNumber { get; set; } = 3;

        public override string ActionLabel => string.Concat(nameof(PromoteTeammate), " : ", NewLeader);

        public override bool NeedToRun => true;

        public override string InternalDisplayName => string.Empty;

        public override bool UseHotSpots => false;

        protected override bool IntenalConditions => EntityManager.LocalPlayer.PlayerTeam.IsLeader;

        protected override Vector3 InternalDestination => Vector3.Empty;

        protected override ActionValidity InternalValidity => new ActionValidity();

        public override void GatherInfos() { }

        public override void InternalReset()
        {
            tryNum = 0;
        }

        public override void OnMapDraw(GraphicsNW graph) { }

        public override ActionResult Run()
        {
            var player = EntityManager.LocalPlayer;
            var playerTeam = player.PlayerTeam;

            if (!playerTeam.IsInTeam) 
                return ActionResult.Fail;
            
            if (!playerTeam.IsLeader)
                return ActionResult.Skip;

            var team = playerTeam.Team;
            var leader = team.Leader;
            var leaderId = team.Leader.EntityId;
            var playerId = player.ContainerId;
            var members = team.Members;
            var memberCount = members.Count;
            TeamMember newLeader = null;

            switch (NewLeader)
            {
                case PromotionType.FirstAvailable:
                {
                    foreach (var teamMember in members)
                    {
                        if (teamMember.EntityId != leaderId)
                        {
                            newLeader = teamMember;
                            break;
                        }
                    }
                }
                break;
                case PromotionType.NextOne:
                {
                    int newLeaderInd = members.FindIndex(tm => tm.EntityId == playerId) + 1;

                    if (newLeaderInd >= memberCount)
                    {
                        if (!Cycling)
                            return ActionResult.Skip;

                        newLeaderInd %= memberCount;
                    }

                    newLeader = members[newLeaderInd];
                }
                break;
                case PromotionType.NextAlphabetical:
                {
                    var leaderInternalName = leader.InternalName;
                    var orderedMembers = members.OrderBy(member => member.InternalName).ToList();
                    newLeader = orderedMembers.FirstOrDefault( tm => string.Compare(leaderInternalName, tm.InternalName, StringComparison.Ordinal) < 0);
                    
                    if (newLeader is null)
                    {
                        if (!Cycling)
                            return ActionResult.Skip;

                        newLeader = orderedMembers.FirstOrDefault();
                    }
                }
                break;
                case PromotionType.Random:
                {
                    int newLeaderInd = random.Next(memberCount);
                    if (newLeaderInd == leaderId)
                        newLeaderInd++;
                    if (newLeaderInd >= memberCount)
                    {
                        if (!Cycling)
                            return ActionResult.Skip;

                        newLeaderInd %= memberCount;
                    }

                    newLeader = members[newLeaderInd];
                }
                break;
            }

            if (PromoteTeamMember(newLeader))
                return ActionResult.Completed;

            int attemptsNumber = AttemptsNumber;
            if (attemptsNumber <= 0 || tryNum > attemptsNumber)
                return ActionResult.Fail;

            Thread.Sleep(500);
            tryNum++;
            return ActionResult.Running;
        }

        private static readonly Random random = new Random();
        private int tryNum;

        /// <summary>
        /// Передача лидерства члену группы <param name="teamMember"/>
        /// </summary>
        /// <returns>Успех передачи лидерства заданному члену группы</returns>
        private bool PromoteTeamMember(TeamMember teamMember)
        {
            if (teamMember is null)
                return false;

            var id = teamMember.EntityId;
            var teamMemberName = teamMember.InternalName;
            if (string.IsNullOrEmpty(teamMemberName))
                return false;

            var command = "Team_Promote " + teamMemberName;
            GameCommands.Execute(command);
            Logger.WriteLine(command);
            Thread.Sleep(1000);

            return EntityManager.LocalPlayer.PlayerTeam.Team.Leader.EntityId == id;
        }
    }
}