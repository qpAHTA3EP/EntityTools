using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using Astral.Logic.NW;
using Astral.Quester.Classes;
using Astral.Quester.Forms;
using Astral.Quester.UIEditors.Forms;
using DevExpress.XtraEditors;
using EntityTools.Enums;
using EntityTools.Reflection;
using EntityTools.Tools.BuySellItems;
using MyNW.Classes;
using MyNW.Internals;
//using static EntityTools.Reflection.PrivateConsructor;
using NPCInfos = Astral.Quester.Classes.NPCInfos;

namespace EntityTools.Editors
{
#if DEVELOPER
    internal class VendorInfoEditor : UITypeEditor
    {
#if Using_GetAnId_Form
        static readonly GetAnId vendorTypeSelectForm = null;
        static readonly InstanceFieldAccessor<GetAnId, System.Action> vendorTypeSelectForm_RefreshList;
        static readonly InstanceFieldAccessor<GetAnId, ListBoxControl> vendorTypeSelectForm_List; 
#else
        private static readonly VendorType[] displayedVendors = new VendorType[] { //VendorType.Auto,
                                                                          VendorType.Normal,
                                                                          VendorType.ArtifactVendor,
                                                                          VendorType.RemoteVendor,
                                                                          VendorType.VIPSealTrader,
                                                                          VendorType.VIPProfessionVendor
                                                                        };
#endif

        static VendorInfoEditor()
        {
#if Using_GetAnId_Form
            vendorTypeSelectForm = Activator<GetAnId>.CreateInstance("Select vendor", ReflectionHelper.DefaultFlags);
            if (vendorTypeSelectForm != null)
            {
                vendorTypeSelectForm_RefreshList = vendorTypeSelectForm.GetInstanceField<GetAnId, System.Action>("refreshList");
                vendorTypeSelectForm_RefreshList.Value = RefreshVendorList;

                vendorTypeSelectForm_List = vendorTypeSelectForm.GetInstanceField<GetAnId, ListBoxControl>("listBoxControl1");
            }
            else throw new NullReferenceException($"Failed to initialize '{nameof(NPCVendorInfosExtEditor)}.{nameof(vendorTypeSelectForm)}'"); 
#endif

        }

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
#if Using_GetAnId_Form
            VendorType vendor = GetVendor();
#else
            VendorType vndType = VendorType.None;
#endif

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
                                vendor = new VendorInfo()
                                {
                                    VendorType = vndType,
                                    CostumeName = betterEntityToInteract.CostumeRef.CostumeName,
                                    DisplayName = betterEntityToInteract.Name,
                                    Position = betterEntityToInteract.Location.Clone(),
                                    MapName = EntityManager.LocalPlayer.MapState.MapName,
                                    RegionName = EntityManager.LocalPlayer.RegionInternalName,
                                };
                                return true;
                            }
                        }
                        return false;
                    case VendorType.RemoteVendor:
                        string contactName = GetAnId.GetARemoteContact();
                        if (!string.IsNullOrEmpty(contactName))
                        {
                            vendor = new VendorInfo()
                            {
                                VendorType = vndType,
                                CostumeName = contactName,
                                DisplayName = contactName,
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
                        vendor = new VendorInfo()
                        {
                            VendorType = vndType,
                            CostumeName = vndType.ToString(),
                            DisplayName = vndType.ToString(),
                        };
                        return true;
                }
            }
            return false;
        }

#if Using_GetAnId_Form
        public static VendorType GetVendor()
        {
            if (vendorTypeSelectForm.ShowDialog() == System.Windows.Forms.DialogResult.OK
                && vendorTypeSelectForm_List.Value.SelectedItem is VendorType vendorType)
            {
                return vendorType;
            }
            return VendorType.None;
        }

        private static void RefreshVendorList()
        {
            if (vendorTypeSelectForm_List != null)
            {
#if false
                vendorTypeSelectForm_List.Value.Items.Clear();
#if false
                vendorTypeSelectForm_List.Value.Items.Add("Normal");
                vendorTypeSelectForm_List.Value.Items.Add("ArtifactVendor");
                vendorTypeSelectForm_List.Value.Items.Add("RemoteVendor");
                vendorTypeSelectForm_List.Value.Items.Add("VIPSummonSealTrader");
                vendorTypeSelectForm_List.Value.Items.Add("VIPProfessionVendor"); 
#endif
                vendorTypeSelectForm_List.Value.Items.Add(Enum.GetValues(typeof(VendorType))); 
#else
                vendorTypeSelectForm_List.Value.DataSource = Enum.GetValues(typeof(VendorType));
#endif
            }
        } 
#endif
    }
#endif
}
