using System;
using System.Windows.Forms;
using Astral.Logic.NW;
using DevExpress.XtraEditors;
using EntityTools.Forms;
using EntityTools.Tools;
using MyNW.Classes;
using MyNW.Internals;
using static EntityTools.Forms.EntitySelectForm;

namespace EntityTools.Editors.Forms
{
    public enum SelectedAuraSource
    {
        Player,
        Target,
        Entity
    };

    public partial class AuraSelectForm : XtraForm
    {
        private static AuraSelectForm auraSelectForm = null;

        internal Entity entity = null;
        internal Entity UnitRef
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
                                while (TargetSelectForm.TargetGuiRequest("Target NPC and press ok.", this) == DialogResult.OK)
                                {
                                    Entity betterEntityToInteract = Interact.GetBetterEntityToInteract();
                                    if (betterEntityToInteract != null && betterEntityToInteract.IsValid)
                                    {
                                        entity = betterEntityToInteract;
                                        unitRefName.Text = $"{entity.Name} [{entity.InternalName}]";
                                            ;
                                        break;
                                    }
                                }
                            return entity;
                        case SelectedAuraSource.Entity:
                            if (entity == null || !entity.IsValid)
                            {
                                entity = EntitySelectForm.GetEntity()?.entity;
                                if (!string.IsNullOrEmpty(entity.InternalName))
                                    unitRefName.Text = entity.InternalName;
                                else if (!string.IsNullOrEmpty(entity.Name))
                                    unitRefName.Text = entity.Name;
                                else unitRefName.Text = entity.NameUntranslated;
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

        public static string GetAuraId()
        {
            if(auraSelectForm == null || auraSelectForm.IsDisposed)
                auraSelectForm = new AuraSelectForm();

            auraSelectForm.NameFilter.Text = string.Empty;
            auraSelectForm.InternalNameFilter.Text = string.Empty;

            if (auraSelectForm.ShowDialog() == DialogResult.OK)
                if (auraSelectForm.Auras.SelectedItem is AuraDef aura)
                    return aura.InternalName;
            
            return string.Empty;
        }

        private void FillAuraList(Character character)
        {
            Auras.Items.Clear();

            if (character != null && character.IsValid)
            {
                // Конструирование предиката
                Predicate<AttribModNet> predicate;
                if (string.IsNullOrEmpty(NameFilter.Text))
                    if (string.IsNullOrEmpty(InternalNameFilter.Text))
                        predicate = (AttribModNet mDef) => mDef.PowerDef.IsValid;
                    else predicate = (AttribModNet mDef) => mDef.PowerDef.IsValid
                                                            && mDef.PowerDef.InternalName.IndexOf(InternalNameFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                else if (string.IsNullOrEmpty(InternalNameFilter.Text))
                    predicate = (AttribModNet mDef) => mDef.PowerDef.IsValid
                                                       && mDef.PowerDef.DisplayName.IndexOf(NameFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                else predicate = (AttribModNet mDef) => mDef.PowerDef.IsValid
                                                        && mDef.PowerDef.DisplayName.IndexOf(NameFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0 
                                                        && mDef.PowerDef.InternalName.IndexOf(InternalNameFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0;

                foreach (AttribModNet def in character.Mods)
                    if (predicate(def))
                        //auraList.Add(new AuraDef(def.PowerDef));
                        Auras.Items.Add(new AuraDef(def.PowerDef));
            }
        }

        #region Обработчики
        private void btnPlayer_Click(object sender, EventArgs e)
        {
            //selectedAuraSource = SelectedAuraSource.Player;
            Selector.SelectedItem = SelectedAuraSource.Player;
            unitRefName.Text = EntityManager.LocalPlayer.InternalName;
            FillAuraList(EntityManager.LocalPlayer.Character);
        }

        private void btnTarget_Click(object sender, EventArgs e)
        {
            //selectedAuraSource = SelectedAuraSource.Target;
            Selector.SelectedItem = SelectedAuraSource.Target;
            unitRefName.Text = EntityManager.LocalPlayer.Character.CurrentTarget.InternalName;
            FillAuraList(EntityManager.LocalPlayer.Character.CurrentTarget.Character);
        }

        private void btnEntity_Click(object sender, EventArgs e)
        {
            //selectedAuraSource = SelectedAuraSource.Entity;
            Selector.SelectedItem = SelectedAuraSource.Entity;
            EntityDif entity = EntitySelectForm.GetEntity();
            if (!string.IsNullOrEmpty(entity.InternalName))
                unitRefName.Text = entity.InternalName;
            else if (!string.IsNullOrEmpty(entity.Name))
                unitRefName.Text = entity.Name;
            else unitRefName.Text = entity.NameUntranslated;
            if (entity != null && entity.entity != null)
                FillAuraList(entity.entity.Character.CurrentTarget.Character);
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void AuraSelectForm_Load(object sender, EventArgs e)
        {
            Selector.DataSource = Enum.GetValues(typeof(SelectedAuraSource));
        }

        private void Selector_SelectedIndexChanged(object sender, EventArgs e)
        {
            entity = null;
            FillAuraList(UnitRef?.Character);
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            FillAuraList(UnitRef?.Character);
        }
        #endregion

        private void lbAuras_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(Auras.SelectedItem is AuraDef aura)
            {
                Description.Text = aura.Description;
            }
        }
    }
}
