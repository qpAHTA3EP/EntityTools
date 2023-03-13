using System.ComponentModel;
using System.Drawing.Design;
using Astral.Quester.Classes;
using Astral.Quester.Classes.Conditions;
using EntityTools.Editors;

namespace EntityTools.Quester.Editor.Classes
{
    public class IsInCustomRegionMetadataType
    {
        private IsInCustomRegion condition;

        public IsInCustomRegionMetadataType(IsInCustomRegion condition)
        {
            this.condition = condition;
        }

        [Description("Type of comparison.")]
        public Condition.Presence Tested { get; set; }

        [Description("The name of evaluated CustomRegion.")]
        [Editor(typeof(CustomRegionEditor), typeof(UITypeEditor))]
        public string CustomRegionName { get; set; }

        //public IsInCustomRegion()
        //{
        //    this.Tested = Condition.Presence.Equal;
        //    foreach (CustomRegion customRegion in Core.Profile.CustomRegions)
        //    {
        //        if (customRegion.IsIn)
        //        {
        //            this.CustomRegionName = customRegion.Name;
        //            break;
        //        }
        //    }
        //}

        public override string ToString()
        {
            return "Test ToString()";//$"CustomRegion {Tested} {CustomRegionName}";
        }

        [Browsable(true)]
        public bool IsValid
        {
            get
            {
                //var customRegions = DebugContext?.CustomRegions ??
                //                    AstralAccessors.Quester.Core.ActiveProfileProxy.CustomRegions;

                //foreach (CustomRegion customRegion in customRegions)
                //{
                //    if (customRegion.Name == CustomRegionName)
                //    {
                //        Condition.Presence tested = Tested;
                //        if (tested == Condition.Presence.Equal)
                //        {
                //            return customRegion.IsIn;
                //        }

                //        if (tested == Condition.Presence.NotEquel)
                //        {
                //            return !customRegion.IsIn;
                //        }
                //    }
                //}

                //return false;
                return true;
            }
        }

        [Browsable(false)]
        public string TestInfos
        {
            get
            {
                //string text = "Detected custom regions :";
                //var customRegions = DebugContext?.CustomRegions ??
                //                    AstralAccessors.Quester.Core.ActiveProfileProxy.CustomRegions;
                //foreach (CustomRegion customRegion in customRegions)
                //{
                //    if (customRegion.IsIn)
                //    {
                //        text = text + customRegion.Name + Environment.NewLine;
                //    }
                //}

                //return text;
                return "Test text";
            }
        }

        //[Browsable(false)]
        //public QuesterProfileProxy DebugContext { get; set; }
    }
}