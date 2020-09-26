namespace EntityCore.Forms
{
    partial class MountBonusPriorityListForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.cbPlayersClass = new System.Windows.Forms.ComboBox();
            this.dgvBonusPriorityList = new System.Windows.Forms.DataGridView();
            this.clmnBonusName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmnNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmnButton = new System.Windows.Forms.DataGridViewButtonColumn();
            this.btnDown = new System.Windows.Forms.Button();
            this.bntUp = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.lbClasses = new System.Windows.Forms.Label();
            this.btnRemove = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBonusPriorityList)).BeginInit();
            this.SuspendLayout();
            // 
            // cbPlayersClass
            // 
            this.cbPlayersClass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbPlayersClass.FormattingEnabled = true;
            this.cbPlayersClass.Location = new System.Drawing.Point(89, 12);
            this.cbPlayersClass.Name = "cbPlayersClass";
            this.cbPlayersClass.Size = new System.Drawing.Size(405, 21);
            this.cbPlayersClass.Sorted = true;
            this.cbPlayersClass.TabIndex = 0;
            this.cbPlayersClass.SelectedIndexChanged += new System.EventHandler(this.cbPlayersClass_SelectedIndexChanged);
            // 
            // dgvBonusPriorityList
            // 
            this.dgvBonusPriorityList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvBonusPriorityList.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvBonusPriorityList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBonusPriorityList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmnBonusName,
            this.clmnNumber,
            this.clmnButton});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Tahoma", 8.25F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(31)))), ((int)(((byte)(53)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvBonusPriorityList.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvBonusPriorityList.Location = new System.Drawing.Point(12, 48);
            this.dgvBonusPriorityList.MultiSelect = false;
            this.dgvBonusPriorityList.Name = "dgvBonusPriorityList";
            this.dgvBonusPriorityList.Size = new System.Drawing.Size(482, 218);
            this.dgvBonusPriorityList.TabIndex = 1;
            this.dgvBonusPriorityList.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvBonusPriorityList_CellClick);
            // 
            // clmnBonusName
            // 
            this.clmnBonusName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clmnBonusName.HeaderText = "Bonus Name";
            this.clmnBonusName.Name = "clmnBonusName";
            this.clmnBonusName.ReadOnly = true;
            this.clmnBonusName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmnNumber
            // 
            this.clmnNumber.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.Format = "N0";
            dataGridViewCellStyle1.NullValue = "1";
            this.clmnNumber.DefaultCellStyle = dataGridViewCellStyle1;
            this.clmnNumber.FillWeight = 40F;
            this.clmnNumber.HeaderText = "Num";
            this.clmnNumber.MinimumWidth = 40;
            this.clmnNumber.Name = "clmnNumber";
            this.clmnNumber.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.clmnNumber.ToolTipText = "Number of bonuses at the same time";
            this.clmnNumber.Width = 40;
            // 
            // clmnButton
            // 
            this.clmnButton.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.clmnButton.FillWeight = 23F;
            this.clmnButton.HeaderText = "";
            this.clmnButton.MinimumWidth = 23;
            this.clmnButton.Name = "clmnButton";
            this.clmnButton.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.clmnButton.Text = "...";
            this.clmnButton.ToolTipText = "Select insignia bonus";
            this.clmnButton.UseColumnTextForButtonValue = true;
            this.clmnButton.Width = 23;
            // 
            // btnDown
            // 
            this.btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDown.Location = new System.Drawing.Point(45, 282);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(23, 23);
            this.btnDown.TabIndex = 2;
            this.btnDown.Text = "Down";
            this.btnDown.UseVisualStyleBackColor = true;
            // 
            // bntUp
            // 
            this.bntUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bntUp.Location = new System.Drawing.Point(16, 282);
            this.bntUp.Name = "bntUp";
            this.bntUp.Size = new System.Drawing.Size(23, 23);
            this.bntUp.TabIndex = 2;
            this.bntUp.Text = "Up";
            this.bntUp.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(419, 282);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(338, 282);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // lbClasses
            // 
            this.lbClasses.AutoSize = true;
            this.lbClasses.Location = new System.Drawing.Point(13, 15);
            this.lbClasses.Name = "lbClasses";
            this.lbClasses.Size = new System.Drawing.Size(70, 13);
            this.lbClasses.TabIndex = 3;
            this.lbClasses.Text = "Player\'s class";
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemove.Location = new System.Drawing.Point(74, 282);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(23, 23);
            this.btnRemove.TabIndex = 2;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            // 
            // MountBonusPriorityListForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(506, 317);
            this.ControlBox = false;
            this.Controls.Add(this.lbClasses);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.bntUp);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.dgvBonusPriorityList);
            this.Controls.Add(this.cbPlayersClass);
            this.LookAndFeel.SkinName = "Office 2013 Light Gray";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.MinimumSize = new System.Drawing.Size(380, 324);
            this.Name = "MountBonusPriorityListForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MountBonusList";
            ((System.ComponentModel.ISupportInitialize)(this.dgvBonusPriorityList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbPlayersClass;
        private System.Windows.Forms.DataGridView dgvBonusPriorityList;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button bntUp;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lbClasses;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmnBonusName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmnNumber;
        private System.Windows.Forms.DataGridViewButtonColumn clmnButton;
    }
}