//#define DELEGATES_FILL

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AcTp0Tools.Reflection;
using DevExpress.XtraEditors;

namespace EntityCore.Forms
{
    public partial class ItemSelectForm :XtraForm
    {
        private static readonly Func<List<Type>> PluginsGetTypes = typeof(Astral.Controllers.Plugins).GetStaticFunction<List<Type>>("GetTypes");

        private Action fillListAction;

        public ItemSelectForm()
        {
            InitializeComponent();
        }

        #region GetAnInstanceOfType
        /// <summary>
        /// Выбор типа и конструирование обекта выбранного типа
        /// </summary>
        /// <typeparam name="T">базовый тип</typeparam>
        /// <returns></returns>
        public static bool GetAnInstanceOfType<T>(out T selectedValue, bool includeBase = true) where T : class
        {
            ItemSelectForm selectForm = new ItemSelectForm();
            if (includeBase)
                selectForm.fillListAction = () => FillItems<T>(selectForm.ItemList);
            else selectForm.fillListAction = () => FillDerivedItems<T>(selectForm.ItemList);

            if (selectForm.ShowDialog() == DialogResult.OK
                && selectForm.ItemList.SelectedIndex >= 0)
            {
                if (selectForm.ItemList.SelectedItem is Type type)
                {
                    selectedValue = Activator.CreateInstance(type) as T;
                    return selectedValue != null;
                }
            }
            selectedValue = default(T);
            return false;
        }
        /// <summary>
        /// Добавление производных типов в список
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemList"></param>
        /// <returns></returns>
        private static object FillDerivedItems<T>(ListBox itemList)
        {
            itemList.Items.Clear();
            itemList.DisplayMember = string.Empty;
            Type baseType = typeof(T);
            List<Type> typeList = PluginsGetTypes();
            if (typeList != null)
            {
                foreach (Type t in typeList)
                {
                    if (baseType.Equals(t.BaseType))
                    {
                        int ind = itemList.Items.Add(t);
                    }
                }
            }
            if (itemList.Items.Count > 0)
                itemList.DisplayMember = "Name";
            return itemList.Items.Count;
        }
        /// <summary>
        /// Добавление в списко и базового и производных типов
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemList"></param>
        /// <returns></returns>
        private static object FillItems<T>(ListBox itemList)
        {
            FillDerivedItems<T>(itemList);
            itemList.Items.Add(typeof(T));
            if (itemList.Items.Count > 0)
                itemList.DisplayMember = "Name";
            return itemList.Items.Count;
        }

        /// <summary>
        /// конструирование объекта выбранного типа
        /// </summary>
        /// <param name="itemList"></param>
        /// <returns></returns>
        private static object GetItem(ListBox itemList)
        {
            if (itemList.SelectedItem is Type t)
                return Activator.CreateInstance(t);
            return null;
        }
        #endregion

        #region GetAnItem
        /// <summary>
        /// Выбор элемента из списка, формируемого функтором source
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="selectedValue"></param>
        /// <returns></returns>
        public static bool GetAnItem<T>(Func<IEnumerable<T>> source, ref T selectedValue, string displayMember = "")
        {
            ItemSelectForm selectForm = new ItemSelectForm();
            T value = selectedValue;
            selectForm.fillListAction = () => {
                selectForm.ItemList.DataSource = source();
                if (value != null) selectForm.ItemList.SelectedItem = value;
            };
            selectForm.ItemList.DisplayMember = displayMember;

            if (selectForm.ShowDialog() == DialogResult.OK
                && selectForm.ItemList.SelectedIndex >= 0)
            {
                selectedValue = (T)selectForm.ItemList.SelectedItem;
                return true;
            }
            else return false;
        }

#if true
        public static bool GetAnItem<T>(Func<IEnumerable<T>> source, ref T selectedValue, ListControlConvertEventHandler itemFormater)
        {
            ItemSelectForm selectForm = new ItemSelectForm();
            T value = selectedValue;
            selectForm.fillListAction = () =>
            {
                selectForm.ItemList.DataSource = source();
                if (value != null) selectForm.ItemList.SelectedItem = value;
            };
            bool useFormatter = itemFormater != null;
            if (useFormatter)
            {
                selectForm.ItemList.FormattingEnabled = true;
                selectForm.ItemList.Format += itemFormater;
            }

            if (selectForm.ShowDialog() == DialogResult.OK
                && selectForm.ItemList.SelectedIndex >= 0)
            {
                selectedValue = (T)selectForm.ItemList.SelectedItem;
                return true;
            }
            else
            {
                return false;
            }
        } 
#endif
        #endregion

        #region Обработчики
        private void btnSelect_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            fillListAction?.Invoke();
        }

            private void Form_Shown(object sender, EventArgs e)
        {
            fillListAction?.Invoke();
        }
        #endregion

        #region Переход к элементу начинающемуся с itemNameBuf (по нажатию клавиш) 
        string itemNameBuf = string.Empty;
        private void ItemList_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                // Сброс буфера
                itemNameBuf = string.Empty;
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
                string itemStr = string.Empty;
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
    }
}