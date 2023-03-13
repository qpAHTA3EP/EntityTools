
namespace EntityTools.Quester.Mapper.Tools
{
    partial class MapperSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapperSettingsForm));
            this.navBarCustomization = new DevExpress.XtraNavBar.NavBarControl();
            this.navGroupGeneral = new DevExpress.XtraNavBar.NavBarGroup();
            this.containerGeneral = new DevExpress.XtraNavBar.NavBarGroupControlContainer();
            this.lblLayerDepth = new System.Windows.Forms.Label();
            this.editLayerDepth = new DevExpress.XtraEditors.SpinEdit();
            this.ckbChacheEnable = new DevExpress.XtraEditors.CheckEdit();
            this.containerMeshes = new DevExpress.XtraNavBar.NavBarGroupControlContainer();
            this.lblBackground = new System.Windows.Forms.Label();
            this.lblUnidirPath = new System.Windows.Forms.Label();
            this.lblBidirPath = new System.Windows.Forms.Label();
            this.colorEditBidirPath = new DevExpress.XtraEditors.ColorEdit();
            this.colorBackground = new DevExpress.XtraEditors.ColorEdit();
            this.colorEditUnidirPath = new DevExpress.XtraEditors.ColorEdit();
            this.containerObjects = new DevExpress.XtraNavBar.NavBarGroupControlContainer();
            this.grpNodeCustomization = new DevExpress.XtraEditors.GroupControl();
            this.ckbNodes = new DevExpress.XtraEditors.CheckEdit();
            this.colorNodes = new DevExpress.XtraEditors.ColorEdit();
            this.ckbSkillnodes = new DevExpress.XtraEditors.CheckEdit();
            this.colorSkillnodes = new DevExpress.XtraEditors.ColorEdit();
            this.grpEntityCustomization = new DevExpress.XtraEditors.GroupControl();
            this.ckbEnemies = new DevExpress.XtraEditors.CheckEdit();
            this.ckbOtherNPC = new DevExpress.XtraEditors.CheckEdit();
            this.colorEnemies = new DevExpress.XtraEditors.ColorEdit();
            this.ckbPlayers = new DevExpress.XtraEditors.CheckEdit();
            this.colorFriends = new DevExpress.XtraEditors.ColorEdit();
            this.colorPlayers = new DevExpress.XtraEditors.ColorEdit();
            this.colorOtherNPC = new DevExpress.XtraEditors.ColorEdit();
            this.ckbFriends = new DevExpress.XtraEditors.CheckEdit();
            this.navGroupMeshes = new DevExpress.XtraNavBar.NavBarGroup();
            this.navGroupObjects = new DevExpress.XtraNavBar.NavBarGroup();
            ((System.ComponentModel.ISupportInitialize)(this.navBarCustomization)).BeginInit();
            this.navBarCustomization.SuspendLayout();
            this.containerGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.editLayerDepth.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckbChacheEnable.Properties)).BeginInit();
            this.containerMeshes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.colorEditBidirPath.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorBackground.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorEditUnidirPath.Properties)).BeginInit();
            this.containerObjects.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grpNodeCustomization)).BeginInit();
            this.grpNodeCustomization.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ckbNodes.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorNodes.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckbSkillnodes.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorSkillnodes.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpEntityCustomization)).BeginInit();
            this.grpEntityCustomization.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ckbEnemies.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckbOtherNPC.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorEnemies.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckbPlayers.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorFriends.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorPlayers.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorOtherNPC.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckbFriends.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // navBarCustomization
            // 
            this.navBarCustomization.ActiveGroup = this.navGroupGeneral;
            this.navBarCustomization.Controls.Add(this.containerMeshes);
            this.navBarCustomization.Controls.Add(this.containerObjects);
            this.navBarCustomization.Controls.Add(this.containerGeneral);
            this.navBarCustomization.Dock = System.Windows.Forms.DockStyle.Fill;
            this.navBarCustomization.Groups.AddRange(new DevExpress.XtraNavBar.NavBarGroup[] {
            this.navGroupGeneral,
            this.navGroupMeshes,
            this.navGroupObjects});
            this.navBarCustomization.Location = new System.Drawing.Point(0, 0);
            this.navBarCustomization.Name = "navBarCustomization";
            this.navBarCustomization.OptionsNavPane.ExpandedWidth = 393;
            this.navBarCustomization.Size = new System.Drawing.Size(393, 363);
            this.navBarCustomization.TabIndex = 3;
            this.navBarCustomization.Text = "Customization";
            // 
            // navGroupGeneral
            // 
            this.navGroupGeneral.Caption = "General";
            this.navGroupGeneral.ControlContainer = this.containerGeneral;
            this.navGroupGeneral.Expanded = true;
            this.navGroupGeneral.GroupClientHeight = 64;
            this.navGroupGeneral.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.ControlContainer;
            this.navGroupGeneral.Name = "navGroupGeneral";
            // 
            // containerGeneral
            // 
            this.containerGeneral.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.containerGeneral.Appearance.Options.UseBackColor = true;
            this.containerGeneral.Controls.Add(this.lblLayerDepth);
            this.containerGeneral.Controls.Add(this.editLayerDepth);
            this.containerGeneral.Controls.Add(this.ckbChacheEnable);
            this.containerGeneral.Name = "containerGeneral";
            this.containerGeneral.Size = new System.Drawing.Size(393, 64);
            this.containerGeneral.TabIndex = 2;
            // 
            // lblLayerDepth
            // 
            this.lblLayerDepth.AutoSize = true;
            this.lblLayerDepth.Location = new System.Drawing.Point(12, 37);
            this.lblLayerDepth.Name = "lblLayerDepth";
            this.lblLayerDepth.Size = new System.Drawing.Size(238, 13);
            this.lblLayerDepth.TabIndex = 3;
            this.lblLayerDepth.Text = "The limit of the layer\'s depth drawing on Mapper";
            // 
            // editLayerDepth
            // 
            this.editLayerDepth.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.editLayerDepth.Location = new System.Drawing.Point(256, 34);
            this.editLayerDepth.Name = "editLayerDepth";
            this.editLayerDepth.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.editLayerDepth.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.editLayerDepth.Properties.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.editLayerDepth.Properties.MaxValue = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.editLayerDepth.Size = new System.Drawing.Size(120, 20);
            this.editLayerDepth.TabIndex = 2;
            this.editLayerDepth.EditValueChanged += new System.EventHandler(this.handler_LayerDepth_Changed);
            // 
            // ckbChacheEnable
            // 
            this.ckbChacheEnable.Location = new System.Drawing.Point(12, 10);
            this.ckbChacheEnable.Name = "ckbChacheEnable";
            this.ckbChacheEnable.Properties.Caption = "Caching of the visible meshes (will take effect in a new Mapper window)";
            this.ckbChacheEnable.Size = new System.Drawing.Size(374, 20);
            this.ckbChacheEnable.TabIndex = 0;
            // 
            // containerMeshes
            // 
            this.containerMeshes.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.containerMeshes.Appearance.Options.UseBackColor = true;
            this.containerMeshes.Controls.Add(this.lblBackground);
            this.containerMeshes.Controls.Add(this.lblUnidirPath);
            this.containerMeshes.Controls.Add(this.lblBidirPath);
            this.containerMeshes.Controls.Add(this.colorEditBidirPath);
            this.containerMeshes.Controls.Add(this.colorBackground);
            this.containerMeshes.Controls.Add(this.colorEditUnidirPath);
            this.containerMeshes.Name = "containerMeshes";
            this.containerMeshes.Size = new System.Drawing.Size(393, 79);
            this.containerMeshes.TabIndex = 0;
            // 
            // lblBackground
            // 
            this.lblBackground.Image = ((System.Drawing.Image)(resources.GetObject("lblBackground.Image")));
            this.lblBackground.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblBackground.Location = new System.Drawing.Point(5, 56);
            this.lblBackground.Name = "lblBackground";
            this.lblBackground.Size = new System.Drawing.Size(139, 16);
            this.lblBackground.TabIndex = 2;
            this.lblBackground.Text = "        Background";
            // 
            // lblUnidirPath
            // 
            this.lblUnidirPath.Image = ((System.Drawing.Image)(resources.GetObject("lblUnidirPath.Image")));
            this.lblUnidirPath.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblUnidirPath.Location = new System.Drawing.Point(5, 30);
            this.lblUnidirPath.Name = "lblUnidirPath";
            this.lblUnidirPath.Size = new System.Drawing.Size(139, 16);
            this.lblUnidirPath.TabIndex = 2;
            this.lblUnidirPath.Text = "        Unidirectional path color";
            // 
            // lblBidirPath
            // 
            this.lblBidirPath.Image = ((System.Drawing.Image)(resources.GetObject("lblBidirPath.Image")));
            this.lblBidirPath.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblBidirPath.Location = new System.Drawing.Point(5, 4);
            this.lblBidirPath.Name = "lblBidirPath";
            this.lblBidirPath.Size = new System.Drawing.Size(139, 16);
            this.lblBidirPath.TabIndex = 2;
            this.lblBidirPath.Text = "        Bidirectional path color";
            // 
            // colorEditBidirPath
            // 
            this.colorEditBidirPath.EditValue = System.Drawing.Color.Empty;
            this.colorEditBidirPath.Location = new System.Drawing.Point(150, 1);
            this.colorEditBidirPath.MaximumSize = new System.Drawing.Size(150, 20);
            this.colorEditBidirPath.MinimumSize = new System.Drawing.Size(45, 20);
            this.colorEditBidirPath.Name = "colorEditBidirPath";
            this.colorEditBidirPath.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.colorEditBidirPath.Size = new System.Drawing.Size(150, 20);
            this.colorEditBidirPath.TabIndex = 0;
            // 
            // colorBackground
            // 
            this.colorBackground.EditValue = System.Drawing.Color.Empty;
            this.colorBackground.Location = new System.Drawing.Point(150, 53);
            this.colorBackground.MaximumSize = new System.Drawing.Size(150, 20);
            this.colorBackground.MinimumSize = new System.Drawing.Size(45, 20);
            this.colorBackground.Name = "colorBackground";
            this.colorBackground.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.colorBackground.Size = new System.Drawing.Size(150, 20);
            this.colorBackground.TabIndex = 0;
            // 
            // colorEditUnidirPath
            // 
            this.colorEditUnidirPath.EditValue = System.Drawing.Color.Empty;
            this.colorEditUnidirPath.Location = new System.Drawing.Point(150, 27);
            this.colorEditUnidirPath.MaximumSize = new System.Drawing.Size(150, 20);
            this.colorEditUnidirPath.MinimumSize = new System.Drawing.Size(45, 20);
            this.colorEditUnidirPath.Name = "colorEditUnidirPath";
            this.colorEditUnidirPath.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.colorEditUnidirPath.Size = new System.Drawing.Size(150, 20);
            this.colorEditUnidirPath.TabIndex = 0;
            // 
            // containerObjects
            // 
            this.containerObjects.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.containerObjects.Appearance.Options.UseBackColor = true;
            this.containerObjects.Controls.Add(this.grpNodeCustomization);
            this.containerObjects.Controls.Add(this.grpEntityCustomization);
            this.containerObjects.Name = "containerObjects";
            this.containerObjects.Size = new System.Drawing.Size(393, 176);
            this.containerObjects.TabIndex = 1;
            // 
            // grpNodeCustomization
            // 
            this.grpNodeCustomization.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpNodeCustomization.CaptionLocation = DevExpress.Utils.Locations.Left;
            this.grpNodeCustomization.Controls.Add(this.ckbNodes);
            this.grpNodeCustomization.Controls.Add(this.colorNodes);
            this.grpNodeCustomization.Controls.Add(this.ckbSkillnodes);
            this.grpNodeCustomization.Controls.Add(this.colorSkillnodes);
            this.grpNodeCustomization.GroupStyle = DevExpress.Utils.GroupStyle.Card;
            this.grpNodeCustomization.Location = new System.Drawing.Point(3, 116);
            this.grpNodeCustomization.Name = "grpNodeCustomization";
            this.grpNodeCustomization.Size = new System.Drawing.Size(387, 55);
            this.grpNodeCustomization.TabIndex = 2;
            this.grpNodeCustomization.Text = "Nodes";
            // 
            // ckbNodes
            // 
            this.ckbNodes.Location = new System.Drawing.Point(25, 4);
            this.ckbNodes.Name = "ckbNodes";
            this.ckbNodes.Properties.Caption = "Draw Nodes";
            this.ckbNodes.Size = new System.Drawing.Size(96, 20);
            this.ckbNodes.TabIndex = 0;
            // 
            // colorNodes
            // 
            this.colorNodes.EditValue = System.Drawing.Color.Empty;
            this.colorNodes.Location = new System.Drawing.Point(127, 4);
            this.colorNodes.MaximumSize = new System.Drawing.Size(150, 20);
            this.colorNodes.MinimumSize = new System.Drawing.Size(45, 20);
            this.colorNodes.Name = "colorNodes";
            this.colorNodes.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.colorNodes.Size = new System.Drawing.Size(150, 20);
            this.colorNodes.TabIndex = 0;
            // 
            // ckbSkillnodes
            // 
            this.ckbSkillnodes.Location = new System.Drawing.Point(25, 30);
            this.ckbSkillnodes.Name = "ckbSkillnodes";
            this.ckbSkillnodes.Properties.Caption = "Draw Skillnodes";
            this.ckbSkillnodes.Size = new System.Drawing.Size(96, 20);
            this.ckbSkillnodes.TabIndex = 0;
            // 
            // colorSkillnodes
            // 
            this.colorSkillnodes.EditValue = System.Drawing.Color.Empty;
            this.colorSkillnodes.Location = new System.Drawing.Point(127, 30);
            this.colorSkillnodes.MaximumSize = new System.Drawing.Size(150, 20);
            this.colorSkillnodes.MinimumSize = new System.Drawing.Size(45, 20);
            this.colorSkillnodes.Name = "colorSkillnodes";
            this.colorSkillnodes.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.colorSkillnodes.Size = new System.Drawing.Size(150, 20);
            this.colorSkillnodes.TabIndex = 0;
            // 
            // grpEntityCustomization
            // 
            this.grpEntityCustomization.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpEntityCustomization.CaptionLocation = DevExpress.Utils.Locations.Left;
            this.grpEntityCustomization.Controls.Add(this.ckbEnemies);
            this.grpEntityCustomization.Controls.Add(this.ckbOtherNPC);
            this.grpEntityCustomization.Controls.Add(this.colorEnemies);
            this.grpEntityCustomization.Controls.Add(this.ckbPlayers);
            this.grpEntityCustomization.Controls.Add(this.colorFriends);
            this.grpEntityCustomization.Controls.Add(this.colorPlayers);
            this.grpEntityCustomization.Controls.Add(this.colorOtherNPC);
            this.grpEntityCustomization.Controls.Add(this.ckbFriends);
            this.grpEntityCustomization.GroupStyle = DevExpress.Utils.GroupStyle.Card;
            this.grpEntityCustomization.Location = new System.Drawing.Point(3, 3);
            this.grpEntityCustomization.Name = "grpEntityCustomization";
            this.grpEntityCustomization.Size = new System.Drawing.Size(387, 107);
            this.grpEntityCustomization.TabIndex = 1;
            this.grpEntityCustomization.Text = "Draw Entities";
            // 
            // ckbEnemies
            // 
            this.ckbEnemies.Location = new System.Drawing.Point(25, 4);
            this.ckbEnemies.Name = "ckbEnemies";
            this.ckbEnemies.Properties.Caption = "Enemies";
            this.ckbEnemies.Size = new System.Drawing.Size(62, 20);
            this.ckbEnemies.TabIndex = 0;
            // 
            // ckbOtherNPC
            // 
            this.ckbOtherNPC.Location = new System.Drawing.Point(25, 82);
            this.ckbOtherNPC.Name = "ckbOtherNPC";
            this.ckbOtherNPC.Properties.Caption = "Other";
            this.ckbOtherNPC.Size = new System.Drawing.Size(62, 20);
            this.ckbOtherNPC.TabIndex = 0;
            // 
            // colorEnemies
            // 
            this.colorEnemies.EditValue = System.Drawing.Color.Empty;
            this.colorEnemies.Location = new System.Drawing.Point(93, 4);
            this.colorEnemies.MaximumSize = new System.Drawing.Size(150, 20);
            this.colorEnemies.MinimumSize = new System.Drawing.Size(45, 20);
            this.colorEnemies.Name = "colorEnemies";
            this.colorEnemies.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.colorEnemies.Size = new System.Drawing.Size(150, 20);
            this.colorEnemies.TabIndex = 0;
            // 
            // ckbPlayers
            // 
            this.ckbPlayers.Location = new System.Drawing.Point(25, 56);
            this.ckbPlayers.Name = "ckbPlayers";
            this.ckbPlayers.Properties.Caption = "Players";
            this.ckbPlayers.Size = new System.Drawing.Size(62, 20);
            this.ckbPlayers.TabIndex = 0;
            // 
            // colorFriends
            // 
            this.colorFriends.EditValue = System.Drawing.Color.Empty;
            this.colorFriends.Location = new System.Drawing.Point(93, 30);
            this.colorFriends.MaximumSize = new System.Drawing.Size(150, 20);
            this.colorFriends.MinimumSize = new System.Drawing.Size(45, 20);
            this.colorFriends.Name = "colorFriends";
            this.colorFriends.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.colorFriends.Size = new System.Drawing.Size(150, 20);
            this.colorFriends.TabIndex = 0;
            // 
            // colorPlayers
            // 
            this.colorPlayers.EditValue = System.Drawing.Color.Empty;
            this.colorPlayers.Location = new System.Drawing.Point(93, 56);
            this.colorPlayers.MaximumSize = new System.Drawing.Size(150, 20);
            this.colorPlayers.MinimumSize = new System.Drawing.Size(45, 20);
            this.colorPlayers.Name = "colorPlayers";
            this.colorPlayers.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.colorPlayers.Size = new System.Drawing.Size(150, 20);
            this.colorPlayers.TabIndex = 0;
            // 
            // colorOtherNPC
            // 
            this.colorOtherNPC.EditValue = System.Drawing.Color.Empty;
            this.colorOtherNPC.Location = new System.Drawing.Point(93, 82);
            this.colorOtherNPC.MaximumSize = new System.Drawing.Size(150, 20);
            this.colorOtherNPC.MinimumSize = new System.Drawing.Size(45, 20);
            this.colorOtherNPC.Name = "colorOtherNPC";
            this.colorOtherNPC.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.colorOtherNPC.Size = new System.Drawing.Size(150, 20);
            this.colorOtherNPC.TabIndex = 0;
            // 
            // ckbFriends
            // 
            this.ckbFriends.Location = new System.Drawing.Point(25, 30);
            this.ckbFriends.Name = "ckbFriends";
            this.ckbFriends.Properties.Caption = "Friends";
            this.ckbFriends.Size = new System.Drawing.Size(62, 20);
            this.ckbFriends.TabIndex = 0;
            // 
            // navGroupMeshes
            // 
            this.navGroupMeshes.Caption = "Meshes";
            this.navGroupMeshes.ControlContainer = this.containerMeshes;
            this.navGroupMeshes.Expanded = true;
            this.navGroupMeshes.GroupClientHeight = 79;
            this.navGroupMeshes.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.ControlContainer;
            this.navGroupMeshes.Name = "navGroupMeshes";
            // 
            // navGroupObjects
            // 
            this.navGroupObjects.Caption = "Objects";
            this.navGroupObjects.ControlContainer = this.containerObjects;
            this.navGroupObjects.Expanded = true;
            this.navGroupObjects.GroupClientHeight = 176;
            this.navGroupObjects.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.ControlContainer;
            this.navGroupObjects.Name = "navGroupObjects";
            // 
            // MapperSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(393, 363);
            this.Controls.Add(this.navBarCustomization);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.IconOptions.ShowIcon = false;
            this.MinimumSize = new System.Drawing.Size(388, 395);
            this.Name = "MapperSettingsForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MapperSettings";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.navBarCustomization)).EndInit();
            this.navBarCustomization.ResumeLayout(false);
            this.containerGeneral.ResumeLayout(false);
            this.containerGeneral.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.editLayerDepth.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckbChacheEnable.Properties)).EndInit();
            this.containerMeshes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.colorEditBidirPath.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorBackground.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorEditUnidirPath.Properties)).EndInit();
            this.containerObjects.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grpNodeCustomization)).EndInit();
            this.grpNodeCustomization.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ckbNodes.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorNodes.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckbSkillnodes.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorSkillnodes.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpEntityCustomization)).EndInit();
            this.grpEntityCustomization.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ckbEnemies.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckbOtherNPC.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorEnemies.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckbPlayers.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorFriends.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorPlayers.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorOtherNPC.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckbFriends.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraNavBar.NavBarControl navBarCustomization;
        private DevExpress.XtraNavBar.NavBarGroup navGroupGeneral;
        private DevExpress.XtraNavBar.NavBarGroupControlContainer containerGeneral;
        private System.Windows.Forms.Label lblLayerDepth;
        private DevExpress.XtraEditors.SpinEdit editLayerDepth;
        private DevExpress.XtraEditors.CheckEdit ckbChacheEnable;
        private DevExpress.XtraNavBar.NavBarGroupControlContainer containerMeshes;
        private System.Windows.Forms.Label lblBackground;
        private System.Windows.Forms.Label lblUnidirPath;
        private System.Windows.Forms.Label lblBidirPath;
        private DevExpress.XtraEditors.ColorEdit colorEditBidirPath;
        private DevExpress.XtraEditors.ColorEdit colorBackground;
        private DevExpress.XtraEditors.ColorEdit colorEditUnidirPath;
        private DevExpress.XtraNavBar.NavBarGroupControlContainer containerObjects;
        private DevExpress.XtraEditors.GroupControl grpNodeCustomization;
        private DevExpress.XtraEditors.CheckEdit ckbNodes;
        private DevExpress.XtraEditors.ColorEdit colorNodes;
        private DevExpress.XtraEditors.CheckEdit ckbSkillnodes;
        private DevExpress.XtraEditors.ColorEdit colorSkillnodes;
        private DevExpress.XtraEditors.GroupControl grpEntityCustomization;
        private DevExpress.XtraEditors.CheckEdit ckbEnemies;
        private DevExpress.XtraEditors.CheckEdit ckbOtherNPC;
        private DevExpress.XtraEditors.ColorEdit colorEnemies;
        private DevExpress.XtraEditors.CheckEdit ckbPlayers;
        private DevExpress.XtraEditors.ColorEdit colorFriends;
        private DevExpress.XtraEditors.ColorEdit colorPlayers;
        private DevExpress.XtraEditors.ColorEdit colorOtherNPC;
        private DevExpress.XtraEditors.CheckEdit ckbFriends;
        private DevExpress.XtraNavBar.NavBarGroup navGroupMeshes;
        private DevExpress.XtraNavBar.NavBarGroup navGroupObjects;
    }
}