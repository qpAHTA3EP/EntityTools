﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityTools.Enums
{
    public enum ItemFilterEntryType
    {
        /// <summary>
        /// Предмет задан идентификатором
        /// </summary>
        Identifier,
        /// <summary>
        /// Предмет задан категорией
        /// </summary>
        Category
    }

    public enum VendorType
    {
        None,
        /// <summary>
        /// Автоматический выбор вендора
        /// </summary>
        Auto,
        /// <summary>
        /// Конкретный торговец на карте
        /// </summary>
        Normal,
        /// <summary>
        /// Призываемый торговец (актефактом Каталог Аврора для всех миров)
        /// </summary>
        ArtifactVendor,
        /// <summary>
        /// Игровой магазин кампаний
        /// </summary>
        RemouteVendor,
        /// <summary>
        /// ВИП-магазин печатей
        /// </summary>
        VIPSummonSealTrader,
        /// <summary>
        /// ВИП-магазин магазина профессий
        /// </summary>
        VIPProfessionVendor
    }

    /// <summary>
    /// Результат торговой операции
    /// </summary>
    public enum BuyItemResult
    {
        /// <summary>
        /// Покупка успешна
        /// </summary>
        Succeeded,
        /// <summary>
        /// Операция завершена без ошибок, покупок не совершено
        /// </summary>
        Completed,
        /// <summary>
        /// Предмет найден, но валюты для покупки недостаточно
        /// </summary>
        NotEnoughCurrency,
        /// <summary>
        /// Полная сумка, поэтому покупка не состоялась
        /// </summary>
        FullBag,
        /// <summary>
        /// Предмет не подлежит покупке
        /// </summary>
        Skiped,
        /// <summary>
        /// Ошибка во время сделки
        /// </summary>
        Error
    }

    public enum EquipmentCategory
    {
        None = 0,
        DDWeapon = 1,
        DDImplement = 2,
        DDShield = 3,
#if false
        Light = 4,
        Medium = 5,
        Heavy = 6,
        Garment = 7,
        Melee = 8,
        Ranged = 9,
        Blade = 10,
        Blunt = 11,
        Pierce = 12, 
#endif
        Secondary = 13,
        Weapon = 14,
#if false
        Onehanded = 15,
        Twohanded = 16,
        Dual = 17,
        Kukri = 18,
        Spear = 19,
        Glaive = 20,
        Bow = 21,
        Crossbow = 22,
        Cutlass = 23, 
#endif

        Mainhand = 24,

#if false
        Katana = 25,
        Critter_Club = 26,
        Critter_Mstaff = 27,
        Critter_Spear = 28,
        Critter_Shield = 29,
        Cloth = 30,
        Leather = 31,
        Hide = 32,
        Chain = 33,
        Scale = 34,
        Plate = 35, 
#endif
        Shield = 36,
        Armor = 37,
#if false
        Tier1 = 38,
        Tier2 = 39,
        Tier3 = 40,
        Tier4 = 41,
        Tier5 = 42,
        Tier6 = 43,
        Salvageable = 44, 
#endif
        Head = 45,
        Neck = 46,
        Cloak = 47,
        Arms = 48,
        Feet = 49,
        Belt = 50,
        Ring = 51,
        Swordknot = 52,
        Talisman = 53,
        Icon = 54,
        Grimoire = 55,
        Wand = 56,
        Orb = 57,
        Rod = 58,
        Staff = 59,
        Holysymbol = 60,
        Pactblade = 61,
#if false
        Divine = 62,
        Arcane = 63,
        Natural = 64,
        Historic = 65,
        Illicit = 66, 
#endif
        Artifact = 67,
        Artifactgear = 68,
        Artifactprimary = 69,
        Artifactsecondary = 70,
#if false
        Lore = 71,
        Mundanegoods = 72,
        Tradegoods = 73,
        Consumable = 74,
        Bags = 75,
        Resurrection = 76,
        Dye = 77,
        Seal = 78,
        Enchantment = 79,
        Runestone = 80,
        Insignia = 81,
        Shard = 82,
        Mount = 83,
        Scroll = 84,
        Potion = 85,
        Armorkit = 86,
        Bounty = 87,
        Treasure = 88,
        Skillconsumable = 89,
        Companionconsumable = 90,
        Companiononly = 91,
        Bountyconsumable = 92,
        Playerconsumable = 93,
        Mountconsumable = 94,
        Enchantmentward = 95,
        Runestoneward = 96,
        Novelty = 97,
        Freetransmute = 98,
        Refiningstone = 99,
        Artifactpower = 100,
        Artifactstability = 101,
        Artifactunion = 102,
        Artifactresonance = 103,
        Demogorgonresonance = 104,
        Relicresonance = 105,
        Notusableinrefining = 106,
        Uniqueequipped = 107,
        Mount_With_Combat_Power = 108,
        Underdark = 109,
        Shadowcladresonance = 110,
        Dreadresonance = 111,
        Treasure_River = 112,
        Deprecated = 113,
        Hunt_Trophy = 114,
        Hunt_Trophy_01 = 115,
        Hunt_Trophy_02 = 116,
        Hunt_Trophy_03 = 117,
        Hunt_Lure = 118,
        Hunt_Lure_01 = 119,
        Hunt_Lure_02 = 120,
        Hunt_Lure_03 = 121,
        Refinement_Currency = 122,
        Refinement_Jewel = 123,
        Hunt_Trophy_Omu = 124,
        Limited_Event_Item = 125,
        Magicitem = 126,
        Rewardtabledyes = 127,
        Avernus_Potions = 128,
        Mundane = 129,
        Cup = 130,
        Plates = 131,
        Gem = 132,
        Trinket = 133,
        Statue = 134,
        Jewelry = 135,
        Professions_Asset = 136,
        Professions_Asset_Artisan = 137,
        Professions_Asset_Tool = 138,
        Professions_Asset_Supplement = 139,
        Professions_Category_Blacksmithing = 140,
        Professions_Category_Artificing = 141,
        Professions_Category_Armorsmithing = 142,
        Professions_Category_Leatherworking = 143,
        Professions_Category_Tailoring = 144,
        Professions_Category_Jewelcrafting = 145,
        Professions_Category_Alchemy = 146,
        Professions_Category_Gathering = 147,
        Professions_Category_Summerevent = 148,
        Professions_Category_Event_Siege = 149,
        Professions_Asset_Artisan_Blacksmithing = 150,
        Professions_Asset_Artisan_Artificing = 151,
        Professions_Asset_Artisan_Armorsmithing = 152,
        Professions_Asset_Artisan_Leatherworking = 153,
        Professions_Asset_Artisan_Tailoring = 154,
        Professions_Asset_Artisan_Jewelcrafting = 155,
        Professions_Asset_Artisan_Alchemy = 156,
        Professions_Asset_Artisan_Gathering = 157,
        Professions_Asset_Tool_Blacksmithing = 158,
        Professions_Asset_Tool_Artificing = 159,
        Professions_Asset_Tool_Armorsmithing = 160,
        Professions_Asset_Tool_Leatherworking = 161,
        Professions_Asset_Tool_Tailoring = 162,
        Professions_Asset_Tool_Jewelcrafting = 163,
        Professions_Asset_Tool_Alchemy = 164,
        Professions_Asset_Tool_Mining = 165,
        Professions_Asset_Tool_Logging = 166,
        Professions_Asset_Tool_Harvesting = 167,
        Professions_Asset_Tool_Collecting = 168,
        Professions_Asset_Tool_Hunting = 169,
        Professions_Asset_Tool_Catching = 170,
        Professions_Material = 171,
        Professions_Material_Stone = 172,
        Professions_Material_Metal = 173,
        Professions_Material_Lumber = 174,
        Professions_Material_Textiles = 175,
        Professions_Material_Leather = 176,
        Professions_Material_Bone = 177,
        Professions_Material_Substances = 178,
        Material_Metal = 179,
        Material_Stone = 180,
        Material_Wood = 181,
        Material_Animalpart = 182,
        Material_Textile = 183,
        Material_Reagent = 184,
        Material_Supplement = 185,
        Craft_Professionrank = 186,
        Craft_Donotdisplay = 187,
        Artisan_Armorsmithing_Heavy = 188,
        Artisan_Weaponsmithing = 189,
        Artisan_Artificing = 190,
        Artisan_Tailoring = 191,
        Artisan_Alchemy = 192,
        Artisan_Leatherworking = 193,
        Artisan_Leadership = 194,
        Artisan_Jewelcrafting = 195,
        Craft_Alchemy = 196,
        Craft_Armorsmithing_Med = 197,
        Craft_Armorsmithing_Heavy = 198,
        Craft_Artificing = 199,
        Craft_Blacksmithing = 200,
        Craft_Bladesmithing = 201,
        Craft_Enchanting = 202,
        Craft_Inscribing = 203,
        Craft_Leadership = 204,
        Craft_Leatherworking = 205,
        Craft_Refining = 206,
        Craft_Tailoring = 207,
        Craft_Weaponsmithing = 208,
        Craft_Jewelcrafting = 209,
        Craft_Woodworking = 210,
        Craft_Summerevent = 211,
        Craft_Winterevent = 212,
        Craft_Blackice = 213,
        Craft_Event_Siege = 214,
        Craft_Events_Ah = 215,
        Craftasset = 216,
        Craftresource = 217,
        Craftslottable = 218,
        Craftperson = 219,
        Craftperson_T1 = 220,
        Craftperson_T2 = 221,
        Craftperson_T3 = 222,
        Crafttool = 223,
        Crafttool_Anvil = 224,
        Crafttool_Armor = 225,
        Crafttool_Awl = 226,
        Crafttool_Bellows = 227,
        Crafttool_Bezelpusher = 228,
        Crafttool_Chisel = 229,
        Crafttool_Crucible = 230,
        Crafttool_File = 231,
        Crafttool_Grindstone = 232,
        Crafttool_Hammer = 233,
        Crafttool_Loupe = 234,
        Crafttool_Mallet = 235,
        Crafttool_Mortar = 236,
        Crafttool_Needle = 237,
        Crafttool_Philostone = 238,
        Crafttool_Saw = 239,
        Crafttool_Shears = 240,
        Crafttool_Swivelknife = 241,
        Crafttool_Tongs = 242,
        Crafttool_Weaponry = 243,
        Crafttool_Gauntlets = 244,
        Crafttool_Graver = 245,
        Crafttool_Cart = 246,
        Crafttool_Masterwork = 247,
        Craftreagent = 248,
        Fashion = 249,
        Fashionhead = 250,
        Fashionneck = 251,
        Fashionbody = 252,
        Fashionfeet = 253,
        Fashionarms = 254,
        Fashionshirt = 255,
        Fashionpants = 256,
        Stats_Tiny = 257,
        Stats_Small = 258,
        Stats_Medium = 259,
        Stats_Large = 260,
        Stats_Dual = 261,
        Hilt_Hip = 262,
        Hilt_Hip_Dual = 263,
        Title_Background = 264,
        Title_Foundry = 265,
        Title_General = 266,
        Title_Profession = 267,
        Title_Quest = 268,
        Title_Slayer = 269,
        Lockbox = 270,
        Firstlockboxtrigger = 271,
        Lockboxthreereward = 272,
        Nointeract = 273,
        Background = 274,
        Origin = 275,
        Deity = 276,
        Testdual = 277,
        Equipmentpack = 278,
        Professionpack = 279,
        Consumablepack = 280,
        Companionpack = 281,
        Mountpack = 282,
        Fashionpack = 283,
        Utilitypack = 284,
        Enchantpack = 285,
        Strongholdpack = 286,
        Choicepack_Autoclaimresults = 287,
        Rewardpackpet = 288,
        Promo_Raredrop_Tymora = 289,
        Promo_Raredrop_Waukeen = 290,
        Fishing_Fish = 291,
        Fishing_Pole = 292,
        Specialgrantrewardui = 293,
        Cotg_Pickup_Costume_Amaunator_01 = 294,
        Cotg_Pickup_Costume_Selune_01 = 295,
        Cotg_Pickup_Costume_Torm_01 = 296,
        Cotg_Pickup_Costume_Kelemvor_01 = 297,
        Cotg_Pickup_Costume_Tymora_01 = 298,
        Cotg_Pickup_Costume_Oghma_01 = 299,
        Cotg_Pickup_Costume_Sune_01 = 300,
        Cotg_Pickup_Costume_Corellon_01 = 301,
        Cotg_Pickup_Costume_Chauntea_01 = 302,
        Cotg_Pickup_Costume_Tempus_01 = 303,
        Cotg_Pickup_Costume_Moradin_01 = 304,
        Cotg_Pickup_Costume_Silvanus_01 = 305,
        Cotg_Lootbox_Costume_01 = 306,
        Venture_Artifacts_01_A = 307,
        Venture_Artifacts_01_B = 308,
        Venture_Artifacts_01_C = 309,
        Lliira_Contest_Fireworks = 310,
        Gondrefinement = 311,
        Refinement_Event_Ah = 312,
        Masqueraderefinement = 313,
        Simrilrefinement = 314,
        Jubileecoupon = 315,
        Jubileestorevoucher = 316,
        Jubileestrongholdvoucher = 317,
        Giftdrop_Jubilee = 318,
        Mt_Free_China = 319,
        Hotspot_Chest = 320,
        Hotspot_Chest_Rank_A = 321,
        Hotspot_Chest_Rank_B = 322,
        Hotspot_Chest_Rank_C = 323,
        Blackice = 324,
        Blackice_Corrupt = 325,
        Blackice_Purified = 326,
        Blackice_Battery = 327, 
#endif
        Artifact_Primary_Fey_R0 = 328,
        Artifact_Primary_Firesoul_R0 = 329,
        Artifact_Primary_Aberrant_R0 = 330,
        Artifact_Primary_Illusion_R0 = 331,
        Artifact_Secondary_Fey_R0 = 332,
        Artifact_Secondary_Firesoul_R0 = 333,
        Artifact_Secondary_Aberrant_R0 = 334,
        Artifact_Secondary_Illusion_R0 = 335,
        Artifact_Primary_Fey_R4 = 336,
        Artifact_Primary_Firesoul_R4 = 337,
        Artifact_Primary_Aberrant_R4 = 338,
        Artifact_Primary_Illusion_R4 = 339,
        Artifact_Secondary_Fey_R4 = 340,
        Artifact_Secondary_Firesoul_R4 = 341,
        Artifact_Secondary_Aberrant_R4 = 342,
        Artifact_Secondary_Illusion_R4 = 343,
        Recruit_Head = 344,
        Recruit_Body = 345,
        Recruit_Feet = 346,
        Recruit_Arms = 347,
        Pilgrim_Head = 348,
        Pilgrim_Body = 349,
        Pilgrim_Feet = 350,
        Pilgrim_Arms = 351,
        Pioneer_Head = 352,
        Pioneer_Body = 353,
        Pioneer_Feet = 354,
        Pioneer_Arms = 355,
        Huntsman_Head = 356,
        Huntsman_Body = 357,
        Huntsman_Feet = 358,
        Huntsman_Arms = 359,
        Ninegods_Head = 360,
        Ninegods_Body = 361,
        Ninegods_Feet = 362,
        Ninegods_Arms = 363,
        Gallant_Head = 364,
        Gallant_Body = 365,
        Gallant_Feet = 366,
        Gallant_Arms = 367,
        Ring_Yuanti = 368,
        Ring_Goblin = 369,
        Ring_Beast = 370,
        Ring_Undead = 371,
        Ring_Dinosaur = 372,
#if false
        Tarokkadeck = 373,
        Tarokkadeckbox = 374,
        Tarokkacoin = 375,
        Tarokkasword = 376,
        Tarokkaglyph = 377,
        Tarokkastar = 378,
        Tarokkahigh = 379,
        Tarokkasortorder_0 = 380,
        Tarokkasortorder_1 = 381,
        Tarokkasortorder_2 = 382,
        Tarokkasortorder_3 = 383,
        Tarokkasortorder_4 = 384,
        Tarokkasortorder_5 = 385,
        Tarokkasortorder_6 = 386,
        Tarokkasortorder_7 = 387,
        Tarokkasortorder_8 = 388,
        Tarokkasortorder_9 = 389,
        Tarokkasortorder_10 = 390,
        Tarokkasortorder_11 = 391,
        Tarokkasortorder_12 = 392,
        Tarokkasortorder_13 = 393,
        Vanity_Pet = 394,
        Pet_Dps = 395,
        Pet_Tank = 396,
        Pet_Healer = 397,
        Wanted_Poster = 398,
        Elemental_Infuse = 399,
        Injurykit = 400,
        Injurykit_Dontautouse = 401,
        Currency_Campaign = 402,
        Currency_Event = 403,
        Currency_Pvp = 404,
        Currency_Lockbox = 405,
        Currency_Seals = 406,
        Currency_Bait = 407,
        Currency_Rune = 408,
        Demonic_He = 409,
        Storyteller_Journal = 410,
        Wondrousitem = 411,
        Blockitemprogressioneventbonus = 412,
        Campaign_Portobello_Quip_Queue = 413,
        Campaign_Portobello_Quip_Contest = 414,
        Campaign_Portobello_Quip_Profession = 415,
        Hidefromcurrencyui = 416,
        Voninblod = 417,
        Relic = 418,
        Restoreditem = 419,
        Stronghold_Deco = 420,
        Stronghold_Siege = 421,
        Stronghold_Voucher = 422,
        Rpbonus_Chest = 423,
        Allow_Protection = 424,
        Block_Protection = 425,
        Runeconsumable = 426,
        Runearmor = 427,
        Alterationmanual = 428,
        Elder_Rune_Seletion = 429,
        Elder_Rune_Influence = 430,
        Elder_Rune_Crystal = 431,
        Elder_Rune_Research = 432,
        Elder_Rune_Armor_R0 = 433,
        Elder_Rune_Armor_R1 = 434,
        Acboffense = 435,
        Acbdefense = 436,
        Acbutility = 437,
        Acbenhancement = 438,
        Pet_Slot_Power = 439,
        Pet_Archon_Air = 440,
        Pet_Archon_Water = 441,
        Pet_Archon_Fire = 442,
        Pet_Archon_Earth = 443,
        Pet_Archon_Ice = 444,
        Pet_Bolster_Fighter = 445,
        Pet_Bolster_Invoker = 446,
        Pet_Bolster_Creature = 447,
        Pet_Bolster_Mystical = 448,
        Pet_Bolster_Beast = 449,
        Companion_Rewardpack = 450,
        Runestonebonding = 451,
        Pvp_Resources = 452 
#endif
    }
}
