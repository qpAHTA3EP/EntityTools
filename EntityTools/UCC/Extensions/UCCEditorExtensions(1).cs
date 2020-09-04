using Astral.Logic.UCC.Classes;
using Astral.Logic.UCC.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace EntityTools.Tools.UCCExtensions
{
    public static class UCCEditorExtensions
    {
        /// <summary>
        /// сохраненное
        /// </summary>
        private static AddClass addUccActionEditor = null;
        private static readonly Type uccActionEditorType = typeof(AddClass);

        /// <summary>
        /// Создание экземпляра модифицированного окна AddClass, предназначенного для выбора UCC-команды
        /// </summary>
        /// <returns></returns>
        internal static bool GetAddUCCActionEditor(ref AddClass addUccActionEditorRef)
        {
            if(addUccActionEditor != null)
            {
                addUccActionEditorRef = addUccActionEditor;
                return true;
            }

            AddClass editor = addUccActionEditorRef;

            if (editor == null || editor.IsDisposed)
            {
                ConstructorInfo ctor = uccActionEditorType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(Type) }, null);
                if (ctor != null)
                {
                    // Создаем экземпляр окна AddClass 
                    editor = ctor.Invoke(new object[] { typeof(UCCAction) }) as AddClass;
                }
            }

            bool fixSucceded = false;
            if (editor != null)
            {
                // получаем список типов, заданных в плагинах
                // Astral.Controllers.Plugins.GetTypes()
                //
                // А также доступ к приватным полям класса AddClass
                // AddClass.typesList
                // AddClass.actions
                object[] args = new object[] { };
                if (ReflectionHelper.ExecStaticMethod(typeof(Astral.Controllers.Plugins)/*ReflectionHelper.GetTypeByName("Astral.Controllers.Plugins", true)*/,
                                                        "GetTypes", ref args, out object typelistObj)
                    && ReflectionHelper.GetFieldValue(editor, "typesList", out object typesListBoxControlObj)
                    && ReflectionHelper.GetFieldValue(editor, "actions", out object actionsDicObj))
                {
                    // typelistObj - списко типов, заданных в плагинах
                    List<Type> typeList = typelistObj as List<Type>;
                    // actionsDicObj - словарь <имя, тип> всех доступных ucc-команд
                    Dictionary<string, Type> actionsDic = actionsDicObj as Dictionary<string, Type>;

                    // typesListBoxControlObj - объект AddClass.typesList типа ListBoxControl, отображающий пользователю список доступных ucc-команд
                    // Не конвертируется, поскольку не совпадают версии DevExpress в VS и в Астрале
                    // ListBoxControl typesListBoxControl = typesListBoxControlObj as ListBoxControl;
                    // Явное преобразова провоцирует ошибку
                    // ListBoxControl typesListBoxControl = (ListBoxControl)typesListBoxControlObj;

                    // Получаем доступ к списку AddClass.typesList.Items
                    ReflectionHelper.GetPropertyValue(typesListBoxControlObj, "Items", out object typesListBoxItemsObj);

                    if (typeList != null
                        //&& typesListBoxControl != null
                        && typesListBoxItemsObj != null
                        && actionsDic != null)
                    {
                        // Добавляем ucc-команды из плагинов в списки доступных действий
                        foreach (Type type in typeList)
                        {
                            if (type.BaseType == typeof(UCCAction))
                            {
                                actionsDic.Add(type.Name, type);
                                //typesListBoxControl.Items.Add(type.Name);
                                object[] addItemArg = new object[] { type.Name };
                                if (ReflectionHelper.ExecMethod(typesListBoxItemsObj, "Add", ref addItemArg, out object res))
                                    fixSucceded = true;
                            }
                        }

                        // настраиваем отображение
                        // AddClass.typesList
                        // Корректируем размер 
                        // AddClass.typesList.Size = new Size(120, 213)
                        ReflectionHelper.SetPropertyValue(typesListBoxControlObj, "Size", new Size(120, 213), BindingFlags.Default, true);
                        // Корректируем привязку к полям
                        // AddClass.typesList.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
                        ReflectionHelper.SetPropertyValue(typesListBoxControlObj, "Anchor", (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right), BindingFlags.Instance | BindingFlags.Public, true);
                        // Устанавливаем соритровку
                        // AddClass.typesList.SortOrder = SortOrder.Ascending;
                        ReflectionHelper.SetPropertyValue(typesListBoxControlObj, "SortOrder", SortOrder.Ascending);

                        if (fixSucceded)
                        {
                            addUccActionEditor = editor;
                            addUccActionEditorRef = editor;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static Editor uccEditor = null;

        /// <summary>
        /// Создание экземпляра модифицированного окна UCC-редактора (Editor)
        /// </summary>
        /// <returns></returns>
        internal static Editor GetUccEditor()
        {
            if (uccEditor == null || uccEditor.IsDisposed)
            {
                // Создаем экземпляр окна
                uccEditor = new Editor(Astral.Logic.UCC.API.CurrentProfile);

                // Замена обработчика кнопки Editor.btn_addAction
                // Получаем доступ к кнопке 
                if(ReflectionHelper.GetFieldValue(uccEditor, "btn_addAction", out object btnAddActionObj))
                {
                    // Отписываем старый обработчик от события
                    if (btnAddActionObj != null && ReflectionHelper.UnsubscribeEvent(btnAddActionObj, "Click", uccEditor, "btn_addAction_Click"))
                    {
                        // Подписываем собственный обработчик к событию
                        if (ReflectionHelper.UnsubscribeEventStatic(btnAddActionObj, "Click", typeof(UCCEditorExtensions), nameof(btn_addAction_Click)))
                            return uccEditor;
                    }
                }
            }
            return null;
        }

        internal static void btn_addAction_Click(object sender, EventArgs e)
        {

        }
    }
}
