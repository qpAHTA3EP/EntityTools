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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using static Astral.Quester.Classes.Action;

namespace EntityCore.Quester.Action
{
    public class PickUpMissionEngine : IQuesterActionEngine
    {
        private PickUpMissionExt @this;

        #region данные ядра
        private const int TIME = 300_000;

        private ContactInfo giverContactInfo;
        private int tries;
#if false
        private Predicate<string> isRewardItem; 
#else
        private Predicate<Item> isRewardItem;
#endif
        private string label = string.Empty;
        private string _idStr = string.Empty;
        #endregion

        public PickUpMissionEngine(PickUpMissionExt pum) 
        {
#if false
            @this = pum;
            @this.Engine = this;
            @this.PropertyChanged += PropertyChanged;
            isRewardItem = initializer_IsRewardIte;

            actionIDstr = string.Concat(@this.GetType().Name, '[', @this.ActionID, ']'); 
#else
            InternalRebase(pum);
#endif

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
                if (InternalRebase(pum))
                {
                    ETLogger.WriteLine(LogType.Debug, $"{_idStr} reinitialized");
                    return true;
                }
                ETLogger.WriteLine(LogType.Debug, $"{_idStr} rebase failed");
                return false;
            }
#if false
            else ETLogger.WriteLine(LogType.Debug, $"Rebase failed. '{action}' has type '{action.GetType().Name}' which not equals to '{nameof(PickUpMissionExt)}'");
            return false; 
#else
            string debugStr = string.Concat("Rebase failed. ", action.GetType().Name, '[', action.ActionID, "] can't be casted to '" + nameof(PickUpMissionExt) + '\'');
            ETLogger.WriteLine(LogType.Error, debugStr);
            throw new InvalidCastException(debugStr);
#endif

        }

        private bool InternalRebase(PickUpMissionExt pum)
        {
            // Убираем привязку к старой команде
            @this?.Unbind();

            @this = pum;

            isRewardItem = initializer_IsRewardItem;

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
                isRewardItem = initializer_IsRewardItem;
            else if (propertyName == nameof(@this.MissionId)) label = string.Empty;
        }

        public bool NeedToRun
        {
            get
            {
                bool debugInfoEnabled = EntityTools.EntityTools.Config.Logger.QuesterActions.DebugPickUpMissionExt;

                string currentMethodName = debugInfoEnabled
                    ? string.Concat(_idStr, '.', MethodBase.GetCurrentMethod().Name)
                    : string.Empty;

                bool needToRun = false;

                if (debugInfoEnabled)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Begins"));

                float searchDistance = Math.Max(@this._interactDistance + 0.5f,
                                @this.ContactHaveMission == ContactHaveMissionCheckType.Disabled ? 10f : 50f);
                float interactDistance = Math.Max(@this._interactDistance + 0.5f, 5.5f);


                if (@this._giver.Type == MissionGiverType.NPC)
                {
                    var giverPos = @this._giver.Position;
                    var giverDistance = @this._giver.Distance;

                    if (giverDistance < searchDistance
                        && Math.Abs(giverPos.Z - EntityManager.LocalPlayer.Z) <= @this._interactZDifference)
                    {
                        if (debugInfoEnabled)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Distance to the Giver is ", giverDistance.ToString("N1"), ". Result 'true'"));
#if false
                        return true; 
#else
                        needToRun = true;
#endif
                    }  
                    else if (debugInfoEnabled)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Faraway(", giverDistance.ToString("N1"), "). Result 'False'"));
                }
                else if(@this._giver.Type == MissionGiverType.Remote)
                {
                    if (debugInfoEnabled)
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
                    if (debugInfoEnabled)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Disable combat"));
                }

                return needToRun;
            }
        }

        public ActionResult Run()
        {
            bool debugInfoEnabled = EntityTools.EntityTools.Config.Logger.QuesterActions.DebugPickUpMissionExt;

            string currentMethodName = debugInfoEnabled
                    ? string.Concat(_idStr, '.', MethodBase.GetCurrentMethod().Name)
                    : string.Empty;

            if (debugInfoEnabled)
                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Begins try #", tries));

            if (!InternalConditions)
            {
                if (debugInfoEnabled)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": InternalConditions is False => ActionResult = '", ActionResult.Fail, '\''));
                return ActionResult.Fail;
            }

            if (@this._ignoreCombat)
            {
                bool inCombat = EntityManager.LocalPlayer.InCombat;
                if (debugInfoEnabled)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Engage combat ",
                        inCombat ? " => ActionResult = '" + ActionResult.Running + "'" : string.Empty));
                AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(false);
                if (inCombat)
                    return ActionResult.Running;
            }

            if (@this._giver.Type == MissionGiverType.NPC)
            {
                float interactDistance = Math.Max(@this._interactDistance + 0.5f, 5.5f);
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

                        if (debugInfoEnabled)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Set GiverContactInfo to [", giverContactInfo, ']'));
                        break;
                    }

                    if (giverContactInfo is null && @this._giver.Distance < interactDistance)
                    {
                        // Персонаж прибыл на место, однако, нужный NPC отсутствует
                        // невозможно ни принять миссию, ни пропустить команду
                        tries++;

                        if (debugInfoEnabled)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": GiverContact is absent..."));

                        goto Results;
                    }
                }
                else if (debugInfoEnabled)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Keep GiverContactInfo [", giverContactInfo, ']'));

                if (giverContactInfo != null)
                {
                    Entity entity = giverContactInfo?.Entity;
                    if (debugInfoEnabled)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Entity [", entity.InternalName, ", ", entity.CostumeRef.CostumeName, "] match to MissionGiverInfo [", @this._giver, ']'));

                    // Проверяем наличие задания у контакта 
                    if (@this._contactHaveMission != ContactHaveMissionCheckType.Disabled)
                    {
                        if (!MissionHelper.ContactHaveMission(giverContactInfo))
                        {
                            if (debugInfoEnabled)
                                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ContactHaveMission is False => ActionResult = '", ActionResult.Skip, '\''));
                            return ActionResult.Skip;
                        }
                        else if (debugInfoEnabled)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ContactHaveMission is True. Continue..."));
                    }
                    else if (debugInfoEnabled)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Skiped checking the condition 'ContactHaveMission'"));

                    // Перемещаемся к квестодателю (в случае необходимости)
                    if (!entity.ApproachMissionGiver(@this._interactDistance, @this._interactZDifference))
                    {
                        if (debugInfoEnabled)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ApproachMissionGiver failed => ActionResult = '", ActionResult.Running, '\''));
                        return ActionResult.Running;
                    }
                    else if (debugInfoEnabled)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ApproachMissionGiver succeeded"));

                    // Взаимодействуем с квестодателем
                    if (!entity.InteractMissionGiver(@this._interactDistance))
                    {
                        if (debugInfoEnabled)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": InteractMissionGiver failed => ActionResult = '", ActionResult.Running, '\''));
                        return ActionResult.Running;
                    }
                    else if (debugInfoEnabled)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": InteractMissionGiver succeeded"));
                }
            }
            else if (@this._giver.Type == MissionGiverType.Remote)
            {
                if (debugInfoEnabled)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Call 'RemoteContact'"));
                var id = @this._giver.Id;
                var remoteContact = EntityManager.LocalPlayer.Player.InteractInfo.RemoteContacts.FirstOrDefault(ct => ct.ContactDef == id);

                if (remoteContact != null && remoteContact.IsValid)
                {
                    remoteContact.Start();
                    if (debugInfoEnabled)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Call RemoteContact '", remoteContact.ContactDef, "'"));
                }
                else
                {
                    tries++;

                    if (debugInfoEnabled)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": RemoteContact '", id, "' does not found"));

                    goto Results;
                }
            }
            else
            {
                if (debugInfoEnabled)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Invalid Giver settings => ", ActionResult.Skip));

                return ActionResult.Skip;
            }

            // Проводим попытку принять задание
#if false
            MissionProcessingResult processingResult = ProccessingDialog(); 
#else
            MissionProcessingResult processingResult = MissionHelper.ProccessingMissionDialog(@this._missionId, false, @this._dialogs, isRewardItem, TIME);
#endif
            tries++;

            if (debugInfoEnabled)
                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ProccessingDialog result is '", processingResult, '\''));
            switch (processingResult)
            {
                case MissionProcessingResult.MissionAccepted:
                    tries = int.MaxValue;
                    if (@this._closeContactDialog)
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
                    if (@this._skipOnFail)
                    {
                        ETLogger.WriteLine(string.Concat(currentMethodName, ": Mission not available..."), true);
                        return ActionResult.Skip;
                    }
                    break;
                case MissionProcessingResult.MissionRequiredRewardNotFound:
                    ETLogger.WriteLine(string.Concat(currentMethodName, ": Required mission reward not found..."), true);
                    if (@this._closeContactDialog)
                    {
                        GameHelper.CloseAllFrames();
                        Thread.Sleep(2000);
                    }
                    else QuesterAssistantAccessors.Classes.Monitoring.Frames.Sleep(2000);
                    return ActionResult.Skip;
                case MissionProcessingResult.Error:
                    GameHelper.CloseAllFrames();
                    Thread.Sleep(1000);
                    break;
            }

            Results:
            ActionResult result = (@this._skipOnFail && tries < 3)
                ? ActionResult.Running
                : ActionResult.Fail;

            if (debugInfoEnabled)
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
                bool isHavingMissionOrCompleted = MissionHelper.CheckHavingMissionOrCompleted(@this._missionId);
                bool result = isGiverAccessible && !isHavingMissionOrCompleted;
                if (EntityTools.EntityTools.Config.Logger.QuesterActions.DebugPickUpMissionExt)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(_idStr, '.', nameof(InternalConditions), ": GiverAccessible(", isGiverAccessible, ") AND Not(HavingMissionOrCompleted(", isHavingMissionOrCompleted, ")) => ", result));
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
        }

        public void GatherInfos()
        {

            MissionGiverInfo missionGiver = null;
            string mission_id = string.Empty;
            MissionProcessingResult missionResult = MissionProcessingResult.Error;
            if (MissionGiverInfoEditor.SetInfos(ref missionGiver))
            {
                if (missionGiver.Type == MissionGiverType.NPC)
                {
                    Entity entity = Interact.GetBetterEntityToInteract();
                    if (!missionGiver.IsMatching(entity)) return;

                    if (!entity.ApproachMissionGiver(@this._interactDistance, @this._interactZDifference)) return;

                    if (!entity.InteractMissionGiver(@this._interactDistance)) return;
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

                            if (startInd >= 0 && startInd + "MissionOffer.".Length < aDialogKey.Length)
                            {
                                mission_id = aDialogKey.Substring(startInd + "MissionOffer.".Length, lastInd - startInd - "MissionOffer.".Length);

                                if (!string.IsNullOrEmpty(mission_id))
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
                                            if (MissionHelper.HaveMission(mission_id, out _))
                                            {
                                                missionResult = MissionProcessingResult.MissionAccepted;
                                                break;
                                            }
                                        }
                                        while (!timer.IsTimedOut);
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
                        mission_id = GetMissionId.Show(true, mission_id);
                }
                
                if (!string.IsNullOrEmpty(mission_id)
                    && missionGiver.IsValid)
                {
                    @this._missionId = mission_id;
                    @this._giver = missionGiver;

                    label = string.Empty;
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
#if false
        /// <summary>
        /// Обработка диалога
        /// </summary>
        private MissionProcessingResult ProccessingDialog()
        {
            bool debugInfoEnabled = EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo;

            string currentMethodName = debugInfoEnabled
                    ? string.Concat(actionIDstr, '.', MethodBase.GetCurrentMethod().Name)
                    : string.Empty;

            if (debugInfoEnabled)
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

                            if (debugInfoEnabled)
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
                        if (debugInfoEnabled)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ":" +
                                "\n\t\tScreenType = ", screenType,
                                "\n\t\tCheckRequeredRewardItem = False" +
                                "\n\t\tSelect 'ViewOfferedMission.Back' = True" +
                                "\n\t" + nameof(ProccessingDialog) + " => ", result));

                        return result;
                    }
#endif
                    result = MissionProcessingResult.Error;
                    if (debugInfoEnabled)
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

                                    if (debugInfoEnabled)
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
                                    if (debugInfoEnabled)
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
                                if (debugInfoEnabled)
                                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ":" +
                                        "\n\t\tScreenType = ", screenType,
                                        "\n\t\tSelect '", @this._missionId, "' = True" +
                                        "\n\t\tCheckRequeredRewardItem = False" +
                                        "\n\t" + nameof(ProccessingDialog) + " => ", result));
                                return result;
                            }
                        } 
#else
                        if (debugInfoEnabled)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Select the option '", @this._missionId, "' on '", screenType, "' screen. Continue..."));
#endif
                    }
                    else
                    {
                        result = MissionProcessingResult.MissionNotFound;
                        if (debugInfoEnabled)
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
                        if (debugInfoEnabled)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Selection of '", lastOption.Key, "' on ScreenType(", screenType, ") succedded. Continue..."));
                    }
                    else
                    {
                        if (debugInfoEnabled)
                        {
                            result = MissionProcessingResult.Error;
                            if (debugInfoEnabled)
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
                    if (debugInfoEnabled)
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
            if (debugInfoEnabled)
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
        private bool initializer_IsRewardItem(Item item)
        {
            if (string.IsNullOrEmpty(@this._requiredRewardItem))
            {
                isRewardItem = internal_IsRewardItem_True;
                return true;
            }

            var strPred = StringToPatternComparer.GetComparer(@this._requiredRewardItem);
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