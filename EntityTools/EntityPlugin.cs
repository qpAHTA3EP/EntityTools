using Astral.Forms;
using Astral.Logic.UCC;
using Astral.Logic.UCC.Forms;
using EntityTools.Actions;
using EntityTools.Tools;
using EntityTools.Tools.Entities;
using System;
using System.Threading;
using System.Windows.Forms;

namespace EntityTools
{
    public class EntityTools : Astral.Addons.Plugin
    {
        //public EntityPluginSettings PluginSettings {get;} = new EntityPluginSettings();

        public EntityTools() : base()
        {
            DebugInfoEnabled = false;
        }
        public static bool DebugInfoEnabled { get; set; }
        public override string Name => "Entity Tools";
        public override string Author => "MichaelProg";
        public override System.Drawing.Image Icon => Properties.Resources.EntityIcon;
        private BasePanel panel = null;
        public override BasePanel Settings
        {
            get
            {
                if (panel == null)
                    panel = new Forms.MainPanel();
                return panel;
            }
        }

        public override void OnBotStart()
        {
#if PROFILING
            InteractEntities.ResetWatch();
            InteractEntitiesCached.ResetWatch();
            InteractEntities.ResetWatch();
            EntitySelectionTools.ResetWatch();
            SearchCached.ResetWatch();
            EntityCache.ResetWatch();
            EntityCacheRecord.ResetWatch();
            SearchDirect.ResetWatch();
#endif
        }

        public override void OnBotStop()
        {
#if PROFILING
            InteractEntities.LogWatch();
            InteractEntitiesCached.LogWatch();
            InteractEntities.LogWatch();
            EntitySelectionTools.LogWatch();
            SearchCached.LogWatch();
            EntityCache.LogWatch();
            EntityCacheRecord.LogWatch();
            SearchDirect.LogWatch();
#endif
        }

        public override void OnLoad()
        {
            States.SpellStuckMonitor.Activate = true;
            States.SlideMonitor.Activate = false;
        }

        public override void OnUnload() { }

        #region Подмена обработчика кнопки "Combats"
        //// Старый обработчик из Astral.Forms.Panels.Main
        ////private void \u0003(object \u0002, TileItemEventArgs \u0003)
        ////{
        ////	for (;;)
        ////	{
        ////		if (!false)
        ////		{
        ////			if (Roles.CurrentRole.Name == "Professions")
        ////			{
        ////				goto IL_10;
        ////			}
        ////          Forms.ShowPanel(new \u001E.\u0003());
        ////			if (!false)
        ////			{
        ////				return;
        ////			}
        ////			goto IL_10;
        ////		}
        ////		IL_18:
        ////		if (7 == 0)
        ////		{
        ////			continue;
        ////		}
        ////		if (8 != 0)
        ////		{
        ////			break;
        ////		}
        ////		IL_10:
        ////		XtraMessageBox.Show("Not configurable with professions role.");
        ////		goto IL_18;
        ////	}
        ////}

        //// Фрагмент инициализации "кнопки" "Combats"
        ////this.\u0007.AppearanceItem.Normal.BackColor = Color.MediumPurple;
        ////this.\u0007.AppearanceItem.Normal.BackColor2 = Color.SlateBlue;
        ////this.\u0007.AppearanceItem.Normal.BorderColor = Color.MediumPurple;
        ////this.\u0007.AppearanceItem.Normal.Options.UseBackColor = true;
        ////this.\u0007.AppearanceItem.Normal.Options.UseBorderColor = true;
        ////tileItemElement3.Image = \u0001.combat;
        ////tileItemElement3.ImageAlignment = 5;
        ////tileItemElement3.Text = "Combats";
        ////this.\u0007.Elements.Add(tileItemElement3);
        ////this.\u0007.Name = "tileItem3";
        ////this.\u0007.ItemClick += new TileItemClickEventHandler(this.\u0003);

        ///// <summary>
        ///// Главная форма Astral'a
        ///// </summary>
        //private Astral.Forms.Main mainForm = null;
        ///// <summary>
        ///// Главная панель Astral'a (Main)
        ///// </summary>
        //private Astral.Forms.Panels.Main mainPanel = null;

        //public void SubscribeEvent_MainPanel_CombatTile_Click()
        //{
        //    // Поиск главной формы 
        //    do
        //    {
        //        foreach (Form form in Application.OpenForms)
        //        {
        //            if (form is Astral.Forms.Main)
        //            {
        //                mainForm = (Astral.Forms.Main)form;
        //                break;
        //            }
        //        }
        //        Thread.Sleep(500);
        //    }
        //    while (mainForm != null);

        //    // поиск главной панели
        //    if (mainForm != null)
        //    {
        //        do
        //        {
        //            if(mainForm.main)
        //            Thread.Sleep(500);
        //        }
        //        while (mainPanel != null);
        //    }
        //}
        #endregion

        #region Новый обработчика кнопки вызова редактора UCC
        private void ucc_Editor_Click(object sender, EventArgs e)
        {
            MessageBox.Show("ucc_Editor_Click(...)");

            Editor uccEditor = new Editor(Core.Get.mProfil, false);
            uccEditor.ShowDialog();
        }
        #endregion
    }
}
