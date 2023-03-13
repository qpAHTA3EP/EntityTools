using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;
using Astral.Classes;
using Astral.Quester.Classes;
using Astral.Quester.UIEditors;

namespace EntityTools.Quester.Editor.TreeViewCustomization
{
    public interface IQuesterAction
    {
        [Category("Common")]
        [ReadOnly(true)]
        Guid ActionID { get; set; }

        [Category("Common")] 
        string OverrideName { get; set; }

        [Editor(typeof(ActionSoundEdit), typeof(UITypeEditor))]
        [Category("Common")]
        ActionSound Sound { get; set; }

        [Category("Common")] 
        bool Disabled { get; set; }

        [XmlIgnore] [Browsable(false)] 
        string ActionLabel { get; }

        [Description("Action will be played until conditions are valid.")]
        [Category("Common")]
        bool PlayWhileConditionsAreOk { get; set; }

        [DisplayName("PlayWhileMissionUnsuccess")]
        [Description(
            "True : action will be repeated until assiociate mission is a success.\r\nFalse : Play one time if associate mission ins't a success.")]
        [Category("Common")]
        bool PlayWhileUnSuccess { get; set; }

        [Description("Associate mission to the action, mission can be an objective.")]
        [Category("Common")]
        //[Editor(typeof(MissionEditor), typeof(UITypeEditor))]
        string AssociateMission { get; set; }

        [Description("Play again while conditions are ok.")]
        [Category("Common")]
        bool Loop { get; set; }

        [Description("Max running time (seconds), 0 : disabled")]
        [Category("Common")]
        int MaxRunningTime { get; set; }

        [XmlIgnore] [Browsable(false)] 
        Timeout RunningTO { get; set; }

        [Description("Only one unlocked condition must be good")]
        [Category("Common")]
        bool OnlyOneConditionMustBeGood { get; set; }

        [Description("Useful only in multi bots mode")]
        [Category("MultiBots")]
        bool OnlyDoneByServer { get; set; }
    }
}