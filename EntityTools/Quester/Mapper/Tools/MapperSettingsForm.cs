using System;
using DevExpress.XtraEditors;
using System.Windows.Forms;

namespace EntityTools.Quester.Mapper.Tools
{
    public partial class MapperSettingsForm : XtraForm//Form
    {
        public MapperSettingsForm()
        {
            InitializeComponent();

            ckbChacheEnable.DataBindings.Add(nameof(ckbChacheEnable.Checked),
                                    EntityTools.Config.Mapper,
                                    nameof(EntityTools.Config.Mapper.CacheActive),
                                    false, DataSourceUpdateMode.OnPropertyChanged);

            editLayerDepth.DataBindings.Add(nameof(editLayerDepth.Value),
                                                EntityTools.Config.Mapper.MapperForm,
                                                nameof(EntityTools.Config.Mapper.MapperForm.LayerDepth),
                                                false, DataSourceUpdateMode.OnPropertyChanged);

            #region Customization
            colorEditBidirPath.DataBindings.Add(nameof(colorEditBidirPath.EditValue),
                                                    EntityTools.Config.Mapper.MapperForm,
                                                    nameof(EntityTools.Config.Mapper.MapperForm.BidirectionalPathColor),
                                                    false, DataSourceUpdateMode.OnPropertyChanged);

            colorEditUnidirPath.DataBindings.Add(nameof(colorEditUnidirPath.EditValue),
                                                EntityTools.Config.Mapper.MapperForm,
                                                nameof(EntityTools.Config.Mapper.MapperForm.UnidirectionalPathColor),
                                                false, DataSourceUpdateMode.OnPropertyChanged);

            colorBackground.DataBindings.Add(nameof(colorBackground.EditValue),
                                                EntityTools.Config.Mapper.MapperForm,
                                                nameof(EntityTools.Config.Mapper.MapperForm.BackgroundColor),
                                                false, DataSourceUpdateMode.OnPropertyChanged);


            ckbEnemies.DataBindings.Add(nameof(ckbEnemies.Checked),
                                            EntityTools.Config.Mapper.MapperForm,
                                            nameof(EntityTools.Config.Mapper.MapperForm.DrawEnemies),
                                            false, DataSourceUpdateMode.OnPropertyChanged);

            colorEnemies.DataBindings.Add(nameof(colorEnemies.EditValue),
                                    EntityTools.Config.Mapper.MapperForm,
                                    nameof(EntityTools.Config.Mapper.MapperForm.EnemyColor),
                                    false, DataSourceUpdateMode.OnPropertyChanged);

            ckbFriends.DataBindings.Add(nameof(ckbFriends.Checked),
                                    EntityTools.Config.Mapper.MapperForm,
                                    nameof(EntityTools.Config.Mapper.MapperForm.DrawFriends),
                                    false, DataSourceUpdateMode.OnPropertyChanged);

            colorFriends.DataBindings.Add(nameof(colorFriends.EditValue),
                                    EntityTools.Config.Mapper.MapperForm,
                                    nameof(EntityTools.Config.Mapper.MapperForm.FriendColor),
                                    false, DataSourceUpdateMode.OnPropertyChanged);

            ckbPlayers.DataBindings.Add(nameof(ckbPlayers.Checked),
                                    EntityTools.Config.Mapper.MapperForm,
                                    nameof(EntityTools.Config.Mapper.MapperForm.DrawPlayers),
                                    false, DataSourceUpdateMode.OnPropertyChanged);

            colorPlayers.DataBindings.Add(nameof(colorPlayers.EditValue),
                                    EntityTools.Config.Mapper.MapperForm,
                                    nameof(EntityTools.Config.Mapper.MapperForm.PlayerColor),
                                    false, DataSourceUpdateMode.OnPropertyChanged);

            ckbOtherNPC.DataBindings.Add(nameof(ckbOtherNPC.Checked),
                                    EntityTools.Config.Mapper.MapperForm,
                                    nameof(EntityTools.Config.Mapper.MapperForm.DrawOtherNPC),
                                    false, DataSourceUpdateMode.OnPropertyChanged);

            colorOtherNPC.DataBindings.Add(nameof(colorOtherNPC.EditValue),
                                    EntityTools.Config.Mapper.MapperForm,
                                    nameof(EntityTools.Config.Mapper.MapperForm.OtherNPCColor),
                                    false, DataSourceUpdateMode.OnPropertyChanged);

            ckbNodes.DataBindings.Add(nameof(ckbNodes.Checked),
                                    EntityTools.Config.Mapper.MapperForm,
                                    nameof(EntityTools.Config.Mapper.MapperForm.DrawNodes),
                                    false, DataSourceUpdateMode.OnPropertyChanged);

            colorNodes.DataBindings.Add(nameof(colorNodes.EditValue),
                                    EntityTools.Config.Mapper.MapperForm,
                                    nameof(EntityTools.Config.Mapper.MapperForm.NodeColor),
                                    false, DataSourceUpdateMode.OnPropertyChanged);

            ckbSkillnodes.DataBindings.Add(nameof(ckbSkillnodes.Checked),
                                    EntityTools.Config.Mapper.MapperForm,
                                    nameof(EntityTools.Config.Mapper.MapperForm.DrawSkillNodes),
                                    false, DataSourceUpdateMode.OnPropertyChanged);

            colorSkillnodes.DataBindings.Add(nameof(colorSkillnodes.EditValue),
                                    EntityTools.Config.Mapper.MapperForm,
                                    nameof(EntityTools.Config.Mapper.MapperForm.SkillNodeColor),
                                    false, DataSourceUpdateMode.OnPropertyChanged);
            #endregion
        }

        private void handler_LayerDepth_Changed(object sender, EventArgs e)
        {
            if(Owner is MapperFormExt mapper)
            {
                mapper.CacheDistanceZ = editLayerDepth.Value > 0
                    ? Convert.ToDouble(editLayerDepth.Value)
                    : double.MaxValue;
            }
        }
    }
}
