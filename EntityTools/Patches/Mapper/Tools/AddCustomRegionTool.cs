using AStar;
using Astral.Quester;
using Astral.Quester.Classes;
using DevExpress.XtraEditors;
using MyNW.Classes;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Infrastructure;
using Infrastructure.Quester;

namespace EntityTools.Patches.Mapper.Tools
{
    /// <summary>
    /// Инструмент для удаления вершин
    /// </summary>
    public sealed class AddCustomRegionTool : CustomRegionToolBase
    {
        /// <summary>
        /// Функция вызываемая после применения изменений к CustomRegion'y, где<br/>
        /// <see cref="CustomRegion">Модифицированный CustomRegion</see><br/>
        /// <see cref="IMapperTool">Объект для отката изменений</see>
        /// </summary>
        private readonly Action<CustomRegion, IMapperTool> onComplete;

        public BindingList<CustomRegion> CustomRegions => _profile.CustomRegions;
        private readonly BaseQuesterProfileProxy _profile;

        public AddCustomRegionTool(MapperFormExt mapperForm = null, Action<CustomRegion, IMapperTool> onCompleteCallback = null)
        {
            onComplete = onCompleteCallback;
            _profile = mapperForm?.Profile ?? AstralAccessors.Quester.Core.CurrentProfile;
            toolForm.Show(CustomRegionToolForm.ViewMode.Add, mapperForm);
            toolForm.SetCustomRegionSize(0, 0, 0, 0);
        }

        /// <summary>
        /// Режим редактирования
        /// </summary>
        public override MapperEditMode EditMode => MapperEditMode.AddCustomRegion;

        public override bool HandleCustomDraw => !(leftX == 0 && topY == 0);

        /// <summary>
        /// Обратка нажатия кнопки клавиатуры
        /// </summary>
        public override void OnKeyUp(IGraph graph, NodeSelectTool nodes, KeyEventArgs e, double worldMouseX, double worldMouseY, out IMapperTool undo)
        {
            undo = null;
            switch (e.KeyCode)
            {
                // Сбрасываем режим трансформации
                case Keys.Escape when transformMode != RegionTransformMode.None && transformMode != RegionTransformMode.Disabled:
                    transformMode = RegionTransformMode.None;
                    break;
                case Keys.Escape:
                    // Сбрасываем выделение
                    leftX = 0;
                    topY = 0;
                    rightX = 0;
                    bottomY = 0;
                    break;
                // Иммитируем нажатие правой кнопки мыши
                case Keys.Enter:
                    OnMouseClick(graph, nodes, new MapperMouseEventArgs(MouseButtons.Right, 1, worldMouseX, worldMouseY), out undo);
                    break;
            }
            toolForm.SetCustomRegionSize(X, Y, Widths, Height);
        }

        // Наследуем реализацию
        //public bool HandleMouseClick => true;
        public override void OnMouseClick(IGraph graph, NodeSelectTool nodes, MapperMouseEventArgs e, out IMapperTool undo)
        {
            undo = null;
            if (e.Button == MouseButtons.Right)
            {
                if(leftX == 0 && topY == 0)
                {
                    // Отмечаем начальную точку CustomRegion'a
                    leftX = Math.Round(e.X);
                    topY = Math.Round(e.Y);
                    transformMode = RegionTransformMode.None;
                }
                else if(rightX == 0 && bottomY == 0)
                {
                    // Отмечаем конечную точку CustomRegion'a
                    MapperHelper.FixRange(leftX, Math.Round(e.X), out leftX, out rightX);
                    MapperHelper.FixRange(topY, Math.Round(e.Y), out bottomY, out topY);
                    transformMode = RegionTransformMode.None;
                }
                else if (transformMode == RegionTransformMode.None)// && transformMode != RegionTransformMode.Disabled)
                {
                    // проверяем выбор якоря и режима трансформации
                    double width = Math.Abs(rightX - leftX),
                           height = Math.Abs(bottomY - topY),
                           anchorSize = MapperHelper.AnchorWorldSize(width, height);

                    MapperHelper.SelectAnchor(leftX, topY, rightX, bottomY, e.X, e.Y, anchorSize, out transformMode);
                }
                else
                {
                    // преобразование CustomRegion'a
                    MapperHelper.TransformRegion(ref leftX, ref topY, ref rightX, ref bottomY, Math.Round(e.X), Math.Round(e.Y), transformMode);
                    transformMode = RegionTransformMode.None;
                }
            }
            toolForm.SetCustomRegionSize(X, Y, Widths, Height);
        }

        /// <summary>
        /// Указывает, что инструмент был применен
        /// </summary>
        public override bool Applied => customRegion != null;

        /// <summary>
        /// Откат изменений, внесенных инструментом
        /// </summary>
        public override void Undo()
        {
            if (customRegion != null)
            {
                API.CurrentProfile.CustomRegions.Remove(customRegion);
                customRegion = null;
            }
        }



        #region Взаимодействие с CustomRegionToolForm
        protected override void OnSelectedCustomRegionChanged(CustomRegionToolForm sender, EventArgs e, object value){}

        protected override void OnCancelChanges(CustomRegionToolForm sender, EventArgs e, object value)
        {
            leftX = 0;
            topY = 0;
            rightX = 0;
            bottomY = 0;
            toolForm.Hide();
            onComplete?.Invoke(customRegion, this);
        }

        protected override void OnAcceptChanges(CustomRegionToolForm sender, EventArgs e, object value)
        {
            string name = sender.CustomRegionName;
            if (string.IsNullOrEmpty(name))
            {
                XtraMessageBox.Show("Empty CustomRegion name are not allowed!", "Naming error !",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var crList = CustomRegions;
            if (crList.Count > 0
                && crList.FirstOrDefault(cr => cr.Name == name) != null)
            {
                XtraMessageBox.Show($"There are exists another CustomRegion named '{name}'!,\n" +
                                    $"Set different name.", "Naming error !",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!IsReady)
            {
                XtraMessageBox.Show("Coordinates  of the CustomRegion is not valid!", "Size Error!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MapperHelper.FixRange(leftX, rightX, out leftX, out rightX);
            // Ось Oy в игровых координатах инвертирована по сравнению с координатами экрана windows (MapPicture)
            // Поэтому координата верхнего правого угла CustomRegion'e должна иметь максимальную Y
            MapperHelper.FixRange(topY, bottomY, out bottomY, out topY);
            int width = (int)Math.Round(rightX - leftX),
                // в Astra'e высота CustomRegion'a должна быть отрицательной в связи с инверсией оси
                height = (int)Math.Round(bottomY - topY);


            if (customRegion is null)
            {
                customRegion = new CustomRegion
                {
                    Position = new Vector3((float)Math.Round(leftX), (float)Math.Round(topY), 0),
                    Eliptic = IsElliptical,
                    Height = height,
                    Width = width,
                    Name = name
                };
            }
            else 
            {
                customRegion.Name = name;
                customRegion.Position.X = (float)Math.Round(leftX);
                customRegion.Position.Y = (float)Math.Round(topY);
                customRegion.Eliptic = IsElliptical;
                customRegion.Height = height;
                customRegion.Width = width;
            }

            crList.Add(customRegion);

            leftX = 0;
            topY = 0;
            rightX = 0;
            bottomY = 0;
            toolForm.Hide();
            onComplete?.Invoke(customRegion, this);
        } 
        #endregion
    }
}