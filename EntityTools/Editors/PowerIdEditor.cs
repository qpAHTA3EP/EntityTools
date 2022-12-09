using EntityTools.Tools;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using EntityTools.Forms;

namespace EntityTools.Editors
{
#if DEVELOPER
    class PowerIdEditor : UITypeEditor
    {
        private static List<Tuple<string, string>> powerCache;
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            return GetPowerId(value?.ToString());
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        // Оборачиваем список Power в функтор
        private static IEnumerable<Tuple<string, string>> GetPowers()
        {
            return powerCache;
        }

        public static string GetPowerId(string powerId)
        {
            Tuple<string, string> defaultValue = null;
            var powers = EntityManager.LocalPlayer.Character.Powers;
            if (string.IsNullOrEmpty(powerId))
            {
                powerCache = powers.Select(p =>
                {
                    var powDef = p.PowerDef;
                    var id = powDef.InternalName;
                    return Tuple.Create(powDef.InternalName,
                        string.IsNullOrEmpty(powDef.DisplayName)
                            ? id
                            : $"{powDef.DisplayName} [{id}]");
                }).ToList();
            }
            else
            {
                var predicate = powerId.GetComparer();
                powerCache = new List<Tuple<string, string>>(powers.Count);
                foreach (var pwr in powers)
                {
                    var powDef = pwr.PowerDef;
                    var id = powDef.InternalName;
                    var tuple = Tuple.Create(powDef.InternalName,
                        string.IsNullOrEmpty(powDef.DisplayName)
                            ? id
                            : $"{powDef.DisplayName} [{id}]");
                    powerCache.Add(tuple);
                    if (predicate(pwr.PowerDef.InternalName))
                        defaultValue = tuple;
                }
            }
            if (ItemSelectForm.GetAnItem(GetPowers, ref defaultValue, nameof(defaultValue.Item2)))
            {
                return defaultValue?.Item1;
            }

            return powerId;
        }
    }
#endif
}
