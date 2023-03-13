using System;
using System.Text;
using Astral.Quester.Classes;
using EntityTools.Core.Interfaces;
using EntityTools.Extensions;
using EntityTools.Patches.Mapper;
using MyNW.Classes;
using MyNW.Internals;
using static Astral.Quester.Classes.Condition;

namespace EntityTools.Quester.Conditions.Engines
{
    class TeamMembersCountEngine : IQuesterConditionEngine
    {
        private TeamMembersCount @this;

        #region данные ядра
        /// <summary>
        /// Кэшированное число членов группы, удовлетворяющих критериям
        /// </summary>
        //private int membersCount;
        private string _label = string.Empty;

        /// <summary>
        /// Префикс, идентифицирующий принадлежность отладочной информации, выводимой в Лог
        /// </summary>
        private string _idStr = string.Empty;

        #endregion

        internal TeamMembersCountEngine(TeamMembersCount tmc)
        {
            InternalRebase(tmc);

            ETLogger.WriteLine(LogType.Debug, $"{_idStr} initialized: {Label()}");
        }
        ~TeamMembersCountEngine()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (@this != null)
            {
                @this.PropertyChanged -= PropertyChanged;
                @this.Engine = null;
                @this = null;
            }
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (ReferenceEquals(sender, @this))
            {
                _label = string.Empty;
                memberCountChecker = Initicalize_MemberCountChecker;
                counter = Initialize_Counter;

                //membersCount = 0;
            }
        }

        public bool Rebase(Condition condition)
        {
            if (condition is null)
                return false;
            if (ReferenceEquals(condition, @this))
                return true;
            if (condition is TeamMembersCount tmc)
            {
                InternalRebase(tmc);
                ETLogger.WriteLine(LogType.Debug, $"{_idStr} reinitialized");
                return true;
            }

            string debugStr = string.Concat("Rebase failed. ", condition.GetType().Name, '[', condition.GetHashCode().ToString("X2"), "] can't be casted to '" + nameof(TeamMembersCount) + '\'');
            ETLogger.WriteLine(LogType.Error, debugStr);
            throw new InvalidCastException(debugStr);
        }

        private bool InternalRebase(TeamMembersCount tmc)
        {
            // Убираем привязку к старому условию
            if (@this != null)
            {
                @this.PropertyChanged -= PropertyChanged;
                @this.Engine = null;//new EntityTools.Core.Proxies.QuesterConditionProxy(@this);
            }

            @this = tmc;
            @this.PropertyChanged += PropertyChanged;

            _idStr = string.Concat(@this.GetType().Name, '[', @this.GetHashCode().ToString("X2"), ']');

            _label = string.Empty;
            memberCountChecker = Initicalize_MemberCountChecker;
            counter = Initialize_Counter;

            @this.Engine = this;

            return true;
        }

        public bool IsValid => EntityManager.LocalPlayer.PlayerTeam.IsInTeam && memberCountChecker(counter());

        public void Reset()
        {
            //membersCount = 0;
            _label = string.Empty;
        }

        public string TestInfos
        {
            get
            {
                var playerTeam = EntityManager.LocalPlayer.PlayerTeam;
                if (playerTeam.IsInTeam
                    && playerTeam.Team.MembersCount > 1)
                {
                    int memsCount = 0;
                    StringBuilder strBldr = new StringBuilder();
                    StringBuilder strBldr2 = new StringBuilder();
                    strBldr.AppendLine();

                    var regionInternalName = EntityManager.LocalPlayer.RegionInternalName;
                    var containerId = EntityManager.LocalPlayer.ContainerId;

                    foreach (TeamMember member in EntityManager.LocalPlayer.PlayerTeam.Team.Members)
                    {
                        var memberEntity = member.Entity;
                        var memberDistance = memberEntity.Location.Distance3DFromPlayer;
                        var memberRegion = memberEntity.RegionInternalName;

                        // if (member.InternalName != EntityManager.LocalPlayer.InternalName)
                        // Эквивалентно следующей строке:
                        if (memberEntity.ContainerId != containerId)
                        {
                            strBldr.AppendLine(member.InternalName);
                            strBldr.AppendFormat("\tDistance: {0:0.##}", memberDistance).AppendLine();

                            if (@this.RegionCheck)
                            {
                                strBldr.Append($"\tRegionCheck[").Append(memberRegion).Append("]: ");
                                if (memberRegion == regionInternalName)
                                    strBldr.AppendLine("succeeded");
                                else strBldr.AppendLine("fail");
                            }

                            strBldr2.Clear();
                            bool match = false;

                            var crSet = @this.CustomRegionNames;
                            if (crSet?.Count > 0)
                            {
                                foreach (var crEntry in crSet)
                                    if (memberEntity.Within(crEntry))
                                    {
                                        match = true;
                                        if (strBldr2.Length > 0)
                                            strBldr2.Append(", ");
                                        strBldr2.Append($"[{crEntry.Name}]");
                                    }
                                strBldr.Append("\tCustomRegions: ").Append(strBldr2).AppendLine();

                                switch (@this.DistanceSign)
                                {
                                    case Relation.Inferior:
                                        if (memberDistance < @this.Distance)
                                        {
                                            if (@this.CustomRegionCheck == Presence.Equal && match)
                                                memsCount++;
                                            if (@this.CustomRegionCheck == Presence.NotEquel && !match)
                                                memsCount++;
                                        }
                                        break;
                                    case Relation.Superior:
                                        if (memberDistance > @this.Distance)
                                        {
                                            if (@this.CustomRegionCheck == Presence.Equal && match)
                                                memsCount++;
                                            if (@this.CustomRegionCheck == Presence.NotEquel && !match)
                                                memsCount++;
                                        }
                                        break;
                                    case Relation.Equal:
                                        if (Math.Abs(memberDistance - @this.Distance) <= 1.0)
                                        {
                                            if (@this.CustomRegionCheck == Presence.Equal && match)
                                                memsCount++;
                                            if (@this.CustomRegionCheck == Presence.NotEquel && !match)
                                                memsCount++;
                                        }
                                        break;
                                    case Relation.NotEqual:
                                        if (Math.Abs(memberDistance - @this.Distance) > 1.0)
                                        {
                                            if (@this.CustomRegionCheck == Presence.Equal && match)
                                                memsCount++;
                                            if (@this.CustomRegionCheck == Presence.NotEquel && !match)
                                                memsCount++;
                                        }
                                        break;
                                }
                            }
                            else
                            {
                                switch (@this.DistanceSign)
                                {
                                    case Relation.Inferior:
                                        if (memberDistance < @this.Distance)
                                            memsCount++;
                                        break;
                                    case Relation.Superior:
                                        if (memberDistance > @this.Distance)
                                            memsCount++;
                                        break;
                                    case Relation.Equal:
                                        if (Math.Abs(memberDistance - @this.Distance) <= 1.0)
                                            memsCount++;
                                        break;
                                    case Relation.NotEqual:
                                        if (Math.Abs(memberDistance - @this.Distance) > 1.0)
                                            memsCount++;
                                        break;
                                }

                            }
                        }
                    }
                    return strBldr.Insert(0, $"Total {memsCount} Team members matches to the conditions.").ToString();
                }
                else
                {
                    return "Player is not in a party";
                }
            }
        }

        public string Label()
        {
            if(string.IsNullOrEmpty(_label))
                _label = $"{@this.GetType().Name} {@this.Sign} to {@this.MemberCount}";
            return _label;
        }
        
        #region MemberCounters
        /// <summary>
        /// Cчетчик <seealso cref="TeamMember"/>, удовлетворяющих условиям команды <seealso cref="TeamMembersCount"/>
        /// </summary>
        Func<uint> counter;
        private uint Initialize_Counter()
        {
            if (@this.CustomRegionNames?.Count > 0)
            {
                switch (@this.DistanceSign)
                {
                    case Relation.Inferior:
                        if (@this.RegionCheck)
                            if (@this.CustomRegionCheck == Presence.Equal)
                                counter = CountMembers_UnderConditions_DistanceInferior_RegionCheck_WithinCustomRegion;
                            else counter = CountMembers_UnderConditions_DistanceInferior_RegionCheck_OutsideCustomRegion;
                        else if (@this.CustomRegionCheck == Presence.Equal)
                            counter = CountMembers_UnderConditions_DistanceInferior_WithinCustomRegion;
                        else counter = CountMembers_UnderConditions_DistanceInferior_OutsideCustomRegion;
                        break;
                    case Relation.Superior:
                        if (@this.RegionCheck)
                            if (@this.CustomRegionCheck == Presence.Equal)
                                counter = CountMembers_UnderConditions_DistanceSuperior_RegionCheck_WithinCustomRegion;
                            else counter = CountMembers_UnderConditions_DistanceSuperior_RegionCheck_OutsideCustomRegion;
                        else if (@this.CustomRegionCheck == Presence.Equal)
                            counter = CountMembers_UnderConditions_DistanceSuperior_WithinCustomRegion;
                        else counter = CountMembers_UnderConditions_DistanceSuperior_OutsideCustomRegion;
                        break;
                    case Relation.Equal:
                        if (@this.RegionCheck)
                            if (@this.CustomRegionCheck == Presence.Equal)
                                counter = CountMembers_UnderConditions_DistanceEqual_RegionCheck_WithinCustomRegion;
                            else counter = CountMembers_UnderConditions_DistanceEqual_RegionCheck_OutsideCustomRegion;
                        else if (@this.CustomRegionCheck == Presence.Equal)
                            counter = CountMembers_UnderConditions_DistanceEqual_WithinCustomRegion;
                        else counter = CountMembers_UnderConditions_DistanceEqual_OutsideCustomRegion;
                        break;
                    case Relation.NotEqual:
                        if (@this.RegionCheck)
                            if (@this.CustomRegionCheck == Presence.Equal)
                                counter = CountMembers_UnderConditions_DistanceNotEqual_RegionCheck_WithinCustomRegion;
                            else counter = CountMembers_UnderConditions_DistanceNotEqual_RegionCheck_OutsideCustomRegion;
                        else if (@this.CustomRegionCheck == Presence.Equal)
                            counter = CountMembers_UnderConditions_DistanceNotEqual_WithinCustomRegion;
                        else counter = CountMembers_UnderConditions_DistanceNotEqual_OutsideCustomRegion;
                        break;
                }
            }
            else
            {
                switch (@this.DistanceSign)
                {
                    case Relation.Inferior:
                        if (@this.RegionCheck)
                            counter = CountMembers_UnderConditions_DistanceInferior_RegionCheck;
                        else counter = CountMembers_UnderConditions_DistanceInferior;
                        break;
                    case Relation.Superior:
                        if (@this.RegionCheck)
                            counter = CountMembers_UnderConditions_DistanceSuperior_RegionCheck;
                        else counter = CountMembers_UnderConditions_DistanceSuperior;
                        break;
                    case Relation.Equal:
                        if (@this.RegionCheck)
                            counter = CountMembers_UnderConditions_DistanceEqual_RegionCheck;
                        else counter = CountMembers_UnderConditions_DistanceEqual;
                        break;
                    case Relation.NotEqual:
                        if (@this.RegionCheck)
                            counter = CountMembers_UnderConditions_DistanceNotEqual_RegionCheck;
                        else counter = CountMembers_UnderConditions_DistanceNotEqual;
                        break;
                }
            }
            return counter();
        }

        private uint CountMembers_UnderConditions_DistanceInferior_RegionCheck_WithinCustomRegion()
        {
            var player = EntityManager.LocalPlayer;
            var playerTeam = player.PlayerTeam;
            if (!playerTeam.IsInTeam)
                return 0;
            uint num = 0;
            var playerContainerId = player.ContainerId;
            var regName = player.RegionInternalName;
            var playerPos = player.Location;
            var sqDist = @this.Distance;
            sqDist *= sqDist;
            var crSet = @this.CustomRegionNames;
            foreach (var member in playerTeam.Team.Members)
            {
                var memberEntity = member.Entity;
                if( memberEntity.ContainerId != playerContainerId
                    && MapperHelper.SquareDistance3D(memberEntity.Location, playerPos) < sqDist
                    && memberEntity.RegionInternalName.Equals(regName, StringComparison.Ordinal)
                    && crSet.Within(memberEntity))
                    num++;
            }
            return num;
        }
        private uint CountMembers_UnderConditions_DistanceInferior_RegionCheck_OutsideCustomRegion()
        {
            var player = EntityManager.LocalPlayer;
            var playerTeam = player.PlayerTeam;
            if (!playerTeam.IsInTeam)
                return 0;
            uint num = 0;
            var playerContainerId = player.ContainerId;
            var regName = player.RegionInternalName;
            var playerPos = player.Location;
            var sqDist = @this.Distance;
            sqDist *= sqDist;
            var crSet = @this.CustomRegionNames;
            foreach (var member in playerTeam.Team.Members)
            {
                var memberEntity = member.Entity;
                if (memberEntity.ContainerId != playerContainerId
                    && MapperHelper.SquareDistance3D(memberEntity.Location, playerPos) < sqDist
                    && memberEntity.RegionInternalName.Equals(regName, StringComparison.Ordinal)
                    && crSet.Outside(memberEntity))
                    num++;
            }
            return num;
        }
        private uint CountMembers_UnderConditions_DistanceInferior_WithinCustomRegion()
        {
            var player = EntityManager.LocalPlayer;
            var playerTeam = player.PlayerTeam;
            if (!playerTeam.IsInTeam)
                return 0;
            uint num = 0;
            var playerContainerId = player.ContainerId;
            var playerPos = player.Location;
            var sqDist = @this.Distance;
            sqDist *= sqDist;
            var crSet = @this.CustomRegionNames;
            foreach (var member in playerTeam.Team.Members)
            {
                var memberEntity = member.Entity;
                if (memberEntity.ContainerId != playerContainerId
                    && MapperHelper.SquareDistance3D(memberEntity.Location, playerPos) < sqDist
                    && crSet.Within(memberEntity))
                    num++;
            }
            return num;
        }
        private uint CountMembers_UnderConditions_DistanceInferior_OutsideCustomRegion()
        {
            var player = EntityManager.LocalPlayer;
            var playerTeam = player.PlayerTeam;
            if (!playerTeam.IsInTeam)
                return 0;
            uint num = 0;
            var playerContainerId = player.ContainerId;
            var playerPos = player.Location;
            var sqDist = @this.Distance;
            sqDist *= sqDist;
            var crSet = @this.CustomRegionNames;
            foreach (var member in playerTeam.Team.Members)
            {
                var memberEntity = member.Entity;
                if (memberEntity.ContainerId != playerContainerId
                    && MapperHelper.SquareDistance3D(memberEntity.Location, playerPos) < sqDist
                    && crSet.Outside(memberEntity))
                    num++;
            }
            return num;
        }
        private uint CountMembers_UnderConditions_DistanceInferior_RegionCheck()
        {
            var player = EntityManager.LocalPlayer;
            var playerTeam = player.PlayerTeam;
            if (!playerTeam.IsInTeam)
                return 0;
            uint num = 0;
            var playerContainerId = player.ContainerId;
            var regName = player.RegionInternalName;
            var playerPos = player.Location;
            var sqDist = @this.Distance;
            sqDist *= sqDist;
            foreach (var member in playerTeam.Team.Members)
            {
                var memberEntity = member.Entity;
                if (memberEntity.ContainerId != playerContainerId
                    && MapperHelper.SquareDistance3D(memberEntity.Location, playerPos) < sqDist
                    && memberEntity.RegionInternalName.Equals(regName, StringComparison.Ordinal))
                    num++;
            }

            return num;
        }
        private uint CountMembers_UnderConditions_DistanceInferior()
        {
            var player = EntityManager.LocalPlayer;
            var playerTeam = player.PlayerTeam;
            if (!playerTeam.IsInTeam)
                return 0;
            uint num = 0;
            var playerContainerId = player.ContainerId;
            var playerPos = player.Location;
            var sqDist = @this.Distance;
            sqDist *= sqDist;
            foreach (var member in playerTeam.Team.Members)
            {
                var memberEntity = member.Entity;
                if (memberEntity.ContainerId != playerContainerId
                    && MapperHelper.SquareDistance3D(memberEntity.Location, playerPos) < sqDist)
                    num++;
            }
            return num;
        }

        private uint CountMembers_UnderConditions_DistanceSuperior_RegionCheck_WithinCustomRegion()
        {
            var player = EntityManager.LocalPlayer;
            var playerTeam = player.PlayerTeam;
            if (!playerTeam.IsInTeam)
                return 0;
            uint num = 0;
            var playerContainerId = player.ContainerId;
            var playerPos = player.Location;
            var regName = player.RegionInternalName;
            var dist = @this.Distance;
            var crSet = @this.CustomRegionNames;
            foreach (var member in playerTeam.Team.Members)
            {
                var memberEntity = member.Entity;
                
                if (memberEntity.ContainerId != playerContainerId
                    && MapperHelper.SquareDistance3D(memberEntity.Location, playerPos) > dist
                    && memberEntity.RegionInternalName.Equals(regName, StringComparison.Ordinal)
                    && crSet.Within(memberEntity))
                    num++;
            }
            return num;
        }
        private uint CountMembers_UnderConditions_DistanceSuperior_RegionCheck_OutsideCustomRegion()
        {
            var player = EntityManager.LocalPlayer;
            var playerTeam = player.PlayerTeam;
            if (!playerTeam.IsInTeam)
                return 0;
            uint num = 0;
            var playerContainerId = player.ContainerId;
            var regName = player.RegionInternalName;
            var playerPos = player.Location;
            var sqDist = @this.Distance;
            sqDist *= sqDist;
            var crSet = @this.CustomRegionNames;
            foreach (var member in playerTeam.Team.Members)
            {
                var memberEntity = member.Entity;
                if (memberEntity.ContainerId != playerContainerId
                    && MapperHelper.SquareDistance3D(memberEntity.Location, playerPos) > sqDist
                    && memberEntity.RegionInternalName.Equals(regName, StringComparison.Ordinal)
                    && crSet.Outside(memberEntity))
                    num++;
            }
            return num;
        }
        private uint CountMembers_UnderConditions_DistanceSuperior_WithinCustomRegion()
        {
            var player = EntityManager.LocalPlayer;
            var playerTeam = player.PlayerTeam;
            if (!playerTeam.IsInTeam)
                return 0;
            uint num = 0;
            var playerContainerId = player.ContainerId;
            var playerPos = player.Location;
            var sqDist = @this.Distance;
            sqDist *= sqDist;
            var crSet = @this.CustomRegionNames;
            foreach (var member in playerTeam.Team.Members)
            {
                var memberEntity = member.Entity;
                if (memberEntity.ContainerId != playerContainerId
                    && MapperHelper.SquareDistance3D(memberEntity.Location, playerPos) > sqDist
                    && crSet.Within(memberEntity))
                    num++;
            }
            return num;
        }
        private uint CountMembers_UnderConditions_DistanceSuperior_OutsideCustomRegion()
        {
            var player = EntityManager.LocalPlayer;
            var playerTeam = player.PlayerTeam;
            if (!playerTeam.IsInTeam)
                return 0;
            uint num = 0;
            var playerContainerId = player.ContainerId;
            var playerPos = player.Location;
            var sqDist = @this.Distance;
            sqDist *= sqDist;
            var crSet = @this.CustomRegionNames;
            foreach (var member in playerTeam.Team.Members)
            {
                var memberEntity = member.Entity;
                if (memberEntity.ContainerId != playerContainerId
                    && MapperHelper.SquareDistance3D(memberEntity.Location, playerPos) > sqDist
                    && crSet.Outside(memberEntity))
                    num++;
            }
            return num;
        }
        private uint CountMembers_UnderConditions_DistanceSuperior_RegionCheck()
        {
            var player = EntityManager.LocalPlayer;
            var playerTeam = player.PlayerTeam;
            if (!playerTeam.IsInTeam)
                return 0;
            uint num = 0;
            var playerContainerId = player.ContainerId;
            var regName = player.RegionInternalName;
            var playerPos = player.Location;
            var sqDist = @this.Distance;
            sqDist *= sqDist;
            foreach (var member in playerTeam.Team.Members)
            {
                var memberEntity = member.Entity;
                if (memberEntity.ContainerId != playerContainerId
                    && MapperHelper.SquareDistance3D(memberEntity.Location, playerPos) > sqDist
                    && memberEntity.RegionInternalName.Equals(regName, StringComparison.Ordinal))
                    num++;
            }
            return num;
        }
        private uint CountMembers_UnderConditions_DistanceSuperior()
        {
            var player = EntityManager.LocalPlayer;
            var playerTeam = player.PlayerTeam;
            if (!playerTeam.IsInTeam)
                return 0;
            uint num = 0;
            var playerContainerId = player.ContainerId;
            var playerPos = player.Location;
            var sqDist = @this.Distance;
            sqDist *= sqDist;
            foreach (var member in playerTeam.Team.Members)
            {
                var memberEntity = member.Entity;
                if (memberEntity.ContainerId != playerContainerId
                    && MapperHelper.SquareDistance3D(memberEntity.Location, playerPos) > sqDist)
                    num++;
            }
            return num;
        }

        private uint CountMembers_UnderConditions_DistanceEqual_RegionCheck_WithinCustomRegion()
        {
            var player = EntityManager.LocalPlayer;
            var playerTeam = player.PlayerTeam;
            if (!playerTeam.IsInTeam)
                return 0;
            uint num = 0;
            var playerContainerId = player.ContainerId;
            var regName = player.RegionInternalName;
            var playerPos = player.Location;
            var sqDist = @this.Distance;
            sqDist *= sqDist;
            var crSet = @this.CustomRegionNames;
            foreach (var member in playerTeam.Team.Members)
            {
                var memberEntity = member.Entity;
                if (memberEntity.ContainerId != playerContainerId
                    && Math.Abs(MapperHelper.SquareDistance3D(memberEntity.Location, playerPos) - sqDist) <= 1
                    && memberEntity.RegionInternalName.Equals(regName, StringComparison.Ordinal)
                    && crSet.Within(memberEntity))
                    num++;
            }
            return num;
        }
        private uint CountMembers_UnderConditions_DistanceEqual_RegionCheck_OutsideCustomRegion()
        {
            var player = EntityManager.LocalPlayer;
            var playerTeam = player.PlayerTeam;
            if (!playerTeam.IsInTeam)
                return 0;
            uint num = 0;
            var playerContainerId = player.ContainerId;
            var regName = player.RegionInternalName;
            var playerPos = player.Location;
            var sqDist = @this.Distance;
            sqDist *= sqDist;
            var crSet = @this.CustomRegionNames;
            foreach (var member in playerTeam.Team.Members)
            {
                var memberEntity = member.Entity;
                if (memberEntity.ContainerId != playerContainerId
                    && Math.Abs(MapperHelper.SquareDistance3D(memberEntity.Location, playerPos) - sqDist) <= 1
                    && memberEntity.RegionInternalName.Equals(regName, StringComparison.Ordinal)
                    && crSet.Outside(memberEntity))
                    num++;
            }
            return num;
        }
        private uint CountMembers_UnderConditions_DistanceEqual_WithinCustomRegion()
        {
            var player = EntityManager.LocalPlayer;
            var playerTeam = player.PlayerTeam;
            if (!playerTeam.IsInTeam)
                return 0;
            uint num = 0;
            var playerContainerId = player.ContainerId;
            var playerPos = player.Location;
            var sqDist = @this.Distance;
            sqDist *= sqDist;
            var crSet = @this.CustomRegionNames;
            foreach (var member in playerTeam.Team.Members)
            {
                var memberEntity = member.Entity;
                if (memberEntity.ContainerId != playerContainerId
                    && Math.Abs(MapperHelper.SquareDistance3D(memberEntity.Location, playerPos) - sqDist) <= 1
                    && crSet.Within(memberEntity))
                    num++;
            }
            return num;
        }
        private uint CountMembers_UnderConditions_DistanceEqual_OutsideCustomRegion()
        {
            var player = EntityManager.LocalPlayer;
            var playerTeam = player.PlayerTeam;
            if (!playerTeam.IsInTeam)
                return 0;
            uint num = 0;
            var playerContainerId = player.ContainerId;
            var playerPos = player.Location;
            var sqDist = @this.Distance;
            sqDist *= sqDist;
            var crSet = @this.CustomRegionNames;
            foreach (var member in playerTeam.Team.Members)
            {
                var memberEntity = member.Entity;
                if (memberEntity.ContainerId != playerContainerId
                    && Math.Abs(MapperHelper.SquareDistance3D(memberEntity.Location, playerPos) - sqDist) <= 1
                    && crSet.Outside(memberEntity))
                    num++;
            }
            return num;
        }
        private uint CountMembers_UnderConditions_DistanceEqual_RegionCheck()
        {
            var player = EntityManager.LocalPlayer;
            var playerTeam = player.PlayerTeam;
            if (!playerTeam.IsInTeam)
                return 0;
            uint num = 0;
            var playerContainerId = player.ContainerId;
            var regName = player.RegionInternalName;
            var playerPos = player.Location;
            var sqDist = @this.Distance;
            sqDist *= sqDist;
            foreach (var member in playerTeam.Team.Members)
            {
                var memberEntity = member.Entity;
                if (memberEntity.ContainerId != playerContainerId
                    && Math.Abs(MapperHelper.SquareDistance3D(memberEntity.Location, playerPos) - sqDist) <= 1
                    && memberEntity.RegionInternalName.Equals(regName, StringComparison.Ordinal))
                    num++;
            }
            return num;
        }
        private uint CountMembers_UnderConditions_DistanceEqual()
        {
            var player = EntityManager.LocalPlayer;
            var playerTeam = player.PlayerTeam;
            if (!playerTeam.IsInTeam)
                return 0;
            uint num = 0;
            var playerContainerId = player.ContainerId;
            var dist = @this.Distance;
            foreach (var member in playerTeam.Team.Members)
            {
                var memberEntity = member.Entity;
                if (memberEntity.ContainerId != playerContainerId
                    && Math.Abs(memberEntity.Location.Distance3DFromPlayer - dist) <= 1)
                    num++;
            }
            return num;
        }

        private uint CountMembers_UnderConditions_DistanceNotEqual_RegionCheck_WithinCustomRegion()
        {
            var player = EntityManager.LocalPlayer;
            var playerTeam = player.PlayerTeam;
            if (!playerTeam.IsInTeam)
                return 0;
            uint num = 0;
            var playerContainerId = player.ContainerId;
            var regName = player.RegionInternalName;
            var playerPos = player.Location;
            var sqDist = @this.Distance;
            sqDist *= sqDist;
            var crSet = @this.CustomRegionNames;
            foreach (var member in playerTeam.Team.Members)
            {
                var memberEntity = member.Entity;
                if (memberEntity.ContainerId != playerContainerId
                    && Math.Abs(MapperHelper.SquareDistance3D(memberEntity.Location, playerPos) - sqDist) > 1
                    && memberEntity.RegionInternalName.Equals(regName, StringComparison.Ordinal)
                    && crSet.Within(memberEntity))
                    num++;
            }
            return num;
        }
        private uint CountMembers_UnderConditions_DistanceNotEqual_RegionCheck_OutsideCustomRegion()
        {
            var player = EntityManager.LocalPlayer;
            var playerTeam = player.PlayerTeam;
            if (!playerTeam.IsInTeam)
                return 0;
            uint num = 0;
            var playerContainerId = player.ContainerId;
            var regName = player.RegionInternalName;
            var playerPos = player.Location;
            var sqDist = @this.Distance;
            sqDist *= sqDist;
            var crSet = @this.CustomRegionNames;
            foreach (var member in playerTeam.Team.Members)
            {
                var memberEntity = member.Entity;
                if (memberEntity.ContainerId != playerContainerId
                    && Math.Abs(MapperHelper.SquareDistance3D(memberEntity.Location, playerPos) - sqDist) > 1
                    && memberEntity.RegionInternalName.Equals(regName, StringComparison.Ordinal)
                    && crSet.Outside(memberEntity))
                    num++;
            }
            return num;
        }
        private uint CountMembers_UnderConditions_DistanceNotEqual_WithinCustomRegion()
        {
            var player = EntityManager.LocalPlayer;
            var playerTeam = player.PlayerTeam;
            if (!playerTeam.IsInTeam)
                return 0;
            uint num = 0;
            var playerContainerId = player.ContainerId;
            var playerPos = player.Location;
            var sqDist = @this.Distance;
            sqDist *= sqDist;
            var crSet = @this.CustomRegionNames;
            foreach (var member in playerTeam.Team.Members)
            {
                var memberEntity = member.Entity;
                if (memberEntity.ContainerId != playerContainerId
                    && Math.Abs(MapperHelper.SquareDistance3D(memberEntity.Location, playerPos) - sqDist) > 1
                    && crSet.Within(memberEntity))
                    num++;
            }
            return num;
        }
        private uint CountMembers_UnderConditions_DistanceNotEqual_OutsideCustomRegion()
        {
            var player = EntityManager.LocalPlayer;
            var playerTeam = player.PlayerTeam;
            if (!playerTeam.IsInTeam)
                return 0;
            uint num = 0;
            var playerContainerId = player.ContainerId;
            var playerPos = player.Location;
            var sqDist = @this.Distance;
            sqDist *= sqDist;
            var crSet = @this.CustomRegionNames;
            foreach (var member in playerTeam.Team.Members)
            {
                var memberEntity = member.Entity;
                if (memberEntity.ContainerId != playerContainerId
                    && Math.Abs(MapperHelper.SquareDistance3D(memberEntity.Location, playerPos) - sqDist) > 1
                    && crSet.Outside(memberEntity))
                    num++;
            }
            return num;
        }
        private uint CountMembers_UnderConditions_DistanceNotEqual_RegionCheck()
        {
            var player = EntityManager.LocalPlayer;
            var playerTeam = player.PlayerTeam;
            if (!playerTeam.IsInTeam)
                return 0;
            uint num = 0;
            var playerContainerId = player.ContainerId;
            var regName = player.RegionInternalName;
            var playerPos = player.Location;
            var sqDist = @this.Distance;
            sqDist *= sqDist;
            foreach (var member in playerTeam.Team.Members)
            {
                var memberEntity = member.Entity;
                if (memberEntity.ContainerId != playerContainerId
                    && Math.Abs(MapperHelper.SquareDistance3D(memberEntity.Location, playerPos) - sqDist) > 1
                    && memberEntity.RegionInternalName.Equals(regName, StringComparison.Ordinal))
                    num++;
            }
            return num;
        }
        private uint CountMembers_UnderConditions_DistanceNotEqual()
        {
            var player = EntityManager.LocalPlayer;
            var playerTeam = player.PlayerTeam;
            if (!playerTeam.IsInTeam)
                return 0;
            uint num = 0;
            var playerContainerId = player.ContainerId;
            var playerPos = player.Location;
            var sqDist = @this.Distance;
            sqDist *= sqDist;
            foreach (var member in playerTeam.Team.Members)
            {
                var memberEntity = member.Entity;
                if (memberEntity.ContainerId != playerContainerId
                    && Math.Abs(MapperHelper.SquareDistance3D(memberEntity.Location, playerPos) - sqDist) > 1)
                    num++;
            }
            return num;
        }
        #endregion

        #region MemberCountChecker
        /// <summary>
        /// Предикат, проверяющий истинность соотношения <seealso cref="TeamMembersCount.Sign"/> 
        /// между подсчитанным количеством <seealso cref="TeamMember"/>, удовлетворяющих улосвиям <seealso cref="TeamMembersCount"/>
        /// и заданным значеним <seealso cref="TeamMembersCount.MemberCount"/>
        /// </summary>
        Predicate<uint> memberCountChecker;

        private bool Initicalize_MemberCountChecker(uint count)
        {
            switch (@this.Sign)
            {
                case Relation.Inferior:
                    memberCountChecker = Inferior_Than_MemberCount;
                    break;
                case Relation.Superior:
                    memberCountChecker = Superior_Than_MemberCount;
                    break;
                case Relation.Equal:
                    memberCountChecker = Equal_To_MemberCount;
                    break;
                case Relation.NotEqual:
                    memberCountChecker = NotEqual_To_MemberCount;
                    break;
            }
            return memberCountChecker(count);
        }
        private bool Inferior_Than_MemberCount(uint count)
        {
            // Если персонаж не состоит в группе, то "count == 0"
            // В этом случае условие "Count Inferior N" будет истинно при любом N > 0
            return count < @this.MemberCount;
        }
        private bool Superior_Than_MemberCount(uint count)
        {
            return count > @this.MemberCount;
        }
        private bool Equal_To_MemberCount(uint count)
        {
            // Если персонаж не состоит в группе, то "count == 0"
            // В этом случае условие "Count Equal 0" будет истинно
            return count == @this.MemberCount;
        }
        private bool NotEqual_To_MemberCount(uint count)
        {
            // Если персонаж не состоит в группе, то "count == 0"
            // В этом случае условие "Count NotEqual N" будет истинно при N > 0
            return count != @this.MemberCount;
        } 
        #endregion
    }
}
