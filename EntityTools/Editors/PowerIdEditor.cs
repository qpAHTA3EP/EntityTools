using Astral.Logic.NW;
using EntityTools.Tools.Export;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;

namespace EntityTools.Editors
{
#if DEVELOPER
    class PowerIdEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var powerId = value?.ToString();
            PowerInfo powerInfo = null;
            if (!string.IsNullOrEmpty(powerId))
            {
                var power = Powers.GetPowerByInternalName(powerId);
                if (power != null && power.IsValid)
                    powerInfo = new PowerInfo(power);
            }

            if (EntityTools.Core.UserRequest_SelectItem(GetPower, ref powerInfo, nameof(PowerInfo.FullName)))
            {
                return powerInfo?.InternalName;
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        // Оборачиваем список Power в 
        private static IEnumerable<PowerInfo> GetPower()
        {
            return new List<PowerInfo>(EntityManager.LocalPlayer.Character.Powers.Select(p => new PowerInfo(p)));
        }
    }
#endif
}
