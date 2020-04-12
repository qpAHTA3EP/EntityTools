using Astral.Classes;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EntityTools.Tools
{
    public static class EnchantHelper
    {
        //private Timeout timeout = new Timeout(1000);
        private static Task task = null;

        public static int CheckingTime { get; set; } = 250;


        // Элемент GUI: пункт контекстного меню "Зачаровать предмет" в окне персонажа
        private static UIGen Equippeditemmenu_Enchant = null;
        // Элемент GUI: пункт контекстного меню "Зачаровать предмет" в сумке инвентаря
        private static UIGen Itemmenu_Enchant = null;
        // Элемент GUI: пункт контекстного меню "Извлечь целым" в окне "Наложение чар"
        private static UIGen Enchantmenu_Itemremoveintact = null;
        // Элемент GUI: кнопка подтверждение в окне предупреждения о взимании платы за извлечение волшебного камня
        private static UIGen Enchantitem_Unslotdialog_Resources = null;
        // Элемент GUI: кнопка "Зачаровать" в окне "Наложение чар"
        private static UIGen Enchantitem_Commit = null;
        // Элемент GUI: кнопка подтверждения в окне предупреждения о бесплатной вставке волшебного камня и о взимании платы за его извлечение
        private static UIGen Modaldialog_Ok = null;


        private static bool enabled;
        public static bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                if (!enabled)
                {
                    enabled = value;
                    if (enabled && task.Status != TaskStatus.Running)
                    {
                        task = Task.Factory.StartNew(Run);
                        Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"EnchantHelper: Start new background Task");
                    }
                }
                else enabled = value;
            }
        }

        private static void Run()
        {
            while (enabled)
            {
                // Активация пункта контекстного меню "Зачаровать предмет" в окне пероснажа
                if (Equippeditemmenu_Enchant == null || !Equippeditemmenu_Enchant.IsValid)
                    Equippeditemmenu_Enchant = UIManager.GetUIGenByName("Equippeditemmenu_Enchant");
                if (Equippeditemmenu_Enchant != null && Equippeditemmenu_Enchant.IsValid && Equippeditemmenu_Enchant.IsVisible)
                {
                    GameCommands.Execute("GenButtonClick Equippeditemmenu_Enchant");
                    Thread.Sleep(100);
                }

                // Активация пункта контекстного меню "Зачаровать предмет" в сумке инвентаря
                if (Itemmenu_Enchant == null || !Itemmenu_Enchant.IsValid)
                    Itemmenu_Enchant = UIManager.GetUIGenByName("Itemmenu_Enchant");
                if (Itemmenu_Enchant != null && Itemmenu_Enchant.IsValid && Itemmenu_Enchant.IsVisible)
                {
                    GameCommands.Execute("GenButtonClick Itemmenu_Enchant");
                    Thread.Sleep(100);
                }

                // Активация пункта контекстного меню "Извлечь целым" в окне "Наложение чар"
                if (Enchantmenu_Itemremoveintact == null || !Enchantmenu_Itemremoveintact.IsValid)
                    Enchantmenu_Itemremoveintact = UIManager.GetUIGenByName("Enchantmenu_Itemremoveintact");
                if (Enchantmenu_Itemremoveintact != null && Enchantmenu_Itemremoveintact.IsValid && Enchantmenu_Itemremoveintact.IsValid)
                {
                    GameCommands.Execute("GenButtonClick Enchantmenu_Itemremoveintact");
                    Thread.Sleep(100);

                    // Активация кнопки подтверждения
                    if (Enchantitem_Unslotdialog_Resources == null || !Enchantitem_Unslotdialog_Resources.IsValid)
                        Enchantitem_Unslotdialog_Resources = UIManager.GetUIGenByName("Enchantitem_Unslotdialog_Resources");
                    if (Enchantitem_Unslotdialog_Resources != null && Enchantitem_Unslotdialog_Resources.IsValid && Enchantitem_Unslotdialog_Resources.IsVisible)
                    {
                        GameCommands.Execute("GenButtonClick Enchantitem_Unslotdialog_Resources");
                        Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"EnchantHelper: Enchant unsloted");
                    }
                }


                // Активация кнопка "Зачаровать" в окне "Наложение чар"
                if (Enchantitem_Commit == null || !Enchantitem_Unslotdialog_Resources.IsValid)
                    Enchantitem_Commit = UIManager.GetUIGenByName("Enchantitem_Commit");
                if (Enchantitem_Commit != null && Enchantitem_Commit.IsValid && Enchantitem_Commit.IsVisible)
                {
                    GameCommands.Execute("GenButtonClick Enchantitem_Commit");
                    Thread.Sleep(100);

                    // Активация кнопки подтверждения
                    if (Modaldialog_Ok == null || !Modaldialog_Ok.IsValid)
                        Modaldialog_Ok = UIManager.GetUIGenByName("Modaldialog_Ok");
                    if (Modaldialog_Ok != null && Modaldialog_Ok.IsValid && Modaldialog_Ok.IsVisible)
                    {
                        GameCommands.Execute("GenButtonClick Modaldialog_Ok");
                        Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"EnchantHelper: Enchant sloted");
                    }

                }

                Thread.Sleep(CheckingTime);
            }

            Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"EnchantHelper: Background Task stoped");

            Equippeditemmenu_Enchant = null;
            Enchantmenu_Itemremoveintact = null;
            Enchantitem_Unslotdialog_Resources = null;
            Enchantitem_Commit = null;
            Modaldialog_Ok = null;

            task = null;
        }
    }
}
