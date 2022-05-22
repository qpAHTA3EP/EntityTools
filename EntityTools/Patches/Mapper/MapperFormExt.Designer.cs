using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Repository;

namespace EntityTools.Patches.Mapper
{
#if PATCH_ASTRAL
    partial class MapperFormExt
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapperFormExt));
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.barManager = new DevExpress.XtraBars.BarManager(this.components);
            this.barMapping = new DevExpress.XtraBars.Bar();
            this.groupMapping = new DevExpress.XtraBars.BarButtonGroup();
            this.btnMappingBidirectional = new DevExpress.XtraBars.BarCheckItem();
            this.btnMappingUnidirectional = new DevExpress.XtraBars.BarCheckItem();
            this.btnMappingStop = new DevExpress.XtraBars.BarCheckItem();
            this.btnMappingLinearPath = new DevExpress.XtraBars.BarCheckItem();
            this.btnMappingForceLink = new DevExpress.XtraBars.BarCheckItem();
            this.btnOptions = new DevExpress.XtraBars.BarButtonItem();
            this.popMenuOptions = new DevExpress.XtraBars.PopupMenu(this.components);
            this.editWaypointDistance = new DevExpress.XtraBars.BarEditItem();
            this.seWaypointDistance = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.editMaxZDifference = new DevExpress.XtraBars.BarEditItem();
            this.seMaxZDifference = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.editEquivalenceDistance = new DevExpress.XtraBars.BarEditItem();
            this.seEquivalenceDistance = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.barGraphEditTools = new DevExpress.XtraBars.Bar();
            this.btnUndo = new DevExpress.XtraBars.BarButtonItem();
            this.groupEditMeshes = new DevExpress.XtraBars.BarButtonGroup();
            this.btnMoveNodes = new DevExpress.XtraBars.BarCheckItem();
            this.btnRemoveNodes = new DevExpress.XtraBars.BarCheckItem();
            this.btnAddNodes = new DevExpress.XtraBars.BarCheckItem();
            this.btnEditEdges = new DevExpress.XtraBars.BarCheckItem();
            this.barGraphTools = new DevExpress.XtraBars.Bar();
            this.btnSaveMeshes = new DevExpress.XtraBars.BarButtonItem();
            this.groupImportExportNodes = new DevExpress.XtraBars.BarButtonGroup();
            this.btnImportMeshesFromGame = new DevExpress.XtraBars.BarButtonItem();
            this.btnImportMeshesFromProfile = new DevExpress.XtraBars.BarButtonItem();
            this.btnExportMeshes = new DevExpress.XtraBars.BarButtonItem();
            this.btnClearMeshes = new DevExpress.XtraBars.BarButtonItem();
            this.btnMeshesInfo = new DevExpress.XtraBars.BarButtonItem();
            this.btnCompression = new DevExpress.XtraBars.BarButtonItem();
            this.btnDistanceMeasurement = new DevExpress.XtraBars.BarCheckItem();
            this.btnObjectInfo = new DevExpress.XtraBars.BarCheckItem();
            this.barCustomRegions = new DevExpress.XtraBars.Bar();
            this.btnAddCR = new DevExpress.XtraBars.BarButtonItem();
            this.btnEditCR = new DevExpress.XtraBars.BarButtonItem();
            this.barStatus = new DevExpress.XtraBars.Bar();
            this.btnLockMapOnPlayer = new DevExpress.XtraBars.BarCheckItem();
            this.btnSettings = new DevExpress.XtraBars.BarButtonItem();
            this.groupSaveUndo = new DevExpress.XtraBars.BarButtonGroup();
            this.groupZoom = new DevExpress.XtraBars.BarButtonGroup();
            this.btnZoomIn = new DevExpress.XtraBars.BarButtonItem();
            this.lblZoom = new DevExpress.XtraBars.BarStaticItem();
            this.btnZoomOut = new DevExpress.XtraBars.BarButtonItem();
            this.btnPanelVisibility = new DevExpress.XtraBars.BarButtonItem();
            this.lblPlayerPos = new DevExpress.XtraBars.BarStaticItem();
            this.lblMousePos = new DevExpress.XtraBars.BarStaticItem();
            this.lblDrawInfo = new DevExpress.XtraBars.BarStaticItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.seDeleteRadius = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.editItemColor = new DevExpress.XtraEditors.Repository.RepositoryItemColorEdit();
            this.bsrcAstralSettings = new System.Windows.Forms.BindingSource(this.components);
            this.btnShowStatBar = new System.Windows.Forms.Button();
            this.MapPicture = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.popMenuOptions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seWaypointDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seMaxZDifference)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seEquivalenceDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seDeleteRadius)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.editItemColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsrcAstralSettings)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.work_MapperFormUpdate);
            // 
            // barManager
            // 
            this.barManager.AllowCustomization = false;
            this.barManager.AllowQuickCustomization = false;
            this.barManager.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.barMapping,
            this.barGraphEditTools,
            this.barGraphTools,
            this.barCustomRegions,
            this.barStatus});
            this.barManager.DockControls.Add(this.barDockControlTop);
            this.barManager.DockControls.Add(this.barDockControlBottom);
            this.barManager.DockControls.Add(this.barDockControlLeft);
            this.barManager.DockControls.Add(this.barDockControlRight);
            this.barManager.Form = this;
            this.barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.btnMappingUnidirectional,
            this.btnAddCR,
            this.btnImportMeshesFromGame,
            this.btnImportMeshesFromProfile,
            this.btnExportMeshes,
            this.editWaypointDistance,
            this.editMaxZDifference,
            this.editEquivalenceDistance,
            this.btnClearMeshes,
            this.btnMappingBidirectional,
            this.btnMappingForceLink,
            this.btnMappingLinearPath,
            this.btnMappingStop,
            this.groupMapping,
            this.groupImportExportNodes,
            this.btnOptions,
            this.lblMousePos,
            this.btnSaveMeshes,
            this.btnMoveNodes,
            this.btnRemoveNodes,
            this.btnEditEdges,
            this.groupEditMeshes,
            this.btnUndo,
            this.btnEditCR,
            this.btnMeshesInfo,
            this.btnCompression,
            this.btnLockMapOnPlayer,
            this.groupZoom,
            this.btnZoomIn,
            this.btnZoomOut,
            this.lblZoom,
            this.groupSaveUndo,
            this.btnSettings,
            this.lblPlayerPos,
            this.lblDrawInfo,
            this.btnDistanceMeasurement,
            this.btnObjectInfo,
            this.btnPanelVisibility,
            this.btnAddNodes});
            this.barManager.MaxItemId = 146;
            this.barManager.OptionsLayout.AllowAddNewItems = false;
            this.barManager.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.seDeleteRadius,
            this.seWaypointDistance,
            this.seMaxZDifference,
            this.seEquivalenceDistance,
            this.editItemColor});
            this.barManager.StatusBar = this.barStatus;
            // 
            // barMapping
            // 
            this.barMapping.BarName = "MappingTools";
            this.barMapping.DockCol = 0;
            this.barMapping.DockRow = 0;
            this.barMapping.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.barMapping.FloatLocation = new System.Drawing.Point(325, 181);
            this.barMapping.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.groupMapping, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnOptions, true)});
            this.barMapping.OptionsBar.AllowQuickCustomization = false;
            this.barMapping.Text = "Mapping Tools";
            this.barMapping.Visible = false;
            this.barMapping.VisibleChanged += new System.EventHandler(this.handler_BarVisibleChanged);
            // 
            // groupMapping
            // 
            this.groupMapping.Caption = "Mapping";
            this.groupMapping.Id = 70;
            this.groupMapping.ItemLinks.Add(this.btnMappingBidirectional);
            this.groupMapping.ItemLinks.Add(this.btnMappingUnidirectional);
            this.groupMapping.ItemLinks.Add(this.btnMappingStop);
            this.groupMapping.ItemLinks.Add(this.btnMappingLinearPath, true);
            this.groupMapping.ItemLinks.Add(this.btnMappingForceLink);
            this.groupMapping.Name = "groupMapping";
            // 
            // btnMappingBidirectional
            // 
            this.btnMappingBidirectional.Caption = "Bidirectional Mapping";
            this.btnMappingBidirectional.GroupIndex = 1;
            this.btnMappingBidirectional.Id = 64;
            this.btnMappingBidirectional.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnMappingBidirectional.ImageOptions.Image")));
            this.btnMappingBidirectional.ItemShortcut = new DevExpress.XtraBars.BarShortcut(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                | System.Windows.Forms.Keys.B));
            this.btnMappingBidirectional.Name = "btnMappingBidirectional";
            this.btnMappingBidirectional.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_Mapping_BidirectionalPath);
            // 
            // btnMappingUnidirectional
            // 
            this.btnMappingUnidirectional.Caption = "Unidirectional Mapping";
            this.btnMappingUnidirectional.GroupIndex = 1;
            this.btnMappingUnidirectional.Id = 44;
            this.btnMappingUnidirectional.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnMappingUnidirectional.ImageOptions.Image")));
            this.btnMappingUnidirectional.ItemShortcut = new DevExpress.XtraBars.BarShortcut(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                | System.Windows.Forms.Keys.U));
            this.btnMappingUnidirectional.Name = "btnMappingUnidirectional";
            this.btnMappingUnidirectional.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_Mapping_UnidirectionalPath);
            // 
            // btnMappingStop
            // 
            this.btnMappingStop.BindableChecked = true;
            this.btnMappingStop.Caption = "Stop Mapping";
            this.btnMappingStop.Checked = true;
            this.btnMappingStop.GroupIndex = 1;
            this.btnMappingStop.Id = 67;
            this.btnMappingStop.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnMappingStop.ImageOptions.Image")));
            this.btnMappingStop.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.End));
            this.btnMappingStop.Name = "btnMappingStop";
            this.btnMappingStop.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_Mapping_Stop);
            // 
            // btnMappingLinearPath
            // 
            this.btnMappingLinearPath.Caption = "Linear Path";
            this.btnMappingLinearPath.Id = 66;
            this.btnMappingLinearPath.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnMappingLinearPath.ImageOptions.Image")));
            this.btnMappingLinearPath.ItemShortcut = new DevExpress.XtraBars.BarShortcut(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                | System.Windows.Forms.Keys.L));
            this.btnMappingLinearPath.Name = "btnMappingLinearPath";
            this.btnMappingLinearPath.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_Mapping_LinearPath);
            // 
            // btnMappingForceLink
            // 
            this.btnMappingForceLink.BindableChecked = true;
            this.btnMappingForceLink.Caption = "Force Linking to Last Node";
            this.btnMappingForceLink.Checked = true;
            this.btnMappingForceLink.Id = 65;
            this.btnMappingForceLink.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnMappingForceLink.ImageOptions.Image")));
            this.btnMappingForceLink.ItemShortcut = new DevExpress.XtraBars.BarShortcut(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
                | System.Windows.Forms.Keys.L));
            this.btnMappingForceLink.Name = "btnMappingForceLink";
            this.btnMappingForceLink.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_Mapping_ForceLink);
            // 
            // btnOptions
            // 
            this.btnOptions.ActAsDropDown = true;
            this.btnOptions.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
            this.btnOptions.Caption = "Options";
            this.btnOptions.DropDownControl = this.popMenuOptions;
            this.btnOptions.Hint = "Options";
            this.btnOptions.Id = 74;
            this.btnOptions.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnOptions.ImageOptions.Image")));
            this.btnOptions.Name = "btnOptions";
            // 
            // popMenuOptions
            // 
            this.popMenuOptions.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.editWaypointDistance),
            new DevExpress.XtraBars.LinkPersistInfo(this.editMaxZDifference),
            new DevExpress.XtraBars.LinkPersistInfo(this.editEquivalenceDistance)});
            this.popMenuOptions.Manager = this.barManager;
            this.popMenuOptions.Name = "popMenuOptions";
            // 
            // editWaypointDistance
            // 
            this.editWaypointDistance.Caption = "Waypoint Distance";
            this.editWaypointDistance.Edit = this.seWaypointDistance;
            this.editWaypointDistance.Id = 58;
            this.editWaypointDistance.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.editWaypointDistance.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("editWaypointDistance.ImageOptions.Image")));
            this.editWaypointDistance.Name = "editWaypointDistance";
            // 
            // seWaypointDistance
            // 
            this.seWaypointDistance.AutoHeight = false;
            this.seWaypointDistance.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.seWaypointDistance.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.seWaypointDistance.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.seWaypointDistance.IsFloatValue = false;
            this.seWaypointDistance.Mask.EditMask = "N00";
            this.seWaypointDistance.Name = "seWaypointDistance";
            // 
            // editMaxZDifference
            // 
            this.editMaxZDifference.Caption = "MaxElevationDifference";
            this.editMaxZDifference.Edit = this.seMaxZDifference;
            this.editMaxZDifference.Id = 59;
            this.editMaxZDifference.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.editMaxZDifference.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("editMaxZDifference.ImageOptions.Image")));
            this.editMaxZDifference.Name = "editMaxZDifference";
            // 
            // seMaxZDifference
            // 
            this.seMaxZDifference.AutoHeight = false;
            this.seMaxZDifference.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.seMaxZDifference.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.seMaxZDifference.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.seMaxZDifference.IsFloatValue = false;
            this.seMaxZDifference.Mask.EditMask = "N00";
            this.seMaxZDifference.Name = "seMaxZDifference";
            // 
            // editEquivalenceDistance
            // 
            this.editEquivalenceDistance.Caption = "Node Equivalence Distance";
            this.editEquivalenceDistance.Edit = this.seEquivalenceDistance;
            this.editEquivalenceDistance.Id = 60;
            this.editEquivalenceDistance.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.editEquivalenceDistance.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("editEquivalenceDistance.ImageOptions.Image")));
            this.editEquivalenceDistance.Name = "editEquivalenceDistance";
            // 
            // seEquivalenceDistance
            // 
            this.seEquivalenceDistance.AutoHeight = false;
            this.seEquivalenceDistance.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.seEquivalenceDistance.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.seEquivalenceDistance.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.seEquivalenceDistance.IsFloatValue = false;
            this.seEquivalenceDistance.Mask.EditMask = "N00";
            this.seEquivalenceDistance.Name = "seEquivalenceDistance";
            // 
            // barGraphEditTools
            // 
            this.barGraphEditTools.BarName = "GraphEditTools";
            this.barGraphEditTools.DockCol = 1;
            this.barGraphEditTools.DockRow = 0;
            this.barGraphEditTools.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.barGraphEditTools.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnUndo),
            new DevExpress.XtraBars.LinkPersistInfo(this.groupEditMeshes, true)});
            this.barGraphEditTools.Offset = 207;
            this.barGraphEditTools.Text = "Graph Edit Tools";
            this.barGraphEditTools.Visible = false;
            this.barGraphEditTools.VisibleChanged += new System.EventHandler(this.handler_BarVisibleChanged);
            // 
            // btnUndo
            // 
            this.btnUndo.Caption = "Undo";
            this.btnUndo.Id = 101;
            this.btnUndo.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.btnUndo.ImageOptions.AllowStubGlyph = DevExpress.Utils.DefaultBoolean.False;
            this.btnUndo.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnUndo.ImageOptions.Image")));
            this.btnUndo.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnUndo.ImageOptions.LargeImage")));
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_Undo);
            // 
            // groupEditMeshes
            // 
            this.groupEditMeshes.Caption = "EditMeshes";
            this.groupEditMeshes.Id = 96;
            this.groupEditMeshes.ItemLinks.Add(this.btnMoveNodes);
            this.groupEditMeshes.ItemLinks.Add(this.btnRemoveNodes);
            this.groupEditMeshes.ItemLinks.Add(this.btnAddNodes);
            this.groupEditMeshes.ItemLinks.Add(this.btnEditEdges);
            this.groupEditMeshes.Name = "groupEditMeshes";
            // 
            // btnMoveNodes
            // 
            this.btnMoveNodes.Caption = "Relocate Nodes";
            this.btnMoveNodes.Id = 92;
            this.btnMoveNodes.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnMoveNodes.ImageOptions.Image")));
            this.btnMoveNodes.ItemShortcut = new DevExpress.XtraBars.BarShortcut(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                | System.Windows.Forms.Keys.R));
            this.btnMoveNodes.Name = "btnMoveNodes";
            this.btnMoveNodes.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_RelocateNodes_ModeChanged);
            // 
            // btnRemoveNodes
            // 
            this.btnRemoveNodes.Caption = "Delete Nodes";
            this.btnRemoveNodes.Id = 93;
            this.btnRemoveNodes.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnRemoveNodes.ImageOptions.Image")));
            this.btnRemoveNodes.ItemShortcut = new DevExpress.XtraBars.BarShortcut(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                | System.Windows.Forms.Keys.D));
            this.btnRemoveNodes.Name = "btnRemoveNodes";
            this.btnRemoveNodes.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_DeleteNodes_ModeChanged);
            // 
            // btnAddNodes
            // 
            this.btnAddNodes.Caption = "Add Nodes";
            this.btnAddNodes.Id = 145;
            this.btnAddNodes.ImageOptions.Image = global::EntityTools.Properties.Resources.miniAdd;
            this.btnAddNodes.Name = "btnAddNodes";
            this.btnAddNodes.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_AddNode_ModeChanged);
            // 
            // btnEditEdges
            // 
            this.btnEditEdges.Caption = "Edit Edges";
            this.btnEditEdges.Id = 94;
            this.btnEditEdges.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnEditEdges.ImageOptions.Image")));
            this.btnEditEdges.ItemShortcut = new DevExpress.XtraBars.BarShortcut(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                | System.Windows.Forms.Keys.E));
            this.btnEditEdges.Name = "btnEditEdges";
            this.btnEditEdges.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_EditEdges_ModeChanged);
            // 
            // barGraphTools
            // 
            this.barGraphTools.BarName = "GraphTools";
            this.barGraphTools.DockCol = 0;
            this.barGraphTools.DockRow = 1;
            this.barGraphTools.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.barGraphTools.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnUndo),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnSaveMeshes),
            new DevExpress.XtraBars.LinkPersistInfo(this.groupImportExportNodes, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnMeshesInfo, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnCompression),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnDistanceMeasurement, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnObjectInfo)});
            this.barGraphTools.OptionsBar.AllowQuickCustomization = false;
            this.barGraphTools.Text = "Graph Tools";
            this.barGraphTools.Visible = false;
            this.barGraphTools.VisibleChanged += new System.EventHandler(this.handler_BarVisibleChanged);
            // 
            // btnSaveMeshes
            // 
            this.btnSaveMeshes.Caption = "Save";
            this.btnSaveMeshes.Id = 91;
            this.btnSaveMeshes.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.btnSaveMeshes.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveMeshes.ImageOptions.Image")));
            this.btnSaveMeshes.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnSaveMeshes.ImageOptions.LargeImage")));
            this.btnSaveMeshes.ItemShortcut = new DevExpress.XtraBars.BarShortcut(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                | System.Windows.Forms.Keys.S));
            this.btnSaveMeshes.Name = "btnSaveMeshes";
            this.btnSaveMeshes.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_SaveChanges2QuesterProfile);
            // 
            // groupImportExportNodes
            // 
            this.groupImportExportNodes.Caption = "ImportExportNodes";
            this.groupImportExportNodes.Id = 72;
            this.groupImportExportNodes.ItemLinks.Add(this.btnImportMeshesFromGame, true);
            this.groupImportExportNodes.ItemLinks.Add(this.btnImportMeshesFromProfile);
            this.groupImportExportNodes.ItemLinks.Add(this.btnExportMeshes);
            this.groupImportExportNodes.ItemLinks.Add(this.btnClearMeshes, true);
            this.groupImportExportNodes.Name = "groupImportExportNodes";
            // 
            // btnImportMeshesFromGame
            // 
            this.btnImportMeshesFromGame.Caption = "Import from Game";
            this.btnImportMeshesFromGame.Id = 51;
            this.btnImportMeshesFromGame.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnImportMeshesFromGame.ImageOptions.Image")));
            this.btnImportMeshesFromGame.Name = "btnImportMeshesFromGame";
            this.btnImportMeshesFromGame.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_ImportCurrentMapMeshesFromGame);
            // 
            // btnImportMeshesFromProfile
            // 
            this.btnImportMeshesFromProfile.Caption = "Import from Profile";
            this.btnImportMeshesFromProfile.Id = 52;
            this.btnImportMeshesFromProfile.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnImportMeshesFromProfile.ImageOptions.Image")));
            this.btnImportMeshesFromProfile.Name = "btnImportMeshesFromProfile";
            this.btnImportMeshesFromProfile.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_ImportMapMeshesFromFile);
            // 
            // btnExportMeshes
            // 
            this.btnExportMeshes.Caption = "Export to Mesh";
            this.btnExportMeshes.Enabled = false;
            this.btnExportMeshes.Id = 55;
            this.btnExportMeshes.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnExportMeshes.ImageOptions.Image")));
            this.btnExportMeshes.Name = "btnExportMeshes";
            this.btnExportMeshes.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            this.btnExportMeshes.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_ExportCurrentMapMeshes2File);
            // 
            // btnClearMeshes
            // 
            this.btnClearMeshes.Caption = "Clear";
            this.btnClearMeshes.Id = 63;
            this.btnClearMeshes.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnClearMeshes.ImageOptions.Image")));
            this.btnClearMeshes.Name = "btnClearMeshes";
            this.btnClearMeshes.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_ClearCurrentMapMeshes);
            // 
            // btnMeshesInfo
            // 
            this.btnMeshesInfo.Caption = "MeshesInfo";
            this.btnMeshesInfo.Hint = "Meshes information";
            this.btnMeshesInfo.Id = 103;
            this.btnMeshesInfo.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnMeshesInfo.ImageOptions.Image")));
            this.btnMeshesInfo.Name = "btnMeshesInfo";
            this.btnMeshesInfo.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_MeshesInfo);
            // 
            // btnCompression
            // 
            this.btnCompression.Caption = "Compression";
            this.btnCompression.Hint = "Удаление из графа непроходимых(невидимых) вершин и ребер";
            this.btnCompression.Id = 104;
            this.btnCompression.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCompression.ImageOptions.Image")));
            this.btnCompression.Name = "btnCompression";
            this.btnCompression.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_MeshesCompression);
            // 
            // btnDistanceMeasurement
            // 
            this.btnDistanceMeasurement.Caption = "DistanceMeasurement";
            this.btnDistanceMeasurement.Id = 132;
            this.btnDistanceMeasurement.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnDistanceMeasurement.ImageOptions.Image")));
            this.btnDistanceMeasurement.Name = "btnDistanceMeasurement";
            this.btnDistanceMeasurement.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_DistanceMeasurement_ModeChanged);
            // 
            // btnObjectInfo
            // 
            this.btnObjectInfo.Caption = "ObjectInfo";
            this.btnObjectInfo.Id = 134;
            this.btnObjectInfo.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnObjectInfo.ImageOptions.Image")));
            this.btnObjectInfo.Name = "btnObjectInfo";
            this.btnObjectInfo.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_ObjectInfo);
            // 
            // barCustomRegions
            // 
            this.barCustomRegions.BarName = "CustomRegionTools";
            this.barCustomRegions.DockCol = 1;
            this.barCustomRegions.DockRow = 1;
            this.barCustomRegions.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.barCustomRegions.FloatLocation = new System.Drawing.Point(66, 208);
            this.barCustomRegions.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnUndo),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnAddCR),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnEditCR)});
            this.barCustomRegions.Offset = 266;
            this.barCustomRegions.OptionsBar.AllowQuickCustomization = false;
            this.barCustomRegions.Text = "CustomRegion Tools";
            this.barCustomRegions.Visible = false;
            this.barCustomRegions.VisibleChanged += new System.EventHandler(this.handler_BarVisibleChanged);
            // 
            // btnAddCR
            // 
            this.btnAddCR.Caption = "Add CustomRegion";
            this.btnAddCR.Id = 48;
            this.btnAddCR.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnAddCR.ImageOptions.Image")));
            this.btnAddCR.Name = "btnAddCR";
            this.btnAddCR.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_StartAddingCustomRegion);
            // 
            // btnEditCR
            // 
            this.btnEditCR.Caption = "Edit CustomRegion";
            this.btnEditCR.Id = 102;
            this.btnEditCR.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnEditCR.ImageOptions.Image")));
            this.btnEditCR.Name = "btnEditCR";
            this.btnEditCR.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_StartEditingCustomRegion);
            // 
            // barStatus
            // 
            this.barStatus.BarName = "statusBar";
            this.barStatus.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
            this.barStatus.DockCol = 0;
            this.barStatus.DockRow = 0;
            this.barStatus.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
            this.barStatus.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnLockMapOnPlayer),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnSettings, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.groupSaveUndo, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.groupZoom, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnPanelVisibility, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.lblPlayerPos, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.lblMousePos, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.lblDrawInfo, true)});
            this.barStatus.OptionsBar.AllowQuickCustomization = false;
            this.barStatus.OptionsBar.DrawDragBorder = false;
            this.barStatus.OptionsBar.UseWholeRow = true;
            this.barStatus.Text = "StatusBar";
            this.barStatus.VisibleChanged += new System.EventHandler(this.handler_BarVisibleChanged);
            // 
            // btnLockMapOnPlayer
            // 
            this.btnLockMapOnPlayer.BindableChecked = true;
            this.btnLockMapOnPlayer.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.AfterText;
            this.btnLockMapOnPlayer.Checked = true;
            this.btnLockMapOnPlayer.Hint = "Check to hold Player in the center of the Map";
            this.btnLockMapOnPlayer.Id = 114;
            this.btnLockMapOnPlayer.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.btnLockMapOnPlayer.ItemShortcut = new DevExpress.XtraBars.BarShortcut(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                | System.Windows.Forms.Keys.P));
            this.btnLockMapOnPlayer.Name = "btnLockMapOnPlayer";
            // 
            // btnSettings
            // 
            this.btnSettings.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Left;
            this.btnSettings.Caption = "Settings";
            this.btnSettings.Hint = "Open Mapper settings panel";
            this.btnSettings.Id = 129;
            this.btnSettings.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.btnSettings.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSettings.ImageOptions.Image")));
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_ShowSettingsTab);
            // 
            // groupSaveUndo
            // 
            this.groupSaveUndo.Caption = "SaveUndo";
            this.groupSaveUndo.Id = 125;
            this.groupSaveUndo.ItemLinks.Add(this.btnUndo);
            this.groupSaveUndo.ItemLinks.Add(this.btnSaveMeshes);
            this.groupSaveUndo.Name = "groupSaveUndo";
            // 
            // groupZoom
            // 
            this.groupZoom.Caption = "Zoom";
            this.groupZoom.Id = 118;
            this.groupZoom.ItemLinks.Add(this.btnZoomIn);
            this.groupZoom.ItemLinks.Add(this.lblZoom);
            this.groupZoom.ItemLinks.Add(this.btnZoomOut);
            this.groupZoom.Name = "groupZoom";
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Caption = "ZoomIn";
            this.btnZoomIn.Id = 119;
            this.btnZoomIn.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.btnZoomIn.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnZoomIn.ImageOptions.Image")));
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_ZoomIn);
            // 
            // lblZoom
            // 
            this.lblZoom.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.lblZoom.Caption = "zoom";
            this.lblZoom.Id = 122;
            this.lblZoom.Name = "lblZoom";
            this.lblZoom.ItemDoubleClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_DoubleClickZoom);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Caption = "ZoomOut";
            this.btnZoomOut.Id = 120;
            this.btnZoomOut.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.btnZoomOut.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnZoomOut.ImageOptions.Image")));
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_ZoomOut);
            // 
            // btnPanelVisibility
            // 
            this.btnPanelVisibility.Caption = "Panel Visibility";
            this.btnPanelVisibility.Hint = "Show (hide) tools panel";
            this.btnPanelVisibility.Id = 135;
            this.btnPanelVisibility.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnPanelVisibility.ImageOptions.Image")));
            this.btnPanelVisibility.Name = "btnPanelVisibility";
            this.btnPanelVisibility.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_Change_PanelVisibility);
            // 
            // lblPlayerPos
            // 
            this.lblPlayerPos.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.lblPlayerPos.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.lblPlayerPos.Caption = "player";
            this.lblPlayerPos.Hint = "Player coordinates";
            this.lblPlayerPos.Id = 130;
            this.lblPlayerPos.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.lblPlayerPos.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("lblPlayerPos.ImageOptions.Image")));
            this.lblPlayerPos.Name = "lblPlayerPos";
            this.lblPlayerPos.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // lblMousePos
            // 
            this.lblMousePos.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.lblMousePos.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.lblMousePos.Caption = "mouse";
            this.lblMousePos.Description = "Mouse position";
            this.lblMousePos.Hint = "The coordinates of the mouse pointer";
            this.lblMousePos.Id = 82;
            this.lblMousePos.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.lblMousePos.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("lblMousePos.ImageOptions.Image")));
            this.lblMousePos.Name = "lblMousePos";
            this.lblMousePos.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // lblDrawInfo
            // 
            this.lblDrawInfo.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.lblDrawInfo.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.lblDrawInfo.Caption = "form";
            this.lblDrawInfo.Hint = "Windows redraw information";
            this.lblDrawInfo.Id = 131;
            this.lblDrawInfo.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.lblDrawInfo.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("lblDrawInfo.ImageOptions.Image")));
            this.lblDrawInfo.Name = "lblDrawInfo";
            this.lblDrawInfo.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager;
            this.barDockControlTop.Size = new System.Drawing.Size(429, 49);
            this.barDockControlTop.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.handler_PreviewKeyDown);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 373);
            this.barDockControlBottom.Manager = this.barManager;
            this.barDockControlBottom.Size = new System.Drawing.Size(429, 26);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 49);
            this.barDockControlLeft.Manager = this.barManager;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 324);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(429, 49);
            this.barDockControlRight.Manager = this.barManager;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 324);
            // 
            // seDeleteRadius
            // 
            this.seDeleteRadius.AutoHeight = false;
            this.seDeleteRadius.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.seDeleteRadius.IsFloatValue = false;
            this.seDeleteRadius.Mask.EditMask = "N00";
            this.seDeleteRadius.MaxLength = 4;
            this.seDeleteRadius.MaxValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.seDeleteRadius.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.seDeleteRadius.Name = "seDeleteRadius";
            // 
            // editItemColor
            // 
            this.editItemColor.AutoHeight = false;
            this.editItemColor.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.editItemColor.Name = "editItemColor";
            // 
            // btnShowStatBar
            // 
            this.btnShowStatBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnShowStatBar.BackColor = System.Drawing.Color.Transparent;
            this.btnShowStatBar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnShowStatBar.BackgroundImage")));
            this.btnShowStatBar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnShowStatBar.FlatAppearance.BorderSize = 0;
            this.btnShowStatBar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnShowStatBar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnShowStatBar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShowStatBar.Location = new System.Drawing.Point(412, 382);
            this.btnShowStatBar.Name = "btnShowStatBar";
            this.btnShowStatBar.Size = new System.Drawing.Size(16, 16);
            this.btnShowStatBar.TabIndex = 4;
            this.btnShowStatBar.UseVisualStyleBackColor = false;
            this.btnShowStatBar.Visible = false;
            this.btnShowStatBar.Click += new System.EventHandler(this.handler_ShowStatusBar);
            // 
            // MapPicture
            // 
            this.MapPicture.BackColor = System.Drawing.Color.Black;
            this.MapPicture.Cursor = System.Windows.Forms.Cursors.Cross;
            this.MapPicture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MapPicture.Location = new System.Drawing.Point(0, 49);
            this.MapPicture.Name = "MapPicture";
            this.MapPicture.Size = new System.Drawing.Size(429, 324);
            this.MapPicture.TabIndex = 9;
            this.MapPicture.TabStop = false;
            this.MapPicture.MouseClick += new System.Windows.Forms.MouseEventHandler(this.handler_MouseClick);
            this.MapPicture.MouseMove += new System.Windows.Forms.MouseEventHandler(this.handler_MouseMove);
            this.MapPicture.MouseUp += new System.Windows.Forms.MouseEventHandler(this.handler_MouseUp);
            this.MapPicture.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.handler_PreviewKeyDown);
            // 
            // MapperFormExt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(429, 399);
            this.Controls.Add(this.btnShowStatBar);
            this.Controls.Add(this.MapPicture);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.DoubleBuffered = true;
            this.HelpButton = true;
            this.IconOptions.ShowIcon = false;
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "MapperFormExt";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Mapper(Extended)";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.handler_FormClose);
            this.Load += new System.EventHandler(this.handler_FormLoad);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.handler_HelpRequested);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.handler_KeyUp);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.handler_PreviewKeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.popMenuOptions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seWaypointDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seMaxZDifference)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seEquivalenceDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seDeleteRadius)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.editItemColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsrcAstralSettings)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapPicture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

    #endregion

        private BackgroundWorker backgroundWorker;
        private BarManager barManager;
        private BarDockControl barDockControlTop;
        private BarDockControl barDockControlBottom;
        private BarDockControl barDockControlLeft;
        private BarDockControl barDockControlRight;
        private BarCheckItem btnMappingUnidirectional;
        private RepositoryItemSpinEdit seDeleteRadius;
        private BarButtonItem btnAddCR;
        private BarButtonItem btnEditCR;
        private BarButtonItem btnImportMeshesFromGame;
        private BarButtonItem btnImportMeshesFromProfile;
        private BarButtonItem btnExportMeshes;
        private BarEditItem editWaypointDistance;
        private RepositoryItemSpinEdit seWaypointDistance;
        private BarEditItem editMaxZDifference;
        private RepositoryItemSpinEdit seMaxZDifference;
        private BarEditItem editEquivalenceDistance;
        private RepositoryItemSpinEdit seEquivalenceDistance;
        private BarButtonItem btnClearMeshes;
        private BarCheckItem btnMappingBidirectional;
        private BarCheckItem btnMappingForceLink;
        private BarCheckItem btnMappingLinearPath;
        private BarCheckItem btnMappingStop;
        private Bar barMapping;
        private BarButtonGroup groupMapping;
        private BarButtonGroup groupImportExportNodes;
        private BarButtonItem btnOptions;
        private PopupMenu popMenuOptions;
        private Bar barStatus;
        private BarStaticItem lblMousePos;
        private BindingSource bsrcAstralSettings;
        private Button btnShowStatBar;
        private BarButtonItem btnSaveMeshes;
        private Bar barGraphTools;
        private BarCheckItem btnMoveNodes;
        private BarCheckItem btnRemoveNodes;
        private BarCheckItem btnEditEdges;
        private BarButtonGroup groupEditMeshes;
        private BarButtonItem btnUndo;
        private BarButtonItem btnMeshesInfo;
        private BarButtonItem btnCompression;
        private BarCheckItem btnLockMapOnPlayer;
        private PictureBox MapPicture;
        private BarButtonGroup groupZoom;
        private BarButtonItem btnZoomIn;
        private BarButtonItem btnZoomOut;
        private BarStaticItem lblZoom;
        private Bar barCustomRegions;
        private Bar barGraphEditTools;
        private BarButtonGroup groupSaveUndo;
        private RepositoryItemColorEdit editItemColor;
        private BarButtonItem btnSettings;
        private BarStaticItem lblPlayerPos;
        private BarStaticItem lblDrawInfo;
        private BarCheckItem btnDistanceMeasurement;
        private BarCheckItem btnObjectInfo;
        private BarButtonItem btnPanelVisibility;
        private BarCheckItem btnAddNodes;
    } 
#endif
}