using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Astral.Logic.NW;
using EntityCore.Enums;
using EntityCore.Forms;
using EntityCore.Tools.Navigation;
using EntityTools;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;

namespace EntityCore.Tools.Missions
{
    public static class MissionHelper
    {
        /// <summary>
        /// Проверка наличия пункта диалога соответствующего ключу <paramref name="key"/> и его активация
        /// </summary>
        public static bool CheckOptionByKeyAndSelect(this ContactDialog contactDialog, string key)
        {
            foreach (var dialogOption in contactDialog.Options)
            {
                if(dialogOption.Key.Equals(key)) continue;

                bool result = dialogOption.Select();
                Thread.Sleep(500);
                return result;
            }

            return false;
        }

        /// <summary>
        /// Проверка пункта диалога на соответствие <paramref name="optionCheck"/>, его активация 
        /// и последующее ожидание пока верно <paramref name="waitCheck"/> или до истечения <paramref name="time"/>
        /// </summary>
        public static bool CheckDialogOptionAndSelect(this ContactDialog contactDialog, Func<ContactDialogOption, bool> optionCheck, Func<bool> waitCheck = null, int time = 2000)
        {
            //TODO: Научиться отслеживать изменение диалогового окна, происходящие в ответ на активацию пунктов диалога
            bool result = false;
            foreach (var contactDialogOption in contactDialog.Options)
            {
                if (!optionCheck(contactDialogOption))
                    continue;

                if (contactDialogOption.CannotChoose)
                    break;

                var waitTimeout = new Astral.Classes.Timeout(time);

                result = contactDialogOption.Select();
                Thread.Sleep(500);

                if (waitCheck is null)
                    break;

                while (!waitTimeout.IsTimedOut && waitCheck())
                {
                    Thread.Sleep(100);
                }
                break;
            }

            return result;
        }

        /// <summary>
        /// Проверка наличия задачи в списке активных или завершенных
        /// </summary>
        public static bool CheckHavingMissionOrCompleted(string missionId)
        {
            var missionParts = missionId.Split('/');

            if (missionParts.Length == 0) return false;

            // Поиск активной задачи
            var mission = EntityManager.LocalPlayer.Player.MissionInfo.Missions.FirstOrDefault(m => m.MissionName == missionParts[0])
                          ?? EntityManager.LocalPlayer.MapState.OpenMissions.FirstOrDefault(m => m.Mission.MissionName == missionParts[0])?.Mission;
            if (missionParts.Length > 1 && mission != null)
            {
                for (int i = 1; i < missionParts.Length; i++)
                {
                    mission = mission.Childrens.FirstOrDefault(m => m.MissionName == missionParts[i]);
                    if (mission is null)
                        break;
                }
            }
            if (mission != null && mission.IsValid)
                return true;

            // Поиск 'выполненной' задачи
            var completedMission = EntityManager.LocalPlayer.Player.MissionInfo.CompletedMissions.FirstOrDefault(m =>
                m.MissionDef.Name == missionParts[0]);
            return completedMission?.MissionDef.CanRepeat == false;
        }

        /// <summary>
        /// Проверка наличия задачи <paramref name="missionId"/>
        /// </summary>
        public static bool HaveMission(string missionId, out Mission mission)
        {
            mission = null;
            var missionParts = missionId.Split('/');

            if (missionParts.Length == 0)
                return false;

            mission = EntityManager.LocalPlayer.Player.MissionInfo.Missions.FirstOrDefault(m => m.MissionName == missionParts[0])
                ?? EntityManager.LocalPlayer.MapState.OpenMissions.FirstOrDefault(m => m.Mission.MissionName == missionParts[0])?.Mission;

            if (missionParts.Length > 1 && mission != null)
            {
                for (int i = 1; i < missionParts.Length; i++)
                {
                    mission = mission.Childrens.FirstOrDefault(m => m.MissionName == missionParts[i]);
                    if (mission is null)
                        break;
                }
            }

            return mission != null && mission.IsValid;
        }

        /// <summary>
        /// Проверка наличия задачи <paramref name="missionId"/>
        /// </summary>
        public static bool CompletedMission(string missionId, out MissionDef mission)
        {
            mission = null;
            var missionParts = missionId.Split('/');

            if (missionParts.Length == 0)
                return false;

            var missionRoot = EntityManager.LocalPlayer.Player.MissionInfo.CompletedMissions.FirstOrDefault(m => m.MissionDef.Name == missionParts[0])?.MissionDef;

            if (missionParts.Length > 1 && mission != null)
            {
                for (int i = 1; i < missionParts.Length; i++)
                {
                    var missionDef = mission.SubMissions.FirstOrDefault(m => m.Name == missionParts[i]);
                    if (missionDef is null)
                        break;
                }
            }
            mission = missionRoot;

            return mission != null && mission.IsValid;
        }

        /// <summary>
        /// Проверка наличия у Квестгивера <paramref name="contactInfo"/> миссии типа <paramref name="checkType"/>
        /// </summary>
        public static bool ContactHaveMission(this ContactInfo contactInfo, ContactHaveMissionCheckType checkType = ContactHaveMissionCheckType.Any)
        {
            var indicator = contactInfo.Indicator;
            if (checkType == ContactHaveMissionCheckType.Any)
                return indicator == IndicatorType.MissionAvailable
                    || indicator == IndicatorType.MissionRepeatableAvailable
                    || indicator == IndicatorType.MissionRepeatableAvailableOverride;
            if (checkType == ContactHaveMissionCheckType.RepeatablesOnly)
                return indicator == IndicatorType.MissionRepeatableAvailable
                    || indicator == IndicatorType.MissionRepeatableAvailableOverride;

            return true;
        }

        /// <summary>
        /// Проверка наличия в наградах предмета, соответствующего критерию <paramref name="isRewardItem"/>
        /// </summary>
        /// <returns></returns>
        public static bool CheckRequeredRewardItem(Predicate<Item> isRewardItem)
        {
            if (isRewardItem is null)
                return true;

            var contactDialog = EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog;
            var screenType = contactDialog.ScreenType;
            // Проверяем тип активного экрана диалогового окна
            if (screenType == ScreenType.MissionOffer
                || screenType == ScreenType.MissionTurnIn)
            {
                // Открыт экран получения квеста
                // Проверяем наличие в сумках с наградой заданного итема
                foreach (var bag in contactDialog.RewardBags)
                    foreach (var itemSlot in bag.GetItems)
                        if (isRewardItem(itemSlot.Item))
                            return true;
            }

            return isRewardItem(GameHelper.EmptyItem);
        }


        /// <summary>
        /// Оценка расстояния до квестодателя <paramref name="giverEntity"/> и перемещение к нему, в случае необходимости
        /// </summary>
        public static bool ApproachMissionGiver(this Entity giverEntity, float interactDistance = 5.5f, float maxZDifference = 5)
        {
            bool debugInfoEnabled = EntityTools.EntityTools.Config.Logger.DebugMissionTools;

            string currentMethodName = debugInfoEnabled
                    ? string.Concat(MethodBase.GetCurrentMethod().Name)
                    : string.Empty;

            // Проверяем расстояние до квестодателя и перемещаемся к нему 
            var giverLocation = giverEntity.Location;
            var playerLocation = EntityManager.LocalPlayer.Location;
            double distance = giverLocation.Distance3D(playerLocation);
            bool withingInteractDistance = distance <= interactDistance;
            bool isInteractzDifferenceConstraint = maxZDifference <= 0;
            float zDifference = isInteractzDifferenceConstraint ? 0 : Math.Abs(giverLocation.Z - playerLocation.Z);
            bool withingInteractZDifference = isInteractzDifferenceConstraint || zDifference <= maxZDifference;
            if (debugInfoEnabled)
                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Begins (",
                    "CalculatedInteractDistance = ", interactDistance.ToString("N1"),
                    "; Distance = ", distance.ToString("N1"), withingInteractDistance ? "(withing)" : "(out)",
                    "; zDifference = ", zDifference.ToString("N1"), isInteractzDifferenceConstraint ? (withingInteractZDifference ? " (withing))" : " (out))") : " (unlimited))"));

            if (!(withingInteractDistance && withingInteractZDifference))
            {
                // Случается, что Approach.EntityByDistance() возвращает True, даже если расстояние превышает заданное
                if (debugInfoEnabled)
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
#if false
                    Approach.EntityByDistance(giverEntity, interactDistance);
#else
                    Approach.EntityForInteraction(giverEntity);
#endif
                    sw.Stop();
                    playerLocation = EntityManager.LocalPlayer.Location;
                    distance = giverLocation.Distance3D(playerLocation);
                    withingInteractDistance = distance <= interactDistance;
                    zDifference = isInteractzDifferenceConstraint ? 0 : Math.Abs(giverLocation.Z - playerLocation.Z);
                    withingInteractZDifference = isInteractzDifferenceConstraint || zDifference <= maxZDifference;
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
#if false
                    Approach.EntityByDistance(giverEntity, interactDistance);
#else
                    Approach.EntityForInteraction(giverEntity);
#endif
                    return giverLocation.Distance3D(playerLocation) <= interactDistance && (isInteractzDifferenceConstraint || Math.Abs(giverLocation.Z - playerLocation.Z) <= maxZDifference);
                }
            }
            else if (debugInfoEnabled)
            {
                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": " + nameof(Approach.EntityByDistance) + "Skiped"));
            }
            return true;
        }

        /// <summary>
        /// Взаимодействие с квестодателем <paramref name="giverEntity"/> (открытитие диалогового окна) 
        /// </summary>
        public static bool InteractMissionGiver(this Entity giverEntity, float interactDistance = 5.5f)
        {
            bool debugInfoEnabled = EntityTools.EntityTools.Config.Logger.DebugMissionTools;

            string currentMethodName = debugInfoEnabled
                    ? string.Concat(MethodBase.GetCurrentMethod().Name)
                    : string.Empty;

            GameHelper.CloseSpecialFrames();

            var contactDialog = EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog;

            if (debugInfoEnabled)
                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Begins (ContactDialog = ",
                    contactDialog.IsValid ? "Valid; " : "Invalid; ",
                    "ScreenType = ", contactDialog.ScreenType, ')'));

            bool result;
#if true
            if (!contactDialog.IsValid || contactDialog.ScreenType == ScreenType.None)
            {
                if (debugInfoEnabled)
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
            }
            else
            {
                contactDialog = EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog;
                result = contactDialog.IsValid;
                if (debugInfoEnabled)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ", nameof(NavigationHelper.SmartInteract), " Skiped"));
            }
#else
            if (!contactDialog.IsValid || contactDialog.ScreenType == ScreenType.None)
            {
                if (debugInfoEnabled)
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
            }
            else
            {
                contactDialog = EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog;
                result = contactDialog.IsValid;
                if (debugInfoEnabled)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ", nameof(Interact.ForContactDialog), " Skiped"));
            }
#endif
            return result;
        }

        /// <summary>
        /// Обработка диалога взятия(сдачи) квеста
        /// </summary>
        public static MissionProcessingResult ProccessingMissionDialog(string missionId, bool turnInMission = false, IList<string> optionalDialog = null, Predicate<Item> isRewardItem = null, int timeout = 5000)
        {
            bool debugInfoEnabled = EntityTools.EntityTools.Config.Logger.DebugMissionTools;

            string currentMethodName = debugInfoEnabled
                    ? MethodBase.GetCurrentMethod().Name
                    : string.Empty;

            if (debugInfoEnabled)
                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Begin"));

            Interact.WaitForInteraction();

            var player = EntityManager.LocalPlayer.Player;
            var contactDialog = player.InteractInfo.ContactDialog;
            var screenType = contactDialog.ScreenType;

            // обрабатываем предварительные пункты диалога, если задано
            if (optionalDialog?.Count > 0)
                MissionHelper.ProcessingOptionalDialogs(optionalDialog);

            //TODO: Добавить возможность сдавать выполненную миссию (т.е. прописать необходимые для этого DialogKey
            var timer = new Astral.Classes.Timeout(timeout);
            // Ключ принятия миссии:
            //      OptionsList.MissionOffer.Идентификатор_миссии_\d*
            // Ключ сдачи миссии:
            //      OptionsList.CompleteMission.Идентификатор_миссии_\d*
            var missionDialogKey = turnInMission ? "OptionsList.CompleteMission." + missionId : "OptionsList.MissionOffer." + missionId;
            MissionProcessingResult result;
            while (contactDialog.IsValid && !timer.IsTimedOut)
            {
                // Диалоговое окно открыто
                // Проверяем статус экрана 
                if (!turnInMission && screenType == ScreenType.MissionOffer)
                {
                    // Открыт экран принятия миссии
                    // На экране принятия миссии узнать из АПИ, к какой миссии он относится, - нельзя
                    // Проверяем наличие обязательных наград в окне миссии
                    if (CheckRequeredRewardItem(isRewardItem))
                    {
                        // Принимаем миссию
                        if (contactDialog.CheckDialogOptionAndSelect(d => d.Key.Equals("ViewOfferedMission.Accept"),
                            () => contactDialog.ScreenType == ScreenType.MissionOffer))
                        {
                            result = MissionProcessingResult.MissionOfferAccepted;
                            var timer2 = new Astral.Classes.Timeout(timeout);
                            while (!timer2.IsTimedOut)
                            {
                                if (HaveMission(missionId, out _))
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
                                    "\n\t" + nameof(ProccessingMissionDialog) + " => ", result));
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
                                "\n\t" + nameof(ProccessingMissionDialog) + " => ", result));

                        return result;
                    }
#endif
                    result = MissionProcessingResult.Error;
                    if (debugInfoEnabled)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ":" +
                            "\n\t\tScreenType = ", screenType,
                            "\n\t\tCheckRequeredRewardItem = False" +
                            "\n\t\tSelect 'ViewOfferedMission.Back' = False" +
                            "\n\t" + nameof(ProccessingMissionDialog) + " => ", result));

                    return result;
                }
                else if(turnInMission && screenType == ScreenType.MissionTurnIn)
                {
                    // Открыт экран принятия миссии
                    // На экране принятия миссии узнать из АПИ, к какой миссии он относится, - нельзя
                    // Проверяем наличие обязательных наград в окне миссии
                    if (CheckRequeredRewardItem(isRewardItem))
                    {
                        // Сдаем миссию
                        if (contactDialog.CheckDialogOptionAndSelect(d => d.Key.Equals("ViewCompleteMission.Continue"),
                            () => contactDialog.ScreenType == ScreenType.MissionOffer))
                        {
                            result = MissionProcessingResult.Error;
                            var timer2 = new Astral.Classes.Timeout(timeout);
                            while (!timer2.IsTimedOut)
                            {
                                if (CompletedMission(missionId, out _))
                                {
                                    result = MissionProcessingResult.MissionTurnedIn;
                                    break;
                                }
                                Thread.Sleep(250);
                            }

                            if (debugInfoEnabled)
                                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ":" +
                                    "\n\t\tScreenType = ", screenType,
                                    "\n\t\tCheckRequeredRewardItem = True" +
                                    "\n\t\tSelect 'ViewCompleteMission.Continue' = True" +
                                    "\n\t" + nameof(ProccessingMissionDialog) + " => ", result));
                            return result;
                        }
                    }

                    // В наградах отсутствуют обязательные итемы - отказываемся сдавать миссию.
#if true
                    else if (contactDialog.CheckDialogOptionAndSelect(d => d.Key.Equals("ViewCompleteMission.Back"),
                        () => contactDialog.ScreenType == ScreenType.MissionOffer))
                    {
                        result = MissionProcessingResult.MissionRequiredRewardNotFound;
                        if (debugInfoEnabled)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ":" +
                                "\n\t\tScreenType = ", screenType,
                                "\n\t\tCheckRequeredRewardItem = False" +
                                "\n\t\tSelect 'ViewCompleteMission.Back' = True" +
                                "\n\t" + nameof(ProccessingMissionDialog) + " => ", result));

                        return result;
                    }
#endif
                    result = MissionProcessingResult.Error;
                    if (debugInfoEnabled)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ":" +
                            "\n\t\tScreenType = ", screenType,
                            "\n\t\tCheckRequeredRewardItem = False" +
                            "\n\t\tSelect 'ViewCompleteMission.Back' = False" +
                            "\n\t" + nameof(ProccessingMissionDialog) + " => ", result));

                    return result;
                }
                else if (screenType == ScreenType.List)
                {
                    // Открыт экран списка пунктов диалога
                    // ContactDialog.ScreenType = List
                    // ContactDialog.Options[].Key:
                    //      OptionsList.MissionOffer.Идентификато_миссии* - Пункт диалога, инициирующий принятие миссии
                    //      OptionsList.CompleteMission.Идентификатор_миссии_\d* - Пункт диалога, инициирующия сдачу миссии
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
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Select the option '", missionId, "' on '", screenType, "' screen. Continue..."));
#endif
                    }
                    else
                    {
                        result = MissionProcessingResult.MissionNotFound;
                        if (debugInfoEnabled)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ":" +
                                "\n\t\tScreenType = ", screenType,
                                "\n\t\tSelect '", missionId, "' = False" +
                                "\n\t" + nameof(ProccessingMissionDialog) + " => ", result));
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
                                    "\n\t" + nameof(ProccessingMissionDialog) + " => ", result));
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
                            "\n\t" + nameof(ProccessingMissionDialog) + " => ", result));
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

        /// <summary>
        /// Обработка дополнительный пунктов диалога 'Dialogs' перед принятием миссии (если заданы)
        /// </summary>
        public static void ProcessingOptionalDialogs(IList<string> dialogs)
        {
            if (dialogs?.Count > 0)
            {
                bool debugInfoEnabled = EntityTools.EntityTools.Config.Logger.DebugMissionTools;

                string currentMethodName = debugInfoEnabled
                    ? MethodBase.GetCurrentMethod().Name
                    : string.Empty;

                var player = EntityManager.LocalPlayer.Player;
                var interactInfo = EntityManager.LocalPlayer.Player.InteractInfo;
                foreach (string key in dialogs)
                {
                    var contactDialog = player.InteractInfo.ContactDialog;
                    var screenType = contactDialog.ScreenType;

                    if (screenType == ScreenType.MissionOffer
                        || screenType == ScreenType.MissionTurnIn)
                    {
                        if (debugInfoEnabled)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": '", screenType, "' screen is opened. Break"));
                        break;
                    }
#if true
                    contactDialog.SelectOptionByKey(key, string.Empty);
                    Thread.Sleep(1000);
#else
                    contactDialog.CheckDialogOptionAndSelect(d => d.Key.Contains(key));
#endif
                    if (debugInfoEnabled)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Dialog option '", key, "' selected on ", screenType, " screen"));
                }
            }
        }

#if false // Astral.Quester.Forms.GetAnId.GetADialogKey()
        public static string GetADialogKey()
        {
            GetAnId form = new GetAnId("Dialog window must be opened :");
            form.refreshList = delegate ()
            {
                form.listBoxControl1.Items.Clear();
                foreach (ContactDialogOption contactDialogOption2 in Class1.LocalPlayer.Player.InteractInfo.ContactDialog.Options)
                {
                    form.listBoxControl1.Items.Add(contactDialogOption2.ToString());
                }
            };
            form.ShowDialog();
            if (form.valid && form.listBoxControl1.SelectedItem != null)
            {
                using (List<ContactDialogOption>.Enumerator enumerator = Class1.LocalPlayer.Player.InteractInfo.ContactDialog.Options.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        ContactDialogOption contactDialogOption = enumerator.Current;
                        if (contactDialogOption.ToString() == form.listBoxControl1.SelectedItem.ToString())
                        {
                            return contactDialogOption.Key;
                        }
                    }
                    goto IL_CB;
                }
                string result;
                return result;
            }
            IL_CB:
            return string.Empty;
        } 
#endif
        public static bool GetADialogKey(out string key)
        {
            key = string.Empty;
            Func<IEnumerable<string>> source = () =>
            {
                var options = EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Options;
                if (options.Count > 0)
                {
                    List<string> keys = new List<string>();
                    foreach (var option in options)
                    {
                        keys.Add(option.Key);
                    }
                    return keys;
                }
                return Enumerable.Empty<string>();
            };

            string value = string.Empty;
            if (ItemSelectForm.GetAnItem(source, ref value))
            {
                key = value;
                return true;
            }
            return false;
        }
        public static bool GetADialogOption(out ContactDialogOption selectedOption)
        {
            selectedOption = null;
            Func<IEnumerable<ContactDialogOption>> source = () =>
            {
                var options = EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Options;
                if (options.Count > 0)
                {
                    var optionList = new List<ContactDialogOption>(options);
                    return optionList;
                }
                return Enumerable.Empty<ContactDialogOption>();
            };

            ContactDialogOption selectedValue = null;
            if (ItemSelectForm.GetAnItem(source, ref selectedValue, dialogOptionFormatter))
            {
                selectedOption = selectedValue;
                return selectedOption.IsValid;
            }
            return false;
        }
        private static void dialogOptionFormatter(object sender, ListControlConvertEventArgs e)
        {
            if (e.Value is ContactDialogOption option)
            {
                e.Value = string.Concat(Regex.Replace(option.DisplayName.ToString(), "<[^>]+>", string.Empty), " [", option.Key, ']');
            }
        } 
    }
}
