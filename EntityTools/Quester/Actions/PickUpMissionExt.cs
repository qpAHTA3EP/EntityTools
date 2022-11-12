using ACTP0Tools;
using Astral.Logic.Classes.Map;
using Astral.Logic.NW;
using Astral.Quester.Classes.Actions;
using Astral.Quester.Forms;
using Astral.Quester.UIEditors;
using EntityCore.Tools;
using EntityTools.Annotations;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Patches.Mapper;
using EntityTools.Tools;
using EntityTools.Tools.Missions;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.Serialization;
using Action = Astral.Quester.Classes.Action;
// ReSharper disable InconsistentNaming

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class PickUpMissionExt : Action, INotifyPropertyChanged
    {
        #region данные ядра
        /// <summary>
        /// Константа длины строки "MissionOffer." 
        /// </summary>
        static readonly int MISSION_OFFER_LENGTH = "MissionOffer.".Length;

        private const int TIME = 10_000;

        private ContactInfo giverContactInfo;
        private int tries;
        private string label = string.Empty;
        private string _idStr = string.Empty;
        #endregion
        



        #region Опции команды
#if DEVELOPER
        [Description("Identifier of the Mission.\n" +
            "Allows simple mask (*) at the begin and at the end.")]
        [Editor(typeof(MainMissionEditor), typeof(UITypeEditor))]
        [Category("Mission Options")]
#else
        [Browsable(false)]
#endif
        public string MissionId
        {
            get => _missionId;
            set
            {
                if (_missionId != value)
                {
                    _missionId = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _missionId = string.Empty;

#if DEVELOPER
        [Editor(typeof(MissionGiverInfoEditor), typeof(UITypeEditor))]
        [Category("Mission Options")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
#else
        [Browsable(false)]
#endif
        [NotifyParentProperty(true)]
        public MissionGiverInfo Giver
        {
            get => _giver;
            set
            {
                if (_giver != value)
                {
                    _giver = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private MissionGiverInfo _giver = new MissionGiverInfo();

        [Browsable(false)]
        public string GiverId
        {
            get => string.Empty;
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _giver.Id = value;
            }
        }
        [Browsable(false)]
        public Vector3 GiverPosition
        {
            get => null;
            set
            {
                if (value != null && value.IsValid)
                    _giver.Position = value;
            }
        }

#if !DEVELOPER
        [Browsable(false)]
#else
        [Description("Remote checking if contact have mission.")]
        [Category("Mission Options")]
#endif
        public ContactHaveMissionCheckType ContactHaveMission
        {
            get => _contactHaveMission;
            set
            {
                if (_contactHaveMission == value) return;
                _contactHaveMission = value;
                NotifyPropertyChanged();
            }
        }
        private ContactHaveMissionCheckType _contactHaveMission = ContactHaveMissionCheckType.Disabled;
        #endregion


        #region Interaction
#if DEVELOPER
        [Description("The minimum value is 5.")]
        [Category("Interaction")]
#else
        [Browsable(false)]
#endif
        public float InteractDistance
        {
            get => _interactDistance;
            set
            {
                value = Math.Max(value, 5);
                if (Math.Abs(_interactDistance - value) < 0.1f) return;
                _interactDistance = value;
                NotifyPropertyChanged();
            }
        }
        private float _interactDistance = 10;

#if DEVELOPER
        [Description("The minimum value is 1.")]
        [Category("Interaction")]
#else
        [Browsable(false)]
#endif
        public float InteractZDifference
        {
            get => _interactZDifference;
            set
            {
                value = Math.Max(value, 1);
                if (Math.Abs(_interactZDifference - value) < 0.1f) return;
                _interactZDifference = value;
                NotifyPropertyChanged();
            }
        }
        private float _interactZDifference = 10;

#if DEVELOPER
        [Description("Answers in dialog which have to be performed before mission picking up.")]
        [Editor(typeof(DialogEditor), typeof(UITypeEditor))]
        [Category("Interaction")]
#else
        [Browsable(false)]
#endif
        public List<string> Dialogs
        {
            get => _dialogs; set
            {
                if (_dialogs != value)
                {
                    _dialogs = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private List<string> _dialogs = new List<string>();

#if DEVELOPER
        [Description("Skip directly if mission is not available.")]
        [Category("Interaction")]
#else
        [Browsable(false)]
#endif
        public bool SkipOnFail
        {
            get => _skipOnFail;
            set
            {
                if (_skipOnFail != value)
                {
                    _skipOnFail = value;
                    NotifyPropertyChanged();
                }

            }
        }
        private bool _skipOnFail;

#if DEVELOPER
        [Category("Interaction")]
#else
        [Browsable(false)]
#endif
        public bool CloseContactDialog
        {
            get => _closeContactDialog;
            set
            {
                if (_closeContactDialog == value) return;
                _closeContactDialog = value;
                NotifyPropertyChanged();
            }
        }
        private bool _closeContactDialog;
        #endregion


        #region Manage Combat Options
#if DEVELOPER
        [Description("Ignore the enemies while approaching the " + nameof(Giver) + ".")]
        [Category("Manage Combat Options")]
#else
        [Browsable(false)]
#endif
        public bool IgnoreCombat
        {
            get => _ignoreCombat;
            set
            {
                if (_ignoreCombat == value) return;
                _ignoreCombat = value;
                NotifyPropertyChanged();
            }
        }
        private bool _ignoreCombat;

#if DEVELOPER
        [Description("Sets the ucc option '" + nameof(IgnoreCombatMinHP) + "' when disabling combat mode.\n" +
                     "Options ignored if the value is -1.")]
        [Category("Manage Combat Options")]
#else
        [Browsable(false)]
#endif
        public int IgnoreCombatMinHP
        {
            get => _ignoreCombatMinHp; set
            {
                if (value < -1)
                    value = 0;
                if (value > 100)
                    value = 100;
                if (_ignoreCombatMinHp != value)
                {
                    _ignoreCombatMinHp = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _ignoreCombatMinHp = -1;

#if DEVELOPER
        [Description("Special check before disabling combat while playing action.\n" +
                     "The condition is checking when option '" + nameof(IgnoreCombat) + "' is active.")]
        [Category("Manage Combat Options")]
        [Editor(typeof(QuesterConditionEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(ExpandableObjectConverter))]
#else
        [Browsable(false)]
#endif
        public Astral.Quester.Classes.Condition IgnoreCombatCondition
        {
            get => _ignoreCombatCondition;
            set
            {
                if (value != _ignoreCombatCondition)
                {
                    _ignoreCombatCondition = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private Astral.Quester.Classes.Condition _ignoreCombatCondition;
        #endregion


        #region Optional
#if DEVELOPER
        [Editor(typeof(RewardsEditor), typeof(UITypeEditor))]
        [Description("Item that is requered in rewards to " + nameof(PickUpMission) + ".\n" +
                     "Simple wildcard (*) is allowed.\n" +
                     "Mission Offer dialog have to be opened for choosen the " + nameof(RequiredRewardItem) + ".")]
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
        public string RequiredRewardItem
        {
            get => _requiredRewardItem;
            set
            {
                if (_requiredRewardItem == value) return;
                _requiredRewardItem = value;
                NotifyPropertyChanged();
            }
        }
        private string _requiredRewardItem = string.Empty;

#if DEVELOPER
        [Description("The Id of the Action executing if the '" + nameof(RequiredRewardItem) + "' would be missing.")]
        [Category("Optional")]
        [DisplayName("TargetActionOnRequiredRewardMissing")]
#else
        [Browsable(false)]
#endif
        public Guid TargetAction
        {
            get => _targetActionId;
            set
            {
                if (_targetActionId == value) return;
                _targetActionId = value;
                NotifyPropertyChanged();
            }
        }
        private Guid _targetActionId = Guid.Empty;
        #endregion


        #region HiddenOptions
        [Browsable(false)]
        [XmlIgnore]
        public new bool PlayWhileUnSuccess => false;

        [Browsable(false)]
        [XmlIgnore]
        public new bool PlayWhileConditionsAreOk => false;

        [Browsable(false)]
        [XmlIgnore]
        public new bool Loop => false;

        [Browsable(false)]
        [XmlIgnore]
        public new string AssociateMissionSuccess => string.Empty;

        [XmlIgnore]
        [Browsable(false)]
        public override string Category => "Basic";
        #endregion




        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            InternalResetOnPropertyChanged(propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void InternalResetOnPropertyChanged([CallerMemberName] string propertyName = default)
        {
            if (propertyName == nameof(RequiredRewardItem))
                //isRewardItem = initializer_IsRewardItem;
                _rewardItemCheck = null;
            else if (propertyName == nameof(MissionId)) label = string.Empty;
        }
        #endregion




        public override bool NeedToRun
        {
            get
            {
                bool debugInfoEnabled = ExtendedDebugInfo;

                string currentMethodName = debugInfoEnabled
                    ? string.Concat(_idStr, '.', MethodBase.GetCurrentMethod()?.Name ?? nameof(NeedToRun))
                    : string.Empty;

                bool needToRun = false;

                if (debugInfoEnabled)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Begins"));

                float searchDistance = Math.Max(InteractDistance + 0.5f,
                                ContactHaveMission == ContactHaveMissionCheckType.Disabled ? 10f : 50f);

                var giver = Giver;
                if (giver.Type == MissionGiverType.NPC)
                {
                    var giverPos = giver.Position;
                    var giverDistance = giver.Distance;

                    if (giverDistance < searchDistance
                        && Math.Abs(giverPos.Z - EntityManager.LocalPlayer.Z) <= InteractZDifference)
                    {
                        if (debugInfoEnabled)
                            ETLogger.WriteLine(LogType.Debug,
                                $"{currentMethodName}: Distance to the Giver is {giverDistance:N1}. Result 'true'");
                        needToRun = true;
                    }
                    else if (debugInfoEnabled)
                        ETLogger.WriteLine(LogType.Debug,
                            $"{currentMethodName}: Faraway({giverDistance:N1}). Result 'False'");
                }
                else if (giver.Type == MissionGiverType.Remote)
                {
                    if (debugInfoEnabled)
                        ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Giver is remote. Result 'True'");
                    needToRun = true;
                }

                if (IgnoreCombat && !needToRun
                    && CheckingIgnoreCombatCondition())
                {
                    AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(true, IgnoreCombatMinHP, 5_000);

                    if (debugInfoEnabled)
                        ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Disable combat");
                }

                return needToRun;
            }
        }

        public override ActionResult Run()
        {
            bool debugInfoEnabled = ExtendedDebugInfo;

            string currentMethodName = debugInfoEnabled
                    ? string.Concat(_idStr, '.', MethodBase.GetCurrentMethod()?.Name ?? nameof(Run))
                    : string.Empty;

            if (debugInfoEnabled)
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Begins try #{tries}");

            if (!IntenalConditions)
            {
                if (debugInfoEnabled)
                    ETLogger.WriteLine(LogType.Debug,
                        $"{currentMethodName}: {nameof(IntenalConditions)} are False => ActionResult = '{ActionResult.Fail}{'\''}");
                return ActionResult.Fail;
            }

            if (IgnoreCombat)
            {
                bool inCombat = EntityManager.LocalPlayer.InCombat;
                if (debugInfoEnabled)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Engage combat ",
                        inCombat ? " => ActionResult = '" + ActionResult.Running + "'" : string.Empty));
                AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(false);
                if (inCombat)
                    return ActionResult.Running;
            }

            var giver = Giver;
            switch (giver.Type)
            {
                case MissionGiverType.NPC:
                    {
                        float interactDistance = Math.Max(InteractDistance + 0.5f, 5.5f);
                        var giverPos = giver.Position;

                        // Производит поиск NPC-Квестодателя
                        if (giverContactInfo is null || !giver.IsMatch(giverContactInfo.Entity))
                        {
                            giverContactInfo = null;
                            foreach (ContactInfo contactInfo in EntityManager.LocalPlayer.Player.InteractInfo.NearbyContacts)
                            {
                                var contactEntity = contactInfo.Entity;
                                var contactLocation = contactEntity.Location;


                                if (Math.Abs(giverPos.Z - contactLocation.Z) <= InteractZDifference &&
                                    giver.IsMatch(contactEntity))
                                {
                                    giverContactInfo = contactInfo;

                                    if (debugInfoEnabled)
                                        ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Set GiverContactInfo to [{giverContactInfo.GetDebugDescription()}]");
                                    break;
                                }
                            }

                            if (giverContactInfo is null && giver.Distance < interactDistance)
                            {
                                // Персонаж прибыл на место, однако, нужный NPC отсутствует
                                // невозможно ни принять миссию, ни пропустить команду
                                tries++;

                                if (debugInfoEnabled)
                                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: GiverContact is absent...");

                                goto Results;
                            }
                        }
                        else if (debugInfoEnabled)
                            ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Keep GiverContactInfo [{giverContactInfo.GetDebugDescription()}]");

                        if (giverContactInfo != null)
                        {
                            Entity entity = giverContactInfo.Entity;
                            if (debugInfoEnabled)
                                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Entity [{entity.GetDebugDescription()}] match to MissionGiverInfo [{giver}]");

                            // Проверяем наличие задания у контакта 
                            var contactHaveMission = ContactHaveMission;
                            if (contactHaveMission != ContactHaveMissionCheckType.Disabled)
                            {
                                if (!giverContactInfo.ContactHaveMission(contactHaveMission))
                                {
                                    if (debugInfoEnabled)
                                        ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: ContactHaveMission is False => ActionResult = '{ActionResult.Skip}'");
                                    return ActionResult.Skip;
                                }

                                if (debugInfoEnabled)
                                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: ContactHaveMission is True. Continue...");
                            }
                            else if (debugInfoEnabled)
                                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Skipped checking the condition 'ContactHaveMission'");

                            // Перемещаемся к квестодателю (в случае необходимости)
                            if (!entity.ApproachMissionGiver(InteractDistance, InteractZDifference))
                            {
                                if (debugInfoEnabled)
                                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: ApproachMissionGiver failed => ActionResult = '{ActionResult.Running}'");
                                return ActionResult.Running;
                            }
                            if (debugInfoEnabled)
                                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: ApproachMissionGiver succeeded");

                            // Взаимодействуем с квестодателем
                            if (!entity.InteractMissionGiver(InteractDistance))
                            {
                                if (debugInfoEnabled)
                                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: InteractMissionGiver failed => ActionResult = '{ActionResult.Running}'");
                                return ActionResult.Running;
                            }
                            if (debugInfoEnabled)
                                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: InteractMissionGiver succeeded");
                        }

                        break;
                    }
                case MissionGiverType.Remote:
                    {
                        if (debugInfoEnabled)
                            ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Calling 'RemoteContact'");
                        var id = giver.Id;
                        var remoteContact = EntityManager.LocalPlayer.Player.InteractInfo.RemoteContacts.FirstOrDefault(ct => ct.ContactDef == id);

                        //TODO для вызова RemoteVendor, которые не видные в окне выбора, нужно реализовать торговлю как в QuesterAssistant через Injection.cmdwrapper_contact_StartRemoteContact(this.RemoteContact);
                        if (remoteContact != null && remoteContact.IsValid)
                        {
                            remoteContact.Start();
                            if (debugInfoEnabled)
                                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Call RemoteContact '{remoteContact.ContactDef}'");
                        }
                        else
                        {
                            tries++;

                            if (debugInfoEnabled)
                                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: RemoteContact '{id}' does not found");

                            goto Results;
                        }

                        break;
                    }
                default:
                    {
                        if (debugInfoEnabled)
                            ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Invalid Giver settings => {ActionResult.Skip}");

                        return ActionResult.Skip;
                    }
            }

            // Проводим попытку принять задание
#if false
            MissionProcessingResult processingResult = ProccessingDialog(); 
#else
            MissionProcessingResult processingResult = MissionHelper.ProcessingMissionDialog(MissionId, false, Dialogs, RewardItemCheck, TIME);
#endif
            tries++;

            if (debugInfoEnabled)
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: ProcessingDialog result is '{processingResult}");
            switch (processingResult)
            {
                case MissionProcessingResult.MissionAccepted:
                    tries = int.MaxValue;
                    if (CloseContactDialog)
                    {
                        GameHelper.CloseAllFrames();
                        Thread.Sleep(2000);
                    }
                    else QuesterAssistantAccessors.Classes.Monitoring.Frames.Sleep(2000);
                    return ActionResult.Completed;
                case MissionProcessingResult.MissionOfferAccepted:
                    Thread.Sleep(1000);
                    break;
                case MissionProcessingResult.MissionNotFound:
                    if (SkipOnFail)
                    {
                        ETLogger.WriteLine($"{currentMethodName}: Mission not available...", true);
                        return ActionResult.Skip;
                    }
                    break;
                case MissionProcessingResult.MissionRequiredRewardMissing:
                    ETLogger.WriteLine($"{currentMethodName}: Required mission reward not found...", true);
                    if (CloseContactDialog)
                    {
                        GameHelper.CloseAllFrames();
                        Thread.Sleep(1000);
                    }
                    else QuesterAssistantAccessors.Classes.Monitoring.Frames.Sleep(2000);

                    if (TargetAction != Guid.Empty)
                    {
                        var targetAction = Astral.Quester.API.CurrentProfile.GetActionByID(TargetAction);
                        if (targetAction == null)
                        {
                            //ETLogger.WriteLine($"{currentMethodName}: {GetPropertyDisplayName(o => o.TargetAction)}: {nameof(TargetAction)} not found !", true);
                            ETLogger.WriteLine($"{currentMethodName}: Action[{TargetAction}] not found !", true);
                            return ActionResult.Fail;
                        }
                        ETLogger.WriteLine($"Set action to '{targetAction}' {targetAction.ActionID}");
                        Astral.Quester.API.CurrentProfile.MainActionPack.SetStartPoint(targetAction);

                        AstralAccessors.Controllers.BotComs.BotServer.ForceRefreshTasks = true;
                    }
                    return ActionResult.Skip;
                case MissionProcessingResult.Error:
                    GameHelper.CloseAllFrames();
                    Thread.Sleep(1000);
                    break;
            }

        Results:
            ActionResult result = (SkipOnFail && tries < 3)
                ? ActionResult.Running
                : ActionResult.Fail;

            if (debugInfoEnabled)
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName} => {result}");

            return result;
        }

        public override string ActionLabel
        {
            get
            {
                if (string.IsNullOrEmpty(label))
                    label = $"{GetType().Name}: [{MissionId}]";
                return label;
            }
        }

        public override string InternalDisplayName => string.Empty;

        protected override bool IntenalConditions
        {
            get
            {
                bool isGiverAccessible = Giver.IsAccessible;

                bool haveMission = MissionHelper.HaveMission(MissionId, out _);
                bool result = isGiverAccessible && !haveMission;
                if (ExtendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug,
                        $"{_idStr}.{nameof(IntenalConditions)}: GiverAccessible({isGiverAccessible}) AND Not(HaveMission({haveMission})) => {result}");
                return result;
            }
        }

        protected override ActionValidity InternalValidity
        {
            get
            {
                if (!Giver.IsValid)
                    return new ActionValidity("Invalid Giver.");
                if (string.IsNullOrEmpty(MissionId))
                    return new ActionValidity("Invalid mission id.");
                return new ActionValidity();
            }
        }

        public override bool UseHotSpots => false;

        protected override Vector3 InternalDestination
        {
            get
            {
                var giver = Giver;
                if (giver.IsAccessible
                    && giver.Distance >= InteractDistance)
                    return giver.Position.Clone();
                return Vector3.Empty;
            }
        }

        public override void InternalReset()
        {
            tries = 0;
            giverContactInfo = null;
            label = string.Empty;
        }

        public override void GatherInfos()
        {
            label = string.Empty;
            MissionGiverInfo missionGiver = null;
            string missionId = string.Empty;
            MissionProcessingResult missionResult = MissionProcessingResult.Error;
            if (MissionGiverInfoEditor.SetInfos(ref missionGiver))
            {
                if (missionGiver.Type == MissionGiverType.NPC)
                {
                    Entity entity = Interact.GetBetterEntityToInteract();
                    // проверять не имеет смысла, поскольку данные квестодатель только-что был указан пользователем
                    // if (!missionGiver.IsMatching(entity)) return;

                    //BUG ApproachMissionGiver не подводит к NPC
                    var interactDistance = InteractDistance;
                    if (!entity.ApproachMissionGiver(interactDistance, InteractZDifference)) return;

                    if (!entity.InteractMissionGiver(interactDistance)) return;

                }
                else if (missionGiver.Type == MissionGiverType.Remote)
                {
                    var remoteContact = EntityManager.LocalPlayer.Player.InteractInfo.RemoteContacts.FirstOrDefault(ct => ct.ContactDef == missionGiver.Id);

                    if (remoteContact is null || !remoteContact.IsValid) return;
                }
                else return;

                //TODO: Проверять иконку на квестодателе, и если она отличается деактивировать опцию ContactHaveMission
                Interact.WaitForInteraction();

                var interactInfo = EntityManager.LocalPlayer.Player.InteractInfo;
                var contactDialog = interactInfo.ContactDialog;

                if (contactDialog.Options.Count > 0)
                {
                    string aDialogKey = MissionHelper.GetADialogOption(out ContactDialogOption contactDialogOption)
                        ? contactDialogOption.Key : string.Empty;

                    // BUG: Входит в бесконечный цикл, если выбран неативный пункт диалога, соответствующий недоступной и (или) ранее принятой миссии
                    while (!string.IsNullOrEmpty(aDialogKey))
                    {
                        // Ищем индекс начала "текстового идентификатора миссии" в выбранном пункте диалога
                        // "OptionsList.MissionOffer.Текстовый_Идентификатор_Миссии_\d*"
                        int startInd = aDialogKey.IndexOf("MissionOffer.", 0, StringComparison.OrdinalIgnoreCase);

                        if (startInd >= 0)
                        {
                            // Вычисляем идентификатор миссии из строки диалога
                            // Индекс окончания "текстового идентификатора миссии"
                            // Если последний символ - цифра, значит нужно удалить суффикс
                            int lastInd = char.IsDigit(aDialogKey[aDialogKey.Length - 1])
                                ? aDialogKey.LastIndexOf('_')
                                : aDialogKey.Length - 1;

                            if (startInd + MISSION_OFFER_LENGTH < aDialogKey.Length)
                            {
                                missionId = aDialogKey.Substring(startInd + MISSION_OFFER_LENGTH, lastInd - startInd - MISSION_OFFER_LENGTH);

                                if (!string.IsNullOrEmpty(missionId))
                                {
                                    // Принимаем миссию
                                    interactInfo.ContactDialog.SelectOptionByKey(aDialogKey);
                                    var timer = new Astral.Classes.Timeout(TIME);
                                    do
                                    {
                                        Thread.Sleep(100);
                                    }
                                    while (!timer.IsTimedOut && interactInfo.ContactDialog.ScreenType != ScreenType.MissionOffer);

                                    if (interactInfo.ContactDialog.ScreenType == ScreenType.MissionOffer)
                                    {
                                        // Выбираем обязательную награду
                                        GetAnItem.ListItem rewardItem = GetAnItem.Show(4);
                                        if (rewardItem != null && !string.IsNullOrEmpty(rewardItem.ItemId))
                                        {
                                            RequiredRewardItem = rewardItem.ItemId;
                                            //patternPos = PUM.RequiredRewardItem.GetSimplePatternPosition(out rewardItemPattern);
                                        }

                                        // Принимаем миссию
                                        interactInfo.ContactDialog.SelectOptionByKey("ViewOfferedMission.Accept");

                                        // Проверяем корректность определения "идентификатора миссии"
                                        timer.Reset();
                                        do
                                        {
                                            Thread.Sleep(250);
                                            if (MissionHelper.HaveMission(missionId, out _))
                                            {
                                                missionResult = MissionProcessingResult.MissionAccepted;
                                                break;
                                            }
                                        }
                                        while (!timer.IsTimedOut);
                                    }
                                }
                            }
                            break;
                        }
                        else
                        {
                            // Выбранный пункт диалога не содержит идентификатора миссии
                            // добавляем его в Dialogs
                            Dialogs.Add(aDialogKey);
                            interactInfo.ContactDialog.SelectOptionByKey(aDialogKey);
                            Thread.Sleep(1000);
#if false
                            aDialogKey = GetAnId.GetADialogKey(); 
#elif false
                            MissionHelper.GetADialogKey(out aDialogKey);
#else
                            aDialogKey = MissionHelper.GetADialogOption(out contactDialogOption)
                                            ? contactDialogOption.Key : string.Empty;

#endif
                        }
                    }

                    // Ручной выбор идентификатора миссии
                    if (missionResult != MissionProcessingResult.MissionAccepted)
                        missionId = GetMissionId.Show(true, missionId);
                }

                if (!string.IsNullOrEmpty(missionId)
                    && missionGiver.IsValid)
                {
                    MissionId = missionId;
                    Giver = missionGiver;

                    label = string.Empty;
                }
            }
        }

        public override void OnMapDraw(GraphicsNW graphics)
        {
            var giver = Giver;
            if (giver != null && giver.Position.IsValid)
            {
                if (graphics is MapperGraphics graphicsExt)
                    graphicsExt.FillCircleCentered(Brushes.Beige, giver.Position, 10);
                else graphics.drawFillEllipse(giver.Position, MapperHelper.Size_10x10, Brushes.Beige);
            }
        }

        #region Вспомогательные методы
        /// <summary>
        /// Флаг настроек вывода расширенной отлаточной информации
        /// </summary>
        private bool ExtendedDebugInfo
        {
            get
            {
                var logConf = EntityTools.Config.Logger;
                return logConf.QuesterActions.DebugPickUpMissionExt && logConf.Active;
            }
        }

        private Predicate<Item> RewardItemCheck
        {
            get
            {
                if (_rewardItemCheck is null)
                {
                    var requiredRewardItem = RequiredRewardItem;
                    if (!string.IsNullOrEmpty(requiredRewardItem))
                    {
                        var strPred = RequiredRewardItem.GetComparer();
                        if (strPred != null)
                            return _rewardItemCheck = itm => strPred(itm.ItemDef.InternalName);
                    }
                    _rewardItemCheck = itm => true;
                }
                return _rewardItemCheck;
            }
        }
        Predicate<Item> _rewardItemCheck;

        /// <summary>
        /// Проверка условия отключения боя <see cref="PickUpMissionExt.IgnoreCombatCondition"/>
        /// </summary>
        /// <returns>Результат проверки <see cref="PickUpMissionExt.IgnoreCombatCondition"/> либо True, если оно не задано.</returns>
        private bool CheckingIgnoreCombatCondition()
        {
            var check = IgnoreCombatCondition;
            if (check != null)
                return check.IsValid;
            return true;
        }
        #endregion
    }
}
