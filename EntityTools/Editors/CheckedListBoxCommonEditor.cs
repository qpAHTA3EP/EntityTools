using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Xml.Serialization;

namespace EntityTools.Editors
{

    internal class CheckedListBoxCommonEditor<T> : UITypeEditor , IDisposable
    {
        private readonly CheckedListBox cbx = new CheckedListBox();
        private object value;
        private IWindowsFormsEditorService es;
        public override bool IsDropDownResizable => true;
        private ToolTip toolTip;
        private int toolTipIndex;

        internal CheckedListBoxCommonEditor()
        {
            cbx.Leave += cbx_Leave;
            cbx.KeyDown += cbx_KeyDown;
            cbx.MouseHover += cbx_ShowTooltip;
            toolTip = new ToolTip() { ToolTipTitle = "Ctrl+A to select all, Ctrl+D to deselect, Ctrl+I to inverse, Ctrl+S to sort" };
        }

        private void cbx_ShowTooltip(object sender, EventArgs e)
        {
            toolTipIndex = cbx.IndexFromPoint(cbx.PointToClient(Control.MousePosition));
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
            es = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            if (es != null)
            {
                LoadListBoxItems(value);
                es.DropDownControl(cbx);
            }
            return this.value;
        }

        private void LoadListBoxItems(object value)
        {
            int idx = 0;
            var dict = value as CheckedListBoxCommonSelector<T>;
            cbx.Items.Clear();
            foreach (var item in dict.Dictionary)
            {
                cbx.Items.Add(item.Key);
                cbx.SetItemChecked(idx, item.Value);
                idx++;
            }
            this.value = value;
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
            var listSelector = value as CheckedListBoxCommonSelector<T>;
            listSelector.Dictionary = dict;
        }

        public void Dispose()
        {
            cbx.Dispose();
        }
    }

    [Serializable]
    public class CheckedListBoxCommonSelector<T>
    {
        public List<T> Items { get; set; } = new List<T>();
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
        public override string ToString()
        {
            return $"{Items.Count} of {Dictionary.Count} in {typeof(T).Name}";
        }

        public CheckedListBoxCommonSelector<T> Clone()
        {
            CheckedListBoxCommonSelector<T> newSelector = new CheckedListBoxCommonSelector<T>();
            newSelector.Items.AddRange(Items);

            return newSelector;
        }
    }

}
