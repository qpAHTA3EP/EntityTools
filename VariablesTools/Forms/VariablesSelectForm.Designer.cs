namespace VariableTools.Forms
{
    partial class VariablesSelectForm
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
            this.clmnProfScope = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmnAccScope = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmnValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.dntReload = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVariables)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvVariables
            // 
            this.dgvVariables.AllowUserToAddRows = false;
            this.dgvVariables.AllowUserToDeleteRows = false;
            this.dgvVariables.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgvVariables.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvVariables.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvVariables.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvVariables.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvVariables.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmnProfScope,
            this.clmnAccScope,
            this.clmnName,
            this.clmnValue});
            this.dgvVariables.Location = new System.Drawing.Point(4, 4);
            this.dgvVariables.MultiSelect = false;
            this.dgvVariables.Name = "dgvVariables";
            this.dgvVariables.ReadOnly = true;
            this.dgvVariables.RowHeadersVisible = false;
            this.dgvVariables.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvVariables.RowTemplate.ReadOnly = true;
            this.dgvVariables.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvVariables.Size = new System.Drawing.Size(484, 327);
            this.dgvVariables.TabIndex = 0;
            // 
            // clmnProfScope
            // 
            this.clmnProfScope.FillWeight = 60F;
            this.clmnProfScope.Frozen = true;
            this.clmnProfScope.HeaderText = "Profile";
            this.clmnProfScope.MinimumWidth = 90;
            this.clmnProfScope.Name = "clmnProfScope";
            this.clmnProfScope.ReadOnly = true;
            this.clmnProfScope.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.clmnProfScope.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.clmnProfScope.Width = 90;
            // 
            // clmnAccScope
            // 
            this.clmnAccScope.FillWeight = 90F;
            this.clmnAccScope.Frozen = true;
            this.clmnAccScope.HeaderText = "Account";
            this.clmnAccScope.MinimumWidth = 90;
            this.clmnAccScope.Name = "clmnAccScope";
            this.clmnAccScope.ReadOnly = true;
            this.clmnAccScope.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.clmnAccScope.Width = 90;
            // 
            // clmnName
            // 
            this.clmnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clmnName.HeaderText = "Name";
            this.clmnName.MinimumWidth = 100;
            this.clmnName.Name = "clmnName";
            this.clmnName.ReadOnly = true;
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
            this.clmnValue.ReadOnly = true;
            this.clmnValue.ToolTipText = "Значение переменной (The value of the variable)";
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelect.Location = new System.Drawing.Point(358, 342);
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
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(424, 342);
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
            this.dntReload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dntReload.Location = new System.Drawing.Point(8, 343);
            this.dntReload.Name = "dntReload";
            this.dntReload.Size = new System.Drawing.Size(60, 22);
            this.dntReload.TabIndex = 3;
            this.dntReload.Text = "Reload";
            this.dntReload.UseVisualStyleBackColor = true;
            this.dntReload.Click += new System.EventHandler(this.dntReload_Click);
            // 
            // VariablesSelectForm
            // 
            this.AcceptButton = this.btnSelect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(492, 373);
            this.ControlBox = false;
            this.Controls.Add(this.dntReload);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.dgvVariables);
            this.Name = "VariablesSelectForm";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Variables";
            ((System.ComponentModel.ISupportInitialize)(this.dgvVariables)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvVariables;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button dntReload;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmnProfScope;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmnAccScope;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmnValue;
    }
}