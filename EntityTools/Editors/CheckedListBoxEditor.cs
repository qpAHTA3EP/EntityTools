using EntityTools.Tools.BuySellItems;
using MyNW.Patchables.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Xml.Serialization;

namespace EntityTools.Editors
{
#if DEVELOPER
    internal class BagsListEditor : UITypeEditor , IDisposable
    {
        private readonly CheckedListBox cbx = new CheckedListBox();
#if Disable_Selector
        private object editedValue;
        private IWindowsFormsEditorService es;
        private int toolTipIndex;
#endif
        public override bool IsDropDownResizable => true;
        private ToolTip toolTip = new ToolTip() { ToolTipTitle = "Ctrl+A to select all, Ctrl+D to deselect, Ctrl+I to inverse, Ctrl+S to sort" };

        internal BagsListEditor()
        {
#if Disable_Selector
            cbx.Leave += cbx_Leave;

#endif
            cbx.KeyDown += cbx_KeyDown;
            cbx.MouseHover += cbx_ShowTooltip;
        }

        private void cbx_ShowTooltip(object sender, EventArgs e)
        {
            int toolTipIndex = cbx.IndexFromPoint(cbx.PointToClient(Control.MousePosition));
            if (toolTipIndex > -1)
            {
                toolTip.SetToolTip(cbx, cbx.Items[toolTipIndex].ToString());
            }
        }

        private void cbx_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                e.SuppressKeyPress = true;
                for (int i = 0; i < cbx.Items.Count; i++)
                {
                    cbx.SetItemChecked(i, true);
                }
            }
            if (e.Control && e.KeyCode == Keys.D)
            {
                e.SuppressKeyPress = true;
                for (int i = 0; i < cbx.Items.Count; i++)
                {
                    cbx.SetItemChecked(i, false);
                }
            }
            if (e.Control && e.KeyCode == Keys.I)
            {
                e.SuppressKeyPress = true;
                for (int i = 0; i < cbx.Items.Count; i++)
                {
                    cbx.SetItemChecked(i, !cbx.GetItemChecked(i));
                }
            }
            if (e.Control && e.KeyCode == Keys.S)
            {
                e.SuppressKeyPress = true;
                for (int i = 0; i < cbx.Items.Count; i++)
                {
                    cbx.Sorted = true;
                }
            }
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider.GetService(typeof(IWindowsFormsEditorService)) is IWindowsFormsEditorService es
                && value is BagsList bags)
            {
                foreach(InvBagIDs bagId in Enum.GetValues(typeof(InvBagIDs)))
                {
                    int ind = cbx.Items.Add(bagId);
                    cbx.SetItemChecked(ind, bags[bagId]);
                }
                es.DropDownControl(cbx);

                bags = new BagsList(null);
                foreach (InvBagIDs id in cbx.CheckedItems)
                    bags[id] = true;
            }
            return value;
        }

#if Disable_Selector
        private void LoadListBoxItems(object value)
        {
            int idx = 0;
            var dict = value as BitMat<T>;
            cbx.Items.Clear();
            foreach (var item in dict.Dictionary)
            {
                cbx.Items.Add(item.Key);
                cbx.SetItemChecked(idx, item.Value);
                idx++;
            }
            this.editedValue = value;
        }

        private void cbx_Leave(object sender, EventArgs e)
        {
            var dict = new Dictionary<T, bool>();
            for (int i = 0; i < cbx.Items.Count; i++)
            {
                if (i == 90)
                {
                    int a = 10 + i;
                }
                var item = (T)cbx.Items[i];
                dict.Add(item, cbx.GetItemChecked(i));
            }
            var listSelector = editedValue as BitMap<T>;
            listSelector.Dictionary = dict;
        } 
#endif

        public void Dispose()
        {
            cbx.Dispose();
        }
    }
#endif
#if Disable_Selector
    [Serializable]
    public class BitMap<T> where T : Enum, IEnumerable<T>
    {
        /// <summary>
        /// Список флагов-сумок
        /// </summary>
        BitArray _map = new BitArray(Enum.GetValues(typeof(T)).Length, false);

        public IEnumerable<T> Items
        {
            get
            {
                for (int i = 0; i < _map.Count; i++)
                {
                    if (_map[i])
                        yield return (T)i;
                }
            }
            set
            {
                _map.SetAll(false);
                if (value != null)
                {
                    foreach (var v in value)
                        _map[(int)v] = true;
                }
            }
        }

#if false
        [XmlIgnore]
        public Dictionary<T, bool> Dictionary
        {
            get
            {
                var value = new Dictionary<T, bool>();
                if (typeof(T).BaseType.Name == nameof(Enum))
                {
                    foreach (var s in Enum.GetNames(typeof(T)))
                    {
                        if (s != "None")
                        {
                            var item = (T)Enum.Parse(typeof(T), s);
                            value.Add(item, Items.Contains(item));
                        }
                    }
                }
                return value;
            }
            set
            {
                Items.Clear();
                foreach (var item in value)
                {
                    if (item.Value)
                    {
                        Items.Add(item.Key);
                    }
                }
            }
        }

#endif
        public bool this[T id]
        {
            get
            {
                return _map[(int)id];
            }
            set
            {
                _map[(int)id] = value;
            }

        }

        public override string ToString()
        {
            return $"{Items.Count} of {Dictionary.Count} in {typeof(T).Name}";
        }

        public BitMap<T> Clone()
        {
            BitMap<T> newSelector = new BitMap<T>();
            foreach (var m in _map)
                newSelector._map[(int)m] = true;

            return newSelector;
        }
    }

#endif
}
