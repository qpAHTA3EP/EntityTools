using ACTP0Tools;
using ACTP0Tools.Classes.Quester;
using Astral.Quester.Classes;
using DevExpress.XtraEditors;
using EntityTools.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

// ReSharper disable InconsistentNaming


namespace EntityTools.Patches.Mapper.Tools
{
    public partial class CustomRegionToolForm : XtraForm //Form
    {
        /// <summary>
        /// Режим внешнего вида CustomRegionToolForm
        /// </summary>
        public enum ViewMode
        {
            Undefined,
            /// <summary>
            /// Режим добавления нового CustomRegion'a
            /// </summary>
            Add,
            /// <summary>
            /// Режим редактирования CustomRegion'a
            /// </summary>
            Edit
        }

        /// <summary>
        /// 
        /// </summary>
        public enum SizeAttribute
        {
            X, 
            Y,
            Width,
            Height
        }

        public CustomRegionToolForm()
        {
            InitializeComponent();
        }

        public int crX
        {
            get => Convert.ToInt32(editX.EditValue);
            set
            {
                allowNotification = false;
                editX.EditValue = value;
                allowNotification = true;
            }
        }

        public int crY
        {
            get => Convert.ToInt32(editY.EditValue);
            set
            {
                allowNotification = false;
                editY.EditValue = value;
                allowNotification = true;
            }
        }

        public int crWidths
        {
            get => int.TryParse(editWidth.Text, out int result) ? result : 1;
            set
            {
                allowNotification = false;
                editWidth.EditValue = value;
                allowNotification = true;
            }
        }

        public int crHeight
        {
            get => int.TryParse(editHeight.Text, out int result) ? result : 1;
            set
            {
                allowNotification = false;
                editHeight.EditValue = value;
                allowNotification = true;
            }
        }

        private bool allowNotification = true;

        public void SetCustomRegionCoordinates(int x, int y)
        {
            allowNotification = false;
            editX.EditValue = x;
            editY.EditValue = y;
            allowNotification = true;
        }

        public void SetCustomRegionSize(int x, int y, int width, int height)
        {
            allowNotification = false;
            editX.EditValue = x;
            editY.EditValue = y;
            editWidth.EditValue = width;
            editHeight.EditValue = height;
            allowNotification = true;
        }

        /// <summary>
        /// Переключение режима отображения
        /// </summary>
        public ViewMode Mode
        {
            get => mode;
            set
            {
                if (mode != value)
                {
                    switch (value)
                    {
                        case ViewMode.Edit:
                            Text = @"Change CustomRegion";
                            CustomRegionSelector.Visible = true;
                            editCRName.Visible = false;
                            allowNotification = true;
                            break;
                        case ViewMode.Add:
                            Text = @"Add CustomRegion";
                            CustomRegionSelector.Visible = false;
                            editCRName.Visible = true;
                            allowNotification = true;
                            break;
                        case ViewMode.Undefined:
                            editCRName.Text = string.Empty;
                            editX.EditValue = 0;
                            editY.EditValue = 0;
                            editWidth.EditValue = 0;
                            editHeight.EditValue = 0;
                            AcceptChanges = null;
                            CancelChanges = null;
                            CustomRegionSizeChanged = null;
                            SelectedCustomRegionChanged = null;
                            allowNotification = false;
                            break;
                    }

                    mode = value;
                    RefreshCustomRegionList();
                }
            }
        }
        private ViewMode mode;

        public virtual bool IsElliptical
        {
            get => isElliptical;
            set
            {
                if (isElliptical != value)
                {
                    btnCRTypeSwitcher.ImageOptions.Image = value
                        ? Resources.EllipceCR
                        : Resources.RectCR;
                    isElliptical = value;
                }
            }
        }
        private bool isElliptical;

        /// <summary>
        /// Имя CustomRegion'a
        /// </summary>
        public string CustomRegionName
        {
            get
            {
                switch (mode)
                {
                    case ViewMode.Add:
                        return editCRName.EditValue?.ToString().Trim();
                    case ViewMode.Edit:
                        return CustomRegionSelector.Text.Trim();
                    default:
                        return String.Empty;
                }
            }
        }

        protected BindingList<CustomRegion> CustomRegions
        {
            get
            {
                if (_profile is null)
                {
                    if (Owner is MapperFormExt mapper)
                        return (_profile = mapper.Profile).CustomRegions;
                    _profile = AstralAccessors.Quester.Core.ActiveProfileProxy;
                }

                return _profile.CustomRegions;
            }
        }

        private QuesterProfileProxy _profile;

        public void Show(ViewMode viewMode, MapperFormExt owner = null, CustomRegion customRegion = null)
        {
            Mode = viewMode;
            if (viewMode == ViewMode.Undefined)
            {
                return;
            }

            if (!Visible)
            {
                if (viewMode == ViewMode.Edit)
                {
                    RefreshCustomRegionList(customRegion);
                }

                if (customRegion != null)
                {
                    SetCustomRegionSize((int)customRegion.Position.X, (int)customRegion.Position.Y, 
                                        Math.Abs(customRegion.Width), Math.Abs(customRegion.Height));
                    IsElliptical = customRegion.Eliptic;
                }
                else SetCustomRegionSize(0,0,0,0);

                if (owner != null)
                {
                    var pos = owner.Location;
                    var y = pos.Y - Height - 10;
                    if (y <= 10)
                    {
                        y = pos.Y + owner.Height + 10;
                        if (y + Height > Screen.PrimaryScreen.WorkingArea.Height)
                        {
                            y = pos.Y + 20;
                        }
                    }
                    StartPosition = FormStartPosition.Manual;
                    Location = new Point(pos.X + 20, y);
                    base.Show(owner);
                }
                else
                {
                    StartPosition = FormStartPosition.CenterParent;
                    base.Show();
                }
            }
        }

        protected new void Show() => base.Show();
        protected new void Show(IWin32Window owner) => base.Show(owner);

        public delegate void CustomRegionToolEvent(CustomRegionToolForm sender, EventArgs e, object value = null);

        public delegate void CustomRegionSizeEvent(CustomRegionToolForm sender, SizeAttribute sizeAttribute, int value);

        /// <summary>
        /// Принять изменения CustomRegion'a
        /// </summary>
        protected event CustomRegionToolEvent AcceptChanges;

        /// <summary>
        /// Отменить изменения CustomRegion'a
        /// </summary>
        protected event CustomRegionToolEvent CancelChanges;

        /// <summary>
        /// Уведомление об изменении размеров CustomRegion'a
        /// </summary>
        protected event CustomRegionSizeEvent CustomRegionSizeChanged;

        /// <summary>
        /// Отменить изменения CustomRegion'a
        /// </summary>
        protected event CustomRegionToolEvent SelectedCustomRegionChanged;

        public void SetEventHandlers(CustomRegionToolEvent onAccept, CustomRegionToolEvent onCancel, CustomRegionSizeEvent onCRSizeChanged,
            CustomRegionToolEvent onSelectCR)
        {
            if (mode != ViewMode.Undefined)
            {
                AcceptChanges = onAccept;
                CancelChanges = onCancel;
                CustomRegionSizeChanged = onCRSizeChanged;
                SelectedCustomRegionChanged = onSelectCR;
            }
            else
            {
                XtraMessageBox.Show($"Can not set the event handler in '{ViewMode.Undefined}' mode",
                    $"{nameof(CustomRegionToolForm)} initialization error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public CustomRegion SelectedCustomRegion
        {
            get =>
                mode == ViewMode.Edit
                    ? CustomRegionSelector.SelectedItem as CustomRegion
                    : null;
            set
            {
                if (mode == ViewMode.Edit)
                    CustomRegionSelector.SelectedItem = value;
            }
        }

        protected void handler_FormClosed(object sender, FormClosedEventArgs e)
        {
            AcceptChanges = null;
            CancelChanges = null;
            CustomRegionSizeChanged = null;
            SelectedCustomRegionChanged = null; 

            CustomRegionSelector.DataSource = null;
            editCRName.Text = string.Empty;
            Mode = ViewMode.Undefined;
        }

        protected void handler_ChangeCRType(object sender, EventArgs e)
        {
            IsElliptical = !IsElliptical;
        }

        protected void handler_Accept(object sender, EventArgs e)
        {
            var accept = AcceptChanges;
            if (allowNotification && accept != null)
                accept(this, e);
        }

        protected void handler_Cancel(object sender, EventArgs e)
        {
            var cancel = CancelChanges;
            if (allowNotification && cancel != null)
                cancel.Invoke(this, e);
        }

        protected void handler_SelectedCustomRegionChanged(object sender, EventArgs e)
        {
            var crChanged = SelectedCustomRegionChanged;
            if (allowNotification && crChanged != null)
                crChanged(this, e, CustomRegionSelector.SelectedItem);

            if (CustomRegionSelector.SelectedItem is CustomRegion cr)
            {
                var pos = cr.Position;
                SetCustomRegionSize((int)pos.X, (int)pos.Y, Math.Abs(cr.Width), Math.Abs(cr.Height));
                IsElliptical = cr.Eliptic;
                if (Owner is MapperFormExt mapper)
                {
                    mapper.CenterOfMap = pos.Clone();
                }
            }
        }

        public void RefreshCustomRegionList(CustomRegion customRegion = null)
        {
            CustomRegionSelector.BeginUpdate();
            if (mode == ViewMode.Edit)
            {
                var selectedItem = customRegion ?? CustomRegionSelector.SelectedItem;
                CustomRegionSelector.DataSource = null;
                CustomRegionSelector.DisplayMember = nameof(CustomRegion.Name);
                CustomRegionSelector.DataSource = CustomRegions;
                CustomRegionSelector.SelectedItem = selectedItem;
                if (customRegion != null
                    && Owner is MapperFormExt mapper)
                {
                    mapper.CenterOfMap = customRegion.Position.Clone();
                }
            }
            else
            {
                CustomRegionSelector.DataSource = null;
            }
            CustomRegionSelector.EndUpdate();
        }

        private void handler_XChanged(object sender, EventArgs e)
        {
            var sizeChanged = CustomRegionSizeChanged;
            if (allowNotification && sizeChanged != null)
                sizeChanged(this, SizeAttribute.X, crX);
        }

        private void handler_YChanged(object sender, EventArgs e)
        {
            var sizeChanged = CustomRegionSizeChanged;
            if (allowNotification && sizeChanged != null)
                sizeChanged.Invoke(this, SizeAttribute.Y, crY);
        }

        private void handled_HeightChanged(object sender, EventArgs e)
        {
            var sizeChanged = CustomRegionSizeChanged;
            if (allowNotification && sizeChanged != null)
                sizeChanged.Invoke(this, SizeAttribute.Height, crHeight);
        }

        private void handled_WidthChanged(object sender, EventArgs e)
        {
            var sizeChanged = CustomRegionSizeChanged;
            if (allowNotification && sizeChanged != null)
                sizeChanged.Invoke(this, SizeAttribute.Width, crWidths);
        }

        private void handler_SizeChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            e.Cancel = !int.TryParse(e.NewValue.ToString(), out int result) || result < 0;
        }
    }
}
