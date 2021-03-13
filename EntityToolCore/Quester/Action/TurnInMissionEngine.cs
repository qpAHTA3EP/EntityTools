#define MissionGiverInfo

using Astral.Logic.Classes.Map;
using Astral.Logic.NW;
using Astral.Quester.Forms;
using EntityCore.Enums;
using EntityCore.Tools;
using EntityCore.Tools.Missions;
using EntityTools;
using EntityTools.Core.Interfaces;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Patches.Mapper;
using EntityTools.Quester.Actions;
using EntityTools.Reflection;
using EntityTools.Tools;
using EntityTools.Tools.Missions;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using static Astral.Quester.Classes.Action;

namespace EntityCore.Quester.Action
{
    //TODO: Исправить работу PickUpMissionExt TurnInMissionExt на Stokely Silverstone в Кер-Кенниге (М3)
    public class TurnInMissionEngine : IQuesterActionEngine
    {
        private TurnInMissionExt @this;

        #region данные ядра
        private const int TIME = 5_000;

        private ContactInfo giverContactInfo;
        private int tries;
        private Astral.Classes.Timeout failTo = null;
#if false
        private Predicate<string> isRewardItem; 
#else
        private Predicate<Item> isRewardItem;
#endif
        private string label = string.Empty;
        private string actionIDstr = string.Empty;
        #endregion

        public TurnInMissionEngine(TurnInMissionExt tim) 
        {
#if false
            @this = tim;
            @this.Engine = this;
            @this.PropertyChanged += PropertyChanged;
            isRewardItem = internal_IsRewardItem_Initializer;

            actionIDstr = string.Concat(@this.GetType().Name, '[', @this.ActionID, ']');
#else
            InternalRebase(tim);
#endif
            ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, "initialized: ", ActionLabel));
        }

        public bool Rebase(Astral.Quester.Classes.Action action)
        {
            if (action is null)
                return false;
            if (ReferenceEquals(action, @this))
                return true;
            if (action is TurnInMissionExt tim)
            {
                if (InternalRebase(tim))
                {
                    ETLogger.WriteLine(LogType.Debug, $"{actionIDstr} reinitialized");
                    return true;
                }
                ETLogger.WriteLine(LogType.Debug, $"{actionIDstr} rebase failed");
                return false;
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
            if (@this != null)
            {
                @this.PropertyChanged -= PropertyChanged;
                @this.Engine = new EntityTools.Core.Proxies.QuesterActionProxy(@this);
            }

            @this = tim;
            @this.PropertyChanged += PropertyChanged;

            isRewardItem = initialize_IsRewardItem;

            actionIDstr = string.Concat(@this.GetType().Name, '[', @this.ActionID, ']');

            @this.Engine = this;

            return true;
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!ReferenceEquals(sender, @this)) return;
            if (e.PropertyName == nameof(@this.RequiredRewardItem))
                isRewardItem = initialize_IsRewardItem;
            else if (e.PropertyName == nameof(@this.MissionId)) label = string.Empty;
        }

        public bool NeedToRun
        {
            get
            {
                bool extendedActionDebugInfo = EntityTools.EntityTools.Config.Logger.QuesterActions.DebugTurnInMissionExt;

                string currentMethodName = extendedActionDebugInfo
                    ? string.Concat(actionIDstr, '.', MethodBase.GetCurrentMethod().Name)
                    : string.Empty;

                bool needToRun = false;

                if (extendedActionDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Begins"));

                float searchDistance = Math.Max(@this._interactDistance + 0.5f, 10f);
                float interactDistance = Math.Max(@this._interactDistance + 0.5f, 5.5f);


                if (@this._giver.Type == MissionGiverType.NPC)
                {
                    var giverPos = @this._giver.Position;
                    var giverDistance = @this._giver.Distance;

                    if (giverDistance < searchDistance
                        && Math.Abs(giverPos.Z - EntityManager.LocalPlayer.Z) <= @this._interactZDifference)
                    {
                        if (extendedActionDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Distance to the Giver is ", giverDistance.ToString("N1"), ". Result 'true'"));
#if false
                        return true; 
#else
                        needToRun = true;
#endif
                    }  
                    else if (extendedActionDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Faraway(", giverDistance.ToString("N1"), "). Result 'False'"));
                }
                else if(@this._giver.Type == MissionGiverType.Remote)
                {
                    if (extendedActionDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Giver is remote. Result 'True'"));
#if false
                    return true; 
#else
                    needToRun = true;
#endif
                }

                if (@this._ignoreCombat && !needToRun)
                {
#if false
                    Astral.Quester.API.IgnoreCombat = true; 
#else
                    AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(true);
#endif
                    if (extendedActionDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Disable combat"));
                }

                return needToRun;
            }
        }

        public ActionResult Run()
        {
            bool extendedActionDebugInfo = EntityTools.EntityTools.Config.Logger.QuesterActions.DebugTurnInMissionExt;

            string currentMethodName = extendedActionDebugInfo
                    ? string.Concat(actionIDstr, '.', MethodBase.GetCurrentMethod().Name)
                    : string.Empty;

            if (extendedActionDebugInfo)
                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Begins try #", tries));

            if (!InternalConditions)
            {
                if (extendedActionDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": InternalConditions is False => ActionResult = '", ActionResult.Fail, '\''));
                return ActionResult.Fail;
            }
            if (failTo is null)
                failTo = new Astral.Classes.Timeout(30000);

            if (@this._ignoreCombat)
            {
                bool inCombat = EntityManager.LocalPlayer.InCombat;
                if (extendedActionDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Engage combat ",
                        inCombat ? " => ActionResult = '" + ActionResult.Running + "'" : string.Empty));
                AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(false);
                if (inCombat)
                    return ActionResult.Running;
            }

            if (@this._giver.Type == MissionGiverType.NPC)
            {
                float interactDistance = Math.Max(@this._interactDistance, 5.5f);
                var giverPos = @this._giver.Position;
                var giverDistance = @this._giver.Distance;

                // Производит поиск NPC-Квестодателя
                if (giverContactInfo is null || !@this._giver.IsMatching(giverContactInfo.Entity))
                {
                    giverContactInfo = null;
                    foreach (ContactInfo contactInfo in EntityManager.LocalPlayer.Player.InteractInfo.NearbyContacts)
                    {
                        var contactEntity = contactInfo.Entity;
                        var contactLocation = contactEntity.Location;


                        if (Math.Abs(giverPos.Z - contactLocation.Z) >= @this._interactZDifference
                            || !@this._giver.IsMatching(contactEntity))
                                continue;

                        giverContactInfo = contactInfo;

                        if (extendedActionDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Set GiverContactInfo to [", giverContactInfo, ']'));
                        break;
                    }

                    if (giverContactInfo is null && @this._giver.Distance < interactDistance)
                    {
                        // Персонаж прибыл на место, однако, нужный NPC отсутствует
                        // невозможно ни принять миссию, ни пропустить команду
                        tries++;

                        if (extendedActionDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": GiverContact is absent..."));

                        goto Results;
                    }
                }
                else if (extendedActionDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Keep GiverContactInfo [", giverContactInfo, ']'));

                if (giverContactInfo != null)
                {
                    Entity entity = giverContactInfo?.Entity;
                    if (extendedActionDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Entity [", entity.InternalName, ", ", entity.CostumeRef.CostumeName, "] match to MissionGiverInfo [", @this._giver, ']'));

                    // Перемещаемся к квестодателю (в случае необходимости)
                    if (!entity.ApproachMissionGiver(interactDistance, @this._interactZDifference))
                    {
                        if (extendedActionDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ApproachMissionGiver failed => ActionResult = '", ActionResult.Running, '\''));
                        return ActionResult.Running;
                    }
                    else if (extendedActionDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ApproachMissionGiver succeeded"));

                    // Взаимодействуем с квестодателем
                    if (!entity.InteractMissionGiver(interactDistance))
                    {
                        if (extendedActionDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": InteractMissionGiver failed => ActionResult = '", ActionResult.Running, '\''));
                        return ActionResult.Running;
                    }
                    else if (extendedActionDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": InteractMissionGiver succeeded"));
                }
            }
            else if (@this._giver.Type == MissionGiverType.Remote)
            {
                if (extendedActionDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Call 'RemoteContact'"));
                var id = @this._giver.Id;
                var remoteContact = EntityManager.LocalPlayer.Player.InteractInfo.RemoteContacts.FirstOrDefault(ct => ct.ContactDef == id);

                if (remoteContact != null && remoteContact.IsValid)
                {
                    remoteContact.Start();
                    if (extendedActionDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Call RemoteContact '", remoteContact.ContactDef, "'"));
                }
                else
                {
                    tries++;

                    if (extendedActionDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": RemoteContact '", id, "' does not found"));

                    goto Results;
                }
            }
            else
            {
                if (extendedActionDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Invalid Giver settings => ", ActionResult.Skip));

                return ActionResult.Skip;
            }

            // Проводим попытку принять задание
#if false
            MissionProcessingResult processingResult = ProccessingDialog(); 
#else
            MissionProcessingResult processingResult = MissionHelper.ProccessingMissionDialog(@this._missionId, true, @this._dialogs, isRewardItem, TIME);
#endif
            tries++;

            if (extendedActionDebugInfo)
                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ProccessingDialog result is '", processingResult, '\''));
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
                    ETLogger.WriteLine(string.Concat(currentMethodName, ": Required mission reward not found..."), true);
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

            if (extendedActionDebugInfo)
                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, " => ", result));

            return result;
        }

        public string ActionLabel
        {
            get
            {
                if (string.IsNullOrEmpty(label))
                    label = $"{@this.GetType().Name}: [{@this._missionId}]";
                return label;
            }
        }

        public bool InternalConditions
        {
            get
            {
                bool isGiverAccessible = @this._giver.IsAccessible;
                uint bagsFreeSlots = EntityManager.LocalPlayer.BagsFreeSlots;
                bool isMissionSucceded = MissionHelper.HaveMission(@this._missionId, out Mission mission) && mission.State == MissionState.Succeeded;
                bool result = isGiverAccessible && bagsFreeSlots > 0 && isMissionSucceded;
                if (EntityTools.EntityTools.Config.Logger.QuesterActions.DebugTurnInMissionExt)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', nameof(InternalConditions), ": GiverAccessible(", isGiverAccessible, ") AND BagsFreeSlots(", bagsFreeSlots, ") MissionSucceded(", isMissionSucceded, ")) => ", result));
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
            label = string.Empty;
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
                if (!missionGiver.IsMatching(entity)) return;

                if (!entity.ApproachMissionGiver(@this._interactDistance, @this._interactZDifference)) return;

                if (!entity.InteractMissionGiver(@this._interactDistance)) return;

                Interact.WaitForInteraction();

                var interactInfo = EntityManager.LocalPlayer.Player.InteractInfo;
                var contactDialog = interactInfo.ContactDialog;

                if (contactDialog.Options.Count > 0)
                {
#if false
                    string aDialogKey = GetAnId.GetADialogKey(); 
#elif false
                    MissionHelper.GetADialogKey(out string aDialogKey);
#else
                    string aDialogKey = MissionHelper.GetADialogOption(out ContactDialogOption contactDialogOption)
                        ? contactDialogOption.Key : string.Empty;
#endif

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

                            if (startInd >= 0 && startInd + "CompleteMission.".Length < aDialogKey.Length)
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

#if false                               // Проверка через сдачу миссии
                                        // Сдаем миссию
                                        interactInfo.ContactDialog.SelectOptionByKey("ViewCompleteMission.Continue");

                                        // Проверяем корректность определения "идентификатора миссии"
                                        timer.Reset();
                                        do
                                        {
                                            Thread.Sleep(250);
                                            if (MissionHelper.CompletedMission(missionId, out _))
                                            {
                                                missionIdDetected = true;
                                                break;
                                            }
                                        }
                                        while (!timer.IsTimedOut); 
#else                                   // Проверка через поиск миссии в журнале
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
#endif
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Выбранный пункт диалога не содержит идентификатора миссии
                            // добавляем его в Dialogs
                            @this._dialogs.Add(aDialogKey);
                            interactInfo.ContactDialog.SelectOptionByKey(aDialogKey);
                            Thread.Sleep(1000);
#if true
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
                    if (!missionIdDetected)
                        missionId = GetMissionId.Show(true, missionId);
                }

                if (!string.IsNullOrEmpty(missionId)
                    && missionGiver.IsValid)
                {
                    @this._missionId = missionId;
                    @this._giver = missionGiver;

                    label = string.Empty;
                }
            }
        }

        public void OnMapDraw(GraphicsNW graph)
        {
            if (@this._giver != null && @this._giver.Position.IsValid)
            {
                if (graph is MapperGraphics graphicsExt)
                    graphicsExt.FillCircleCentered(Brushes.Beige, @this._giver.Position, 10);
                else graph.drawFillEllipse(@this._giver.Position, MapperHelper.Size_10x10, Brushes.Beige);
            }
        }

        #region Вспомогательные методы
#if false
        /// <summary>
        /// Обработка диалога
        /// </summary>
        private MissionProcessingResult ProccessingDialog()
        {
            bool extendedActionDebugInfo = EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo;

            string currentMethodName = extendedActionDebugInfo
                    ? string.Concat(actionIDstr, '.', MethodBase.GetCurrentMethod().Name)
                    : string.Empty;

            if (extendedActionDebugInfo)
                ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', currentMethodName, ": Begin"));

            Interact.WaitForInteraction();

            var player = EntityManager.LocalPlayer.Player;
            var contactDialog = player.InteractInfo.ContactDialog;
            var screenType = contactDialog.ScreenType;

            // обрабатываем предварительные пункты диалога, если задано
            if (@this._dialogs.Count > 0)
                MissionHelper.ProcessingOptionalDialogs(@this._dialogs);

            var timeout = new Astral.Classes.Timeout(TIME);
            var missionDialogKey = "OptionsList.MissionOffer." + @this._missionId;
            MissionProcessingResult result;
            while (contactDialog.IsValid && !timeout.IsTimedOut)
            {
                // Диалоговое окно открыто
                // Проверяем статус экрана 
                if (screenType == ScreenType.MissionOffer)
                {
                    // Открыт экран принятия миссии
                    // На экране принятия миссии узнать из АПИ, к какой миссии он относится, - нельзя
                    // Проверяем наличие обязательных наград в окне миссии
                    if (MissionHelper.CheckRequeredRewardItem(isRewardItem))
                    {
                        // Принимаем миссию
                        if (contactDialog.CheckDialogOptionAndSelect(d => d.Key.Equals("ViewOfferedMission.Accept"),
                            () => contactDialog.ScreenType == ScreenType.MissionOffer))
                        {
                            result = MissionProcessingResult.MissionOfferAccepted;
                            var timer = new Astral.Classes.Timeout(TIME);
                            while (!timer.IsTimedOut)
                            {
                                if (MissionHelper.HaveMission(@this._missionId, out _))
                                {
                                    result = MissionProcessingResult.MissionAccepted;
                                    break;
                                }
                                Thread.Sleep(250);
                            }

                            if (extendedActionDebugInfo)
                                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ":" +
                                    "\n\t\tScreenType = ", screenType,
                                    "\n\t\tCheckRequeredRewardItem = True" +
                                    "\n\t\tSelect 'ViewOfferedMission.Accept' = True" +
                                    "\n\t" + nameof(ProccessingDialog) + " => ", result));
                            return result;
                        }
                    }

                    // В наградах отсутствуют обязательные итемы - отказываемся от миссии.
#if true
                    else if (contactDialog.CheckDialogOptionAndSelect(d => d.Key.Equals("ViewOfferedMission.Back"),
                        () => contactDialog.ScreenType == ScreenType.MissionOffer))
                    {
                        result = MissionProcessingResult.MissionRequiredRewardNotFound;
                        if (extendedActionDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ":" +
                                "\n\t\tScreenType = ", screenType,
                                "\n\t\tCheckRequeredRewardItem = False" +
                                "\n\t\tSelect 'ViewOfferedMission.Back' = True" +
                                "\n\t" + nameof(ProccessingDialog) + " => ", result));

                        return result;
                    }
#endif
                    result = MissionProcessingResult.Error;
                    if (extendedActionDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ":" +
                            "\n\t\tScreenType = ", screenType,
                            "\n\t\tCheckRequeredRewardItem = False" +
                            "\n\t\tSelect 'ViewOfferedMission.Back' = False" +
                            "\n\t" + nameof(ProccessingDialog) + " => ", result));

                    return result;
                }
                else if (screenType == ScreenType.List)
                {
                    // Открыт экран списка пунктов диалога
                    // ContactDialog.ScreenType = List
                    // ContactDialog.Options[].Key:
                    //      OptionsList.MissionOffer.Идентификато_миссии* - Пункт диалога, инициирующий принятие миссии
                    //      OptionsList.Exit - Закрыть диалоговое окно
                    // Принимаем задачу в несколько этапов
                    // 1. Активируем пункт диалога, содержащий идентификатору задачи
                    // 2. Проверяем наличие награды
                    // 3. Принимаем задачу путем активации соответствующего пункта диалога
                    if (contactDialog.CheckDialogOptionAndSelect(d => d.Key.StartsWith(missionDialogKey), () => contactDialog.ScreenType == screenType))
                    {
#if false               // 2й и 3й этапы производятся на экране ScreenType.MissionOffer, обработка которого будет произведена на следующем цикле
                        contactDialog = EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog;
                        screenType = contactDialog.ScreenType;

                        if (screenType == ScreenType.MissionOffer)
                        {
                            // 2. Проверяем 
                            if (CheckRequeredRewardItem())
                            {
                                // 3. Принимаем задачу путем активации соответствующего пункта диалога
                                bool haveMission = false;
                                if (contactDialog.CheckDialogOptionAndSelect(d => d.Key.Equals("ViewOfferedMission.Accept"),
                                                        () => !(haveMission = MissionHelper.HaveMission(@this._missionId, out _))))
                                {
                                    result = haveMission ? MissionPickUpResult.MissionAccepted 
                                                         : MissionPickUpResult.Error;

                                    if (extendedActionDebugInfo)
                                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ":" +
                                            "\n\t\tScreenType = ", screenType,
                                            "\n\t\tSelect '", @this._missionId, "' = True" +
                                            "\n\t\tCheckRequeredRewardItem = True" +
                                            "\n\t\tSelect 'ViewOfferedMission.Accept' = True" +
                                            "\n\t\tHaveMission = ", haveMission,
                                            "\n\t" + nameof(ProccessingDialog) + " => ", result));
                                    return result;
                                }
                                else
                                {
                                    result = MissionPickUpResult.Error;
                                    if (extendedActionDebugInfo)
                                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ":" +
                                            "\n\t\tScreenType = ", screenType,
                                            "\n\t\tSelect '", @this._missionId, "' = True" +
                                            "\n\t\tCheckRequeredRewardItem = True" +
                                            "\n\t\tSelect 'ViewOfferedMission.Accept' = False" +
                                            "\n\t" + nameof(ProccessingDialog) + " => ", result));
                                    return result;
                                }
                            }
                            else
                            {
                                result = MissionPickUpResult.MissionRequiredRewardNotFound;
                                if (extendedActionDebugInfo)
                                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ":" +
                                        "\n\t\tScreenType = ", screenType,
                                        "\n\t\tSelect '", @this._missionId, "' = True" +
                                        "\n\t\tCheckRequeredRewardItem = False" +
                                        "\n\t" + nameof(ProccessingDialog) + " => ", result));
                                return result;
                            }
                        } 
#else
                        if (extendedActionDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Select the option '", @this._missionId, "' on '", screenType, "' screen. Continue..."));
#endif
                    }
                    else
                    {
                        result = MissionProcessingResult.MissionNotFound;
                        if (extendedActionDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ":" +
                                "\n\t\tScreenType = ", screenType,
                                "\n\t\tSelect '", @this._missionId, "' = False" +
                                "\n\t" + nameof(ProccessingDialog) + " => ", result));
                        return result;
                    }
                }
                else if (screenType == ScreenType.Buttons)
                {
                    // Как правило в разговоре (ContactDialog.ScreenType = Buttons)
                    // последний пункт в перечне ответов (ContactDialog.Options[])
                    // соответствует возврату в предыдущее меню
                    // Пункты диалога имеют идентификтор вида:
                    // 'SpecialDialog.action_Х'
                    // где Х - порядковый номер соответствующего пункта, начиная с нуля
                    var lastOption = contactDialog.Options.Last();
                    bool isAccessible = lastOption != null && !lastOption.CannotChoose;
#if false
                    bool isSpecialDialog = isAccessible ? lastOption.Key.StartsWith("SpecialDialog.action_") : false;
                    if (isSpecialDialog) 
#else
                    if (isAccessible)
#endif
                    {
                        bool selectResult = lastOption.Select();
                        if (extendedActionDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Selection of '", lastOption.Key, "' on ScreenType(", screenType, ") succedded. Continue..."));
                    }
                    else
                    {
                        if (extendedActionDebugInfo)
                        {
                            result = MissionProcessingResult.Error;
                            if (extendedActionDebugInfo)
                                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ":" +
                                    "\n\t\tScreenType = ", screenType,
                                    "\n\t\tLast item in ContactDialog.Options inaccessible" +
                                    "\n\t" + nameof(ProccessingDialog) + " => ", result));
                            return result;
                        }
                    }
                }
                else
                {
                    result = MissionProcessingResult.Error;
                    if (extendedActionDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ":" +
                            "\n\t\tScreenType = ", screenType,
                            "\n\t" + nameof(ProccessingDialog) + " => ", result));
                    return result;
                }

                Thread.Sleep(1000);
                contactDialog = player.InteractInfo.ContactDialog;
                screenType = contactDialog.ScreenType;
            }


            result = MissionProcessingResult.Error;
            if (extendedActionDebugInfo)
                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Time is out => ", result));
            return result;
        } 
#endif
#if false
        private bool internal_IsRewardItem_Initializer(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                isRewardItem = (string s) => true;
                return true;
            }

            var pred = StringToPatternComparer.Get(str);
            if (pred is null)
                return true;

            isRewardItem = pred;
            return isRewardItem(str);
        } 
#else
        private bool initialize_IsRewardItem(Item item)
        {
            if (string.IsNullOrEmpty(@this._requiredRewardItem))
            {
                isRewardItem = internal_IsRewardItem_True;
                return true;
            }

            var strPred = StringToPatternComparer.Get(@this._requiredRewardItem);
            if (strPred is null)
            {
                isRewardItem = internal_IsRewardItem_True;
                return true;
            }

            isRewardItem = (Item itm) => strPred(itm.ItemDef.InternalName);
            return isRewardItem(item);
        }
        private bool internal_IsRewardItem_True(Item item) => true;
#endif
        #endregion
    }
}