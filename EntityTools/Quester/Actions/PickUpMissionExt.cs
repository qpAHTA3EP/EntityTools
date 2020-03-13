using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Threading;
using System.Xml.Serialization;
using Astral.Logic.Classes.Map;
using Astral.Logic.NW;
using Astral.Quester.Forms;
using Astral.Quester.UIEditors;
using MyNW.Classes;
using Astral;
using MyNW.Internals;
using EntityTools.Editors.Forms;
using System.Collections.Generic;
using MyNW.Patchables.Enums;
using EntityTools.Extentions;
using EntityTools.Enums;
using EntityTools;
using EntityTools.Tools;
using System.Runtime.CompilerServices;
using EntityTools.Core;

[assembly:InternalsVisibleTo("EntityCore")]

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class PickUpMissionExt : Astral.Quester.Classes.Action,
                                    INotifyPropertyChanged
    {
        #region Опции команды
        [Editor(typeof(MainMissionEditor), typeof(UITypeEditor))]
        [Category("Required")]
        public string MissionId
        {
            get => _missionId;
            set
            {
                if (_missionId != value)
                {
                    _missionId = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MissionId)));
                }
            }
        }
        private string _missionId = string.Empty;

        [Description("Skip directly if mission is not available.")]
        public bool SkipOnFail
        {
            get => _skipOnFail; set
            {
                if (_skipOnFail != value)
                {
                    _skipOnFail = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SkipOnFail)));
                }

            }
        }
        private bool _skipOnFail = false;

        [Editor(typeof(Astral.Quester.UIEditors.NPCInfos), typeof(UITypeEditor))]
        [Category("Required")]
        public Astral.Quester.Classes.NPCInfos Giver
        {
            get => _giver;
            set
            {
                if (_giver != value)
                {
                    _giver = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Giver)));
                }
            }
        }
        private Astral.Quester.Classes.NPCInfos _giver = new Astral.Quester.Classes.NPCInfos();

        public bool CloseContactDialog
        {
            get => _closeContactDialog;
            set
            {
                if (_closeContactDialog != value)
                {
                    _closeContactDialog = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CloseContactDialog)));
                }
            }
        }
        private bool _closeContactDialog = false;

        public float InteractDistance
        {
            get => _interactDistance;
            set
            {
                if (_interactDistance != value)
                {
                    _interactDistance = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InteractDistance)));
                }
            }
        }
        private float _interactDistance = 0f;

        public bool AutoAcceptOfferedMission
        {
            get => _autoAcceptOfferedMission; set
            {
                if(_autoAcceptOfferedMission != value)
                {
                    _autoAcceptOfferedMission = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AutoAcceptOfferedMission)));
                }
                
            }
        }
        private bool _autoAcceptOfferedMission = true;

        [Editor(typeof(RewardsEditor), typeof(UITypeEditor))]
        [Description("Item that is requered in rewards to PickUpMission\n" +
                     "Simple wildcard (*) is allowed\n" +
                     "Mission Offer dialog have to be opened for choosen the ReqieredRewardItem")]
        public string RequiredRewardItem
        {
            get => _requiredRewardItem;
            set
            {
                if (_requiredRewardItem != value)
                {
                    _requiredRewardItem = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RequiredRewardItem)));
                }
            }
        }
        private string _requiredRewardItem = string.Empty;
        #endregion

        #region Взаимодействие с EntityToolsCore
        public event PropertyChangedEventHandler PropertyChanged;

        internal Func<ActionResult> coreRun = null;
        internal Func<bool> coreNeedToRun = null;
        internal Func<bool> coreValidate = null;
        internal Action coreReset = null;
        internal Action coreGatherInfos = null;
        internal Func<string> coreString = null;

        public PickUpMissionExt()
        {
            coreRun = () => Core.Initializer.Initialize(ref coreRun);
            coreNeedToRun = () => Core.Initializer.Initialize(ref coreNeedToRun);
            coreValidate = () => Core.Initializer.Initialize(ref coreValidate);
            coreReset = () => Core.Initializer.Initialize(ref coreReset);
            coreGatherInfos = () => Core.Initializer.Initialize(ref coreGatherInfos);
            coreString = () => Core.Initializer.Initialize(ref coreString);
        }
        #endregion

        #region Интерфейс Quester.Action
        public override bool NeedToRun => coreNeedToRun();

        public override ActionResult Run() => coreRun();

        public override void GatherInfos() => coreGatherInfos();

        [Browsable(false)]
        [XmlIgnore]
        public new string AssociateMission => string.Empty;

        [Browsable(false)]
        [XmlIgnore]
        public new bool PlayWhileConditionsAreOk => false;//true;

        protected override ActionValidity InternalValidity
        {
            get
            {
                if (Giver == null || !Giver.Position.IsValid)
                {
                    return new ActionValidity("Giver position invalid.");
                }
                if (this.MissionId.Length == 0)
                {
                    return new ActionValidity("Invalid mission id.");
                }
                return new ActionValidity();
            }
        }

        public override string ActionLabel => coreString();

        protected override bool IntenalConditions => coreValidate();

        protected override Vector3 InternalDestination
        {
            get
            {
                if (Giver != null && Giver.Position.IsValid)
                    return Giver.Position.Clone();
                else return new Vector3();
            }
        }

        public override bool UseHotSpots => false;

        public override void OnMapDraw(GraphicsNW graph)
        {
            if (Giver != null && Giver.Position.IsValid)
            {
                graph.drawFillEllipse(Giver.Position, new Size(10, 10), Brushes.Beige);
            }
        }

        public override void InternalReset() => coreReset();

        [XmlIgnore]
        [Browsable(false)]
        public override string Category => "Basic";

        public override string InternalDisplayName => string.Empty;
        #endregion
    }
}
