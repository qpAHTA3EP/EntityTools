using EntityTools.Editors;
using System.ComponentModel;
using System.Drawing.Design;

namespace EntityCore.Quester.Classes
{
    public class PushProfileToStackAndLoadMetadataType
    {
        [Description("Select profile for loading.")]
        [Editor(typeof(RelativeProfileFilePathEditor), typeof(UITypeEditor))]
        [TypeConverter]
        public string ProfileName { get; set; }
    }
}
