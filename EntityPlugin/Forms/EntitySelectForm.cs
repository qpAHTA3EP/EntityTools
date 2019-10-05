using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EntityTools.Forms
{
    public partial class EntitySelectForm : Form
    {
        private static EntitySelectForm selectForm;

        public EntitySelectForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// краткое описание объекта Entity
        /// </summary>
        public class EntityDif
        {
            public Entity entity = new Entity(IntPtr.Zero);
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
                        entity = entity,
                        Name = entity.Name,
                        NameUntranslated = entity.NameUntranslated,
                        InternalName = entity.InternalName,
                        Distance = entity.Location.Distance3DFromPlayer
                    });
            }
        }

        private void FillEntitiesDgv(string currentEntityId = "")
        {
            if (currentEntityId == string.Empty && dgvEntities.CurrentRow != null && !dgvEntities.CurrentRow.IsNewRow)
                currentEntityId = dgvEntities.CurrentRow.Cells[clmnNameUntranslated.DisplayIndex].Value.ToString(); 

            dgvEntities.DataSource = null;

            clmnName.DataPropertyName = string.Empty;
            clmnInternalName.DataPropertyName = string.Empty;
            clmnNameUntranslated.DataPropertyName = string.Empty;

            dgvEntities.Rows.Clear();

            int currentInd = -1;
            foreach (Entity entity in EntityManager.GetEntities())
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgvEntities);
                row.Cells[clmnName.DisplayIndex].Value = entity.Name;
                row.Cells[clmnNameUntranslated.DisplayIndex].Value = entity.NameUntranslated;
                row.Cells[clmnInternalName.DisplayIndex].Value = entity.InternalName;
                row.Cells[clmnDistance.DisplayIndex].Value = entity.Location.Distance3DFromPlayer;
                row.Tag = entity;

                int ind = dgvEntities.Rows.Add(row);

                if (entity.NameUntranslated == currentEntityId)
                    currentInd = ind;
            }

            if (currentInd >= 0 && currentInd < dgvEntities.RowCount)
                dgvEntities.Rows[currentInd].Cells[0].Selected = true;
        }


        public static EntityDif GetEntity()
        {
            if(selectForm == null)
                selectForm = new EntitySelectForm();

            selectForm.FillEntitiesDgv();

            DialogResult dialogResult = selectForm.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                if (!selectForm.dgvEntities.CurrentRow.IsNewRow)
                {
                    return new EntityDif
                    {
                        entity = selectForm.dgvEntities.CurrentRow.Tag as Entity,
                        Name = selectForm.dgvEntities.CurrentRow.Cells[selectForm.clmnName.DisplayIndex].Value.ToString(),
                        NameUntranslated = selectForm.dgvEntities.CurrentRow.Cells[selectForm.clmnNameUntranslated.DisplayIndex].Value.ToString(),
                        InternalName = selectForm.dgvEntities.CurrentRow.Cells[selectForm.clmnInternalName.DisplayIndex].Value.ToString(),
                        Distance = (double)selectForm.dgvEntities.CurrentRow.Cells[selectForm.clmnDistance.DisplayIndex].Value
                    };
                }
            }

            return null;
        }

        public static EntityDif GetEntity(string entityUntrName)
        {
            if (selectForm == null)
                selectForm = new EntitySelectForm();

            selectForm.FillEntitiesDgv(entityUntrName);

            if (selectForm.dgvEntities.RowCount > 1)
            {
                DialogResult dialogResult = selectForm.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    if (!selectForm.dgvEntities.CurrentRow.IsNewRow)
                    {
                        return new EntityDif
                        {
                            entity = selectForm.dgvEntities.CurrentRow.Tag as Entity,
                            Name = selectForm.dgvEntities.CurrentRow.Cells[selectForm.clmnName.DisplayIndex].Value.ToString(),
                            NameUntranslated = selectForm.dgvEntities.CurrentRow.Cells[selectForm.clmnNameUntranslated.DisplayIndex].Value.ToString(),
                            InternalName = selectForm.dgvEntities.CurrentRow.Cells[selectForm.clmnInternalName.DisplayIndex].Value.ToString(),
                            Distance = (double)selectForm.dgvEntities.CurrentRow.Cells[selectForm.clmnDistance.DisplayIndex].Value
                        };
                    }
                }

                return new EntityDif();
            }
            return null;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
