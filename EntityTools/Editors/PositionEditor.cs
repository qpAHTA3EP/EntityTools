using AcTp0Tools;
using MyNW.Internals;
using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace EntityTools.Editors
{
    public class PositionEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var player = EntityManager.LocalPlayer;
            if (!player.IsLoading)
            {
                var playerPos = player.Location.Clone();

                var node = AstralAccessors.Quester.Core.Meshes.ClosestNode(playerPos.X, playerPos.Y, playerPos.Z,
                    out double dist, false);

                if (node != null && dist <= 10)
                {
                    playerPos = node.Position;
                }

                return playerPos;
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
