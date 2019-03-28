using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EntityPlugin.Forms
{
    public partial class EntitySelectForm : Form
    {
        /// <summary>
        /// краткое описание объекта Entity
        /// </summary>
        public class EntityDif
        {
            public IntPtr ptr = IntPtr.Zero;
            public string Name = string.Empty;
            public string NameUntranslated = string.Empty;
            public string InternalName = string.Empty;
            public double Distance = 0;
        }

        /// <summary>
        /// локальный список кратких описаний Entity
        /// </summary>
        private List<EntityDif> entityDifs = new List<EntityDif>();

        /// <summary>
        /// Создание списка кратких описаний объектов Entity
        /// </summary>
        private void FillEntityDifs()
        {
            entityDifs.Clear();

            foreach(Entity entity in EntityManager.GetEntities())
            {
                entityDifs.Add(new EntityDif() {
                        ptr = entity.Pointer,
                        Name = entity.Name,
                        NameUntranslated = entity.NameUntranslated,
                        InternalName = entity.InternalName,
                        Distance = entity.Location.Distance3DFromPlayer
                    });
            }
        }

        private static EntitySelectForm selectForm;

        public EntitySelectForm()
        {
            InitializeComponent();
        }

        public static Entity GetEntity()
        {
            if(selectForm == null)
                selectForm = new EntitySelectForm();

            selectForm.dgvEntities.AutoGenerateColumns = false;
            selectForm.dgvEntities.DataSource = EntityManager.GetEntities();

            selectForm.clmnName.DataPropertyName = "Name";
            selectForm.clmnInternalName.DataPropertyName = "InternalName";
            selectForm.clmnNameUntranslated.DataPropertyName = "NameUntranslated";
                       
            DialogResult dialogResult = selectForm.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                if (selectForm.dgvEntities.CurrentRow.DataBoundItem is Entity selectedEntity)
                    return selectedEntity;
            }
            return new Entity(IntPtr.Zero);
        }

        public static EntityDif GetEntity(string entityUntrName)
        {
            if (selectForm == null)
                selectForm = new EntitySelectForm();

            selectForm.dgvEntities.Rows.Clear();

            int currentInd = -1;
            foreach (Entity entity in EntityManager.GetEntities())
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(selectForm.dgvEntities);
                row.Cells[selectForm.clmnName.DisplayIndex].Value = entity.Name;
                row.Cells[selectForm.clmnNameUntranslated.DisplayIndex].Value = entity.NameUntranslated;
                row.Cells[selectForm.clmnInternalName.DisplayIndex].Value = entity.InternalName;
                row.Cells[selectForm.clmnDistance.DisplayIndex].Value = entity.Location.Distance3DFromPlayer;
                row.Tag = entity;

                int ind = selectForm.dgvEntities.Rows.Add(row);

                if (entity.NameUntranslated == entityUntrName)
                    currentInd = ind;
            }

            if (currentInd >= 0 && currentInd < selectForm.dgvEntities.RowCount)
                selectForm.dgvEntities.Rows[currentInd].Cells[0].Selected = true;

            if (selectForm.dgvEntities.RowCount > 1)
            {
                DialogResult dialogResult = selectForm.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    if (selectForm.dgvEntities.CurrentRow.Tag is Entity selectedEntity)
                    {
                        return new EntityDif()
                        {
                            ptr = selectedEntity.Pointer,
                            Name = selectedEntity.Name,
                            InternalName = selectedEntity.InternalName,
                            NameUntranslated = selectedEntity.NameUntranslated,
                            Distance = selectedEntity.Location.Distance3DFromPlayer
                        };
                    }
                }
            }
            return null;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}
