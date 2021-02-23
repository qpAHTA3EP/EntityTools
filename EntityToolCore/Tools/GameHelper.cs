using MyNW.Internals;
using MyNW.Classes;
using System;

namespace EntityCore.Tools
{
    public static class GameHelper
    {
        /// <summary>
        /// Пустой предмет MyNW.Classes.Item (заглушка)
        /// </summary>
        public static readonly Item EmptyItem = new Item(IntPtr.Zero);

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
    }
}
