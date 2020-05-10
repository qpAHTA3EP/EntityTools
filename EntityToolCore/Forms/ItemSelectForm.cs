//#define DELEGATES_FILL

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Astral.Controllers;
using System.Reflection;
using EntityTools.Reflection;
using Astral.Logic.UCC.Classes;

namespace EntityCore.Forms
{
    public partial class ItemSelectForm :XtraForm
    {
        private static readonly Func<List<Type>> PluginsGetTypes = typeof(Astral.Controllers.Plugins).GetStaticFunction<List<Type>>("GetTypes");

#if DELEGATES_FILL
        public delegate object ProcessingItems(ListBox itemList);

        private ProcessingItems FillList;
        private ProcessingItems GetSelectedItem;

#endif
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
#if DELEGATES_FILL
        {
            ItemSelectForm selectForm =
                (includeBase) ?
                new ItemSelectForm()
                {
                    FillList = FillItems<T>,
                    GetSelectedItem = GetItem
                } :
                new ItemSelectForm()
                {
                    FillList = FillDerivedItems<T>,
                    GetSelectedItem = GetItem
                };

            if (selectForm.ShowDialog() == DialogResult.OK)
                return selectForm.GetSelectedItem?.Invoke(selectForm.ItemList) as T;
            else return null;
        }
#else
        {
            ItemSelectForm selectForm = new ItemSelectForm();
            if (includeBase)
                selectForm.fillListAction = () => FillItems<T>(selectForm.ItemList);
            else selectForm.fillListAction = () => FillDerivedItems<T>(selectForm.ItemList);

            if (selectForm.ShowDialog() == DialogResult.OK
                && selectForm.ItemList.SelectedIndex >= 0)
            {
                selectedValue = selectForm.ItemList.SelectedItem as T;
                return selectedValue != null;
            }
            else
            {
                selectedValue = default(T);
                return false;
            }
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
#endif
        #endregion

#if DELEGATES_FILL
        public static object GetAnItem(ProcessingItems fill, ProcessingItems get)
        {
            ItemSelectForm selectForm = new ItemSelectForm()
            {
                FillList = fill,
                GetSelectedItem = get
            };
            if (selectForm.ShowDialog() == DialogResult.OK)
                return selectForm.GetSelectedItem?.Invoke(selectForm.ItemList);
            else return null;
        } 
#endif

        #region GetAnItem
        /// <summary>
        /// Выбор элемента из списка, формируемого функтором source
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="selectedValue"></param>
        /// <returns></returns>
        public static bool GetAnItem<T>(Func<IEnumerable<T>> source, ref T selectedValue)
        {
            ItemSelectForm selectForm = new ItemSelectForm();
            T value = selectedValue;
            selectForm.fillListAction = () => {
                    selectForm.ItemList.DataSource = source();
                    if (value != null) selectForm.ItemList.SelectedItem = value;
                };
#if DELEGATES_FILL
            selectForm.FillList = null;
            selectForm.GetSelectedItem = null; 
#endif

            if (selectForm.ShowDialog() == DialogResult.OK
                && selectForm.ItemList.SelectedIndex >= 0)
            {
                selectedValue = (T)selectForm.ItemList.SelectedItem;
                return true;
            }
            else
            {
                //selectedValue = default(T);
                return false;
            }
        } 
        #endregion

        #region Обработчики
        private void btnSelect_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
#if DELEGATES_FILL
            FillList?.Invoke(ItemList);
#else
            fillListAction?.Invoke();
#endif
        }

            private void Form_Shown(object sender, EventArgs e)
        {
#if DELEGATES_FILL
            FillList?.Invoke(ItemList);
#else
            fillListAction?.Invoke();
#endif
        }
        #endregion
    }
}