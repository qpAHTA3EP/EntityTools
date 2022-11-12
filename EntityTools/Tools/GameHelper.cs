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
            foreach (var uiGen in UIManager.AllUIGen)
            {
                if(uiGen.IsVisible)
                {
                    if (uiGen.Type == MyNW.Patchables.Enums.UIGenType.Button)
                    {
                        switch (uiGen.Name)
                        {
                            case "Gamemenu_Return": // Кнопка возврата из "игрового меню"
                                GameCommands.Execute("GenButtonClick Gamemenu_Return");
                                break;
                            case "Eventdetails_Cancelbutton": // Кнопка закрытия окна "детали эвента"
                                GameCommands.Execute("GenButtonClick Eventdetails_Cancelbutton");
                                break;
                        } 
                    }
                    else if(uiGen.Type == MyNW.Patchables.Enums.UIGenType.MovableBox)
                    {
                        switch(uiGen.Name)
                        {
                            case "Invocationresults_Root": // Окно награды за молитву
                                GameCommands.Execute("GenSendMessage Invocationresults_Root Close");
                                break;
                            case "Store_Invocation":    // Окно Молельни, где можно потратить монеты
                                GameCommands.Execute("GenSendMessage Store_Invocation Close");
                                break;
                        }
                    }
                }
            }

        }
        public static void CloseAllFrames()
        {
            CloseSpecialFrames();
            var contactDialog = EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog;
            if (contactDialog.IsValid)
            {
                contactDialog.Close();
            }
            GameCommands.Execute("ContactDialogEnd");

            Game.ToggleCursorMode(false);
        }
    }
}
