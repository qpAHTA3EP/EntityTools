using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Astral.Classes.ItemFilter;
using Astral.Logic.NW;
using DevExpress.XtraEditors;
using EntityCore.Entities;
using EntityCore.Forms;
using EntityTools.Enums;
using EntityTools.Tools;
using MyNW.Classes;
using MyNW.Internals;
using static EntityCore.Forms.EntitySelectForm;

namespace EntityCore.Forms
{
    public enum SelectedAuraSource
    {
        Player,
        Target,
        Entity
    };

    public partial class AuraSelectForm : XtraForm //*/ Form
    {
        private static AuraSelectForm @this = null;

        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        private CancellationToken cancellationToken;
        private Task backgroundTaskFillAuraList;

        private Entity entity = null;
        private Entity UnitRef
        {
            get
            {
                if (Selector.SelectedItem == null)
                    Selector.SelectedItem = SelectedAuraSource.Player;

                if (Selector.SelectedItem is SelectedAuraSource selectedAuraSource)
                {
                    switch (selectedAuraSource)
                    {
                        case SelectedAuraSource.Player:
                            unitRefName.Text = EntityManager.LocalPlayer.InternalName;
                            return entity = EntityManager.LocalPlayer;
                        case SelectedAuraSource.Target:
                            if(entity == null || !entity.IsValid)
                                while (TargetSelectForm.GUIRequest("Target NPC and press ok.", this) == DialogResult.OK)
                                {
                                    Entity betterEntityToInteract = Interact.GetBetterEntityToInteract();
                                    if (betterEntityToInteract != null && betterEntityToInteract.IsValid)
                                    {
                                        entity = betterEntityToInteract;
                                        unitRefName.Text = $"{entity.Name} [{entity.InternalName}]";
                                        break;
                                    }
                                }
                            return entity;
                        case SelectedAuraSource.Entity:
                            if (entity == null || !entity.IsValid)
                            {
                                string pattern = string.Empty;
                                ItemFilterStringType strMatch = ItemFilterStringType.Simple;
                                EntityNameType nameType = EntityNameType.InternalName;
                                entity = EntitySelectForm.GUIRequest(ref pattern, ref strMatch, ref nameType);

                                if (entity != null)
                                {
                                    if(!string.IsNullOrEmpty(pattern))
                                        unitRefName.Text = pattern;
                                    else if (!string.IsNullOrEmpty(entity.InternalName))
                                        unitRefName.Text = entity.InternalName;
                                    else if (!string.IsNullOrEmpty(entity.Name))
                                        unitRefName.Text = entity.Name;
                                    else unitRefName.Text = entity.NameUntranslated;
                                }
                            }
                            return entity;
                    }
                }
                unitRefName.Text = "-";
                return null;
            }
        }

        public AuraSelectForm()
        {
            InitializeComponent();
        }

        public static string GUIRequest()
        {
            if(@this == null || @this.IsDisposed)
                @this = new AuraSelectForm();

            @this.NameFilter.Text = string.Empty;
            @this.InternalNameFilter.Text = string.Empty;

            if (@this.ShowDialog() == DialogResult.OK)
                if (@this.Auras.SelectedItem is AuraInfo aura)
                    return aura.InternalName;
            
            return string.Empty;
        }

        //static public void ShowFreeTool()
        //{
        //    AuraSelectForm @this = new AuraSelectForm();

        //    @this.NameFilter.Text = string.Empty;
        //    @this.InternalNameFilter.Text = string.Empty;

        //    @this.Show(Astral.Forms.Main.ActiveForm);
        //}

        private void FillAuraList(Character character)
        {
            Auras.Items.Clear();

            if (character != null && character.IsValid)
            {
                // Конструирование предиката
                Predicate<AttribModNet> predicate = GetAuraPredicate();

                foreach (AttribModNet def in character.Mods)
                    if (predicate(def))
                        Auras.Items.Add(new AuraInfo(def.PowerDef));
            }
        }

        private void BackgroundFillAuraList(Character character, CancellationToken token)
        {
            if (character != null && character.IsValid)
            {
                Astral.Controllers.Forms.InvokeOnMainThread(() => Auras.Items.Clear());


                // Список обработанных аур
                List<AuraInfo> auraList = new List<AuraInfo>();

                foreach(AttribModNet def in character.Mods)
                    auraList.Add(new AuraInfo(def.PowerDef));

                while(!token.IsCancellationRequested)
                {
                    // Конструирование предиката
                    Predicate<AttribModNet> predicate = GetAuraPredicate();

                    foreach (AttribModNet def in character.Mods)
                    {
                        AuraInfo auraDef = new AuraInfo(def.PowerDef);
                        if (!auraList.Contains(auraDef))
                        {
                            auraList.Add(auraDef);
                            if(predicate(def))
                                Astral.Controllers.Forms.InvokeOnMainThread(() => Auras.Items.Add(auraDef));
                        }
                    }

                    Thread.Sleep(100);
                }
            }
        }

        private Predicate<AttribModNet> GetAuraPredicate()
        {
            Predicate<AttribModNet> predicate;
            if (string.IsNullOrEmpty(NameFilter.Text))
                if (string.IsNullOrEmpty(InternalNameFilter.Text))
                    predicate = (AttribModNet mDef) => mDef.PowerDef.IsValid && !Auras.Items.Contains(mDef);
                else predicate = (AttribModNet mDef) => mDef.PowerDef.IsValid
                                                        && mDef.PowerDef.InternalName.IndexOf(InternalNameFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0
                                                        && !Auras.Items.Contains(mDef);
            else if (string.IsNullOrEmpty(InternalNameFilter.Text))
                predicate = (AttribModNet mDef) => mDef.PowerDef.IsValid
                                                   && mDef.PowerDef.DisplayName.IndexOf(NameFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0
                                                   && !Auras.Items.Contains(mDef);
            else predicate = (AttribModNet mDef) => mDef.PowerDef.IsValid
                                                    && mDef.PowerDef.DisplayName.IndexOf(NameFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0
                                                    && mDef.PowerDef.InternalName.IndexOf(InternalNameFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0
                                                    && !Auras.Items.Contains(mDef);
            return predicate;
        }

        #region Обработчики
        //private void btnPlayer_Click(object sender, EventArgs e)
        //{
        //    //selectedAuraSource = SelectedAuraSource.Player;
        //    Selector.SelectedItem = SelectedAuraSource.Player;
        //    unitRefName.Text = EntityManager.LocalPlayer.InternalName;
        //    FillAuraList(EntityManager.LocalPlayer.Character);
        //}

        //private void btnTarget_Click(object sender, EventArgs e)
        //{
        //    //selectedAuraSource = SelectedAuraSource.Target;
        //    Selector.SelectedItem = SelectedAuraSource.Target;
        //    unitRefName.Text = EntityManager.LocalPlayer.Character.CurrentTarget.InternalName;
        //    FillAuraList(EntityManager.LocalPlayer.Character.CurrentTarget.Character);
        //}

        //private void btnEntity_Click(object sender, EventArgs e)
        //{
        //    //selectedAuraSource = SelectedAuraSource.Entity;
        //    Selector.SelectedItem = SelectedAuraSource.Entity;
        //    Entity entity = EntitySelectForm.GUIRequest();
        //    if (entity != null)
        //    {
        //        if (!string.IsNullOrEmpty(entity.InternalName))
        //            unitRefName.Text = entity.InternalName;
        //        else if (!string.IsNullOrEmpty(entity.Name))
        //            unitRefName.Text = entity.Name;
        //        else unitRefName.Text = entity.NameUntranslated;
        //        FillAuraList(entity.Character.CurrentTarget.Character);
        //    }
        //}

        private void btnSelect_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void AuraSelectForm_Load(object sender, EventArgs e)
        {
            Selector.Properties.Items.Clear();
            Selector.Properties.Items.AddRange(Enum.GetValues(typeof(SelectedAuraSource)));
        }

        private void Selector_SelectedIndexChanged(object sender, EventArgs e)
        {
            ckbNewAuras.Checked = false;
            entity = null;
            FillAuraList(UnitRef?.Character);
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            if (ckbNewAuras.Checked)
                ckbNewAuras_CheckedChanged(sender, e);
            else FillAuraList(UnitRef?.Character);
        }

        private void lbAuras_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(Auras.SelectedItem is AuraInfo aura)
            {
                Description.Text = aura.Description;
                //webDescription.Document.OpenNew(true);
                //webDescription.Document.Write(aura.Description);
            }
        }

        private void ckbNewAuras_CheckedChanged(object sender, EventArgs e)
        {
            if (backgroundTaskFillAuraList?.Status == TaskStatus.Running)
                tokenSource?.Cancel();

            if (ckbNewAuras.Checked)
            {
                tokenSource = new CancellationTokenSource();
                cancellationToken = tokenSource.Token;
                backgroundTaskFillAuraList = Task.Factory.StartNew(() => BackgroundFillAuraList(UnitRef?.Character, tokenSource.Token), tokenSource.Token);
            }
        }
        #endregion
    }
}
