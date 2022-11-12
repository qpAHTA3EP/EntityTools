using System;
using System.ComponentModel;
using System.Drawing.Design;
using Astral.Quester.Classes;
using EntityTools.Editors;
using EntityTools.Patches;
using MyNW.Internals;

namespace EntityTools.Quester.Conditions
{
    [Serializable]
    public class CheckShard : Condition
    {
        public CheckShard()
        {
            var characterChoices = Game.CharacterSelectionData.CharacterChoices;
            if (characterChoices.IsValid && characterChoices.LastPlayedCharacter.IsValid)
            {
                Name = characterChoices.LastPlayedCharacter.ShardName;
            }
        }

        [Description("A name of the Shard (GameServer)")]
        [Editor(typeof(CurrentShardEditor), typeof(UITypeEditor))]
        public string Name { get; set; }




        public override bool IsValid
        {
            get
            {
                var characterChoices = Game.CharacterSelectionData.CharacterChoices;
                if (characterChoices.IsValid && characterChoices.LastPlayedCharacter.IsValid)
                    return Name == characterChoices.LastPlayedCharacter.ShardName;
                return false;
            }
        }

        public override void Reset(){ }

        public override string ToString() => $"{GetType().Name} [{Name}]";

        public override string TestInfos
        {
            get
            {
                var characterChoices = Game.CharacterSelectionData.CharacterChoices;
                if (characterChoices.IsValid && characterChoices.LastPlayedCharacter.IsValid)
                    return $"Current Shard is [{characterChoices.LastPlayedCharacter.ShardName}]";
                return "No valid information";
            }
        }
    }
}
