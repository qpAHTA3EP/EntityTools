using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Text;

using Astral.Quester.Classes;

using EntityTools.Core.Interfaces;
using EntityTools.Editors;
using EntityTools.Extensions;
using EntityTools.Patches.Mapper;
using EntityTools.Tools.CustomRegions;

using MyNW.Classes;
using MyNW.Internals;

namespace EntityTools.Quester.Conditions
{
    [Serializable]
    public class TeamMembersCount : Condition, INotifyPropertyChanged
    {
        #region Опции команды
#if DEVELOPER
        [Description("CustomRegion names collection")]
        [Editor(typeof(CustomRegionCollectionEditor), typeof(UITypeEditor))]
        [Category("Location")]
        [DisplayName("CustomRegions")]
#else
        [Browsable(false)]
#endif
        public CustomRegionCollection CustomRegionNames
        {
            get => _customRegionNames;
            set
            {
                if (_customRegionNames != value)
                {
                    _customRegionNames = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private CustomRegionCollection _customRegionNames = new CustomRegionCollection();

#if DEVELOPER
        [Description("The Check of the Team member's location relative to the custom regions\n" +
            "Equals: Count only members located WITHIN the CustomRegions\n" +
            "NotEquals: Count only members located OUTSIDE the CustomRegions")]
        [Category("Location")]
#else
        [Browsable(false)]
#endif
        public Presence CustomRegionCheck
        {
            get => _customRegionCheck; set
            {
                _customRegionCheck = value;
                NotifyPropertyChanged();
            }
        }
        private Presence _customRegionCheck = Presence.Equal;

#if DEVELOPER
        [Description("The Value which is compared by 'DistanceSign' with the distance between Player and Team member")]
        [Category("Distance")]
#else
        [Browsable(false)]
#endif
        public float Distance
        {
            get => _distance; set
            {
                if (value < 0)
                    value = 0;
                _distance = value;
                NotifyPropertyChanged();
            }
        }
        private float _distance;

#if DEVELOPER
        [Description("Option specifies the comparison of the distance to the group member")]
        [Category("Distance")]
#else
        [Browsable(false)]
#endif
        public Relation DistanceSign
        {
            get => _distanceSign; set
            {
                _distanceSign = value;
                NotifyPropertyChanged();
            }
        }
        private Relation _distanceSign = Relation.Superior;

#if DEVELOPER
        [Description("The Value which is compared by 'Sign' with the counted Team members")]
        [Category("Members")]
#else
        [Browsable(false)]
#endif
        public uint MemberCount
        {
            get => _memberCount; set
            {
                _memberCount = value;
                NotifyPropertyChanged();
            }
        }
        private uint _memberCount = 3;

#if DEVELOPER
        [Description("Option specifies the comparison of 'MemberCount' and the counted Team members")]
        [Category("Members")]
#else
        [Browsable(false)]
#endif
        public Relation Sign
        {
            get => _sign; set
            {
                _sign = value;
                NotifyPropertyChanged();
            }
        }
        private Relation _sign = Relation.Inferior;

#if DEVELOPER
        [Description("The Check of the Team member's Region (not CustomRegion)):\n" +
            "True: Count Team member if it is located in the same Region as Player\n" +
            "False: Does not consider the region when counting Team members")]
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
        public bool RegionCheck
        {
            get => _regionCheck; set
            {
                _regionCheck = value;
                NotifyPropertyChanged();
            }
        }
        private bool _regionCheck;
        #endregion




        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            InternalResetOnPropertyChanged(propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void InternalResetOnPropertyChanged([CallerMemberName] string memberName = default)
        {
            _label = string.Empty;
            _idStr = string.Concat(GetType().Name, '[', GetHashCode().ToString("X2"), ']');
            memberCountChecker = Initicalize_MemberCountChecker;
            counter = Initialize_Counter;
        }
        #endregion




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




        public override bool IsValid => EntityManager.LocalPlayer.PlayerTeam.IsInTeam && memberCountChecker(counter());

        public override void Reset()
        {
            memberCountChecker = Initicalize_MemberCountChecker;
            counter = Initialize_Counter;
        }

        public override string TestInfos
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

                            if (RegionCheck)
                            {
                                strBldr.Append($"\tRegionCheck[").Append(memberRegion).Append("]: ");
                                if (memberRegion == regionInternalName)
                                    strBldr.AppendLine("succeeded");
                                else strBldr.AppendLine("fail");
                            }

                            strBldr2.Clear();
                            bool match = false;

                            var crSet = CustomRegionNames;
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

                                switch (DistanceSign)
                                {
                                    case Relation.Inferior:
                                        if (memberDistance < Distance)
                                        {
                                            if (CustomRegionCheck == Presence.Equal && match)
                                                memsCount++;
                                            if (CustomRegionCheck == Presence.NotEquel && !match)
                                                memsCount++;
                                        }
                                        break;
                                    case Relation.Superior:
                                        if (memberDistance > Distance)
                                        {
                                            if (CustomRegionCheck == Presence.Equal && match)
                                                memsCount++;
                                            if (CustomRegionCheck == Presence.NotEquel && !match)
                                                memsCount++;
                                        }
                                        break;
                                    case Relation.Equal:
                                        if (Math.Abs(memberDistance - Distance) <= 1.0)
                                        {
                                            if (CustomRegionCheck == Presence.Equal && match)
                                                memsCount++;
                                            if (CustomRegionCheck == Presence.NotEquel && !match)
                                                memsCount++;
                                        }
                                        break;
                                    case Relation.NotEqual:
                                        if (Math.Abs(memberDistance - Distance) > 1.0)
                                        {
                                            if (CustomRegionCheck == Presence.Equal && match)
                                                memsCount++;
                                            if (CustomRegionCheck == Presence.NotEquel && !match)
                                                memsCount++;
                                        }
                                        break;
                                }
                            }
                            else
                            {
                                switch (DistanceSign)
                                {
                                    case Relation.Inferior:
                                        if (memberDistance < Distance)
                                            memsCount++;
                                        break;
                                    case Relation.Superior:
                                        if (memberDistance > Distance)
                                            memsCount++;
                                        break;
                                    case Relation.Equal:
                                        if (Math.Abs(memberDistance - Distance) <= 1.0)
                                            memsCount++;
                                        break;
                                    case Relation.NotEqual:
                                        if (Math.Abs(memberDistance - Distance) > 1.0)
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
            if (string.IsNullOrEmpty(_label))
                _label = $"{GetType().Name} {Sign} to {MemberCount}";
            return _label;
        }

        #region MemberCounters
        /// <summary>
        /// Cчетчик <seealso cref="TeamMember"/>, удовлетворяющих условиям команды <seealso cref="TeamMembersCount"/>
        /// </summary>
        Func<uint> counter;
        private uint Initialize_Counter()
        {
            if (CustomRegionNames?.Count > 0)
            {
                switch (DistanceSign)
                {
                    case Relation.Inferior:
                        if (RegionCheck)
                            if (CustomRegionCheck == Presence.Equal)
                                counter = CountMembers_UnderConditions_DistanceInferior_RegionCheck_WithinCustomRegion;
                            else counter = CountMembers_UnderConditions_DistanceInferior_RegionCheck_OutsideCustomRegion;
                        else if (CustomRegionCheck == Presence.Equal)
                            counter = CountMembers_UnderConditions_DistanceInferior_WithinCustomRegion;
                        else counter = CountMembers_UnderConditions_DistanceInferior_OutsideCustomRegion;
                        break;
                    case Relation.Superior:
                        if (RegionCheck)
                            if (CustomRegionCheck == Presence.Equal)
                                counter = CountMembers_UnderConditions_DistanceSuperior_RegionCheck_WithinCustomRegion;
                            else counter = CountMembers_UnderConditions_DistanceSuperior_RegionCheck_OutsideCustomRegion;
                        else if (CustomRegionCheck == Presence.Equal)
                            counter = CountMembers_UnderConditions_DistanceSuperior_WithinCustomRegion;
                        else counter = CountMembers_UnderConditions_DistanceSuperior_OutsideCustomRegion;
                        break;
                    case Relation.Equal:
                        if (RegionCheck)
                            if (CustomRegionCheck == Presence.Equal)
                                counter = CountMembers_UnderConditions_DistanceEqual_RegionCheck_WithinCustomRegion;
                            else counter = CountMembers_UnderConditions_DistanceEqual_RegionCheck_OutsideCustomRegion;
                        else if (CustomRegionCheck == Presence.Equal)
                            counter = CountMembers_UnderConditions_DistanceEqual_WithinCustomRegion;
                        else counter = CountMembers_UnderConditions_DistanceEqual_OutsideCustomRegion;
                        break;
                    case Relation.NotEqual:
                        if (RegionCheck)
                            if (CustomRegionCheck == Presence.Equal)
                                counter = CountMembers_UnderConditions_DistanceNotEqual_RegionCheck_WithinCustomRegion;
                            else counter = CountMembers_UnderConditions_DistanceNotEqual_RegionCheck_OutsideCustomRegion;
                        else if (CustomRegionCheck == Presence.Equal)
                            counter = CountMembers_UnderConditions_DistanceNotEqual_WithinCustomRegion;
                        else counter = CountMembers_UnderConditions_DistanceNotEqual_OutsideCustomRegion;
                        break;
                }
            }
            else
            {
                switch (DistanceSign)
                {
                    case Relation.Inferior:
                        if (RegionCheck)
                            counter = CountMembers_UnderConditions_DistanceInferior_RegionCheck;
                        else counter = CountMembers_UnderConditions_DistanceInferior;
                        break;
                    case Relation.Superior:
                        if (RegionCheck)
                            counter = CountMembers_UnderConditions_DistanceSuperior_RegionCheck;
                        else counter = CountMembers_UnderConditions_DistanceSuperior;
                        break;
                    case Relation.Equal:
                        if (RegionCheck)
                            counter = CountMembers_UnderConditions_DistanceEqual_RegionCheck;
                        else counter = CountMembers_UnderConditions_DistanceEqual;
                        break;
                    case Relation.NotEqual:
                        if (RegionCheck)
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
            var sqDist = Distance;
            sqDist *= sqDist;
            var crSet = CustomRegionNames;
            foreach (var member in playerTeam.Team.Members)
            {
                var memberEntity = member.Entity;
                if (memberEntity.ContainerId != playerContainerId
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
            var sqDist = Distance;
            sqDist *= sqDist;
            var crSet = CustomRegionNames;
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
            var sqDist = Distance;
            sqDist *= sqDist;
            var crSet = CustomRegionNames;
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
            var sqDist = Distance;
            sqDist *= sqDist;
            var crSet = CustomRegionNames;
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
            var sqDist = Distance;
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
            var sqDist = Distance;
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
            var dist = Distance;
            var crSet = CustomRegionNames;
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
            var sqDist = Distance;
            sqDist *= sqDist;
            var crSet = CustomRegionNames;
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
            var sqDist = Distance;
            sqDist *= sqDist;
            var crSet = CustomRegionNames;
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
            var sqDist = Distance;
            sqDist *= sqDist;
            var crSet = CustomRegionNames;
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
            var sqDist = Distance;
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
            var sqDist = Distance;
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
            var sqDist = Distance;
            sqDist *= sqDist;
            var crSet = CustomRegionNames;
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
            var sqDist = Distance;
            sqDist *= sqDist;
            var crSet = CustomRegionNames;
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
            var sqDist = Distance;
            sqDist *= sqDist;
            var crSet = CustomRegionNames;
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
            var sqDist = Distance;
            sqDist *= sqDist;
            var crSet = CustomRegionNames;
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
            var sqDist = Distance;
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
            var dist = Distance;
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
            var sqDist = Distance;
            sqDist *= sqDist;
            var crSet = CustomRegionNames;
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
            var sqDist = Distance;
            sqDist *= sqDist;
            var crSet = CustomRegionNames;
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
            var sqDist = Distance;
            sqDist *= sqDist;
            var crSet = CustomRegionNames;
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
            var sqDist = Distance;
            sqDist *= sqDist;
            var crSet = CustomRegionNames;
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
            var sqDist = Distance;
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
            var sqDist = Distance;
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
            switch (Sign)
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
            return count < MemberCount;
        }
        private bool Superior_Than_MemberCount(uint count)
        {
            return count > MemberCount;
        }
        private bool Equal_To_MemberCount(uint count)
        {
            // Если персонаж не состоит в группе, то "count == 0"
            // В этом случае условие "Count Equal 0" будет истинно
            return count == MemberCount;
        }
        private bool NotEqual_To_MemberCount(uint count)
        {
            // Если персонаж не состоит в группе, то "count == 0"
            // В этом случае условие "Count NotEqual N" будет истинно при N > 0
            return count != MemberCount;
        }
        #endregion
    }
}
