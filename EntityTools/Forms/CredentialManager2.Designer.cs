
namespace EntityTools.Forms
{
    partial class CredentialManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CredentialManager));
            this.btnLoad = new DevExpress.XtraEditors.SimpleButton();
            this.splitContainerControl = new DevExpress.XtraEditors.SplitContainerControl();
            this.btnAccountDelete = new DevExpress.XtraEditors.SimpleButton();
            this.btnLoginImport = new DevExpress.XtraEditors.SimpleButton();
            this.bntPasswordShow = new DevExpress.XtraEditors.SimpleButton();
            this.btnAccountImport = new DevExpress.XtraEditors.SimpleButton();
            this.btnAccountDown = new DevExpress.XtraEditors.SimpleButton();
            this.btnAccountUp = new DevExpress.XtraEditors.SimpleButton();
            this.btnAccountSort = new DevExpress.XtraEditors.SimpleButton();
            this.btnAccountEdit = new DevExpress.XtraEditors.SimpleButton();
            this.btnAccountAdd = new DevExpress.XtraEditors.SimpleButton();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblLogin = new System.Windows.Forms.Label();
            this.lblAccounts = new System.Windows.Forms.Label();
            this.tbPassword = new DevExpress.XtraEditors.TextEdit();
            this.tbLogin = new DevExpress.XtraEditors.TextEdit();
            this.listAccounts = new DevExpress.XtraEditors.ListBoxControl();
            this.lblCharacters = new System.Windows.Forms.Label();
            this.btnCharacterImport = new DevExpress.XtraEditors.SimpleButton();
            this.btnCharacterDelete = new DevExpress.XtraEditors.SimpleButton();
            this.listCharacters = new DevExpress.XtraEditors.ListBoxControl();
            this.btnCharacterSort = new DevExpress.XtraEditors.SimpleButton();
            this.btnCharacterDown = new DevExpress.XtraEditors.SimpleButton();
            this.btnCharacterUp = new DevExpress.XtraEditors.SimpleButton();
            this.btnCharacterEdit = new DevExpress.XtraEditors.SimpleButton();
            this.btnCharacterAdd = new DevExpress.XtraEditors.SimpleButton();
            this.lblMachineId = new System.Windows.Forms.Label();
            this.btnMachineIdImport = new DevExpress.XtraEditors.SimpleButton();
            this.tbMachineId = new DevExpress.XtraEditors.TextEdit();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.bindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl.Panel1)).BeginInit();
            this.splitContainerControl.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl.Panel2)).BeginInit();
            this.splitContainerControl.Panel2.SuspendLayout();
            this.splitContainerControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbPassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbLogin.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.listAccounts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.listCharacters)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbMachineId.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoad.ImageOptions.Image = global::EntityTools.Properties.Resources.miniLoad;
            this.btnLoad.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnLoad.Location = new System.Drawing.Point(348, 233);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnLoad.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.btnLoad.Size = new System.Drawing.Size(50, 23);
            this.btnLoad.TabIndex = 3;
            this.btnLoad.Text = "Load";
            // 
            // splitContainerControl
            // 
            this.splitContainerControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainerControl.Location = new System.Drawing.Point(12, 3);
            this.splitContainerControl.Name = "splitContainerControl";
            // 
            // splitContainerControl.Panel1
            // 
            this.splitContainerControl.Panel1.Controls.Add(this.btnAccountDelete);
            this.splitContainerControl.Panel1.Controls.Add(this.btnLoginImport);
            this.splitContainerControl.Panel1.Controls.Add(this.bntPasswordShow);
            this.splitContainerControl.Panel1.Controls.Add(this.btnAccountImport);
            this.splitContainerControl.Panel1.Controls.Add(this.btnAccountDown);
            this.splitContainerControl.Panel1.Controls.Add(this.btnAccountUp);
            this.splitContainerControl.Panel1.Controls.Add(this.btnAccountSort);
            this.splitContainerControl.Panel1.Controls.Add(this.btnAccountEdit);
            this.splitContainerControl.Panel1.Controls.Add(this.btnAccountAdd);
            this.splitContainerControl.Panel1.Controls.Add(this.lblPassword);
            this.splitContainerControl.Panel1.Controls.Add(this.lblLogin);
            this.splitContainerControl.Panel1.Controls.Add(this.lblAccounts);
            this.splitContainerControl.Panel1.Controls.Add(this.tbPassword);
            this.splitContainerControl.Panel1.Controls.Add(this.tbLogin);
            this.splitContainerControl.Panel1.Controls.Add(this.listAccounts);
            this.splitContainerControl.Panel1.MinSize = 220;
            this.splitContainerControl.Panel1.Text = "Panel1";
            // 
            // splitContainerControl.Panel2
            // 
            this.splitContainerControl.Panel2.Controls.Add(this.lblCharacters);
            this.splitContainerControl.Panel2.Controls.Add(this.btnCharacterImport);
            this.splitContainerControl.Panel2.Controls.Add(this.btnCharacterDelete);
            this.splitContainerControl.Panel2.Controls.Add(this.listCharacters);
            this.splitContainerControl.Panel2.Controls.Add(this.btnCharacterSort);
            this.splitContainerControl.Panel2.Controls.Add(this.btnCharacterDown);
            this.splitContainerControl.Panel2.Controls.Add(this.btnCharacterUp);
            this.splitContainerControl.Panel2.Controls.Add(this.btnCharacterEdit);
            this.splitContainerControl.Panel2.Controls.Add(this.btnCharacterAdd);
            this.splitContainerControl.Panel2.MinSize = 230;
            this.splitContainerControl.Panel2.Text = "Panel2";
            this.splitContainerControl.Size = new System.Drawing.Size(504, 198);
            this.splitContainerControl.SplitterPosition = 263;
            this.splitContainerControl.TabIndex = 0;
            // 
            // btnAccountDelete
            // 
            this.btnAccountDelete.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnAccountDelete.ImageOptions.Image")));
            this.btnAccountDelete.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
            this.btnAccountDelete.Location = new System.Drawing.Point(112, 7);
            this.btnAccountDelete.Name = "btnAccountDelete";
            this.btnAccountDelete.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnAccountDelete.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.btnAccountDelete.Size = new System.Drawing.Size(20, 20);
            this.btnAccountDelete.TabIndex = 3;
            this.btnAccountDelete.ToolTip = "Delete selected account record";
            // 
            // btnLoginImport
            // 
            this.btnLoginImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoginImport.Enabled = false;
            this.btnLoginImport.ImageOptions.Image = global::EntityTools.Properties.Resources.miniImport;
            this.btnLoginImport.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
            this.btnLoginImport.Location = new System.Drawing.Point(243, 149);
            this.btnLoginImport.Name = "btnLoginImport";
            this.btnLoginImport.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnLoginImport.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.btnLoginImport.Size = new System.Drawing.Size(20, 20);
            this.btnLoginImport.TabIndex = 3;
            this.btnLoginImport.ToolTip = "Import currently loaded account\'s login";
            this.btnLoginImport.Click += new System.EventHandler(this.handler_ImportLogin);
            // 
            // bntPasswordShow
            // 
            this.bntPasswordShow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bntPasswordShow.ImageOptions.Image = global::EntityTools.Properties.Resources.miniShow;
            this.bntPasswordShow.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
            this.bntPasswordShow.Location = new System.Drawing.Point(243, 175);
            this.bntPasswordShow.Name = "bntPasswordShow";
            this.bntPasswordShow.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.bntPasswordShow.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.bntPasswordShow.Size = new System.Drawing.Size(20, 20);
            this.bntPasswordShow.TabIndex = 3;
            this.bntPasswordShow.ToolTip = "Show password";
            this.bntPasswordShow.Click += new System.EventHandler(this.handler_ChangePasswordVisibility);
            // 
            // btnAccountImport
            // 
            this.btnAccountImport.ImageOptions.Image = global::EntityTools.Properties.Resources.miniImport;
            this.btnAccountImport.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
            this.btnAccountImport.Location = new System.Drawing.Point(134, 7);
            this.btnAccountImport.Name = "btnAccountImport";
            this.btnAccountImport.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnAccountImport.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.btnAccountImport.Size = new System.Drawing.Size(20, 20);
            this.btnAccountImport.TabIndex = 3;
            this.btnAccountImport.ToolTip = "Import currently loaded account data from the game";
            this.btnAccountImport.Click += new System.EventHandler(this.btnAccountImport_Click);
            // 
            // btnAccountDown
            // 
            this.btnAccountDown.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnAccountDown.ImageOptions.Image")));
            this.btnAccountDown.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
            this.btnAccountDown.Location = new System.Drawing.Point(178, 7);
            this.btnAccountDown.Name = "btnAccountDown";
            this.btnAccountDown.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnAccountDown.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.btnAccountDown.Size = new System.Drawing.Size(20, 20);
            this.btnAccountDown.TabIndex = 3;
            this.btnAccountDown.ToolTip = "Move account record to the down";
            // 
            // btnAccountUp
            // 
            this.btnAccountUp.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnAccountUp.ImageOptions.Image")));
            this.btnAccountUp.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
            this.btnAccountUp.Location = new System.Drawing.Point(156, 7);
            this.btnAccountUp.Name = "btnAccountUp";
            this.btnAccountUp.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnAccountUp.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.btnAccountUp.Size = new System.Drawing.Size(20, 20);
            this.btnAccountUp.TabIndex = 3;
            this.btnAccountUp.ToolTip = "Move account record to the up";
            // 
            // btnAccountSort
            // 
            this.btnAccountSort.ImageOptions.Image = global::EntityTools.Properties.Resources.miniReverse;
            this.btnAccountSort.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
            this.btnAccountSort.Location = new System.Drawing.Point(200, 7);
            this.btnAccountSort.Name = "btnAccountSort";
            this.btnAccountSort.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnAccountSort.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.btnAccountSort.Size = new System.Drawing.Size(20, 20);
            this.btnAccountSort.TabIndex = 3;
            this.btnAccountSort.ToolTip = "Sort the account list";
            // 
            // btnAccountEdit
            // 
            this.btnAccountEdit.ImageOptions.Image = global::EntityTools.Properties.Resources.miniPen;
            this.btnAccountEdit.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
            this.btnAccountEdit.Location = new System.Drawing.Point(90, 7);
            this.btnAccountEdit.Name = "btnAccountEdit";
            this.btnAccountEdit.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnAccountEdit.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.btnAccountEdit.Size = new System.Drawing.Size(20, 20);
            this.btnAccountEdit.TabIndex = 3;
            this.btnAccountEdit.ToolTip = "Edit account name";
            // 
            // btnAccountAdd
            // 
            this.btnAccountAdd.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnAccountAdd.ImageOptions.Image")));
            this.btnAccountAdd.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
            this.btnAccountAdd.Location = new System.Drawing.Point(68, 7);
            this.btnAccountAdd.Name = "btnAccountAdd";
            this.btnAccountAdd.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnAccountAdd.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.btnAccountAdd.Size = new System.Drawing.Size(20, 20);
            this.btnAccountAdd.TabIndex = 3;
            this.btnAccountAdd.ToolTip = "Add empty account record";
            // 
            // lblPassword
            // 
            this.lblPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(3, 178);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(57, 13);
            this.lblPassword.TabIndex = 2;
            this.lblPassword.Text = "Password:";
            // 
            // lblLogin
            // 
            this.lblLogin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLogin.AutoSize = true;
            this.lblLogin.Location = new System.Drawing.Point(3, 152);
            this.lblLogin.Name = "lblLogin";
            this.lblLogin.Size = new System.Drawing.Size(36, 13);
            this.lblLogin.TabIndex = 2;
            this.lblLogin.Text = "Login:";
            // 
            // lblAccounts
            // 
            this.lblAccounts.AutoSize = true;
            this.lblAccounts.Location = new System.Drawing.Point(7, 9);
            this.lblAccounts.Name = "lblAccounts";
            this.lblAccounts.Size = new System.Drawing.Size(55, 13);
            this.lblAccounts.TabIndex = 2;
            this.lblAccounts.Text = "Accounts:";
            // 
            // tbPassword
            // 
            this.tbPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPassword.EditValue = "o-3094867;asldkf";
            this.tbPassword.Location = new System.Drawing.Point(66, 175);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Properties.UseSystemPasswordChar = true;
            this.tbPassword.Size = new System.Drawing.Size(171, 20);
            this.tbPassword.TabIndex = 1;
            // 
            // tbLogin
            // 
            this.tbLogin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbLogin.EditValue = "Account_01@mail.com";
            this.tbLogin.Location = new System.Drawing.Point(66, 149);
            this.tbLogin.Name = "tbLogin";
            this.tbLogin.Size = new System.Drawing.Size(171, 20);
            this.tbLogin.TabIndex = 1;
            // 
            // listAccounts
            // 
            this.listAccounts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listAccounts.Items.AddRange(new object[] {
            "Account_01",
            "Account_02",
            "Account_03",
            "Account_04",
            "Account_05",
            "Account_06",
            "Account_07",
            "Account_08",
            "Account_09",
            "Account_10"});
            this.listAccounts.Location = new System.Drawing.Point(3, 29);
            this.listAccounts.Name = "listAccounts";
            this.listAccounts.Size = new System.Drawing.Size(260, 114);
            this.listAccounts.TabIndex = 0;
            this.listAccounts.SelectedValueChanged += new System.EventHandler(this.handler_SelectedAccountChanged);
            // 
            // lblCharacters
            // 
            this.lblCharacters.AutoSize = true;
            this.lblCharacters.Location = new System.Drawing.Point(3, 9);
            this.lblCharacters.Name = "lblCharacters";
            this.lblCharacters.Size = new System.Drawing.Size(64, 13);
            this.lblCharacters.TabIndex = 2;
            this.lblCharacters.Text = "Characters:";
            // 
            // btnCharacterImport
            // 
            this.btnCharacterImport.ImageOptions.Image = global::EntityTools.Properties.Resources.miniImport;
            this.btnCharacterImport.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
            this.btnCharacterImport.Location = new System.Drawing.Point(139, 7);
            this.btnCharacterImport.Name = "btnCharacterImport";
            this.btnCharacterImport.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnCharacterImport.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.btnCharacterImport.Size = new System.Drawing.Size(20, 20);
            this.btnCharacterImport.TabIndex = 3;
            this.btnCharacterImport.ToolTip = "Import from the game the characters of the loaded account";
            this.btnCharacterImport.Click += new System.EventHandler(this.handler_ImportCharacters);
            // 
            // btnCharacterDelete
            // 
            this.btnCharacterDelete.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCharacterDelete.ImageOptions.Image")));
            this.btnCharacterDelete.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
            this.btnCharacterDelete.Location = new System.Drawing.Point(117, 7);
            this.btnCharacterDelete.Name = "btnCharacterDelete";
            this.btnCharacterDelete.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnCharacterDelete.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.btnCharacterDelete.Size = new System.Drawing.Size(20, 20);
            this.btnCharacterDelete.TabIndex = 3;
            this.btnCharacterDelete.ToolTip = "Delete selected character";
            // 
            // listCharacters
            // 
            this.listCharacters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listCharacters.Items.AddRange(new object[] {
            "Character_1",
            "Character_2",
            "Character_3",
            "Character_4",
            "Character_5",
            "Character_6",
            "Character_7",
            "Character_8",
            "Character_9",
            "Character_10",
            "Character_11",
            "Character_12",
            "Character_13",
            "Character_14"});
            this.listCharacters.Location = new System.Drawing.Point(0, 29);
            this.listCharacters.Name = "listCharacters";
            this.listCharacters.Size = new System.Drawing.Size(228, 169);
            this.listCharacters.TabIndex = 0;
            // 
            // btnCharacterSort
            // 
            this.btnCharacterSort.ImageOptions.Image = global::EntityTools.Properties.Resources.miniReverse;
            this.btnCharacterSort.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
            this.btnCharacterSort.Location = new System.Drawing.Point(207, 7);
            this.btnCharacterSort.Name = "btnCharacterSort";
            this.btnCharacterSort.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnCharacterSort.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.btnCharacterSort.Size = new System.Drawing.Size(20, 20);
            this.btnCharacterSort.TabIndex = 3;
            this.btnCharacterSort.ToolTip = "Sort the character list";
            // 
            // btnCharacterDown
            // 
            this.btnCharacterDown.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCharacterDown.ImageOptions.Image")));
            this.btnCharacterDown.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
            this.btnCharacterDown.Location = new System.Drawing.Point(185, 7);
            this.btnCharacterDown.Name = "btnCharacterDown";
            this.btnCharacterDown.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnCharacterDown.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.btnCharacterDown.Size = new System.Drawing.Size(20, 20);
            this.btnCharacterDown.TabIndex = 3;
            this.btnCharacterDown.ToolTip = "Move character to the down";
            // 
            // btnCharacterUp
            // 
            this.btnCharacterUp.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCharacterUp.ImageOptions.Image")));
            this.btnCharacterUp.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
            this.btnCharacterUp.Location = new System.Drawing.Point(163, 7);
            this.btnCharacterUp.Name = "btnCharacterUp";
            this.btnCharacterUp.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnCharacterUp.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.btnCharacterUp.Size = new System.Drawing.Size(20, 20);
            this.btnCharacterUp.TabIndex = 3;
            this.btnCharacterUp.ToolTip = "Move character to the up";
            // 
            // btnCharacterEdit
            // 
            this.btnCharacterEdit.ImageOptions.Image = global::EntityTools.Properties.Resources.miniPen;
            this.btnCharacterEdit.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
            this.btnCharacterEdit.Location = new System.Drawing.Point(95, 7);
            this.btnCharacterEdit.Name = "btnCharacterEdit";
            this.btnCharacterEdit.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnCharacterEdit.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.btnCharacterEdit.Size = new System.Drawing.Size(20, 20);
            this.btnCharacterEdit.TabIndex = 3;
            this.btnCharacterEdit.ToolTip = "Edit character name";
            // 
            // btnCharacterAdd
            // 
            this.btnCharacterAdd.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCharacterAdd.ImageOptions.Image")));
            this.btnCharacterAdd.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
            this.btnCharacterAdd.Location = new System.Drawing.Point(73, 7);
            this.btnCharacterAdd.Name = "btnCharacterAdd";
            this.btnCharacterAdd.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnCharacterAdd.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.btnCharacterAdd.Size = new System.Drawing.Size(20, 20);
            this.btnCharacterAdd.TabIndex = 3;
            this.btnCharacterAdd.ToolTip = "Add empty character record";
            // 
            // lblMachineId
            // 
            this.lblMachineId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblMachineId.AutoSize = true;
            this.lblMachineId.Location = new System.Drawing.Point(15, 210);
            this.lblMachineId.Name = "lblMachineId";
            this.lblMachineId.Size = new System.Drawing.Size(60, 13);
            this.lblMachineId.TabIndex = 2;
            this.lblMachineId.Text = "MachineId:";
            // 
            // btnMachineIdImport
            // 
            this.btnMachineIdImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMachineIdImport.ImageOptions.Image = global::EntityTools.Properties.Resources.miniImport;
            this.btnMachineIdImport.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
            this.btnMachineIdImport.Location = new System.Drawing.Point(493, 207);
            this.btnMachineIdImport.Name = "btnMachineIdImport";
            this.btnMachineIdImport.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnMachineIdImport.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.btnMachineIdImport.Size = new System.Drawing.Size(20, 20);
            this.btnMachineIdImport.TabIndex = 3;
            this.btnMachineIdImport.ToolTip = "Import MachineId from the windows registry";
            this.btnMachineIdImport.Click += new System.EventHandler(this.handler_ImportMachineId);
            // 
            // tbMachineId
            // 
            this.tbMachineId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbMachineId.EditValue = "B4A993BF5A32A626389875D4B836D658B054BD1B25CAFCA53ECDEAD79774ECB4";
            this.tbMachineId.Location = new System.Drawing.Point(78, 207);
            this.tbMachineId.Name = "tbMachineId";
            this.tbMachineId.Size = new System.Drawing.Size(409, 20);
            this.tbMachineId.TabIndex = 0;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.ImageOptions.Image = global::EntityTools.Properties.Resources.miniSave;
            this.btnSave.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnSave.Location = new System.Drawing.Point(404, 233);
            this.btnSave.Name = "btnSave";
            this.btnSave.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnSave.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.btnSave.Size = new System.Drawing.Size(50, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Enabled = false;
            this.btnCancel.ImageOptions.Image = global::EntityTools.Properties.Resources.miniCancel;
            this.btnCancel.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnCancel.Location = new System.Drawing.Point(460, 233);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnCancel.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.btnCancel.Size = new System.Drawing.Size(56, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            // 
            // CredentialManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(528, 268);
            this.Controls.Add(this.lblMachineId);
            this.Controls.Add(this.btnMachineIdImport);
            this.Controls.Add(this.splitContainerControl);
            this.Controls.Add(this.tbMachineId);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.MinimumSize = new System.Drawing.Size(530, 230);
            this.Name = "CredentialManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CredentialManager";
            this.Load += new System.EventHandler(this.handler_Load);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl.Panel1)).EndInit();
            this.splitContainerControl.Panel1.ResumeLayout(false);
            this.splitContainerControl.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl.Panel2)).EndInit();
            this.splitContainerControl.Panel2.ResumeLayout(false);
            this.splitContainerControl.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl)).EndInit();
            this.splitContainerControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tbPassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbLogin.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.listAccounts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.listCharacters)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbMachineId.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnLoad;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl;
        private System.Windows.Forms.Label lblMachineId;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Label lblLogin;
        private System.Windows.Forms.Label lblAccounts;
        private DevExpress.XtraEditors.TextEdit tbPassword;
        private DevExpress.XtraEditors.TextEdit tbLogin;
        private DevExpress.XtraEditors.ListBoxControl listAccounts;
        private System.Windows.Forms.Label lblCharacters;
        private DevExpress.XtraEditors.ListBoxControl listCharacters;
        private DevExpress.XtraEditors.SimpleButton btnAccountDelete;
        private DevExpress.XtraEditors.SimpleButton btnAccountDown;
        private DevExpress.XtraEditors.SimpleButton btnAccountUp;
        private DevExpress.XtraEditors.SimpleButton btnAccountAdd;
        private DevExpress.XtraEditors.SimpleButton btnCharacterDelete;
        private DevExpress.XtraEditors.SimpleButton btnCharacterDown;
        private DevExpress.XtraEditors.SimpleButton btnCharacterUp;
        private DevExpress.XtraEditors.SimpleButton btnCharacterAdd;
        private DevExpress.XtraEditors.TextEdit tbMachineId;
        private DevExpress.XtraEditors.SimpleButton btnCharacterImport;
        private DevExpress.XtraEditors.SimpleButton btnAccountImport;
        private DevExpress.XtraEditors.SimpleButton btnAccountSort;
        private DevExpress.XtraEditors.SimpleButton btnCharacterSort;
        private DevExpress.XtraEditors.SimpleButton btnMachineIdImport;
        private System.Windows.Forms.BindingSource bindingSource;
        private DevExpress.XtraEditors.SimpleButton btnLoginImport;
        private DevExpress.XtraEditors.SimpleButton bntPasswordShow;
        private DevExpress.XtraEditors.SimpleButton btnAccountEdit;
        private DevExpress.XtraEditors.SimpleButton btnCharacterEdit;
    }
}