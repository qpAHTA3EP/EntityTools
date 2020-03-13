using Astral;
using Astral.Logic.NW;
using Astral.Quester.Forms;
using EntityTools.Extentions;
using EntityCore.Forms;
using EntityTools.Enums;
using EntityTools.Quester.Actions;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using EntityTools.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using static Astral.Quester.Classes.Action;
using EntityCore.Enums;
using Astral.Quester.Classes;

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
        /// Награда за предложенную миссию не содержит обязательного предмета
        /// </summary>
        OfferMissionRequiredRewardNotFound,
        /// <summary>
        /// Принята миссия, предложенная НПС
        /// </summary>
        MissionOfferAccepted,
        /// <summary>
        /// Целвая миссия принята
        /// </summary>
        MissionAccepted
    }
}

namespace EntityCore.Quester.Action
{
    public class PickUpMissionEngine : IQuesterActionEngine
    {
        PickUpMissionExt @this;
        ContactInfo giverContactInfo = null;
        private int tries = 0;
        private SimplePatternPos patternPos = SimplePatternPos.None;
        private string rewardItemPattern = string.Empty;
        private string label = string.Empty;

        public PickUpMissionEngine(PickUpMissionExt pum) 
        {
            @this = pum;
            @this.coreNeedToRun = NeedToRun;
            @this.coreRun = Run;
            @this.coreValidate = Validate;
            @this.coreReset = Reset;
            @this.coreGatherInfos = GatherInfos;
            @this.coreString = Label;

            @this.PropertyChanged += PropertyChanged;
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (object.ReferenceEquals(sender, @this))
                switch(e.PropertyName)
                {
                    case "RequiredRewardItem":
                        patternPos = @this.RequiredRewardItem.GetSimplePatternPosition(out rewardItemPattern);
                        break;
                    case "MissionId":
                        label = $"{@this.GetType().Name}: [{@this.MissionId}]";
                        break;
                }
        }

        public bool NeedToRun()
        {
            if (@this.Giver.MapName == EntityManager.LocalPlayer.MapState.MapName
                && @this.Giver.RegionName == EntityManager.LocalPlayer.RegionInternalName
                && @this.Giver.Position.Distance3DFromPlayer < Math.Max(@this.InteractDistance, 20f))
            {
                foreach (ContactInfo contactInfo in EntityManager.LocalPlayer.Player.InteractInfo.NearbyContacts)
                {
                    if (@this.Giver.IsMatching(contactInfo.Entity)
                        && ((contactInfo.Entity.Location.Distance3DFromPlayer < Math.Max(@this.InteractDistance, 20f)
                                && Astral.Logic.General.ZAxisDiffFromPlayer(contactInfo.Entity.Location) < 10.0)
                            || contactInfo.Entity.CanInteract))
                    {
                        giverContactInfo = contactInfo;
                        return true;
                    }
                }
            }
            giverContactInfo = null;
            return false;
        }

        public ActionResult Run()
        {
            if (Validate())
            {
                if (giverContactInfo == null
                    || !giverContactInfo.IsValid)
                {
                    // giverContactInfo недействительный, поэтому производим повторный поиск
                    foreach (ContactInfo contactInfo in EntityManager.LocalPlayer.Player.InteractInfo.NearbyContacts)
                    {
                        if (contactInfo.Entity.IsValid && @this.Giver.IsMatching(contactInfo.Entity))
                        {
                            giverContactInfo = contactInfo;
                            break;
                        }
                    }
                }

                if (giverContactInfo != null
                    && giverContactInfo.IsValid)
                {
                    Entity entity = giverContactInfo.Entity;
                    if (entity.IsValid && @this.Giver.IsMatching(entity))
                    {
                        MissionPickUpResult pickUpResult = PickUpMission(entity, @this.MissionId);
                        switch (pickUpResult)
                        {
                            case MissionPickUpResult.MissionAccepted:
                                tries = 0;
                                if (@this.CloseContactDialog)
                                {
                                    EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Close();
                                    Thread.Sleep(2000);
                                }
                                return ActionResult.Completed;
                            case MissionPickUpResult.MissionOfferAccepted:
                                tries++;
                                break;
                            case MissionPickUpResult.MissionNotFound:
                                if (@this.SkipOnFail)
                                {
                                    Logger.WriteLine("Mission not available...");
                                    return ActionResult.Skip;
                                }
                                tries++;
                                break;
                            case MissionPickUpResult.MissionRequiredRewardNotFound:
                                Logger.WriteLine("Required misstion reward not found...");
                                if (@this.CloseContactDialog)
                                {
                                    EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Close();
                                    Thread.Sleep(2000);
                                }
                                return ActionResult.Skip;
                            case MissionPickUpResult.OfferMissionRequiredRewardNotFound:
                                Logger.WriteLine("Required misstion reward not found...");
                                if (@this.CloseContactDialog)
                                {
                                    EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Close();
                                    Thread.Sleep(2000);
                                }
                                return ActionResult.Skip;
                            case MissionPickUpResult.Error:
                                tries++;
                                break;
                        }

                        if (tries > 2)
                        {
                            Logger.WriteLine("Mission not available...");
                            return ActionResult.Fail;
                        }
                    }
                }
                return ActionResult.Running;
            }
            else return ActionResult.Fail;
        }

        public void GatherInfos()
        {
            if (TargetSelectForm.TargetGuiRequest("Target mission giver and press ok."/*, Application.OpenForms.Find<Astral.Quester.Forms.Editor>()*/) == DialogResult.OK)
            {
                Entity betterEntityToInteract = Interact.GetBetterEntityToInteract();
                if (betterEntityToInteract.IsValid)
                {
                    @this.Giver = new Astral.Quester.Classes.NPCInfos()
                    {
                        CostumeName = betterEntityToInteract.CostumeRef.CostumeName,
                        DisplayName = betterEntityToInteract.Name,
                        Position = betterEntityToInteract.Location.Clone(),
                        MapName = EntityManager.LocalPlayer.MapState.MapName,
                        RegionName = EntityManager.LocalPlayer.RegionInternalName
                    };

                    // Взаимодействие с betterEntityToInteract, чтобы открыть диалоговое окно
                    if (betterEntityToInteract.CombatDistance3 <= ((@this.InteractDistance <= 0) ?
                        Math.Max(7f, betterEntityToInteract.InteractOption.InteractDistance) : Math.Max(@this.InteractDistance, betterEntityToInteract.InteractOption.InteractDistance))
                        || Approach.EntityForInteraction(betterEntityToInteract, null))
                    {
                        betterEntityToInteract.Interact();
                        Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(3000);
                        while (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Options.Count == 0)
                        {
                            if (timeout.IsTimedOut)
                            {
                                break;
                            }
                            Thread.Sleep(100);
                        }

                        //CommonTools.FocusForm(typeof(Editor));

                        string mission_id = string.Empty;
                        if (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Options.Count != 0)
                        {
                            // выбор пункта меню, соответствующего миссии
                            string adialogKey = GetAnId.GetADialogKey();
                            if (adialogKey.Length > 0)
                            {
                                // Вычисляем идентификатор миссии из строки диалога
                                // "OptionsList.MissionOffer.Az_Ee_Ed_No_Crevice_Untouched_0"

                                // Индекс начала "текстового идентификатора миссии"
                                int startInd = adialogKey.IndexOf("MissionOffer.", 0, StringComparison.OrdinalIgnoreCase);

                                // Индекс окончания "текстового идентификатора миссии"
                                // Если последний символ - цифра, значит нужно удалить суффикс
                                int lastInd = char.IsDigit(adialogKey[adialogKey.Length - 1]) ?
                                                    adialogKey.LastIndexOf('_') : adialogKey.Length - 1;

                                if (startInd >= 0 && startInd + 13 < adialogKey.Length)
                                {
                                    mission_id = adialogKey.Substring(startInd + 13, lastInd - startInd - 13);

                                    if (!string.IsNullOrEmpty(mission_id))
                                    {
                                        // Принимаем миссию
                                        EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.SelectOptionByKey(adialogKey, "");
                                        Thread.Sleep(1000);


                                        if (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.ScreenType == ScreenType.MissionOffer)
                                        {
                                            // Выбираем обязательную награду
                                            GetAnItem.ListItem rewardItem = GetAnItem.Show(4);
                                            if (rewardItem != null && !string.IsNullOrEmpty(rewardItem.ItemId))
                                            {
                                                @this.RequiredRewardItem = rewardItem.ItemId;
                                                //patternPos = PUM.RequiredRewardItem.GetSimplePatternPosition(out rewardItemPattern);
                                            }

                                            // Принимаем миссию
                                            EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.SelectOptionByKey("ViewOfferedMission.Accept");

                                            // Проверяем корректность определения "идентификатора миссии"
                                            timeout.Reset();
                                            while (!Missions.HaveMissionByPath(mission_id))
                                            {
                                                if (timeout.IsTimedOut)
                                                {
                                                    // Миссия была "принята", однако, по найденному идентификатору она "не определяется"
                                                    //XtraMessageBox.Show("Select MissionId\n" +
                                                    //    "Автоматическое определение идентификатора миссии не удалось.\n" +
                                                    //    "Укажите миссию вручную");
                                                    mission_id = GetMissionId.Show(true, mission_id, false, false);
                                                    if (!string.IsNullOrEmpty(mission_id))
                                                        @this.MissionId = mission_id;

                                                    return;
                                                }
                                                Thread.Sleep(100);
                                            }

                                            @this.MissionId = mission_id;
                                            return;
                                        }
                                    }
                                }
                            }
                        }

                        // Ручной выбор идентификатора миссии
                        mission_id = GetMissionId.Show(true, "", false, false);
                        if (!string.IsNullOrEmpty(mission_id))
                            @this.MissionId = mission_id;
                    }
                }
            }
        }

        #region Аналоги функций из Astral.Logic.NW.Missions
        /// <summary>
        /// Функция принятия миссии
        /// </summary>
        /// <param name="giver"></param>
        /// <param name="missionName"></param>
        /// <returns></returns>
        private MissionPickUpResult PickUpMission(Entity giver, string missionName)
        {
            MissionInteractResult missionInteractResult = InteractForMission(giver, missionName);

            switch (missionInteractResult)
            {
                case MissionInteractResult.Succeed:

                    // Проверяем наличие обязательных наград в окне миссии
                    if (CheckRequeredRewardItem())
                    {
                        // Принимаем миссию
                        if (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.HasOptionByKey("ViewOfferedMission.Accept"))
                        {
                            EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.SelectOptionByKey("ViewOfferedMission.Accept", "");
                            Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(5000);
                            while (!Missions.HaveMissionByPath(missionName))
                            {
                                if (timeout.IsTimedOut)
                                {
                                    return MissionPickUpResult.Error;
                                }
                                Thread.Sleep(100);
                            }
                            Thread.Sleep(500);
                            return MissionPickUpResult.MissionAccepted;
                        }
                    }
                    else
                    {
                        // В наградах отсутствуют обязательные итемы - отказываемся от миссии.
                        if (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.HasOptionByKey("ViewOfferedMission.Back"))
                        {
                            EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.SelectOptionByKey("ViewOfferedMission.Back", "");
                            Thread.Sleep(1000);

                            return MissionPickUpResult.MissionRequiredRewardNotFound;
                        }
                    }
                    break;
                case MissionInteractResult.MissionOffer:
                    if (@this.AutoAcceptOfferedMission)
                    {
                        // Проверяем наличие обязательных наград в окне миссии
                        if (CheckRequeredRewardItem())
                        {
                            // Принимаем миссию
                            if (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.HasOptionByKey("ViewOfferedMission.Accept"))
                            {
                                EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.SelectOptionByKey("ViewOfferedMission.Accept", "");
                                Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(5000);
                                while (!Missions.HaveMissionByPath(missionName))
                                {
                                    if (timeout.IsTimedOut)
                                    {
                                        return MissionPickUpResult.Error;
                                    }
                                    Thread.Sleep(100);
                                }
                                Thread.Sleep(500);
                                return MissionPickUpResult.MissionOfferAccepted;
                            }
                        }
                        else
                        {
                            // В наградах отсутствуют обязательные итемы - отказываемся от миссии.
                            if (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.HasOptionByKey("ViewOfferedMission.Back"))
                            {
                                EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.SelectOptionByKey("ViewOfferedMission.Back", "");
                                Thread.Sleep(1000);

                                if (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.ScreenType != ScreenType.List)
                                    return MissionPickUpResult.Error;
                                return MissionPickUpResult.OfferMissionRequiredRewardNotFound;
                            }
                        }
                    }
                    break;
                default:
                    return MissionPickUpResult.Error;
            }
            return MissionPickUpResult.Error;
        }

        /// <summary>
        /// Функция взаимодействия с квестодателем
        /// в случа необходимости задействуется навигационная система для перемещения персонажа к месту нахождения квестодателя
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="missionName"></param>
        /// <returns></returns>
        private MissionInteractResult InteractForMission(Entity entity, string missionName)
        {
            Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(5000);
            // Попытка открыть экран принятия заданной миссии
            ContactDialogOption missionOption = GetMissionOption(missionName);
            if (!missionOption.IsValid)
            {
                // Диалоговое окно открыто, но экран заданной миссии открыть не удалось 
                // поэтому закрываем диалоговое окно для новой попытки
                //if (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.IsValid)
                //{
                //    Astral.Logic.NW.General.CloseContactDialog();
                //}

                // Проверяем карту и регион
                if (@this.Giver.MapName == EntityManager.LocalPlayer.MapState.MapName
                    && @this.Giver.RegionName == EntityManager.LocalPlayer.RegionInternalName)
                {
                    // Проверяем расстояние до цели и возможность взаимодействия в квестодателем
                    if (/*!entity.CanInteract || */(entity.CombatDistance3 > ((@this.InteractDistance == 0) ?
                        Math.Max(7f, entity.InteractOption.InteractDistance) : Math.Max(@this.InteractDistance, entity.InteractOption.InteractDistance))))
                        // Включение навигации с целью приближения к персонажу и вступления с ним во взаимодействие
                        if (!Approach.EntityForInteraction(entity, null))
                            return MissionInteractResult.Error;
                }
                else return MissionInteractResult.Error;

                // Взаимодействие с персонажем для открытия диалогового окна
                entity.Interact();
                timeout.Reset();
                while (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Options.Count == 0)
                {
                    if (timeout.IsTimedOut)
                    {
                        return MissionInteractResult.Error;
                    }
                    Thread.Sleep(100);
                }
                Thread.Sleep(500);
                // Попытка ищем пункт диалога, связанного с заданной миссией
                missionOption = GetMissionOption(missionName);
            }

            if (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.ScreenType == ScreenType.MissionOffer
                && @this.AutoAcceptOfferedMission)
            {
                return MissionInteractResult.MissionOffer;
            }

            // Проверяем результаты поиска соответствующего миссии пункта диалога
            if (!missionOption.IsValid)
            {
                // Соответствующий пункт диалога не найден
                // Экран принятия миисии не обнаружен
                return MissionInteractResult.MissionNotFound;
            }
            // Активируем пункт диалога, связанный с нужной миссией
            missionOption.Select();
            timeout.Reset();
            while (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.ScreenType != ScreenType.MissionOffer)
            {
                if (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.ScreenType == ScreenType.MissionTurnIn)
                {
                    break;
                }
                if (timeout.IsTimedOut)
                {
                    return MissionInteractResult.Error;
                }
                Thread.Sleep(100);
            }
            Thread.Sleep(750);
            return MissionInteractResult.Succeed;
        }

        /// <summary>
        /// Функция выброра пункта диалога, связанного с заданной миссией
        /// </summary>
        /// <param name="missionName"></param>
        /// <returns></returns>
        private ContactDialogOption GetMissionOption(string missionName)
        {
            Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(5000);
            while (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.IsValid
                    && !timeout.IsTimedOut)
            {
                // Диалоговое окно открыто
                //Проверяем статус экрана 
                if (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.ScreenType == ScreenType.MissionOffer)
                {
                    // Открыт экран принятия миссии
                    // На экране принятия миссии узнать из АПИ, к какой миссии он относится, - нельзя
                    if (@this.AutoAcceptOfferedMission)
                    {
                        // Активирована опция автопринятия предлагаемой миссии
                        return new ContactDialogOption(IntPtr.Zero);
                    }

                    // поэтому отказываемся принять миссию, чтобы вернуться к списку
                    if (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.HasOptionByKey("ViewOfferedMission.Back"))
                    {
                        EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.SelectOptionByKey("ViewOfferedMission.Back", "");
                        while (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.ScreenType == ScreenType.MissionOffer)
                        {
                            if (timeout.IsTimedOut)
                            {
                                return new ContactDialogOption(IntPtr.Zero);
                            }
                            Thread.Sleep(100);
                        }
                    }
                }
                if (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.ScreenType == ScreenType.Buttons)
                {
                    // Обычный разговор
                    // ContactDialog.ScreenType = Buttons
                    // Не содержит 
                    // ContactDialog.Options[].Key:
                    // SpecialDialog.action_1 - Цифра означает пункт диалога. 
                    // Последний пункт "обычно" соответствует возврату в предыдущее меню
                    if (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Options.Count > 0
                        && EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.HasOptionByKey("SpecialDialog.action_" + (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Options.Count - 1)))
                    {
                        // Пытаемся вернуться к предыдущему пункту вызвав последний пункт диалога
                        EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.SelectOptionByKey("SpecialDialog.action_" + (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Options.Count - 1), "");
                        while (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.ScreenType == ScreenType.Buttons)
                        {
                            if (timeout.IsTimedOut)
                            {
                                return new ContactDialogOption(IntPtr.Zero);
                            }
                            Thread.Sleep(100);
                        }
                    }
                }
                if (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.ScreenType == ScreenType.List)
                {
                    // Открыт экран списка пунктов диалога
                    // ContactDialog.ScreenType = List
                    // ContactDialog.Options[].Key:
                    // OptionList.Exit - Закрыть диалоговое окно

                    using (/*List<ContactDialogOption>.Enumerator*/ var enumerator = EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Options.GetEnumerator())
                    {
                        // Открыт экран со списком ответов
                        // Ищем пункт диалога, соответствующий заданной миссии
                        while (enumerator.MoveNext())
                        {
                            ContactDialogOption contactDialogOption = enumerator.Current;
                            if (!contactDialogOption.CannotChoose && (contactDialogOption.Key.Contains(missionName + "_") || contactDialogOption.Key.EndsWith(missionName)))
                            {
                                return contactDialogOption;
                            }
                        }

                        // Пункт диалога, соответствующий миссии не найден
                        return new ContactDialogOption(IntPtr.Zero);
                    }
                }
            }
            return new ContactDialogOption(IntPtr.Zero);
        }
        
        /// <summary>
        /// Проверка наличия в наградах заданного итема
        /// </summary>
        /// <returns></returns>
        private bool CheckRequeredRewardItem()
        {
            if (patternPos == SimplePatternPos.None)
            {
                if (string.IsNullOrEmpty(@this.RequiredRewardItem) || string.IsNullOrWhiteSpace(@this.RequiredRewardItem))
                    return true;

                patternPos = @this.RequiredRewardItem.GetSimplePatternPosition(out rewardItemPattern);
            }

            // Проверяем тип активного экрана диалогового окна
            if (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.ScreenType == ScreenType.MissionOffer)
            {
                // Открыт экран получения квеста
                // Проверяем наличие в сумках с наградой заданного итема
                foreach (InventoryBag bag in EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.RewardBags)
                    foreach (InventorySlot iSlot in bag.GetItems)
                    {
                        if (iSlot.Item.ItemDef.InternalName.CompareToSimplePattern(patternPos, rewardItemPattern))
                            return true;
                    }
            }

            return false;
        }
        #endregion

        public bool Validate()
        {
            return !Missions.MissionIsCompletedByPath(@this.MissionId)
                && !Missions.HaveMissionByPath(@this.MissionId)
                && @this.Giver.MapName == EntityManager.LocalPlayer.MapState.MapName
                && @this.Giver.RegionName == EntityManager.LocalPlayer.RegionInternalName;
        }

        public void Reset()
        {
            /*patternPos = SimplePatternPos.None;
            rewardItemPattern = string.Empty;*/
            tries = 0;
        }

        public string Label()
        {
            if (string.IsNullOrEmpty(label))
                label = $"{@this.GetType().Name}: [{@this.MissionId}]";
            return label;
        }
    }
}