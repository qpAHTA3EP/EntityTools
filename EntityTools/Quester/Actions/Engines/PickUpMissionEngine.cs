using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using ACTP0Tools;
using Astral.Logic.Classes.Map;
using Astral.Logic.NW;
using Astral.Quester.Forms;
using EntityCore.Tools;
using EntityTools.Core.Interfaces;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Patches.Mapper;
using EntityTools.Tools;
using EntityTools.Tools.Missions;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using static Astral.Quester.Classes.Action;

namespace EntityTools.Quester.Actions.Engines
{
    //TODO: Добавить предобработку MissionId (деление на подзадачи по знаку '\')
    public class PickUpMissionEngine : IQuesterActionEngine
    {
        /// <summary>
        /// Константа длины строки "MissionOffer." 
        /// </summary>
        static readonly int MISSION_OFFER_LENGTH = "MissionOffer.".Length;

        private PickUpMissionExt @this;

        #region данные ядра
        private const int TIME = 10_000;

        private ContactInfo giverContactInfo;
        private int tries;
        private string label = string.Empty;
        private string _idStr = string.Empty;
        #endregion

        public PickUpMissionEngine(PickUpMissionExt pum) 
        {
            InternalRebase(pum);

            ETLogger.WriteLine(LogType.Debug, string.Concat(_idStr, "initialized: ", ActionLabel));
        }
        ~PickUpMissionEngine()
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
            if (action is PickUpMissionExt pum)
            {
                InternalRebase(pum);
                ETLogger.WriteLine(LogType.Debug, $"{_idStr} reinitialized");
                return true;
            }

            string debugStr = string.Concat("Rebase failed. ", action.GetType().Name, '[', action.ActionID, "] can't be cast to '" + nameof(PickUpMissionExt) + '\'');
            ETLogger.WriteLine(LogType.Error, debugStr);
            throw new InvalidCastException(debugStr);
        }

        private bool InternalRebase(PickUpMissionExt pum)
        {
            // Убираем привязку к старой команде
            @this?.Unbind();

            @this = pum;

            //isRewardItem = initializer_IsRewardItem;
            _rewardItemCheck = null;

            _idStr = string.Concat(@this.GetType().Name, '[', @this.ActionID, ']');

            @this.Bind(this);
            @this.PropertyChanged += OnPropertyChanged;

            return true;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(sender as Astral.Quester.Classes.Action, e.PropertyName);
        }

        public void OnPropertyChanged(Astral.Quester.Classes.Action sender, string propertyName)
        {
            if (!ReferenceEquals(sender, @this)) return;
            if (propertyName == nameof(@this.RequiredRewardItem))
                //isRewardItem = initializer_IsRewardItem;
                _rewardItemCheck = null;
            else if (propertyName == nameof(@this.MissionId)) label = string.Empty;
        }

        public bool NeedToRun
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

                float searchDistance = Math.Max(@this.InteractDistance + 0.5f,
                                @this.ContactHaveMission == ContactHaveMissionCheckType.Disabled ? 10f : 50f);

                var giver = @this.Giver;
                if (giver.Type == MissionGiverType.NPC)
                {
                    var giverPos = giver.Position;
                    var giverDistance = giver.Distance;

                    if (giverDistance < searchDistance
                        && Math.Abs(giverPos.Z - EntityManager.LocalPlayer.Z) <= @this.InteractZDifference)
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
                else if(giver.Type == MissionGiverType.Remote)
                {
                    if (debugInfoEnabled)
                        ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Giver is remote. Result 'True'");
                    needToRun = true;
                }

                if (@this.IgnoreCombat && !needToRun
                    && CheckingIgnoreCombatCondition())
                {
                    AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(true, @this.IgnoreCombatMinHP, 5_000);

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
                    ? string.Concat(_idStr, '.', MethodBase.GetCurrentMethod()?.Name ?? nameof(Run) )
                    : string.Empty;

            if (debugInfoEnabled)
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Begins try #{tries}");

            if (!InternalConditions)
            {
                if (debugInfoEnabled)
                    ETLogger.WriteLine(LogType.Debug,
                        $"{currentMethodName}: {nameof(InternalConditions)} are False => ActionResult = '{ActionResult.Fail}{'\''}");
                return ActionResult.Fail;
            }

            if (@this.IgnoreCombat)
            {
                bool inCombat = EntityManager.LocalPlayer.InCombat;
                if (debugInfoEnabled)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Engage combat ",
                        inCombat ? " => ActionResult = '" + ActionResult.Running + "'" : string.Empty));
                AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(false);
                if (inCombat)
                    return ActionResult.Running;
            }

            var giver = @this.Giver;
            switch (giver.Type)
            {
                case MissionGiverType.NPC:
                {
                    float interactDistance = Math.Max(@this.InteractDistance + 0.5f, 5.5f);
                    var giverPos = giver.Position;

                    // Производит поиск NPC-Квестодателя
                    if (giverContactInfo is null || !giver.IsMatch(giverContactInfo.Entity))
                    {
                        giverContactInfo = null;
                        foreach (ContactInfo contactInfo in EntityManager.LocalPlayer.Player.InteractInfo.NearbyContacts)
                        {
                            var contactEntity = contactInfo.Entity;
                            var contactLocation = contactEntity.Location;


                            if (Math.Abs(giverPos.Z - contactLocation.Z) <= @this.InteractZDifference &&
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
                        var contactHaveMission = @this.ContactHaveMission;
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
                        if (!entity.ApproachMissionGiver(@this.InteractDistance, @this.InteractZDifference))
                        {
                            if (debugInfoEnabled)
                                ETLogger.WriteLine(LogType.Debug,  $"{currentMethodName}: ApproachMissionGiver failed => ActionResult = '{ActionResult.Running}'");
                            return ActionResult.Running;
                        }
                        if (debugInfoEnabled)
                            ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: ApproachMissionGiver succeeded");

                        // Взаимодействуем с квестодателем
                        if (!entity.InteractMissionGiver(@this.InteractDistance))
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
            MissionProcessingResult processingResult = MissionHelper.ProcessingMissionDialog(@this.MissionId, false, @this.Dialogs, RewardItemCheck, TIME);
#endif
            tries++;

            if (debugInfoEnabled)
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: ProcessingDialog result is '{processingResult}");
            switch (processingResult)
            {
                case MissionProcessingResult.MissionAccepted:
                    tries = int.MaxValue;
                    if (@this.CloseContactDialog)
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
                    if (@this.SkipOnFail)
                    {
                        ETLogger.WriteLine($"{currentMethodName}: Mission not available...", true);
                        return ActionResult.Skip;
                    }
                    break;
                case MissionProcessingResult.MissionRequiredRewardMissing:
                    ETLogger.WriteLine($"{currentMethodName}: Required mission reward not found...", true);
                    if (@this.CloseContactDialog)
                    {
                        GameHelper.CloseAllFrames();
                        Thread.Sleep(1000);
                    }
                    else QuesterAssistantAccessors.Classes.Monitoring.Frames.Sleep(2000);

                    if (@this.TargetAction != Guid.Empty)
                    {
                        var targetAction = Astral.Quester.API.CurrentProfile.GetActionByID(@this.TargetAction);
                        if (targetAction == null)
                        {
                            //ETLogger.WriteLine($"{currentMethodName}: {@this.GetPropertyDisplayName(o => o.TargetAction)}: {nameof(@this.TargetAction)} not found !", true);
                            ETLogger.WriteLine($"{currentMethodName}: Action[{@this.TargetAction}] not found !", true);
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
            ActionResult result = (@this.SkipOnFail && tries < 3)
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
                if (string.IsNullOrEmpty(label))
                    label = $"{@this.GetType().Name}: [{@this.MissionId}]";
                return label;
            }
        }

        public bool InternalConditions
        {
            get
            {
                bool isGiverAccessible = @this.Giver.IsAccessible;

                bool haveMission = MissionHelper.HaveMission(@this.MissionId, out _);
                bool result = isGiverAccessible && !haveMission;
                if (ExtendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug,
                        $"{_idStr}.{nameof(InternalConditions)}: GiverAccessible({isGiverAccessible}) AND Not(HaveMission({haveMission})) => {result}");
                return result;
            }
        }

        public ActionValidity InternalValidity
        {
            get
            {
                if (!@this.Giver.IsValid) 
                    return new ActionValidity("Invalid Giver.");
                if (string.IsNullOrEmpty(@this.MissionId))
                    return new ActionValidity("Invalid mission id.");
                return new ActionValidity();
            }
        }

        public bool UseHotSpots => false;

        public Vector3 InternalDestination
        {
            get
            {
                var giver = @this.Giver;
                if (giver.IsAccessible
                    && giver.Distance >= @this.InteractDistance)
                    return giver.Position.Clone();
                return Vector3.Empty;
            }
        }

        public void InternalReset()
        {
            tries = 0;
            giverContactInfo = null;
            label = string.Empty;
        }

        public void GatherInfos()
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
                    var interactDistance = @this.InteractDistance;
                    if (!entity.ApproachMissionGiver(interactDistance, @this.InteractZDifference)) return;

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
                                            @this.RequiredRewardItem = rewardItem.ItemId;
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
                            @this.Dialogs.Add(aDialogKey);
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
                    @this.MissionId = missionId;
                    @this.Giver = missionGiver;

                    label = string.Empty;
                }
            }
        }

        public void OnMapDraw(GraphicsNW graphics)
        {
            var giver = @this.Giver;
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
                var logConf = global::EntityTools.EntityTools.Config.Logger;
                return logConf.QuesterActions.DebugPickUpMissionExt && logConf.Active;
            }
        }

        private Predicate<Item> RewardItemCheck
        {
            get
            {
                if (_rewardItemCheck is null)
                {
                    var requiredRewardItem = @this.RequiredRewardItem;
                    if (!string.IsNullOrEmpty(requiredRewardItem))
                    {
                        var strPred = @this.RequiredRewardItem.GetComparer();
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
            var check = @this.IgnoreCombatCondition;
            if (check != null)
                return check.IsValid;
            return true;
        }
        #endregion
    }
}