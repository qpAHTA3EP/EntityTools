﻿using EntityTools.Editors;
using MyNW.Internals;
using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace EntityTools.Quester.Conditions
{
    [Serializable]
    public class CheckShard : Astral.Quester.Classes.Condition
    {
        public CheckShard()
        {
            if (Game.CharacterSelectionData.CharacterChoices.IsValid && Game.CharacterSelectionData.CharacterChoices.LastPlayedCharacter.IsValid)
            {
                Name = Game.CharacterSelectionData.CharacterChoices.LastPlayedCharacter.ShardName;
            }
        }
#if PATCH_ASTRAL
        static CheckShard()
        {
            // Пременение патча на этапе десериализации (до инициализации плагина)
            Patches.ETPatcher.Apply();
        }
#endif

        [Description("A name of the Shard (GameServer)")]
        [Editor(typeof(CurrentShardEditor), typeof(UITypeEditor))]
        public string Name { get; set; }

        public override bool IsValid
        {
            get
            {
                if(Game.CharacterSelectionData.CharacterChoices.IsValid && Game.CharacterSelectionData.CharacterChoices.LastPlayedCharacter.IsValid)
                    return (Name == Game.CharacterSelectionData.CharacterChoices.LastPlayedCharacter.ShardName);
                return false;
            }
        }

        public override void Reset(){ }

        public override string ToString()
        {
            return $"{GetType().Name} [{Name}]";
        }

        public override string TestInfos
        {
            get
            {
                if (Game.CharacterSelectionData.CharacterChoices.IsValid && Game.CharacterSelectionData.CharacterChoices.LastPlayedCharacter.IsValid)
                    return $"Current Shard is [{Game.CharacterSelectionData.CharacterChoices.LastPlayedCharacter.ShardName}]";
                return "No valid information";
            }
        }
    }
}
