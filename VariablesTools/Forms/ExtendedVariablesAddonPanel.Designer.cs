namespace VariableTools.Forms
{
    partial class ExtendedVariablesAddonPanel
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvVariables = new System.Windows.Forms.DataGridView();
            this.clmnSave = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.clmnScope = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.clmnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmnValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dntLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblAuthor = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVariables)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvVariables
            // 
            this.dgvVariables.AllowUserToAddRows = false;
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
            this.clmnSave,
            this.clmnScope,
            this.clmnName,
            this.clmnValue});
            this.dgvVariables.Location = new System.Drawing.Point(3, 3);
            this.dgvVariables.MultiSelect = false;
            this.dgvVariables.Name = "dgvVariables";
            this.dgvVariables.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvVariables.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvVariables.Size = new System.Drawing.Size(734, 381);
            this.dgvVariables.TabIndex = 1;
            // 
            // clmnSave
            // 
            this.clmnSave.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.clmnSave.FillWeight = 36F;
            this.clmnSave.Frozen = true;
            this.clmnSave.HeaderText = "Save";
            this.clmnSave.MinimumWidth = 36;
            this.clmnSave.Name = "clmnSave";
            this.clmnSave.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.clmnSave.Width = 36;
            // 
            // clmnScope
            // 
            this.clmnScope.FillWeight = 50F;
            this.clmnScope.Frozen = true;
            this.clmnScope.HeaderText = "Scope";
            this.clmnScope.MinimumWidth = 50;
            this.clmnScope.Name = "clmnScope";
            this.clmnScope.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.clmnScope.Width = 50;
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
            // dntLoad
            // 
            this.dntLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.dntLoad.Location = new System.Drawing.Point(611, 391);
            this.dntLoad.Name = "dntLoad";
            this.dntLoad.Size = new System.Drawing.Size(60, 22);
            this.dntLoad.TabIndex = 6;
            this.dntLoad.Text = "Load";
            this.dntLoad.UseVisualStyleBackColor = true;
            this.dntLoad.Click += new System.EventHandler(this.dntReload_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(677, 390);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(60, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblAuthor
            // 
            this.lblAuthor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblAuthor.AutoSize = true;
            this.lblAuthor.Location = new System.Drawing.Point(3, 394);
            this.lblAuthor.Name = "lblAuthor";
            this.lblAuthor.Size = new System.Drawing.Size(97, 13);
            this.lblAuthor.TabIndex = 7;
            this.lblAuthor.Text = "Athor: MichaelProg";
            // 
            // ExtendedVariablesAddonPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblAuthor);
            this.Controls.Add(this.dntLoad);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dgvVariables);
            this.Name = "ExtendedVariablesAddonPanel";
            this.Size = new System.Drawing.Size(740, 416);
            this.Load += new System.EventHandler(this.ExtendedVariablesAddonPanel_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvVariables)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvVariables;
        private System.Windows.Forms.DataGridViewCheckBoxColumn clmnSave;
        private System.Windows.Forms.DataGridViewComboBoxColumn clmnScope;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmnValue;
        private System.Windows.Forms.Button dntLoad;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblAuthor;
    }
}
