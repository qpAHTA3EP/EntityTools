namespace EntityTools.Forms
{
    partial class MultiSelectForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnSelect = new DevExpress.XtraEditors.SimpleButton();
            this.btnReload = new DevExpress.XtraEditors.SimpleButton();
            this.lbl = new System.Windows.Forms.Label();
            this.dgvItems = new System.Windows.Forms.DataGridView();
            this.clmnSelect = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.clmnItems = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.Location = new System.Drawing.Point(12, 367);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(271, 23);
            this.btnSelect.TabIndex = 0;
            this.btnSelect.Text = "Select";
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnReload
            // 
            this.btnReload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReload.Location = new System.Drawing.Point(289, 367);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(23, 23);
            this.btnReload.TabIndex = 1;
            this.btnReload.Text = "R";
            this.btnReload.ToolTip = "Reload";
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // lbl
            // 
            this.lbl.AutoSize = true;
            this.lbl.Location = new System.Drawing.Point(9, 9);
            this.lbl.Name = "lbl";
            this.lbl.Size = new System.Drawing.Size(166, 13);
            this.lbl.TabIndex = 2;
            this.lbl.Text = "Select several items from the list:";
            // 
            // dgvItems
            // 
            this.dgvItems.AllowUserToAddRows = false;
            this.dgvItems.AllowUserToDeleteRows = false;
            this.dgvItems.AllowUserToResizeColumns = false;
            this.dgvItems.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            this.dgvItems.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvItems.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvItems.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvItems.ColumnHeadersVisible = false;
            this.dgvItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmnSelect,
            this.clmnItems});
            this.dgvItems.GridColor = System.Drawing.SystemColors.Window;
            this.dgvItems.Location = new System.Drawing.Point(12, 34);
            this.dgvItems.MultiSelect = false;
            this.dgvItems.Name = "dgvItems";
            this.dgvItems.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvItems.RowHeadersVisible = false;
            this.dgvItems.RowTemplate.Height = 16;
            this.dgvItems.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvItems.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvItems.Size = new System.Drawing.Size(300, 318);
            this.dgvItems.TabIndex = 3;
            // 
            // clmnSelect
            // 
            this.clmnSelect.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.clmnSelect.FillWeight = 22F;
            this.clmnSelect.Frozen = true;
            this.clmnSelect.HeaderText = "Select";
            this.clmnSelect.MinimumWidth = 22;
            this.clmnSelect.Name = "clmnSelect";
            this.clmnSelect.Width = 22;
            // 
            // clmnItems
            // 
            this.clmnItems.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clmnItems.HeaderText = "Items";
            this.clmnItems.Name = "clmnItems";
            this.clmnItems.ReadOnly = true;
            this.clmnItems.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.clmnItems.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // MultiSelectForm
            // 
            this.AcceptButton = this.btnSelect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 402);
            this.Controls.Add(this.dgvItems);
            this.Controls.Add(this.lbl);
            this.Controls.Add(this.btnReload);
            this.Controls.Add(this.btnSelect);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(332, 429);
            this.Name = "MultiSelectForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MultiSelectForm";
            this.Shown += new System.EventHandler(this.MultiSelectForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        internal DevExpress.XtraEditors.SimpleButton btnSelect;
        internal DevExpress.XtraEditors.SimpleButton btnReload;
        internal System.Windows.Forms.Label lbl;
        internal System.Windows.Forms.DataGridView dgvItems;
        internal System.Windows.Forms.DataGridViewTextBoxColumn clmnItemsNames;
        internal System.Windows.Forms.DataGridViewCheckBoxColumn clmnSelect;
        internal System.Windows.Forms.DataGridViewTextBoxColumn clmnItems;
    }
}