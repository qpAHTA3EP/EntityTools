using ACTP0Tools;
using Astral.Logic.Classes.Map;
using Astral.Logic.NW;
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
    public class TurnInMissionExt : Action, INotifyPropertyChanged
    {
        #region данные ядра
        private const int TIME = 10_000;

        private ContactInfo giverContactInfo;
        private int tries;
        private Astral.Classes.Timeout failTo;
        private string _label = string.Empty;
        private string _idStr = string.Empty;
        #endregion




        #region Mission Options
#if DEVELOPER
        [Description("Identifier of the Mission.\n\r" +
            "Allows simple mask (*) at the begin and at the end")]
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
            get => string.Empty;//_giver.Id;
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
                {
                    _giver.Position = value;
                }
            }
        }
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


        #region Interaction
#if DEVELOPER
        [Description("The minimum value is 5")]
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
        [Description("The minimum value is 1")]
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
                if (Math.Abs(_interactZDifference - value) < 0.1) return;
                _interactZDifference = value;
                NotifyPropertyChanged();
            }
        }
        private float _interactZDifference = 10;

#if DEVELOPER
        [Description("Answers in dialog which have to be performed before mission picking up")]
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Dialogs)));
                }
            }
        }
        private List<string> _dialogs = new List<string>();

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


        #region Optional
#if DEVELOPER
        [Editor(typeof(RewardsEditor), typeof(UITypeEditor))]
        [Description("Item that is requered in rewards to TurnInMission\n" +
                     "Simple wildcard (*) is allowed\n" +
                     "Mission Offer dialog have to be opened for choosen the ReqieredRewardItem")]
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
        [Description("The Id of the Action executing if the '" + nameof(RequiredRewardItem) + "' would be missing")]
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

        #region Взаимодействие с EntityToolsCore
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            InternalResetOnPropertyChanged(propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void InternalResetOnPropertyChanged([CallerMemberName] string propertyName = default)
        {
            switch (propertyName)
            {
                case nameof(RequiredRewardItem):
                    _rewardItemCheck = null;
                    break;
                case nameof(MissionId):
                    _label = string.Empty;
                    break;
            }
        }
        #endregion




        public override bool NeedToRun
        {
            get
            {
                bool debugInfoEnabled = ExtendedDebugInfo;

                string currentMethodName = debugInfoEnabled
                    ? $"{_idStr}.{MethodBase.GetCurrentMethod()?.Name ?? nameof(NeedToRun)}"
                    : string.Empty;

                bool needToRun = false;

                if (debugInfoEnabled)
                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Begins");

                float searchDistance = Math.Max(InteractDistance + 0.5f, 10f);
                //float interactDistance = Math.Max(_interactDistance + 0.5f, 5.5f);

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
                                $"{currentMethodName}: Distance to the Giver is {giverDistance:N1}. Result 'True'");
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
                        $"{currentMethodName}: InternalConditions is False => ActionResult = '{ActionResult.Fail}'");
                return ActionResult.Fail;
            }
            if (failTo is null)
                failTo = new Astral.Classes.Timeout(30000);

            if (IgnoreCombat)
            {
                bool inCombat = EntityManager.LocalPlayer.InCombat;
                if (debugInfoEnabled)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Engage combat ",
                        inCombat ? " => ActionResult = '" + ActionResult.Running + "'"
                                 : string.Empty));
                AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(false);
                if (inCombat)
                    return ActionResult.Running;
            }

            var giver = Giver;
            if (giver.Type == MissionGiverType.NPC)
            {
                float interactDistance = Math.Max(InteractDistance, 5.5f);
                var giverPos = giver.Position;

                // Производит поиск NPC-Квестодателя
                if (giverContactInfo is null || !giver.IsMatch(giverContactInfo.Entity))
                {
                    giverContactInfo = null;
                    foreach (ContactInfo contactInfo in EntityManager.LocalPlayer.Player.InteractInfo.NearbyContacts)
                    {
                        var contactEntity = contactInfo.Entity;
                        var contactLocation = contactEntity.Location;


                        if (Math.Abs(giverPos.Z - contactLocation.Z) >= InteractZDifference
                            || !giver.IsMatch(contactEntity))
                            continue;

                        giverContactInfo = contactInfo;

                        if (debugInfoEnabled)
                            ETLogger.WriteLine(LogType.Debug,
                                $"{currentMethodName}: Set GiverContactInfo to [{giverContactInfo.GetDebugDescription()}]");
                        break;
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
                    ETLogger.WriteLine(LogType.Debug,
                        $"{currentMethodName}: Keep GiverContactInfo [{giverContactInfo.GetDebugDescription()}]");

                if (giverContactInfo != null)
                {
                    Entity entity = giverContactInfo.Entity;
                    if (debugInfoEnabled)
                        ETLogger.WriteLine(LogType.Debug,
                            $"{currentMethodName}: Entity [{entity.GetDebugDescription()}] match to MissionGiverInfo [{giver}]");

                    // Перемещаемся к квестодателю (в случае необходимости)
                    if (!entity.ApproachMissionGiver(interactDistance, InteractZDifference))
                    {
                        if (debugInfoEnabled)
                            ETLogger.WriteLine(LogType.Debug,
                                $"{currentMethodName}: ApproachMissionGiver failed => ActionResult = '{ActionResult.Running}'");
                        return ActionResult.Running;
                    }
                    if (debugInfoEnabled)
                        ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: ApproachMissionGiver succeeded");

                    // Взаимодействуем с квестодателем
                    if (!entity.InteractMissionGiver(interactDistance))
                    {
                        if (debugInfoEnabled)
                            ETLogger.WriteLine(LogType.Debug,
                                $"{currentMethodName}: InteractMissionGiver failed => ActionResult = '{ActionResult.Running}'");
                        return ActionResult.Running;
                    }
                    if (debugInfoEnabled)
                        ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: InteractMissionGiver succeeded");
                }
            }
            else if (giver.Type == MissionGiverType.Remote)
            {
                if (debugInfoEnabled)
                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Call 'RemoteContact'");
                var id = giver.Id;
                var remoteContact = EntityManager.LocalPlayer.Player.InteractInfo.RemoteContacts.FirstOrDefault(ct => ct.ContactDef == id);

                if (remoteContact != null && remoteContact.IsValid)
                {
                    remoteContact.Start();
                    if (debugInfoEnabled)
                        ETLogger.WriteLine(LogType.Debug,
                            $"{currentMethodName}: Call RemoteContact '{remoteContact.ContactDef}'");
                }
                else
                {
                    tries++;

                    if (debugInfoEnabled)
                        ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: RemoteContact '{id}' does not found");

                    goto Results;
                }
            }
            else
            {
                if (debugInfoEnabled)
                    ETLogger.WriteLine(LogType.Debug,
                        $"{currentMethodName}: Invalid Giver settings => {ActionResult.Skip}");

                return ActionResult.Skip;
            }

            // Проводим попытку принять задание
            MissionProcessingResult processingResult = MissionHelper.ProcessingMissionDialog(MissionId, true, Dialogs, RewardItemCheck, TIME);

            tries++;

            if (debugInfoEnabled)
                ETLogger.WriteLine(LogType.Debug,
                    $"{currentMethodName}: ProcessingDialog result is '{processingResult}'");
            switch (processingResult)
            {
                case MissionProcessingResult.MissionTurnedIn:
                    tries = int.MaxValue;
                    if (CloseContactDialog)
                    {
                        GameHelper.CloseAllFrames();
                        Thread.Sleep(2000);
                    }
                    else QuesterAssistantAccessors.Classes.Monitoring.Frames.Sleep(2000);
                    return ActionResult.Completed;
                case MissionProcessingResult.MissionRequiredRewardMissing:
                    ETLogger.WriteLine($"{currentMethodName}: Required mission reward not found...", true);
                    if (CloseContactDialog)
                    {
                        GameHelper.CloseAllFrames();
                        Thread.Sleep(2000);
                    }
                    else QuesterAssistantAccessors.Classes.Monitoring.Frames.Sleep(2000);

                    if (TargetAction != Guid.Empty)
                    {
                        var targetAction = Astral.Quester.API.CurrentProfile.GetActionByID(TargetAction);
                        if (targetAction == null)
                        {
                            ETLogger.WriteLine($"{currentMethodName}: Action[{TargetAction}] not found !", true);
                            return ActionResult.Fail;
                        }
                        ETLogger.WriteLine($"Set action to '{targetAction}' {targetAction.ActionID}");
                        Astral.Quester.API.CurrentProfile.MainActionPack.SetStartPoint(targetAction);

                        AstralAccessors.Controllers.BotComs.BotServer.ForceRefreshTasks = true;
                    }
                    return ActionResult.Skip;
                default:
                    GameHelper.CloseAllFrames();
                    Thread.Sleep(1000);
                    break;
            }

        Results:
            ActionResult result = failTo.IsTimedOut || tries < 3
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
                if (string.IsNullOrEmpty(_label))
                    _label = $"{GetType().Name}: [{MissionId}]";
                return _label;
            }
        }

        public override string InternalDisplayName => string.Empty;

        protected override bool IntenalConditions
        {
            get
            {
                bool isGiverAccessible = Giver.IsAccessible;
                uint bagsFreeSlots = EntityManager.LocalPlayer.BagsFreeSlots;
                bool isMissionSucceeded = MissionHelper.HaveMission(MissionId, out Mission mission) && mission.State == MissionState.Succeeded;
                bool result = isGiverAccessible && bagsFreeSlots > 0 && isMissionSucceeded;
                if (ExtendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug,
                        $"{_idStr}.{nameof(IntenalConditions)}: GiverAccessible({isGiverAccessible}) AND BagsFreeSlots({bagsFreeSlots}) AND MissionSucceeded({isMissionSucceeded}) => {result}");
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
            _label = string.Empty;
            failTo = null;
        }

        public override void GatherInfos()
        {
            MissionGiverInfo missionGiver = null;
            string missionId = string.Empty;
            bool missionIdDetected = false;
            if (MissionGiverInfoEditor.SetInfos(ref missionGiver, MissionGiverType.NPC))
            {
                Entity entity = Interact.GetBetterEntityToInteract();

                // проверять не имеет смысла, поскольку данные квестодатель только-что был указан пользователем
                // if (!missionGiver.IsMatching(entity)) return;

                // BUG ApproachMissionGiver не подводит к NPC
                if (!entity.ApproachMissionGiver(InteractDistance, InteractZDifference)) return;

                if (!entity.InteractMissionGiver(InteractDistance)) return;

                Interact.WaitForInteraction();

                var interactInfo = EntityManager.LocalPlayer.Player.InteractInfo;
                var contactDialog = interactInfo.ContactDialog;

                if (contactDialog.Options.Count > 0)
                {
                    string aDialogKey = MissionHelper.GetADialogOption(out ContactDialogOption contactDialogOption)
                        ? contactDialogOption.Key : string.Empty;

                    while (!string.IsNullOrEmpty(aDialogKey))
                    {
                        // Ищем индекс начала "текстового идентификатора миссии" в выбранном пункте диалога
                        // "OptionsList.CompleteMission.Текстовый_Идентификатор_Миссии_\d*"
                        int startInd = aDialogKey.IndexOf("CompleteMission.", 0, StringComparison.OrdinalIgnoreCase);

                        if (startInd >= 0)
                        {
                            // Вычисляем идентификатор миссии из строки диалога
                            // Индекс окончания "текстового идентификатора миссии"
                            // Если последний символ - цифра, значит нужно удалить суффикс
                            int lastInd = char.IsDigit(aDialogKey[aDialogKey.Length - 1])
                                ? aDialogKey.LastIndexOf('_')
                                : aDialogKey.Length - 1;

                            if (startInd + "CompleteMission.".Length < aDialogKey.Length)
                            {
                                missionId = aDialogKey.Substring(startInd + "CompleteMission.".Length, lastInd - startInd - "CompleteMission.".Length);

                                if (!string.IsNullOrEmpty(missionId))
                                {
                                    // Открываем окно сдачи миссии
                                    interactInfo.ContactDialog.SelectOptionByKey(aDialogKey);
                                    var timer = new Astral.Classes.Timeout(TIME);
                                    do
                                    {
                                        Thread.Sleep(100);
                                    }
                                    while (!timer.IsTimedOut && interactInfo.ContactDialog.ScreenType != ScreenType.MissionTurnIn);

                                    if (interactInfo.ContactDialog.ScreenType == ScreenType.MissionTurnIn)
                                    {
                                        // Выбираем обязательную награду
                                        GetAnItem.ListItem rewardItem = GetAnItem.Show(4);
                                        if (rewardItem != null && !string.IsNullOrEmpty(rewardItem.ItemId))
                                        {
                                            RequiredRewardItem = rewardItem.ItemId;
                                            //patternPos = PUM.RequiredRewardItem.GetSimplePatternPosition(out rewardItemPattern);
                                        }

                                        // Проверка через поиск миссии в журнале
                                        timer.Reset();
                                        do
                                        {
                                            Thread.Sleep(250);
                                            if (MissionHelper.HaveMission(missionId, out Mission mission)
                                                && mission.State == MissionState.Succeeded)
                                            {
                                                missionIdDetected = true;
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

                            aDialogKey = GetAnId.GetADialogKey();
                        }
                    }

                    // Ручной выбор идентификатора миссии
                    if (!missionIdDetected)
                        missionId = GetMissionId.Show(true, missionId);
                }

                if (!string.IsNullOrEmpty(missionId)
                    && missionGiver.IsValid)
                {
                    MissionId = missionId;
                    Giver = missionGiver;

                    _label = string.Empty;
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
                return logConf.QuesterActions.DebugTurnInMissionExt && logConf.Active;
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
                        var strPred = requiredRewardItem.GetComparer();
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
        /// Проверка условия отключения боя <see cref="TurnInMissionExt.IgnoreCombatCondition"/>
        /// </summary>
        /// <returns>Результат проверки <see cref="TurnInMissionExt.IgnoreCombatCondition"/> либо True, если оно не задано.</returns>
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
