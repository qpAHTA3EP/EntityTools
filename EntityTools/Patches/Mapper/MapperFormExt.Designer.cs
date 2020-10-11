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
            this.btnBidirectional = new DevExpress.XtraBars.BarCheckItem();
            this.btnUnidirectional = new DevExpress.XtraBars.BarCheckItem();
            this.btnStopMapping = new DevExpress.XtraBars.BarCheckItem();
            this.btnLinearPath = new DevExpress.XtraBars.BarCheckItem();
            this.btnForceLinkLast = new DevExpress.XtraBars.BarCheckItem();
            this.btnOptions = new DevExpress.XtraBars.BarButtonItem();
            this.popMenuOptions = new DevExpress.XtraBars.PopupMenu(this.components);
            this.editWaypointDistance = new DevExpress.XtraBars.BarEditItem();
            this.seWaypointDistance = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.editMaxZDifference = new DevExpress.XtraBars.BarEditItem();
            this.seMaxZDifference = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.editEquivalenceDistance = new DevExpress.XtraBars.BarEditItem();
            this.seEquivalenceDistance = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.editDeleteRadius = new DevExpress.XtraBars.BarEditItem();
            this.seDeleteRadius = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.barNodeTools = new DevExpress.XtraBars.Bar();
            this.btnUndo = new DevExpress.XtraBars.BarButtonItem();
            this.groupEditMeshes = new DevExpress.XtraBars.BarButtonGroup();
            this.btnMoveNodes = new DevExpress.XtraBars.BarCheckItem();
            this.btnRemoveNodes = new DevExpress.XtraBars.BarCheckItem();
            this.btnEditEdges = new DevExpress.XtraBars.BarCheckItem();
            this.barMeshes = new DevExpress.XtraBars.Bar();
            this.btnSaveMeshes = new DevExpress.XtraBars.BarButtonItem();
            this.groupImportExportNodes = new DevExpress.XtraBars.BarButtonGroup();
            this.btnImportMeshesFromGame = new DevExpress.XtraBars.BarButtonItem();
            this.btnImportMeshesFromProfile = new DevExpress.XtraBars.BarButtonItem();
            this.btnExportMeshes = new DevExpress.XtraBars.BarButtonItem();
            this.btnClearMeshes = new DevExpress.XtraBars.BarButtonItem();
            this.btnMeshesInfo = new DevExpress.XtraBars.BarButtonItem();
            this.btnCompression = new DevExpress.XtraBars.BarButtonItem();
            this.barCustomRegions = new DevExpress.XtraBars.Bar();
            this.btnAddCR = new DevExpress.XtraBars.BarButtonItem();
            this.btnEditCR = new DevExpress.XtraBars.BarCheckItem();
            this.barEditCustomRegion = new DevExpress.XtraBars.Bar();
            this.btnCRTypeSelector = new DevExpress.XtraBars.BarCheckItem();
            this.editCRSelector = new DevExpress.XtraBars.BarEditItem();
            this.itemEditCRList = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.editCRName = new DevExpress.XtraBars.BarEditItem();
            this.itemEditCRName = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.btnCRRename = new DevExpress.XtraBars.BarCheckItem();
            this.btnCRAdditionAccept = new DevExpress.XtraBars.BarButtonItem();
            this.btnCREditionAccept = new DevExpress.XtraBars.BarButtonItem();
            this.btnCRCancel = new DevExpress.XtraBars.BarButtonItem();
            this.barStatus = new DevExpress.XtraBars.Bar();
            this.btnLockMapOnPlayer = new DevExpress.XtraBars.BarCheckItem();
            this.groupZoom = new DevExpress.XtraBars.BarButtonGroup();
            this.btnZoomIn = new DevExpress.XtraBars.BarButtonItem();
            this.lblZoom = new DevExpress.XtraBars.BarStaticItem();
            this.btnZoomOut = new DevExpress.XtraBars.BarButtonItem();
            this.lblMousePos = new DevExpress.XtraBars.BarStaticItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.menuImportMesh = new DevExpress.XtraBars.BarButtonItem();
            this.groupCR = new DevExpress.XtraBars.BarButtonGroup();
            this.repositoryItemZoomTrackBar1 = new DevExpress.XtraEditors.Repository.RepositoryItemZoomTrackBar();
            this.bsrcAstralSettings = new System.Windows.Forms.BindingSource(this.components);
            this.btnShowStatBar = new System.Windows.Forms.Button();
            this.MapPicture = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.popMenuOptions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seWaypointDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seMaxZDifference)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seEquivalenceDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seDeleteRadius)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.itemEditCRList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.itemEditCRName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemZoomTrackBar1)).BeginInit();
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
            this.barNodeTools,
            this.barMeshes,
            this.barCustomRegions,
            this.barEditCustomRegion,
            this.barStatus});
            this.barManager.DockControls.Add(this.barDockControlTop);
            this.barManager.DockControls.Add(this.barDockControlBottom);
            this.barManager.DockControls.Add(this.barDockControlLeft);
            this.barManager.DockControls.Add(this.barDockControlRight);
            this.barManager.Form = this;
            this.barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.btnUnidirectional,
            this.btnAddCR,
            this.btnImportMeshesFromGame,
            this.btnImportMeshesFromProfile,
            this.menuImportMesh,
            this.btnExportMeshes,
            this.editWaypointDistance,
            this.editMaxZDifference,
            this.editEquivalenceDistance,
            this.editDeleteRadius,
            this.btnClearMeshes,
            this.btnBidirectional,
            this.btnForceLinkLast,
            this.btnLinearPath,
            this.btnStopMapping,
            this.groupMapping,
            this.groupCR,
            this.groupImportExportNodes,
            this.btnOptions,
            this.editCRName,
            this.btnCRAdditionAccept,
            this.btnCRCancel,
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
            this.btnCRTypeSelector,
            this.btnCREditionAccept,
            this.editCRSelector,
            this.btnLockMapOnPlayer,
            this.btnCRRename,
            this.groupZoom,
            this.btnZoomIn,
            this.btnZoomOut,
            this.lblZoom});
            this.barManager.MaxItemId = 125;
            this.barManager.OptionsLayout.AllowAddNewItems = false;
            this.barManager.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.seDeleteRadius,
            this.seWaypointDistance,
            this.seMaxZDifference,
            this.seEquivalenceDistance,
            this.itemEditCRName,
            this.itemEditCRList,
            this.repositoryItemZoomTrackBar1});
            this.barManager.StatusBar = this.barStatus;
            // 
            // barMapping
            // 
            this.barMapping.BarName = "Mapping";
            this.barMapping.DockCol = 0;
            this.barMapping.DockRow = 0;
            this.barMapping.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.barMapping.FloatLocation = new System.Drawing.Point(325, 181);
            this.barMapping.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.groupMapping, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnOptions, true)});
            this.barMapping.OptionsBar.AllowQuickCustomization = false;
            this.barMapping.Text = "Mapping";
            this.barMapping.Visible = false;
            this.barMapping.VisibleChanged += new System.EventHandler(this.handler_BarVisibleChanged);
            // 
            // groupMapping
            // 
            this.groupMapping.Caption = "Mapping";
            this.groupMapping.Id = 70;
            this.groupMapping.ItemLinks.Add(this.btnBidirectional);
            this.groupMapping.ItemLinks.Add(this.btnUnidirectional);
            this.groupMapping.ItemLinks.Add(this.btnStopMapping);
            this.groupMapping.ItemLinks.Add(this.btnLinearPath, true);
            this.groupMapping.ItemLinks.Add(this.btnForceLinkLast);
            this.groupMapping.Name = "groupMapping";
            // 
            // btnBidirectional
            // 
            this.btnBidirectional.Caption = "Bidirectional Mapping";
            this.btnBidirectional.GroupIndex = 1;
            this.btnBidirectional.Id = 64;
            this.btnBidirectional.ImageOptions.Image = global::EntityTools.Properties.Resources.miniBiPath;
            this.btnBidirectional.ItemShortcut = new DevExpress.XtraBars.BarShortcut(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                | System.Windows.Forms.Keys.B));
            this.btnBidirectional.Name = "btnBidirectional";
            this.btnBidirectional.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_Mapping_BidirectionalPath);
            // 
            // btnUnidirectional
            // 
            this.btnUnidirectional.Caption = "Unidirectional Mapping";
            this.btnUnidirectional.GroupIndex = 1;
            this.btnUnidirectional.Id = 44;
            this.btnUnidirectional.ImageOptions.Image = global::EntityTools.Properties.Resources.miniUniPath;
            this.btnUnidirectional.ItemShortcut = new DevExpress.XtraBars.BarShortcut(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                | System.Windows.Forms.Keys.U));
            this.btnUnidirectional.Name = "btnUnidirectional";
            this.btnUnidirectional.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_Mapping_UnidirectionalPath);
            // 
            // btnStopMapping
            // 
            this.btnStopMapping.BindableChecked = true;
            this.btnStopMapping.Caption = "Stop Mapping";
            this.btnStopMapping.Checked = true;
            this.btnStopMapping.GroupIndex = 1;
            this.btnStopMapping.Id = 67;
            this.btnStopMapping.ImageOptions.Image = global::EntityTools.Properties.Resources.miniStop;
            this.btnStopMapping.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.End));
            this.btnStopMapping.Name = "btnStopMapping";
            this.btnStopMapping.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.btnStopMapping_CheckedChanged);
            this.btnStopMapping.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_Mapping_Stop);
            // 
            // btnLinearPath
            // 
            this.btnLinearPath.Caption = "Linear Path";
            this.btnLinearPath.Id = 66;
            this.btnLinearPath.ImageOptions.Image = global::EntityTools.Properties.Resources.miniLinePath;
            this.btnLinearPath.ItemShortcut = new DevExpress.XtraBars.BarShortcut(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                | System.Windows.Forms.Keys.L));
            this.btnLinearPath.Name = "btnLinearPath";
            this.btnLinearPath.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_Mapping_LinearPath);
            // 
            // btnForceLinkLast
            // 
            this.btnForceLinkLast.BindableChecked = true;
            this.btnForceLinkLast.Caption = "Force Linking to Last Node";
            this.btnForceLinkLast.Checked = true;
            this.btnForceLinkLast.Id = 65;
            this.btnForceLinkLast.ImageOptions.Image = global::EntityTools.Properties.Resources.miniHurdLink;
            this.btnForceLinkLast.ItemShortcut = new DevExpress.XtraBars.BarShortcut(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
                | System.Windows.Forms.Keys.L));
            this.btnForceLinkLast.Name = "btnForceLinkLast";
            this.btnForceLinkLast.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_Mapping_ForceLink);
            // 
            // btnOptions
            // 
            this.btnOptions.ActAsDropDown = true;
            this.btnOptions.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
            this.btnOptions.Caption = "Options";
            this.btnOptions.DropDownControl = this.popMenuOptions;
            this.btnOptions.Hint = "Options";
            this.btnOptions.Id = 74;
            this.btnOptions.ImageOptions.Image = global::EntityTools.Properties.Resources.miniGear;
            this.btnOptions.Name = "btnOptions";
            // 
            // popMenuOptions
            // 
            this.popMenuOptions.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.editWaypointDistance),
            new DevExpress.XtraBars.LinkPersistInfo(this.editMaxZDifference),
            new DevExpress.XtraBars.LinkPersistInfo(this.editEquivalenceDistance),
            new DevExpress.XtraBars.LinkPersistInfo(this.editDeleteRadius)});
            this.popMenuOptions.Manager = this.barManager;
            this.popMenuOptions.Name = "popMenuOptions";
            // 
            // editWaypointDistance
            // 
            this.editWaypointDistance.Caption = "Waypoint Distance";
            this.editWaypointDistance.Edit = this.seWaypointDistance;
            this.editWaypointDistance.Id = 58;
            this.editWaypointDistance.ImageOptions.Image = global::EntityTools.Properties.Resources.miniNodeDistance;
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
            this.editMaxZDifference.ImageOptions.Image = global::EntityTools.Properties.Resources.miniZdiff;
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
            this.editEquivalenceDistance.ImageOptions.Image = global::EntityTools.Properties.Resources.miniDistance;
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
            // editDeleteRadius
            // 
            this.editDeleteRadius.Caption = "Node Delete Radius";
            this.editDeleteRadius.Edit = this.seDeleteRadius;
            this.editDeleteRadius.Id = 61;
            this.editDeleteRadius.ImageOptions.Image = global::EntityTools.Properties.Resources.miniTarget;
            this.editDeleteRadius.Name = "editDeleteRadius";
            this.editDeleteRadius.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
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
            // barNodeTools
            // 
            this.barNodeTools.BarName = "NodeTools";
            this.barNodeTools.DockCol = 1;
            this.barNodeTools.DockRow = 0;
            this.barNodeTools.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.barNodeTools.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnUndo),
            new DevExpress.XtraBars.LinkPersistInfo(this.groupEditMeshes, true)});
            this.barNodeTools.Offset = 207;
            this.barNodeTools.Text = "NodeTools";
            this.barNodeTools.Visible = false;
            // 
            // btnUndo
            // 
            this.btnUndo.Caption = "Undo";
            this.btnUndo.Id = 101;
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
            this.groupEditMeshes.ItemLinks.Add(this.btnEditEdges);
            this.groupEditMeshes.Name = "groupEditMeshes";
            // 
            // btnMoveNodes
            // 
            this.btnMoveNodes.Caption = "Relocation Nodes";
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
            this.btnRemoveNodes.ImageOptions.Image = global::EntityTools.Properties.Resources.miniCancel;
            this.btnRemoveNodes.ItemShortcut = new DevExpress.XtraBars.BarShortcut(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                | System.Windows.Forms.Keys.D));
            this.btnRemoveNodes.Name = "btnRemoveNodes";
            this.btnRemoveNodes.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_DeleteNodes_ModeChanged);
            // 
            // btnEditEdges
            // 
            this.btnEditEdges.Caption = "Edit Edges";
            this.btnEditEdges.Id = 94;
            this.btnEditEdges.ImageOptions.Image = global::EntityTools.Properties.Resources.miniEditEdge;
            this.btnEditEdges.ItemShortcut = new DevExpress.XtraBars.BarShortcut(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                | System.Windows.Forms.Keys.E));
            this.btnEditEdges.Name = "btnEditEdges";
            this.btnEditEdges.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_EditEdges_ModeChanged);
            // 
            // barMeshes
            // 
            this.barMeshes.BarName = "ImportExport";
            this.barMeshes.DockCol = 0;
            this.barMeshes.DockRow = 1;
            this.barMeshes.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.barMeshes.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnUndo),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnSaveMeshes),
            new DevExpress.XtraBars.LinkPersistInfo(this.groupImportExportNodes, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnMeshesInfo, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnCompression)});
            this.barMeshes.OptionsBar.AllowQuickCustomization = false;
            this.barMeshes.Text = "ImportExport";
            this.barMeshes.Visible = false;
            this.barMeshes.VisibleChanged += new System.EventHandler(this.handler_BarVisibleChanged);
            // 
            // btnSaveMeshes
            // 
            this.btnSaveMeshes.Caption = "Save";
            this.btnSaveMeshes.Id = 91;
            this.btnSaveMeshes.ImageOptions.Image = global::EntityTools.Properties.Resources.miniDiskette;
            this.btnSaveMeshes.ImageOptions.LargeImage = global::EntityTools.Properties.Resources.miniDiskette;
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
            this.btnImportMeshesFromGame.ImageOptions.Image = global::EntityTools.Properties.Resources.miniClone;
            this.btnImportMeshesFromGame.Name = "btnImportMeshesFromGame";
            this.btnImportMeshesFromGame.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_ImportCurrentMapMeshesFromGame);
            // 
            // btnImportMeshesFromProfile
            // 
            this.btnImportMeshesFromProfile.Caption = "Import from Profile";
            this.btnImportMeshesFromProfile.Id = 52;
            this.btnImportMeshesFromProfile.ImageOptions.Image = global::EntityTools.Properties.Resources.miniImport;
            this.btnImportMeshesFromProfile.Name = "btnImportMeshesFromProfile";
            this.btnImportMeshesFromProfile.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_ImportMapMeshesFromFile);
            // 
            // btnExportMeshes
            // 
            this.btnExportMeshes.Caption = "Export to Mesh";
            this.btnExportMeshes.Enabled = false;
            this.btnExportMeshes.Id = 55;
            this.btnExportMeshes.ImageOptions.Image = global::EntityTools.Properties.Resources.miniExport;
            this.btnExportMeshes.Name = "btnExportMeshes";
            this.btnExportMeshes.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            this.btnExportMeshes.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_ExportCurrentMapMeshes2File);
            // 
            // btnClearMeshes
            // 
            this.btnClearMeshes.Caption = "Clear";
            this.btnClearMeshes.Id = 63;
            this.btnClearMeshes.ImageOptions.Image = global::EntityTools.Properties.Resources.miniDelete;
            this.btnClearMeshes.Name = "btnClearMeshes";
            this.btnClearMeshes.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_ClearCurrentMapMeshes);
            // 
            // btnMeshesInfo
            // 
            this.btnMeshesInfo.Caption = "MeshesInfo";
            this.btnMeshesInfo.Hint = "Meshes information";
            this.btnMeshesInfo.Id = 103;
            this.btnMeshesInfo.ImageOptions.Image = global::EntityTools.Properties.Resources.miniAnalize;
            this.btnMeshesInfo.Name = "btnMeshesInfo";
            this.btnMeshesInfo.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_MeshesInfo);
            // 
            // btnCompression
            // 
            this.btnCompression.Caption = "Compression";
            this.btnCompression.Hint = "Удаление из графа непроходимых(невидимых) вершин и ребер";
            this.btnCompression.Id = 104;
            this.btnCompression.ImageOptions.Image = global::EntityTools.Properties.Resources.miniWizard;
            this.btnCompression.Name = "btnCompression";
            this.btnCompression.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_MeshesCompression);
            // 
            // barCustomRegions
            // 
            this.barCustomRegions.BarName = "CustomRegions";
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
            this.barCustomRegions.Text = "CustomRegions";
            this.barCustomRegions.Visible = false;
            // 
            // btnAddCR
            // 
            this.btnAddCR.Caption = "Add CustomRegion";
            this.btnAddCR.Id = 48;
            this.btnAddCR.ImageOptions.Image = global::EntityTools.Properties.Resources.miniAddCR_3;
            this.btnAddCR.Name = "btnAddCR";
            this.btnAddCR.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_AddCustomRegion);
            // 
            // btnEditCR
            // 
            this.btnEditCR.Caption = "Edit CustomRegion";
            this.btnEditCR.Id = 102;
            this.btnEditCR.ImageOptions.Image = global::EntityTools.Properties.Resources.miniEditCR;
            this.btnEditCR.Name = "btnEditCR";
            this.btnEditCR.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_EditCustomRegion);
            // 
            // barEditCustomRegion
            // 
            this.barEditCustomRegion.BarName = "barEditCustomRegion";
            this.barEditCustomRegion.CanDockStyle = ((DevExpress.XtraBars.BarCanDockStyle)((((DevExpress.XtraBars.BarCanDockStyle.Floating | DevExpress.XtraBars.BarCanDockStyle.Top) 
            | DevExpress.XtraBars.BarCanDockStyle.Bottom) 
            | DevExpress.XtraBars.BarCanDockStyle.Standalone)));
            this.barEditCustomRegion.DockCol = 0;
            this.barEditCustomRegion.DockRow = 0;
            this.barEditCustomRegion.FloatLocation = new System.Drawing.Point(63, 216);
            this.barEditCustomRegion.FloatSize = new System.Drawing.Size(397, 88);
            this.barEditCustomRegion.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnCRTypeSelector),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.Width, this.editCRSelector, "", false, true, true, 231),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.Width, this.editCRName, "", false, true, true, 216),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnCRRename),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnCRAdditionAccept),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnCREditionAccept),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnCRCancel)});
            this.barEditCustomRegion.Offset = 7;
            this.barEditCustomRegion.OptionsBar.AllowQuickCustomization = false;
            this.barEditCustomRegion.OptionsBar.DisableClose = true;
            this.barEditCustomRegion.Text = "Edit CustomRegion";
            this.barEditCustomRegion.Visible = false;
            // 
            // btnCRTypeSelector
            // 
            this.btnCRTypeSelector.Caption = "CustormRegion Type";
            this.btnCRTypeSelector.Hint = "Change type of the CustomRegion (Rectangular to Elliptical)";
            this.btnCRTypeSelector.Id = 106;
            this.btnCRTypeSelector.ImageOptions.Image = global::EntityTools.Properties.Resources.miniCRRectang;
            this.btnCRTypeSelector.Name = "btnCRTypeSelector";
            this.btnCRTypeSelector.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_ChangeCustomRegionType);
            // 
            // editCRSelector
            // 
            this.editCRSelector.Edit = this.itemEditCRList;
            this.editCRSelector.Id = 112;
            this.editCRSelector.Name = "editCRSelector";
            this.editCRSelector.EditValueChanged += new System.EventHandler(this.handler_ChangeSelectedCustomRegion);
            // 
            // itemEditCRList
            // 
            this.itemEditCRList.AcceptEditorTextAsNewValue = DevExpress.Utils.DefaultBoolean.False;
            this.itemEditCRList.AllowDropDownWhenReadOnly = DevExpress.Utils.DefaultBoolean.True;
            this.itemEditCRList.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.itemEditCRList.AutoHeight = false;
            this.itemEditCRList.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.itemEditCRList.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "", 20, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.Ascending, DevExpress.Utils.DefaultBoolean.Default)});
            this.itemEditCRList.Name = "itemEditCRList";
            this.itemEditCRList.NullText = "";
            this.itemEditCRList.ShowFooter = false;
            // 
            // editCRName
            // 
            this.editCRName.Edit = this.itemEditCRName;
            this.editCRName.Hint = "Enter name of the CustomRegion";
            this.editCRName.Id = 78;
            this.editCRName.Name = "editCRName";
            // 
            // itemEditCRName
            // 
            this.itemEditCRName.AutoHeight = false;
            this.itemEditCRName.Name = "itemEditCRName";
            // 
            // btnCRRename
            // 
            this.btnCRRename.Caption = "Edit CRName";
            this.btnCRRename.Id = 115;
            this.btnCRRename.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCRRename.ImageOptions.Image")));
            this.btnCRRename.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnCRRename.ImageOptions.LargeImage")));
            this.btnCRRename.Name = "btnCRRename";
            this.btnCRRename.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_ChangedRenameCRMode);
            // 
            // btnCRAdditionAccept
            // 
            this.btnCRAdditionAccept.Caption = "Accept";
            this.btnCRAdditionAccept.DropDownEnabled = false;
            this.btnCRAdditionAccept.Id = 79;
            this.btnCRAdditionAccept.ImageOptions.Image = global::EntityTools.Properties.Resources.miniAdd;
            this.btnCRAdditionAccept.Name = "btnCRAdditionAccept";
            this.btnCRAdditionAccept.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_AcceptCRAddition);
            // 
            // btnCREditionAccept
            // 
            this.btnCREditionAccept.Caption = "Accept";
            this.btnCREditionAccept.Id = 107;
            this.btnCREditionAccept.ImageOptions.Image = global::EntityTools.Properties.Resources.miniValid;
            this.btnCREditionAccept.Name = "btnCREditionAccept";
            this.btnCREditionAccept.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_AcceptCREdition);
            // 
            // btnCRCancel
            // 
            this.btnCRCancel.Caption = "Cancel";
            this.btnCRCancel.Id = 80;
            this.btnCRCancel.ImageOptions.Image = global::EntityTools.Properties.Resources.miniCancel;
            this.btnCRCancel.Name = "btnCRCancel";
            this.btnCRCancel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_CancelCRManipulation);
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
            new DevExpress.XtraBars.LinkPersistInfo(this.groupZoom, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.lblMousePos, true)});
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
            this.btnLockMapOnPlayer.ItemShortcut = new DevExpress.XtraBars.BarShortcut(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                | System.Windows.Forms.Keys.P));
            this.btnLockMapOnPlayer.Name = "btnLockMapOnPlayer";
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
            this.btnZoomIn.ImageOptions.Image = global::EntityTools.Properties.Resources.miniZoomIn;
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_ZoomIn);
            // 
            // lblZoom
            // 
            this.lblZoom.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.lblZoom.Caption = "lblZoom";
            this.lblZoom.Id = 122;
            this.lblZoom.Name = "lblZoom";
            this.lblZoom.ItemDoubleClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_DoubleClickZoom);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Caption = "ZoomOut";
            this.btnZoomOut.Id = 120;
            this.btnZoomOut.ImageOptions.Image = global::EntityTools.Properties.Resources.miniZoomOut;
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_ZoomOut);
            // 
            // lblMousePos
            // 
            this.lblMousePos.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.lblMousePos.Caption = "mousePos";
            this.lblMousePos.Description = "Player position";
            this.lblMousePos.Hint = "Player position";
            this.lblMousePos.Id = 82;
            this.lblMousePos.Name = "lblMousePos";
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager;
            this.barDockControlTop.Size = new System.Drawing.Size(398, 56);
            this.barDockControlTop.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.handler_PreviewKeyDown);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 351);
            this.barDockControlBottom.Manager = this.barManager;
            this.barDockControlBottom.Size = new System.Drawing.Size(398, 28);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 56);
            this.barDockControlLeft.Manager = this.barManager;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 295);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(398, 56);
            this.barDockControlRight.Manager = this.barManager;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 295);
            // 
            // menuImportMesh
            // 
            this.menuImportMesh.Caption = "Import from Mesh";
            this.menuImportMesh.Id = 53;
            this.menuImportMesh.ImageOptions.Image = global::EntityTools.Properties.Resources.miniImport;
            this.menuImportMesh.Name = "menuImportMesh";
            // 
            // groupCR
            // 
            this.groupCR.Caption = "CustomRegion";
            this.groupCR.Id = 71;
            this.groupCR.ItemLinks.Add(this.btnAddCR);
            this.groupCR.ItemLinks.Add(this.btnEditCR);
            this.groupCR.Name = "groupCR";
            // 
            // repositoryItemZoomTrackBar1
            // 
            this.repositoryItemZoomTrackBar1.Name = "repositoryItemZoomTrackBar1";
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
            this.btnShowStatBar.Location = new System.Drawing.Point(382, 363);
            this.btnShowStatBar.Name = "btnShowStatBar";
            this.btnShowStatBar.Size = new System.Drawing.Size(16, 16);
            this.btnShowStatBar.TabIndex = 4;
            this.btnShowStatBar.UseVisualStyleBackColor = false;
            this.btnShowStatBar.Visible = false;
            this.btnShowStatBar.Click += new System.EventHandler(this.handler_ShowStatusBar);
            // 
            // MapPicture
            // 
            this.MapPicture.Cursor = System.Windows.Forms.Cursors.Cross;
            this.MapPicture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MapPicture.Location = new System.Drawing.Point(0, 56);
            this.MapPicture.Name = "MapPicture";
            this.MapPicture.Size = new System.Drawing.Size(398, 295);
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
            this.ClientSize = new System.Drawing.Size(398, 379);
            this.Controls.Add(this.btnShowStatBar);
            this.Controls.Add(this.MapPicture);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.DoubleBuffered = true;
            this.LookAndFeel.SkinName = "Office 2013 Dark Gray";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "MapperFormExt";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Mapper(Extended)";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.handler_FormClose);
            this.Load += new System.EventHandler(this.handler_FormLoad);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.handler_KeyUp);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.handler_PreviewKeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.popMenuOptions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seWaypointDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seMaxZDifference)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seEquivalenceDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seDeleteRadius)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.itemEditCRList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.itemEditCRName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemZoomTrackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsrcAstralSettings)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapPicture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

    #endregion

        private static MapperFormExt @this;

        private BackgroundWorker backgroundWorker;
        private BarManager barManager;
        private BarDockControl barDockControlTop;
        private BarDockControl barDockControlBottom;
        private BarDockControl barDockControlLeft;
        private BarDockControl barDockControlRight;
        private BarCheckItem btnUnidirectional;
        private RepositoryItemSpinEdit seDeleteRadius;
        private BarButtonItem btnAddCR;
        private BarButtonItem btnImportMeshesFromGame;
        private BarButtonItem btnImportMeshesFromProfile;
        private BarButtonItem menuImportMesh;
        private BarButtonItem btnExportMeshes;
        private BarEditItem editWaypointDistance;
        private RepositoryItemSpinEdit seWaypointDistance;
        private BarEditItem editMaxZDifference;
        private RepositoryItemSpinEdit seMaxZDifference;
        private BarEditItem editEquivalenceDistance;
        private RepositoryItemSpinEdit seEquivalenceDistance;
        private BarEditItem editDeleteRadius;
        private BarButtonItem btnClearMeshes;
        private BarCheckItem btnBidirectional;
        private BarCheckItem btnForceLinkLast;
        private BarCheckItem btnLinearPath;
        private BarCheckItem btnStopMapping;
        private Bar barMapping;
        private BarButtonGroup groupMapping;
        private BarButtonGroup groupCR;
        private BarButtonGroup groupImportExportNodes;
        private BarButtonItem btnOptions;
        private PopupMenu popMenuOptions;
        private BarEditItem editCRName;
        private RepositoryItemTextEdit itemEditCRName;
        private BarButtonItem btnCRAdditionAccept;
        private BarButtonItem btnCRCancel;
        private Bar barStatus;
        private BarStaticItem lblMousePos;
        private BindingSource bsrcAstralSettings;
        private Button btnShowStatBar;
        private BarButtonItem btnSaveMeshes;
        private Bar barMeshes;
        private BarCheckItem btnMoveNodes;
        private BarCheckItem btnRemoveNodes;
        private BarCheckItem btnEditEdges;
        private BarButtonGroup groupEditMeshes;
        private BarButtonItem btnUndo;
        private BarCheckItem btnEditCR;
        private BarButtonItem btnMeshesInfo;
        private BarButtonItem btnCompression;
        private Bar barEditCustomRegion;
        private BarCheckItem btnCRTypeSelector;
        private BarButtonItem btnCREditionAccept;
        private BarEditItem editCRSelector;
        private RepositoryItemLookUpEdit itemEditCRList;
        private BarCheckItem btnLockMapOnPlayer;
        private BarCheckItem btnCRRename;
        private PictureBox MapPicture;
        private BarButtonGroup groupZoom;
        private BarButtonItem btnZoomIn;
        private BarButtonItem btnZoomOut;
        private BarStaticItem lblZoom;
        private RepositoryItemZoomTrackBar repositoryItemZoomTrackBar1;
        private Bar barCustomRegions;
        private Bar barNodeTools;
    } 
#endif
}