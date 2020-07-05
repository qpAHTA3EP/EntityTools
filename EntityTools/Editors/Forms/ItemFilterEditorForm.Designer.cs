﻿using Astral.Classes.ItemFilter;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using EntityTools.Tools.ItemFilter;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace EntityTools.Forms
{
    partial class ItemFilterEditorForm<TFilterEntry> where TFilterEntry : IFilterEntry
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
            this.colGroup = new DevExpress.XtraGrid.Columns.GridColumn();
            this.editorNumber = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.colStringType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colEntryType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPattern = new DevExpress.XtraGrid.Columns.GridColumn();
            this.editorItemFilterEntry = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.colCount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCheckEquipmentLevel = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCheckPlayerLevel = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colKeepNumber = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colWear = new DevExpress.XtraGrid.Columns.GridColumn();
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
            this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
            this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.purchaseOptions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.editorNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.editorItemFilterEntry)).BeginInit();
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
            this.editorItemFilterEntry,
            this.editorNumber});
            this.purchaseOptions.Size = new System.Drawing.Size(620, 230);
            this.purchaseOptions.TabIndex = 6;
            this.purchaseOptions.UseEmbeddedNavigator = true;
            this.purchaseOptions.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView});
            // 
            // gridView
            // 
            this.gridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colGroup,
            this.colStringType,
            this.colEntryType,
            this.colMode,
            this.colPattern,
            this.colCount,
            this.colCheckEquipmentLevel,
            this.colCheckPlayerLevel,
            this.colKeepNumber,
            this.colWear});
            this.gridView.GridControl = this.purchaseOptions;
            this.gridView.Name = "gridView";
            this.gridView.OptionsView.ShowGroupPanel = false;
            this.gridView.OptionsView.ShowIndicator = false;
            this.gridView.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.colGroup, DevExpress.Data.ColumnSortOrder.Ascending)});
            // 
            // colGroup
            // 
            this.colGroup.Caption = "Group";
            this.colGroup.ColumnEdit = this.editorNumber;
            this.colGroup.DisplayFormat.FormatString = "N00";
            this.colGroup.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colGroup.FieldName = "Group";
            this.colGroup.MinWidth = 60;
            this.colGroup.Name = "colGroup";
            this.colGroup.ToolTip = "Index of the Group. All filter in the Group checks at conjunction rule";
            this.colGroup.Visible = true;
            this.colGroup.VisibleIndex = 0;
            this.colGroup.Width = 60;
            // 
            // editorNumber
            // 
            this.editorNumber.AutoHeight = false;
            this.editorNumber.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.editorNumber.DisplayFormat.FormatString = "N0";
            this.editorNumber.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.editorNumber.IsFloatValue = false;
            this.editorNumber.Mask.EditMask = "N00";
            this.editorNumber.Name = "editorNumber";
            // 
            // colStringType
            // 
            this.colStringType.Caption = "TextType";
            this.colStringType.FieldName = "StringType";
            this.colStringType.MaxWidth = 60;
            this.colStringType.MinWidth = 60;
            this.colStringType.Name = "colStringType";
            this.colStringType.Visible = true;
            this.colStringType.VisibleIndex = 1;
            this.colStringType.Width = 60;
            // 
            // colEntryType
            // 
            this.colEntryType.Caption = "FilterType";
            this.colEntryType.FieldName = "EntryType";
            this.colEntryType.MaxWidth = 60;
            this.colEntryType.MinWidth = 60;
            this.colEntryType.Name = "colEntryType";
            this.colEntryType.Visible = true;
            this.colEntryType.VisibleIndex = 2;
            this.colEntryType.Width = 60;
            // 
            // colMode
            // 
            this.colMode.Caption = "Inclusion";
            this.colMode.FieldName = "Mode";
            this.colMode.MaxWidth = 50;
            this.colMode.MinWidth = 50;
            this.colMode.Name = "colMode";
            this.colMode.Visible = true;
            this.colMode.VisibleIndex = 3;
            this.colMode.Width = 50;
            // 
            // colPattern
            // 
            this.colPattern.Caption = "Pattern";
            this.colPattern.ColumnEdit = this.editorItemFilterEntry;
            this.colPattern.FieldName = "Pattern";
            this.colPattern.Name = "colPattern";
            this.colPattern.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
            this.colPattern.Visible = true;
            this.colPattern.VisibleIndex = 4;
            this.colPattern.Width = 180;
            // 
            // editorItemFilterEntry
            // 
            this.editorItemFilterEntry.AutoHeight = false;
            this.editorItemFilterEntry.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.editorItemFilterEntry.Name = "editorItemFilterEntry";
            this.editorItemFilterEntry.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.event_EditItemFilterEntry);
            // 
            // colCount
            // 
            this.colCount.Caption = "Count";
            this.colCount.ColumnEdit = this.editorNumber;
            this.colCount.CustomizationCaption = "N0";
            this.colCount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colCount.FieldName = "Count";
            this.colCount.MinWidth = 48;
            this.colCount.Name = "colCount";
            this.colCount.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            this.colCount.Visible = true;
            this.colCount.VisibleIndex = 5;
            this.colCount.Width = 48;
            // 
            // colCheckEquipmentLevel
            // 
            this.colCheckEquipmentLevel.Caption = "EqpLvl";
            this.colCheckEquipmentLevel.FieldName = "CheckEquipmentLevel";
            this.colCheckEquipmentLevel.MaxWidth = 40;
            this.colCheckEquipmentLevel.MinWidth = 40;
            this.colCheckEquipmentLevel.Name = "colCheckEquipmentLevel";
            this.colCheckEquipmentLevel.OptionsColumn.AllowSize = false;
            this.colCheckEquipmentLevel.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colCheckEquipmentLevel.OptionsColumn.FixedWidth = true;
            this.colCheckEquipmentLevel.ToolTip = "Check Equipment Level";
            this.colCheckEquipmentLevel.UnboundType = DevExpress.Data.UnboundColumnType.Boolean;
            this.colCheckEquipmentLevel.Visible = true;
            this.colCheckEquipmentLevel.VisibleIndex = 7;
            this.colCheckEquipmentLevel.Width = 40;
            // 
            // colCheckPlayerLevel
            // 
            this.colCheckPlayerLevel.Caption = "PlLvl";
            this.colCheckPlayerLevel.FieldName = "CheckPlayerLevel";
            this.colCheckPlayerLevel.MaxWidth = 40;
            this.colCheckPlayerLevel.MinWidth = 40;
            this.colCheckPlayerLevel.Name = "colCheckPlayerLevel";
            this.colCheckPlayerLevel.OptionsColumn.AllowSize = false;
            this.colCheckPlayerLevel.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colCheckPlayerLevel.OptionsColumn.FixedWidth = true;
            this.colCheckPlayerLevel.ToolTip = "Check Player Level";
            this.colCheckPlayerLevel.UnboundType = DevExpress.Data.UnboundColumnType.Boolean;
            this.colCheckPlayerLevel.Visible = true;
            this.colCheckPlayerLevel.VisibleIndex = 8;
            this.colCheckPlayerLevel.Width = 40;
            // 
            // colKeepNumber
            // 
            this.colKeepNumber.Caption = "Keep";
            this.colKeepNumber.FieldName = "KeepNumber";
            this.colKeepNumber.MaxWidth = 40;
            this.colKeepNumber.MinWidth = 40;
            this.colKeepNumber.Name = "colKeepNumber";
            this.colKeepNumber.OptionsColumn.AllowShowHide = false;
            this.colKeepNumber.OptionsColumn.AllowSize = false;
            this.colKeepNumber.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colKeepNumber.OptionsColumn.FixedWidth = true;
            this.colKeepNumber.ToolTip = "Keep number of item equals to Count";
            this.colKeepNumber.UnboundType = DevExpress.Data.UnboundColumnType.Boolean;
            this.colKeepNumber.Visible = true;
            this.colKeepNumber.VisibleIndex = 6;
            this.colKeepNumber.Width = 40;
            // 
            // colWear
            // 
            this.colWear.Caption = "Wear";
            this.colWear.FieldName = "Wear";
            this.colWear.MaxWidth = 40;
            this.colWear.MinWidth = 40;
            this.colWear.Name = "colWear";
            this.colWear.OptionsColumn.AllowSize = false;
            this.colWear.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colWear.ToolTip = "Equip item arter buying";
            this.colWear.UnboundType = DevExpress.Data.UnboundColumnType.Boolean;
            this.colWear.Visible = true;
            this.colWear.VisibleIndex = 9;
            this.colWear.Width = 40;
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
            this.barDockControl_0.Size = new System.Drawing.Size(644, 0);
            // 
            // barDockControl_1
            // 
            this.barDockControl_1.CausesValidation = false;
            this.barDockControl_1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControl_1.Location = new System.Drawing.Point(0, 286);
            this.barDockControl_1.Manager = this.barManager;
            this.barDockControl_1.Size = new System.Drawing.Size(644, 0);
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
            this.barDockControl_3.Location = new System.Drawing.Point(644, 0);
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
            this.btnShowItems.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnShowItems.Location = new System.Drawing.Point(422, 251);
            this.btnShowItems.Name = "btnShowItems";
            this.btnShowItems.Size = new System.Drawing.Size(62, 23);
            this.btnShowItems.TabIndex = 19;
            this.btnShowItems.Text = "Test";
            this.btnShowItems.ToolTip = "Test filter list on PlayerBags";
            this.btnShowItems.Click += new System.EventHandler(this.event_TestFilters);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClear.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnClear.ImageOptions.Image = global::EntityTools.Properties.Resources.miniDelete;
            this.btnClear.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnClear.Location = new System.Drawing.Point(148, 251);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(62, 23);
            this.btnClear.TabIndex = 24;
            this.btnClear.Text = "Clear";
            this.btnClear.ToolTip = "Clear list";
            this.btnClear.Click += new System.EventHandler(this.event_ClearFilters);
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnExport.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnExport.ImageOptions.Image = global::EntityTools.Properties.Resources.miniExport;
            this.btnExport.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnExport.Location = new System.Drawing.Point(354, 251);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(62, 23);
            this.btnExport.TabIndex = 29;
            this.btnExport.Text = "Export";
            this.btnExport.ToolTip = "Export Filter list to the File";
            this.btnExport.Click += new System.EventHandler(this.event_ExportFilters);
            // 
            // btnImport
            // 
            this.btnImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnImport.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnImport.ImageOptions.Image = global::EntityTools.Properties.Resources.miniImport;
            this.btnImport.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnImport.Location = new System.Drawing.Point(286, 251);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(62, 23);
            this.btnImport.TabIndex = 30;
            this.btnImport.Text = "Import";
            this.btnImport.ToolTip = "Import Filter list from the File";
            this.btnImport.Click += new System.EventHandler(this.event_ImportFilters);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.ImageOptions.Image = global::EntityTools.Properties.Resources.miniCancel;
            this.btnCancel.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnCancel.Location = new System.Drawing.Point(570, 251);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(62, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.event_Cancel);
            // 
            // bntSave
            // 
            this.bntSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bntSave.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.bntSave.ImageOptions.Image = global::EntityTools.Properties.Resources.miniValid;
            this.bntSave.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.bntSave.Location = new System.Drawing.Point(502, 251);
            this.bntSave.Name = "bntSave";
            this.bntSave.Size = new System.Drawing.Size(62, 23);
            this.bntSave.TabIndex = 8;
            this.bntSave.Text = "Save";
            this.bntSave.Click += new System.EventHandler(this.event_SaveAndClose);
            // 
            // btnReload
            // 
            this.btnReload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReload.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnReload.ImageOptions.Image = global::EntityTools.Properties.Resources.miniRefresh;
            this.btnReload.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnReload.Location = new System.Drawing.Point(216, 251);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(64, 23);
            this.btnReload.TabIndex = 24;
            this.btnReload.Text = "Reload";
            this.btnReload.ToolTip = "Reload edited filter list from the source";
            this.btnReload.Click += new System.EventHandler(this.event_ReloadFilters);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDelete.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnDelete.ImageOptions.Image = global::EntityTools.Properties.Resources.miniCancel;
            this.btnDelete.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnDelete.Location = new System.Drawing.Point(80, 251);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(62, 23);
            this.btnDelete.TabIndex = 24;
            this.btnDelete.Text = "Delete";
            this.btnDelete.ToolTip = "Delete selected filter entry";
            this.btnDelete.Click += new System.EventHandler(this.event_DeleteFilterEntry);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAdd.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnAdd.ImageOptions.Image = global::EntityTools.Properties.Resources.miniAdd;
            this.btnAdd.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnAdd.Location = new System.Drawing.Point(12, 251);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(62, 23);
            this.btnAdd.TabIndex = 30;
            this.btnAdd.Text = "Add";
            this.btnAdd.ToolTip = "Import Filter list from the File";
            this.btnAdd.Click += new System.EventHandler(this.event_AddFilterEntry);
            // 
            // ItemFilterEditorForm
            // 
            this.AcceptButton = this.bntSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(644, 286);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnReload);
            this.Controls.Add(this.btnDelete);
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
            this.Shown += new System.EventHandler(this.event_FormShown);
            ((System.ComponentModel.ISupportInitialize)(this.purchaseOptions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.editorNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.editorItemFilterEntry)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fastAddItemCombo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.addEntryMenu)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private GridControl purchaseOptions;
        private GridView gridView;
        private GridColumn colStringType;
        private GridColumn colEntryType;
        private GridColumn colMode;
        private GridColumn colPattern;
        private GridColumn colCount;
        private GridColumn colCheckEquipmentLevel;
        private GridColumn colCheckPlayerLevel;
        private GridColumn colKeepNumber;
        private GridColumn colWear;
        private PopupMenu addEntryMenu;
        private BarButtonItem barButtonItem1;
        private BarManager barManager;
        private BarDockControl barDockControl_0;
        private BarDockControl barDockControl_1;
        private BarDockControl barDockControl_2;
        private BarDockControl barDockControl_3;
        private BarEditItem barEditItem;
        private RepositoryItemComboBox fastAddItemCombo;
        private RepositoryItemButtonEdit editorItemFilterEntry;
        private BarListItem barListItem;
        private BarToolbarsListItem barToolbarsListItem;
        private SimpleButton btnShowItems;
        private SimpleButton btnClear;
        private SimpleButton btnImport;
        private SimpleButton btnExport;
        private SimpleButton bntSave;
        private SimpleButton btnCancel;
        private SimpleButton btnReload;
        private GridColumn colGroup;
        private SimpleButton btnAdd;
        private SimpleButton btnDelete;
        private RepositoryItemSpinEdit editorNumber;
    }
}