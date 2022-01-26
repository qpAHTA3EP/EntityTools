using Astral.Quester.Classes;
using EntityCore.Extensions;
using EntityTools;
using EntityTools.Extensions;
using EntityTools.Quester.Conditions;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityTools.Core.Interfaces;
using static Astral.Quester.Classes.Condition;

namespace EntityCore.Quester.Conditions
{
    class TeamMembersCountEngine : IQuesterConditionEngine
    {
        private TeamMembersCount @this = null;

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

        public bool IsValid
        {
            get
            {
                return memberCountChecker(counter());
            }
        }

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

                            if (@this._regionCheck)
                            {
                                strBldr.Append($"\tRegionCheck[").Append(memberRegion).Append("]: ");
                                if (memberRegion == regionInternalName)
                                    strBldr.AppendLine("succeeded");
                                else strBldr.AppendLine("fail");
                            }

                            strBldr2.Clear();
                            bool match = false;

                            if (@this._customRegionNames.Count > 0)
                            {
                                foreach (var crEntry in @this._customRegionNames)
                                    if (memberEntity.Within(crEntry))
                                    {
                                        match = true;
                                        if (strBldr2.Length > 0)
                                            strBldr2.Append(", ");
                                        strBldr2.Append($"[{crEntry.Name}]");
                                    }
                                strBldr.Append("\tCustomRegions: ").Append(strBldr2).AppendLine();

                                switch (@this._distanceSign)
                                {
                                    case Relation.Inferior:
                                        if (memberDistance < @this._distance)
                                        {
                                            if (@this._customRegionCheck == Presence.Equal && match)
                                                memsCount++;
                                            if (@this._customRegionCheck == Presence.NotEquel && !match)
                                                memsCount++;
                                        }
                                        break;
                                    case Relation.Superior:
                                        if (memberDistance > @this._distance)
                                        {
                                            if (@this._customRegionCheck == Presence.Equal && match)
                                                memsCount++;
                                            if (@this._customRegionCheck == Presence.NotEquel && !match)
                                                memsCount++;
                                        }
                                        break;
                                    case Relation.Equal:
                                        if (Math.Abs(memberDistance - @this._distance) <= 1.0)
                                        {
                                            if (@this._customRegionCheck == Presence.Equal && match)
                                                memsCount++;
                                            if (@this._customRegionCheck == Presence.NotEquel && !match)
                                                memsCount++;
                                        }
                                        break;
                                    case Relation.NotEqual:
                                        if (Math.Abs(memberDistance - @this._distance) > 1.0)
                                        {
                                            if (@this._customRegionCheck == Presence.Equal && match)
                                                memsCount++;
                                            if (@this._customRegionCheck == Presence.NotEquel && !match)
                                                memsCount++;
                                        }
                                        break;
                                }
                            }
                            else
                            {
                                switch (@this._distanceSign)
                                {
                                    case Relation.Inferior:
                                        if (memberDistance < @this._distance)
                                            memsCount++;
                                        break;
                                    case Relation.Superior:
                                        if (memberDistance > @this._distance)
                                            memsCount++;
                                        break;
                                    case Relation.Equal:
                                        if (Math.Abs(memberDistance - @this._distance) <= 1.0)
                                            memsCount++;
                                        break;
                                    case Relation.NotEqual:
                                        if (Math.Abs(memberDistance - @this._distance) > 1.0)
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
                _label = $"{@this.GetType().Name} {@this._sign} to {@this._memberCount}";
            return _label;
        }
        
        #region TeamMemberCheckers
        private bool DistanceInferior_RegionCheck_WithinCustomRegion(TeamMember member)
        {
            var player = EntityManager.LocalPlayer;
            var memberEntity = member.Entity;

            return memberEntity.ContainerId != player.ContainerId
                    && memberEntity.Location.Distance3D(player.Location) < @this._distance
                    && memberEntity.RegionInternalName == player.RegionInternalName
                    && @this._customRegionNames.Within(memberEntity);
        }
        private bool DistanceInferior_RegionCheck_OutsideCustomRegion(TeamMember member)
        {
            var player = EntityManager.LocalPlayer;
            var memberEntity = member.Entity;

            return memberEntity.ContainerId != player.ContainerId
                    && memberEntity.Location.Distance3D(player.Location) < @this._distance
                    && memberEntity.RegionInternalName == player.RegionInternalName
                    && @this._customRegionNames.Outside(memberEntity);
        }
        private bool DistanceInferior_WithinCustomRegion(TeamMember member)
        {
            var player = EntityManager.LocalPlayer;
            var memberEntity = member.Entity;

            return memberEntity.ContainerId != player.ContainerId
                    && memberEntity.Location.Distance3D(player.Location) < @this._distance
                    && @this._customRegionNames.Within(memberEntity);
        }
        private bool DistanceInferior_OutsideCustomRegion(TeamMember member)
        {
            var player = EntityManager.LocalPlayer;
            var memberEntity = member.Entity;

            return memberEntity.ContainerId != player.ContainerId
                    && memberEntity.Location.Distance3D(player.Location) < @this._distance
                    && @this._customRegionNames.Outside(memberEntity);
        }
        private bool DistanceInferior_RegionCheck(TeamMember member)
        {
            var player = EntityManager.LocalPlayer;
            var memberEntity = member.Entity;

            return memberEntity.ContainerId != player.ContainerId
                    && memberEntity.Location.Distance3D(player.Location) < @this._distance
                    && memberEntity.RegionInternalName == player.RegionInternalName;
        }
        private bool DistanceInferior(TeamMember member)
        {
            var player = EntityManager.LocalPlayer;
            var memberEntity = member.Entity;

            return memberEntity.ContainerId != player.ContainerId
                    && memberEntity.Location.Distance3D(player.Location) < @this._distance;
        }

        private bool DistanceSuperior_RegionCheck_WithinCustomRegion(TeamMember member)
        {
            var player = EntityManager.LocalPlayer;
            var memberEntity = member.Entity;

            return memberEntity.ContainerId != player.ContainerId
                    && memberEntity.Location.Distance3D(player.Location) > @this._distance
                    && memberEntity.RegionInternalName == player.RegionInternalName
                    && @this._customRegionNames.Within(memberEntity);
        }
        private bool DistanceSuperior_RegionCheck_OutsideCustomRegion(TeamMember member)
        {
            var player = EntityManager.LocalPlayer;
            var memberEntity = member.Entity;

            return memberEntity.ContainerId != player.ContainerId
                    && memberEntity.Location.Distance3D(player.Location) > @this._distance
                    && memberEntity.RegionInternalName == player.RegionInternalName
                    && @this._customRegionNames.Outside(memberEntity);
        }
        private bool DistanceSuperior_WithinCustomRegion(TeamMember member)
        {
            var player = EntityManager.LocalPlayer;
            var memberEntity = member.Entity;

            return memberEntity.ContainerId != player.ContainerId
                    && memberEntity.Location.Distance3D(player.Location) > @this._distance
                    && @this._customRegionNames.Within(memberEntity);
        }
        private bool DistanceSuperior_OutsideCustomRegion(TeamMember member)
        {
            var player = EntityManager.LocalPlayer;
            var memberEntity = member.Entity;

            return memberEntity.ContainerId != player.ContainerId
                    && memberEntity.Location.Distance3D(player.Location) > @this._distance
                    && @this._customRegionNames.Outside(memberEntity);
        }
        private bool DistanceSuperior_RegionCheck(TeamMember member)
        {
            var player = EntityManager.LocalPlayer;
            var memberEntity = member.Entity;

            return memberEntity.ContainerId != player.ContainerId
                    && memberEntity.Location.Distance3D(player.Location) > @this._distance
                    && memberEntity.RegionInternalName == player.RegionInternalName;
        }
        private bool DistanceSuperior(TeamMember member)
        {
            var player = EntityManager.LocalPlayer;
            var memberEntity = member.Entity;

            return memberEntity.ContainerId != player.ContainerId
                    && memberEntity.Location.Distance3D(player.Location) > @this._distance;
        }

        private bool DistanceEqual_RegionCheck_WithinCustomRegion(TeamMember member)
        {
            var player = EntityManager.LocalPlayer;
            var memberEntity = member.Entity;

            return memberEntity.ContainerId != player.ContainerId
                    && Math.Abs(memberEntity.Location.Distance3D(player.Location) - @this._distance) <= 1
                    && memberEntity.RegionInternalName == player.RegionInternalName
                    && @this._customRegionNames.Within(memberEntity);
        }
        private bool DistanceEqual_RegionCheck_OutsideCustomRegion(TeamMember member)
        {
            var player = EntityManager.LocalPlayer;
            var memberEntity = member.Entity;

            return memberEntity.ContainerId != player.ContainerId
                    && Math.Abs(memberEntity.Location.Distance3D(player.Location) - @this._distance) <= 1
                    && memberEntity.RegionInternalName == player.RegionInternalName
                    && @this._customRegionNames.Outside(memberEntity);
        }
        private bool DistanceEqual_WithinCustomRegion(TeamMember member)
        {
            var player = EntityManager.LocalPlayer;
            var memberEntity = member.Entity;

            return memberEntity.ContainerId != player.ContainerId
                    && Math.Abs(memberEntity.Location.Distance3D(player.Location) - @this._distance) <= 1
                    && @this._customRegionNames.Within(memberEntity);
        }
        private bool DistanceEqual_OutsideCustomRegion(TeamMember member)
        {
            var player = EntityManager.LocalPlayer;
            var memberEntity = member.Entity;

            return memberEntity.ContainerId != player.ContainerId
                    && Math.Abs(memberEntity.Location.Distance3D(player.Location) - @this._distance) <= 1
                    && @this._customRegionNames.Outside(memberEntity);
        }
        private bool DistanceEqual_RegionCheck(TeamMember member)
        {
            var player = EntityManager.LocalPlayer;
            var memberEntity = member.Entity;

            return memberEntity.ContainerId != player.ContainerId
                    && Math.Abs(memberEntity.Location.Distance3D(player.Location) - @this._distance) <= 1
                    && memberEntity.RegionInternalName == player.RegionInternalName;
        }
        private bool DistanceEqual(TeamMember member)
        {
            var player = EntityManager.LocalPlayer;
            var memberEntity = member.Entity;

            return memberEntity.ContainerId != player.ContainerId
                    && Math.Abs(memberEntity.Location.Distance3D(player.Location) - @this._distance) <= 1;
        }

        private bool DistanceNotEqual_RegionCheck_WithinCustomRegion(TeamMember member)
        {
            var player = EntityManager.LocalPlayer;
            var memberEntity = member.Entity;

            return memberEntity.ContainerId != player.ContainerId
                    && Math.Abs(memberEntity.Location.Distance3D(player.Location) - @this._distance) > 1
                    && memberEntity.RegionInternalName == player.RegionInternalName
                    && @this._customRegionNames.Within(memberEntity);
        }
        private bool DistanceNotEqual_RegionCheck_OutsideCustomRegion(TeamMember member)
        {
            var player = EntityManager.LocalPlayer;
            var memberEntity = member.Entity;

            return memberEntity.ContainerId != player.ContainerId
                    && Math.Abs(memberEntity.Location.Distance3D(player.Location) - @this._distance) > 1
                    && memberEntity.RegionInternalName == player.RegionInternalName
                    && @this._customRegionNames.Outside(memberEntity);
        }
        private bool DistanceNotEqual_WithinCustomRegion(TeamMember member)
        {
            var player = EntityManager.LocalPlayer;
            var memberEntity = member.Entity;

            return memberEntity.ContainerId != player.ContainerId
                    && Math.Abs(memberEntity.Location.Distance3D(player.Location) - @this._distance) > 1
                    && @this._customRegionNames.Within(memberEntity);
        }
        private bool DistanceNotEqual_OutsideCustomRegion(TeamMember member)
        {
            var player = EntityManager.LocalPlayer;
            var memberEntity = member.Entity;

            return memberEntity.ContainerId != player.ContainerId
                    && Math.Abs(memberEntity.Location.Distance3D(player.Location) - @this._distance) > 1
                    && @this._customRegionNames.Outside(memberEntity);
        }
        private bool DistanceNotEqual_RegionCheck(TeamMember member)
        {
            var player = EntityManager.LocalPlayer;
            var memberEntity = member.Entity;

            return memberEntity.ContainerId != player.ContainerId
                    && Math.Abs(memberEntity.Location.Distance3D(player.Location) - @this._distance) > 1
                    && memberEntity.RegionInternalName == player.RegionInternalName;
        }
        private bool DistanceNotEqual(TeamMember member)
        {
            var player = EntityManager.LocalPlayer;
            var memberEntity = member.Entity;

            return memberEntity.ContainerId != player.ContainerId
                    && Math.Abs(memberEntity.Location.Distance3D(player.Location) - @this._distance) > 1;
        }
        #endregion

        #region MemberCounters
        /// <summary>
        /// Cчетчик <seealso cref="TeamMember"/>, удовлетворяющих условиям команды <seealso cref="TeamMembersCount"/>
        /// </summary>
        Func<int> counter;
#if false
        Func<int> Counter
        {
            get
            {
                if (counter is null)
                {
                    if (@this._customRegionNames?.Count > 0)
                    {
                        switch (@this._distanceSign)
                        {
                            case Relation.Inferior:
                                if (@this._regionCheck)
                                    if (@this._customRegionCheck == Presence.Equal)
                                        counter = CountMembers_UnderConditions_DistanceInferior_RegionCheck_WithinCustomRegion;
                                    else counter = CountMembers_UnderConditions_DistanceInferior_RegionCheck_OutsideCustomRegion;
                                else if (@this._customRegionCheck == Presence.Equal)
                                    counter = CountMembers_UnderConditions_DistanceInferior_WithinCustomRegion;
                                else counter = CountMembers_UnderConditions_DistanceInferior_OutsideCustomRegion;
                                break;
                            case Relation.Superior:
                                if (@this._regionCheck)
                                    if (@this._customRegionCheck == Presence.Equal)
                                        counter = CountMembers_UnderConditions_DistanceSuperior_RegionCheck_WithinCustomRegion;
                                    else counter = CountMembers_UnderConditions_DistanceSuperior_RegionCheck_OutsideCustomRegion;
                                else if (@this._customRegionCheck == Presence.Equal)
                                    counter = CountMembers_UnderConditions_DistanceSuperior_WithinCustomRegion;
                                else counter = CountMembers_UnderConditions_DistanceSuperior_OutsideCustomRegion;
                                break;
                            case Relation.Equal:
                                if (@this._regionCheck)
                                    if (@this._customRegionCheck == Presence.Equal)
                                        counter = CountMembers_UnderConditions_DistanceEqual_RegionCheck_WithinCustomRegion;
                                    else counter = CountMembers_UnderConditions_DistanceEqual_RegionCheck_OutsideCustomRegion;
                                else if (@this._customRegionCheck == Presence.Equal)
                                    counter = CountMembers_UnderConditions_DistanceEqual_WithinCustomRegion;
                                else counter = CountMembers_UnderConditions_DistanceEqual_OutsideCustomRegion;
                                break;
                            case Relation.NotEqual:
                                if (@this._regionCheck)
                                    if (@this._customRegionCheck == Presence.Equal)
                                        counter = CountMembers_UnderConditions_DistanceNotEqual_RegionCheck_WithinCustomRegion;
                                    else counter = CountMembers_UnderConditions_DistanceNotEqual_RegionCheck_OutsideCustomRegion;
                                else if (@this._customRegionCheck == Presence.Equal)
                                    counter = CountMembers_UnderConditions_DistanceNotEqual_WithinCustomRegion;
                                else counter = CountMembers_UnderConditions_DistanceNotEqual_OutsideCustomRegion;
                                break;
                        }
                    }
                    else
                    {
                        switch (@this._distanceSign)
                        {
                            case Relation.Inferior:
                                if (@this._regionCheck)
                                    counter = CountMembers_UnderConditions_DistanceInferior_RegionCheck;
                                else counter = CountMembers_UnderConditions_DistanceInferior;
                                break;
                            case Relation.Superior:
                                if (@this._regionCheck)
                                    counter = CountMembers_UnderConditions_DistanceSuperior_RegionCheck;
                                else counter = CountMembers_UnderConditions_DistanceSuperior;
                                break;
                            case Relation.Equal:
                                if (@this._regionCheck)
                                    counter = CountMembers_UnderConditions_DistanceEqual_RegionCheck;
                                else counter = CountMembers_UnderConditions_DistanceEqual;
                                break;
                            case Relation.NotEqual:
                                if (@this._regionCheck)
                                    counter = CountMembers_UnderConditions_DistanceNotEqual_RegionCheck;
                                else counter = CountMembers_UnderConditions_DistanceNotEqual;
                                break;
                        }
                    }
                }
                return counter;
            }
        } 
#endif

        private int Initialize_Counter()
        {
            if (@this._customRegionNames?.Count > 0)
            {
                switch (@this._distanceSign)
                {
                    case Relation.Inferior:
                        if (@this._regionCheck)
                            if (@this._customRegionCheck == Presence.Equal)
                                counter = CountMembers_UnderConditions_DistanceInferior_RegionCheck_WithinCustomRegion;
                            else counter = CountMembers_UnderConditions_DistanceInferior_RegionCheck_OutsideCustomRegion;
                        else if (@this._customRegionCheck == Presence.Equal)
                            counter = CountMembers_UnderConditions_DistanceInferior_WithinCustomRegion;
                        else counter = CountMembers_UnderConditions_DistanceInferior_OutsideCustomRegion;
                        break;
                    case Relation.Superior:
                        if (@this._regionCheck)
                            if (@this._customRegionCheck == Presence.Equal)
                                counter = CountMembers_UnderConditions_DistanceSuperior_RegionCheck_WithinCustomRegion;
                            else counter = CountMembers_UnderConditions_DistanceSuperior_RegionCheck_OutsideCustomRegion;
                        else if (@this._customRegionCheck == Presence.Equal)
                            counter = CountMembers_UnderConditions_DistanceSuperior_WithinCustomRegion;
                        else counter = CountMembers_UnderConditions_DistanceSuperior_OutsideCustomRegion;
                        break;
                    case Relation.Equal:
                        if (@this._regionCheck)
                            if (@this._customRegionCheck == Presence.Equal)
                                counter = CountMembers_UnderConditions_DistanceEqual_RegionCheck_WithinCustomRegion;
                            else counter = CountMembers_UnderConditions_DistanceEqual_RegionCheck_OutsideCustomRegion;
                        else if (@this._customRegionCheck == Presence.Equal)
                            counter = CountMembers_UnderConditions_DistanceEqual_WithinCustomRegion;
                        else counter = CountMembers_UnderConditions_DistanceEqual_OutsideCustomRegion;
                        break;
                    case Relation.NotEqual:
                        if (@this._regionCheck)
                            if (@this._customRegionCheck == Presence.Equal)
                                counter = CountMembers_UnderConditions_DistanceNotEqual_RegionCheck_WithinCustomRegion;
                            else counter = CountMembers_UnderConditions_DistanceNotEqual_RegionCheck_OutsideCustomRegion;
                        else if (@this._customRegionCheck == Presence.Equal)
                            counter = CountMembers_UnderConditions_DistanceNotEqual_WithinCustomRegion;
                        else counter = CountMembers_UnderConditions_DistanceNotEqual_OutsideCustomRegion;
                        break;
                }
            }
            else
            {
                switch (@this._distanceSign)
                {
                    case Relation.Inferior:
                        if (@this._regionCheck)
                            counter = CountMembers_UnderConditions_DistanceInferior_RegionCheck;
                        else counter = CountMembers_UnderConditions_DistanceInferior;
                        break;
                    case Relation.Superior:
                        if (@this._regionCheck)
                            counter = CountMembers_UnderConditions_DistanceSuperior_RegionCheck;
                        else counter = CountMembers_UnderConditions_DistanceSuperior;
                        break;
                    case Relation.Equal:
                        if (@this._regionCheck)
                            counter = CountMembers_UnderConditions_DistanceEqual_RegionCheck;
                        else counter = CountMembers_UnderConditions_DistanceEqual;
                        break;
                    case Relation.NotEqual:
                        if (@this._regionCheck)
                            counter = CountMembers_UnderConditions_DistanceNotEqual_RegionCheck;
                        else counter = CountMembers_UnderConditions_DistanceNotEqual;
                        break;
                }
            }
            return counter();
        }

        private int CountMembers_UnderConditions_DistanceInferior_RegionCheck_WithinCustomRegion()
        {
            int counter = 0;
            foreach (var member in EntityManager.LocalPlayer.PlayerTeam.Team.Members)
                if (DistanceInferior_RegionCheck_WithinCustomRegion(member))
                    counter++;
            return counter;
        }
        private int CountMembers_UnderConditions_DistanceInferior_RegionCheck_OutsideCustomRegion()
        {
            int counter = 0;
            foreach (var member in EntityManager.LocalPlayer.PlayerTeam.Team.Members)
                if (DistanceInferior_RegionCheck_OutsideCustomRegion(member))
                    counter++;
            return counter;
        }
        private int CountMembers_UnderConditions_DistanceInferior_WithinCustomRegion()
        {
            int counter = 0;
            foreach (var member in EntityManager.LocalPlayer.PlayerTeam.Team.Members)
                if (DistanceInferior_WithinCustomRegion(member))
                    counter++;
            return counter;
        }
        private int CountMembers_UnderConditions_DistanceInferior_OutsideCustomRegion()
        {
            int counter = 0;
            foreach (var member in EntityManager.LocalPlayer.PlayerTeam.Team.Members)
                if (DistanceInferior_OutsideCustomRegion(member))
                    counter++;
            return counter;
        }
        private int CountMembers_UnderConditions_DistanceInferior_RegionCheck()
        {
            int counter = 0;
            foreach (var member in EntityManager.LocalPlayer.PlayerTeam.Team.Members)
                if (DistanceInferior_RegionCheck(member))
                    counter++;
            return counter;
        }
        private int CountMembers_UnderConditions_DistanceInferior()
        {
            int counter = 0;
            foreach (var member in EntityManager.LocalPlayer.PlayerTeam.Team.Members)
                if (DistanceInferior(member))
                    counter++;
            return counter;
        }

        private int CountMembers_UnderConditions_DistanceSuperior_RegionCheck_WithinCustomRegion()
        {
            int counter = 0;
            foreach (var member in EntityManager.LocalPlayer.PlayerTeam.Team.Members)
                if (DistanceSuperior_RegionCheck_WithinCustomRegion(member))
                    counter++;
            return counter;
        }
        private int CountMembers_UnderConditions_DistanceSuperior_RegionCheck_OutsideCustomRegion()
        {
            int counter = 0;
            foreach (var member in EntityManager.LocalPlayer.PlayerTeam.Team.Members)
                if (DistanceSuperior_RegionCheck_OutsideCustomRegion(member))
                    counter++;
            return counter;
        }
        private int CountMembers_UnderConditions_DistanceSuperior_WithinCustomRegion()
        {
            int counter = 0;
            foreach (var member in EntityManager.LocalPlayer.PlayerTeam.Team.Members)
                if (DistanceSuperior_WithinCustomRegion(member))
                    counter++;
            return counter;
        }
        private int CountMembers_UnderConditions_DistanceSuperior_OutsideCustomRegion()
        {
            int counter = 0;
            foreach (var member in EntityManager.LocalPlayer.PlayerTeam.Team.Members)
                if (DistanceSuperior_OutsideCustomRegion(member))
                    counter++;
            return counter;
        }
        private int CountMembers_UnderConditions_DistanceSuperior_RegionCheck()
        {
            int counter = 0;
            foreach (var member in EntityManager.LocalPlayer.PlayerTeam.Team.Members)
                if (DistanceSuperior_RegionCheck(member))
                    counter++;
            return counter;
        }
        private int CountMembers_UnderConditions_DistanceSuperior()
        {
            int counter = 0;
            foreach (var member in EntityManager.LocalPlayer.PlayerTeam.Team.Members)
                if (DistanceSuperior(member))
                    counter++;
            return counter;
        }

        private int CountMembers_UnderConditions_DistanceEqual_RegionCheck_WithinCustomRegion()
        {
            var player = EntityManager.LocalPlayer;
            int counter = 0;
            foreach (var member in player.PlayerTeam.Team.Members)
                if (DistanceEqual_RegionCheck_WithinCustomRegion(member))
                    counter++;
            return counter;
        }
        private int CountMembers_UnderConditions_DistanceEqual_RegionCheck_OutsideCustomRegion()
        {
            var player = EntityManager.LocalPlayer;
            int counter = 0;
            foreach (var member in player.PlayerTeam.Team.Members)
                if (DistanceEqual_RegionCheck_OutsideCustomRegion(member))
                    counter++;
            return counter;
        }
        private int CountMembers_UnderConditions_DistanceEqual_WithinCustomRegion()
        {
            var player = EntityManager.LocalPlayer;
            int counter = 0;
            foreach (var member in player.PlayerTeam.Team.Members)
                if (DistanceEqual_WithinCustomRegion(member))
                    counter++;
            return counter;
        }
        private int CountMembers_UnderConditions_DistanceEqual_OutsideCustomRegion()
        {
            var player = EntityManager.LocalPlayer;
            int counter = 0;
            foreach (var member in player.PlayerTeam.Team.Members)
                if (DistanceEqual_OutsideCustomRegion(member))
                    counter++;
            return counter;
        }
        private int CountMembers_UnderConditions_DistanceEqual_RegionCheck()
        {
            var player = EntityManager.LocalPlayer;
            int counter = 0;
            foreach (var member in player.PlayerTeam.Team.Members)
                if (DistanceEqual_RegionCheck(member))
                    counter++;
            return counter;
        }
        private int CountMembers_UnderConditions_DistanceEqual()
        {
            var player = EntityManager.LocalPlayer;
            int counter = 0;
            foreach (var member in player.PlayerTeam.Team.Members)
            {
                var memberEntity = member.Entity;

                if (memberEntity.ContainerId != player.ContainerId
                    && memberEntity.Location.Distance3DFromPlayer == @this._distance)
                    counter++;
            }
            return counter;
        }

        private int CountMembers_UnderConditions_DistanceNotEqual_RegionCheck_WithinCustomRegion()
        {
            var player = EntityManager.LocalPlayer;
            int counter = 0;
            foreach (var member in player.PlayerTeam.Team.Members)
                if (DistanceNotEqual_RegionCheck_WithinCustomRegion(member))
                    counter++;
            return counter;
        }
        private int CountMembers_UnderConditions_DistanceNotEqual_RegionCheck_OutsideCustomRegion()
        {
            var player = EntityManager.LocalPlayer;
            int counter = 0;
            foreach (var member in player.PlayerTeam.Team.Members)
                if (DistanceNotEqual_RegionCheck_OutsideCustomRegion(member))
                    counter++;
            return counter;
        }
        private int CountMembers_UnderConditions_DistanceNotEqual_WithinCustomRegion()
        {
            var player = EntityManager.LocalPlayer;
            int counter = 0;
            foreach (var member in player.PlayerTeam.Team.Members)
                if (DistanceNotEqual_WithinCustomRegion(member))
                    counter++;
            return counter;
        }
        private int CountMembers_UnderConditions_DistanceNotEqual_OutsideCustomRegion()
        {
            var player = EntityManager.LocalPlayer;
            int counter = 0;
            foreach (var member in player.PlayerTeam.Team.Members)
                if (DistanceNotEqual_OutsideCustomRegion(member))
                    counter++;
            return counter;
        }
        private int CountMembers_UnderConditions_DistanceNotEqual_RegionCheck()
        {
            var player = EntityManager.LocalPlayer;
            int counter = 0;
            foreach (var member in player.PlayerTeam.Team.Members)
                if (DistanceNotEqual_RegionCheck(member))
                    counter++;
            return counter;
        }
        private int CountMembers_UnderConditions_DistanceNotEqual()
        {
            var player = EntityManager.LocalPlayer;
            int counter = 0;
            foreach (var member in player.PlayerTeam.Team.Members)
                if (DistanceNotEqual(member))
                    counter++;
            return counter;
        }
        #endregion

        /// <summary>
        /// Предикат, проверяющий истинность соотношения <seealso cref="TeamMembersCount.Sign"/> 
        /// между подсчитанным количеством <seealso cref="TeamMember"/>, удовлетворяющих улосвиям <seealso cref="TeamMembersCount"/>
        /// и заданным значеним <seealso cref="TeamMembersCount.MemberCount"/>
        /// </summary>
        Predicate<int> memberCountChecker;
        
        private bool Initicalize_MemberCountChecker(int count)
        { 
            switch (@this._sign)
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
        private bool Inferior_Than_MemberCount(int count)
        {
            return count < @this._memberCount;
        }
        private bool Superior_Than_MemberCount(int count)
        {
            return count > @this._memberCount;
        }
        private bool Equal_To_MemberCount(int count)
        {
            return count == @this._memberCount;
        }
        private bool NotEqual_To_MemberCount(int count)
        {
            return count != @this._memberCount;
        }
    }
}
