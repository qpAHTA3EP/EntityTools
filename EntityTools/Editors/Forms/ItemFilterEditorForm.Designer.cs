﻿using Astral.Classes.ItemFilter;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace EntityTools.Forms
{
    partial class ItemFilterEditorForm
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
            this.purchaseOptions = new DevExpress.XtraGrid.GridControl();
            this.gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colStringType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colEntryType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colIdentifier = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnEditItemFilterEntry = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.colCount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCheckEquipmentLevel = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCheckPlayerLevel = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colKeepNumber = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPutOnItem = new DevExpress.XtraGrid.Columns.GridColumn();
            this.bindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.barManager = new DevExpress.XtraBars.BarManager(this.components);
            this.barDockControl_0 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl_1 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl_2 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl_3 = new DevExpress.XtraBars.BarDockControl();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.barEditItem = new DevExpress.XtraBars.BarEditItem();
            this.fastAddItemCombo = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.barListItem = new DevExpress.XtraBars.BarListItem();
            this.barToolbarsListItem = new DevExpress.XtraBars.BarToolbarsListItem();
            this.addEntryMenu = new DevExpress.XtraBars.PopupMenu(this.components);
            this.btnShowItems = new DevExpress.XtraEditors.SimpleButton();
            this.btnClear = new DevExpress.XtraEditors.SimpleButton();
            this.btnExport = new DevExpress.XtraEditors.SimpleButton();
            this.btnImport = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.bntSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnReload = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.purchaseOptions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnEditItemFilterEntry)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fastAddItemCombo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.addEntryMenu)).BeginInit();
            this.SuspendLayout();
            // 
            // purchaseOptions
            // 
            this.purchaseOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.purchaseOptions.Cursor = System.Windows.Forms.Cursors.Default;
            this.purchaseOptions.Location = new System.Drawing.Point(12, 12);
            this.purchaseOptions.MainView = this.gridView;
            this.purchaseOptions.Name = "purchaseOptions";
            this.purchaseOptions.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.btnEditItemFilterEntry});
            this.purchaseOptions.Size = new System.Drawing.Size(650, 230);
            this.purchaseOptions.TabIndex = 6;
            this.purchaseOptions.UseEmbeddedNavigator = true;
            this.purchaseOptions.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView});
            // 
            // gridView
            // 
            this.gridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colStringType,
            this.colEntryType,
            this.colMode,
            this.colIdentifier,
            this.colCount,
            this.colCheckEquipmentLevel,
            this.colCheckPlayerLevel,
            this.colKeepNumber,
            this.colPutOnItem});
            this.gridView.GridControl = this.purchaseOptions;
            this.gridView.Name = "gridView";
            this.gridView.OptionsView.ShowGroupPanel = false;
            this.gridView.OptionsView.ShowIndicator = false;
            // 
            // colStringType
            // 
            this.colStringType.Caption = "Text type";
            this.colStringType.FieldName = "StringType";
            this.colStringType.Name = "colStringType";
            this.colStringType.Visible = true;
            this.colStringType.VisibleIndex = 0;
            // 
            // colEntryType
            // 
            this.colEntryType.Caption = "Filter type";
            this.colEntryType.FieldName = "EntryType";
            this.colEntryType.Name = "colEntryType";
            this.colEntryType.Visible = true;
            this.colEntryType.VisibleIndex = 1;
            // 
            // colMode
            // 
            this.colMode.Caption = "Inclusion";
            this.colMode.FieldName = "Mode";
            this.colMode.Name = "colMode";
            this.colMode.Visible = true;
            this.colMode.VisibleIndex = 2;
            // 
            // colIdentifier
            // 
            this.colIdentifier.ColumnEdit = this.btnEditItemFilterEntry;
            this.colIdentifier.FieldName = "Identifier";
            this.colIdentifier.Name = "colIdentifier";
            this.colIdentifier.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
            this.colIdentifier.Visible = true;
            this.colIdentifier.VisibleIndex = 3;
            this.colIdentifier.Width = 355;
            // 
            // btnEditItemFilterEntry
            // 
            this.btnEditItemFilterEntry.AutoHeight = false;
            this.btnEditItemFilterEntry.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.btnEditItemFilterEntry.Name = "btnEditItemFilterEntry";
            this.btnEditItemFilterEntry.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnEditItemFilterEntry_ButtonClick);
            // 
            // colCount
            // 
            this.colCount.Caption = "Count";
            this.colCount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colCount.FieldName = "Count";
            this.colCount.Name = "colCount";
            this.colCount.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            this.colCount.Visible = true;
            this.colCount.VisibleIndex = 4;
            this.colCount.Width = 65;
            // 
            // colCheckEquipmentLevel
            // 
            this.colCheckEquipmentLevel.Caption = "EqpLvl";
            this.colCheckEquipmentLevel.FieldName = "CheckEquipmentLevel";
            this.colCheckEquipmentLevel.Name = "colCheckEquipmentLevel";
            this.colCheckEquipmentLevel.OptionsColumn.AllowSize = false;
            this.colCheckEquipmentLevel.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colCheckEquipmentLevel.OptionsColumn.FixedWidth = true;
            this.colCheckEquipmentLevel.ToolTip = "Check Equipment Level";
            this.colCheckEquipmentLevel.UnboundType = DevExpress.Data.UnboundColumnType.Boolean;
            this.colCheckEquipmentLevel.Visible = true;
            this.colCheckEquipmentLevel.VisibleIndex = 6;
            this.colCheckEquipmentLevel.Width = 39;
            // 
            // colCheckPlayerLevel
            // 
            this.colCheckPlayerLevel.Caption = "PlLvl";
            this.colCheckPlayerLevel.FieldName = "CheckPlayerLevel";
            this.colCheckPlayerLevel.Name = "colCheckPlayerLevel";
            this.colCheckPlayerLevel.OptionsColumn.AllowSize = false;
            this.colCheckPlayerLevel.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colCheckPlayerLevel.OptionsColumn.FixedWidth = true;
            this.colCheckPlayerLevel.ToolTip = "Check Player Level";
            this.colCheckPlayerLevel.UnboundType = DevExpress.Data.UnboundColumnType.Boolean;
            this.colCheckPlayerLevel.Visible = true;
            this.colCheckPlayerLevel.VisibleIndex = 7;
            this.colCheckPlayerLevel.Width = 38;
            // 
            // colKeepNumber
            // 
            this.colKeepNumber.Caption = "Keep";
            this.colKeepNumber.FieldName = "KeepNumber";
            this.colKeepNumber.Name = "colKeepNumber";
            this.colKeepNumber.OptionsColumn.AllowShowHide = false;
            this.colKeepNumber.OptionsColumn.AllowSize = false;
            this.colKeepNumber.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colKeepNumber.OptionsColumn.FixedWidth = true;
            this.colKeepNumber.ToolTip = "Keep number of item equals to Count";
            this.colKeepNumber.UnboundType = DevExpress.Data.UnboundColumnType.Boolean;
            this.colKeepNumber.Visible = true;
            this.colKeepNumber.VisibleIndex = 5;
            this.colKeepNumber.Width = 40;
            // 
            // colPutOnItem
            // 
            this.colPutOnItem.Caption = "Wear";
            this.colPutOnItem.FieldName = "PutOnItem";
            this.colPutOnItem.Name = "colPutOnItem";
            this.colPutOnItem.OptionsColumn.AllowSize = false;
            this.colPutOnItem.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colPutOnItem.ToolTip = "Equip item arter buying";
            this.colPutOnItem.UnboundType = DevExpress.Data.UnboundColumnType.Boolean;
            this.colPutOnItem.Visible = true;
            this.colPutOnItem.VisibleIndex = 8;
            this.colPutOnItem.Width = 42;
            // 
            // barManager
            // 
            this.barManager.DockControls.Add(this.barDockControl_0);
            this.barManager.DockControls.Add(this.barDockControl_1);
            this.barManager.DockControls.Add(this.barDockControl_2);
            this.barManager.DockControls.Add(this.barDockControl_3);
            this.barManager.Form = this;
            this.barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barButtonItem1,
            this.barEditItem,
            this.barListItem,
            this.barToolbarsListItem});
            this.barManager.MaxItemId = 4;
            this.barManager.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.fastAddItemCombo});
            // 
            // barDockControl_0
            // 
            this.barDockControl_0.CausesValidation = false;
            this.barDockControl_0.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControl_0.Location = new System.Drawing.Point(0, 0);
            this.barDockControl_0.Manager = this.barManager;
            this.barDockControl_0.Size = new System.Drawing.Size(674, 0);
            // 
            // barDockControl_1
            // 
            this.barDockControl_1.CausesValidation = false;
            this.barDockControl_1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControl_1.Location = new System.Drawing.Point(0, 286);
            this.barDockControl_1.Manager = this.barManager;
            this.barDockControl_1.Size = new System.Drawing.Size(674, 0);
            // 
            // barDockControl_2
            // 
            this.barDockControl_2.CausesValidation = false;
            this.barDockControl_2.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControl_2.Location = new System.Drawing.Point(0, 0);
            this.barDockControl_2.Manager = this.barManager;
            this.barDockControl_2.Size = new System.Drawing.Size(0, 286);
            // 
            // barDockControl_3
            // 
            this.barDockControl_3.CausesValidation = false;
            this.barDockControl_3.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControl_3.Location = new System.Drawing.Point(674, 0);
            this.barDockControl_3.Manager = this.barManager;
            this.barDockControl_3.Size = new System.Drawing.Size(0, 286);
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Caption = "Advanced";
            this.barButtonItem1.Id = 0;
            this.barButtonItem1.Name = "barButtonItem1";
            // 
            // barEditItem
            // 
            this.barEditItem.Caption = "Add an item";
            this.barEditItem.Edit = this.fastAddItemCombo;
            this.barEditItem.Id = 1;
            this.barEditItem.Name = "barEditItem";
            // 
            // fastAddItemCombo
            // 
            this.fastAddItemCombo.AutoHeight = false;
            this.fastAddItemCombo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.fastAddItemCombo.Items.AddRange(new object[] {
            "fgrte",
            "gfdg",
            "trterg"});
            this.fastAddItemCombo.Name = "fastAddItemCombo";
            this.fastAddItemCombo.Sorted = true;
            // 
            // barListItem
            // 
            this.barListItem.Caption = "Test";
            this.barListItem.Id = 2;
            this.barListItem.Name = "barListItem";
            this.barListItem.Strings.AddRange(new object[] {
            "uytru",
            "yturtyut",
            "ghjfghj",
            "turty",
            "jhgfitj"});
            // 
            // barToolbarsListItem
            // 
            this.barToolbarsListItem.Caption = "test";
            this.barToolbarsListItem.Id = 3;
            this.barToolbarsListItem.Name = "barToolbarsListItem";
            // 
            // addEntryMenu
            // 
            this.addEntryMenu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem1)});
            this.addEntryMenu.Manager = this.barManager;
            this.addEntryMenu.MinWidth = 300;
            this.addEntryMenu.Name = "addEntryMenu";
            // 
            // btnShowItems
            // 
            this.btnShowItems.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnShowItems.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnShowItems.ImageOptions.Image = global::EntityTools.Properties.Resources.miniPlayAll;
            this.btnShowItems.Location = new System.Drawing.Point(370, 251);
            this.btnShowItems.Name = "btnShowItems";
            this.btnShowItems.Size = new System.Drawing.Size(70, 23);
            this.btnShowItems.TabIndex = 19;
            this.btnShowItems.Text = "Test";
            this.btnShowItems.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClear.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnClear.ImageOptions.Image = global::EntityTools.Properties.Resources.miniDelete;
            this.btnClear.Location = new System.Drawing.Point(191, 251);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(70, 23);
            this.btnClear.TabIndex = 24;
            this.btnClear.Text = "Clear";
            this.btnClear.ToolTip = "Clear list";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnExport.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnExport.ImageOptions.Image = global::EntityTools.Properties.Resources.miniExport;
            this.btnExport.Location = new System.Drawing.Point(88, 251);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(70, 23);
            this.btnExport.TabIndex = 29;
            this.btnExport.Text = "Export";
            this.btnExport.ToolTip = "Export";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnImport
            // 
            this.btnImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnImport.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnImport.ImageOptions.Image = global::EntityTools.Properties.Resources.miniImport;
            this.btnImport.Location = new System.Drawing.Point(12, 251);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(70, 23);
            this.btnImport.TabIndex = 30;
            this.btnImport.Text = "Import";
            this.btnImport.ToolTip = "Import";
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.ImageOptions.Image = global::EntityTools.Properties.Resources.miniCancel;
            this.btnCancel.Location = new System.Drawing.Point(588, 251);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(74, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // bntSave
            // 
            this.bntSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bntSave.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.bntSave.ImageOptions.Image = global::EntityTools.Properties.Resources.miniValid;
            this.bntSave.Location = new System.Drawing.Point(508, 251);
            this.bntSave.Name = "bntSave";
            this.bntSave.Size = new System.Drawing.Size(74, 23);
            this.bntSave.TabIndex = 8;
            this.bntSave.Text = "Save";
            this.bntSave.Click += new System.EventHandler(this.bntSave_Click);
            // 
            // btnReload
            // 
            this.btnReload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReload.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnReload.ImageOptions.Image = global::EntityTools.Properties.Resources.miniRefresh;
            this.btnReload.Location = new System.Drawing.Point(267, 251);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(70, 23);
            this.btnReload.TabIndex = 24;
            this.btnReload.Text = "Reload";
            this.btnReload.ToolTip = "Clear list";
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // ItemFilterEditorForm
            // 
            this.AcceptButton = this.bntSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(674, 286);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnReload);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnShowItems);
            this.Controls.Add(this.bntSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.purchaseOptions);
            this.Controls.Add(this.barDockControl_2);
            this.Controls.Add(this.barDockControl_3);
            this.Controls.Add(this.barDockControl_1);
            this.Controls.Add(this.barDockControl_0);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(660, 200);
            this.Name = "ItemFilterEditorForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ItemFilterEditor";
            this.Shown += new System.EventHandler(this.ItemFilterEditorForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.purchaseOptions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnEditItemFilterEntry)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fastAddItemCombo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.addEntryMenu)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private GridControl purchaseOptions;
        private GridView gridView;
        private BindingSource bindingSource;
        private GridColumn colStringType;
        private GridColumn colEntryType;
        private GridColumn colMode;
        private GridColumn colIdentifier;
        private GridColumn colCount;
        private GridColumn colCheckEquipmentLevel;
        private GridColumn colCheckPlayerLevel;
        private GridColumn colKeepNumber;
        private GridColumn colPutOnItem;
        private PopupMenu addEntryMenu;
        private BarButtonItem barButtonItem1;
        private BarManager barManager;
        private BarDockControl barDockControl_0;
        private BarDockControl barDockControl_1;
        private BarDockControl barDockControl_2;
        private BarDockControl barDockControl_3;
        private BarEditItem barEditItem;
        private RepositoryItemComboBox fastAddItemCombo;
        private RepositoryItemButtonEdit btnEditItemFilterEntry;
        private BarListItem barListItem;
        private BarToolbarsListItem barToolbarsListItem;
        private SimpleButton btnShowItems;
        private SimpleButton btnClear;
        private SimpleButton btnImport;
        private SimpleButton btnExport;
        private SimpleButton bntSave;
        private SimpleButton btnCancel;
        private SimpleButton btnReload;
    }
}