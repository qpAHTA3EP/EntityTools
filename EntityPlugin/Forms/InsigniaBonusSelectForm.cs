﻿using DevExpress.XtraEditors;
using EntityTools.Tools.MountInsignias;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using InsigniaSet = EntityTools.Tools.Triple<EntityTools.Tools.MountInsignias.InsigniaType, EntityTools.Tools.MountInsignias.InsigniaType, EntityTools.Tools.MountInsignias.InsigniaType>;

namespace EntityTools.Forms
{
    public partial class InsigniaBonusSelectForm : XtraForm //*/Form
    {
        private static InsigniaBonusSelectForm insigniaBonusesForm;

        /// <summary>
        /// Полный список бонусов скакунов
        /// </summary>
        private BindingList<MountBonusDef> mountBonuses;

        public static MountBonusDef GetMountBonuses(MountBonusDef bonus = null)
        {
            if (insigniaBonusesForm == null)
                insigniaBonusesForm = new InsigniaBonusSelectForm();

            if(bonus != null)
            {
                for(int i =0; i< insigniaBonusesForm.BonusList.Items.Count; i++)
                {
                    MountBonusDef itemBonus = insigniaBonusesForm.BonusList.Items[i] as MountBonusDef;
                    if (itemBonus != null && itemBonus.InternalName == bonus.InternalName)
                    {
                        insigniaBonusesForm.BonusList.SelectedIndex = i;
                        break;
                    }
                }
            }

            if(insigniaBonusesForm.ShowDialog() == DialogResult.OK)
            {
                MountBonusDef curBonus = insigniaBonusesForm.BonusList.SelectedItem as MountBonusDef;

                return curBonus;
            }

            return null;
        }

        public InsigniaBonusSelectForm()
        {
            InitializeComponent();

            string fileName = Path.Combine(Astral.Controllers.Directories.SettingsPath, typeof(EntityTools).Name, "MountBonuses.xml");


            if (!File.Exists(fileName))
            {
                // Файл списка бонусов ОТСУТСТВУЕТ
                // Создаем копию файла списка бонусов из ресурса
                Directory.CreateDirectory(fileName);
                File.WriteAllText(fileName, Properties.Resources.MountBonuses);
            }

            if (File.Exists(fileName))
            {
                // Файл найден списка бонусов
                XmlSerializer serializer = new XmlSerializer(typeof(BindingList<MountBonusDef>));
                using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
                    mountBonuses = serializer.Deserialize(fs) as BindingList<MountBonusDef>;

                if (mountBonuses != null)
                {
                    BonusList.DataSource = mountBonuses;
                    BonusList.DisplayMember = nameof(MountBonusDef.Name);
                    return;
                }
            }
            else
            {
                BonusList.DataSource = null;
                BonusList.DisplayMember = String.Empty;
            }
        }

        private void BonusList_SelectedIndexChanged(object sender, EventArgs e)
        {
            MountBonusDef currentBonus = BonusList.SelectedItem as MountBonusDef;
            
            if(currentBonus != null)
            {
                Description.Text = currentBonus.Description;

                InsigniaSet iSet = currentBonus.GetInsigniaSet();
                SetInsigniaPicture(Insignia1, iSet.First);
                SetInsigniaPicture(Insignia2, iSet.Second);
                SetInsigniaPicture(Insignia3, iSet.Third);
            }
            else
            {
                Description.Text = string.Empty;
                Insignia1.Image = null;
                Insignia2.Image = null;
                Insignia3.Image = null;
            }

        }

        private void tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void SetInsigniaPicture(PictureBox pb, InsigniaType iType)
        {
            if (pb == null)
                return;

            switch(iType)
            {
                case InsigniaType.Barbed:
                    pb.Image = Properties.Resources.Insignia_Barbed;
                    return;
                case InsigniaType.Crescent:
                    pb.Image = Properties.Resources.Insignia_Crescent;
                    return;
                case InsigniaType.Illuminated:
                    pb.Image = Properties.Resources.Insignia_Illuminated;
                    return;
                case InsigniaType.Enlightened:
                    pb.Image = Properties.Resources.Insignia_Enlightened;
                    return;
                case InsigniaType.Regal:
                    pb.Image = Properties.Resources.Insignia_Regal;
                    return;
                default:
                    pb.Image = null;
                    return;
            }
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            int barb = 0,
                cres = 0,
                illum = 0,
                enl = 0,
                reg = 0;

            if (!string.IsNullOrEmpty(tbBarbed.Text))
                int.TryParse(tbBarbed.Text, out barb);
            if (!string.IsNullOrEmpty(tbCrescent.Text))
                int.TryParse(tbCrescent.Text, out cres);
            if (!string.IsNullOrEmpty(tbllluminated.Text))
                int.TryParse(tbllluminated.Text, out illum);
            if (!string.IsNullOrEmpty(tbEnlightened.Text))
                int.TryParse(tbEnlightened.Text, out enl);
            if (!string.IsNullOrEmpty(tbRegal.Text))
                int.TryParse(tbRegal.Text, out reg);


            if (barb > 0 
                || cres > 0 
                || illum > 0                
                || enl > 0
                || reg > 0)
            {
                var filteredBonusList = from bonDef in mountBonuses
                                           where  (barb == 0  || bonDef.Barbed == barb) 
                                               && (cres == 0  || bonDef.Crescent == cres)
                                               && (illum == 0 || bonDef.Illuminated == illum)
                                               && (enl == 0   || bonDef.Enlightened == enl)
                                               && (reg == 0   || bonDef.Regal == reg)
                                        select bonDef;
                bindingSource.DataSource = filteredBonusList.ToList();
                BonusList_SelectedIndexChanged(BonusList, new EventArgs());
            }
            else BonusList.DataSource = mountBonuses;
        }
    }
}
