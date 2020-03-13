using Astral.Forms;
using Astral.Logic.UCC.Forms;
using EntityTools.Patches;
using EntityTools.Tools;
using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Reflection;
using EntityTools.Core;
using EntityTools.Core.Interfaces;

namespace EntityTools
{
    public class EntityTools : Astral.Addons.Plugin
    {
        public static IEntityToolsCore Core { get; }

        public override string Name => "Entity Tools";
        public override string Author => "MichaelProg";
        public override System.Drawing.Image Icon => Properties.Resources.EntityIcon;
        private BasePanel panel = null;
        public override BasePanel Settings
        {
            get
            {
                if (panel == null)
                    panel = new Core.EntityToolsMainPanel();
                return panel;
            }
        }

        public static SettingsContainer PluginSettings { get; set; } = new SettingsContainer();

        public override void OnBotStart()
        {
            Services.UnstuckSpells.Active = EntityTools.PluginSettings.UnstuckSpells.Active;

#if PROFILING && DEBUG
            InteractEntities.ResetWatch();
            //InteractEntitiesCached.ResetWatch();
            //InteractEntitiesCachedTimeout.ResetWatch();
            EntitySelectionTools.ResetWatch();
            SearchCached.ResetWatch();
            EntityCache.ResetWatch();
            EntityCacheRecord.ResetWatch();
            SearchDirect.ResetWatch();
#endif
        }

        public override void OnBotStop()
        {
#if PROFILING && DEBUG
            InteractEntities.LogWatch();
            //InteractEntitiesCached.LogWatch();
            //InteractEntitiesCachedTimeout.LogWatch();
            EntitySelectionTools.LogWatch();
            SearchCached.LogWatch();
            EntityCache.LogWatch();
            EntityCacheRecord.LogWatch();
            SearchDirect.LogWatch();
#endif
        }

        public override void OnLoad()
        {
            Assembly.Load(Properties.Resources.EntityCore);
            LoadSettings();
            //if (!File.Exists(@".\Logs\Assemplies.log"))
            //    File.Create(@".\Logs\Assemplies.log");

            using (StreamWriter file = new StreamWriter(@".\Logs\Assemblies.log", false))
            {
                foreach(Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    file.WriteLine("=================================================================");
                    file.WriteLine(a.FullName);
                    file.WriteLine("-----------------------------------------------------------------");
                    foreach(Type t in a.GetTypes())
                    {
                        file.Write('\t');
                        file.WriteLine(t.FullName);
                    }
                }
            }

            //Patcher.Apply();
        }

        public override void OnUnload()
        {
            SaveSettings();
        }

        /// <summary>
        /// Сохранение настроек в файл
        /// </summary>
        private void LoadSettings()
        {
            if (!Directory.Exists(Path.GetDirectoryName(FileTools.SettingsFile)))
                Directory.CreateDirectory(Path.GetDirectoryName(FileTools.SettingsFile));

            try
            {
                if (File.Exists(FileTools.SettingsFile))
                {
                    XmlSerializer serialiser = new XmlSerializer(PluginSettings.GetType());
                    using (StreamReader fileStream = new StreamReader(FileTools.SettingsFile))
                    {
                        if (serialiser.Deserialize(fileStream) is SettingsContainer settings)
                        {
                            PluginSettings = settings;
                            Astral.Logger.WriteLine($"{GetType().Name}: Load settings from {Path.GetFileName(FileTools.SettingsFile)}");
                        }
                        else
                        {
                            PluginSettings = new SettingsContainer();
                            Astral.Logger.WriteLine($"{GetType().Name}: Settings file not found. Use default");
                        }
                    }
                }
            }
            catch
            {
                PluginSettings = new SettingsContainer();
                Astral.Logger.WriteLine($"{GetType().Name}: Error load settings file {Path.GetFileName(FileTools.SettingsFile)}. Use default");
            }
        }

        /// <summary>
        /// Сохранение свойств
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SaveSettings(object sender = null, EventArgs e = null)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(FileTools.SettingsFile)))
                    Directory.CreateDirectory(Path.GetDirectoryName(FileTools.SettingsFile));

                XmlSerializer serialiser = new XmlSerializer(/*typeof(SettingsContainer)*/PluginSettings.GetType());
                using (TextWriter FileStream = new StreamWriter(FileTools.SettingsFile, false))
                {
                    serialiser.Serialize(FileStream, PluginSettings);
                }
            }
            catch (Exception exp)
            {
                Astral.Logger.WriteLine($"{GetType().Name}: Error to save settings file {Path.GetFileName(FileTools.SettingsFile)}");
                Astral.Logger.WriteLine(exp.ToString());
            }
        }

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

        #region Новый обработчика кнопки вызова редактора UCC
        private void ucc_Editor_Click(object sender, EventArgs e)
        {
            MessageBox.Show("ucc_Editor_Click(...)");

            Editor uccEditor = new Editor(Astral.Logic.UCC.Core.Get.mProfil, false);
            uccEditor.ShowDialog();
        }
        #endregion
        #endregion
    }
}
