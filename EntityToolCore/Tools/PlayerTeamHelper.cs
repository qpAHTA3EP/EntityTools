using EntityTools.Enums;
using EntityTools.Tools;
using EntityTools.Tools.Navigation;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using System.Linq;

namespace EntityCore.Tools
{
    public static class PlayerTeamHelper
    {
        //TODO : Развернуть циклы, т.к. известно максимальное число персонажей в группе

        #region GetFellow
#if false
        /// <summary>
        /// Выбор подзащитного
        /// </summary>
        public static Entity GetComrade(FellowType fellowType, int range)
        {
            var player = EntityManager.LocalPlayer;
            var team = player.PlayerTeam;

            if (!team.IsInTeam)
                return null;

            if (range <= 0)
                range = int.MaxValue;
            else range *= range;

            switch (fellowType)
            {
                case ComradeType.Leader:
                    return GetLeader(range);
                case ComradeType.Tank:
                    return GetTank(range);
                case ComradeType.Healer:
                    return GetHealer(range);
                case ComradeType.Sturdiest:
                    return GetSturdiest(range);
                case ComradeType.SturdiestDD:
                    return GetSturdiestDamageDealer(range);
                case ComradeType.Weakest:
                    return GetWeakest(range);
                case ComradeType.WeakestDD:
                    return GetWeakestDamageDealer(range);
            }

            return null;
        }

        /// <summary>
        /// Поиск лидера группы
        /// </summary>
        /// <param name="squareRange">Квадрат радиуса, в пределах которого производится поиск</param>
        public static Entity GetLeader(float squareRange)
        {
            var player = EntityManager.LocalPlayer;
            var team = player.PlayerTeam;

            if (team.IsLeader)
                return null;

            var leader = team.Team.Leader;
            var leaderEntity = leader.Entity;

            if (!leaderEntity.IsValid)
                return null;

            if (NavigationHelper.SquareDistance3D(player.Location, leaderEntity.Location) < squareRange &&
                player.MapState.MapName == leader.MapName &&
                player.RegionInternalName == leaderEntity.RegionInternalName)
            {
                return leaderEntity;
            }

            return null;
        }
        /// <summary>
        /// Поиск Танка среди членов группы без учета расстояния до него
        /// </summary>
        public static Entity GetLeader()
        {
            var player = EntityManager.LocalPlayer;
            var team = player.PlayerTeam;

            if (team.IsLeader)
                return null;

            var leader = team.Team.Leader;
            var leaderEntity = leader.Entity;

            if (!leaderEntity.IsValid)
                return null;

            if (player.MapState.MapName == leader.MapName &&
                player.RegionInternalName == leaderEntity.RegionInternalName)
            {
                return leaderEntity;
            }

            return null;
        }

        /// <summary>
        /// Поиск Танка среди членов группы 
        /// </summary>
        /// <param name="squareRange">Квадрат радиуса, в пределах которого производится поиск</param>
        public static Entity GetTank(float squareRange)
        {
            var player = EntityManager.LocalPlayer;
            var playerRegion = player.RegionInternalName;
            var playerMap = player.MapState.MapName;
            var team = player.PlayerTeam;
            var playerPos = player.Location;
            var selfId = player.ContainerId;

            foreach (var tm in team.Team.Members)
            {
                if (tm.EntityId == selfId)
                    continue;

                var tmEntity = tm.Entity;

                if (playerMap != tm.MapName ||
                    playerRegion != tmEntity.RegionInternalName)
                    continue;

                if (NavigationHelper.SquareDistance3D(playerPos, tmEntity.Location) < squareRange)
                {
                    var role = tmEntity.GetTeamRole();
                    if (role == TeamRoles.Tank)
                        return tmEntity;
                }
            }

            return null;
        }
        /// <summary>
        /// Поиск Танка среди членов группы без учета расстояния до него
        /// </summary>
        public static Entity GetTank()
        {
            var player = EntityManager.LocalPlayer;
            var playerRegion = player.RegionInternalName;
            var playerMap = player.MapState.MapName;
            var team = player.PlayerTeam;
            var selfId = player.ContainerId;

            foreach (var tm in team.Team.Members)
            {
                if (tm.EntityId == selfId)
                    continue;

                var tmEntity = tm.Entity;

                if (playerMap != tm.MapName ||
                    playerRegion != tmEntity.RegionInternalName)
                    continue;

                var role = tmEntity.GetTeamRole();
                if (role == TeamRoles.Tank)
                    return tmEntity;
            }

            return null;
        }

        /// <summary>
        /// Поиск Целителя среди членов группы 
        /// </summary>
        /// <param name="squareRange">Квадрат радиуса, в пределах которого производится поиск</param>
        public static Entity GetHealer(float squareRange)
        {
            var player = EntityManager.LocalPlayer;
            var playerRegion = player.RegionInternalName;
            var playerMap = player.MapState.MapName;
            var team = player.PlayerTeam;
            var playerPos = player.Location;
            var selfId = player.ContainerId;

            foreach (var tm in team.Team.Members)
            {
                if (tm.EntityId == selfId)
                    continue;

                var tmEntity = tm.Entity;

                if (playerMap != tm.MapName ||
                    playerRegion != tmEntity.RegionInternalName)
                    continue;

                if (NavigationHelper.SquareDistance3D(playerPos, tmEntity.Location) < squareRange)
                {
                    var role = tmEntity.GetTeamRole();
                    if (role == TeamRoles.Healer)
                        return tmEntity;
                }
            }

            return null;
        }
        /// <summary>
        /// Поиск Целителя среди членов группы без учета расстояния до него
        /// </summary>
        public static Entity GetHealer()
        {
            var player = EntityManager.LocalPlayer;
            var playerRegion = player.RegionInternalName;
            var playerMap = player.MapState.MapName;
            var team = player.PlayerTeam;
            var selfId = player.ContainerId;

            foreach (var tm in team.Team.Members)
            {
                if (tm.EntityId == selfId)
                    continue;

                var tmEntity = tm.Entity;

                if (playerMap != tm.MapName ||
                    playerRegion != tmEntity.RegionInternalName)
                    continue;

                var role = tmEntity.GetTeamRole();
                if (role == TeamRoles.Healer)
                    return tmEntity;
            }

            return null;
        }

        /// <summary>
        /// Поиск самого живучего члена группы (с наибольшим максимальным HP)
        /// </summary>
        /// <param name="squareRange">Квадрат радиуса, в пределах которого производится поиск</param>
        public static Entity GetSturdiest(float squareRange)
        {
            var player = EntityManager.LocalPlayer;
            var playerRegion = player.RegionInternalName;
            var playerMap = player.MapState.MapName;
            var team = player.PlayerTeam;
            var playerPos = player.Location;
            var selfId = player.ContainerId;
            float maxHealth = 0;
            Entity hardiest = null;

            foreach (var tm in team.Team.Members)
            {
                if (tm.EntityId == selfId)
                    continue;

                var tmEntity = tm.Entity;

                if (playerMap != tm.MapName ||
                    playerRegion != tmEntity.RegionInternalName)
                    continue;
                var curMaxHealth = tmEntity.Character.AttribsBasic.MaxHealth;
                if (curMaxHealth > maxHealth)
                {
                    if (NavigationHelper.SquareDistance3D(playerPos, tmEntity.Location) < squareRange)
                    {
                        maxHealth = curMaxHealth;
                        hardiest = tmEntity;
                    }
                }
            }

            return hardiest;
        }
        /// <summary>
        /// Поиск самого живучего члена группы (с наибольшим максимальным HP) без учета расстояния до него
        /// </summary>
        public static Entity GetSturdiest()
        {
            var player = EntityManager.LocalPlayer;
            var playerRegion = player.RegionInternalName;
            var playerMap = player.MapState.MapName;
            var team = player.PlayerTeam;
            var selfId = player.ContainerId;
            float maxHealth = 0;
            Entity hardiest = null;

            foreach (var tm in team.Team.Members)
            {
                if (tm.EntityId == selfId)
                    continue;

                var tmEntity = tm.Entity;

                if (playerMap != tm.MapName ||
                    playerRegion != tmEntity.RegionInternalName)
                    continue;
                var curMaxHealth = tmEntity.Character.AttribsBasic.MaxHealth;
                if (curMaxHealth > maxHealth)
                {
                    maxHealth = curMaxHealth;
                    hardiest = tmEntity;
                }
            }

            return hardiest;
        }

        /// <summary>
        /// Поиск самого живучего дамагера среди членов группы (с наибольшим максимальным HP)
        /// </summary>
        /// <param name="squareRange">Квадрат радиуса, в пределах которого производится поиск</param>
        public static Entity GetSturdiestDamageDealer(float squareRange)
        {
            var player = EntityManager.LocalPlayer;
            var playerRegion = player.RegionInternalName;
            var playerMap = player.MapState.MapName;
            var team = player.PlayerTeam;
            var playerPos = player.Location;
            var selfId = player.ContainerId;
            float maxHealth = 0;
            Entity hardiest = null;

            foreach (var tm in team.Team.Members)
            {
                if (tm.EntityId == selfId)
                    continue;

                var tmEntity = tm.Entity;

                if (playerMap != tm.MapName ||
                    playerRegion != tmEntity.RegionInternalName)
                    continue;
                var curMaxHealth = tmEntity.Character.AttribsBasic.MaxHealth;
                if (curMaxHealth > maxHealth)
                {
                    if (NavigationHelper.SquareDistance3D(playerPos, tmEntity.Location) < squareRange)
                    {
                        var role = tmEntity.GetTeamRole();
                        if (role != TeamRoles.DD)
                            continue;

                        maxHealth = curMaxHealth;
                        hardiest = tmEntity;
                    }
                }
            }

            return hardiest;
        }
        /// <summary>
        /// Поиск самого живучего дамагера среди членов группы (с наибольшим максимальным HP) без учета расстояния до него
        /// </summary>
        public static Entity GetSturdiestDamageDealer()
        {
            var player = EntityManager.LocalPlayer;
            var playerRegion = player.RegionInternalName;
            var playerMap = player.MapState.MapName;
            var team = player.PlayerTeam;
            var selfId = player.ContainerId;
            float maxHealth = 0;
            Entity hardiest = null;

            foreach (var tm in team.Team.Members)
            {
                if (tm.EntityId == selfId)
                    continue;

                var tmEntity = tm.Entity;

                if (playerMap != tm.MapName ||
                    playerRegion != tmEntity.RegionInternalName)
                    continue;
                var curMaxHealth = tmEntity.Character.AttribsBasic.MaxHealth;
                if (curMaxHealth > maxHealth)
                {
                    var role = tmEntity.GetTeamRole();
                    if (role != TeamRoles.DD)
                        continue;

                    maxHealth = curMaxHealth;
                    hardiest = tmEntity;
                }
            }

            return hardiest;
        }

        /// <summary>
        /// Поиск наименее живучего члена группы (с наибольшим максимальным HP)
        /// </summary>
        /// <param name="squareRange">Квадрат радиуса, в пределах которого производится поиск</param>
        public static Entity GetWeakest(float squareRange)
        {
            var player = EntityManager.LocalPlayer;
            var playerRegion = player.RegionInternalName;
            var playerMap = player.MapState.MapName;
            var team = player.PlayerTeam;
            var playerPos = player.Location;
            var selfId = player.ContainerId;
            float maxHealth = float.MaxValue;
            Entity hardiest = null;

            foreach (var tm in team.Team.Members)
            {
                if (tm.EntityId == selfId)
                    continue;

                var tmEntity = tm.Entity;

                if (playerMap != tm.MapName ||
                    playerRegion != tmEntity.RegionInternalName)
                    continue;
                var curMaxHealth = tmEntity.Character.AttribsBasic.MaxHealth;
                if (curMaxHealth < maxHealth)
                {
                    if (NavigationHelper.SquareDistance3D(playerPos, tmEntity.Location) < squareRange)
                    {
                        maxHealth = curMaxHealth;
                        hardiest = tmEntity;
                    }
                }
            }

            return hardiest;
        }
        /// <summary>
        /// Поиск наименее живучело члена группы (с наибольшим максимальным HP) без учета расстояния до него
        /// </summary>
        public static Entity GetWeakest()
        {
            var player = EntityManager.LocalPlayer;
            var playerRegion = player.RegionInternalName;
            var playerMap = player.MapState.MapName;
            var team = player.PlayerTeam;
            var selfId = player.ContainerId;
            float maxHealth = float.MaxValue;
            Entity hardiest = null;

            foreach (var tm in team.Team.Members)
            {
                if (tm.EntityId == selfId)
                    continue;

                var tmEntity = tm.Entity;

                if (playerMap != tm.MapName ||
                    playerRegion != tmEntity.RegionInternalName)
                    continue;
                var curMaxHealth = tmEntity.Character.AttribsBasic.MaxHealth;
                if (curMaxHealth < maxHealth)
                {
                    maxHealth = curMaxHealth;
                    hardiest = tmEntity;
                }
            }

            return hardiest;
        }

        /// <summary>
        /// Поиск наименее живучего дамагера среди членов группы (с наибольшим максимальным HP)
        /// </summary>
        /// <param name="squareRange">Квадрат радиуса, в пределах которого производится поиск</param>
        public static Entity GetWeakestDamageDealer(float squareRange)
        {
            var player = EntityManager.LocalPlayer;
            var playerRegion = player.RegionInternalName;
            var playerMap = player.MapState.MapName;
            var team = player.PlayerTeam;
            var playerPos = player.Location;
            var selfId = player.ContainerId;
            float maxHealth = float.MaxValue;
            Entity hardiest = null;

            foreach (var tm in team.Team.Members)
            {
                if (tm.EntityId == selfId)
                    continue;

                var tmEntity = tm.Entity;

                if (playerMap != tm.MapName ||
                    playerRegion != tmEntity.RegionInternalName)
                    continue;
                var curMaxHealth = tmEntity.Character.AttribsBasic.MaxHealth;
                if (curMaxHealth < maxHealth)
                {
                    if (NavigationHelper.SquareDistance3D(playerPos, tmEntity.Location) < squareRange)
                    {
                        var role = tmEntity.GetTeamRole();
                        if (role != TeamRoles.DD)
                            continue;

                        maxHealth = curMaxHealth;
                        hardiest = tmEntity;
                    }
                }
            }

            return hardiest;
        }
        /// <summary>
        /// Поиск наименее живучело дамагера среди членов группы (с наибольшим максимальным HP) без учета расстояния до него
        /// </summary>
        public static Entity GetWeakestDamageDealer()
        {
            var player = EntityManager.LocalPlayer;
            var playerRegion = player.RegionInternalName;
            var playerMap = player.MapState.MapName;
            var team = player.PlayerTeam;
            var selfId = player.ContainerId;
            float maxHealth = float.MaxValue;
            Entity hardiest = null;

            foreach (var tm in team.Team.Members)
            {
                if (tm.EntityId == selfId)
                    continue;

                var tmEntity = tm.Entity;

                if (playerMap != tm.MapName ||
                    playerRegion != tmEntity.RegionInternalName)
                    continue;
                var curMaxHealth = tmEntity.Character.AttribsBasic.MaxHealth;
                if (curMaxHealth < maxHealth)
                {
                    var role = tmEntity.GetTeamRole();
                    if (role != TeamRoles.DD)
                        continue;

                    maxHealth = curMaxHealth;
                    hardiest = tmEntity;
                }
            }

            return hardiest;
        } 
#else
        /// <summary>
        /// Выбор подзащитного
        /// </summary>
        public static TeamMember GetComrade(Teammates teammates, int range)
        {
            var player = EntityManager.LocalPlayer;
            var team = player.PlayerTeam;

            if (!team.IsInTeam)
                return null;

            if (range <= 0)
                range = int.MaxValue;
            else range *= range;

            switch (teammates)
            {
                case Teammates.Leader:
                    return GetLeader(range);
                case Teammates.Tank:
                    return GetTank(range);
                case Teammates.Healer:
                    return GetHealer(range);
                case Teammates.Sturdiest:
                    return GetSturdiest(range);
                case Teammates.SturdiestDD:
                    return GetSturdiestDamageDealer(range);
                case Teammates.Weakest:
                    return GetWeakest(range);
                case Teammates.WeakestDD:
                    return GetWeakestDamageDealer(range);
            }

            return null;
        }

        /// <summary>
        /// Поиск лидера группы
        /// </summary>
        /// <param name="squareRange">Квадрат радиуса, в пределах которого производится поиск</param>
        public static TeamMember GetLeader(float squareRange)
        {
            var player = EntityManager.LocalPlayer;
            var team = player.PlayerTeam;

            if (team.IsLeader)
                return null;

#if false
            var leader = team.Team.Leader;
            var leaderEntity = leader.Entity;

            if (!leaderEntity.IsValid)
                return null;

            // EntityManager.LocalPlayer.PlayerTeam.Team.Leader не содержит достоверной информации кроме EntityId
            // поэтому следующие проверки всегда ложные
            // player.MapState.MapName == leader.MapName
            // && player.RegionInternalName == leaderEntity.RegionInternalName
            if (NavigationHelper.SquareDistance3D(player.Location, leaderEntity.Location) < squareRange &&
                    player.MapState.MapName == leader.MapName &&
                    player.RegionInternalName == leaderEntity.RegionInternalName)
            {
                return leader;
            } 
#endif
            var leaderId = team.Team.Leader.EntityId;
            var leaderTeamMember = team.Team.Members.Find((tm) => tm.EntityId == leaderId);
            if (leaderTeamMember != null)
            {
                var leaderEntity = leaderTeamMember.Entity;
                // EntityManager.LocalPlayer.PlayerTeam.Team.Leader не содержит достоверной информации кроме EntityId,
                // поэтому следующая проверка всегда ложная
                // player.MapState.MapName == leader.MapName
                // Проверку карты и региона нужно выполнять по TeamMember
                if (NavigationHelper.SquareDistance3D(player.Location, leaderEntity.Location) < squareRange
                    && player.MapState.MapName == leaderTeamMember.MapName
                    && player.RegionInternalName == leaderEntity.RegionInternalName)
                {
                    return leaderTeamMember;
                }
            }

            return null;
        }
        /// <summary>
        /// Поиск Танка среди членов группы без учета расстояния до него
        /// </summary>
        public static TeamMember GetLeader()
        {
            var player = EntityManager.LocalPlayer;
            var team = player.PlayerTeam;

            if (team.IsLeader)
                return null;

#if fale
            var leader = team.Team.Leader;
            var leaderEntity = leader.Entity;

            if (!leaderEntity.IsValid)
                return null;

            // EntityManager.LocalPlayer.PlayerTeam.Team.Leader не содержит достоверной информации кроме EntityId
            // поэтому следующие проверки всегда ложные
            // player.MapState.MapName == leader.MapName
            // && player.RegionInternalName == leaderEntity.RegionInternalName
            if (player.MapState.MapName == leader.MapName
                && player.RegionInternalName == leaderEntity.RegionInternalName)
            {
                return leader;
            }
#endif
            var leaderId = team.Team.Leader.EntityId;
            var leaderTeamMember = team.Team.Members.Find((tm) => tm.EntityId == leaderId);
            if (leaderTeamMember != null)
            {
                var leaderEntity = leaderTeamMember.Entity;
                // EntityManager.LocalPlayer.PlayerTeam.Team.Leader не содержит достоверной информации кроме EntityId,
                // поэтому следующая проверка всегда ложная
                // player.MapState.MapName == leader.MapName
                // Проверку карты и региона нужно выполнять по TeamMember
                if (player.MapState.MapName == leaderTeamMember.MapName
                    && player.RegionInternalName == leaderEntity.RegionInternalName)
                {
                    return leaderTeamMember;
                }
            }

            return null;
        }

        /// <summary>
        /// Поиск Танка среди членов группы 
        /// </summary>
        /// <param name="squareRange">Квадрат радиуса, в пределах которого производится поиск</param>
        public static TeamMember GetTank(float squareRange)
        {
            var player = EntityManager.LocalPlayer;
            var playerRegion = player.RegionInternalName;
            var playerMap = player.MapState.MapName;
            var team = player.PlayerTeam;
            var playerPos = player.Location;
            var selfId = player.ContainerId;

            foreach (var tm in team.Team.Members)
            {
                if (tm.EntityId == selfId)
                    continue;

                var tmEntity = tm.Entity;

                if (playerMap != tm.MapName ||
                    playerRegion != tmEntity.RegionInternalName)
                    continue;

                if (NavigationHelper.SquareDistance3D(playerPos, tmEntity.Location) < squareRange)
                {
                    var role = tmEntity.GetTeamRole();
                    if (role == TeamRoles.Tank)
                        return tm;
                }
            }

            return null;
        }
        /// <summary>
        /// Поиск Танка среди членов группы без учета расстояния до него
        /// </summary>
        public static TeamMember GetTank()
        {
            var player = EntityManager.LocalPlayer;
            var playerRegion = player.RegionInternalName;
            var playerMap = player.MapState.MapName;
            var team = player.PlayerTeam;
            var selfId = player.ContainerId;

            foreach (var tm in team.Team.Members)
            {
                if (tm.EntityId == selfId)
                    continue;

                var tmEntity = tm.Entity;

                if (playerMap != tm.MapName ||
                    playerRegion != tmEntity.RegionInternalName)
                    continue;

                var role = tmEntity.GetTeamRole();
                if (role == TeamRoles.Tank)
                    return tm;
            }

            return null;
        }

        /// <summary>
        /// Поиск Целителя среди членов группы 
        /// </summary>
        /// <param name="squareRange">Квадрат радиуса, в пределах которого производится поиск</param>
        public static TeamMember GetHealer(float squareRange)
        {
            var player = EntityManager.LocalPlayer;
            var playerRegion = player.RegionInternalName;
            var playerMap = player.MapState.MapName;
            var team = player.PlayerTeam;
            var playerPos = player.Location;
            var selfId = player.ContainerId;

            foreach (var tm in team.Team.Members)
            {
                if (tm.EntityId == selfId)
                    continue;

                var tmEntity = tm.Entity;

                if (playerMap != tm.MapName ||
                    playerRegion != tmEntity.RegionInternalName)
                    continue;

                if (NavigationHelper.SquareDistance3D(playerPos, tmEntity.Location) < squareRange)
                {
                    var role = tmEntity.GetTeamRole();
                    if (role == TeamRoles.Healer)
                        return tm;
                }
            }

            return null;
        }
        /// <summary>
        /// Поиск Целителя среди членов группы без учета расстояния до него
        /// </summary>
        public static TeamMember GetHealer()
        {
            var player = EntityManager.LocalPlayer;
            var playerRegion = player.RegionInternalName;
            var playerMap = player.MapState.MapName;
            var team = player.PlayerTeam;
            var selfId = player.ContainerId;

            foreach (var tm in team.Team.Members)
            {
                if (tm.EntityId == selfId)
                    continue;

                var tmEntity = tm.Entity;

                if (playerMap != tm.MapName ||
                    playerRegion != tmEntity.RegionInternalName)
                    continue;

                var role = tmEntity.GetTeamRole();
                if (role == TeamRoles.Healer)
                    return tm;
            }

            return null;
        }

        /// <summary>
        /// Поиск самого живучего члена группы (с наибольшим максимальным HP)
        /// </summary>
        /// <param name="squareRange">Квадрат радиуса, в пределах которого производится поиск</param>
        public static TeamMember GetSturdiest(float squareRange)
        {
            var player = EntityManager.LocalPlayer;
            var playerRegion = player.RegionInternalName;
            var playerMap = player.MapState.MapName;
            var team = player.PlayerTeam;
            var playerPos = player.Location;
            var selfId = player.ContainerId;
            float maxHealth = 0;
            TeamMember hardiest = null;

            foreach (var tm in team.Team.Members)
            {
                if (tm.EntityId == selfId)
                    continue;

                var tmEntity = tm.Entity;

                if (playerMap != tm.MapName ||
                    playerRegion != tmEntity.RegionInternalName)
                    continue;
                var curMaxHealth = tmEntity.Character.AttribsBasic.MaxHealth;
                if (curMaxHealth > maxHealth)
                {
                    if (NavigationHelper.SquareDistance3D(playerPos, tmEntity.Location) < squareRange)
                    {
                        maxHealth = curMaxHealth;
                        hardiest = tm;
                    }
                }
            }

            return hardiest;
        }
        /// <summary>
        /// Поиск самого живучего члена группы (с наибольшим максимальным HP) без учета расстояния до него
        /// </summary>
        public static TeamMember GetSturdiest()
        {
            var player = EntityManager.LocalPlayer;
            var playerRegion = player.RegionInternalName;
            var playerMap = player.MapState.MapName;
            var team = player.PlayerTeam;
            var selfId = player.ContainerId;
            float maxHealth = 0;
            TeamMember hardiest = null;

            foreach (var tm in team.Team.Members)
            {
                if (tm.EntityId == selfId)
                    continue;

                var tmEntity = tm.Entity;

                if (playerMap != tm.MapName ||
                    playerRegion != tmEntity.RegionInternalName)
                    continue;
                var curMaxHealth = tmEntity.Character.AttribsBasic.MaxHealth;
                if (curMaxHealth > maxHealth)
                {
                    maxHealth = curMaxHealth;
                    hardiest = tm;
                }
            }

            return hardiest;
        }

        /// <summary>
        /// Поиск самого живучего дамагера среди членов группы (с наибольшим максимальным HP)
        /// </summary>
        /// <param name="squareRange">Квадрат радиуса, в пределах которого производится поиск</param>
        public static TeamMember GetSturdiestDamageDealer(float squareRange)
        {
            var player = EntityManager.LocalPlayer;
            var playerRegion = player.RegionInternalName;
            var playerMap = player.MapState.MapName;
            var team = player.PlayerTeam;
            var playerPos = player.Location;
            var selfId = player.ContainerId;
            float maxHealth = 0;
            TeamMember hardiest = null;

            foreach (var tm in team.Team.Members)
            {
                if (tm.EntityId == selfId)
                    continue;

                var tmEntity = tm.Entity;

                if (playerMap != tm.MapName ||
                    playerRegion != tmEntity.RegionInternalName)
                    continue;
                var curMaxHealth = tmEntity.Character.AttribsBasic.MaxHealth;
                if (curMaxHealth > maxHealth)
                {
                    if (NavigationHelper.SquareDistance3D(playerPos, tmEntity.Location) < squareRange)
                    {
                        var role = tmEntity.GetTeamRole();
                        if (role != TeamRoles.DD)
                            continue;

                        maxHealth = curMaxHealth;
                        hardiest = tm;
                    }
                }
            }

            return hardiest;
        }
        /// <summary>
        /// Поиск самого живучего дамагера среди членов группы (с наибольшим максимальным HP) без учета расстояния до него
        /// </summary>
        public static TeamMember GetSturdiestDamageDealer()
        {
            var player = EntityManager.LocalPlayer;
            var playerRegion = player.RegionInternalName;
            var playerMap = player.MapState.MapName;
            var team = player.PlayerTeam;
            var selfId = player.ContainerId;
            float maxHealth = 0;
            TeamMember hardiest = null;

            foreach (var tm in team.Team.Members)
            {
                if (tm.EntityId == selfId)
                    continue;

                var tmEntity = tm.Entity;

                if (playerMap != tm.MapName ||
                    playerRegion != tmEntity.RegionInternalName)
                    continue;
                var curMaxHealth = tmEntity.Character.AttribsBasic.MaxHealth;
                if (curMaxHealth > maxHealth)
                {
                    var role = tmEntity.GetTeamRole();
                    if (role != TeamRoles.DD)
                        continue;

                    maxHealth = curMaxHealth;
                    hardiest = tm;
                }
            }

            return hardiest;
        }

        /// <summary>
        /// Поиск наименее живучего члена группы (с наибольшим максимальным HP)
        /// </summary>
        /// <param name="squareRange">Квадрат радиуса, в пределах которого производится поиск</param>
        public static TeamMember GetWeakest(float squareRange)
        {
            var player = EntityManager.LocalPlayer;
            var playerRegion = player.RegionInternalName;
            var playerMap = player.MapState.MapName;
            var team = player.PlayerTeam;
            var playerPos = player.Location;
            var selfId = player.ContainerId;
            float maxHealth = float.MaxValue;
            TeamMember hardiest = null;

            foreach (var tm in team.Team.Members)
            {
                if (tm.EntityId == selfId)
                    continue;

                var tmEntity = tm.Entity;

                if (playerMap != tm.MapName ||
                    playerRegion != tmEntity.RegionInternalName)
                    continue;
                var curMaxHealth = tmEntity.Character.AttribsBasic.MaxHealth;
                if (curMaxHealth < maxHealth)
                {
                    if (NavigationHelper.SquareDistance3D(playerPos, tmEntity.Location) < squareRange)
                    {
                        maxHealth = curMaxHealth;
                        hardiest = tm;
                    }
                }
            }

            return hardiest;
        }
        /// <summary>
        /// Поиск наименее живучело члена группы (с наибольшим максимальным HP) без учета расстояния до него
        /// </summary>
        public static TeamMember GetWeakest()
        {
            var player = EntityManager.LocalPlayer;
            var playerRegion = player.RegionInternalName;
            var playerMap = player.MapState.MapName;
            var team = player.PlayerTeam;
            var selfId = player.ContainerId;
            float maxHealth = float.MaxValue;
            TeamMember hardiest = null;

            foreach (var tm in team.Team.Members)
            {
                if (tm.EntityId == selfId)
                    continue;

                var tmEntity = tm.Entity;

                if (playerMap != tm.MapName ||
                    playerRegion != tmEntity.RegionInternalName)
                    continue;
                var curMaxHealth = tmEntity.Character.AttribsBasic.MaxHealth;
                if (curMaxHealth < maxHealth)
                {
                    maxHealth = curMaxHealth;
                    hardiest = tm;
                }
            }

            return hardiest;
        }

        /// <summary>
        /// Поиск наименее живучего дамагера среди членов группы (с наибольшим максимальным HP)
        /// </summary>
        /// <param name="squareRange">Квадрат радиуса, в пределах которого производится поиск</param>
        public static TeamMember GetWeakestDamageDealer(float squareRange)
        {
            var player = EntityManager.LocalPlayer;
            var playerRegion = player.RegionInternalName;
            var playerMap = player.MapState.MapName;
            var team = player.PlayerTeam;
            var playerPos = player.Location;
            var selfId = player.ContainerId;
            float maxHealth = float.MaxValue;
            TeamMember hardiest = null;

            foreach (var tm in team.Team.Members)
            {
                if (tm.EntityId == selfId)
                    continue;

                var tmEntity = tm.Entity;

                if (playerMap != tm.MapName ||
                    playerRegion != tmEntity.RegionInternalName)
                    continue;
                var curMaxHealth = tmEntity.Character.AttribsBasic.MaxHealth;
                if (curMaxHealth < maxHealth)
                {
                    if (NavigationHelper.SquareDistance3D(playerPos, tmEntity.Location) < squareRange)
                    {
                        var role = tmEntity.GetTeamRole();
                        if (role != TeamRoles.DD)
                            continue;

                        maxHealth = curMaxHealth;
                        hardiest = tm;
                    }
                }
            }

            return hardiest;
        }
        /// <summary>
        /// Поиск наименее живучело дамагера среди членов группы (с наибольшим максимальным HP) без учета расстояния до него
        /// </summary>
        public static TeamMember GetWeakestDamageDealer()
        {
            var player = EntityManager.LocalPlayer;
            var playerRegion = player.RegionInternalName;
            var playerMap = player.MapState.MapName;
            var team = player.PlayerTeam;
            var selfId = player.ContainerId;
            float maxHealth = float.MaxValue;
            TeamMember hardiest = null;

            foreach (var tm in team.Team.Members)
            {
                if (tm.EntityId == selfId)
                    continue;

                var tmEntity = tm.Entity;

                if (playerMap != tm.MapName ||
                    playerRegion != tmEntity.RegionInternalName)
                    continue;
                var curMaxHealth = tmEntity.Character.AttribsBasic.MaxHealth;
                if (curMaxHealth < maxHealth)
                {
                    var role = tmEntity.GetTeamRole();
                    if (role != TeamRoles.DD)
                        continue;

                    maxHealth = curMaxHealth;
                    hardiest = tm;
                }
            }

            return hardiest;
        }

        /// <summary>
        /// Поиск наиболее израненного члена члена группы (с наименьшим HP)
        /// </summary>
        /// <param name="squareRange">Квадрат радиуса, в пределах которого производится поиск</param>
        public static TeamMember GetMostInjured(float squareRange)
        {
            var player = EntityManager.LocalPlayer;
            var playerRegion = player.RegionInternalName;
            var playerMap = player.MapState.MapName;
            var team = player.PlayerTeam;
            var playerPos = player.Location;
            var selfId = player.ContainerId;
            float maxHealth = float.MaxValue;
            TeamMember hardiest = null;

            foreach (var tm in team.Team.Members)
            {
                if (tm.EntityId == selfId)
                    continue;

                var tmEntity = tm.Entity;

                if (playerMap != tm.MapName ||
                    playerRegion != tmEntity.RegionInternalName)
                    continue;
                var curMaxHealth = tmEntity.Character.AttribsBasic.Health;
                if (curMaxHealth < maxHealth)
                {
                    if (NavigationHelper.SquareDistance3D(playerPos, tmEntity.Location) < squareRange)
                    {
                        maxHealth = curMaxHealth;
                        hardiest = tm;
                    }
                }
            }

            return hardiest;
        }
        /// <summary>
        /// Поиск наиболее израненного члена группы (с наименьшим HP)
        /// </summary>
        public static TeamMember GetMostInjured()
        {
            var player = EntityManager.LocalPlayer;
            var playerRegion = player.RegionInternalName;
            var playerMap = player.MapState.MapName;
            var team = player.PlayerTeam;
            var selfId = player.ContainerId;
            float maxHealth = float.MaxValue;
            TeamMember hardiest = null;

            foreach (var tm in team.Team.Members)
            {
                if (tm.EntityId == selfId)
                    continue;

                var tmEntity = tm.Entity;

                if (playerMap != tm.MapName ||
                    playerRegion != tmEntity.RegionInternalName)
                    continue;
                var curMaxHealth = tmEntity.Character.AttribsBasic.Health;
                if (curMaxHealth < maxHealth)
                {
                    maxHealth = curMaxHealth;
                    hardiest = tm;
                }
            }

            return hardiest;
        }

        /// <summary>
        /// Поиск наиболее израненного члена дамагера (с наименьшим HP)
        /// </summary>
        /// <param name="squareRange">Квадрат радиуса, в пределах которого производится поиск</param>
        public static TeamMember GetMostInjuredDamageDealer(float squareRange)
        {
            var player = EntityManager.LocalPlayer;
            var playerRegion = player.RegionInternalName;
            var playerMap = player.MapState.MapName;
            var team = player.PlayerTeam;
            var playerPos = player.Location;
            var selfId = player.ContainerId;
            float maxHealth = float.MaxValue;
            TeamMember hardiest = null;

            foreach (var tm in team.Team.Members)
            {
                if (tm.EntityId == selfId)
                    continue;

                var tmEntity = tm.Entity;

                if (playerMap != tm.MapName ||
                    playerRegion != tmEntity.RegionInternalName)
                    continue;
                var curMaxHealth = tmEntity.Character.AttribsBasic.Health;
                if (curMaxHealth < maxHealth)
                {
                    if (NavigationHelper.SquareDistance3D(playerPos, tmEntity.Location) < squareRange)
                    {
                        var role = tmEntity.GetTeamRole();
                        if (role != TeamRoles.DD)
                            continue;

                        maxHealth = curMaxHealth;
                        hardiest = tm;
                    }
                }
            }

            return hardiest;
        }
        /// <summary>
        /// Поиск наиболее израненного дамагера (с наименьшим HP)
        /// </summary>
        public static TeamMember GetMostInjuredDamageDealer()
        {
            var player = EntityManager.LocalPlayer;
            var playerRegion = player.RegionInternalName;
            var playerMap = player.MapState.MapName;
            var team = player.PlayerTeam;
            var selfId = player.ContainerId;
            float maxHealth = float.MaxValue;
            TeamMember hardiest = null;

            foreach (var tm in team.Team.Members)
            {
                if (tm.EntityId == selfId)
                    continue;

                var tmEntity = tm.Entity;

                if (playerMap != tm.MapName ||
                    playerRegion != tmEntity.RegionInternalName)
                    continue;
                var curMaxHealth = tmEntity.Character.AttribsBasic.Health;
                if (curMaxHealth < maxHealth)
                {
                    var role = tmEntity.GetTeamRole();
                    if (role != TeamRoles.DD)
                        continue;

                    maxHealth = curMaxHealth;
                    hardiest = tm;
                }
            }

            return hardiest;
        }
#endif
        #endregion


        #region GetAttaker
        /// <summary>
        /// Выбор цели, которую атакует <param name="teammate"/>
        /// </summary>
        public static Entity GetTeammatesTarget(this Entity teammate)
        {
            Entity target = teammate.Character.CurrentTarget;
            if (!target.IsValid || target.IsDead || target.RelationToPlayer != EntityRelation.Foe)
                return null;

            return target;
        }

        /// <summary>
        /// Выбор ближайшего к игроку противника, который атакует <param name="teammate"/>
        /// </summary>
        public static Entity GetClosestToPlayerAttacker(this Entity teammate)
        {
            double minDist = float.MaxValue;
            Entity target = null;
            var teammateId = teammate.ContainerId;
            var playerPos = EntityManager.LocalPlayer.Location;

            foreach (var attacker in from entity in Astral.Logic.NW.Attackers.List
                                     where entity.Character.CurrentTarget.ContainerId == teammateId
                                     select entity)
            {
                var sqrDist = NavigationHelper.SquareDistance3D(playerPos, attacker.Location);
                if (sqrDist < minDist)
                {
                    minDist = sqrDist;
                    target = attacker;
                }
            }

            return target;
        }

        /// <summary>
        /// Выбор ближайшего к <param name="teammate"/> противника, который его атакует
        /// </summary>
        public static Entity GetClosestAttacker(this Entity teammate)
        {
            double minDist = float.MaxValue;
            Entity target = null;
            var teammateId = teammate.ContainerId;
            var teammatePos = teammate.Location;

#if true
            foreach (var attacker in from entity in Astral.Logic.NW.Attackers.List
                                     where entity.Character.CurrentTarget.ContainerId == teammateId
                                     select entity)
            {
                var sqrDist = NavigationHelper.SquareDistance3D(teammatePos, attacker.Location);
                if (sqrDist < minDist)
                {
                    minDist = sqrDist;
                    target = attacker;
                }
            }
#else
            target = Astral.Logic.NW.Attackers.List
                .Where(entity => entity.Character.CurrentTarget.ContainerId == teammateId)
                .OrderBy(entity => NavigationHelper.SquareDistance3D(teammatePos, entity.Location))
                .FirstOrDefault();
#endif

            return target;
        }


        /// <summary>
        /// Выбор наиболее стойкого противника, который атакует <param name="teammate"/> 
        /// </summary>
        public static Entity GetSturdiestAttacker(this Entity teammate)
        {
            Entity target = null;
            var teammateId = teammate.ContainerId;
            float maxHealth = 0;

            foreach (var attacker in from entity in Astral.Logic.NW.Attackers.List
                where entity.Character.CurrentTarget.ContainerId == teammateId
                select entity)
            {
                var health = attacker.Character.AttribsBasic.MaxHealth;
                if (maxHealth < health)
                {
                    maxHealth = health;
                    target = attacker;
                }
            }

            return target;
        }

        /// <summary>
        /// Выбор наменее стойкого противника, который атакует <param name="teammate"/> 
        /// </summary>
        public static Entity GetWeakestAttacker(this Entity teammate)
        {
            Entity target = null;
            var teammateId = teammate.ContainerId;
            float maxHealth = float.MaxValue;

            foreach (var attacker in from entity in Astral.Logic.NW.Attackers.List
                where entity.Character.CurrentTarget.ContainerId == teammateId
                select entity)
            {
                var health = attacker.Character.AttribsBasic.MaxHealth;
                if (maxHealth > health)
                {
                    maxHealth = health;
                    target = attacker;
                }
            }

            return target;
        }

        /// <summary>
        /// Выбор наиболее раненого противника, который атакует <param name="teammate"/> 
        /// </summary>
        public static Entity GetMostInjuredAttacker(this Entity teammate)
        {
            Entity target = null;
            var teammateId = teammate.ContainerId;
            float maxHealth = float.MaxValue;

            foreach (var attacker in from entity in Astral.Logic.NW.Attackers.List
                where entity.Character.CurrentTarget.ContainerId == teammateId
                select entity)
            {
                var health = attacker.Character.AttribsBasic.Health;
                if (maxHealth > health)
                {
                    maxHealth = health;
                    target = attacker;
                }
            }

            return target;
        }
        #endregion
    }
}
