using Astral.Quester.Classes;
using EntityCore.Extensions;
using EntityTools.Logger;
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
    class TeamMembersCountEngine
#if CORE_INTERFACES
        : IQuesterConditionEngine
#endif
    {
        private TeamMembersCount @this = null;

        #region данные ядра
        private Func<List<CustomRegion>> getCustomRegions = null;

        private List<CustomRegion> customRegions = null;
        private Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(0);
        /// <summary>
        /// Кэшированное число членов группы, удовлетворяющих критериям
        /// </summary>
        private int membersCount = 0;
        private string label = string.Empty;

        #endregion

        internal TeamMembersCountEngine(TeamMembersCount tmc)
        {
            @this = tmc;
#if CORE_INTERFACES
            @this.ConditionEngine = this;
#endif
            @this.PropertyChanged += PropertyChanged;

            getCustomRegions = internal_GetCustomRegion_Initializer;

            EntityToolsLogger.WriteLine(LogType.Debug, $"{@this.GetType().Name}[{@this.GetHashCode().ToString("X2")}] initialized");
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (object.ReferenceEquals(sender, @this))
            {
                if (e.PropertyName == nameof(@this.Sign)
                    || e.PropertyName == nameof(@this.MemberCount))
                    label = string.Empty;
                else if (e.PropertyName == nameof(@this.CustomRegionNames))
                    getCustomRegions = internal_GetCustomRegion_Initializer;

                membersCount = 0;
                timeout.ChangeTime(0);
            }
        }


#if CORE_INTERFACES
        public bool IsValid
        {
            get
            {
                if (EntityManager.LocalPlayer.PlayerTeam?.IsInTeam == true
                    && EntityManager.LocalPlayer.PlayerTeam?.Team?.MembersCount > 1)
                {
                    if (timeout.IsTimedOut)
                    {
                        membersCount = 0;

                        List<CustomRegion> crList = getCustomRegions();
                        if (crList != null && crList.Count > 0)
                        {
                            switch (@this._distanceSign)
                            {
                                case Relation.Inferior:
                                    {
                                        membersCount = EntityManager.LocalPlayer.PlayerTeam.Team.Members.FindAll((TeamMember member) =>
                                                        (/* member.InternalName != EntityManager.LocalPlayer.InternalName
                                                      * Эквивалентно строке: */
                                                            member.Entity.ContainerId != EntityManager.LocalPlayer.ContainerId
                                                            && (!@this._regionCheck || member.Entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                            && member.Entity.Location.Distance3DFromPlayer < @this._distance
                                                            && (crList.Find((CustomRegion cr) => member.Entity.Within(cr)) != null) ? @this._customRegionCheck == Presence.Equal : @this._customRegionCheck == Presence.NotEquel)
                                                        ).Count;

                                        break;
                                    }
                                case Relation.Superior:
                                    {
                                        membersCount = EntityManager.LocalPlayer.PlayerTeam.Team.Members.FindAll((TeamMember member) =>
                                                        (/* member.InternalName != EntityManager.LocalPlayer.InternalName
                                                      * Эквивалентно строке: */
                                                            member.Entity.ContainerId != EntityManager.LocalPlayer.ContainerId
                                                            && (!@this._regionCheck || member.Entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                            && member.Entity.Location.Distance3DFromPlayer > @this._distance
                                                            && (crList.Find((CustomRegion cr) => member.Entity.Within(cr)) != null) ? @this._customRegionCheck == Presence.Equal : @this._customRegionCheck == Presence.NotEquel)
                                                        ).Count;
                                        break;
                                    }
                                case Relation.Equal:
                                    {
                                        membersCount = EntityManager.LocalPlayer.PlayerTeam.Team.Members.FindAll((TeamMember member) =>
                                                        (/* member.InternalName != EntityManager.LocalPlayer.InternalName
                                                      * Эквивалентно строке: */
                                                            member.Entity.ContainerId != EntityManager.LocalPlayer.ContainerId
                                                            && (!@this._regionCheck || member.Entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                            && member.Entity.Location.Distance3DFromPlayer == @this._distance
                                                            && (crList.Find((CustomRegion cr) => member.Entity.Within(cr)) != null) ? @this._customRegionCheck == Presence.Equal : @this._customRegionCheck == Presence.NotEquel)
                                                        ).Count;
                                        break;
                                    }
                                case Relation.NotEqual:
                                    {
                                        membersCount = EntityManager.LocalPlayer.PlayerTeam.Team.Members.FindAll((TeamMember member) =>
                                                        (/* member.InternalName != EntityManager.LocalPlayer.InternalName
                                                          * Эквивалентно строке: */
                                                            member.Entity.ContainerId != EntityManager.LocalPlayer.ContainerId
                                                            && (!@this._regionCheck || member.Entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                            && member.Entity.Location.Distance3DFromPlayer != @this._distance
                                                            && (crList.Find((CustomRegion cr) => member.Entity.Within(cr)) != null) ? @this._customRegionCheck == Presence.Equal : @this._customRegionCheck == Presence.NotEquel)
                                                        ).Count;
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
                                        membersCount = EntityManager.LocalPlayer.PlayerTeam.Team.Members.FindAll((TeamMember member) =>
                                                        (/* member.InternalName != EntityManager.LocalPlayer.InternalName
                                                      * Эквивалентно строке: */
                                                            member.Entity.ContainerId != EntityManager.LocalPlayer.ContainerId
                                                            && (!@this._regionCheck || member.Entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                            && member.Entity.Location.Distance3DFromPlayer < @this._distance)
                                                        ).Count;

                                        break;
                                    }
                                case Relation.Superior:
                                    {
                                        membersCount = EntityManager.LocalPlayer.PlayerTeam.Team.Members.FindAll((TeamMember member) =>
                                                        (/* member.InternalName != EntityManager.LocalPlayer.InternalName
                                                      * Эквивалентно строке: */
                                                            member.Entity.ContainerId != EntityManager.LocalPlayer.ContainerId
                                                            && (!@this._regionCheck || member.Entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                            && member.Entity.Location.Distance3DFromPlayer > @this._distance)
                                                        ).Count;
                                        break;
                                    }
                                case Relation.Equal:
                                    {
                                        membersCount = EntityManager.LocalPlayer.PlayerTeam.Team.Members.FindAll((TeamMember member) =>
                                                        (/* member.InternalName != EntityManager.LocalPlayer.InternalName
                                                      * Эквивалентно строке: */
                                                            member.Entity.ContainerId != EntityManager.LocalPlayer.ContainerId
                                                            && (!@this._regionCheck || member.Entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                            && member.Entity.Location.Distance3DFromPlayer == @this._distance)
                                                        ).Count;
                                        break;
                                    }
                                case Relation.NotEqual:
                                    {
                                        membersCount = EntityManager.LocalPlayer.PlayerTeam.Team.Members.FindAll((TeamMember member) =>
                                                        (/* member.InternalName != EntityManager.LocalPlayer.InternalName
                                                      * Эквивалентно строке: */
                                                            member.Entity.ContainerId != EntityManager.LocalPlayer.ContainerId
                                                            && (!@this._regionCheck || member.Entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                                            && member.Entity.Location.Distance3DFromPlayer != @this._distance)
                                                        ).Count;
                                        break;
                                    }
                            }
                        }
                        timeout.ChangeTime(EntityTools.EntityTools.PluginSettings.EntityCache.LocalCacheTime);
                    }

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

        public void Reset() { }

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

                    foreach (TeamMember member in EntityManager.LocalPlayer.PlayerTeam.Team.Members)
                    {
                        /* if (member.InternalName != EntityManager.LocalPlayer.InternalName)
                         * Эквивалентно следующей строке: */
                        if (member.Entity.ContainerId != EntityManager.LocalPlayer.ContainerId)
                        {
                            strBldr.AppendLine(member.InternalName);
                            strBldr.AppendFormat("\tDistance: {0:0.##}", member.Entity.Location.Distance3DFromPlayer).AppendLine();

                            if (@this._regionCheck)
                            {
                                strBldr.Append($"\tRegionCheck[").Append(member.Entity.RegionInternalName).Append("]: ");
                                if (member.Entity.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName)
                                    strBldr.AppendLine("succeeded");
                                else strBldr.AppendLine("fail");
                            }

                            strBldr2.Clear();
                            bool match = false;

                            var crList = getCustomRegions();
                            if (crList != null && crList.Count > 0)
                            {
                                foreach (CustomRegion cr in crList)
                                    if (member.Entity.Within(cr))
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
                                        if (member.Entity.Location.Distance3DFromPlayer < @this._distance)
                                        {
                                            if (@this._customRegionCheck == Presence.Equal && match)
                                                memsCount++;
                                            if (@this._customRegionCheck == Presence.NotEquel && !match)
                                                memsCount++;
                                        }
                                        break;
                                    case Relation.Superior:
                                        if (member.Entity.Location.Distance3DFromPlayer > @this._distance)
                                        {
                                            if (@this._customRegionCheck == Presence.Equal && match)
                                                memsCount++;
                                            if (@this._customRegionCheck == Presence.NotEquel && !match)
                                                memsCount++;
                                        }
                                        break;
                                    case Relation.Equal:
                                        if (member.Entity.Location.Distance3DFromPlayer == @this._distance)
                                        {
                                            if (@this._customRegionCheck == Presence.Equal && match)
                                                memsCount++;
                                            if (@this._customRegionCheck == Presence.NotEquel && !match)
                                                memsCount++;
                                        }
                                        break;
                                    case Relation.NotEqual:
                                        if (member.Entity.Location.Distance3DFromPlayer != @this._distance)
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
                                        if (member.Entity.Location.Distance3DFromPlayer < @this._distance)
                                            memsCount++;
                                        break;
                                    case Relation.Superior:
                                        if (member.Entity.Location.Distance3DFromPlayer > @this._distance)
                                            memsCount++;
                                        break;
                                    case Relation.Equal:
                                        if (member.Entity.Location.Distance3DFromPlayer == @this._distance)
                                            memsCount++;
                                        break;
                                    case Relation.NotEqual:
                                        if (member.Entity.Location.Distance3DFromPlayer != @this._distance)
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
            if(string.IsNullOrEmpty(label))
                label = $"{@this.GetType().Name} {@this._sign} to {@this._memberCount}";
            return label;
        }
#endif

        private List<CustomRegion> internal_GetCustomRegion_Initializer()
        {
            if (customRegions == null && @this._customRegionNames != null)
            {
                getCustomRegions = internal_GetCustomRegion_Getter;
                customRegions = CustomRegionExtentions.GetCustomRegions(@this._customRegionNames);
                return customRegions;
            }
            return null;
        }

        private List<CustomRegion> internal_GetCustomRegion_Getter()
        {
            return customRegions;
        }
    }
}
