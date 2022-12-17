using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using ACTP0Tools;
using ACTP0Tools.Classes.Quester;
using AStar;
using Astral.Quester.Classes;
using DevExpress.XtraEditors;
// ReSharper disable InconsistentNaming

namespace EntityTools.Patches.Mapper.Tools
{
    /// <summary>
    /// Инструмент для удаления вершин
    /// </summary>
    public sealed class EditCustomRegionTool : CustomRegionToolBase
    {
        /// <summary>
        /// Функция вызываемая после применения изменений к CustomRegion'y, где<br/>
        /// <see cref="CustomRegion">Модифицированный CustomRegion</see><br/>
        /// <see cref="IMapperTool">Объект для отката изменений</see>
        /// </summary>
        private readonly Action<CustomRegion, IMapperTool> onComplete;

        public BindingList<CustomRegion> CustomRegions => _profile.CustomRegions;
        private readonly BaseQuesterProfileProxy _profile;

        public EditCustomRegionTool(CustomRegion cr = null, MapperFormExt mapperForm = null, Action<CustomRegion, IMapperTool> onCompleteCallback = null)
        {
            onComplete = onCompleteCallback;
            AttachTo(cr);
            _profile = mapperForm?.Profile ?? AstralAccessors.Quester.Core.CurrentProfile;
            toolForm.Mode = CustomRegionToolForm.ViewMode.Edit;
            toolForm.Show(CustomRegionToolForm.ViewMode.Edit, mapperForm, cr);
        }

        private void AttachTo(CustomRegion cr)
        {
            customRegion = cr;
            if (cr != null)
            {
                crName = cr.Name;
                crElliptic = IsElliptical = cr.Eliptic;
                crX = (float)(leftX = cr.Position.X);
                crY = (float)(topY = cr.Position.Y);
                crWidth = cr.Width;
                rightX = leftX + crWidth;
                crHeight = cr.Height;
                bottomY = topY + crHeight; 
            }
            else
            {
                crName = string.Empty;
                crElliptic = IsElliptical = false;
                crX = 0;
                crY = 0;
                crWidth = 0;
                rightX = 0;
                crHeight = 0;
                bottomY = 0;
            }
            transformMode = RegionTransformMode.None;
        }

        #region данные
        #region исходные данные CustomRegion'a для отката изменений
        private string crName;
        private float crX, crY;
        private int crWidth, crHeight;
        private bool crElliptic;
        #endregion
        #endregion

        /// <summary>
        /// Введенные данные корректны
        /// </summary>
        public override bool IsReady
        {
            get
            {
                MapperHelper.FixRange(leftX, rightX, out double minX, out double maxX);
                MapperHelper.FixRange(topY, bottomY, out double minY, out double maxY);
                return customRegion != null
                       && (Math.Abs(crX - (float)minX) >= 1
                           || Math.Abs(crY - (float)maxY) >= 1
                           || crElliptic != IsElliptical
                           || Math.Abs(crWidth - (maxX - minX)) >= 1
                           || Math.Abs(crHeight - (minY - maxY)) >= 1
                           || CustomRegionName != crName);
            }
        }

        /// <summary>
        /// Режим редактирования
        /// </summary>
        public override MapperEditMode EditMode => MapperEditMode.EditCustomRegion;

        /// <summary>
        /// Обратка нажатия кнопки клавиатуры
        /// </summary>
        public override void OnKeyUp(IGraph graph, NodeSelectTool nodes, KeyEventArgs e, double worldMouseX, double worldMouseY, out IMapperTool undo)
        {
            undo = null;
            switch (e.KeyCode)
            {
                // Сбрасываем режим трансформации
                case Keys.Escape when transformMode != RegionTransformMode.None:
                    transformMode = RegionTransformMode.None;
                    break;
                case Keys.Escape:
                    // Сбрасываем выделение
                    if (customRegion != null)
                    {
                        IsElliptical = customRegion.Eliptic;
                        leftX = customRegion.Position.X;
                        topY = customRegion.Position.Y;
                        rightX = leftX + customRegion.Width;
                        bottomY = topY + customRegion.Height; 
                    }
                    break;
                // Имитируем нажатие правой кнопки мыши
                case Keys.Enter:
                    OnMouseClick(graph, nodes, new MapperMouseEventArgs(MouseButtons.Right, 1, worldMouseX, worldMouseY), out undo);
                    break;
            }
            toolForm.SetCustomRegionSize(X, Y, Widths, Height);
        }

        //public bool HandleMouseClick => true;
        public override void OnMouseClick(IGraph graph, NodeSelectTool nodes, MapperMouseEventArgs e, out IMapperTool undo)
        {
            undo = null;
            if (e.Button != MouseButtons.Right) return;

            if (transformMode == RegionTransformMode.None)
            {
                // проверяем выбор якоря и режима трансформации
                double width = Math.Abs(rightX - leftX),
                       height = Math.Abs(topY - bottomY),
                       anchorSize = MapperHelper.AnchorWorldSize(width, height);

                MapperHelper.SelectAnchor(leftX, topY, rightX, bottomY, e.X, e.Y, anchorSize, out transformMode);
            }
            else
            {
                // преобразование CustomRegion'a
                MapperHelper.TransformRegion(ref leftX, ref topY, ref rightX, ref bottomY, Math.Round(e.X), Math.Round(e.Y), transformMode);
                transformMode = RegionTransformMode.None;
            }
            toolForm.SetCustomRegionSize(X, Y, Widths, Height);
        }


        public override bool Applied => applied;
        private bool applied;

        /// <summary>
        /// Откат изменений, внесенных инструментом
        /// </summary>
        public override void Undo()
        {
            if (customRegion != null)
            {
                customRegion.Name = crName;
                customRegion.Position.X = crX;
                customRegion.Position.Y = crY;
                customRegion.Eliptic = IsElliptical;
                customRegion.Height = crHeight;
                customRegion.Width = crWidth;

                applied = false;
                customRegion = null;
            }
        }

        #region MyRegion
        protected override void OnCancelChanges(CustomRegionToolForm sender, EventArgs e, object value)
        {
            if (IsReady)
            {
                IsElliptical = customRegion.Eliptic;
                leftX = customRegion.Position.X;
                topY = customRegion.Position.Y;
                rightX = leftX + customRegion.Width;
                bottomY = topY + customRegion.Height;
                sender.SetCustomRegionSize(X, Y, Widths, Height); 
            }
            else
            {
                customRegion = null;
                crName = String.Empty;
                crX = 0;
                crY = 0;
                crWidth = 0;
                crHeight = 0;
                crElliptic = false;
                leftX = 0;
                topY = 0;
                rightX = 0;
                bottomY = 0;
                applied = false;

                toolForm.Hide();
                onComplete?.Invoke(null, null);
            }
        }

        protected override void OnAcceptChanges(CustomRegionToolForm sender, EventArgs e, object value)
        {
            if (!IsReady)
            {
                return;
            }
            var name = toolForm.CustomRegionName;

            if (string.IsNullOrEmpty(name))
            {
                XtraMessageBox.Show("Empty CustomRegion name are not allowed!", "Naming error !",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var crList = CustomRegions;

            if (crList.Count > 0
                && crList.FirstOrDefault(cr => !ReferenceEquals(customRegion, cr) && cr.Name == name) != null)
            {
                XtraMessageBox.Show($"There are exists another CustomRegion named '{name}'!,\n" +
                                    $"Set different name.", "Naming error !",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MapperHelper.FixRange(leftX, rightX, out leftX, out rightX);
            // Ось Oy в игровых координатах инвертирована по сравнению с координатами экрана windows (MapPicture)
            // Поэтому координата верхнего правого угла CustomRegion'e должна иметь максимальную Y
            MapperHelper.FixRange(topY, bottomY, out bottomY, out topY);
            int width = (int)Math.Round(rightX - leftX),
                // в Astral'e высота CustomRegion'a должна быть отрицательной в связи с инверсией оси
                height = (int)Math.Round(bottomY - topY);

            customRegion.Name = name;

            customRegion.Position.X = (float)Math.Round(leftX);
            customRegion.Position.Y = (float)Math.Round(topY);
            customRegion.Eliptic = IsElliptical;
            customRegion.Height = height;
            customRegion.Width = width;

            // Конструирование объекта для отката изменений
            var undo = new EditCustomRegionTool
            {
                customRegion = customRegion,
                crName = crName,
                crX = crX, 
                crY = crY,
                crWidth = crWidth,
                crHeight = crHeight,
                crElliptic = crElliptic,
                leftX = leftX,
                topY = topY,
                rightX = rightX,
                bottomY = bottomY,
                applied = true
            };

            crName = customRegion.Name;
            var pos = customRegion.Position;
            crX = pos.X;
            crY = pos.Y;
            crWidth = customRegion.Width;
            crHeight = customRegion.Height;
            crElliptic = customRegion.Eliptic;

            toolForm.RefreshCustomRegionList(customRegion);
            toolForm.IsElliptical = customRegion.Eliptic;

            onComplete?.Invoke(customRegion, undo);
        }

        protected override void OnSelectedCustomRegionChanged(CustomRegionToolForm sender, EventArgs e, object value)
        {
            var selectedCR = sender.SelectedCustomRegion;
            if (selectedCR == customRegion) return;

            if (IsReady)
            {
                var name = sender.CustomRegionName;
                if (string.IsNullOrEmpty(name))
                {
                    XtraMessageBox.Show("Empty CustomRegion name are not allowed!", "Naming error !",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var crList = CustomRegions;
                if (crList.Count > 0
                    && crList.FirstOrDefault(cr => !ReferenceEquals(customRegion, cr) && cr.Name == name) != null)
                {
                    XtraMessageBox.Show($"There are exists another CustomRegion named '{name}'!,\n" +
                                        "Set different name.", "Naming error !",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (XtraMessageBox.Show($"Changes of the '{customRegion.Name}' can be lost!\n" +
                                        "Press 'Yes' to save changes or 'No' to proceed", "",
                                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    OnAcceptChanges(sender, e, null);

                    //Обновление списка CustomRegion'ов в Quester-редакторе
                    AstralAccessors.Quester.Forms.Editor.EditorForm?.RefreshRegions();
                    sender.RefreshCustomRegionList();
                }
            }
            AttachTo(selectedCR);
        }
        #endregion
    }
}