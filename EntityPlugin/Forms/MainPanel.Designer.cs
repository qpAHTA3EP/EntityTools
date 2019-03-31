namespace EntityPlugin.Forms
{
    partial class MainPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnEntities = new System.Windows.Forms.Button();
            this.ckbDebugInfo = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnEntities
            // 
            this.btnEntities.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEntities.Location = new System.Drawing.Point(3, 390);
            this.btnEntities.Name = "btnEntities";
            this.btnEntities.Size = new System.Drawing.Size(364, 23);
            this.btnEntities.TabIndex = 0;
            this.btnEntities.Text = "GetEntities";
            this.btnEntities.UseVisualStyleBackColor = true;
            this.btnEntities.Click += new System.EventHandler(this.btnEntities_Click);
            // 
            // ckbDebugInfo
            // 
            this.ckbDebugInfo.AutoSize = true;
            this.ckbDebugInfo.Location = new System.Drawing.Point(3, 43);
            this.ckbDebugInfo.Name = "ckbDebugInfo";
            this.ckbDebugInfo.Size = new System.Drawing.Size(200, 17);
            this.ckbDebugInfo.TabIndex = 1;
            this.ckbDebugInfo.Text = "Enable DubugInfo for InteractEntities";
            this.ckbDebugInfo.UseVisualStyleBackColor = true;
            // 
            // MainPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ckbDebugInfo);
            this.Controls.Add(this.btnEntities);
            this.Name = "MainPanel";
            this.Load += new System.EventHandler(this.MainPanel_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnEntities;
        private System.Windows.Forms.CheckBox ckbDebugInfo;
    }
}
