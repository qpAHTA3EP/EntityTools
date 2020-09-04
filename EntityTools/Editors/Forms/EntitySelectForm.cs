using DevExpress.XtraEditors;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EntityTools.Editors.Forms
{
    public partial class EntitySelectForm : XtraForm //*/ Form
    {
        private static EntitySelectForm @this;

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

            public bool IsValid
            {
                get
                {
                    return entity.Pointer != IntPtr.Zero;
                }
            }
        }

#if ENTITY_DIF_BINDINGS
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
#endif

        public int clmnNameInd { get => clmnName.DisplayIndex; }
        public int clmnNameUntranslatedInd { get => clmnNameUntranslated.DisplayIndex; }
        public int clmnInternalNameInd { get => clmnInternalName.DisplayIndex; }
        public int clmnDistanceInd { get => clmnDistance.DisplayIndex; }


        private void FillEntitiesDgv(string currentEntityId = "")
        {
            if (currentEntityId == string.Empty && dgvEntities.CurrentRow != null && !dgvEntities.CurrentRow.IsNewRow)
                currentEntityId = dgvEntities.CurrentRow.Cells[clmnNameUntranslatedInd].Value.ToString(); 

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
                row.Cells[clmnNameInd].Value = entity.Name;
                row.Cells[clmnNameUntranslatedInd].Value = entity.NameUntranslated;
                row.Cells[clmnInternalNameInd].Value = entity.InternalName;
                row.Cells[clmnDistanceInd].Value = entity.Location.Distance3DFromPlayer;
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
            if(@this == null)
                @this = new EntitySelectForm();

            @this.FillEntitiesDgv();

            if (@this.ShowDialog() == DialogResult.OK)
            {
                if (!@this.dgvEntities.CurrentRow.IsNewRow)
                {
                    return new EntityDif
                    {
                        entity = @this.dgvEntities.CurrentRow.Tag as Entity,
                        Name = @this.dgvEntities.CurrentRow.Cells[@this.clmnNameInd].Value.ToString(),
                        NameUntranslated = @this.dgvEntities.CurrentRow.Cells[@this.clmnNameUntranslatedInd].Value.ToString(),
                        InternalName = @this.dgvEntities.CurrentRow.Cells[@this.clmnInternalNameInd].Value.ToString(),
                        Distance = (double)@this.dgvEntities.CurrentRow.Cells[@this.clmnDistanceInd].Value
                    };
                }
            }

            return null;
        }

        public static EntityDif GetEntity(string entityUntrName)
        {
            if (@this == null)
                @this = new EntitySelectForm();

            @this.FillEntitiesDgv(entityUntrName);

            if (@this.dgvEntities.RowCount > 1)
            {
                DialogResult dialogResult = @this.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    if (!@this.dgvEntities.CurrentRow.IsNewRow)
                    {
                        return new EntityDif
                        {
                            entity = @this.dgvEntities.CurrentRow.Tag as Entity,
                            Name = @this.dgvEntities.CurrentRow.Cells[@this.clmnNameInd].Value.ToString(),
                            NameUntranslated = @this.dgvEntities.CurrentRow.Cells[@this.clmnNameUntranslatedInd].Value.ToString(),
                            InternalName = @this.dgvEntities.CurrentRow.Cells[@this.clmnInternalNameInd].Value.ToString(),
                            Distance = (double)@this.dgvEntities.CurrentRow.Cells[@this.clmnDistanceInd].Value
                        };
                    }
                }

                return new EntityDif();
            }
            return null;
        }

        public static void ShowFreeTool()
        {
            EntitySelectForm localForm = new EntitySelectForm();

            localForm.FillEntitiesDgv();

            localForm.Show(Astral.Forms.Main.ActiveForm);
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

        private void btnReload_Click(object sender, EventArgs e)
        {
            string entityUntrName = dgvEntities?.CurrentRow.Cells[clmnNameUntranslatedInd].Value?.ToString() ?? string.Empty;

            DataGridViewColumn sortedColumn = dgvEntities.SortedColumn;
            SortOrder sortOrder = dgvEntities.SortOrder;

            FillEntitiesDgv(entityUntrName);

            if (sortedColumn != null)
            {
                switch (sortOrder)
                {
                    case SortOrder.Ascending:
                        dgvEntities.Sort(dgvEntities.SortedColumn, System.ComponentModel.ListSortDirection.Ascending);
                        break;
                    case SortOrder.Descending:
                        dgvEntities.Sort(dgvEntities.SortedColumn, System.ComponentModel.ListSortDirection.Descending);
                        break;
                }
            }
        }
    }
}
