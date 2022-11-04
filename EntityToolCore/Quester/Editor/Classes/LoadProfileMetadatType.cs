using System.ComponentModel;
using System.Drawing.Design;
using EntityTools.Editors;

namespace EntityCore.Quester.Editor.Classes
{
    public class LoadProfileMetadataType
    {
        [Editor(typeof(RelativeProfileFilePathEditor), typeof(UITypeEditor))]
        public string ProfileName { get; set; }
    }
}
