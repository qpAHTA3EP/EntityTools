namespace EntityTools.Forms
{
    partial class EntitySelectForm
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
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.dgvEntities = new System.Windows.Forms.DataGridView();
            this.clmnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmnInternalName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmnNameUntranslated = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmnDistance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnReload = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEntities)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelect.Location = new System.Drawing.Point(562, 427);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(102, 23);
            this.btnSelect.TabIndex = 0;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(670, 427);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(102, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // dgvEntities
            // 
            this.dgvEntities.AllowUserToAddRows = false;
            this.dgvEntities.AllowUserToDeleteRows = false;
            this.dgvEntities.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            this.dgvEntities.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvEntities.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvEntities.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvEntities.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvEntities.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvEntities.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEntities.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmnName,
            this.clmnInternalName,
            this.clmnNameUntranslated,
            this.clmnDistance});
            this.dgvEntities.Location = new System.Drawing.Point(13, 13);
            this.dgvEntities.MultiSelect = false;
            this.dgvEntities.Name = "dgvEntities";
            this.dgvEntities.ReadOnly = true;
            this.dgvEntities.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvEntities.RowHeadersVisible = false;
            this.dgvEntities.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvEntities.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEntities.ShowEditingIcon = false;
            this.dgvEntities.ShowRowErrors = false;
            this.dgvEntities.Size = new System.Drawing.Size(759, 399);
            this.dgvEntities.TabIndex = 1;
            // 
            // clmnName
            // 
            this.clmnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.clmnName.Frozen = true;
            this.clmnName.HeaderText = "Name";
            this.clmnName.Name = "clmnName";
            this.clmnName.ReadOnly = true;
            this.clmnName.Width = 200;
            // 
            // clmnInternalName
            // 
            this.clmnInternalName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.clmnInternalName.HeaderText = "InternalName";
            this.clmnInternalName.Name = "clmnInternalName";
            this.clmnInternalName.ReadOnly = true;
            this.clmnInternalName.Width = 200;
            // 
            // clmnNameUntranslated
            // 
            this.clmnNameUntranslated.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clmnNameUntranslated.HeaderText = "NameUntranslated";
            this.clmnNameUntranslated.Name = "clmnNameUntranslated";
            this.clmnNameUntranslated.ReadOnly = true;
            // 
            // clmnDistance
            // 
            this.clmnDistance.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.clmnDistance.HeaderText = "Distance";
            this.clmnDistance.Name = "clmnDistance";
            this.clmnDistance.ReadOnly = true;
            this.clmnDistance.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.clmnDistance.Width = 74;
            // 
            // btnReload
            // 
            this.btnReload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReload.Location = new System.Drawing.Point(13, 427);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(102, 23);
            this.btnReload.TabIndex = 0;
            this.btnReload.Text = "Reload";
            this.btnReload.UseVisualStyleBackColor = true;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // EntitySelectForm
            // 
            this.AcceptButton = this.btnSelect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(784, 462);
            this.ControlBox = false;
            this.Controls.Add(this.dgvEntities);
            this.Controls.Add(this.btnReload);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSelect);
            this.MinimumSize = new System.Drawing.Size(500, 300);
            this.Name = "EntitySelectForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EntitySelectForm";
            ((System.ComponentModel.ISupportInitialize)(this.dgvEntities)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DataGridView dgvEntities;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmnInternalName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmnNameUntranslated;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmnDistance;
        private System.Windows.Forms.Button btnReload;
    }
}