using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Astral;
using Astral.Logic.Classes.Map;
using Astral.Logic.NW;
using Astral.Quester.Classes;
using Astral.Quester.Forms;
using Astral.Quester.UIEditors;
using Astral.Quester.UIEditors.Forms;
using DevExpress.XtraEditors;
using MyNW.Classes;
using MyNW.Internals;

namespace QuesterAssistant.Actions
{
    public class BuyItemRemote : Astral.Quester.Classes.Action
    {
        public override string ActionLabel => GetType().Name;
        //public override string Category => Core.Category;
        public override bool NeedToRun => true;
        public override string InternalDisplayName => GetType().Name;
        public override bool UseHotSpots => false;
        protected override bool IntenalConditions => true;
        protected override Vector3 InternalDestination => new Vector3();

        public BuyItemRemote()
        {
            Dialogs = new List<string>();
            RemoteContact = string.Empty;
            BuyOptions = new List<BuyItemsOption>();
        }

        public override void GatherInfos()
        {
            RemoteContact = GetAnId.GetARemoteContact();
            
            if (!string.IsNullOrEmpty(RemoteContact) && 
                (XtraMessageBox.Show("Call it now ?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))
            {
                foreach (RemoteContact remoteContact in EntityManager.LocalPlayer.Player.InteractInfo.RemoteContacts)
                {
                    if (remoteContact.ContactDef == RemoteContact)
                    {
                        remoteContact.Start();
                    }
                }
            }
           
            if (XtraMessageBox.Show("Add a dialog ? (open the dialog window before)", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DialogEdit.Show(Dialogs);
            }
        }
        public override void InternalReset() { }
        public override void OnMapDraw(GraphicsNW graph) { }

        [Editor(typeof(DialogEditor), typeof(UITypeEditor))]
        [Category("Purchase")]
        public List<string> Dialogs { get; set; }

        [Editor(typeof(RemoteContactEditor), typeof(UITypeEditor))]
        [Category("Purchase")]
        public string RemoteContact { get; set; }

        [Editor(typeof(BuyOptionsEditor), typeof(UITypeEditor))]
        [Category("Purchase")]
        public List<BuyItemsOption> BuyOptions { get; set; }

        protected override ActionValidity InternalValidity
        {
            get
            {
                if (string.IsNullOrEmpty(RemoteContact))
                    return new ActionValidity("No remote contact set.");

                if(BuyOptions == null || BuyOptions.Count == 0)
                    return new ActionValidity("Items to buy are not specified.");

                return new ActionValidity();
            }
        }

        public override ActionResult Run()
        {
            foreach (RemoteContact remoteContact in EntityManager.LocalPlayer.Player.InteractInfo.RemoteContacts)
            {
                if (remoteContact.ContactDef == RemoteContact)
                {
                    remoteContact.Start();
                    Interact.WaitForInteraction();
                    if (Dialogs.Count > 0)
                    {
                        Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(5000);
                        while (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Options.Count == 0)
                        {
                            if (timeout.IsTimedOut)
                            {
                                //IL_107:
                                return ActionResult.Fail;
                            }
                            Thread.Sleep(100);
                        }
                        Thread.Sleep(500);
                        using (List<string>.Enumerator dialogItem = this.Dialogs.GetEnumerator())
                        {
                            while (dialogItem.MoveNext())
                            {
                                string key = dialogItem.Current;
                                EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.SelectOptionByKey(key, "");
                                Thread.Sleep(1000);
                            }
                            //goto IL_10B;
                        }
                        //goto IL_107;
                    }

                    foreach (BuyItemsOption buyItemsOption in this.BuyOptions)
                    {
                        foreach (StoreItemInfo storeItemInfo in EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.StoreItems)
                        {
                            if (storeItemInfo.CanBuyError == 0u && buyItemsOption.ItemId == storeItemInfo.Item.ItemDef.InternalName)
                            {
                                Logger.WriteLine($"Buy {buyItemsOption.Count} {storeItemInfo.Item.ItemDef.DisplayName} ...");
                                storeItemInfo.BuyItem(buyItemsOption.Count);
                                Thread.Sleep(250);
                                break;
                            }
                        }
                    }

                    EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Close();
                    return ActionResult.Completed;
                }
            }
            Logger.WriteLine("Contact not found !");
            return ActionResult.Fail;
        }
    }
}
