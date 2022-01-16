#define MissionGiverInfo

using AcTp0Tools;
using Astral.Logic.Classes.Map;
using Astral.Logic.NW;
using Astral.Quester.Forms;
using EntityCore.Enums;
using EntityCore.Tools;
using EntityTools;
using EntityTools.Core.Interfaces;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Patches.Mapper;
using EntityTools.Quester.Actions;
using EntityTools.Tools;
using EntityTools.Tools.Missions;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using static Astral.Quester.Classes.Action;

namespace EntityCore.Quester.Action
{
    //TODO: Добавить предобработку MissionId (деление на подзадачи по знаку '\')
    public class TurnInMissionEngine : IQuesterActionEngine
    {
        private TurnInMissionExt @this;

        #region данные ядра
        private const int TIME = 10_000;

        private ContactInfo giverContactInfo;
        private int tries;
        private Astral.Classes.Timeout failTo;
        private string _label = string.Empty;
        private string _idStr = string.Empty;
        #endregion

        public TurnInMissionEngine(TurnInMissionExt tim) 
        {
            InternalRebase(tim);

            ETLogger.WriteLine(LogType.Debug, string.Concat(_idStr, "initialized: ", ActionLabel));
        }
        ~TurnInMissionEngine()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (@this != null)
            {
                @this.Unbind();
                @this = null;
            }
        }

        public bool Rebase(Astral.Quester.Classes.Action action)
        {
            if (action is null)
                return false;
            if (ReferenceEquals(action, @this))
                return true;
            if (action is TurnInMissionExt tim)
            {
                InternalRebase(tim);
                ETLogger.WriteLine(LogType.Debug, $"{_idStr} reinitialized");
                return true;
            }
#if false
            else ETLogger.WriteLine(LogType.Debug, $"Rebase failed. '{action}' has type '{action.GetType().Name}' which not equals to '{nameof(TurnInMissionExt)}'");
            return false; 
#else
            string debugStr = string.Concat("Rebase failed. ", action.GetType().Name, '[', action.ActionID, "] can't be casted to '" + nameof(TurnInMissionExt) + '\'');
            ETLogger.WriteLine(LogType.Error, debugStr);
            throw new InvalidCastException(debugStr);
#endif

        }

        private bool InternalRebase(TurnInMissionExt tim)
        {
            // Убираем привязку к старой команде
            @this?.Unbind();

            @this = tim;
            @this.PropertyChanged += OnPropertyChanged;

            //isRewardItem = initialize_IsRewardItem;
            _rewardItemCheck = null;

            _idStr = string.Concat(@this.GetType().Name, '[', @this.ActionID, ']');

            @this.Bind(this);

            return true;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(sender as Astral.Quester.Classes.Action, e.PropertyName);
        }
        public void OnPropertyChanged(Astral.Quester.Classes.Action sender, string propertyName)
        {
            if (!ReferenceEquals(sender, @this)) return;

            switch (propertyName)
            {
                case nameof(@this.RequiredRewardItem):
                    _rewardItemCheck = null;
                    break;
                case nameof(@this.MissionId):
                    _label = string.Empty;
                    break;
            }
        }

        public bool NeedToRun
        {
            get
            {
                bool debugInfoEnabled = ExtendedDebugInfo;

                string currentMethodName = debugInfoEnabled
                    ? $"{_idStr}.{MethodBase.GetCurrentMethod().Name}"
                    : string.Empty;

                bool needToRun = false;

                if (debugInfoEnabled)
                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Begins");

                float searchDistance = Math.Max(@this._interactDistance + 0.5f, 10f);
                //float interactDistance = Math.Max(@this._interactDistance + 0.5f, 5.5f);


                if (@this._giver.Type == MissionGiverType.NPC)
                {
                    var giverPos = @this._giver.Position;
                    var giverDistance = @this._giver.Distance;

                    if (giverDistance < searchDistance
                        && Math.Abs(giverPos.Z - EntityManager.LocalPlayer.Z) <= @this._interactZDifference)
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
                else if(@this._giver.Type == MissionGiverType.Remote)
                {
                    if (debugInfoEnabled)
                        ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Giver is remote. Result 'True'");
                    needToRun = true;
                }

                if (@this._ignoreCombat && !needToRun)
                {
                    AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(true);

                    if (debugInfoEnabled)
                        ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Disable combat");
                }

                return needToRun;
            }
        }

        public ActionResult Run()
        {
            bool debugInfoEnabled = ExtendedDebugInfo;

            string currentMethodName = debugInfoEnabled
                    ? string.Concat(_idStr, '.', MethodBase.GetCurrentMethod().Name)
                    : string.Empty;

            if (debugInfoEnabled)
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Begins try #{tries}");

            if (!InternalConditions)
            {
                if (debugInfoEnabled)
                    ETLogger.WriteLine(LogType.Debug,
                        $"{currentMethodName}: InternalConditions is False => ActionResult = '{ActionResult.Fail}'");
                return ActionResult.Fail;
            }
            if (failTo is null)
                failTo = new Astral.Classes.Timeout(30000);

            if (@this._ignoreCombat)
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

            if (@this._giver.Type == MissionGiverType.NPC)
            {
                float interactDistance = Math.Max(@this._interactDistance, 5.5f);
                var giverPos = @this._giver.Position;

                // Производит поиск NPC-Квестодателя
                if (giverContactInfo is null || !@this._giver.IsMatch(giverContactInfo.Entity))
                {
                    giverContactInfo = null;
                    foreach (ContactInfo contactInfo in EntityManager.LocalPlayer.Player.InteractInfo.NearbyContacts)
                    {
                        var contactEntity = contactInfo.Entity;
                        var contactLocation = contactEntity.Location;


                        if (Math.Abs(giverPos.Z - contactLocation.Z) >= @this._interactZDifference
                            || !@this._giver.IsMatch(contactEntity))
                                continue;

                        giverContactInfo = contactInfo;

                        if (debugInfoEnabled)
                            ETLogger.WriteLine(LogType.Debug,
                                $"{currentMethodName}: Set GiverContactInfo to [{giverContactInfo.GetDebugDescription()}]");
                        break;
                    }

                    if (giverContactInfo is null && @this._giver.Distance < interactDistance)
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
                            $"{currentMethodName}: Entity [{entity.GetDebugDescription()}] match to MissionGiverInfo [{@this._giver}]");

                    // Перемещаемся к квестодателю (в случае необходимости)
                    if (!entity.ApproachMissionGiver(interactDistance, @this._interactZDifference))
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
            else if (@this._giver.Type == MissionGiverType.Remote)
            {
                if (debugInfoEnabled)
                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Call 'RemoteContact'");
                var id = @this._giver.Id;
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
            MissionProcessingResult processingResult = MissionHelper.ProcessingMissionDialog(@this._missionId, true, @this._dialogs, RewardItemCheck, TIME);

            tries++;

            if (debugInfoEnabled)
                ETLogger.WriteLine(LogType.Debug,
                    $"{currentMethodName}: ProcessingDialog result is '{processingResult}'");
            switch (processingResult)
            {
                case MissionProcessingResult.MissionTurnedIn:
                    tries = int.MaxValue;
                    if (@this._closeContactDialog)
                    {
                        GameHelper.CloseAllFrames();
                        Thread.Sleep(2000);
                    }
                    else QuesterAssistantAccessors.Classes.Monitoring.Frames.Sleep(2000);
                    return ActionResult.Completed;
                case MissionProcessingResult.MissionRequiredRewardNotFound:
                    ETLogger.WriteLine($"{currentMethodName}: Required mission reward not found...", true);
                    if (@this._closeContactDialog)
                    {
                        GameHelper.CloseAllFrames();
                        Thread.Sleep(2000);
                    }
                    else QuesterAssistantAccessors.Classes.Monitoring.Frames.Sleep(2000);
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

        public string ActionLabel
        {
            get
            {
                if (string.IsNullOrEmpty(_label))
                    _label = $"{@this.GetType().Name}: [{@this._missionId}]";
                return _label;
            }
        }

        public bool InternalConditions
        {
            get
            {
                bool isGiverAccessible = @this._giver.IsAccessible;
                uint bagsFreeSlots = EntityManager.LocalPlayer.BagsFreeSlots;
                bool isMissionSucceeded = MissionHelper.HaveMission(@this._missionId, out Mission mission) && mission.State == MissionState.Succeeded;
                bool result = isGiverAccessible && bagsFreeSlots > 0 && isMissionSucceeded;
                if (ExtendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug,
                        $"{_idStr}.{nameof(InternalConditions)}: GiverAccessible({isGiverAccessible}) AND BagsFreeSlots({bagsFreeSlots}) AND MissionSucceeded({isMissionSucceeded}) => {result}");
                return result;
            }
        }

        public ActionValidity InternalValidity
        {
            get
            {
                if (!@this._giver.IsValid) 
                    return new ActionValidity("Invalid Giver.");
                if (string.IsNullOrEmpty(@this._missionId))
                    return new ActionValidity("Invalid mission id.");
                return new ActionValidity();
            }
        }

        public bool UseHotSpots => false;

        public Vector3 InternalDestination
        {
            get
            {
                if (@this._giver.IsAccessible
                    && @this._giver.Distance >= @this._interactDistance)
                        return @this._giver.Position.Clone();
                return Vector3.Empty;
            }
        }

        public void InternalReset()
        {
            tries = 0;
            giverContactInfo = null;
            _label = string.Empty;
            failTo = null;
        }

        public void GatherInfos()
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
                if (!entity.ApproachMissionGiver(@this._interactDistance, @this._interactZDifference)) return;

                if (!entity.InteractMissionGiver(@this._interactDistance)) return; 

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
                                            @this.RequiredRewardItem = rewardItem.ItemId;
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
                            @this._dialogs.Add(aDialogKey);
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
                    @this._missionId = missionId;
                    @this._giver = missionGiver;

                    _label = string.Empty;
                }
            }
        }

        public void OnMapDraw(GraphicsNW graphics)
        {
            if (@this._giver != null && @this._giver.Position.IsValid)
            {
                if (graphics is MapperGraphics graphicsExt)
                    graphicsExt.FillCircleCentered(Brushes.Beige, @this._giver.Position, 10);
                else graphics.drawFillEllipse(@this._giver.Position, MapperHelper.Size_10x10, Brushes.Beige);
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
                var logConf = EntityTools.EntityTools.Config.Logger;
                return logConf.QuesterActions.DebugTurnInMissionExt && logConf.Active;
            }
        }

        private Predicate<Item> RewardItemCheck
        {
            get
            {
                if (_rewardItemCheck is null)
                {
                    if (!string.IsNullOrEmpty(@this._requiredRewardItem))
                    {
                        var strPred = StringToPatternComparer.GetComparer(@this._requiredRewardItem);
                        if (strPred != null)
                            return _rewardItemCheck = itm => strPred(itm.ItemDef.InternalName);
                    }
                    _rewardItemCheck = itm => true;
                }
                return _rewardItemCheck;
            }
        }
        Predicate<Item> _rewardItemCheck;
        #endregion
    }
}