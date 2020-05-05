using System;
using System.ComponentModel;
using System.Drawing.Design;
using Astral.Logic.NW;
using Astral.Quester.Classes;
using Astral.Quester.Forms;
using DevExpress.XtraEditors;
using EntityTools.Reflection;
using MyNW.Classes;
using MyNW.Internals;
//using static EntityTools.Reflection.PrivateConsructor;
using NPCInfos = Astral.Quester.Classes.NPCInfos;

namespace EntityTools.UIEditors
{
#if DEVELOPER
    internal class NPCVendorInfosExtEditor : UITypeEditor
    {
        static readonly GetAnId vendorTypeSelectForm = null;
        static readonly InstanceFieldAccessor<GetAnId, System.Action> vendorTypeSelectForm_RefreshList;
        static readonly InstanceFieldAccessor<GetAnId, ListBoxControl> vendorTypeSelectForm_List;

        static NPCVendorInfosExtEditor()
        {
            vendorTypeSelectForm = Activator<GetAnId>.CreateInstance("Select vendor");

            vendorTypeSelectForm_RefreshList = vendorTypeSelectForm.GetInstanceField<GetAnId, System.Action>("refreshList");
            vendorTypeSelectForm_RefreshList.Value = RefreshVendorList;

            vendorTypeSelectForm_List = vendorTypeSelectForm.GetInstanceField<GetAnId, ListBoxControl>("listBoxControl1");
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            SetInfos(value as NPCInfos);
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public static bool SetInfos(NPCInfos npc)
        {
            string vendor = GetVendor();
            if (!(vendor == "Normal"))
            {
                npc.CostumeName = vendor;
                npc.DisplayName = vendor;
                npc.MapName = "All";
                return true;
            }
            MessageBoxSpc.smethod_0("Target npc and press ok.", null);
            Entity betterEntityToInteract = Interact.GetBetterEntityToInteract();
            if (betterEntityToInteract.IsValid)
            {
                npc.CostumeName = betterEntityToInteract.CostumeRef.CostumeName;
                npc.DisplayName = betterEntityToInteract.Name;
                npc.Position = betterEntityToInteract.Location.Clone();
                npc.MapName = EntityManager.LocalPlayer.MapState.MapName;
                npc.RegionName = EntityManager.LocalPlayer.RegionInternalName;
                return true;
            }
            return false;
        }

        public static string GetVendor()
        {
            form.ShowDialog();
            if (form.valid && form.listBoxControl1.SelectedItem != null)
            {
                return form.listBoxControl1.SelectedItem.ToString();
            }
            return string.Empty;
        }

        private static void RefreshVendorList()
        {
            if (!(vendorTypeSelectForm_List is null))
            {
                vendorTypeSelectForm_List.Value.Items.Clear();
                vendorTypeSelectForm_List.Value.Items.Add("Normal");
                vendorTypeSelectForm_List.Value.Items.Add("ArtifactVendor");
                vendorTypeSelectForm_List.Value.Items.Add("VIPSummonSealTrader");
                vendorTypeSelectForm_List.Value.Items.Add("VIPProfessionVendor");
            }
        }
    }
#endif
}
