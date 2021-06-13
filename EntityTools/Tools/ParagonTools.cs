using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MyNW.Classes;
using MyNW.Internals;
// ReSharper disable InconsistentNaming

namespace EntityTools.Tools
{
    public static class ParagonTools
    {
        /// <summary>
        /// Внутриигровой идентификатор класса Волшебник
        /// </summary>
        public static readonly string idWizard = "Player_Controller";
        /// <summary>
        /// Внутриигровой идентификатор класса Волшебник (Чудотворец)
        /// </summary>
        public static readonly string idWizard_Arcanist = "Player_Secondary_Spellstormmage";
        /// <summary>
        /// Внутриигровой идентификатор класса Волшебник (Арканист)
        /// </summary>
        public static readonly string idWizard_Thaumaturge = "Player_Secondary_MasterofFlame";

        /// <summary>
        /// Внутриигровой идентификатор класса Клирик
        /// </summary>
        public static readonly string idCleric = "Player_Devoted";
        /// <summary>
        /// Внутриигровой идентификатор класса Клирик (Благочестивец)
        /// </summary>
        public static readonly string idCleric_Devout = "Player_Secondary_AnointedChampion";
        /// <summary>
        /// Внутриигровой идентификатор класса Клирик (Судья)
        /// </summary>
        public static readonly string idCleric_Arbiter = "Player_Secondary_DivineOracle";

        /// <summary>
        /// Внутриигровой идентификатор класса Воин (бывший Страж)
        /// </summary>
        public static readonly string idFighter = "Player_Guardian";
        /// <summary>
        /// Внутриигровой идентификатор класса Воин (Авангард)
        /// </summary>
        public static readonly string idFighter_Vanguard = "Player_Secondary_Ironvanguard";
        /// <summary>
        /// Внутриигровой идентификатор класса Воин (Сорвиголова)
        /// </summary>
        public static readonly string idFighter_Dreadnought = "Player_Secondary_Swordmaster_GF";

        /// <summary>
        /// Внутриигровой идентификатор класса Следопыт
        /// </summary>
        public static readonly string idRanger = "Player_Archer";
        /// <summary>
        /// Внутриигровой идентификатор класса Следопыт (Охотник)
        /// </summary>
        public static readonly string idRanger_Hunter = "Player_Secondary_Pathfinder";
        /// <summary>
        /// Внутриигровой идентификатор класса Следопыт (Хранитель)
        /// </summary>
        public static readonly string idRanger_Warden = "Player_Secondary_Stormwarden";

        /// <summary>
        /// Внутриигровой идентификатор класса Варвар
        /// </summary>
        public static readonly string idBarbarian = "Player_Greatweapon";
        /// <summary>
        /// Внутриигровой идентификатор класса Варвар (Страж)
        /// </summary>
        public static readonly string idBarbarian_Sentinel = "Player_Secondary_Ironvanguard_GWF";
        /// <summary>
        /// Внутриигровой идентификатор класса Варвар (Мастер клинка)
        /// </summary>
        public static readonly string idBarbarian_BladeMaster = "Player_Secondary_Swordmaster";

        /// <summary>
        /// Внутриигровой идентификатор класса Паладин
        /// </summary>
        public static readonly string idPaladin = "Player_Paladin";
        /// <summary>
        /// Внутриигровой идентификатор класса Паладин (Клятвохранитель)
        /// </summary>
        public static readonly string idPaladin_OathKeeper = "Player_Secondary_OathofDevotion";
        /// <summary>
        /// Внутриигровой идентификатор класса Паладин (Юстициар)
        /// </summary>
        public static readonly string idPaladin_Justicar = "Player_Secondary_OathofProtection";

        /// <summary>
        /// Внутриигровой идентификатор класса Чернокнижник
        /// </summary>
        public static readonly string idWarlock = "Player_Scourge";
        /// <summary>
        /// Внутриигровой идентификатор класса Чернокнижник (Ткач душ)
        /// </summary>
        public static readonly string idWarlock_SoulWeaver = "Player_Secondary_Soulbinder";
        /// <summary>
        /// Внутриигровой идентификатор класса Чернокнижник (Вестник ада)
        /// </summary>
        public static readonly string idWarlock_HellBringer = "Player_Secondary_Hellbringer";

        /// <summary>
        /// Внутриигровой идентификатор класса Плут
        /// </summary>
        public static readonly string idRogue = "Player_Trickster";
        /// <summary>
        /// Внутриигровой идентификатор класса Плут (Убийца)
        /// </summary>
        public static readonly string idRogue_WhisperKnife = "Player_Secondary_Whisperknife";
        /// <summary>
        /// Внутриигровой идентификатор класса Плут (Шепчущий нож)
        /// </summary>
        public static readonly string idRogue_Assassin = "Player_Secondary_MasterInfiltrator";

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
            
            var id = entity.Character.CurrentPowerTreeBuild.SecondaryPaths.FirstOrDefault()?.Path.Name;
            return GetParagonType(id);
        }
        private static readonly Dictionary<string, PlayerParagonType> id2type =
            new Dictionary<string, PlayerParagonType>(24);


        /// <summary>
        /// Отображение типа парагона <seealso cref="PlayerParagonType"/> в соответствующий текстовый идентификатор
        /// </summary>
        public static string GetParagonId(PlayerParagonType type)
        {
            if (type2id.TryGetValue(type, out string id))
                return id;
            return string.Empty;
        }
        /// <summary>
        /// Определение текстового идентификатора парагона класса <param name="id"/> для заданного <param name="entity"/>
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
            if (((uint) TeamRoles.Tank & (uint)type) > 0)
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


        static ParagonTools()
        {
            id2type.Add(idWizard, PlayerParagonType.Wizard);
            id2type.Add(idWizard_Arcanist, PlayerParagonType.Wizard_Arcanist);
            id2type.Add(idWizard_Thaumaturge, PlayerParagonType.Wizard_Thaumaturge);
            
            id2type.Add(idCleric, PlayerParagonType.Cleric);
            id2type.Add(idCleric_Arbiter, PlayerParagonType.Cleric_Arbiter);
            id2type.Add(idCleric_Devout, PlayerParagonType.Cleric_Devout);
            
            id2type.Add(idFighter, PlayerParagonType.Fighter);
            id2type.Add(idFighter_Dreadnought, PlayerParagonType.Fighter_Dreadnought);
            id2type.Add(idFighter_Vanguard, PlayerParagonType.Fighter_Vanguard);
            
            id2type.Add(idRanger, PlayerParagonType.Ranger);
            id2type.Add(idRanger_Hunter, PlayerParagonType.Ranger_Hunter);
            id2type.Add(idRanger_Warden, PlayerParagonType.Ranger_Warden);
            
            id2type.Add(idBarbarian, PlayerParagonType.Barbarian);
            id2type.Add(idBarbarian_BladeMaster, PlayerParagonType.Barbarian_BladeMaster);
            id2type.Add(idBarbarian_Sentinel, PlayerParagonType.Barbarian_Sentinel);
            
            id2type.Add(idPaladin, PlayerParagonType.Paladin);
            id2type.Add(idPaladin_Justicar, PlayerParagonType.Paladin_Justicar);
            id2type.Add(idPaladin_OathKeeper, PlayerParagonType.Paladin_OathKeeper);
            
            id2type.Add(idWarlock, PlayerParagonType.Warlock);
            id2type.Add(idWarlock_HellBringer, PlayerParagonType.Warlock_HellBringer);
            id2type.Add(idWarlock_SoulWeaver, PlayerParagonType.Warlock_SoulWeaver);

            id2type.Add(idRogue, PlayerParagonType.Rogue);
            id2type.Add(idRogue_Assassin, PlayerParagonType.Rogue_Assassin);
            id2type.Add(idRogue_WhisperKnife, PlayerParagonType.Rogue_WhisperKnife);


            type2id.Add(PlayerParagonType.Wizard, idWizard);
            type2id.Add(PlayerParagonType.Wizard_Arcanist, idWizard_Arcanist);
            type2id.Add(PlayerParagonType.Wizard_Thaumaturge, idWizard_Thaumaturge);

            type2id.Add(PlayerParagonType.Cleric, idCleric);
            type2id.Add(PlayerParagonType.Cleric_Arbiter, idCleric_Arbiter);
            type2id.Add(PlayerParagonType.Cleric_Devout, idCleric_Devout);

            type2id.Add(PlayerParagonType.Fighter, idFighter);
            type2id.Add(PlayerParagonType.Fighter_Dreadnought, idFighter_Dreadnought);
            type2id.Add(PlayerParagonType.Fighter_Vanguard, idFighter_Vanguard);

            type2id.Add(PlayerParagonType.Ranger, idRanger);
            type2id.Add(PlayerParagonType.Ranger_Hunter, idRanger_Hunter);
            type2id.Add(PlayerParagonType.Ranger_Warden, idRanger_Warden);

            type2id.Add(PlayerParagonType.Barbarian, idBarbarian);
            type2id.Add(PlayerParagonType.Barbarian_BladeMaster, idBarbarian_BladeMaster);
            type2id.Add(PlayerParagonType.Barbarian_Sentinel, idBarbarian_Sentinel);

            type2id.Add(PlayerParagonType.Paladin, idPaladin);
            type2id.Add(PlayerParagonType.Paladin_Justicar, idPaladin_Justicar);
            type2id.Add(PlayerParagonType.Paladin_OathKeeper, idPaladin_OathKeeper);

            type2id.Add(PlayerParagonType.Warlock, idWarlock);
            type2id.Add(PlayerParagonType.Warlock_HellBringer, idWarlock_HellBringer);
            type2id.Add(PlayerParagonType.Warlock_SoulWeaver, idWarlock_SoulWeaver);

            type2id.Add(PlayerParagonType.Rogue, idRogue);
            type2id.Add(PlayerParagonType.Rogue_Assassin, idRogue_Assassin);
            type2id.Add(PlayerParagonType.Rogue_WhisperKnife, idRogue_WhisperKnife);
        }
    }

    [Serializable]
    public class SelectedParagons
    {
        //public bool Wizard { get; set; }
        public bool Wizard_Thaumaturge { get; set; }
        public bool Wizard_Arcanist { get; set; }
        //public bool Cleric { get; set; }
        public bool Cleric_Arbiter { get; set; }
        public bool Cleric_Devout { get; set; }
        //public bool Barbarian { get; set; }
        public bool Barbarian_Blademaster { get; set; }
        public bool Barbarian_Sentinel { get; set; }
        //public bool Fighter { get; set; }
        public bool Fighter_Dreadnought { get; set; }
        public bool Fighter_Vanguard { get; set; }
        //public bool Ranger { get; set; }
        public bool Ranger_Warden { get; set; }
        public bool Ranger_Hunter { get; set; }
        //public bool Paladin { get; set; }
        public bool Paladin_Oathkeeper { get; set; }
        public bool Paladin_Justicar { get; set; }
        //public bool Warlock { get; set; }
        public bool Warlock_Hellbringer { get; set; }
        public bool Warlock_Soulweaver { get; set; }
        //public bool Rogue { get; set; }
        public bool Rogue_Whisperknife { get; set; }
        public bool Rogue_Assassin { get; set; }

        public bool IsValid
        {
            get
            {
                string currentParagon = EntityManager.LocalPlayer.Character.CurrentPowerTreeBuild.SecondaryPaths.FirstOrDefault()?.Path.Name;

                // Реализовать через Switch не удалось, т.к. case допускает только статичную строку.
                if (currentParagon == ParagonTools.idWizard_Thaumaturge)
                    return Wizard_Thaumaturge;
                if (currentParagon == ParagonTools.idWizard_Arcanist)
                        return Wizard_Arcanist;
                if (currentParagon == ParagonTools.idCleric_Arbiter)
                    return Cleric_Arbiter;
                if (currentParagon == ParagonTools.idCleric_Devout)
                    return Cleric_Devout;
                if (currentParagon == ParagonTools.idBarbarian_BladeMaster)
                    return Barbarian_Blademaster;
                if (currentParagon == ParagonTools.idBarbarian_Sentinel)
                    return Barbarian_Sentinel;
                if (currentParagon == ParagonTools.idFighter_Dreadnought)
                    return Fighter_Dreadnought;
                if (currentParagon == ParagonTools.idFighter_Vanguard)
                    return Fighter_Vanguard;
                if (currentParagon == ParagonTools.idRanger_Warden)
                    return Ranger_Warden;
                if (currentParagon == ParagonTools.idRanger_Hunter)
                    return Ranger_Hunter;
                if (currentParagon == ParagonTools.idPaladin_OathKeeper)
                    return Paladin_Oathkeeper;
                if (currentParagon == ParagonTools.idPaladin_Justicar)
                    return Paladin_Justicar;
                if (currentParagon == ParagonTools.idWarlock_HellBringer)
                    return Warlock_Hellbringer;
                if (currentParagon == ParagonTools.idWarlock_SoulWeaver)
                    return Warlock_Soulweaver;
                if (currentParagon == ParagonTools.idRogue_WhisperKnife)
                    return Rogue_Whisperknife;
                if (currentParagon == ParagonTools.idRogue_Assassin)
                    return Rogue_Assassin;

                return false;
            }
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }

    [Flags]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum PlayerParagonType : uint
    {
        /// <summary>
        /// Неизвестный тип
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Следопыт
        /// </summary>
        Ranger =                0b00_00_00_00_00_00_00_00_11,
        /// <summary>
        /// Следопыт (Охотник)
        /// </summary>
        Ranger_Hunter =         0b00_00_00_00_00_00_00_00_01,
        /// <summary>
        /// Следопыт (Хранитель)
        /// </summary>
        Ranger_Warden =         0b00_00_00_00_00_00_00_00_10,

        /// <summary>
        /// Волшебник
        /// </summary>
        Wizard =                0b00_00_00_00_00_00_00_11_00,
        /// <summary>
        /// Волшебник (Чудотворец)
        /// </summary>            
        Wizard_Arcanist =       0b00_00_00_00_00_00_00_01_00,
        /// <summary>
        /// Волшебник (Арканист)
        /// </summary>
        Wizard_Thaumaturge =    0b00_00_00_00_00_00_00_10_00,

        /// <summary>
        /// Клирик      
        /// </summary> 
        Cleric =                0b00_00_00_00_00_00_11_00_00,
        /// <summary>
        /// Клирик (Благочестивец)
        /// </summary>
        Cleric_Devout =         0b00_00_00_00_00_00_01_00_00,
        /// <summary>
        /// Клирик (Судья)
        /// </summary>
        Cleric_Arbiter =        0b00_00_00_00_00_00_10_00_00,

        /// <summary>  
        /// Варвар
        /// </summary>
        Barbarian =             0b00_00_00_00_00_11_00_00_00,
        /// <summary>
        /// Варвар (Страж)
        /// </summary>
        Barbarian_Sentinel =    0b00_00_00_00_00_01_00_00_00,
        /// <summary>
        /// Варвар (Мастер клинка)
        /// </summary>
        Barbarian_BladeMaster = 0b00_00_00_00_00_10_00_00_00,

        /// <summary>
        /// Воин (бывший Страж)
        /// </summary>
        Fighter =               0b00_00_00_00_11_00_00_00_00,
        /// <summary>
        /// Воин (Авангард)
        /// </summary>
        Fighter_Vanguard =      0b00_00_00_00_01_00_00_00_00,
        /// <summary>
        /// Воин (Сорвиголова)
        /// </summary>
        Fighter_Dreadnought =   0b00_00_00_00_10_00_00_00_00,

        /// <summary>
        /// Паладин
        /// </summary>
        Paladin =               0b00_00_00_11_00_00_00_00_00,
        /// <summary>
        /// Паладин (Клятвохранитель)
        /// </summary>
        Paladin_OathKeeper =    0b00_00_00_01_00_00_00_00_00,
        /// <summary>
        /// Паладин (Юстициар)
        /// </summary>
        Paladin_Justicar =      0b00_00_00_10_00_00_00_00_00,

        /// <summary>
        /// Чернокнижник
        /// </summary>
        Warlock =               0b00_00_11_00_00_00_00_00_00,
        /// <summary>
        /// Чернокнижник (Вестник ада)
        /// </summary>
        Warlock_HellBringer =   0b00_00_01_00_00_00_00_00_00,
        /// <summary>
        /// Чернокнижник (Ткач душ)
        /// </summary>
        Warlock_SoulWeaver =    0b00_00_10_00_00_00_00_00_00,

        /// <summary>
        /// Плут
        /// </summary>
        Rogue =                 0b00_11_00_00_00_00_00_00_00,
        /// <summary>
        /// Плут (Убийца)
        /// </summary>
        Rogue_Assassin =        0b00_01_00_00_00_00_00_00_00,
        /// <summary>
        /// Плут (Шепчущий нож)
        /// </summary>
        Rogue_WhisperKnife =    0b00_10_00_00_00_00_00_00_00,

        /// <summary>
        /// Бард
        /// </summary>
        Bard =                  0b11_00_00_00_00_00_00_00_00,
        /// <summary>
        /// Бард (хил)
        /// </summary>
        Bard_Healer =           0b01_00_00_00_00_00_00_00_00,
        /// <summary>
        /// Бард (ДД)
        /// </summary>
        Bard_DD =               0b10_00_00_00_00_00_00_00_00
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
                 PlayerParagonType.Bard_Healer,

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
             PlayerParagonType.Bard_DD
    }
}
