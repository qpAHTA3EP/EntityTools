using DevExpress.XtraEditors;
using EntityCore.MountInsignias;
using EntityTools.Tools;
using EntityCore.MountInsignias;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace EntityCore.Forms
{
    public partial class MountBonusPriorityListForm : XtraForm //*/Form
    {
        /// <summary>
        /// Список приоритетов бонусов знаков для каждого класса
        /// </summary>
        private BindingList<ParagonMountBonusPriorityDef> MountBonusPriorityList;
        //private Dictionary<PlayerClassParagonType, MountBonusPriorityDef> MountBonusPriorityList = new Dictionary<PlayerClassParagonType, MountBonusPriorityDef>();

        private static MountBonusPriorityListForm bonusListForm;

        public static void GetBonusList()
        {
            if (bonusListForm == null)
                bonusListForm = new MountBonusPriorityListForm();

            bonusListForm.Show();
        }

        /// <summary>
        /// Словарь соответствия индексов элементов компонента PlayersClass
        /// и типов PlayerClassParagonType
        /// </summary>
        private Dictionary<int, PlayerClassParagonType> PlayerClassIndexer = new Dictionary<int, PlayerClassParagonType>();

        public MountBonusPriorityListForm()
        {
            InitializeComponent();


            string fileName = Path.Combine(Astral.Controllers.Directories.SettingsPath, "EntityTools", "MountBonusPriorityList.xml");


            dgvBonusPriorityList.AutoGenerateColumns = false;
            if (File.Exists(fileName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(BindingList<ParagonMountBonusPriorityDef>));
                using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
                    MountBonusPriorityList = serializer.Deserialize(fs) as BindingList<ParagonMountBonusPriorityDef>;
            }
            else
            {
                // Файл списка прионитетов отсутствует
                // Формируем пустой список приоритетов
                MountBonusPriorityList = new BindingList<ParagonMountBonusPriorityDef>();
                foreach (CharacterPath paragon in Game.CharacterPaths)
                {
                    Enum.TryParse(paragon.Name, out PlayerClassParagonType paragonType);
                    MountBonusPriorityList.Add(new ParagonMountBonusPriorityDef(paragonType));
                }
            }
            
            if (MountBonusPriorityList != null)
            {
                cbPlayersClass.DataSource = MountBonusPriorityList;
                cbPlayersClass.DisplayMember = nameof(ParagonMountBonusPriorityDef.DisplayName);
                for(int i = 0; i < cbPlayersClass.Items.Count; i++)
                {
                    ParagonMountBonusPriorityDef curBonList = cbPlayersClass.Items[i] as ParagonMountBonusPriorityDef;
                    if (curBonList!= null
                        && EntityManager.LocalPlayer.Character?.CurrentPowerTreeBuild?.SecondaryPaths[0]?.Path?.PowerTree?.Name == curBonList.ClassParagonType.ToString())
                    {
                        cbPlayersClass.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private void cbPlayersClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            ParagonMountBonusPriorityDef curBonList = cbPlayersClass.SelectedItem as ParagonMountBonusPriorityDef;
            dgvBonusPriorityList.DataSource = curBonList.MountBonusPriorityList;
            clmnBonusName.DataPropertyName = nameof(MountBonusPriorityDef.Name);
            clmnNumber.DataPropertyName = nameof(MountBonusPriorityDef.Number);
        }

        private void dgvBonusPriorityList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == clmnButton.DisplayIndex
                || e.ColumnIndex == clmnBonusName.DisplayIndex)
            {
                MountBonusPriorityDef bonusDef = dgvBonusPriorityList.Rows[e.ColumnIndex].DataBoundItem as MountBonusPriorityDef;
                MountBonusDef newBonusDef = InsigniaBonusSelectForm.GetMountBonuses(bonusDef?.Bonus);
                if (newBonusDef != null && !bonusDef.Bonus.Equals(newBonusDef))
                    bonusDef.Bonus = newBonusDef;
            }

        }
    }
}
