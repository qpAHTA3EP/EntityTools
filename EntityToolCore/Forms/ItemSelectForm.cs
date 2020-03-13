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

        public delegate object ProcessingItems(ListBox itemList);

        internal ProcessingItems FillList;
        internal ProcessingItems GetSelectedItem;

        public ItemSelectForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Выбор типа и конструирование обекта выбранного типа
        /// </summary>
        /// <typeparam name="T">базовый тип</typeparam>
        /// <returns></returns>
        public static T GetAnItem<T>(bool includeBase = true) where T: class
        {
            ItemSelectForm selectForm = 
                (includeBase) ? 
                new ItemSelectForm()
                {
                    FillList =  FillItems<T>,
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

        #region Обработчики
        private void btnSelect_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            FillList?.Invoke(ItemList);
        }

        private void Form_Shown(object sender, EventArgs e)
        {
            FillList?.Invoke(ItemList);
        }
        #endregion

        /// <summary>
        /// Добавление производных типов в список
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemList"></param>
        /// <returns></returns>
        internal static object FillDerivedItems<T>(ListBox itemList)
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
            if(itemList.Items.Count > 0)
                itemList.DisplayMember = "Name";
            return itemList.Items.Count;
        }
        /// <summary>
        /// Добавление в списко и базового и производных типов
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemList"></param>
        /// <returns></returns>
        internal static object FillItems<T>(ListBox itemList)
        {
            FillDerivedItems<T>(itemList);
            itemList.Items.Add(typeof(T));
            if(itemList.Items.Count > 0)
                itemList.DisplayMember = "Name";
            return itemList.Items.Count;
        }
        /// <summary>
        /// конструирование объекта выбранного типа
        /// </summary>
        /// <param name="itemList"></param>
        /// <returns></returns>
        internal static object GetItem(ListBox itemList)
        {
            if (itemList.SelectedItem is Type t)
                return Activator.CreateInstance(t);
            return null;
        }
    }
}