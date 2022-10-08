using ACTP0Tools;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace EntityCore.Forms
{
    public partial class ItemSelectForm : XtraForm
    {
        /// <summary>
        /// Функтор заполнения списка нефильтрованным данными
        /// </summary>
        private Action<ListBox.ObjectCollection> dataFilling;
        /// <summary>
        ///  Функтор заполнения списка фильтрованным данными
        /// </summary>
        private Action<ListBox.ObjectCollection, string> dataFiltering;
        private object defaultValue;

        public ItemSelectForm()
        {
            InitializeComponent();
        }

        #region GetAnInstanceOfType
        /// <summary>
        /// Выбор типа и конструирование объекта выбранного типа
        /// </summary>
        public static bool GetAnInstance<T>(out T selectedValue, bool includeBase = false)
        {
            // Полный список типов не меняется после загрузки приложения
            // поэтому его обновлять нет смысла
            var data = AstralAccessors.Controllers.Plugins.GetPluginTypesDerivedFrom<T>(includeBase).ToList();

            var @this = new ItemSelectForm
            {
                dataFilling = (items) =>
                {
                    items.Clear();
                    foreach (var type in data)
                    {
                        items.Add(type);
                    }
                },
                dataFiltering = (items, filter) =>
                {
                    items.Clear();
                    foreach (var type in data)
                    {
                        if (type.Name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                            items.Add(type);
                    }
                },
                ItemList =
                {
                    DisplayMember = nameof(Type.Name)
                }
            };

            if (@this.ShowDialog() == DialogResult.OK
                && @this.ItemList.SelectedIndex >= 0)
            {
                if (@this.ItemList.SelectedItem is Type type)
                {
                    selectedValue = (T)Activator.CreateInstance(type);
                    return !Equals(selectedValue, default(T));
                }
            }
            selectedValue = default;
            return false;
        }
        #endregion

        #region GetAnItem

        /// <summary>
        /// Выбор элемента из списка, формируемого функтором <param name="source"/>
        /// </summary>
        public static bool GetAnItem<T>(Func<IEnumerable<T>> source, ref T selectedValue, string displayMember = "")
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            ItemSelectForm @this = new ItemSelectForm();
            @this.defaultValue = selectedValue;

            @this.dataFilling = items =>
            {
                items.Clear();
                foreach (var t in source())
                {
                    items.Add(t);
                }
            };

            if (!string.IsNullOrEmpty(displayMember))
            {
                // Если задан параметр displayMember, то фильтрация выполняется по свойству с данном именем
                PropertyInfo pi = typeof(T).GetProperty(displayMember);
                if (pi != null)
                {
                    @this.dataFiltering = (items, filter) =>
                    {
                        items.Clear();
                        foreach (var t in source()
                            .Where(i => pi.GetValue(i).ToString().IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0))
                        {
                            items.Add(t);
                        }
                    };
                }
                else
                {
                    // Свойство с именем displayMember не найдено, поэтому сбрасываем данный параметр
                    displayMember = string.Empty;
                }
            }
            if (string.IsNullOrEmpty(displayMember))
            {
                // Выполняем фильтрацию по строковому представлению объекта
                @this.dataFiltering = (items, filter) =>
                {
                    items.Clear();
                    foreach (var t in source().Where(i =>
                        i.ToString().IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0))
                    {
                        items.Add(t);
                    }
                };
            }

            @this.ItemList.DisplayMember = displayMember;

            if (@this.ShowDialog() == DialogResult.OK
                && @this.ItemList.SelectedIndex >= 0)
            {
                selectedValue = (T)@this.ItemList.SelectedItem;
                return !Equals(selectedValue, default(T));
            }

            return false;
        }

        /// <summary>
        /// Выбор элемента из списка, формируемого функтором <param name="source"/>
        /// При этом каждый элемент выводится в виде, отформатированном <param name="itemFormatter"/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="selectedValue"></param>
        /// <param name="itemFormatter"></param>
        /// <returns></returns>
        public static bool GetAnItem<T>(Func<IEnumerable<T>> source, ref T selectedValue, ListControlConvertEventHandler itemFormatter)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            Type type = typeof(T);

            var @this = new ItemSelectForm();
            @this.defaultValue = selectedValue;

            @this.dataFilling = items =>
            {
                items.Clear();
                foreach (var t in source())
                {
                    items.Add(t);
                }
            };

            bool useFormatter = itemFormatter != null;
            if (useFormatter)
            {
                @this.ItemList.FormattingEnabled = true;
                @this.ItemList.Format += itemFormatter;
                // Выполняем фильтрацию по отформатированному представлению объекта
                @this.dataFiltering = (items, filter) =>
                {
                    items.Clear();

                    foreach (var item in source())
                    {
                        var arg = new ListControlConvertEventArgs(item, type, null);
                        itemFormatter(item, arg);

                        if (arg.Value.ToString().IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                            items.Add(item);
                    }
                };
            }
            else
            {
                // Выполняем фильтрацию по строковому представлению объекта
                @this.dataFiltering = (items, filter) =>
                {
                    items.Clear();
                    foreach (var item in source().Where(i =>
                        i.ToString().IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0))
                    {
                        items.Add(item);
                    }
                };
            }
            if (@this.ShowDialog() == DialogResult.OK
                && @this.ItemList.SelectedIndex >= 0)
            {
                var val = @this.ItemList.SelectedItem;
                if (val != null)
                {
                    selectedValue = (T) val;
                    return !Equals(selectedValue, default(T));
                }
            }

            return false;
        }
        #endregion

        #region Обработчики
        private void btnSelect_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            var selectedItem = ItemList.SelectedItem ?? defaultValue;
            tbFilter.Text = string.Empty;

            dataFilling(ItemList.Items);
            ItemList.SelectedItem = selectedItem;
        }
        #endregion

        #region Переход к элементу начинающемуся с itemNameBuf (по нажатию клавиш) 
        //string itemNameBuf = string.Empty;
        private void ItemList_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                // Сброс буфера
                //itemNameBuf = string.Empty;
                e.SuppressKeyPress = true;
            }
            if (ItemList.DataSource is null && e.Control && e.KeyCode == Keys.S)
            {
                // сортировка списка невозможна, если задан DataSource
                ItemList.Sorted = !ItemList.Sorted;
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// Поиск элементов, начинающихся с нажатой буквы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemList_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetter(e.KeyChar))
            {
                char startChar = e.KeyChar;
                int startInd = 0;
                string itemStr;
                // Определяем стартовую позицию для поиска
                if (ItemList.SelectedIndex >= 0)
                {
                    itemStr = ItemList.SelectedItem.ToString();
                    if (!string.IsNullOrEmpty(itemStr) && itemStr[0] == startChar)
                        startInd = ItemList.SelectedIndex + 1;
                }
                // Ищем следующий элемент, начинающийся с данной буквы
                // в диапазоне [startInd, Count)
                for (int ind = startInd; ind < ItemList.Items.Count; ind++)
                {
                    itemStr = ItemList.Items[ind].ToString();
                    if (!string.IsNullOrEmpty(itemStr) && itemStr[0] == startChar)
                    {
                        // Устанавливаем фокус на первый попавшийся элемент, начинающийся со startChar
                        ItemList.SelectedIndex = ind;
                        return;
                    }
                }
                if (startInd > 0)
                {
                    // В диапазоне [startInd, Count) не удалось найти подходящий элемент
                    // Ищем следующий элемент, начинающийся с данной буквы
                    // в диапазоне [0, startInd)
                    for (int ind = 0; ind < startInd; ind++)
                    {
                        itemStr = ItemList.Items[ind].ToString();
                        if (!string.IsNullOrEmpty(itemStr) && itemStr[0] == startChar)
                        {
                            // Устанавливаем фокус на первый попавшийся элемент, начинающийся со startChar
                            ItemList.SelectedIndex = ind;
                            return;
                        }
                    }
                }
            }
        }
        #endregion

        #region Filtering
        private void tbFilter_TextChanged(object sender, EventArgs e)
        {
            var filter = tbFilter.Text;
            if (string.IsNullOrEmpty(filter))
            {
                btnReload_Click(sender, null);
            }
            else
            {
                var selectedItem = ItemList.SelectedItem ?? defaultValue;

                dataFiltering(ItemList.Items, filter);

                ItemList.SelectedItem = selectedItem;
            }
        }
        #endregion
    }
}