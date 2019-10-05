using DevExpress.XtraEditors;
using EntityTools.Tools;
using MyNW.Internals;
using System;
using System.Windows.Forms;

namespace EntityTools.Forms
{
    public partial class PlayerParagonSelectForm : XtraForm //*/Form
    {
        private static PlayerParagonSelectForm paragonForm = null;

        public PlayerParagonSelectForm()
        {
            InitializeComponent();

            Wizard.Text = Game.CharacterPaths.Find(path => path.Name == ParagonTools.idWizard)?.DisplayName ?? "Wizard";
            Wizard_Arcanist.Text = Game.CharacterPaths.Find(path => path.Name == ParagonTools.idWizard_Arcanist)?.DisplayName ?? "Arcanist";
            Wizard_Thaumaturge.Text = Game.CharacterPaths.Find(path => path.Name == ParagonTools.idWizard_Thaumaturge)?.DisplayName ?? "Thaumaturge";

            Cleric.Text = Game.CharacterPaths.Find(path => path.Name == ParagonTools.idCleric)?.DisplayName ?? "Cleric";
            Cleric_Devout.Text = Game.CharacterPaths.Find(path => path.Name == ParagonTools.idCleric_Devout)?.DisplayName ?? "Devout";
            Cleric_Arbiter.Text = Game.CharacterPaths.Find(path => path.Name == ParagonTools.idCleric_Arbiter)?.DisplayName ?? "Arbiter";

            Barbarian.Text = Game.CharacterPaths.Find(path => path.Name == ParagonTools.idBarbarian)?.DisplayName ?? "Barbarian";
            Barbarian_Sentinel.Text = Game.CharacterPaths.Find(path => path.Name == ParagonTools.idBarbarian_Sentinel)?.DisplayName ?? "Sentinel";
            Barbarian_Blademaster.Text = Game.CharacterPaths.Find(path => path.Name == ParagonTools.idBarbarian_Blademaster)?.DisplayName ?? "Blademaster";

            Fighter.Text = Game.CharacterPaths.Find(path => path.Name == ParagonTools.idFighter)?.DisplayName ?? "Fighter";
            Fighter_Vanguard.Text = Game.CharacterPaths.Find(path => path.Name == ParagonTools.idFighter_Vanguard)?.DisplayName ?? "Vanguard";
            Fighter_Dreadnought.Text = Game.CharacterPaths.Find(path => path.Name == ParagonTools.idFighter_Dreadnought)?.DisplayName ?? "Dreadnought";

            Ranger.Text = Game.CharacterPaths.Find(path => path.Name == ParagonTools.idRanger)?.DisplayName ?? "Ranger";
            Ranger_Hunter.Text = Game.CharacterPaths.Find(path => path.Name == ParagonTools.idRanger_Hunter)?.DisplayName ?? "Hunter";
            Ranger_Warden.Text = Game.CharacterPaths.Find(path => path.Name == ParagonTools.idRanger_Warden)?.DisplayName ?? "Warden";

            Paladin.Text = Game.CharacterPaths.Find(path => path.Name == ParagonTools.idPaladin)?.DisplayName ?? "Paladin";
            Paladin_Oathkeeper.Text = Game.CharacterPaths.Find(path => path.Name == ParagonTools.idPaladin_Oathkeeper)?.DisplayName ?? "Oathkeeper";
            Paladin_Justicar.Text = Game.CharacterPaths.Find(path => path.Name == ParagonTools.idPaladin_Justicar)?.DisplayName ?? "Justicar";

            Warlock.Text = Game.CharacterPaths.Find(path => path.Name == ParagonTools.idWarlock)?.DisplayName ?? "Warlock";
            Warlock_Hellbringer.Text = Game.CharacterPaths.Find(path => path.Name == ParagonTools.idWarlock_Hellbringer)?.DisplayName ?? "Hellbringer";
            Warlock_Soulweaver.Text = Game.CharacterPaths.Find(path => path.Name == ParagonTools.idWarlock_Soulweaver)?.DisplayName ?? "Soulweaver";

            Rogue.Text = Game.CharacterPaths.Find(path => path.Name == ParagonTools.idRogue)?.DisplayName ?? "Rogue";
            Rogue_Assassin.Text = Game.CharacterPaths.Find(path => path.Name == ParagonTools.idRogue_Assassin)?.DisplayName ?? "Assassin";
            Rogue_Whisperknife.Text = Game.CharacterPaths.Find(path => path.Name == ParagonTools.idRogue_Whisperknife)?.DisplayName ?? "Whisperknife";
        }

        public static SelectedParagons GetParagons(SelectedParagons paragons = null)
        {
            if (paragonForm == null)
                paragonForm = new PlayerParagonSelectForm();

            if (paragons != null)
            {
                paragonForm.Wizard.Checked = paragons.Wizard_Arcanist && paragons.Wizard_Thaumaturge;
                paragonForm.Wizard_Arcanist.Checked = paragons.Wizard_Arcanist;
                paragonForm.Wizard_Thaumaturge.Checked = paragons.Wizard_Thaumaturge;

                paragonForm.Cleric.Checked = paragons.Cleric_Devout && paragons.Cleric_Arbiter;
                paragonForm.Cleric_Devout.Checked = paragons.Cleric_Devout;
                paragonForm.Cleric_Arbiter.Checked = paragons.Cleric_Arbiter;

                paragonForm.Barbarian.Checked = paragons.Barbarian_Sentinel && paragons.Barbarian_Blademaster;
                paragonForm.Barbarian_Sentinel.Checked = paragons.Barbarian_Sentinel;
                paragonForm.Barbarian_Blademaster.Checked = paragons.Barbarian_Blademaster;

                paragonForm.Fighter.Checked = paragons.Fighter_Vanguard && paragons.Fighter_Dreadnought;
                paragonForm.Fighter_Vanguard.Checked = paragons.Fighter_Vanguard;
                paragonForm.Fighter_Dreadnought.Checked = paragons.Fighter_Dreadnought;

                paragonForm.Ranger.Checked = paragons.Ranger_Hunter && paragons.Ranger_Warden;
                paragonForm.Ranger_Hunter.Checked = paragons.Ranger_Hunter;
                paragonForm.Ranger_Warden.Checked = paragons.Ranger_Warden;

                paragonForm.Paladin.Checked = paragons.Paladin_Oathkeeper && paragons.Paladin_Justicar;
                paragonForm.Paladin_Oathkeeper.Checked = paragons.Paladin_Oathkeeper;
                paragonForm.Paladin_Justicar.Checked = paragons.Paladin_Justicar;

                paragonForm.Warlock.Checked = paragons.Warlock_Soulweaver && paragons.Warlock_Hellbringer;
                paragonForm.Warlock_Soulweaver.Checked = paragons.Warlock_Soulweaver;
                paragonForm.Warlock_Hellbringer.Checked = paragons.Warlock_Hellbringer;

                paragonForm.Rogue.Checked = paragons.Rogue_Assassin && paragons.Rogue_Whisperknife;
                paragonForm.Rogue_Assassin.Checked = paragons.Rogue_Assassin;
                paragonForm.Rogue_Whisperknife.Checked = paragons.Rogue_Whisperknife;
            }
            else paragons = new SelectedParagons();

            paragonForm.ShowDialog();

            if (paragonForm.DialogResult == DialogResult.OK)
            {
                //paragons.Wizard = paragonForm.Wizard.Checked;
                paragons.Wizard_Arcanist = paragonForm.Wizard_Arcanist.Checked;
                paragons.Wizard_Thaumaturge = paragonForm.Wizard_Thaumaturge.Checked;

                //paragons.Cleric = paragonForm.Cleric.Checked;
                paragons.Cleric_Devout = paragonForm.Cleric_Devout.Checked;
                paragons.Cleric_Arbiter = paragonForm.Cleric_Arbiter.Checked;

                //paragons.Barbarian = paragonForm.Barbarian.Checked;
                paragons.Barbarian_Sentinel = paragonForm.Barbarian_Sentinel.Checked;
                paragons.Barbarian_Blademaster = paragonForm.Barbarian_Blademaster.Checked;

                //paragons.Fighter = paragonForm.Fighter.Checked;
                paragons.Fighter_Vanguard = paragonForm.Fighter_Vanguard.Checked;
                paragons.Fighter_Dreadnought = paragonForm.Fighter_Dreadnought.Checked;

                //paragons.Ranger = paragonForm.Ranger.Checked;
                paragons.Ranger_Hunter = paragonForm.Ranger_Hunter.Checked;
                paragons.Ranger_Warden = paragonForm.Ranger_Warden.Checked;

                //paragons.Paladin = paragonForm.Paladin.Checked;
                paragons.Paladin_Oathkeeper = paragonForm.Paladin_Oathkeeper.Checked;
                paragons.Paladin_Justicar = paragonForm.Paladin_Justicar.Checked;

                //paragons.Warlock = paragonForm.Warlock.Checked;
                paragons.Warlock_Soulweaver = paragonForm.Warlock_Soulweaver.Checked;
                paragons.Warlock_Hellbringer = paragonForm.Warlock_Hellbringer.Checked;

                //paragons.Rogue = paragonForm.Rogue.Checked;
                paragons.Rogue_Assassin = paragonForm.Rogue_Assassin.Checked;
                paragons.Rogue_Whisperknife = paragonForm.Rogue_Whisperknife.Checked;
            }

            return paragons;
        }

        private void Wizard_CheckedChanged(object sender, EventArgs e)
        {
            Wizard_Arcanist.Checked = Wizard.Checked;
            Wizard_Thaumaturge.Checked = Wizard.Checked;
            gbWizard.Enabled = !Wizard.Checked;
        }
        private void Cleric_CheckedChanged(object sender, EventArgs e)
        {
            Cleric_Devout.Checked = Cleric.Checked;
            Cleric_Arbiter.Checked = Cleric.Checked;
            gbCleric.Enabled = !Cleric.Checked;
        }
        private void Barbarian_CheckedChanged(object sender, EventArgs e)
        {
            Barbarian_Blademaster.Checked = Barbarian.Checked;
            Barbarian_Sentinel.Checked = Barbarian.Checked;
            gbBarbarian.Enabled = !Barbarian.Checked;
        }
        private void Fighter_CheckedChanged(object sender, EventArgs e)
        {
            Fighter_Vanguard.Checked = Fighter.Checked;
            Fighter_Dreadnought.Checked = Fighter.Checked;
            gbFighter.Enabled = !Fighter.Checked;
        }
        private void Ranger_CheckedChanged(object sender, EventArgs e)
        {
            Ranger_Hunter.Checked = Ranger.Checked;
            Ranger_Warden.Checked = Ranger.Checked;
            gbRanger.Enabled = !Ranger.Checked;
        }
        private void Paladin_CheckedChanged(object sender, EventArgs e)
        {
            Paladin_Oathkeeper.Checked = Paladin.Checked;
            Paladin_Justicar.Checked = Paladin.Checked;
            gbPaladin.Enabled = !Paladin.Checked;
        }
        private void Warlock_CheckedChanged(object sender, EventArgs e)
        {
            Warlock_Hellbringer.Checked = Warlock.Checked;
            Warlock_Soulweaver.Checked = Warlock.Checked;
            gbWarlock.Enabled = !Warlock.Checked;
        }
        private void Rogue_CheckedChanged(object sender, EventArgs e)
        {
            Rogue_Assassin.Checked = Rogue.Checked;
            Rogue_Whisperknife.Checked = Rogue.Checked;
            gbRogue.Enabled = !Rogue.Checked;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
