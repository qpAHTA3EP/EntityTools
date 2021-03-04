using Astral.Quester.Classes;
using EntityCore.Extensions;
using EntityTools;
using EntityTools.Quester.Conditions;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Astral.Quester.Classes.Condition;

namespace EntityCore.Quester.Conditions
{
    class TeamMembersCountEngine : IQuesterConditionEngine
    {
        private TeamMembersCount @this = null;

        #region данные ядра
        private Func<List<CustomRegion>> getCustomRegions = null;

        private List<CustomRegion> customRegions = null;
#if timeout
        private Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(0); 
#endif
        /// <summary>
        /// Кэшированное число членов группы, удовлетворяющих критериям
        /// </summary>
        private int membersCount = 0;
        private string label = string.Empty;

        /// <summary>
        /// Префикс, идентифицирующий принадлежность отладочной информации, выводимой в Лог
        /// </summary>
        private string conditionIDstr = string.Empty;

        #endregion

        internal TeamMembersCountEngine(TeamMembersCount tmc)
        {
#if false
            @this = tmc;
            @this.Engine = this;
            @this.PropertyChanged += PropertyChanged;

            getCustomRegions = internal_GetCustomRegion_Initializer;

            conditionIDstr = string.Concat(@this.GetType().Name, '[', @this.GetHashCode().ToString("X2"), ']'); 
#else
            InternalRebase(tmc);
#endif

            ETLogger.WriteLine(LogType.Debug, $"{conditionIDstr} initialized: {Label()}");
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (ReferenceEquals(sender, @this))
            {
                if (e.PropertyName == nameof(@this.Sign)
                    || e.PropertyName == nameof(@this.MemberCount))
                    label = string.Empty;
                else if (e.PropertyName == nameof(@this.CustomRegionNames))
                    getCustomRegions = initialize_GetCustomRegion;

                membersCount = 0;
#if timeout
                timeout.ChangeTime(0); 
#endif
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
                if (InternalRebase(tmc))
                {
                    ETLogger.WriteLine(LogType.Debug, $"{conditionIDstr} reinitialized");
                    return true;
                }
                ETLogger.WriteLine(LogType.Debug, $"{conditionIDstr} rebase failed");
                return false;
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
                @this.Engine = new EntityTools.Core.Proxies.QuesterConditionProxy(@this);
            }

            @this = tmc;
            @this.PropertyChanged += PropertyChanged;

            getCustomRegions = initialize_GetCustomRegion;

            conditionIDstr = string.Concat(@this.GetType().Name, '[', @this.GetHashCode().ToString("X2"), ']');

            @this.Engine = this;

            return true;
        }

        public bool IsValid
        {
            get
            {
                var playerTeam = EntityManager.LocalPlayer.PlayerTeam;
                if (playerTeam != null && playerTeam.IsInTeam && playerTeam.Team.MembersCount > 1)
                {
#if timeout
                    if (timeout.IsTimedOut)
                    { 
#endif
                        var containerId = EntityManager.LocalPlayer.ContainerId;
                        var regionInternalName = EntityManager.LocalPlayer.RegionInternalName;
                        membersCount = 0;

                        // Проверка:
                        //      member.InternalName != EntityManager.LocalPlayer.InternalName
                        // эквивалентна проверке: 
                        //      memberEntity.ContainerId != containerId
                        List<CustomRegion> crList = getCustomRegions();
                        if (crList != null && crList.Count > 0)
                        {
                            switch (@this._distanceSign)
                            {
                                case Relation.Inferior:
                                {
                                    membersCount = playerTeam.Team.Members.Count( member =>
                                    {
                                        var memberEntity = member.Entity;
                                        return memberEntity.ContainerId != containerId
                                                && (!@this._regionCheck || memberEntity.RegionInternalName == regionInternalName)
                                                && memberEntity.Location.Distance3DFromPlayer < @this._distance
                                                && (crList.Find(cr => memberEntity.Within(cr)) != null)
                                                    ? @this._customRegionCheck == Presence.Equal
                                                    : @this._customRegionCheck == Presence.NotEquel;
                                    });

                                    break;
                                }
                                case Relation.Superior:
                                {
                                    membersCount = playerTeam.Team.Members.Count( member =>
                                    {
                                        var memberEntity = member.Entity;
                                        return memberEntity.ContainerId != containerId
                                                            && (!@this._regionCheck || memberEntity.RegionInternalName == regionInternalName)
                                                            && memberEntity.Location.Distance3DFromPlayer > @this._distance
                                                            && (crList.Find(cr => memberEntity.Within(cr)) != null) 
                                                                ? @this._customRegionCheck == Presence.Equal 
                                                                : @this._customRegionCheck == Presence.NotEquel;
                                    });
                                    break; 
                                }
                                case Relation.Equal:
                                {
                                    membersCount = playerTeam.Team.Members.Count( member =>
                                    {
                                        var memberEntity = member.Entity;
                                        return memberEntity.ContainerId != containerId
                                                && (!@this._regionCheck || memberEntity.RegionInternalName == regionInternalName)
                                                && memberEntity.Location.Distance3DFromPlayer == @this._distance
                                                && (crList.Find((CustomRegion cr) => memberEntity.Within(cr)) != null) 
                                                    ? @this._customRegionCheck == Presence.Equal 
                                                    : @this._customRegionCheck == Presence.NotEquel;

                                    });
                                    break;
                                }
                                case Relation.NotEqual:
                                {
                                    membersCount = playerTeam.Team.Members.Count( member =>
                                    {
                                        var memberEntity = member.Entity;
                                        return memberEntity.ContainerId != containerId
                                                && (!@this._regionCheck || memberEntity.RegionInternalName == regionInternalName)
                                                && memberEntity.Location.Distance3DFromPlayer != @this._distance
                                                && (crList.Find((CustomRegion cr) => memberEntity.Within(cr)) != null)
                                                    ? @this._customRegionCheck == Presence.Equal
                                                    : @this._customRegionCheck == Presence.NotEquel;
                                    });
                                    break;
                                }
                            }
                        }
                        else
                        {
                            switch (@this._distanceSign)
                            {
                                case Relation.Inferior:
                                {
                                    membersCount = playerTeam.Team.Members.FindAll( member =>
                                    {
                                        var memberEntity = member.Entity;
                                        return memberEntity.ContainerId != containerId
                                                && (!@this._regionCheck || memberEntity.RegionInternalName == regionInternalName)
                                                && memberEntity.Location.Distance3DFromPlayer < @this._distance;
                                    }).Count;
                                    break;
                                }
                                case Relation.Superior:
                                {
                                    membersCount = playerTeam.Team.Members.FindAll( member =>
                                    {
                                        var memberEntity = member.Entity;
                                        return member.Entity.ContainerId != containerId
                                                && (!@this._regionCheck || member.Entity.RegionInternalName == regionInternalName)
                                                && member.Entity.Location.Distance3DFromPlayer > @this._distance;
                                    }).Count;
                                    break;
                                }
                                case Relation.Equal:
                                {
                                    membersCount = playerTeam.Team.Members.FindAll(member =>
                                    {
                                        var memberEntity = member.Entity;
                                        return memberEntity.ContainerId != containerId
                                                && (!@this._regionCheck || memberEntity.RegionInternalName == regionInternalName)
                                                && memberEntity.Location.Distance3DFromPlayer == @this._distance;
                                    }).Count;
                                    break;
                                }
                                case Relation.NotEqual:
                                {
                                    membersCount = playerTeam.Team.Members.FindAll((TeamMember member) =>
                                    {
                                        var memberEntity = member.Entity;
                                        return memberEntity.ContainerId != containerId
                                                && (!@this._regionCheck || memberEntity.RegionInternalName == regionInternalName)
                                                && memberEntity.Location.Distance3DFromPlayer != @this._distance;
                                    }).Count;
                                    break;
                                }
                            }
                        }
#if timeout
                        timeout.ChangeTime(EntityTools.EntityTools.Config.EntityCache.LocalCacheTime);
                    } 
#endif

                    bool result;
                    switch (@this._sign)
                    {
                        case Condition.Relation.Equal:
                            result = membersCount == @this._memberCount;
                            break;
                        case Condition.Relation.NotEqual:
                            result = membersCount != @this._memberCount;
                            break;
                        case Condition.Relation.Inferior:
                            result = membersCount < @this._memberCount;
                            break;
                        case Condition.Relation.Superior:
                            result = membersCount > @this._memberCount;
                            break;
                        default:
                            result = false;
                            break;
                    }
                    return result;
                }
                else return false;
            }
        }

        public void Reset()
        {
            getCustomRegions = initialize_GetCustomRegion;
            membersCount = 0;
            label = string.Empty;
        }

        public string TestInfos
        {
            get
            {
                if (EntityManager.LocalPlayer.PlayerTeam?.IsInTeam == true
                    && EntityManager.LocalPlayer.PlayerTeam?.Team?.MembersCount > 1)
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

                        /* if (member.InternalName != EntityManager.LocalPlayer.InternalName)
                         * Эквивалентно следующей строке: */
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

                            var crList = getCustomRegions();
                            if (crList != null && crList.Count > 0)
                            {
                                foreach (CustomRegion cr in crList)
                                    if (memberEntity.Within(cr))
                                    {
                                        match = true;
                                        if (strBldr2.Length > 0)
                                            strBldr2.Append(", ");
                                        strBldr2.Append($"[{cr.Name}]");
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
                                        if (memberDistance == @this._distance)
                                        {
                                            if (@this._customRegionCheck == Presence.Equal && match)
                                                memsCount++;
                                            if (@this._customRegionCheck == Presence.NotEquel && !match)
                                                memsCount++;
                                        }
                                        break;
                                    case Relation.NotEqual:
                                        if (memberDistance != @this._distance)
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
                                        if (memberDistance == @this._distance)
                                            memsCount++;
                                        break;
                                    case Relation.NotEqual:
                                        if (memberDistance != @this._distance)
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
            // ТОDO: исправить обновление метки в списке условий, после изменения параметров условия
            if(string.IsNullOrEmpty(label))
                label = $"{@this.GetType().Name} {@this._sign} to {@this._memberCount}";
            return label;
        }

        private List<CustomRegion> initialize_GetCustomRegion()
        {
            if (@this._customRegionNames?.Count > 0)
            {
                customRegions = CustomRegionExtentions.GetCustomRegions(@this._customRegionNames);
#if DEBUG
                if (customRegions is null || customRegions.Count == 0)
                    ETLogger.WriteLine(LogType.Debug, $"{conditionIDstr}.{nameof(initialize_GetCustomRegion)}: List of {nameof(@this.CustomRegionNames)} is empty");
                else ETLogger.WriteLine(LogType.Debug, $"{conditionIDstr}.{nameof(initialize_GetCustomRegion)}: Select List of {customRegions.Count} CustomRegions");
#endif
            }
            else
            {
                customRegions = null;
#if DEBUG
                ETLogger.WriteLine(LogType.Debug, $"{conditionIDstr}.{nameof(initialize_GetCustomRegion)}: List of {nameof(@this.CustomRegionNames)} is empty");
#endif
            }

            getCustomRegions = internal_GetCustomRegion_Getter;

            return customRegions;
        }

        private List<CustomRegion> internal_GetCustomRegion_Getter()
        {
            return customRegions;
        }
    }
}
