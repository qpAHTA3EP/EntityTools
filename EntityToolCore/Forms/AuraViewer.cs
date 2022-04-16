using Astral.Classes.ItemFilter;
using Astral.Logic.NW;
using DevExpress.XtraEditors;
using EntityTools.Enums;
using EntityTools.Tools.Export;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EntityCore.Forms
{
    public enum SelectedAuraSource
    {
        Player,
        Target,
        Entity
    }

    public partial class AuraViewer : XtraForm //*/ Form
    {
        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        private Task backgroundTaskFillAuraList;

        private Entity entityCache;
        private Entity UnitRef
        {
            get
            {
                if (entityCache != null && entityCache.IsValid)
                    return entityCache;

                //if (Selector.SelectedItem == null)
                //    Selector.SelectedItem = SelectedAuraSource.Player;

                if (Selector.SelectedItem is SelectedAuraSource selectedAuraSource)
                {
                    switch (selectedAuraSource)
                    {
                        case SelectedAuraSource.Player:
                            unitRefName.Text = EntityManager.LocalPlayer.InternalName;
                            entityCache = EntityManager.LocalPlayer;
                            break;
                        case SelectedAuraSource.Target:
                            while (TargetSelectForm.GUIRequest("Get NPC", "Target NPC and press ok.") == DialogResult.OK)
                            {
                                Entity betterEntityToInteract = Interact.GetBetterEntityToInteract();
                                if (betterEntityToInteract != null && betterEntityToInteract.IsValid)
                                {
                                    entityCache = betterEntityToInteract;
                                    unitRefName.Text = $@"{entityCache.Name} [{entityCache.InternalName}]";
                                    break;
                                }
                            }
                            break;
                        case SelectedAuraSource.Entity:
                            string pattern = string.Empty;
                            ItemFilterStringType strMatch = ItemFilterStringType.Simple;
                            EntityNameType nameType = EntityNameType.InternalName;
                            entityCache = EntityViewer.GUIRequest(ref pattern, ref strMatch, ref nameType);

                            if (entityCache != null)
                            {
                                if(!string.IsNullOrEmpty(pattern))
                                    unitRefName.Text = pattern;
                                else if (!string.IsNullOrEmpty(entityCache.InternalName))
                                    unitRefName.Text = entityCache.InternalName;
                                else if (!string.IsNullOrEmpty(entityCache.Name))
                                    unitRefName.Text = entityCache.Name;
                                else unitRefName.Text = entityCache.NameUntranslated;
                            }
                            break;
                    }

                    if (entityCache != null)
                        return entityCache;
                }
                Selector.SelectedItem = null;
                unitRefName.Text = @"-";
                return null;
            }
        }

        /// <summary>
        /// Полная коллекция аур, наложенных на персонажа
        /// </summary>
        private readonly HashSet<AuraInfo> auraCollection = new HashSet<AuraInfo>();

        public AuraViewer()
        {
            InitializeComponent();
        }

        public static string GUIRequest()
        {
            var @this = new AuraViewer
            {
                Selector = {SelectedItem = SelectedAuraSource.Player},
                NameFilter = {Text = string.Empty},
                InternalNameFilter = {Text = string.Empty},
                ckbAuraInspectionMode = {Checked = false}
            };

            if (@this.ShowDialog() == DialogResult.OK)
                if (@this.Auras.SelectedItem is AuraInfo aura)
                    return aura.InternalName;
            
            return string.Empty;
        }

        private void FillAuraList(Entity entity)
        {
            auraCollection.Clear();
            Auras.Items.Clear();
            Description.Text = string.Empty;

            if (entity != null && entity.IsValid)
            {
                // Конструирование предиката
                var predicate = GetAuraPredicate();

                Auras.BeginUpdate();
                foreach (var mod in entity.Character.Mods)
                {
                    var auraInfo = new AuraInfo(mod.PowerDef);
                    if (auraCollection.Add(auraInfo)
                        && predicate(auraInfo))
                    {
                        Auras.Items.Add(auraInfo);
                    }
                }
                Auras.EndUpdate();
            }
        }

        private void BackgroundFillAuraList(Entity entity, CancellationToken token)
        {
            auraCollection.Clear();
            Auras.BeginUpdate();
            Auras.Items.Clear();
            Auras.EndUpdate();
            Description.Text = string.Empty;

            if (entity != null && entity.IsValid)
            {
                // Кэшируем все ауры, которые наложены на character, но не должны отображаться { Visible = false }
                lock (auraCollection)
                {
                    foreach (var mod in entity.Character.Mods)
                    {
                        var auraInfo = new AuraInfo(mod.PowerDef) { Visible = false };
                        auraCollection.Add(auraInfo);
                    } 
                }

                Character character;
                while (!token.IsCancellationRequested
                       && (character = entity.Character).IsValid)
                {
                    // Конструирование предиката
                    var predicate = GetAuraPredicate();

                    // сканирование аур, которые наложены на character
                    // и отображаем новые в списке
                    Auras.BeginUpdate();
                    lock (auraCollection)
                    {
                        foreach (var mod in character.Mods)
                        {
                            var auraInfo = new AuraInfo(mod.PowerDef);
                            if (auraCollection.Add(auraInfo)
                                && predicate(auraInfo))
                            {
                                Auras.Items.Add(auraInfo);
                            }
                        } 
                    }
                    Auras.EndUpdate();
                    Thread.Sleep(100);
                }
            }
        }

        private Predicate<AuraInfo> GetAuraPredicate()
        {
            if (filteringPredicate is null)
            {
                var nameFilter = NameFilter.Text;
                var internalNameFilter = InternalNameFilter.Text;

                Predicate<AuraInfo> predicate;
                if (string.IsNullOrEmpty(nameFilter))
                {
                    if (string.IsNullOrEmpty(internalNameFilter))
                        // Фильтр отсутствует
                        predicate = aura => aura.IsValid;
                    else predicate = aura => aura.InternalName?.IndexOf(internalNameFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                }
                else 
                {
                    if (string.IsNullOrEmpty(internalNameFilter))
                        predicate = aura => aura.IsValid
                                            && aura.DisplayName?.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                    else predicate = aura => aura.DisplayName?.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0
                                             && aura.InternalName?.IndexOf(internalNameFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                }
                filteringPredicate = predicate;
            }

            return filteringPredicate;
        }

        private Predicate<AuraInfo> filteringPredicate;

        #region Обработчики
        private void handler_SelectAura(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void handler_LoadForm(object sender, EventArgs e)
        {
            auraCollection.Clear();
            Selector.Properties.Items.Clear();
            Selector.Properties.Items.AddRange(Enum.GetValues(typeof(SelectedAuraSource)));
        }

        private void handler_TargetSelect(object sender, EventArgs e)
        {
            ckbAuraInspectionMode.Checked = false;
            entityCache = null;

            var entity = UnitRef;
            if (entity != null)
            {
                FillAuraList(entity);
            }
            else
            {
                auraCollection.Clear();
                Auras.Items.Clear();
            }
        }

        private void handler_Reload(object sender, EventArgs e)
        {
            if (ckbAuraInspectionMode.Checked)
                handler_AuraInspectionModeChanged(sender, e);
            else FillAuraList(UnitRef);
        }

        private void handler_SelectedAuraChanged(object sender, EventArgs e)
        {
            if(Auras.SelectedItem is AuraInfo aura)
            {
                Description.Text = aura.Description;
            }
            else Description.Text = string.Empty;
        }

        private void handler_AuraInspectionModeChanged(object sender, EventArgs e)
        {
            if (backgroundTaskFillAuraList?.Status == TaskStatus.Running)
                tokenSource?.Cancel();

            if (ckbAuraInspectionMode.Checked)
            {
                tokenSource = new CancellationTokenSource();
                backgroundTaskFillAuraList = Task.Factory.StartNew(() => BackgroundFillAuraList(UnitRef, tokenSource.Token), tokenSource.Token);
            }
        }

        private void handler_FilterChanged(object sender, EventArgs e)
        {
            filteringPredicate = null;

            var predicate = GetAuraPredicate();
            var selectedItem = Auras.SelectedItem;

            Auras.BeginUpdate();
            Auras.Items.Clear();

            if (backgroundTaskFillAuraList?.Status == TaskStatus.Running)
            {
                // Поскольку запущено фоновое сканирование ауры, т.е. изменение auraCollection
                // нужно обеспечить синхронизацию потоков
                lock (auraCollection)
                {
                    var filteredAuras = auraCollection.Where(ai => ai.Visible && predicate(ai));
                    if (filteredAuras.Any())
                        Auras.Items.AddRange(filteredAuras.ToArray());
                } 
            }
            else
            {
                var filteredAuras = auraCollection.Where(ai => ai.Visible && predicate(ai));
                if (filteredAuras.Any())
                    Auras.Items.AddRange(filteredAuras.ToArray());
            }
            Auras.EndUpdate();

            Auras.SelectedItem = selectedItem;
        }
        #endregion
    }
}
