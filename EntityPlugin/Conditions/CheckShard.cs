using EntityPlugin.Editors;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;

namespace EntityPlugin.Conditions
{
    public class CheckShard : Astral.Quester.Classes.Condition
    {
        [Description("A name of the Shard")]
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

        public override void Reset()
        {
        }

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
