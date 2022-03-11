using AcTp0Tools;
using DevExpress.XtraEditors;
using System;
using System.Drawing;
using System.Windows.Forms;
using Astral.Quester.Classes;
using EntityTools.Properties;

namespace EntityTools.Patches.Mapper.Tools
{

    public partial class CustomRegionToolForm : XtraForm //Form
    {
        public CustomRegionToolForm()
        {
            InitializeComponent();
        }

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
        };
        
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
                            cbCRSelector.Visible = true;
                            editCRName.Visible = false;
                            break;
                        case ViewMode.Add:
                            Text = @"Add CustomRegion";
                            cbCRSelector.Visible = false;
                            editCRName.Visible = true;
                            break;
                    }

                    mode = value;
                    RefreshCustomRegionList();
                }
            }
        }
        private ViewMode mode;

        public bool IsElliptical
        {
            get => isElliptical;
            set
            {
                if (isElliptical != value)
                {
                    btnCRTypeSwitcher.ImageOptions.Image = value
                        ? Resources.miniCREllipce
                        : Resources.miniCRRectang;
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
                        return cbCRSelector.Text.Trim();
                    default:
                        return String.Empty;
                }
            }
        }

        public void Show(ViewMode mode, IWin32Window owner = null, Point? location = null)
        {
            if(mode == ViewMode.Undefined)
                return;
            Mode = mode;

            if (!Visible)
            {
                if (location != null)
                    Location = (Point)location;

                if(owner != null)
                    base.Show(owner);
                base.Show();
            }
        }

        protected new void Show() => base.Show();
        protected new void Show(IWin32Window owner) => base.Show(owner);

        public delegate void CustomRegionToolEvent(CustomRegionToolForm sender, EventArgs e, object value = null);

        /// <summary>
        /// Изменить тип границы CustomRegion'a
        /// </summary>
        public event CustomRegionToolEvent ChangeCustomRegionType;

        /// <summary>
        /// Принять изменения CustomRegion'a
        /// </summary>
        public event CustomRegionToolEvent AcceptChanges;

        /// <summary>
        /// Отменить изменения CustomRegion'a
        /// </summary>
        public event CustomRegionToolEvent CancelChanges;

        /// <summary>
        /// Отменить изменения CustomRegion'a
        /// </summary>
        public event CustomRegionToolEvent SelectedCustomRegionChanged;

        public CustomRegion SelectedCustomRegion =>
            mode == ViewMode.Edit 
                ? cbCRSelector.SelectedItem as CustomRegion
                : null;

        private void handler_FormClosed(object sender, FormClosedEventArgs e)
        {
#if false
            ChangeCustomRegionType = null;
            AcceptChanges = null;
            CancelChanges = null;
            SelectedCustomRegionChanged = null; 
#endif
            cbCRSelector.DataSource = null;
            editCRName.Text = string.Empty;
        }

        private void handler_ChangeCRShapeType(object sender, EventArgs e)
        {
            IsElliptical = !IsElliptical;
            ChangeCustomRegionType?.Invoke(this, e, IsElliptical);
        }

        private void handler_Accept(object sender, EventArgs e)
        {
            AcceptChanges?.Invoke(this, e);
        }

        private void handler_Cancel(object sender, EventArgs e)
        {
            CancelChanges?.Invoke(this, e);
        }

        private void handler_SelectedCustomRegionChanged(object sender, EventArgs e)
        {
            SelectedCustomRegionChanged?.Invoke(this, e);
        }

        private void handler_VisibleChanged(object sender, EventArgs e)
        {
            if (!Visible)
            {
                Mode = ViewMode.Undefined;
            }
        }

        public void RefreshCustomRegionList()
        {
            cbCRSelector.BeginUpdate();
            if (mode == ViewMode.Edit)
            {
                var selectedItem = cbCRSelector.SelectedItem;
                cbCRSelector.DataSource = null;
                cbCRSelector.DisplayMember = nameof(CustomRegion.Name);
                cbCRSelector.DataSource = Astral.Quester.API.CurrentProfile.CustomRegions;
                cbCRSelector.SelectedItem = selectedItem;
            }
            else
            {
                cbCRSelector.DataSource = null;
            }
            cbCRSelector.EndUpdate();
        }
    }
}
