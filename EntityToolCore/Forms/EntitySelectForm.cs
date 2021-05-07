using Astral.Classes.ItemFilter;
using DevExpress.XtraEditors;
using EntityCore.Entities;
using EntityTools.Enums;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Windows.Forms;

namespace EntityCore.Forms
{
    public partial class EntitySelectForm : XtraForm //*/ Form
    {
        private static EntitySelectForm @this;

        public EntitySelectForm()
        {
            InitializeComponent();
            cbNameType.DataSource = Enum.GetValues(typeof(EntityNameType));
            cbStrMatch.DataSource = Enum.GetValues(typeof(ItemFilterStringType));
        }

        #region Данные
        public int clmnNameInd { get => clmnName.DisplayIndex; }
        public int clmnNameUntranslatedInd { get => clmnNameUntranslated.DisplayIndex; }
        public int clmnInternalNameInd { get => clmnInternalName.DisplayIndex; }
        public int clmnDistanceInd { get => clmnDistance.DisplayIndex; }

        //private Predicate<Entity> entityCheck = null;
        /// <summary>
        /// текущий шаблон
        /// </summary>
        private string entityPattern = string.Empty;
        private ItemFilterStringType stringMatchType = ItemFilterStringType.Simple;
        private EntityNameType entityNameType = EntityNameType.NameUntranslated;
        #endregion

        public static Entity GUIRequest(string entPattern = "", ItemFilterStringType strMatchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated)
        {
            return GUIRequest(ref entPattern, ref strMatchType, ref nameType);
        }

        public static Entity GUIRequest(ref string entPattern, ref ItemFilterStringType strMatchType, ref EntityNameType nameType)
        {
            if (@this == null)
                @this = new EntitySelectForm();

            //@this.entityCheck = EntityToPatternComparer.Get(entPattern, strMatchType, nameType);
            @this.entityPattern = entPattern;
            @this.stringMatchType = strMatchType;
            @this.cbStrMatch.SelectedItem = strMatchType;
            @this.entityNameType = nameType;
            @this.cbNameType.SelectedItem = nameType;

            //@this.Text = $"{@this.GetType().Name} [{strMatchType}; {nameType}]";
            @this.tbPattern.Text = entPattern;

#if false 
            @this.FillEntitiesDgv(EntityTools.Tools.Entities.EntityComparer.Get(entPattern, strMatchType, nameType));
#else
            @this.FillEntitiesDgv(EntityComparer.Get(entPattern, strMatchType, nameType)); 
#endif

            if (@this.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(@this.tbPattern.Text))
                {
                    var currentRow = @this.dgvEntities.CurrentRow;
                    switch (@this.cbNameType.SelectedItem)
                    {
                        case EntityNameType.InternalName:
                            entPattern = currentRow.Cells[@this.clmnInternalNameInd].Value.ToString();
                            break;
                        case EntityNameType.NameUntranslated:
                            entPattern = currentRow.Cells[@this.clmnNameUntranslatedInd].Value.ToString();
                            break;
                        case EntityNameType.Empty:
                            entPattern = string.Empty;
                            break;
                    }
                }
                else entPattern = @this.tbPattern.Text.Trim();

                strMatchType = (ItemFilterStringType)@this.cbStrMatch.SelectedItem;
                nameType = (EntityNameType)@this.cbNameType.SelectedItem;

                return @this.dgvEntities.CurrentRow.Tag as Entity; 
            }
            return null;
        }

        #region События Интерфейса
        private void FillEntitiesDgv(Predicate<Entity> check = null)
        {
            //if (currentEntityId == string.Empty && dgvEntities.CurrentRow != null && !dgvEntities.CurrentRow.IsNewRow)
            //    currentEntityId = dgvEntities.CurrentRow.Cells[clmnNameUntranslatedInd].Value.ToString(); 

            if (check == null)
                check = (Entity e) => false;

#if false
            dgvEntities.DataSource = null;

            clmnName.DataPropertyName = string.Empty;
            clmnInternalName.DataPropertyName = string.Empty;
            clmnNameUntranslated.DataPropertyName = string.Empty; 
#endif

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

                if (check(entity))
                    currentInd = ind;
            }

            if (currentInd >= 0 && currentInd < dgvEntities.RowCount)
                dgvEntities.Rows[currentInd].Cells[0].Selected = true;
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
            //string entityUntrName = dgvEntities?.CurrentRow.Cells[clmnNameUntranslatedInd].Value?.ToString() ?? string.Empty;

            // Запоминаем текущую позицию
            Predicate<object> predicate = null;
            if (!(dgvEntities.CurrentRow?.Tag is null))
            {
                object entt = dgvEntities.CurrentRow.Tag;
                predicate = (object tag) => ReferenceEquals(tag, entt);
            }

            DataGridViewColumn sortedColumn = dgvEntities.SortedColumn;
            SortOrder sortOrder = dgvEntities.SortOrder;

            FillEntitiesDgv(predicate);//EntityToPatternComparer.Get(entityUntrName, (ItemFilterStringType)cbStrMatch.SelectedItem, (EntityNameType)cbNameType.SelectedItem));

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

                // устанавливаем фокус на ранее выбранную строку после сортировки
                // если сортировка не производилась, то фокус нужной строке передается в FillEntitiesDgv(predicate);
                if (predicate != null)
                {
                    foreach (DataGridViewRow row in dgvEntities.Rows)
                    {
                        if (predicate(row.Tag))
                        {
                            row.Cells[0].Selected = true;
                            break;
                        }
                    }
                }
            }
        }

        private void dgvEntities_MouseClick(object sender, MouseEventArgs e)
        {
            if (System.Windows.Forms.Control.ModifierKeys == Keys.Control
                && !(dgvEntities.CurrentRow is null))
            {
                string text = (cbStrMatch.SelectedItem.Equals(ItemFilterStringType.Regex)
                               && !string.IsNullOrEmpty(tbPattern.Text)) ? "|" : string.Empty;
                switch (cbNameType.SelectedItem)
                {
                    case EntityNameType.InternalName:
                        text += dgvEntities.CurrentRow.Cells[clmnInternalNameInd].Value.ToString();
                        break;
                    case EntityNameType.NameUntranslated:
                        text += dgvEntities.CurrentRow.Cells[clmnNameUntranslatedInd].Value.ToString();
                        break;
                }
                tbPattern.Text += text;
            }
        }

        private void dgvEntities_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Запоминаем текущую позицию
            Predicate<object> predicate = null;
            if (!(dgvEntities.CurrentRow?.Tag is null))
            {
                object entt = dgvEntities.CurrentRow.Tag;
                predicate = (object tag) => ReferenceEquals(tag, entt);
            }

            if (dgvEntities.SortedColumn?.DisplayIndex == e.ColumnIndex)
            {
                System.ComponentModel.ListSortDirection sortDirection;
                // переключаем режим сортировки текущего столбца
                if (dgvEntities.SortOrder != SortOrder.Ascending)
                    sortDirection = System.ComponentModel.ListSortDirection.Ascending;
                else sortDirection = System.ComponentModel.ListSortDirection.Descending;
                dgvEntities.Sort(dgvEntities.SortedColumn, sortDirection);
            }
            else
            {
                // переключаемся на сортировку другого столбца
                dgvEntities.Sort(dgvEntities.Columns[e.ColumnIndex], System.ComponentModel.ListSortDirection.Ascending);
            }

            // устанавливаем фокус на ранее выбранную строку
            if (predicate != null)
            {
                foreach (DataGridViewRow row in dgvEntities.Rows)
                {
                    if (predicate(row.Tag))
                    {
                        row.Cells[0].Selected = true;
                        break;
                    }
                }
            }
        }

        private void dgvEntities_MouseDown(object sender, MouseEventArgs e)
        {
            if (!(dgvEntities.CurrentRow is null)
                && System.Windows.Forms.Control.ModifierKeys == Keys.Control)
            {
                switch (cbNameType.SelectedItem)
                {
                    case EntityNameType.InternalName:
                        dgvEntities.DoDragDrop(dgvEntities.CurrentRow.Cells[clmnInternalNameInd].Value, DragDropEffects.Copy);
                        break;
                    case EntityNameType.NameUntranslated:
                        dgvEntities.DoDragDrop(dgvEntities.CurrentRow.Cells[clmnNameUntranslatedInd].Value, DragDropEffects.Copy);
                        break;
                }
            }
        }

        private void tbPattern_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effect = DragDropEffects.Copy;
                /*switch (cbStrMatch.SelectedItem)
                {
                    case ItemFilterStringType.Regex:
                        if (!string.IsNullOrEmpty(tbPattern.Text))
                            tbPattern.Text += '|';
                        tbPattern.Text += e.Data.GetData(DataFormats.Text).ToString();
                        break;
                    case ItemFilterStringType.Simple:
                        tbPattern.Text += e.Data.GetData(DataFormats.Text).ToString();
                        break;
                }*/
            }
        }

        private void tbPattern_DragDrop(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.Text))
            {
                switch(cbStrMatch.SelectedItem)
                {
                    case ItemFilterStringType.Regex:
                        if (!string.IsNullOrEmpty(tbPattern.Text))
                            tbPattern.Text += '|';
                        tbPattern.Text += e.Data.GetData(DataFormats.Text).ToString();
                        break;
                    case ItemFilterStringType.Simple:
                        tbPattern.Text += e.Data.GetData(DataFormats.Text).ToString();
                        break;
                }
            }
        }

        private void dgvEntities_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (System.Windows.Forms.Control.ModifierKeys == Keys.Control)
            {
                string text = (cbStrMatch.SelectedItem.Equals(ItemFilterStringType.Regex)
                               && !string.IsNullOrEmpty(tbPattern.Text)) ? "|" : string.Empty;
                switch (cbNameType.SelectedItem)
                {
                    case EntityNameType.InternalName:
                        text += dgvEntities.Rows[e.RowIndex].Cells[clmnInternalNameInd].Value.ToString();
                        break;
                    case EntityNameType.NameUntranslated:
                        text += dgvEntities.Rows[e.RowIndex].Cells[clmnNameUntranslatedInd].Value.ToString();
                        break;
                }
                tbPattern.Text += text;
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            Predicate<Entity> test = null;
            if (string.IsNullOrEmpty(tbPattern.Text))
                if (dgvEntities.CurrentRow != null)
                    switch ((EntityNameType)cbNameType.SelectedItem)
                    {
                        case EntityNameType.InternalName:
#if false 
                            test = EntityTools.Tools.Entities.EntityComparer.Get(dgvEntities.CurrentRow.Cells[clmnInternalNameInd].Value.ToString(), (ItemFilterStringType)cbStrMatch.SelectedItem, (EntityNameType)cbNameType.SelectedItem);
#else
                            test = EntityComparer.Get(dgvEntities.CurrentRow.Cells[clmnInternalNameInd].Value.ToString(), (ItemFilterStringType)cbStrMatch.SelectedItem, (EntityNameType)cbNameType.SelectedItem); 
#endif
                            break;
                        case EntityNameType.NameUntranslated:
#if false
                            test = EntityTools.Tools.Entities.EntityComparer.Get(dgvEntities.CurrentRow.Cells[clmnNameUntranslatedInd].Value.ToString(), (ItemFilterStringType)cbStrMatch.SelectedItem, (EntityNameType)cbNameType.SelectedItem);
#else
                            test = EntityComparer.Get(dgvEntities.CurrentRow.Cells[clmnNameUntranslatedInd].Value.ToString(), (ItemFilterStringType)cbStrMatch.SelectedItem, (EntityNameType)cbNameType.SelectedItem); 
#endif
                            break;
                        case EntityNameType.Empty:
#if false
                            test = EntityTools.Tools.Entities.EntityComparer.Get(string.Empty, (ItemFilterStringType)cbStrMatch.SelectedItem, (EntityNameType)cbNameType.SelectedItem);
#else
                            test = EntityComparer.Get(string.Empty, (ItemFilterStringType)cbStrMatch.SelectedItem, (EntityNameType)cbNameType.SelectedItem); 
#endif
                            break;
                    }
                else
                {
                    XtraMessageBox.Show("Select Entity first!");
                    return;
                }
#if false
            else test = EntityTools.Tools.Entities.EntityComparer.Get(tbPattern.Text, (ItemFilterStringType)cbStrMatch.SelectedItem, (EntityNameType)cbNameType.SelectedItem);
#else
            else test = EntityComparer.Get(tbPattern.Text, (ItemFilterStringType)cbStrMatch.SelectedItem, (EntityNameType)cbNameType.SelectedItem); 
#endif

            if (test != null)
            {
                foreach (DataGridViewRow row in dgvEntities.Rows)
                {
                    if ((row.Tag is Entity entity) && test(entity))
                    {
                        row.Cells[0].Selected = true;
                        return;
                    }
                }

                XtraMessageBox.Show("No Entity match to the pattern", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion
    }
}
