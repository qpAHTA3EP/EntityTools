using System;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;

namespace EntityTools.Tools.Missions
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
        /// Закрытие всех специальный диалоговых окон: Аукцион, Почта, Молельня, Награды
        /// </summary>
        public static void CloseSpecialFrames()
        {
            if (Auction.IsAuctionFrameVisible())
            {
                Auction.CloseAuctionFrame();
            }
            if (Email.IsMailFrameVisible())
            {
                Email.CloseMailFrame();
            }
            if (Game.IsRewardpackviewerFrameVisible())
            {
                Game.CloseRewardpackviewerFrame();
            }
            if (Game.IsInvocationResultsFrameVisible())
            {
                Game.CloseInvocationResultsFrame();
            }

            // Кнопка закрытия окна "детали эвента"
            // Eventdetails_Cancelbutton

        }
        public static void CloseAllFrames()
        {
            CloseSpecialFrames();
            var contactDialog = EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog;
            if (contactDialog.IsValid)
            {
                contactDialog.Close();
            }
            Game.ToggleCursorMode(false);
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
        public static bool HaveMission(string missionId, out MissionState state)
        {
            state = MissionState.Dropped;
            var missionParts = missionId.Split('/');

            if (missionParts.Length == 0)
                return false;

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
            {
                state = mission.State;
                return true;
            }

            return false;
        }
    }
}
