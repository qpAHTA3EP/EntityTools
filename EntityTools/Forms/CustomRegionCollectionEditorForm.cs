using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;
using EntityTools.Enums;
using EntityTools.Tools.CustomRegions;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace EntityTools.Forms
{
    public partial class CustomRegionCollectionEditorForm : XtraForm
    {
        //TODO: Добавить кнопки: отметить все, снять все отметки, отметить все свободные, снять все отмеченные

        private static CustomRegionCollectionEditorForm @this = null;

#if DataSource
        /// <summary>
        /// Список регионов
        /// </summary>
        BindingList<CustomRegionEntry> customRegions = new BindingList<CustomRegionEntry>();

#endif
        Action fillList;

        public CustomRegionCollectionEditorForm()
        {
            InitializeComponent();

            var culture = System.Globalization.CultureInfo.CurrentCulture;
            if(culture.TwoLetterISOLanguageName != "ru")
            {
                textUnion.Text = "Select several regions, which will form MERGE  of the regions. The character should be within any marked region, as well as within the INTERSECTION area (if preset). At this time, it is forbidden to be withing the EXCLUSION area.";
                textIntersection.Text = "Select several regions, which will form INTERSECTION of the regions. The character should be within every marked region, as well as within the UNION region (if preset).At this time, it is forbidden to be withing the EXCLUSION area";
                textExclusion.Text = "Select several regions, which will be EXCLUDED from the assessed area. The character should be OUTSIDE EVERY marked region";
            }
        }


        public static bool GUIRequiest(ref CustomRegionCollection crCollection)
        {
            if (@this == null || @this.IsDisposed)
                @this = new CustomRegionCollectionEditorForm();
            @this.tabPane.SelectedPage = @this.tabUnion;

            // Копирование исходной коллекции для возможности отказа от внесения изменений
            var originalCrCollection = crCollection;
            if (crCollection?.Count > 0)
                @this.fillList = () =>
                {
                    var currentPage = @this.tabPane.SelectedPage;
                    InclusionType inclusion = InclusionType.Ignore;
                    if (currentPage == @this.tabUnion)
                        inclusion = InclusionType.Union;
                    else if (currentPage == @this.tabIntersection)
                        inclusion = InclusionType.Intersection;
                    else if (currentPage == @this.tabExclusion)
                        inclusion = InclusionType.Exclusion;

                    @this.crList.Items.Clear();

                    foreach (var cr in Astral.Quester.API.CurrentProfile.CustomRegions)
                    {
                        if (originalCrCollection.TryGetValue(cr.Name, out CustomRegionEntry crEntry))
                        {
                            var checkState = CheckState.Unchecked;
                            if (crEntry.Inclusion == inclusion)
                                checkState = CheckState.Checked;
                            else if (crEntry.Inclusion != InclusionType.Ignore)
                                checkState = CheckState.Indeterminate;
                            @this.crList.Items.Add(crEntry.Clone(), checkState);
                        }
                        // Удаление дубликатов из списка отображаемых CustomRegion'ов
                        else if(@this.crList.Items.FirstOrDefault(item => ((CustomRegionEntry)item.Value).Name == cr.Name) is null)
                            @this.crList.Items.Add(new CustomRegionEntry(cr.Name, InclusionType.Ignore), CheckState.Unchecked);
                    }
                };
            else @this.fillList = () =>
            {
                @this.crList.Items.Clear();
                foreach (var cr in Astral.Quester.API.CurrentProfile.CustomRegions)
                {
                    @this.crList.Items.Add(new CustomRegionEntry(cr.Name, InclusionType.Ignore), CheckState.Unchecked);
                }
            };

            if (@this.ShowDialog() == DialogResult.OK)
            {
                var currentPage = @this.tabPane.SelectedPage;
                InclusionType inclusion = InclusionType.Ignore;
                if (currentPage == @this.tabUnion)
                    inclusion = InclusionType.Union;
                else if (currentPage == @this.tabIntersection)
                    inclusion = InclusionType.Intersection;
                if (currentPage == @this.tabExclusion)
                    inclusion = InclusionType.Exclusion;

                CustomRegionCollection newCrCollection = new CustomRegionCollection();

                for (int i = 0; i < @this.crList.ItemCount; i++)
                {
                    var item = @this.crList.Items[i];
                    if (item.Value is CustomRegionEntry crEntry)
                    {
                        // сохраняем тип включения
                        switch (item.CheckState)
                        {
                            case CheckState.Checked:
                                crEntry.Inclusion = inclusion;
                                break;
                            case CheckState.Unchecked:
                                crEntry.Inclusion = InclusionType.Ignore;
                                break;
#if false
                            case CheckState.Indeterminate:
                                break; 
#endif
                        }

                        if (crEntry.Inclusion != InclusionType.Ignore
                            // Удаление дубликатов из списка добавляемых CustomRegion'ов
                            && !newCrCollection.Contains(crEntry))
                        {
                            newCrCollection.Add(crEntry);
                        }
                    }
                } 

                @this.crList.Items.Clear();
                crCollection = newCrCollection;
                return true;
            }
            return false;
        }

        private void handler_Select(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Сборос списка фильтра к исходному состоянию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handler_Reload(object sender, EventArgs e)
        {
            fillList?.Invoke();
        }

        private void handler_SelectedPageChanging(object sender, SelectedPageChangingEventArgs e)
        {
            var pageOld = e.OldPage;
            var pageNew = e.Page;
            InclusionType inclusionOld = InclusionType.Ignore;
            InclusionType inclusionNew = InclusionType.Ignore;
            if (pageOld == tabUnion)
                inclusionOld = InclusionType.Union;
            else if (pageOld == tabIntersection)
                inclusionOld = InclusionType.Intersection;
            if (pageOld == tabExclusion)
                inclusionOld = InclusionType.Exclusion;
            if (pageNew == tabUnion)
                inclusionNew = InclusionType.Union;
            else if (pageNew == tabIntersection)
                inclusionNew = InclusionType.Intersection;
            if (pageNew == tabExclusion)
                inclusionNew = InclusionType.Exclusion;

            for (int i = 0; i < crList.ItemCount; i++)
            {
                var item = crList.Items[i];
                if (item.Value is CustomRegionEntry crEntry)
                {
                    // сохраняем тип включения, соответствовавшего старой inclusionOld
                    // и отображаем тип влкючения, соответсвующего inclusionNew
                    switch (item.CheckState)
                    {
                        case CheckState.Checked:
                            crEntry.Inclusion = inclusionOld;
                            item.CheckState = CheckState.Indeterminate;
                            break;
                        case CheckState.Unchecked:
                            crEntry.Inclusion = InclusionType.Ignore;
                            //item.CheckState = CheckState.Unchecked;
                            break;
                        case CheckState.Indeterminate:
                            if (crEntry.Inclusion == inclusionNew)
                                item.CheckState = CheckState.Checked;
                            break;
                    }
                }
                else item.CheckState = CheckState.Indeterminate;
            } 
        }

        private void handler_FormShown(object sender, EventArgs e)
        {
            fillList?.Invoke();
        }

        private void handler_Mapper(object sender, EventArgs e)
        {
            Astral.Quester.Forms.MapperForm.Open();
        }

        private void handler_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            Process.Start(@"https://qpahta3ep.github.io/EntityToolsDocs/General/CustomRegionSet-RU.html");
        }
    }
}