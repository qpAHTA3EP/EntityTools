using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using ACTP0Tools.Classes.Quester;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;
using EntityTools.Enums;
using EntityTools.Patches.Mapper;
using EntityTools.Tools.CustomRegions;

namespace EntityTools.Forms
{
    public partial class CustomRegionCollectionEditorForm : XtraForm
    {
        //TODO: Добавить кнопки: отметить все, снять все отметки, отметить все свободные, снять все отмеченные

        private static CustomRegionCollectionEditorForm editor;
        private QuesterProfileProxy profile;
        private CustomRegionCollection customRegionCollection;

        public CustomRegionCollectionEditorForm()
        {
            InitializeComponent();

            var culture = System.Globalization.CultureInfo.CurrentCulture;
            if (culture.TwoLetterISOLanguageName != "ru")
            {
                textUnion.Text = "Select several regions, which will form MERGE  of the regions. The character should be within any marked region, as well as within the INTERSECTION area (if preset). At this time, it is forbidden to be withing the EXCLUSION area.";
                textIntersection.Text = "Select several regions, which will form INTERSECTION of the regions. The character should be within every marked region, as well as within the UNION region (if preset).At this time, it is forbidden to be withing the EXCLUSION area";
                textExclusion.Text = "Select several regions, which will be EXCLUDED from the assessed area. The character should be OUTSIDE EVERY marked region";
            }
        }


        public static bool RequestUser(ref CustomRegionCollection crCollection)
        {
            if (editor == null || editor.IsDisposed)
                editor = new CustomRegionCollectionEditorForm();
            editor.tabPane.SelectedPage = editor.tabUnion;

            // Копирование исходной коллекции для возможности отказа от внесения изменений
            editor.customRegionCollection = crCollection;
            editor.profile = crCollection.DesignContext;
            
            if (editor.ShowDialog() == DialogResult.OK)
            {
                crCollection = MakeNewCustomRegionCollection();
                return true;
            }
            return false;
        }

        private static CustomRegionCollection MakeNewCustomRegionCollection()
        {
            var currentPage = editor.tabPane.SelectedPage;
            var inclusion = InclusionType.Ignore;
            if (currentPage == editor.tabUnion)
                inclusion = InclusionType.Union;
            else if (currentPage == editor.tabIntersection)
                inclusion = InclusionType.Intersection;
            if (currentPage == editor.tabExclusion)
                inclusion = InclusionType.Exclusion;

            var newCrCollection = new CustomRegionCollection();

            for (int i = 0; i < editor.crList.ItemCount; i++)
            {
                var item = editor.crList.Items[i];
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
                    }

                    if (crEntry.Inclusion != InclusionType.Ignore
                        // Удаление дубликатов из списка добавляемых CustomRegion'ов
                        && !newCrCollection.Contains(crEntry))
                    {
                        newCrCollection.Add(crEntry);
                    }
                }
            }

            editor.crList.Items.Clear();
            
            return newCrCollection;
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
            var currentPage = editor.tabPane.SelectedPage;
            InclusionType inclusion = InclusionType.Ignore;
            if (currentPage == editor.tabUnion)
                inclusion = InclusionType.Union;
            else if (currentPage == editor.tabIntersection)
                inclusion = InclusionType.Intersection;
            else if (currentPage == editor.tabExclusion)
                inclusion = InclusionType.Exclusion;

            editor.crList.Items.Clear();
            var customRegions = profile.CustomRegions;
            foreach (var cr in customRegions)
            {
                if (customRegionCollection.TryGetValue(cr.Name, out CustomRegionEntry crEntry))
                {
                    var checkState = CheckState.Unchecked;
                    if (crEntry.Inclusion == inclusion)
                        checkState = CheckState.Checked;
                    else if (crEntry.Inclusion != InclusionType.Ignore)
                        checkState = CheckState.Indeterminate;
                    editor.crList.Items.Add(crEntry.Clone(), checkState);
                }
                // Удаление дубликатов из списка отображаемых CustomRegion'ов
                else if (editor.crList.Items.FirstOrDefault(item => ((CustomRegionEntry)item.Value).Name == cr.Name) is null)
                    editor.crList.Items.Add(new CustomRegionEntry(cr.Name, InclusionType.Ignore), CheckState.Unchecked);
            }
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

        private void handler_Mapper(object sender, EventArgs e)
        {
            ComplexPatch_Mapper.OpenMapper(profile);
        }

        private void handler_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            Process.Start(@"https://qpahta3ep.github.io/EntityToolsDocs/General/CustomRegionSet-RU.html");
        }
    }
}