using EntityPlugin.Tools;
using System.IO;

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
            this.btnAuras = new System.Windows.Forms.Button();
            this.lblAuras = new System.Windows.Forms.Label();
            this.btnMissions = new System.Windows.Forms.Button();
            this.lblMissions = new System.Windows.Forms.Label();
            this.bteMissions = new DevExpress.XtraEditors.ButtonEdit();
            this.bteAuras = new DevExpress.XtraEditors.ButtonEdit();
            this.fldrBroserDlg = new System.Windows.Forms.FolderBrowserDialog();
            this.tbclMain = new DevExpress.XtraTab.XtraTabControl();
            this.tabUtilities = new DevExpress.XtraTab.XtraTabPage();
            this.tabOptions = new DevExpress.XtraTab.XtraTabPage();
            ((System.ComponentModel.ISupportInitialize)(this.bteMissions.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bteAuras.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbclMain)).BeginInit();
            this.tbclMain.SuspendLayout();
            this.tabUtilities.SuspendLayout();
            this.tabOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnEntities
            // 
            this.btnEntities.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEntities.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEntities.Location = new System.Drawing.Point(6, 113);
            this.btnEntities.Name = "btnEntities";
            this.btnEntities.Size = new System.Drawing.Size(352, 23);
            this.btnEntities.TabIndex = 0;
            this.btnEntities.Text = "Open the list of the Entities";
            this.btnEntities.UseVisualStyleBackColor = true;
            this.btnEntities.Visible = false;
            this.btnEntities.Click += new System.EventHandler(this.btnEntities_Click);
            // 
            // ckbDebugInfo
            // 
            this.ckbDebugInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ckbDebugInfo.AutoSize = true;
            this.ckbDebugInfo.Location = new System.Drawing.Point(3, 12);
            this.ckbDebugInfo.Name = "ckbDebugInfo";
            this.ckbDebugInfo.Size = new System.Drawing.Size(206, 17);
            this.ckbDebugInfo.TabIndex = 1;
            this.ckbDebugInfo.Text = "Enable DubugInfo for InteractEntities";
            this.ckbDebugInfo.UseVisualStyleBackColor = true;
            // 
            // btnAuras
            // 
            this.btnAuras.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAuras.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAuras.Location = new System.Drawing.Point(308, 26);
            this.btnAuras.Name = "btnAuras";
            this.btnAuras.Size = new System.Drawing.Size(50, 23);
            this.btnAuras.TabIndex = 2;
            this.btnAuras.Text = "Export";
            this.btnAuras.UseVisualStyleBackColor = true;
            this.btnAuras.Click += new System.EventHandler(this.btnAuras_Click);
            // 
            // lblAuras
            // 
            this.lblAuras.AutoSize = true;
            this.lblAuras.Location = new System.Drawing.Point(14, 12);
            this.lblAuras.Name = "lblAuras";
            this.lblAuras.Size = new System.Drawing.Size(142, 13);
            this.lblAuras.TabIndex = 3;
            this.lblAuras.Text = "Export current Auras to file:";
            // 
            // btnMissions
            // 
            this.btnMissions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMissions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMissions.Location = new System.Drawing.Point(308, 65);
            this.btnMissions.Name = "btnMissions";
            this.btnMissions.Size = new System.Drawing.Size(50, 23);
            this.btnMissions.TabIndex = 2;
            this.btnMissions.Text = "Export";
            this.btnMissions.UseVisualStyleBackColor = true;
            this.btnMissions.Click += new System.EventHandler(this.btnMissions_Click);
            // 
            // lblMissions
            // 
            this.lblMissions.AutoSize = true;
            this.lblMissions.Location = new System.Drawing.Point(14, 51);
            this.lblMissions.Name = "lblMissions";
            this.lblMissions.Size = new System.Drawing.Size(153, 13);
            this.lblMissions.TabIndex = 3;
            this.lblMissions.Text = "Export current Missions to file:";
            // 
            // bteMissions
            // 
            this.bteMissions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bteMissions.EditValue = "";
            this.bteMissions.Location = new System.Drawing.Point(6, 67);
            this.bteMissions.Name = "bteMissions";
            this.bteMissions.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.bteMissions.Properties.NullText = "Enter the Filename where store the Missions";
            this.bteMissions.Properties.NullValuePromptShowForEmptyValue = true;
            this.bteMissions.Properties.ReadOnly = true;
            this.bteMissions.Size = new System.Drawing.Size(296, 20);
            this.bteMissions.TabIndex = 6;
            this.bteMissions.ToolTip = "File name to store Missions of the current Character. Allow mask %character%, %ac" +
    "count%, %dateTime%.";
            this.bteMissions.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_ButtonClick);
            // 
            // bteAuras
            // 
            this.bteAuras.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bteAuras.EditValue = "";
            this.bteAuras.Location = new System.Drawing.Point(6, 28);
            this.bteAuras.Name = "bteAuras";
            this.bteAuras.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.bteAuras.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.bteAuras.Properties.NullText = "Enter the Filename where store the Auras";
            this.bteAuras.Properties.NullValuePromptShowForEmptyValue = true;
            this.bteAuras.Properties.ReadOnly = true;
            this.bteAuras.Size = new System.Drawing.Size(296, 20);
            this.bteAuras.TabIndex = 6;
            this.bteAuras.TabStop = false;
            this.bteAuras.ToolTip = "File name to store Auras of the current Character. Allow mask %character%, %accou" +
    "nt%, %dateTime%.";
            this.bteAuras.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_ButtonClick);
            // 
            // tbclMain
            // 
            this.tbclMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbclMain.Location = new System.Drawing.Point(0, 0);
            this.tbclMain.Name = "tbclMain";
            this.tbclMain.SelectedTabPage = this.tabUtilities;
            this.tbclMain.Size = new System.Drawing.Size(370, 416);
            this.tbclMain.TabIndex = 7;
            this.tbclMain.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.tabUtilities,
            this.tabOptions});
            // 
            // tabUtilities
            // 
            this.tabUtilities.Controls.Add(this.lblAuras);
            this.tabUtilities.Controls.Add(this.btnMissions);
            this.tabUtilities.Controls.Add(this.btnEntities);
            this.tabUtilities.Controls.Add(this.bteMissions);
            this.tabUtilities.Controls.Add(this.bteAuras);
            this.tabUtilities.Controls.Add(this.lblMissions);
            this.tabUtilities.Controls.Add(this.btnAuras);
            this.tabUtilities.Name = "tabUtilities";
            this.tabUtilities.Size = new System.Drawing.Size(364, 388);
            this.tabUtilities.Text = "Utilities";
            // 
            // tabOptions
            // 
            this.tabOptions.Controls.Add(this.ckbDebugInfo);
            this.tabOptions.Name = "tabOptions";
            this.tabOptions.Size = new System.Drawing.Size(364, 388);
            this.tabOptions.Text = "Options";
            // 
            // MainPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbclMain);
            this.Name = "MainPanel";
            this.Load += new System.EventHandler(this.MainPanel_Load);
            ((System.ComponentModel.ISupportInitialize)(this.bteMissions.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bteAuras.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbclMain)).EndInit();
            this.tbclMain.ResumeLayout(false);
            this.tabUtilities.ResumeLayout(false);
            this.tabUtilities.PerformLayout();
            this.tabOptions.ResumeLayout(false);
            this.tabOptions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnEntities;
        private System.Windows.Forms.CheckBox ckbDebugInfo;
        private System.Windows.Forms.Button btnAuras;
        private System.Windows.Forms.Label lblAuras;
        private System.Windows.Forms.Button btnMissions;
        private System.Windows.Forms.Label lblMissions;
        private DevExpress.XtraEditors.ButtonEdit bteMissions;
        private DevExpress.XtraEditors.ButtonEdit bteAuras;
        private System.Windows.Forms.FolderBrowserDialog fldrBroserDlg;
        private DevExpress.XtraTab.XtraTabControl tbclMain;
        private DevExpress.XtraTab.XtraTabPage tabUtilities;
        private DevExpress.XtraTab.XtraTabPage tabOptions;
    }
}
