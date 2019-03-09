namespace ScalarAddon.Forms
{
    partial class AstralScalarsEditor
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
            this.dgwScalars = new System.Windows.Forms.DataGridView();
            this.ScalarNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TypeColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgwScalars)).BeginInit();
            this.SuspendLayout();
            // 
            // dgwScalars
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgwScalars.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgwScalars.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgwScalars.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgwScalars.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ScalarNameColumn,
            this.TypeColumn,
            this.ValueColumn});
            this.dgwScalars.Location = new System.Drawing.Point(13, 13);
            this.dgwScalars.Name = "dgwScalars";
            this.dgwScalars.Size = new System.Drawing.Size(447, 360);
            this.dgwScalars.TabIndex = 0;
            this.dgwScalars.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // ScalarNameColumn
            // 
            this.ScalarNameColumn.Frozen = true;
            this.ScalarNameColumn.HeaderText = "Name";
            this.ScalarNameColumn.Name = "ScalarNameColumn";
            this.ScalarNameColumn.ToolTipText = "Имя переменной (The Name of the variable)";
            this.ScalarNameColumn.Width = 150;
            // 
            // TypeColumn
            // 
            this.TypeColumn.Frozen = true;
            this.TypeColumn.HeaderText = "Type";
            this.TypeColumn.Items.AddRange(new object[] {
            "Integer",
            "Boolean",
            "DateTime"});
            this.TypeColumn.Name = "TypeColumn";
            this.TypeColumn.ToolTipText = "Тип переменной (The type of the variable)";
            // 
            // ValueColumn
            // 
            this.ValueColumn.HeaderText = "Value";
            this.ValueColumn.Name = "ValueColumn";
            this.ValueColumn.ToolTipText = "Значение переменной (The value of the variable)";
            // 
            // AstralScalarsEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 450);
            this.Controls.Add(this.dgwScalars);
            this.Name = "AstralScalarsEditor";
            this.Text = "AstralScalarsEditor";
            ((System.ComponentModel.ISupportInitialize)(this.dgwScalars)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgwScalars;
        private System.Windows.Forms.DataGridViewTextBoxColumn ScalarNameColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn TypeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ValueColumn;
    }
}