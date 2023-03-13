using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using Astral.Quester.Forms;
using DevExpress.XtraEditors;
using EntityTools.Enums;
using EntityTools.Forms;
using EntityTools.Tools;
using EntityTools.Tools.Inventory;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityTools.Editors
{
    internal class VendorInfoEditor : UITypeEditor
    {
        private static readonly VendorType[] displayedVendors = { //VendorType.Auto,
                                                                    VendorType.Normal,
                                                                    VendorType.ArtifactVendor,
                                                                    VendorType.RemoteVendor,
                                                                    VendorType.Node,
                                                                    VendorType.VIPSealTrader,
                                                                    VendorType.VIPProfessionVendor
                                                                };

        static VendorInfoEditor() { }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (SetInfos(out VendorInfo vendor))
            {
                return vendor;
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public static bool SetInfos(out VendorInfo vendor)
        {
            vendor = null;
            var vndType = VendorType.None;
            var localPlayer = EntityManager.LocalPlayer;

            if (ItemSelectForm.GetAnItem(() => displayedVendors, ref vndType))
            {
                VendorInfo vendorInfo;
                switch (vndType)
                {
                    case VendorType.None:
                        return false;
#if VendorType_Auto
                    case VendorType.Auto:
                        return false; 
#endif
                    case VendorType.Normal:
                        Entity entity = TargetSelectHelper.GetEntityToInteract();
                        if (entity != null)
                        {
                            vendorInfo = new VendorInfo
                            {
                                VendorType = vndType,
                                CostumeName = entity.CostumeRef.CostumeName,
                                DisplayName = entity.Name,
                                Position = entity.Location.Clone(),
                                MapName = localPlayer.MapState.MapName,
                                RegionName = localPlayer.RegionInternalName
                            };
                            vendor = vendorInfo;
                            return true;
                        }
                        return false;
                    case VendorType.RemoteVendor:
                        string contactName = GetAnId.GetARemoteContact();
                        if (!string.IsNullOrEmpty(contactName))
                        {
                            vendorInfo = new VendorInfo
                            {
                                VendorType = vndType,
                                CostumeName = contactName,
                                DisplayName = contactName
                            };
                            vendor = vendorInfo;
                            if (XtraMessageBox.Show("Call it now ?", "Confirm", 
                                    MessageBoxButtons.YesNo, 
                                    MessageBoxIcon.Question) 
                                == DialogResult.Yes)
                            {
                                var contact = localPlayer.Player.InteractInfo.RemoteContacts.FirstOrDefault(cnt => cnt.ContactDef == contactName);
                                if (contact != null)
                                {
                                    vendor.DisplayName = contact.DisplayName;
                                    contact.Start();
                                    break;
                                }
                            }
                        }
                        return !string.IsNullOrEmpty(contactName);
                    case VendorType.Node:
                        var position = TargetSelectHelper.GetNodeLocation("Get Vendor", "Get vendor-node location");
                        if (position.IsValid)
                        {
                            vendorInfo = new VendorInfo
                            {
                                VendorType = vndType,
                                Position = position,
                                MapName = localPlayer.MapState.MapName,
                                RegionName = localPlayer.RegionInternalName
                            };
                            vendor = vendorInfo;
                            return true;
                        }
                        return true;
                    default:
                        vendorInfo = new VendorInfo
                        {
                            VendorType = vndType,
                            CostumeName = vndType.ToString(),
                            DisplayName = vndType.ToString()
                        };
                        vendor = vendorInfo;
                        return true;
                }
            }
            return false;
        }
    }
}
