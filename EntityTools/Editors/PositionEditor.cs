using AStar;
using EntityTools.Quester.Actions;
using EntityTools.Quester.Editor;
using Infrastructure;
using Infrastructure.Reflection;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace EntityTools.Editors
{
    internal class PositionEditor : UITypeEditor
    {
#if pgAccessor
        private PropertyAccessor<PropertyGrid> pgAccessor; 
#endif

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var player = EntityManager.LocalPlayer;
            if (!player.IsLoading)
            {
                var playerPos = player.Location.Clone();
                Node node = null;
                double dist = double.MaxValue;

#if pgAccessor
                if (pgAccessor is null)
                    pgAccessor = context.GetProperty<PropertyGrid>("OwnerGrid");

                if (pgAccessor.IsValid)
                {
                    if (pgAccessor.Value?.ParentForm is QuesterEditor questerEditor)
                    {
                        node = questerEditor.Profile
                                            .CurrentMesh
                                            .ClosestNode(playerPos.X,
                                                         playerPos.Y,
                                                         playerPos.Z,
                                                         out dist,
                                                         false);
                    }
                }

                if (node is null)
                {
                    node = AstralAccessors.Quester
                                          .Core
                                          .CurrentProfile
                                          .CurrentMesh
                                          .ClosestNode(playerPos.X,
                                                       playerPos.Y,
                                                       playerPos.Z,
                                                       out dist,
                                                       false);
                } 
#else
                node = context.GetQuesterProfile()
                             ?.CurrentMesh
                              .ClosestNode(playerPos.X,
                                           playerPos.Y,
                                           playerPos.Z,
                                           out dist,
                                           false);
#endif

                uint detectWaipointRadius = 10u;
                if (context?.Instance is Jumping jumping)
                {
                    detectWaipointRadius = jumping.DetectR;
                }

                if (node != null && dist <= detectWaipointRadius)
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
