#define MissionGiverInfo

using Astral.Logic.Classes.Map;
using Astral.Logic.NW;
using Astral.Quester.Forms;
using EntityCore.Enums;
using EntityCore.Forms;
using EntityTools;
using EntityTools.Core.Interfaces;
using EntityTools.Quester.Actions;
using EntityTools.Tools;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Astral.Quester.Classes;
using DevExpress.XtraEditors;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Patches.Mapper;
using EntityTools.Tools.Missions;
using EntityTools.Tools.Navigation;
using static Astral.Quester.Classes.Action;
using AstralAccessors = EntityTools.Reflection.AstralAccessors;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Reflection;

namespace EntityCore.Enums
{
    internal enum MissionInteractResult
    {
        /// <summary>
        /// Ошибка во время попытки взаимодействия с НПС при принятии миссии
        /// </summary>
        Error,
        /// <summary>
        /// НПС начинает диалог с автоматического предложения
        /// </summary>
        MissionOffer,
        /// <summary>
        /// Целвая миссия принята
        /// </summary>
        Succeed,
        /// <summary>
        /// Целевая миссия не найдена
        /// </summary>
        MissionNotFound
    }

    internal enum MissionPickUpResult
    {
        /// <summary>
        /// Ошибка во время попытки взаимодействия с НПС при принятии миссии
        /// </summary>
        Error,
        /// <summary>
        /// Целевая миссия не найдена
        /// </summary>
        MissionNotFound,
        /// <summary>
        /// Награда за целевую миссию не содержит обязательного предмета
        /// </summary>
        MissionRequiredRewardNotFound,
        /// <summary>
        /// Целвая миссия принята
        /// </summary>
        MissionAccepted,
        /// <summary>
        /// Принята миссия, предложенная НПС
        /// </summary>
        MissionOfferAccepted,
#if false
        /// <summary>
        /// Награда за предложенную миссию не содержит обязательного предмета
        /// </summary>
        OfferMissionRequiredRewardNotFound, 

        /// <summary>
        /// Отказ от принятия предложенной миссии
        /// </summary>
        MissionOfferAborted
#endif
    }
}

namespace EntityCore.Quester.Action
{
    public class PickUpMissionEngine : IQuesterActionEngine
    {
        private PickUpMissionExt @this;

        private const int TIME = 300_000;

        #region данные ядра
        private ContactInfo giverContactInfo;
        private int tries;
#if false
        private SimplePatternPos patternPos = SimplePatternPos.NoChecking;
        private string rewardItemPattern = string.Empty; 
#else
        private Predicate<string> isRewardItem;
#endif
        private string label = string.Empty;
        private string actionIDstr = string.Empty;
        #endregion

        public PickUpMissionEngine(PickUpMissionExt pum) 
        {
            @this = pum;
            @this.Engine = this;
            @this.PropertyChanged += PropertyChanged;
            isRewardItem = internal_IsRewardItem_Initializer;

            actionIDstr = string.Concat(@this.GetType().Name, '[', @this.ActionID, ']');

            ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, "initialized: ", ActionLabel));
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!ReferenceEquals(sender, @this)) return;
            if (e.PropertyName == "RequiredRewardItem")
                isRewardItem = internal_IsRewardItem_Initializer;
            else if (e.PropertyName == "MissionId") label = string.Empty;
        }

        public bool NeedToRun
        {
            get
            {
                string currentMethodName = EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo
                    ? string.Concat(actionIDstr, '.', MethodBase.GetCurrentMethod().Name)
                    : string.Empty;

                if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Begins"));

                float searchDistance = Math.Max(@this._interactDistance,
                                @this.ContactHaveMission == ContactHaveMissionCheckType.Disabled ? 20f : 50f);
                float interactDistance = Math.Max(@this._interactDistance, 5f);

                var giverPos = @this._giver.Position;

                var giverDistance = giverPos.Distance3DFromPlayer;

                if (giverDistance < searchDistance)
#if false           // проверка карты и региона встроено в InternalCondition, т.е. проверяется ДО вызова NeedToRun
                && @this._giver.MapName == EntityManager.LocalPlayer.MapState.MapName
                && @this._giver.RegionName == EntityManager.LocalPlayer.RegionInternalName) 
#endif
                {
                    if (giverContactInfo is null || !@this._giver.IsMatching(giverContactInfo.Entity))
                    {
                        foreach (ContactInfo contactInfo in EntityManager.LocalPlayer.Player.InteractInfo.NearbyContacts)
                        {
                            var contactEntity = contactInfo.Entity;
                            var contactLocation = contactEntity.Location;

#if false
                        if (!contactInfo.Entity.CanInteract
                            || giverPos.Distance3D(location) >= interactDistance
                            || Math.Abs(giverPos.Z - location.Z) >= @this._interactZDifference
                            || !@this._giver.IsMatching(contactEntity)) 
#else                   // Проверка giverPos.Distance3D(location) > 1 встроена в _giver.IsMatching
                            if (Math.Abs(giverPos.Z - contactLocation.Z) >= @this._interactZDifference
                                || !@this._giver.IsMatching(contactEntity))
#endif
                                continue;

                            giverContactInfo = contactInfo;
                            if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                            {
                                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Set GiverContactInfo to [", giverContactInfo, ']'));
                                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Result 'True'"));
                            }
                            return true;
                        }

                        //TODO: Если персонаж "прибыл на место, на NPC отсутствует, команду нужно "пропускать"
                        if(@this._giver.Distance < interactDistance)
                        {
                            // Персонаж прибыл на место, однако, нужный NPC отсутствует
                            // невозможно ни принять миссию, ни пропустить команду, 
                            // т.к. для этого нужно перейти в Run()
                            giverContactInfo = null;
                            return true;
                        }
                    }
                    else
                    {
                        if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                        {
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Keep GiverContactInfo [", giverContactInfo, ']'));
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Result 'True'"));
                        }
                        return true;
                    }
                }
                giverContactInfo = null;
                if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Faraway(", giverDistance.ToString("N1"), "). Result 'False'"));
                return false;
            }
        }

        //TODO: Упростить замысловатую логику взятия миссии
        //TODO: Добавить возможность взятия миссии дистанционно (RemoteDialog)
        public ActionResult Run()
        {
            string currentMethodName = EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo
                    ? string.Concat(actionIDstr, '.', MethodBase.GetCurrentMethod().Name)
                    : string.Empty;

            if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Begins try #", tries));

            if (!InternalConditions)
            {
                if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": InternalConditions is False => ActionResult = '", ActionResult.Fail, '\''));
                return ActionResult.Fail;
            }

            if (giverContactInfo != null && giverContactInfo.IsValid)
            {
                Entity entity = giverContactInfo.Entity;
                if (@this._giver.IsMatching(entity))
                {
                    if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Entity [", entity.InternalName, ", ", entity.CostumeRef.CostumeName, "] match to MissionGiverInfo [", @this._giver, ']'));

                    // Проверяем наличие задания у контакта 
                    if (@this._contactHaveMission != ContactHaveMissionCheckType.Disabled)
                    {
                        if (!ContactHaveMission(giverContactInfo))
                        {
                            if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ContactHaveMission is False => ActionResult = '", ActionResult.Skip, '\''));
                            return ActionResult.Skip;
                        }
                        else if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ContactHaveMission is True. Continue..."));
                    }
                    else if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Skiped checking the condition 'ContactHaveMission'"));

                    // Перемещаемся к квестодателю (в случае необходимости)
                    if (!ApproachMissionGiver(entity))
                    {
                        if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ApproachMissionGiver failed => ActionResult = '", ActionResult.Running, '\''));
                        return ActionResult.Running;
                    }
                    else if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ApproachMissionGiver succeeded"));

                    // Взаимодействуем с квестодателем
                    if (!InteractMissionGiver(entity))
                    {
                        if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": InteractMissionGiver failed => ActionResult = '", ActionResult.Running, '\''));
                        return ActionResult.Running;
                    }
                    else if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": InteractMissionGiver succeeded"));

#if true
                    // Проводим попытку принять задание
                    MissionPickUpResult processingResult = ProccessingDialog();
                    tries++;

                    if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ProccessingDialog result is '", processingResult, '\''));
                    switch (processingResult)
                    {
                        case MissionPickUpResult.MissionAccepted:
                            tries = int.MaxValue;
                            if (@this._closeContactDialog)
                            {
                                MissionHelper.CloseAllFrames();
                                Thread.Sleep(2000);
                            }
                            return ActionResult.Completed;
#if true
                        case MissionPickUpResult.MissionOfferAccepted:
                            Thread.Sleep(1000);
                            break;
#endif
                        case MissionPickUpResult.MissionNotFound:
                            if (@this._skipOnFail)
                            {
                                ETLogger.WriteLine(string.Concat(currentMethodName, ": Mission not available..."), true);
                                return ActionResult.Skip;
                            }
                            break;
                        case MissionPickUpResult.MissionRequiredRewardNotFound:
                            ETLogger.WriteLine(string.Concat(currentMethodName, ": Required mission reward not found..."), true);
                            if (@this.CloseContactDialog)
                            {
                                MissionHelper.CloseAllFrames();
                                Thread.Sleep(2000);
                            }
                            return ActionResult.Skip;
#if false
                    case MissionPickUpResult.OfferMissionRequiredRewardNotFound:
                        ETLogger.WriteLine(string.Concat(currentMethodName, ": Required mission reward not found..."), true);
                        if (@this.CloseContactDialog)
                        {
                            MissionHelper.CloseAllFrames();
                            Thread.Sleep(2000);
                        }
                        return ActionResult.Skip; 
#endif
                        case MissionPickUpResult.Error:
                            MissionHelper.CloseAllFrames();
                            Thread.Sleep(1000);
                            break;
                    }
#endif
                }
                else if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Entity [", entity.InternalName, ", ", entity.CostumeRef.CostumeName, "] does not match to MissionGiverInfo [", @this._giver, ']'));
            }
            else
            {
                tries++;

                if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": GiverContact is absent..."));
            }

            ActionResult result = (@this._skipOnFail && tries < 3)
                ? ActionResult.Running
                : ActionResult.Fail;

            if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
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
                if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', nameof(InternalConditions), ": GiverAccessible(", isGiverAccessible, ") AND Not(HavingMissionOrCompleted(", !isHavingMissionOrCompleted, ")) => ", result));
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
                    && @this._giver.Position.Distance3DFromPlayer >= @this._interactDistance)
                        return @this._giver.Position.Clone();
                return new Vector3();
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
            MissionGiverType giverType = MissionGiverType.None;
            string mission_id;

            if (TargetSelectForm.GUIRequest("Target mission giver and press ok.") != DialogResult.OK) return;

            Entity betterEntityToInteract = Interact.GetBetterEntityToInteract();
            if (!betterEntityToInteract.IsValid) return;

            @this.Giver = new MissionGiverInfo()
            {
                Id = betterEntityToInteract.CostumeRef.CostumeName,
                Position = betterEntityToInteract.Location.Clone(),
                MapName = EntityManager.LocalPlayer.MapState.MapName,
                RegionName = EntityManager.LocalPlayer.RegionInternalName
            };

            // Взаимодействие с betterEntityToInteract, чтобы открыть диалоговое окно
            float interactDist = Math.Max(@this._interactDistance, Math.Max(betterEntityToInteract.InteractOption.InteractDistance, 7f));

            if (!(betterEntityToInteract.CombatDistance3 <= interactDist) &&
                !Approach.EntityForInteraction(betterEntityToInteract)) return;

            betterEntityToInteract.Interact();
            Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(3000);
            while (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Options.Count == 0)
            {
                if (timeout.IsTimedOut)
                    break;

                Thread.Sleep(100);
            }

            //CommonTools.FocusForm(typeof(Editor));

            if (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Options.Count != 0)
            {
                // выбор пункта меню, соответствующего миссии
                // TODO: Удалить из пунктов диалога тэги форматирования
                string aDialogKey = GetAnId.GetADialogKey();
                if (aDialogKey.Length > 0)
                {
                    // Вычисляем идентификатор миссии из строки диалога
                    // "OptionsList.MissionOffer.Текстовый_Идентификатор_Миссии_\d"

                    // Индекс начала "текстового идентификатора миссии"
                    int startInd = aDialogKey.IndexOf("MissionOffer.", 0, StringComparison.OrdinalIgnoreCase);

                    // Индекс окончания "текстового идентификатора миссии"
                    // Если последний символ - цифра, значит нужно удалить суффикс
                    int lastInd = char.IsDigit(aDialogKey[aDialogKey.Length - 1])
                        ? aDialogKey.LastIndexOf('_')
                        : aDialogKey.Length - 1;

                    if (startInd >= 0 && startInd + 13 < aDialogKey.Length)
                    {
                        mission_id = aDialogKey.Substring(startInd + 13, lastInd - startInd - 13);

                        if (!string.IsNullOrEmpty(mission_id))
                        {
                            // Принимаем миссию
                            EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.SelectOptionByKey(
                                aDialogKey);
                            Thread.Sleep(1000);


                            if (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.ScreenType ==
                                ScreenType.MissionOffer)
                            {
                                // Выбираем обязательную награду
                                GetAnItem.ListItem rewardItem = GetAnItem.Show(4);
                                if (rewardItem != null && !string.IsNullOrEmpty(rewardItem.ItemId))
                                {
                                    @this.RequiredRewardItem = rewardItem.ItemId;
                                    //patternPos = PUM.RequiredRewardItem.GetSimplePatternPosition(out rewardItemPattern);
                                }

                                // Принимаем миссию
                                EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.SelectOptionByKey(
                                    "ViewOfferedMission.Accept");

                                // Проверяем корректность определения "идентификатора миссии"
                                timeout.Reset();
                                while (!Missions.HaveMissionByPath(mission_id))
                                {
                                    if (timeout.IsTimedOut)
                                    {
                                        // Миссия была "принята", однако, по найденному идентификатору она "не определяется"
                                        // XtraMessageBox.Show("Select MissionId\n" +
                                        //    "Автоматическое определение идентификатора миссии не удалось.\n" +
                                        //    "Укажите миссию вручную");
                                        mission_id = GetMissionId.Show(true, mission_id, false, false);
                                        if (!string.IsNullOrEmpty(mission_id))
                                            @this._missionId = mission_id;

                                        return;
                                    }

                                    Thread.Sleep(100);
                                }

                                @this._missionId = mission_id;
                                return;
                            }
                        }
                    }
                }
            }

            // Ручной выбор идентификатора миссии
            mission_id = GetMissionId.Show(true);
            if (!string.IsNullOrEmpty(mission_id))
                @this._missionId = mission_id;

            label = string.Empty;
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
        /// <summary>
        /// Оценка расстояния до квестодателя <paramref name="giverEntity"/> и перемещение к нему, в случае необходимости
        /// </summary>
        private bool ApproachMissionGiver(Entity giverEntity)
        {
            // TODO: пропускать, если дистанционный квестодатель

            string currentMethodName = EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo
                    ? string.Concat(actionIDstr, '.', MethodBase.GetCurrentMethod().Name)
                    : string.Empty;

            // Проверяем расстояние до квестодателя и перемещаемся к нему 
            float interactDistance = Math.Max(@this._interactDistance, 5f);
            var giverLocation = giverEntity.Location;
            var playerLocation = EntityManager.LocalPlayer.Location;
            double distance = giverLocation.Distance3D(playerLocation);
            bool withingInteractDistance = distance <= interactDistance;
            bool isInteractzDifferenceConstraint = @this._interactZDifference <= 0;
            float zDifference = isInteractzDifferenceConstraint ? 0 : Math.Abs(giverLocation.Z - playerLocation.Z);
            bool withingInteractZDifference = isInteractzDifferenceConstraint || zDifference <= @this._interactZDifference;
            if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Begins (",
                    "CalculatedInteractDistance = ", interactDistance.ToString("N1"),
                    "; Distance = ", distance.ToString("N1"), withingInteractDistance ? "(withing)" : "(out)",
                    "; zDifference = ", zDifference.ToString("N1"), isInteractzDifferenceConstraint ? (withingInteractZDifference ? " (withing))" : " (out))") : " (unlimited))"));

            if (!(withingInteractDistance && withingInteractZDifference))
            {
#if false
                Approach.EntityForInteraction(entity);  
#elif true
                // Случается, что Approach.EntityByDistance() возвращает True, даже если расстояние превышает заданное
                if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    /*bool result = */Approach.EntityByDistance(giverEntity, interactDistance);
                    sw.Stop();
                    playerLocation = EntityManager.LocalPlayer.Location;
                    distance = giverLocation.Distance3D(playerLocation);
                    withingInteractDistance = distance <= interactDistance;
                    zDifference = isInteractzDifferenceConstraint ? 0 : Math.Abs(giverLocation.Z - playerLocation.Z);
                    withingInteractZDifference = isInteractzDifferenceConstraint || zDifference <= @this._interactZDifference;
                    bool result = withingInteractDistance && withingInteractZDifference;
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Approach time = ", sw.ElapsedMilliseconds.ToString("N2"), " (", sw.ElapsedTicks, ')',
                        "\n\t\tCalculatedInteractDistance = ", interactDistance.ToString("N1"),
                        "\n\t\tDistance = ", distance.ToString("N1"), withingInteractDistance ? "(withing)" : "(out)",
                        "\n\t\tzDifference = ", zDifference.ToString("N1"), isInteractzDifferenceConstraint ? (withingInteractZDifference ? " (withing)" : " (out)") : " (unlimited)",
                        "\n\t" + nameof(Approach.EntityByDistance) + " => ", result));
                    return result;
                }
                else
                {
                    Approach.EntityByDistance(giverEntity, interactDistance);
                    return giverLocation.Distance3D(playerLocation) <= interactDistance && (isInteractzDifferenceConstraint || Math.Abs(giverLocation.Z - playerLocation.Z) <= @this._interactZDifference);
                }
#else
                return giverLocation.Approach(interactDistance);
#endif
            }
            else if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
            {
                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": " + nameof(Approach.EntityByDistance) + "Skiped"));
            }
            return true;
        }

        /// <summary>
        /// Взаимодействие с квестодателем <paramref name="giverEntity"/> (открытитие диалогового окна) 
        /// </summary>
        private bool InteractMissionGiver(Entity giverEntity)
        {
            string currentMethodName = EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo
                    ? string.Concat(actionIDstr, '.', MethodBase.GetCurrentMethod().Name)
                    : string.Empty;

            MissionHelper.CloseSpecialFrames();

            var contactDialog = EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog;

            if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Begins (ContactDialog = ", 
                    contactDialog.IsValid ? "Valid; " : "Invalid; ",
                    "ScreenType = ", contactDialog.ScreenType, ')'));


            // TODO: предусмотреть взаимодействие с дистаннционным квестодателем
            bool result;
            if (!contactDialog.IsValid || contactDialog.ScreenType == ScreenType.None)
            {
#if true
                float interactDistance = Math.Max(@this._interactDistance, 5f);
                if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    result = giverEntity.SmartInteract(interactDistance);
                    sw.Stop();
                    contactDialog = EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog;
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": SmartInteraction result = ", result, "; time = ", sw.ElapsedMilliseconds.ToString("N2"), "ms (", sw.ElapsedTicks, ")" +
                        "\n\t\tContactDialog = ", contactDialog.IsValid ? "Valid" : "Invalid",
                        "\n\t\tScreenType = ", contactDialog.ScreenType,
                        "\n\t" + nameof(NavigationHelper.SmartInteract) + " => ", result));
                }
                else result = giverEntity.SmartInteract(interactDistance);
#else
                if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    result = Interact.ForContactDialog(giverEntity);
                    sw.Stop();
                    contactDialog = EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog;
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": SmartInteraction result = ", result, "; time = ", sw.ElapsedMilliseconds.ToString("N2"), "ms (", sw.ElapsedTicks, ")" +
                        "\n\t\tContactDialog = ", contactDialog.IsValid ? "Valid" : "Invalid",
                        "\n\t\tScreenType = ", contactDialog.ScreenType,
                        "\n\t" + nameof(Interact.ForContactDialog) + " => ", result));
                }
                else result = Interact.ForContactDialog(giverEntity);
#endif
            }
            else
            {
                contactDialog = EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog;
                result = contactDialog.IsValid;
                if(EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ", nameof(Interact.ForContactDialog), " Skiped"));
            }
            return result;
        }

        /// <summary>
        /// Обработка диалога
        /// </summary>
        private MissionPickUpResult ProccessingDialog()
        {
            string currentMethodName = EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo
                    ? string.Concat(actionIDstr, '.', MethodBase.GetCurrentMethod().Name)
                    : string.Empty;

            if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', currentMethodName, ": Begin"));

            var contactDialog = EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog;
            var screenType = contactDialog.ScreenType;

            // обрабатываем предварительные пункты диалога, если задано
            ProcessingOptionalDialogs();

            var timeout = new Astral.Classes.Timeout(TIME);
            MissionPickUpResult result;
            while (contactDialog.IsValid && !timeout.IsTimedOut) 
            {
                // Диалоговое окно открыто
                // Проверяем статус экрана 
                if (screenType == ScreenType.MissionOffer)
                {
                    // Открыт экран принятия миссии
                    // На экране принятия миссии узнать из АПИ, к какой миссии он относится, - нельзя
                    // Проверяем наличие обязательных наград в окне миссии
                    if (CheckRequeredRewardItem())
                    {
                        // Принимаем миссию
                        if (contactDialog.CheckDialogOptionAndSelect(d => d.Key.Equals("ViewOfferedMission.Accept"),
                            () => contactDialog.ScreenType == ScreenType.MissionOffer))
                        {
                            result = MissionHelper.HaveMission(@this._missionId, out _)
                                        ? MissionPickUpResult.MissionAccepted
                                        : MissionPickUpResult.MissionOfferAccepted;

                            if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
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
                        result = MissionPickUpResult.MissionRequiredRewardNotFound;
                        if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ":" +
                                "\n\t\tScreenType = ", screenType,
                                "\n\t\tCheckRequeredRewardItem = False" +
                                "\n\t\tSelect 'ViewOfferedMission.Back' = True" +
                                "\n\t" + nameof(ProccessingDialog) + " => ", result));

                        return result;
                    }
#endif
                    result = MissionPickUpResult.Error;
                    if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
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
                    // OptionList.Exit - Закрыть диалоговое окно
                    // Принимаем задачу в несколько этапов
                    // 1. Активируем пункт диалога, содержащий идентификатору задачи
                    // 2. Проверяем наличие награды
                    // 3. Принимаем задачу путем активации соответствующего пункта диалога
                    if (contactDialog.CheckDialogOptionAndSelect(d => d.Key.Contains(@this._missionId), () => contactDialog.ScreenType == screenType))
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

                                    if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
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
                                    if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
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
                                if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ":" +
                                        "\n\t\tScreenType = ", screenType,
                                        "\n\t\tSelect '", @this._missionId, "' = True" +
                                        "\n\t\tCheckRequeredRewardItem = False" +
                                        "\n\t" + nameof(ProccessingDialog) + " => ", result));
                                return result;
                            }
                        } 
#else
                        if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Selection of '", @this._missionId, "' on ScreenType(", screenType, ") succedded. Continue..."));
#endif
                    }
                    else
                    {
                        result = MissionPickUpResult.MissionNotFound;
                        if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
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
                        if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Selection of '", lastOption.Key, "' on ScreenType(", screenType, ") succedded. Continue..."));
                    }
                    else
                    {
                        if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                        {
                            result = MissionPickUpResult.Error;
                            if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
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
                    result = MissionPickUpResult.Error;
                    if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ":" +
                            "\n\t\tScreenType = ", screenType,
                            "\n\t" + nameof(ProccessingDialog) + " => ", result));
                    return result;
                }

                Thread.Sleep(1000);
                contactDialog = EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog;
                screenType = contactDialog.ScreenType;
            }
            

            result = MissionPickUpResult.Error;
            if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Time is out => ", result));
            return result;
        }

        /// <summary>
        /// Обработка дополнительный пунктов диалога 'Dialogs' перед принятием миссии (если заданы)
        /// </summary>
        private void ProcessingOptionalDialogs()
        {
            if (@this._dialogs.Count > 0)
            {
                string currentMethodName = EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo
                    ? string.Concat(actionIDstr, '.', MethodBase.GetCurrentMethod().Name)
                    : string.Empty;

                var interactInfo = EntityManager.LocalPlayer.Player.InteractInfo;
                foreach (string key in @this._dialogs)
                {
#if true
                    interactInfo.ContactDialog.SelectOptionByKey(key, "");
                    Thread.Sleep(1000);
#else
                    interactInfo.ContactDialog.CheckDialogOptionAndSelect(d => d.Key.Contains(key));
#endif
                    if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Dialog option '", key, "' selected"));
                }
            }
        }

        /// <summary>
        /// Проверка наличия в наградах заданного итема
        /// </summary>
        /// <returns></returns>
        private bool CheckRequeredRewardItem()
        {
            var contactDialog = EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog;
            // Проверяем тип активного экрана диалогового окна
            if (contactDialog.ScreenType == ScreenType.MissionOffer)
            {
                // Открыт экран получения квеста
                // Проверяем наличие в сумках с наградой заданного итема
                foreach (var bag in contactDialog.RewardBags)
                    foreach (var itemSlot in bag.GetItems)
                        if (isRewardItem(itemSlot.Item.ItemDef.InternalName))
                            return true;
            }

            return false;
        }

        private bool ContactHaveMission(ContactInfo contactInfo)
        {
            var indicator = contactInfo.Indicator;
            if (@this._contactHaveMission == ContactHaveMissionCheckType.Any)
                return indicator == IndicatorType.MissionAvailable
                    || indicator == IndicatorType.MissionRepeatableAvailable
                    || indicator == IndicatorType.MissionRepeatableAvailableOverride;
            if (@this._contactHaveMission == ContactHaveMissionCheckType.RepeatablesOnly)
                return indicator == IndicatorType.MissionRepeatableAvailable
                    || indicator == IndicatorType.MissionRepeatableAvailableOverride;

            return true;
        }

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
        #endregion
    }
}