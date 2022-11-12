using Astral.Classes;
using EntityTools.Enums;
using EntityTools.Tools.Navigation;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ACTP0Tools;
using EntityTools.Tools.Entities;

// ReSharper disable CollectionNeverUpdated.Local
// ReSharper disable InconsistentNaming

namespace EntityCore.Tools
{
    public static class PlayerTeamHelper
    {
        #region Identifiers
        /// <summary>
        /// Внутриигровой идентификатор класса Волшебник
        /// </summary>
        public const string idWizard = "Player_Controller";

        /// <summary>
        /// Внутриигровой идентификатор класса Волшебник (Чудотворец)
        /// </summary>
        public const string idWizard_Arcanist = "Player_Secondary_Spellstormmage";

        /// <summary>
        /// Внутриигровой идентификатор класса Волшебник (Арканист)
        /// </summary>
        public const string idWizard_Thaumaturge = "Player_Secondary_MasterofFlame";

        /// <summary>
        /// Внутриигровой идентификатор класса Клирик
        /// </summary>
        public const string idCleric = "Player_Devoted";

        /// <summary>
        /// Внутриигровой идентификатор класса Клирик (Благочестивец)
        /// </summary>
        public const string idCleric_Devout = "Player_Secondary_AnointedChampion";

        /// <summary>
        /// Внутриигровой идентификатор класса Клирик (Судья)
        /// </summary>
        public const string idCleric_Arbiter = "Player_Secondary_DivineOracle";

        /// <summary>
        /// Внутриигровой идентификатор класса Воин (бывший Страж)
        /// </summary>
        public const string idFighter = "Player_Guardian";

        /// <summary>
        /// Внутриигровой идентификатор класса Воин (Авангард)
        /// </summary>
        public const string idFighter_Vanguard = "Player_Secondary_Ironvanguard";

        /// <summary>
        /// Внутриигровой идентификатор класса Воин (Сорвиголова)
        /// </summary>
        public const string idFighter_Dreadnought = "Player_Secondary_Swordmaster_GF";

        /// <summary>
        /// Внутриигровой идентификатор класса Следопыт
        /// </summary>
        public const string idRanger = "Player_Archer";

        /// <summary>
        /// Внутриигровой идентификатор класса Следопыт (Охотник)
        /// </summary>
        public const string idRanger_Hunter = "Player_Secondary_Pathfinder";

        /// <summary>
        /// Внутриигровой идентификатор класса Следопыт (Хранитель)
        /// </summary>
        public const string idRanger_Warden = "Player_Secondary_Stormwarden";

        /// <summary>
        /// Внутриигровой идентификатор класса Варвар
        /// </summary>
        public const string idBarbarian = "Player_Greatweapon";

        /// <summary>
        /// Внутриигровой идентификатор класса Варвар (Страж)
        /// </summary>
        public const string idBarbarian_Sentinel = "Player_Secondary_Ironvanguard_GWF";

        /// <summary>
        /// Внутриигровой идентификатор класса Варвар (Мастер клинка)
        /// </summary>
        public const string idBarbarian_BladeMaster = "Player_Secondary_Swordmaster";

        /// <summary>
        /// Внутриигровой идентификатор класса Паладин
        /// </summary>
        public const string idPaladin = "Player_Paladin";

        /// <summary>
        /// Внутриигровой идентификатор класса Паладин (Клятвохранитель)
        /// </summary>
        public const string idPaladin_OathKeeper = "Player_Secondary_OathofDevotion";

        /// <summary>
        /// Внутриигровой идентификатор класса Паладин (Юстициар)
        /// </summary>
        public const string idPaladin_Justicar = "Player_Secondary_OathofProtection";

        /// <summary>
        /// Внутриигровой идентификатор класса Чернокнижник
        /// </summary>
        public const string idWarlock = "Player_Scourge";

        /// <summary>
        /// Внутриигровой идентификатор класса Чернокнижник (Ткач душ)
        /// </summary>
        public const string idWarlock_SoulWeaver = "Player_Secondary_Soulbinder";

        /// <summary>
        /// Внутриигровой идентификатор класса Чернокнижник (Вестник ада)
        /// </summary>
        public const string idWarlock_HellBringer = "Player_Secondary_Hellbringer";

        /// <summary>
        /// Внутриигровой идентификатор класса Плут
        /// </summary>
        public const string idRogue = "Player_Trickster";

        /// <summary>
        /// Внутриигровой идентификатор класса Плут (Убийца)
        /// </summary>
        public const string idRogue_WhisperKnife = "Player_Secondary_Whisperknife";

        /// <summary>
        /// Внутриигровой идентификатор класса Плут (Шепчущий нож)
        /// </summary>
        public const string idRogue_Assassin = "Player_Secondary_MasterInfiltrator";

        /// <summary>
        ///  Внутриигровой идентификатор класса Бард
        /// </summary>
        public const string idBard = "Player_Bard";

        /// <summary>
        ///  Внутриигровой идентификатор класса Бард (Поющий клинок)
        /// </summary>
        public const string idBard_Swashbuckler = "Player_Secondary_Swashbuckler";

        /// <summary>
        ///  Внутриигровой идентификатор класса Бард (Менестрель)
        /// </summary>
        public const string idBard_Minstrel = "Player_Secondary_Minstrel";



        /// <summary>
        /// Отображение текстового идентификатора парагона класса <param name="id"/> в соответствующий <seealso cref="PlayerParagonType"/>
        /// </summary>
        public static PlayerParagonType GetParagonType(string id)
        {
            if (!string.IsNullOrEmpty(id) &&
                id2type.TryGetValue(id, out PlayerParagonType type))
                return type;
            return PlayerParagonType.Unknown;
        }
        /// <summary>
        /// Определение <seealso cref="PlayerParagonType"/> для заданного <param name="entity"/>
        /// </summary>
        public static PlayerParagonType GetParagonType(this Entity entity)
        {
            if (entity is null)
                return PlayerParagonType.Unknown;

            var character = entity.Character;
            var id = character.CurrentPowerTreeBuild.SecondaryPaths.FirstOrDefault()?.Path.Name;
            return GetParagonType(id);
        }
        
        private static readonly Dictionary<string, PlayerParagonType> id2type =
            new Dictionary<string, PlayerParagonType>(24);

        /// <summary>
        /// Отображение типа парагона <seealso cref="PlayerParagonType"/> в соответствующий текстовый идентификатор
        /// </summary>
        public static string GetParagonId(PlayerParagonType type)
        {
            return type2id.TryGetValue(type, out string id) 
                ? id 
                : string.Empty;
        }
        /// <summary>
        /// Определение текстового идентификатора парагона класса для заданного <param name="entity"/>
        /// </summary>
        public static string GetParagonId(this Entity entity)
        {
            return entity.Character.CurrentPowerTreeBuild.SecondaryPaths.FirstOrDefault()?.Path.Name;
        }

        private static readonly Dictionary<PlayerParagonType, string> type2id =
            new Dictionary<PlayerParagonType, string>(24);

        /// <summary>
        /// Определение групповой роли <seealso cref="TeamRoles"/> по текстовому идентификатору парагона класса <param name="id"/>
        /// </summary>
        public static TeamRoles GetTeamRole(string id)
        {
            var type = GetParagonType(id);
            if (type == PlayerParagonType.Unknown)
                return TeamRoles.Unknown;
            if (((uint)TeamRoles.DD & (uint)type) > 0)
                return TeamRoles.DD;
            if (((uint)TeamRoles.Tank & (uint)type) > 0)
                return TeamRoles.Tank;
            if (((uint)TeamRoles.Healer & (uint)type) > 0)
                return TeamRoles.Healer;
            return TeamRoles.Unknown;
        }

        /// <summary>
        /// Определение групповой роли <seealso cref="TeamRoles"/> по текстовому идентификатору парагона класса <param name="type"/>
        /// </summary>
        public static TeamRoles GetTeamRole(PlayerParagonType type)
        {
            if (((uint)TeamRoles.DD & (uint)type) > 0)
                return TeamRoles.DD;
            if (((uint)TeamRoles.Tank & (uint)type) > 0)
                return TeamRoles.Tank;
            if (((uint)TeamRoles.Healer & (uint)type) > 0)
                return TeamRoles.Healer;
            return TeamRoles.Unknown;
        }

        /// <summary>
        /// Определение групповой роли <seealso cref="TeamRoles"/> для заданного <param name="entity"/>
        /// </summary>
        public static TeamRoles GetTeamRole(this Entity entity)
        {
            var id = GetParagonId(entity);

            return GetTeamRole(id);
        }
        static PlayerTeamHelper()
        {
            id2type.Add(idWizard, PlayerParagonType.Wizard);
            id2type.Add(idWizard_Arcanist, PlayerParagonType.Wizard_Arcanist);
            id2type.Add(idWizard_Thaumaturge, PlayerParagonType.Wizard_Thaumaturge);

            id2type.Add(idCleric, PlayerParagonType.Cleric);
            id2type.Add(idCleric_Devout, PlayerParagonType.Cleric_Devout);
            id2type.Add(idCleric_Arbiter, PlayerParagonType.Cleric_Arbiter);

            id2type.Add(idFighter, PlayerParagonType.Fighter);
            id2type.Add(idFighter_Vanguard, PlayerParagonType.Fighter_Vanguard);
            id2type.Add(idFighter_Dreadnought, PlayerParagonType.Fighter_Dreadnought);

            id2type.Add(idRanger, PlayerParagonType.Ranger);
            id2type.Add(idRanger_Hunter, PlayerParagonType.Ranger_Hunter);
            id2type.Add(idRanger_Warden, PlayerParagonType.Ranger_Warden);

            id2type.Add(idBarbarian, PlayerParagonType.Barbarian);
            id2type.Add(idBarbarian_Sentinel, PlayerParagonType.Barbarian_Sentinel);
            id2type.Add(idBarbarian_BladeMaster, PlayerParagonType.Barbarian_BladeMaster);

            id2type.Add(idPaladin, PlayerParagonType.Paladin);
            id2type.Add(idPaladin_OathKeeper, PlayerParagonType.Paladin_OathKeeper);
            id2type.Add(idPaladin_Justicar, PlayerParagonType.Paladin_Justicar);

            id2type.Add(idWarlock, PlayerParagonType.Warlock);
            id2type.Add(idWarlock_SoulWeaver, PlayerParagonType.Warlock_SoulWeaver);
            id2type.Add(idWarlock_HellBringer, PlayerParagonType.Warlock_HellBringer);

            id2type.Add(idRogue, PlayerParagonType.Rogue);
            id2type.Add(idRogue_WhisperKnife, PlayerParagonType.Rogue_WhisperKnife);
            id2type.Add(idRogue_Assassin, PlayerParagonType.Rogue_Assassin);

            id2type.Add(idBard, PlayerParagonType.Bard);
            id2type.Add(idBard_Swashbuckler, PlayerParagonType.Bard_Swashbuckler);
            id2type.Add(idBard_Minstrel, PlayerParagonType.Bard_Minstrel);



            type2id.Add(PlayerParagonType.Wizard, idWizard);
            type2id.Add(PlayerParagonType.Wizard_Arcanist, idWizard_Arcanist);
            type2id.Add(PlayerParagonType.Wizard_Thaumaturge, idWizard_Thaumaturge);

            type2id.Add(PlayerParagonType.Cleric, idCleric);
            type2id.Add(PlayerParagonType.Cleric_Devout, idCleric_Devout);
            type2id.Add(PlayerParagonType.Cleric_Arbiter, idCleric_Arbiter);

            type2id.Add(PlayerParagonType.Fighter, idFighter);
            type2id.Add(PlayerParagonType.Fighter_Vanguard, idFighter_Vanguard);
            type2id.Add(PlayerParagonType.Fighter_Dreadnought, idFighter_Dreadnought);

            type2id.Add(PlayerParagonType.Ranger, idRanger);
            type2id.Add(PlayerParagonType.Ranger_Hunter, idRanger_Hunter);
            type2id.Add(PlayerParagonType.Ranger_Warden, idRanger_Warden);

            type2id.Add(PlayerParagonType.Barbarian, idBarbarian);
            type2id.Add(PlayerParagonType.Barbarian_Sentinel, idBarbarian_Sentinel);
            type2id.Add(PlayerParagonType.Barbarian_BladeMaster, idBarbarian_BladeMaster);

            type2id.Add(PlayerParagonType.Paladin, idPaladin);
            type2id.Add(PlayerParagonType.Paladin_OathKeeper, idPaladin_OathKeeper);
            type2id.Add(PlayerParagonType.Paladin_Justicar, idPaladin_Justicar);

            type2id.Add(PlayerParagonType.Warlock, idWarlock);
            type2id.Add(PlayerParagonType.Warlock_SoulWeaver, idWarlock_SoulWeaver);
            type2id.Add(PlayerParagonType.Warlock_HellBringer, idWarlock_HellBringer);

            type2id.Add(PlayerParagonType.Rogue, idRogue);
            type2id.Add(PlayerParagonType.Rogue_WhisperKnife, idRogue_WhisperKnife);
            type2id.Add(PlayerParagonType.Rogue_Assassin, idRogue_Assassin);

            type2id.Add(PlayerParagonType.Bard, idBard);
            type2id.Add(PlayerParagonType.Bard_Swashbuckler, idBard_Swashbuckler);
            type2id.Add(PlayerParagonType.Bard_Minstrel, idBard_Minstrel);
        }

        #endregion


        #region Teammate selection
        /// <summary>
        /// Поиск лидера группы
        /// </summary>
        public static TeamMember GetLeader()
        {
            if (cacheTimeout.IsTimedOut
                && RegenerateCache() == 0)
                return null;
            if (!CacheIsValid())
                return null;

            if (CheckTeammate(Leader))
                return Leader;
            return null;
        }
        /// <summary>
        /// Поиск лидера группы
        /// </summary>
        /// <param name="squareRange">Квадрат радиуса, в пределах которого производится поиск</param>
        public static TeamMember GetLeader(float squareRange)
        {
            if (cacheTimeout.IsTimedOut
                && RegenerateCache() == 0)
                return null;
            if (!CacheIsValid())
                return null;

            if (CheckTeammate(Leader)
                && NavigationHelper.SquareDistance3D(EntityManager.LocalPlayer.Location, Leader.Entity.Location) < squareRange)
                return Leader;

            return null;
        }
        /// <summary>
        /// Поиск лидера группы c дополнительной проверкой <param name="specialCheck"/>
        /// </summary>
        public static TeamMember GetLeader(Predicate<Entity> specialCheck)
        {
            if (cacheTimeout.IsTimedOut
                && RegenerateCache() == 0)
                return null;
            if (!CacheIsValid())
                return null;

            if (CheckTeammate(Leader)
                && specialCheck(Leader.Entity))
                return Leader;

            return null;
        }


        /// <summary>
        /// Поиск Танка среди членов группы без учета расстояния до него
        /// </summary>
        public static TeamMember GetTank()
        {
            if (cacheTimeout.IsTimedOut
                && RegenerateCache() == 0)
                return null;
            if (!CacheIsValid())
                return null;

            if (CheckTeammate(Tank))
                return Tank;

            return null;
        }
        /// <summary>
        /// Поиск Танка среди членов группы 
        /// </summary>
        /// <param name="squareRange">Квадрат радиуса, в пределах которого производится поиск</param>
        public static TeamMember GetTank(float squareRange)
        {
            if (cacheTimeout.IsTimedOut
                && RegenerateCache() == 0)
                return null;
            if (!CacheIsValid())
                return null;

            if (CheckTeammate(Tank)
                && NavigationHelper.SquareDistance3D(EntityManager.LocalPlayer.Location, Tank.Entity.Location) < squareRange)
                return Tank;

            return null;
        }
        /// <summary>
        /// Поиск Танка среди членов группы 
        /// </summary>
        public static TeamMember GetTank(Predicate<Entity> specialCheck)
        {
            if (cacheTimeout.IsTimedOut
                && RegenerateCache() == 0)
                return null;
            if (!CacheIsValid())
                return null;

            if (CheckTeammate(Tank)
                && specialCheck(Tank.Entity))
                return Tank;

            return null;
        }


        /// <summary>
        /// Поиск Целителя среди членов группы без учета расстояния до него
        /// </summary>
        public static TeamMember GetHealer()
        {
            if (cacheTimeout.IsTimedOut
                && RegenerateCache() == 0)
                return null;
            if (!CacheIsValid())
                return null;

            if (CheckTeammate(Healer))
                return Healer;

            return null;
        }
        /// <summary>
        /// Поиск Целителя среди членов группы 
        /// </summary>
        /// <param name="squareRange">Квадрат радиуса, в пределах которого производится поиск</param>
        public static TeamMember GetHealer(float squareRange)
        {
            if (cacheTimeout.IsTimedOut
                && RegenerateCache() == 0)
                return null;
            if (!CacheIsValid())
                return null;

            if (CheckTeammate(Healer)
                && NavigationHelper.SquareDistance3D(EntityManager.LocalPlayer.Location, Healer.Entity.Location) < squareRange)
                return Healer;

            return null;
        }
        /// <summary>
        /// Поиск Целителя среди членов группы 
        /// </summary>
        public static TeamMember GetHealer(Predicate<Entity> specialCheck)
        {
            if (cacheTimeout.IsTimedOut
                && RegenerateCache() == 0)
                return null;
            if (!CacheIsValid())
                return null;

            if (CheckTeammate(Healer)
                && specialCheck(Healer.Entity))
                return Healer;

            return null;
        }

        /// <summary>
        /// Поиск самого живучего члена группы (с наибольшим максимальным HP) без учета расстояния до него
        /// </summary>
        public static TeamMember GetSturdiest()
        {
            if (cacheTimeout.IsTimedOut
                && RegenerateCache() == 0)
                return null;
            if (!CacheIsValid())
                return null;

            if (CheckTeammate(Sturdiest))
                return Sturdiest;

            return null;
        }
        /// <summary>
        /// Поиск самого живучего члена группы (с наибольшим максимальным HP)
        /// </summary>
        /// <param name="squareRange">Квадрат радиуса, в пределах которого производится поиск</param>
        public static TeamMember GetSturdiest(float squareRange)
        {
            if (cacheTimeout.IsTimedOut
                && RegenerateCache() == 0)
                return null;
            if (!CacheIsValid())
                return null;

            if (CheckTeammate(Sturdiest)
                && NavigationHelper.SquareDistance3D(EntityManager.LocalPlayer.Location, Sturdiest.Entity.Location) < squareRange)
                return Sturdiest;

            return null;
        }
        /// <summary>
        /// Поиск самого живучего члена группы (с наибольшим максимальным HP)
        /// </summary>
        public static TeamMember GetSturdiest(Predicate<Entity> specialCheck)
        {
            if (cacheTimeout.IsTimedOut
                && RegenerateCache() == 0)
                return null;
            if (!CacheIsValid())
                return null;

            if (CheckTeammate(Sturdiest)
                && specialCheck(Sturdiest.Entity))
                return Sturdiest;

            return null;
        }

        /// <summary>
        /// Поиск самого живучего дамагера среди членов группы (с наибольшим максимальным HP) без учета расстояния до него
        /// </summary>
        public static TeamMember GetSturdiestDamageDealer()
        {
            if (cacheTimeout.IsTimedOut
                && RegenerateCache() == 0)
                return null;
            if (!CacheIsValid())
                return null;

            if (CheckTeammate(SturdiestDD))
                return SturdiestDD;

            return null;
        }
        /// <summary>
        /// Поиск самого живучего дамагера среди членов группы (с наибольшим максимальным HP)
        /// </summary>
        /// <param name="squareRange">Квадрат радиуса, в пределах которого производится поиск</param>
        public static TeamMember GetSturdiestDamageDealer(float squareRange)
        {
            if (cacheTimeout.IsTimedOut
                && RegenerateCache() == 0)
                return null;
            if (!CacheIsValid())
                return null;

            if (CheckTeammate(SturdiestDD)
                && NavigationHelper.SquareDistance3D(EntityManager.LocalPlayer.Location, SturdiestDD.Entity.Location) < squareRange)
                return SturdiestDD;

            return null;
        }
        /// <summary>
        /// Поиск самого живучего дамагера среди членов группы (с наибольшим максимальным HP)
        /// </summary>
        public static TeamMember GetSturdiestDamageDealer(Predicate<Entity> specialCheck)
        {
            if (cacheTimeout.IsTimedOut
                && RegenerateCache() == 0)
                return null;
            if (!CacheIsValid())
                return null;

            if (CheckTeammate(SturdiestDD)
                && specialCheck(SturdiestDD.Entity))
                return SturdiestDD;

            return null;
        }

        /// <summary>
        /// Поиск наименее живучело члена группы (с наибольшим максимальным HP) без учета расстояния до него
        /// </summary>
        public static TeamMember GetWeakest()
        {
            if (cacheTimeout.IsTimedOut
                && RegenerateCache() == 0)
                return null;
            if (!CacheIsValid())
                return null;

            if (CheckTeammate(Weakest))
                return Weakest;

            return null;
        }
        /// <summary>
        /// Поиск наименее живучего члена группы (с наибольшим максимальным HP)
        /// </summary>
        /// <param name="squareRange">Квадрат радиуса, в пределах которого производится поиск</param>
        public static TeamMember GetWeakest(float squareRange)
        {
            if (cacheTimeout.IsTimedOut
                && RegenerateCache() == 0)
                return null;
            if (!CacheIsValid())
                return null;

            if (CheckTeammate(Weakest)
                && NavigationHelper.SquareDistance3D(EntityManager.LocalPlayer.Location, Weakest.Entity.Location) < squareRange)
                return Weakest;

            return null;
        }
        /// <summary>
        /// Поиск наименее живучего члена группы (с наибольшим максимальным HP)
        /// </summary>
        public static TeamMember GetWeakest(Predicate<Entity> specialCheck)
        {
            if (cacheTimeout.IsTimedOut
                && RegenerateCache() == 0)
                return null;
            if (!CacheIsValid())
                return null;

            if (CheckTeammate(Weakest)
                && specialCheck(Weakest.Entity))
                return Weakest;

            return null;
        }

        /// <summary>
        /// Поиск наименее живучело дамагера среди членов группы (с наибольшим максимальным HP) без учета расстояния до него
        /// </summary>
        public static TeamMember GetWeakestDamageDealer()
        {
            if (cacheTimeout.IsTimedOut
                && RegenerateCache() == 0)
                return null;
            if (!CacheIsValid())
                return null;

            if (CheckTeammate(WeakestDD))
                return WeakestDD;

            return null;
        }
        /// <summary>
        /// Поиск наименее живучего дамагера среди членов группы (с наибольшим максимальным HP)
        /// </summary>
        /// <param name="squareRange">Квадрат радиуса, в пределах которого производится поиск</param>
        public static TeamMember GetWeakestDamageDealer(float squareRange)
        {
            if (cacheTimeout.IsTimedOut
                && RegenerateCache() == 0)
                return null;
            if (!CacheIsValid())
                return null;

            if (CheckTeammate(WeakestDD)
                && NavigationHelper.SquareDistance3D(EntityManager.LocalPlayer.Location, WeakestDD.Entity.Location) < squareRange)
                return WeakestDD;

            return null;
        }
        /// <summary>
        /// Поиск наименее живучего дамагера среди членов группы (с наибольшим максимальным HP)
        /// </summary>
        public static TeamMember GetWeakestDamageDealer(Predicate<Entity> specialCheck)
        {
            if (cacheTimeout.IsTimedOut
                && RegenerateCache() == 0)
                return null;
            if (!CacheIsValid())
                return null;

            if (CheckTeammate(WeakestDD)
                && specialCheck(WeakestDD.Entity))
                return WeakestDD;

            return null;
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
        /// Поиск наиболее израненного члена члена группы (с наименьшим HP)
        /// </summary>
        public static TeamMember GetMostInjured(Predicate<Entity> specialCheck)
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
                    if (specialCheck(tmEntity))
                    {
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
            if (cacheTimeout.IsTimedOut
                && RegenerateCache() == 0)
                return null;
            if (!CacheIsValid())
                return null;

            var player = EntityManager.LocalPlayer;
            var playerInst = Player.MapInstanceNumber;
            var playerRegion = player.RegionInternalName;
            var playerMap = player.MapState.MapName;

            float minHealth = float.MaxValue;
            TeamMember mostInjured = null;

            for (int i = 0; i < DDsNum; i++)
            {
                var tm = DDs[i];
                var tmEntity = tm.Entity;

                var health = tmEntity.Character.AttribsBasic.Health;
                if (health < minHealth)
                {
                    if (playerInst != tm.MapInstanceNumber
                        || playerMap != tm.MapName
                        || playerRegion != tmEntity.RegionInternalName)
                        continue;

                    mostInjured = tm;
                    minHealth = health;
                }
            }

            return mostInjured;
        }
        /// <summary>
        /// Поиск наиболее израненного члена дамагера (с наименьшим HP)
        /// </summary>
        /// <param name="squareRange">Квадрат радиуса, в пределах которого производится поиск</param>
        public static TeamMember GetMostInjuredDamageDealer(float squareRange)
        {
            if (cacheTimeout.IsTimedOut
                && RegenerateCache() == 0)
                return null;
            if (!CacheIsValid())
                return null;

            var player = EntityManager.LocalPlayer;
            var playerInst = Player.MapInstanceNumber;
            var playerRegion = player.RegionInternalName;
            var playerMap = player.MapState.MapName;
            var playerPos = player.Location;

            float minHealth = float.MaxValue;
            TeamMember mostInjured = null;

            for (int i = 0; i < DDsNum; i++)
            {
                var tm = DDs[i];
                var tmEntity = tm.Entity;

                var health = tmEntity.Character.AttribsBasic.Health;
                if (health < minHealth)
                {
                    if (NavigationHelper.SquareDistance3D(playerPos, tmEntity.Location) < squareRange)
                    {
                        if (playerInst != tm.MapInstanceNumber
                            || playerMap != tm.MapName 
                            || playerRegion != tmEntity.RegionInternalName)
                            continue;

                        minHealth = health;
                        mostInjured = tm;
                    }
                }
            }
            return mostInjured;
        }
        /// <summary>
        /// Поиск наиболее израненного члена дамагера (с наименьшим HP)
        /// </summary>
        public static TeamMember GetMostInjuredDamageDealer(Predicate<Entity> specialCheck)
        {

            if (cacheTimeout.IsTimedOut
                && RegenerateCache() == 0)
                return null;
            if (!CacheIsValid())
                return null;

            var player = EntityManager.LocalPlayer;
            var playerInst = Player.MapInstanceNumber;
            var playerRegion = player.RegionInternalName;
            var playerMap = player.MapState.MapName;

            float minHealth = float.MaxValue;
            TeamMember mostInjured = null;

            for (int i = 0; i < DDsNum; i++)
            {
                var tm = DDs[i];
                var tmEntity = tm.Entity;

                var health = tmEntity.Character.AttribsBasic.Health;
                if (health < minHealth)
                {
                    if (specialCheck(tmEntity))
                    {
                        if (playerInst != tm.MapInstanceNumber
                            || playerMap != tm.MapName
                            || playerRegion != tmEntity.RegionInternalName)
                            continue;

                        minHealth = health;
                        mostInjured = tm;
                    }
                }
            }
            return mostInjured;
        }

        private static TeamMember GetMostInjuredTeamMember(TeamMember[] members, uint memberNum, Predicate<Entity> specialCheck)
        {
            if (cacheTimeout.IsTimedOut
                && RegenerateCache() == 0)
                return null;
            if (!CacheIsValid())
                return null;

            var player = EntityManager.LocalPlayer;
            var playerInst = Player.MapInstanceNumber;
            var playerRegion = player.RegionInternalName;
            var playerMap = player.MapState.MapName;

            float minHealth = float.MaxValue;
            TeamMember mostInjured = null;

            for (int i = 0; i < memberNum; i++)
            {
                var tm = members[i];
                var tmEntity = tm.Entity;

                var health = tmEntity.Character.AttribsBasic.Health;
                if (health < minHealth)
                {
                    if (specialCheck(tmEntity))
                    {
                        if (playerInst != tm.MapInstanceNumber
                            || playerMap != tm.MapName
                            || playerRegion != tmEntity.RegionInternalName)
                            continue;

                        minHealth = health;
                        mostInjured = tm;
                    }
                }
            }
            return mostInjured;
        }
        
        private static TeamMember GetWeakestTeamMember(TeamMember[] members, uint memberNum, Predicate<Entity> specialCheck)
        {
            if (cacheTimeout.IsTimedOut
                && RegenerateCache() == 0)
                return null;
            if (!CacheIsValid())
                return null;

            var player = EntityManager.LocalPlayer;
            var playerInst = Player.MapInstanceNumber;
            var playerRegion = player.RegionInternalName;
            var playerMap = player.MapState.MapName;

            float minHealth = float.MaxValue;
            TeamMember weakest = null;

            for (int i = 0; i < memberNum; i++)
            {
                var tm = members[i];
                var tmEntity = tm.Entity;

                var health = tmEntity.Character.AttribsBasic.MaxHealth;
                if (health < minHealth)
                {
                    if (specialCheck(tmEntity))
                    {
                        if (playerInst != tm.MapInstanceNumber
                            || playerMap != tm.MapName
                            || playerRegion != tmEntity.RegionInternalName)
                            continue;

                        minHealth = health;
                        weakest = tm;
                    }
                }
            }
            return weakest;
        }
        
        private static TeamMember GetSturdiestTeamMember(TeamMember[] members, uint memberNum, Predicate<Entity> specialCheck)
        {

            if (cacheTimeout.IsTimedOut
                && RegenerateCache() == 0)
                return null;
            if (!CacheIsValid())
                return null;

            var player = EntityManager.LocalPlayer;
            var playerInst = Player.MapInstanceNumber;
            var playerRegion = player.RegionInternalName;
            var playerMap = player.MapState.MapName;

            float maxHealth = 0;
            TeamMember sturdiest = null;

            for (int i = 0; i < memberNum; i++)
            {
                var tm = members[i];
                var tmEntity = tm.Entity;

                var tmMaxHealth = tmEntity.Character.AttribsBasic.MaxHealth;
                if (tmMaxHealth > maxHealth)
                {
                    if (specialCheck(tmEntity))
                    {
                        if (playerInst != tm.MapInstanceNumber
                            || playerMap != tm.MapName
                            || playerRegion != tmEntity.RegionInternalName)
                            continue;

                        maxHealth = tmMaxHealth;
                        sturdiest = tm;
                    }
                }
            }
            return sturdiest;
        }

        private static uint CachedTeamId;
        private static TeamMember Leader;
        private static int TanksNum;
        private static TeamMember Tank;
        private static readonly TeamMember[] Tanks = new TeamMember[5];
        private static int HealersNum;
        private static TeamMember Healer;
        private static readonly TeamMember[] Healers = new TeamMember[5];

        private static int DDsNum;
        private static readonly TeamMember[] DDs = new TeamMember[5];
        private static TeamMember Sturdiest;
        private static TeamMember SturdiestDD;
        private static TeamMember Weakest;
        private static TeamMember WeakestDD;
        private static TeamMember Player;
        private static readonly Timeout cacheTimeout = new Timeout(0);


        /// <summary>
        /// Проверка и классификация всех членов группы с распределением по группам
        /// </summary>
        private static uint RegenerateCache()
        {
            //BUG Периодически возникает ошибка "Индекс находится вне границ массива"
            Sturdiest = null;
            SturdiestDD = null;
            Weakest = null;
            WeakestDD = null;

            Tank = null;
            Healer = null;
            Player = null;
            
            TanksNum = 0;
            HealersNum = 0;
            DDsNum = 0;
            CachedTeamId = 0;

            var player = EntityManager.LocalPlayer;
            var team = player.PlayerTeam;

            if (!team.IsInTeam)
                return 0;

            var selfId = player.ContainerId;

            if (team.IsLeader)
                Leader = null;

            var leaderId = team.Team.Leader.EntityId;
            float minHealth = float.MaxValue;
            float minDdHealth = float.MaxValue;
            float maxHealth = 0;
            float maxDdHealth = 0;
            float maxTankHealth = 0;
            float maxHealerHealth = 0;

            // Разворачиваем цикл в обратном порядке
            var members = team.Team.Members.ToArray();
            var teamCount = members.Length;

            switch (teamCount)
            {
                case 5: goto TeamMember_4;
                case 4: goto TeamMember_3;
                case 3: goto TeamMember_2;
                case 2: goto TeamMember_1;
                //case 1: goto TeamMember_0;
                default: goto RegenerateCacheFinish;
            }

        TeamMember_4:
            var teamMember = members[4];
            if (teamMember.EntityId == selfId)
            {
                Player = teamMember;
            }
            else
            {
                var tmEntity = teamMember.Entity;

                var health = tmEntity.Character.AttribsBasic.MaxHealth;

                if (health > maxHealth)
                {
                    maxHealth = health;
                    Sturdiest = teamMember;
                }
                else if (health < minHealth)
                {
                    minHealth = health;
                    Weakest = teamMember;
                }

                if (teamMember.EntityId == leaderId)
                {
                    Leader = teamMember;
                }

                var role = tmEntity.GetTeamRole();
                switch (role)
                {
                    case TeamRoles.Tank:
                        Tanks[TanksNum] = teamMember;
                        TanksNum++;
                        if (health > maxTankHealth)
                        {
                            Tank = teamMember;
                            maxTankHealth = health;
                        }
                        break;
                    case TeamRoles.Healer:
                        Healers[HealersNum] = teamMember;
                        HealersNum++;
                        if (health > maxHealerHealth)
                        {
                            Healer = teamMember;
                            maxHealerHealth = health;
                        }
                        break;
                    //case TeamRoles.DD:
                    //case TeamRoles.Unknown:
                    default:
                        DDs[DDsNum] = teamMember;
                        DDsNum++;
                        if (health > maxDdHealth)
                        {
                            maxDdHealth = health;
                            SturdiestDD = teamMember;
                        }
                        else if (health < minDdHealth)
                        {
                            minDdHealth = health;
                            WeakestDD = teamMember;
                        }
                        break;
                }
            }

        TeamMember_3:
            teamMember = members[3];
            if (teamMember.EntityId == selfId)
            {
                Player = teamMember;
            }
            else
            {
                var tmEntity = teamMember.Entity;

                var health = tmEntity.Character.AttribsBasic.MaxHealth;

                if (health > maxHealth)
                {
                    maxHealth = health;
                    Sturdiest = teamMember;
                }
                else if (health < minHealth)
                {
                    minHealth = health;
                    Weakest = teamMember;
                }

                if (teamMember.EntityId == leaderId)
                {
                    Leader = teamMember;
                }

                var role = tmEntity.GetTeamRole();
                switch (role)
                {
                    case TeamRoles.Tank:
                        Tanks[TanksNum] = teamMember;
                        TanksNum++;
                        if (health > maxTankHealth)
                        {
                            Tank = teamMember;
                            maxTankHealth = health;
                        }
                        break;
                    case TeamRoles.Healer:
                        Healers[HealersNum] = teamMember;
                        HealersNum++;
                        if (health > maxHealerHealth)
                        {
                            Healer = teamMember;
                            maxHealerHealth = health;
                        }
                        break;
                    //case TeamRoles.DD:
                    //case TeamRoles.Unknown:
                    default:
                        DDs[DDsNum] = teamMember;
                        DDsNum++;
                        if (health > maxDdHealth)
                        {
                            maxDdHealth = health;
                            SturdiestDD = teamMember;
                        }
                        else if (health < minDdHealth)
                        {
                            minDdHealth = health;
                            WeakestDD = teamMember;
                        }
                        break;
                }
            }

        TeamMember_2:
            teamMember = members[2];
            if (teamMember.EntityId == selfId)
            {
                Player = teamMember;
            }
            else
            {
                var tmEntity = teamMember.Entity;

                var health = tmEntity.Character.AttribsBasic.MaxHealth;

                if (health > maxHealth)
                {
                    maxHealth = health;
                    Sturdiest = teamMember;
                }
                else if (health < minHealth)
                {
                    minHealth = health;
                    Weakest = teamMember;
                }

                if (teamMember.EntityId == leaderId)
                {
                    Leader = teamMember;
                }

                var role = tmEntity.GetTeamRole();
                switch (role)
                {
                    case TeamRoles.Tank:
                        Tanks[TanksNum] = teamMember;
                        TanksNum++;
                        if (health > maxTankHealth)
                        {
                            Tank = teamMember;
                            maxTankHealth = health;
                        }
                        break;
                    case TeamRoles.Healer:
                        Healers[HealersNum] = teamMember;
                        HealersNum++;
                        if (health > maxHealerHealth)
                        {
                            Healer = teamMember;
                            maxHealerHealth = health;
                        }
                        break;
                    //case TeamRoles.DD:
                    //case TeamRoles.Unknown:
                    default:
                        DDs[DDsNum] = teamMember;
                        DDsNum++;
                        if (health > maxDdHealth)
                        {
                            maxDdHealth = health;
                            SturdiestDD = teamMember;
                        }
                        else if (health < minDdHealth)
                        {
                            minDdHealth = health;
                            WeakestDD = teamMember;
                        }
                        break;
                }
            }

        TeamMember_1:
            teamMember = members[1];
            if (teamMember.EntityId == selfId)
            {
                Player = teamMember;
            }
            else
            {
                var tmEntity = teamMember.Entity;

                var health = tmEntity.Character.AttribsBasic.MaxHealth;

                if (health > maxHealth)
                {
                    maxHealth = health;
                    Sturdiest = teamMember;
                }
                else if (health < minHealth)
                {
                    minHealth = health;
                    Weakest = teamMember;
                }

                if (teamMember.EntityId == leaderId)
                {
                    Leader = teamMember;
                }

                var role = tmEntity.GetTeamRole();
                switch (role)
                {
                    case TeamRoles.Tank:
                        Tanks[TanksNum] = teamMember;
                        TanksNum++;
                        if (health > maxTankHealth)
                        {
                            Tank = teamMember;
                            maxTankHealth = health;
                        }
                        break;
                    case TeamRoles.Healer:
                        Healers[HealersNum] = teamMember;
                        HealersNum++;
                        if (health > maxHealerHealth)
                        {
                            Healer = teamMember;
                            maxHealerHealth = health;
                        }
                        break;
                    //case TeamRoles.DD:
                    //case TeamRoles.Unknown:
                    default:
                        DDs[DDsNum] = teamMember;
                        DDsNum++;
                        if (health > maxDdHealth)
                        {
                            maxDdHealth = health;
                            SturdiestDD = teamMember;
                        }
                        else if (health < minDdHealth)
                        {
                            minDdHealth = health;
                            WeakestDD = teamMember;
                        }
                        break;
                }
            }
            
        //TeamMember_0:
            teamMember = members[0];
            if (teamMember.EntityId == selfId)
            {
                Player = teamMember;
            }
            else
            {
                var tmEntity = teamMember.Entity;

                var health = tmEntity.Character.AttribsBasic.MaxHealth;

                if (health > maxHealth)
                {
                    //maxHealth = health;
                    Sturdiest = teamMember;
                }
                else if (health < minHealth)
                {
                    //minHealth = health;
                    Weakest = teamMember;
                }

                if (teamMember.EntityId == leaderId)
                {
                    Leader = teamMember;
                }

                var role = tmEntity.GetTeamRole();
                switch (role)
                {
                    case TeamRoles.Tank:
                        Tanks[TanksNum] = teamMember;
                        TanksNum++;
                        if (health > maxTankHealth)
                        {
                            Tank = teamMember;
                            //maxTankHealth = health;
                        }
                        break;
                    case TeamRoles.Healer:
                        Healers[HealersNum] = teamMember;
                        HealersNum++;
                        if (health > maxHealerHealth)
                        {
                            Healer = teamMember;
                            //maxHealerHealth = health;
                        }
                        break;
                    //case TeamRoles.DD:
                    //case TeamRoles.Unknown:
                    default:
                        DDs[DDsNum] = teamMember;
                        DDsNum++;
                        if (health > maxDdHealth)
                        {
                            SturdiestDD = teamMember;
                            //maxDdHealth = health;
                        }
                        else if (health < minDdHealth)
                        {
                            WeakestDD = teamMember;
                            //minDdHealth = health;
                        }
                        break;
                }
            }

        RegenerateCacheFinish:
            cacheTimeout.ChangeTime(player.InCombat
                ? EntityTools.EntityTools.Config.EntityCache.CombatCacheTime
                : EntityTools.EntityTools.Config.EntityCache.GlobalCacheTime);
            return CachedTeamId = team.TeamId;
        }

        /// <summary>
        /// Проверка валидности кэша по идентификатору группы
        /// </summary>
        /// <returns></returns>
        private static bool CacheIsValid()
        {
            if (Player is null || CachedTeamId == 0)
                return false;

            var player = EntityManager.LocalPlayer;
            var team = player.PlayerTeam;

            if (!team.IsInTeam)
                return false;

            if (CachedTeamId != team.TeamId)
                return false;

            return true;
        }

        private static bool CheckTeammate(TeamMember teammate)
        {
            if (teammate == null || !teammate.IsValid)
                return false;

            if (Player is null || CachedTeamId == 0)
                return false;

            if (Player.EntityId != EntityManager.LocalPlayer.ContainerId)
                return false;

            return Player.MapInstanceNumber == teammate.MapInstanceNumber
                   && Player.MapName == teammate.MapName
                   && Player.Entity.RegionInternalName == teammate.Entity.RegionInternalName;
        }
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

        /// <summary>
        /// Вспомогательный класс для отслеживания внутреннего состояния <seealso cref="PlayerTeamHelper"/>
        /// </summary>
        public class Monitor
        {
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public object PlayerTarget => new SimpleEntityWrapper(AstralAccessors.Logic.UCC.Core.CurrentTarget);

            [TypeConverter(typeof(ExpandableObjectConverter))]
            [Category("Roles")]
            public object Tank => new SimpleEntityWrapper(GetTank());

            [TypeConverter(typeof(ExpandableObjectConverter))]
            [Category("Roles")]
            public object Healer => new SimpleEntityWrapper(GetHealer());

            [TypeConverter(typeof(ExpandableObjectConverter))]
            [Category("Endurance")]
            public object Sturdiest => new SimpleEntityWrapper(GetSturdiest());

            [TypeConverter(typeof(ExpandableObjectConverter))]
            [Category("Endurance")]
            public object SturdiestDD => new SimpleEntityWrapper(GetSturdiestDamageDealer());

            [TypeConverter(typeof(ExpandableObjectConverter))]
            [Category("Endurance")]
            public object Weakest => new SimpleEntityWrapper(GetWeakest());

            [TypeConverter(typeof(ExpandableObjectConverter))]
            [Category("Endurance")]
            public object WeakestDD => new SimpleEntityWrapper(GetWeakestDamageDealer());

            [TypeConverter(typeof(ExpandableObjectConverter))]
            [Category("Endurance")]
            public object MostInjured => new SimpleEntityWrapper(GetMostInjured());

            [TypeConverter(typeof(ExpandableObjectConverter))]
            [Category("Endurance")]
            public object MostInjuredDD => new SimpleEntityWrapper(GetMostInjuredDamageDealer());

            [Category("Team")]
            public object TeamId => CachedTeamId;

            [TypeConverter(typeof(ExpandableObjectConverter))]
            [Category("Team")]
            public object Leader => new SimpleEntityWrapper(GetLeader());
            
            [TypeConverter(typeof(ExpandableObjectConverter))]
            [Category("Team")]
            public object Teammate_0
            {
                get
                {
                    var team = EntityManager.LocalPlayer.PlayerTeam.Team;
                    return new SimpleEntityWrapper(team.MembersCount > 0 ? team.Members[0] : null);
                }
            }

            [TypeConverter(typeof(ExpandableObjectConverter))]
            [Category("Team")]
            public object Teammate_1
            {
                get
                {
                    var team = EntityManager.LocalPlayer.PlayerTeam.Team;
                    return new SimpleEntityWrapper(team.MembersCount > 1 ? team.Members[1] : null);
                }
            }

            [TypeConverter(typeof(ExpandableObjectConverter))]
            [Category("Team")]
            public object Teammate_2
            {
                get
                {
                    var team = EntityManager.LocalPlayer.PlayerTeam.Team;
                    return new SimpleEntityWrapper(team.MembersCount > 2 ? team.Members[2] : null);
                }
            }

            [TypeConverter(typeof(ExpandableObjectConverter))]
            [Category("Team")]
            public object Teammate_3
            {
                get
                {
                    var team = EntityManager.LocalPlayer.PlayerTeam.Team;
                    return new SimpleEntityWrapper(team.MembersCount > 3 ? team.Members[3] : null);
                }
            }

            [TypeConverter(typeof(ExpandableObjectConverter))]
            [Category("Team")]
            public object Teammate_4
            {
                get
                {
                    var team = EntityManager.LocalPlayer.PlayerTeam.Team;
                    return new SimpleEntityWrapper(team.MembersCount > 4 ? team.Members[4] : null);
                }
            }
        }
    }

    /// <summary>
    /// Роль персонажей в группе
    /// </summary>
    public enum TeamRoles : uint
    {
        /// <summary>
        /// Неизвестный тип
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Танк
        /// </summary>
        Tank = PlayerParagonType.Barbarian_Sentinel |
               PlayerParagonType.Fighter_Vanguard |
               PlayerParagonType.Paladin_Justicar,

        /// <summary>
        /// Целитель
        /// </summary>
        Healer = PlayerParagonType.Cleric_Devout |
                 PlayerParagonType.Warlock_SoulWeaver |
                 PlayerParagonType.Paladin_OathKeeper |
                 PlayerParagonType.Bard_Minstrel,

        /// <summary>
        /// Дамагер
        /// </summary>
        DD = PlayerParagonType.Ranger_Hunter |
             PlayerParagonType.Ranger_Warden |
             PlayerParagonType.Ranger |
             PlayerParagonType.Wizard_Arcanist |
             PlayerParagonType.Wizard_Thaumaturge |
             PlayerParagonType.Wizard |
             PlayerParagonType.Cleric_Arbiter |
             PlayerParagonType.Barbarian_BladeMaster |
             PlayerParagonType.Fighter_Dreadnought |
             PlayerParagonType.Warlock_HellBringer |
             PlayerParagonType.Rogue_Assassin |
             PlayerParagonType.Rogue_WhisperKnife |
             PlayerParagonType.Rogue |
             PlayerParagonType.Bard_Swashbuckler
    }
}
