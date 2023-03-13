using System.Collections.Generic;
using System.ComponentModel;

namespace EntityTools.Quester.Editor.Classes
{
    public class ConditionPackMetadataType
    {
        [Browsable(false)]
        public List<Astral.Quester.Classes.Condition> Conditions { get; set; }
    }
}
