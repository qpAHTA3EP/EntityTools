using System.ComponentModel;
using System.Drawing.Design;
using EntityTools.Editors;

namespace EntityCore.Quester.Editor.Classes
{
    public class PushProfileToStackAndLoadMetadataType
    {
        [Description("Specify profile name to loading")]
        [Editor(typeof(RelativeProfileFilePathEditor), typeof(UITypeEditor))]
        [TypeConverter]
        public string ProfileName { get; set; }
    }
}
