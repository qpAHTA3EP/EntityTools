namespace VariableTools.Forms
{
    partial class ExtendedVariablesToolsPanel
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvVariables = new System.Windows.Forms.DataGridView();
            this.clmnSave = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.clmnProfileScope = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.clmnAccScope = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.clmnQualifier = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmnValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dntLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblAuthor = new System.Windows.Forms.Label();
            this.bntAdd = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.ckbDebug = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVariables)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvVariables
            // 
            this.dgvVariables.AllowUserToAddRows = false;
            this.dgvVariables.AllowUserToDeleteRows = false;
            this.dgvVariables.AllowUserToResizeRows = false;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgvVariables.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvVariables.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvVariables.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvVariables.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvVariables.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvVariables.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmnSave,
            this.clmnProfileScope,
            this.clmnAccScope,
            this.clmnQualifier,
            this.clmnName,
            this.clmnValue});
            this.dgvVariables.Location = new System.Drawing.Point(4, 4);
            this.dgvVariables.MultiSelect = false;
            this.dgvVariables.Name = "dgvVariables";
            this.dgvVariables.RowHeadersVisible = false;
            this.dgvVariables.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvVariables.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvVariables.Size = new System.Drawing.Size(732, 379);
            this.dgvVariables.TabIndex = 1;
            this.dgvVariables.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dgvVariables_CellValidating);
            this.dgvVariables.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvVariables_CellValueChanged);
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
            this.clmnSave.ToolTipText = "Флаг необходимости сохранения переменной в файл.\\nThe Flag to save the variable t" +
    "o a file";
            this.clmnSave.Width = 36;
            // 
            // clmnProfileScope
            // 
            this.clmnProfileScope.FillWeight = 85F;
            this.clmnProfileScope.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.clmnProfileScope.Frozen = true;
            this.clmnProfileScope.HeaderText = "Profile Scope";
            this.clmnProfileScope.MinimumWidth = 85;
            this.clmnProfileScope.Name = "clmnProfileScope";
            this.clmnProfileScope.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.clmnProfileScope.ToolTipText = "Ограничение области видимость переменной текущим профилем.\\nThe scope the variabl" +
    "e for the current quester-profile";
            this.clmnProfileScope.Width = 85;
            // 
            // clmnAccScope
            // 
            this.clmnAccScope.FillWeight = 85F;
            this.clmnAccScope.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.clmnAccScope.Frozen = true;
            this.clmnAccScope.HeaderText = "Acc.Scope";
            this.clmnAccScope.MinimumWidth = 85;
            this.clmnAccScope.Name = "clmnAccScope";
            this.clmnAccScope.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.clmnAccScope.ToolTipText = "Ограничение области видимости переменной для персонажей аккаунта\\nThe scope of th" +
    "e variable for the accounts or characters";
            this.clmnAccScope.Width = 85;
            // 
            // clmnQualifier
            // 
            this.clmnQualifier.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clmnQualifier.HeaderText = "Qualifier";
            this.clmnQualifier.MinimumWidth = 100;
            this.clmnQualifier.Name = "clmnQualifier";
            this.clmnQualifier.ReadOnly = true;
            this.clmnQualifier.ToolTipText = "видимости\\nThe scope identificator";
            // 
            // clmnName
            // 
            this.clmnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.clmnName.HeaderText = "Name";
            this.clmnName.MinimumWidth = 60;
            this.clmnName.Name = "clmnName";
            this.clmnName.ToolTipText = "Имя переменной\\nThe Name of the variable";
            this.clmnName.Width = 150;
            // 
            // clmnValue
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.Format = "N3";
            dataGridViewCellStyle4.NullValue = "0";
            dataGridViewCellStyle4.Padding = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.clmnValue.DefaultCellStyle = dataGridViewCellStyle4;
            this.clmnValue.HeaderText = "Value";
            this.clmnValue.MinimumWidth = 30;
            this.clmnValue.Name = "clmnValue";
            this.clmnValue.ToolTipText = "Значение переменной\\nThe value of the variable";
            // 
            // dntLoad
            // 
            this.dntLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.dntLoad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dntLoad.Location = new System.Drawing.Point(610, 389);
            this.dntLoad.Name = "dntLoad";
            this.dntLoad.Size = new System.Drawing.Size(60, 22);
            this.dntLoad.TabIndex = 6;
            this.dntLoad.Text = "Load";
            this.dntLoad.UseVisualStyleBackColor = true;
            this.dntLoad.Click += new System.EventHandler(this.dntLoad_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(676, 389);
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
            this.lblAuthor.Location = new System.Drawing.Point(4, 394);
            this.lblAuthor.Name = "lblAuthor";
            this.lblAuthor.Size = new System.Drawing.Size(171, 13);
            this.lblAuthor.TabIndex = 7;
            this.lblAuthor.Text = "VariableTools from MichaelProg (c)";
            this.lblAuthor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bntAdd
            // 
            this.bntAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bntAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bntAdd.ForeColor = System.Drawing.SystemColors.ControlText;
            this.bntAdd.Location = new System.Drawing.Point(405, 389);
            this.bntAdd.Name = "bntAdd";
            this.bntAdd.Size = new System.Drawing.Size(60, 22);
            this.bntAdd.TabIndex = 6;
            this.bntAdd.Text = "Add";
            this.bntAdd.UseVisualStyleBackColor = true;
            this.bntAdd.Click += new System.EventHandler(this.bntAdd_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Location = new System.Drawing.Point(471, 389);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(60, 22);
            this.btnDelete.TabIndex = 6;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // ckbDebug
            // 
            this.ckbDebug.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ckbDebug.AutoSize = true;
            this.ckbDebug.Location = new System.Drawing.Point(181, 392);
            this.ckbDebug.Name = "ckbDebug";
            this.ckbDebug.Size = new System.Drawing.Size(103, 17);
            this.ckbDebug.TabIndex = 9;
            this.ckbDebug.Text = "Debug message";
            this.ckbDebug.UseVisualStyleBackColor = true;
            this.ckbDebug.CheckedChanged += new System.EventHandler(this.ckbDebug_CheckedChanged);
            // 
            // ExtendedVariablesToolsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ckbDebug);
            this.Controls.Add(this.lblAuthor);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.bntAdd);
            this.Controls.Add(this.dntLoad);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dgvVariables);
            this.Name = "ExtendedVariablesToolsPanel";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Size = new System.Drawing.Size(740, 416);
            this.Load += new System.EventHandler(this.ExtendedVariablesAddonPanel_Load);
            this.Leave += new System.EventHandler(this.ExtendedVariablesAddonPanel_Leave);
            ((System.ComponentModel.ISupportInitialize)(this.dgvVariables)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvVariables;
        private System.Windows.Forms.Button dntLoad;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblAuthor;
        private System.Windows.Forms.Button bntAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.DataGridViewCheckBoxColumn clmnSave;
        private System.Windows.Forms.DataGridViewComboBoxColumn clmnProfileScope;
        private System.Windows.Forms.DataGridViewComboBoxColumn clmnAccScope;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmnQualifier;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmnValue;
        private System.Windows.Forms.CheckBox ckbDebug;
    }
}
