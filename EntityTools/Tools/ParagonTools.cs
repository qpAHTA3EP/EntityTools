using System;
using System.Linq;
using MyNW.Internals;

namespace EntityTools.Tools
{
    public class ParagonTools
    {
        public static readonly string idWizard = "Player_Controller";
        public static readonly string idWizard_Arcanist = "Player_Secondary_Spellstormmage";
        public static readonly string idWizard_Thaumaturge = "Player_Secondary_MasterofFlame";

        public static readonly string idCleric = "Player_Devoted";
        public static readonly string idCleric_Devout = "Player_Secondary_AnointedChampion";
        public static readonly string idCleric_Arbiter = "Player_Secondary_DivineOracle";

        public static readonly string idFighter = "Player_Guardian";
        public static readonly string idFighter_Vanguard = "Player_Secondary_Ironvanguard";
        public static readonly string idFighter_Dreadnought = "Player_Secondary_Swordmaster_GF";

        public static readonly string idRanger = "Player_Archer";
        public static readonly string idRanger_Hunter = "Player_Secondary_Pathfinder";
        public static readonly string idRanger_Warden = "Player_Secondary_Stormwarden";

        public static readonly string idBarbarian = "Player_Greatweapon";
        public static readonly string idBarbarian_Sentinel = "Player_Secondary_Ironvanguard_GWF";
        public static readonly string idBarbarian_Blademaster = "Player_Secondary_Swordmaster";

        public static readonly string idPaladin = "Player_Paladin";
        public static readonly string idPaladin_Oathkeeper = "Player_Secondary_OathofDevotion";
        public static readonly string idPaladin_Justicar = "Player_Secondary_OathofProtection";

        public static readonly string idWarlock = "Player_Scourge";
        public static readonly string idWarlock_Soulweaver = "Player_Secondary_Soulbinder";
        public static readonly string idWarlock_Hellbringer = "Player_Secondary_Hellbringer";

        public static readonly string idRogue = "Player_Trickster";
        public static readonly string idRogue_Whisperknife = "Player_Secondary_Whisperknife";
        public static readonly string idRogue_Assassin = "Player_Secondary_MasterInfiltrator";
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
                if (currentParagon == ParagonTools.idBarbarian_Blademaster)
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
                if (currentParagon == ParagonTools.idPaladin_Oathkeeper)
                    return Paladin_Oathkeeper;
                if (currentParagon == ParagonTools.idPaladin_Justicar)
                    return Paladin_Justicar;
                if (currentParagon == ParagonTools.idWarlock_Hellbringer)
                    return Warlock_Hellbringer;
                if (currentParagon == ParagonTools.idWarlock_Soulweaver)
                    return Warlock_Soulweaver;
                if (currentParagon == ParagonTools.idRogue_Whisperknife)
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

    public enum PlayerClassParagonType
    {
        Unknown = 0,    // Неизвестный тип
        Player_Archer,      // Следопыт
        Player_Secondary_Pathfinder,    // Охотник
        Player_Secondary_Stormwarden,   // Хранитель
        Player_Controller,  // Волшебник
        Player_Secondary_MasterofFlame,     // Чудотворец
        Player_Secondary_Spellstormmage,    // Арканист
        Player_Devoted,     // Клирик
        Player_Secondary_AnointedChampion,  // Благочестивец
        Player_Secondary_DivineOracle,      // Судья
        Player_Greatweapon, // Варвар
        Player_Secondary_Ironvanguard_GWF,  // Страж
        Player_Secondary_Swordmaster,       // Мастер клинка
        Player_Guardian,    // Воин (Танк)
        Player_Secondary_Ironvanguard,      // Авангард
        Player_Secondary_Swordmaster_GF,    // Сорвиголова
        Player_Paladin,     // Паладин
        Player_Secondary_OathofDevotion,    // Клятвохранитель
        Player_Secondary_OathofProtection,  // Юстициар
        Player_Scourge,     // Чернокнижник
        Player_Secondary_Hellbringer,   // Вестник ада
        Player_Secondary_Soulbinder,    // Ткач душ
        Player_Trickster,   // Плут
        Player_Secondary_MasterInfiltrator, // Убийца
        Player_Secondary_Whisperknife       // Шепчущий нож
    }
}
