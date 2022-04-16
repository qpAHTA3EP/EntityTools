using AcTp0Tools.Reflection;
using EntityTools.Tools.Classes;
using MyNW.Internals;
using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace EntityTools.Editors
{
    public class ZRangeEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var player = EntityManager.LocalPlayer;
            if (!player.IsLoading)
            {
                var z = (float)Math.Round(player.Location.Z);
                var zDev = context?.Instance?.GetProperty<uint>("ZDeviation");
                if (zDev?.IsValid==true)
                {
                    var dz = zDev.Value;
                    return new Range<float>
                    {
                        Min = z - dz,
                        Max = z + dz
                    };
                }
                return new Range<float>
                {
                    Min = z - 10,
                    Max = z + 10
                };
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
