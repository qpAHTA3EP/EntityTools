using Astral.Quester.Forms;
using DevExpress.XtraEditors;
using EntityTools.Enums;
using EntityTools.Tools.Inventory;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace EntityTools.Editors
{
#if DEVELOPER
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

            if (EntityTools.Core.UserRequest_SelectItem(() => displayedVendors, ref vndType))
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
                        Entity entity = null;
                        if (EntityTools.Core.UserRequest_GetEntityToInteract(ref entity))
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
                            if ((XtraMessageBox.Show("Call it now ?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))
                            {
                                foreach (RemoteContact remoteContact in localPlayer.Player.InteractInfo.RemoteContacts)
                                {
                                    if (remoteContact.ContactDef == contactName)
                                    {
                                        vendor.DisplayName = remoteContact.DisplayName;
                                        remoteContact.Start();
                                        break;
                                    }
                                }
                            }
                        }
                        return !string.IsNullOrEmpty(contactName);
                    case VendorType.Node:
                        Vector3 pos = new Vector3();
                        if (EntityTools.Core.UserRequest_GetNodeLocation(ref pos, "Get vendor-Node location"))
                        {
                            vendorInfo = new VendorInfo
                            {
                                VendorType = vndType,
                                Position = pos,
                                // DisplayName = $"Node <{pos.X:N1}, {pos.Y:N1}, {pos.Z:N1}>",
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
#endif
}
