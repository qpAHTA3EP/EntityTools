namespace EntityTools.Forms
{
    partial class InsigniaBonusSelectForm
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
            this.components = new System.ComponentModel.Container();
            this.lbFilter = new System.Windows.Forms.Label();
            this.tbBarbed = new System.Windows.Forms.TextBox();
            this.tbCrescent = new System.Windows.Forms.TextBox();
            this.tbllluminated = new System.Windows.Forms.TextBox();
            this.tbEnlightened = new System.Windows.Forms.TextBox();
            this.tbRegal = new System.Windows.Forms.TextBox();
            this.btnFilter = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSelect = new System.Windows.Forms.Button();
            this.BonusList = new System.Windows.Forms.ListBox();
            this.bindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.Description = new System.Windows.Forms.TextBox();
            this.pbBarbed = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.Insignia1 = new System.Windows.Forms.PictureBox();
            this.Insignia2 = new System.Windows.Forms.PictureBox();
            this.Insignia3 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBarbed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Insignia1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Insignia2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Insignia3)).BeginInit();
            this.SuspendLayout();
            // 
            // lbFilter
            // 
            this.lbFilter.AutoSize = true;
            this.lbFilter.Location = new System.Drawing.Point(12, 14);
            this.lbFilter.Name = "lbFilter";
            this.lbFilter.Size = new System.Drawing.Size(50, 13);
            this.lbFilter.TabIndex = 1;
            this.lbFilter.Text = "Filter by:";
            // 
            // tbBarbed
            // 
            this.tbBarbed.Location = new System.Drawing.Point(94, 12);
            this.tbBarbed.Name = "tbBarbed";
            this.tbBarbed.Size = new System.Drawing.Size(20, 21);
            this.tbBarbed.TabIndex = 2;
            this.tbBarbed.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb_KeyPress);
            // 
            // tbCrescent
            // 
            this.tbCrescent.Location = new System.Drawing.Point(147, 12);
            this.tbCrescent.Name = "tbCrescent";
            this.tbCrescent.Size = new System.Drawing.Size(20, 21);
            this.tbCrescent.TabIndex = 2;
            // 
            // tbllluminated
            // 
            this.tbllluminated.Location = new System.Drawing.Point(200, 12);
            this.tbllluminated.Name = "tbllluminated";
            this.tbllluminated.Size = new System.Drawing.Size(20, 21);
            this.tbllluminated.TabIndex = 2;
            // 
            // tbEnlightened
            // 
            this.tbEnlightened.Location = new System.Drawing.Point(253, 12);
            this.tbEnlightened.Name = "tbEnlightened";
            this.tbEnlightened.Size = new System.Drawing.Size(20, 21);
            this.tbEnlightened.TabIndex = 2;
            // 
            // tbRegal
            // 
            this.tbRegal.Location = new System.Drawing.Point(306, 12);
            this.tbRegal.Name = "tbRegal";
            this.tbRegal.Size = new System.Drawing.Size(20, 21);
            this.tbRegal.TabIndex = 2;
            // 
            // btnFilter
            // 
            this.btnFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFilter.Location = new System.Drawing.Point(457, 10);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(75, 23);
            this.btnFilter.TabIndex = 3;
            this.btnFilter.Text = "Filter";
            this.btnFilter.UseVisualStyleBackColor = true;
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(457, 313);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.Location = new System.Drawing.Point(376, 313);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSelect.TabIndex = 3;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            // 
            // BonusList
            // 
            this.BonusList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BonusList.DataSource = this.bindingSource;
            this.BonusList.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.BonusList.FormattingEnabled = true;
            this.BonusList.ItemHeight = 16;
            this.BonusList.Location = new System.Drawing.Point(15, 44);
            this.BonusList.Name = "BonusList";
            this.BonusList.Size = new System.Drawing.Size(517, 196);
            this.BonusList.Sorted = true;
            this.BonusList.TabIndex = 4;
            this.BonusList.SelectedIndexChanged += new System.EventHandler(this.BonusList_SelectedIndexChanged);
            // 
            // Description
            // 
            this.Description.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Description.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Description.Location = new System.Drawing.Point(129, 249);
            this.Description.Multiline = true;
            this.Description.Name = "Description";
            this.Description.ReadOnly = true;
            this.Description.Size = new System.Drawing.Size(403, 52);
            this.Description.TabIndex = 5;
            // 
            // pbBarbed
            // 
            this.pbBarbed.Image = global::EntityTools.Properties.Resources.Insignia_Barbed;
            this.pbBarbed.Location = new System.Drawing.Point(68, 12);
            this.pbBarbed.Name = "pbBarbed";
            this.pbBarbed.Size = new System.Drawing.Size(20, 20);
            this.pbBarbed.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbBarbed.TabIndex = 7;
            this.pbBarbed.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::EntityTools.Properties.Resources.Insignia_Crescent;
            this.pictureBox1.Location = new System.Drawing.Point(121, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(20, 20);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::EntityTools.Properties.Resources.Insignia_Illuminated;
            this.pictureBox2.Location = new System.Drawing.Point(174, 12);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(20, 20);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 7;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::EntityTools.Properties.Resources.Insignia_Enlightened;
            this.pictureBox3.Location = new System.Drawing.Point(227, 12);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(20, 20);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 7;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = global::EntityTools.Properties.Resources.Insignia_Regal;
            this.pictureBox4.Location = new System.Drawing.Point(280, 12);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(20, 20);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox4.TabIndex = 7;
            this.pictureBox4.TabStop = false;
            // 
            // Insignia1
            // 
            this.Insignia1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Insignia1.Location = new System.Drawing.Point(15, 249);
            this.Insignia1.Name = "Insignia1";
            this.Insignia1.Size = new System.Drawing.Size(32, 32);
            this.Insignia1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Insignia1.TabIndex = 8;
            this.Insignia1.TabStop = false;
            // 
            // Insignia2
            // 
            this.Insignia2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Insignia2.Location = new System.Drawing.Point(53, 249);
            this.Insignia2.Name = "Insignia2";
            this.Insignia2.Size = new System.Drawing.Size(32, 32);
            this.Insignia2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Insignia2.TabIndex = 8;
            this.Insignia2.TabStop = false;
            // 
            // Insignia3
            // 
            this.Insignia3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Insignia3.Location = new System.Drawing.Point(91, 249);
            this.Insignia3.Name = "Insignia3";
            this.Insignia3.Size = new System.Drawing.Size(32, 32);
            this.Insignia3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Insignia3.TabIndex = 8;
            this.Insignia3.TabStop = false;
            // 
            // InsigniaBonusSelectForm
            // 
            this.AcceptButton = this.btnSelect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(547, 358);
            this.ControlBox = false;
            this.Controls.Add(this.Insignia3);
            this.Controls.Add(this.Insignia2);
            this.Controls.Add(this.Insignia1);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.pbBarbed);
            this.Controls.Add(this.Description);
            this.Controls.Add(this.BonusList);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnFilter);
            this.Controls.Add(this.tbRegal);
            this.Controls.Add(this.tbEnlightened);
            this.Controls.Add(this.tbllluminated);
            this.Controls.Add(this.tbCrescent);
            this.Controls.Add(this.tbBarbed);
            this.Controls.Add(this.lbFilter);
            this.MinimumSize = new System.Drawing.Size(555, 385);
            this.Name = "InsigniaBonusSelectForm";
            this.Text = "InsigniaBonuses";
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBarbed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Insignia1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Insignia2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Insignia3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lbFilter;
        private System.Windows.Forms.TextBox tbBarbed;
        private System.Windows.Forms.TextBox tbCrescent;
        private System.Windows.Forms.TextBox tbllluminated;
        private System.Windows.Forms.TextBox tbEnlightened;
        private System.Windows.Forms.TextBox tbRegal;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.ListBox BonusList;
        private System.Windows.Forms.TextBox Description;
        private System.Windows.Forms.PictureBox pbBarbed;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.PictureBox Insignia1;
        private System.Windows.Forms.PictureBox Insignia2;
        private System.Windows.Forms.PictureBox Insignia3;
        private System.Windows.Forms.BindingSource bindingSource;
    }
}