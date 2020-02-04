using Astral.Logic.NW;
using Astral.Logic.UCC.Actions;
using Astral.Logic.UCC.Classes;
using Astral.Logic.UCC.Forms;
using EntityTools.Tools;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using static Astral.Logic.UCC.Ressources.Enums;

namespace EntityTools.UCC.Extensions
{
    public static class UCCEditorExtensions
    {
        /// <summary>
        /// Кэшированный экземпляр модифицированного окна Astral.Logic.UCC.Forms.AddClass, предназначенного для выбора UCC-команды
        /// </summary>
        private static AddClass addUccActionEditor = null;
        // ссылка на поле AddClass.typesList
        private static object addUccActionEditor_typesListBoxControlObj = null;
        // ссылка на поле AddClass.actions
        private static Dictionary<string, Type> addUccActionEditor_uccActionsDictionary = null;
        // ссылка на поле AddClass.valuesList
        private static object addUccActionEditor_valuesListObj = null;

        private static readonly Type uccActionEditorType = typeof(AddClass);

        /// <summary>
        /// Создание экземпляра модифицированного окна AddClass, предназначенного для выбора UCC-команды
        /// </summary>
        /// <returns></returns>
        internal static bool GetAddUCCActionEditor(ref AddClass addUccActionEditorRef)
        {
            if (addUccActionEditorRef == null)
            {
                if (addUccActionEditor != null && !addUccActionEditor.IsDisposed)
                {
                    addUccActionEditorRef = addUccActionEditor;
                    return true;
                }
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
                if (ReflectionHelper.ExecStaticMethod(typeof(Astral.Controllers.Plugins)/*ReflectionHelper.GetTypeByName("Astral.Controllers.Plugins", true)*/,
                                                        "GetTypes", new object[] { }, out object typelistObj)
                    && ReflectionHelper.GetFieldValue(editor, "typesList", out addUccActionEditor_typesListBoxControlObj)
                    && ReflectionHelper.GetFieldValue(editor, "actions", out object actionsDicObj)
                    && ReflectionHelper.GetFieldValue(editor, "valuesList", out addUccActionEditor_valuesListObj))
                {
                    // typelistObj - списко типов, заданных в плагинах
                    List<Type> typeList = typelistObj as List<Type>;
                    // actionsDicObj - словарь <имя, тип> всех доступных ucc-команд
                    addUccActionEditor_uccActionsDictionary = actionsDicObj as Dictionary<string, Type>;

                    // typesListBoxControlObj - объект AddClass.typesList типа ListBoxControl, отображающий пользователю список доступных ucc-команд
                    // Не конвертируется, поскольку не совпадают версии DevExpress в VS и в Астрале
                    // ListBoxControl typesListBoxControl = typesListBoxControlObj as ListBoxControl;
                    // Явное преобразова провоцирует ошибку
                    // ListBoxControl typesListBoxControl = (ListBoxControl)typesListBoxControlObj;

                    // Получаем доступ к списку AddClass.typesList.Items
                    ReflectionHelper.GetPropertyValue(addUccActionEditor_typesListBoxControlObj, "Items", out object typesListBoxItemsObj);

                    if (typeList != null
                        //&& typesListBoxControl != null
                        && typesListBoxItemsObj != null
                        && addUccActionEditor_uccActionsDictionary != null)
                    {
                        // Добавляем ucc-команды из плагинов в списки доступных действий
                        foreach (Type type in typeList)
                        {
                            if (type.BaseType == typeof(UCCAction))
                            {
                                addUccActionEditor_uccActionsDictionary.Add(type.Name, type);
                                //typesListBoxControl.Items.Add(type.Name);
                                if (ReflectionHelper.ExecMethod(typesListBoxItemsObj, "Add", new object[] { type.Name }, out object res))
                                    fixSucceded = true;
                            }
                        }

                        // настраиваем отображение
                        // AddClass.typesList
                        // Корректируем размер 
                        // AddClass.typesList.Size = new Size(120, 213)
                        ReflectionHelper.SetPropertyValue(addUccActionEditor_typesListBoxControlObj, "Size", new Size(120, 213), BindingFlags.Default, true);
                        // Корректируем привязку к полям
                        // AddClass.typesList.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
                        ReflectionHelper.SetPropertyValue(addUccActionEditor_typesListBoxControlObj, "Anchor", (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right), BindingFlags.Instance | BindingFlags.Public, true);
                        // Устанавливаем соритровку
                        // AddClass.typesList.SortOrder = SortOrder.Ascending;
                        ReflectionHelper.SetPropertyValue(addUccActionEditor_typesListBoxControlObj, "SortOrder", SortOrder.Ascending);

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
        // Метод замещающий AddClass.Show(Type t)
        internal static object Show(Type type)
        {
            if (GetUccAction(out UCCAction action))
                return action;
            else return null;
        }
        internal static bool GetUccAction(out UCCAction action, AddClass editor = null)
        {
            action = null;
            // если ссылка на редактор не задана - Пытаемся создать модифицированный редактор
            if (editor == null
                 && !GetAddUCCActionEditor(ref editor))
                return false;

            try
            {
                ReflectionHelper.SetFieldValue(editor, "valid", false);
                editor.ShowDialog();
                if (ReflectionHelper.GetFieldValue(editor, "valid", out object valid)
                    && valid.Equals(false))
                    return false;
                else
                {
                    // Получает название выбранной ucc-команды
                    if (!ReflectionHelper.GetFieldValue(editor, "selecteType", out object selecteType))
                        return false;
                    // Получаем тип выбранной ucc-команды
                    if (!addUccActionEditor_uccActionsDictionary.TryGetValue(selecteType.ToString(), out Type selectedActionType))
                        return false;
                    // Создаем экземпляр ucc-команды
                    UCCAction uccAction = Activator.CreateInstance(selectedActionType) as UCCAction;


                    // Если выбрана команда типа Spell
                    if (selecteType.Equals(typeof(Spell).Name))
                    {
                        Spell spell = uccAction as Spell;

                        // Получаем название выбранного спелла
                        if (!ReflectionHelper.GetPropertyValue(addUccActionEditor_valuesListObj, "SelectedItem", out object selectedSpellDisplayName))
                            return false;

                        // Получаем идентификатор выбранного спелла
                        // Просматриваем список Power текущего персонажа, перобразовываем его описание в "отображаемое имя" с помощью метода AddClass.PowerDisplayName
                        // и сравниваем с selectedSpellDisplayName, выбранным пользователем в элементе управления AddClass.valuesList
                        Power selectedPower = EntityManager.LocalPlayer.Character.Powers.Find((Power p) =>
                                                                {
                                                                    return ReflectionHelper.ExecMethod(editor, "PowerDisplayName", new object[] { p.PowerDef }, out object currrentPowerDisplayName)
                                                                              && currrentPowerDisplayName.Equals(selectedSpellDisplayName);
                                                                });

                        if (selectedPower != null)
                        {
                            spell.SpellID = selectedPower.PowerDef.InternalName;
                            action = spell;
                            return true;
                        }

                        // Пролуем найти идентификатор спелла в Powers.CurrentPlayerClassPowers
                        PowerDef selectedPowerDef = Powers.CurrentPlayerClassPowers.Find((PowerDef pDef) =>
                                                                {
                                                                    return ReflectionHelper.ExecMethod(editor, "PowerDisplayName", new object[] { pDef }, out object currrentPowerDisplayName)
                                                                                && currrentPowerDisplayName.Equals(selectedSpellDisplayName);
                                                                });
                        if (selectedPowerDef != null)
                        {
                            spell.SpellID = selectedPowerDef.InternalName;
                            action = spell;
                            return true;
                        }

                        // Настроить команду Spell не удалось
                        return false;
                    }
                    // Если выбрана команда типа Special
                    if (selecteType.Equals(typeof(Special).Name))
                    {
                        Special special = uccAction as Special;

                        // Получаем название типа Special, выбранного пользователем
                        if (!ReflectionHelper.GetPropertyValue(addUccActionEditor_valuesListObj, "SelectedItem", out object selectedSpellDisplayName))
                            return false;

                        // Парсим название типа Special
                        if (!Enum.TryParse(selectedSpellDisplayName.ToString(), out SpecialUCCAction currentCpecialAction))
                            return false;
                        special.Action = currentCpecialAction;

                        action = special;
                        return true;
                    }

                    // Если выбрана команда типа Dodge
                    if (selecteType.Equals(typeof(Dodge).Name))
                    {
                        Dodge dodge = uccAction as Dodge;

                        // Получаем название типа Dodge, выбранного пользователем
                        if (!ReflectionHelper.GetPropertyValue(addUccActionEditor_valuesListObj, "SelectedItem", out object selectedSpellDisplayName))
                            return false;

                        // Парсим название типа Dodge
                        if (!Enum.TryParse(selectedSpellDisplayName.ToString(), out DodgeDirection currentDodgeDirection))
                            return false;
                        dodge.Direction = currentDodgeDirection;

                        action = dodge;
                        return true;
                    }

                    // Если выбрана команда типа Consumables
                    if (selecteType.Equals(typeof(Consumables).Name))
                    {
                        Consumables consumable = uccAction as Consumables;

                        // Получаем название типа Consumables, выбранного пользователем
                        if (!ReflectionHelper.GetPropertyValue(addUccActionEditor_valuesListObj, "SelectedItem", out object selectedConsumableName))
                            return false;

                        // Ищем идентификатор выбранного Consumable
                        InventorySlot consumableSlot = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.Potions).GetItems.Find((InventorySlot iSlot) => iSlot.Item.ItemDef.DisplayName.Equals(selectedConsumableName));

                        if (consumableSlot != null)
                        {
                            consumable.ItemId = consumableSlot.Item.ItemDef.InternalName;

                            action = consumable;
                            return true;
                        }

                        return false;
                    }

                    action = uccAction;
                    return true;
                }
            }
            catch
            {
                action = null;
            }
            return false;
        }


        /// <summary>
        /// Кэшированный экзмепляр модифицированного окна UCC-редактора (Astral.Logic.UCC.Forms.Editor)
        /// </summary>
        private static Editor uccEditor = null;

        /// <summary>
        /// Создание экземпляра модифицированного окна UCC-редактора (Editor)
        /// </summary>
        /// <returns></returns>
        internal static bool GetUccEditor(ref Editor uccEditorRef)
        {
            if(uccEditorRef == null)                
            {
                if (uccEditor != null && !uccEditor.IsDisposed)
                {
                    uccEditorRef = uccEditor;
                    return true;
                }
            }

            Editor editor = uccEditorRef;

            if (editor == null || editor.IsDisposed)
            {
                // Создаем экземпляр окна
                editor = new Editor(Astral.Logic.UCC.API.CurrentProfile);
            }

            // Замена обработчика кнопки Editor.btn_addAction
            // Получаем доступ к кнопке 
            if (editor != null && ReflectionHelper.GetFieldValue(editor, "btn_addAction", out object btnAddActionObj))
            {
                // Отписываем старый обработчик от события
                if (btnAddActionObj != null && ReflectionHelper.UnsubscribeEvent(btnAddActionObj, "Click", editor, "btn_addAction_Click", false, BindingFlags.Default, BindingFlags.Instance | BindingFlags.NonPublic))
                {
                    // Подписываем собственный обработчик к событию
                    if(ReflectionHelper.SubscribeEventStatic(btnAddActionObj, "Click", typeof(UCCEditorExtensions), nameof(btn_addAction_Click), false, BindingFlags.Default, BindingFlags.Static | BindingFlags.NonPublic))
                    {
                        uccEditorRef = editor;
                        uccEditor = editor;
                        return true;                        
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Модифицированный обработчик кнопки Editor.btn_addAction
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal static void btn_addAction_Click(object sender, EventArgs e)
        {
            // Пытаемся получить модифицированный редактор
            Editor editor = null;
            if ( GetUccEditor(ref editor)
                // Вызываем окно добавления ucc-Команды
                && GetUccAction(out UCCAction uccAction))
            {
                // ucc-команда пустая
                if (uccAction == null)
                    return;

                //  Добавляем новую ucc-команду
                //if ( ReflectionHelper.GetPropertyValue(editor, "CurrentMomentList", out object CurrentMomentListObj, BindingFlags.Instance | BindingFlags.NonPublic)
                //    && CurrentMomentListObj is List<UCCAction> CurrentMomentList)
                //{
                //    CurrentMomentList.Add(uccAction);
                //    if (ReflectionHelper.ExecMethod(editor, "refreshActionList", new object[] { }, out object res)
                //        && ReflectionHelper.GetFieldValue(editor, "actionList", out object actionList))
                //        ReflectionHelper.SetPropertyValue(actionList, "SelectedItem", uccAction, BindingFlags.Instance);
                //}    
                ReflectionHelper.GetFieldValue(editor, "profile", out object profile, BindingFlags.Instance | BindingFlags.NonPublic);
                ReflectionHelper.GetPropertyValue(editor, "CurrentMoment", out object currentMoment, BindingFlags.Instance | BindingFlags.NonPublic);
                ReflectionHelper.ExecMethod(profile, "getActionList", new object[] { currentMoment }, out object currentActionListObj);
                if (profile != null
                    && currentMoment != null
                    && currentActionListObj is List<UCCAction> currentActionList)
                {
                    currentActionList.Add(uccAction);
                    ReflectionHelper.ExecMethod(editor, "refreshActionList", new object[] { }, out object res);
                    ReflectionHelper.GetFieldValue(editor, "actionList", out object actionList);
                    ReflectionHelper.SetPropertyValue(actionList, "SelectedItem", uccAction, BindingFlags.Instance | BindingFlags.Public, true);
                }

            }
        }
    }
}
