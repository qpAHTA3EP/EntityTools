namespace AstralVariables.Forms
{
    partial class VariablesEditor
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvVariables = new System.Windows.Forms.DataGridView();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.dntReload = new System.Windows.Forms.Button();
            this.chbAllowEdit = new System.Windows.Forms.CheckBox();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmnValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVariables)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvVariables
            // 
            this.dgvVariables.AllowUserToOrderColumns = true;
            this.dgvVariables.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgvVariables.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvVariables.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvVariables.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvVariables.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvVariables.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvVariables.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmnName,
            this.clmnValue});
            this.dgvVariables.Location = new System.Drawing.Point(13, 13);
            this.dgvVariables.MultiSelect = false;
            this.dgvVariables.Name = "dgvVariables";
            this.dgvVariables.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvVariables.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvVariables.Size = new System.Drawing.Size(317, 386);
            this.dgvVariables.TabIndex = 0;
            this.dgvVariables.ReadOnlyChanged += new System.EventHandler(this.dgvVariables_ReadOnlyChanged);
            this.dgvVariables.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvVariables_RowCanSelect);
            this.dgvVariables.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dgvVariables_CellValidating);
            this.dgvVariables.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvVariables_RowCanSelect);
            this.dgvVariables.RowLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvVariables_RowCanSelect);
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.Location = new System.Drawing.Point(204, 415);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(60, 23);
            this.btnSelect.TabIndex = 1;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(270, 415);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(60, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // dntReload
            // 
            this.dntReload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dntReload.Location = new System.Drawing.Point(13, 415);
            this.dntReload.Name = "dntReload";
            this.dntReload.Size = new System.Drawing.Size(60, 22);
            this.dntReload.TabIndex = 3;
            this.dntReload.Text = "Reload";
            this.dntReload.UseVisualStyleBackColor = true;
            this.dntReload.Click += new System.EventHandler(this.dntReload_Click);
            // 
            // chbAllowEdit
            // 
            this.chbAllowEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chbAllowEdit.AutoSize = true;
            this.chbAllowEdit.Location = new System.Drawing.Point(79, 419);
            this.chbAllowEdit.Name = "chbAllowEdit";
            this.chbAllowEdit.Size = new System.Drawing.Size(80, 17);
            this.chbAllowEdit.TabIndex = 4;
            this.chbAllowEdit.Text = "Enable Edit";
            this.chbAllowEdit.UseVisualStyleBackColor = true;
            this.chbAllowEdit.CheckedChanged += new System.EventHandler(this.chbAllowEdit_CheckedChanged);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn1.Frozen = true;
            this.dataGridViewTextBoxColumn1.HeaderText = "Name";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ToolTipText = "Имя переменной (The Name of the variable)";
            // 
            // clmnName
            // 
            this.clmnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clmnName.HeaderText = "Name";
            this.clmnName.MinimumWidth = 60;
            this.clmnName.Name = "clmnName";
            this.clmnName.ToolTipText = "Имя переменной (The Name of the variable)";
            // 
            // clmnValue
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.Format = "N3";
            dataGridViewCellStyle2.NullValue = "0";
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.clmnValue.DefaultCellStyle = dataGridViewCellStyle2;
            this.clmnValue.HeaderText = "Value";
            this.clmnValue.MinimumWidth = 30;
            this.clmnValue.Name = "clmnValue";
            this.clmnValue.ToolTipText = "Значение переменной (The value of the variable)";
            // 
            // VariablesEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(342, 450);
            this.ControlBox = false;
            this.Controls.Add(this.chbAllowEdit);
            this.Controls.Add(this.dntReload);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.dgvVariables);
            this.Name = "VariablesEditor";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Variables";
            ((System.ComponentModel.ISupportInitialize)(this.dgvVariables)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvVariables;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button dntReload;
        private System.Windows.Forms.CheckBox chbAllowEdit;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmnValue;
    }
}