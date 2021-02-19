using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using Astral.Logic.NW;
using Astral.Quester.Classes;
using Astral.Quester.Forms;
using DevExpress.XtraEditors;
using EntityTools.Enums;
using EntityTools.Tools.BuySellItems;
using MyNW.Classes;
using MyNW.Internals;
//using static EntityTools.Reflection.PrivateConsructor;

namespace EntityTools.Editors
{
#if DEVELOPER
    internal class VendorInfoEditor : UITypeEditor
    {
        private static readonly VendorType[] displayedVendors = { //VendorType.Auto,
                                                                    VendorType.Normal,
                                                                    VendorType.ArtifactVendor,
                                                                    VendorType.RemoteVendor,
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
            VendorType vndType = VendorType.None;

            if (EntityTools.Core.GUIRequest_Item(() => displayedVendors, ref vndType))
            {
                switch (vndType)
                {
                    case VendorType.None:
                        return false;
                    case VendorType.Auto:
                        return false;
                    case VendorType.Normal:
                        NPCInfos giver = null;
                        if (EntityTools.Core.GUIRequest_NPCInfos(ref giver))
                        {
                            Entity betterEntityToInteract = Interact.GetBetterEntityToInteract();
                            if (betterEntityToInteract.IsValid)
                            {
                                vendor = new VendorInfo
                                {
                                    VendorType = vndType,
                                    CostumeName = betterEntityToInteract.CostumeRef.CostumeName,
                                    DisplayName = betterEntityToInteract.Name,
                                    Position = betterEntityToInteract.Location.Clone(),
                                    MapName = EntityManager.LocalPlayer.MapState.MapName,
                                    RegionName = EntityManager.LocalPlayer.RegionInternalName
                                };
                                return true;
                            }
                        }
                        return false;
                    case VendorType.RemoteVendor:
                        string contactName = GetAnId.GetARemoteContact();
                        if (!string.IsNullOrEmpty(contactName))
                        {
                            vendor = new VendorInfo
                            {
                                VendorType = vndType,
                                CostumeName = contactName,
                                DisplayName = contactName
                            };
                            if ((XtraMessageBox.Show("Call it now ?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))
                            {
                                foreach (RemoteContact remoteContact in EntityManager.LocalPlayer.Player.InteractInfo.RemoteContacts)
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
                    default:
                        vendor = new VendorInfo
                        {
                            VendorType = vndType,
                            CostumeName = vndType.ToString(),
                            DisplayName = vndType.ToString()
                        };
                        return true;
                }
            }
            return false;
        }
    }
#endif
}
