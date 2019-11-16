using Astral.Logic.Classes.Map;
using VariableTools.Editors;
using VariableTools.Expressions;
using MyNW.Classes;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;
using VariableTools.Classes;
using DevExpress.XtraEditors;
using System.Windows.Forms;

namespace VariableTools.Actions
{
    [Serializable]
    public class SetVariable : Astral.Quester.Classes.Action
    {
        [XmlIgnore]
        private bool variableNameOk = false;
        [XmlIgnore]
        private string variableName = string.Empty;
        [XmlIgnore]
        internal VariableContainer variableContainer = new VariableContainer();

        [Category("Variable")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public VariableContainer VarContainer { get => variableContainer; set => variableContainer = value; }


        [Editor(typeof(VariableSelectUiEditor), typeof(UITypeEditor))]
        [Description("The Name of the {Variable}.\r\n" +
            "Идентификатор (имя) переменной.\r\n" +
            "В имени переменной допускается использовние букв, цифр и символа \'_\'")]
        [Category("Variable")]
        public string Variable
        {
            get => variableName;
            set
            {
                if (Parser.CorrectForbiddenName(value, out string corrected))
                {
                    // Имя переменной некорректно
                    // Запрашиваем замену
                    if (XtraMessageBox.Show($"The name '{value}' is incorrect! \n" +
                        $"Whold you like to change it to '{corrected}'?",
                        "Warring", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    {
                        // Пользователь не согласился заменить некорректное имя переменной
                        Save = false;
                        AccountScope = AccountScopeType.Global;
                        StoredValue = 0;

                        variableNameOk = false;
                        variableName = value;
                        return;
                    }
                    else
                    {
                        // Пользователь согласился заменить имя переменной на корректное
                        value = corrected;
                    }
                }

                // value корректное, или заменено на исправленное
                if (VariableTools.Variables.TryGetValue(out variableContainer, value))
                {
                    Save = variableContainer.Save;
                    AccountScope = variableContainer.AccountScope;
                    StoredValue = variableContainer.Value;
                }
                else
                {
                    variableContainer = VariableTools.Variables.TryAdd(0, value);
                    if (variableContainer != null)
                    {
                        Save = variableContainer.Save;
                        AccountScope = variableContainer.AccountScope;
                    }
                }
                variableNameOk = true;
                variableName = value;
            }
        }

        [Category("Variable")]
        [XmlIgnore]
        public AccountScopeType AccountScope { get; protected set; }

        [Category("Variable")]
        [XmlIgnore]
        public bool ProfileScope { get; protected set; }

        [Category("Variable")]
        [XmlIgnore]
        public bool Save { get; protected set; }

        [Category("Variable")]
        [XmlIgnore]
        public double StoredValue{ get; protected set; }

        private NumberExpression equation = new NumberExpression();
        [Editor(typeof(EquationUiEditor), typeof(UITypeEditor))]
        [Description("An expression whose result is assigned to the {Variable}.\r\n" +
            "Выражение, результат которого присваивается переменной {Variable}.")]
        public NumberExpression Equation
        {
            get => equation;
            set
            {
                if (value.Text != equation.Text)
                    equation = value;
            }
        }

        public override bool NeedToRun => true;

        public override ActionResult Run()
        {
            if (variableNameOk && equation.IsValid)
            {
                if(equation.Calcucate(out double result))
                {
                    // Реализация через Dictionary<string, double>
                    //if (VariableTools.Variables.ContainsKey(variableName))
                    //    VariableTools.Variables[variableName] = result;
                    //else VariableTools.Variables.Add(variableName, result);
#if DEBUG
                    Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{nameof(VariableTools)}::{GetType().Name}[{ActionID}]: Equation '{equation.Text}' result is {result}.");
#endif

                    // Реализация через VariableCollection
                    if (VariableTools.Variables.TryGetValue(out variableContainer, new VariableCollection.VariableKey(variableName, VariableTools.GetScopeQualifier(AccountScope, ProfileScope))))
                    {
                        variableContainer.Value = result;
                        //variable.Save = Save;
#if DEBUG
                        Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{nameof(VariableTools)}::{GetType().Name}[{ActionID}]: Variable {{{variableContainer.Name}}}({variableContainer.AccountScope}) set to '{variableContainer.Value}' (Equation = {equation.Text})");
#endif
                        return ActionResult.Completed;
                    }
                    else
                    {
                        variableContainer = VariableTools.Variables.TryAdd(result, variableName, AccountScope, ProfileScope);
                        if (variableContainer != null)
                        {
#if DEBUG
                            Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{nameof(VariableTools)}::{GetType().Name}[{ActionID}]: Add variable {{{variableContainer.Name}}}({variableContainer.AccountScope}) with the value '{variableContainer.Value}' (Equation = {equation.Text})");
#endif
                            return ActionResult.Completed;
                        }
                        else
                        {
#if DEBUG
                            Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{nameof(VariableTools)}::{GetType().Name}[{ActionID}]: FAILED to set the value to the Variable {{{variableName}}}");
#endif
                            return ActionResult.Fail;
                        }
                    }
                }
#if DEBUG
                else Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{nameof(VariableTools)}::{GetType().Name}[{ActionID}]: Equation '{equation.Text}' calculation FAILED.");
#endif
            }
            return ActionResult.Fail;
        }


        public override string ActionLabel
        {
            get
            {
                if (variableNameOk)
                    return $"{GetType().Name}: {variableName} => {equation.Text}";
                else return GetType().Name;
            }
        }

        public override string InternalDisplayName => GetType().Name;
        public override bool UseHotSpots => false;
        protected override bool IntenalConditions => variableNameOk && equation.IsValid;
        protected override Vector3 InternalDestination => new Vector3();
        protected override ActionValidity InternalValidity
        {
            get
            {
                if (equation.IsValid)
                {
                    if (variableNameOk)
                        return new ActionValidity();
                    else return new ActionValidity("The name of the {Variable} contains forbidden symbols or equals to Function name");
                }
                else
                {
                    if (variableNameOk)
                        return new ActionValidity("Equation is incorrect");
                    else return new ActionValidity("Equation is incorrect.\n" +
                        "The name of the {Variable} contains forbidden symbols or equals to Function name");
                }
            }
        }
        public override void GatherInfos() { }
        public override void InternalReset() { }
        public override void OnMapDraw(GraphicsNW graph) { }
    }
}