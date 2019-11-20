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
using static VariableTools.Classes.VariableCollection;

namespace VariableTools.Actions
{
    [Serializable]
    public class SetVariable : Astral.Quester.Classes.Action
    {
        [XmlIgnore]
        private bool variableNameOk = false;
        [XmlIgnore]
        private string variableName = string.Empty;
        //[XmlIgnore]
        //internal VariableContainer variableContainer = new VariableContainer();

        //[Category("Variable")]
        //[DisplayName("Variable")]
        //[TypeConverter(typeof(ExpandableObjectConverter))]
        //public VariableScopeIdentifier VarContainer { get => variableContainer; set => variableContainer = value; }


        [Editor(typeof(VariableSelectUiEditor), typeof(UITypeEditor))]
        [Description("The Name of the {Variable}.\r\n" +
            "Идентификатор (имя) переменной.\r\n" +
            "В имени переменной допускается использовние букв, цифр и символа \'_\'")]
        [Category("Variable")]
        [DisplayName("Variable Name")]
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
                        //variableContainer = null;

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
                if (VariableTools.Variables.TryGetValue(out VariableContainer variable, value))
                {
                    Save = variable.Save;
                    AccountScope = variable.AccountScope;
                    StoredValue = variable.Value;
                }
                else
                {
                    variable = new VariableContainer(0, value, AccountScope, ProfileScope);
                    if (VariableTools.Variables.TryAdd(variable))
                    {
                        Save = variable.Save;
                        AccountScope = variable.AccountScope;
                        StoredValue = variable.Value;
                    }
                }
                variableNameOk = true;
                variableName = value;
            }
        }

        [Category("Variable")]
        public AccountScopeType AccountScope { get; set; }

        [Category("Variable")]
        public ProfileScopeType ProfileScope { get; set; }

        //[Category("Variable")]
        [XmlIgnore]
        [ReadOnly(true)]
        public bool Save { get; protected set; }

        //[Category("Variable")]
        [XmlIgnore]
        [ReadOnly(true)]
        public double StoredValue{ get; protected set; }

        private NumberExpression equation = new NumberExpression();
        [Editor(typeof(EquationUiEditor), typeof(UITypeEditor))]
        [Description("An expression whose result is assigned to the {Variable}.\r\n" +
            "Выражение, результат которого присваивается переменной {Variable}.")]
        [Category("Variable assigne to:")]
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
                    if (VariableTools.Variables.TryGetValue(out VariableContainer variable, variableName, AccountScope, ProfileScope))
                    {
                        variable.Value = result;
                        Save = variable.Save;
                        StoredValue = variable.Value;
#if DEBUG
                        Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{nameof(VariableTools)}::{GetType().Name}[{ActionID}]: Variable {{{variable.Name}}}[{variable.AccountScope}, {variable.ProfileScope}] set to '{variable.Value}' (Equation = {equation.Text})");
#endif
                        return ActionResult.Completed;
                    }
                    else
                    {
                        variable = VariableTools.Variables.Add(result, variableName, AccountScope, ProfileScope);
                        if (variable != null)
                        {
#if DEBUG
                            Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{nameof(VariableTools)}::{GetType().Name}[{ActionID}]: Add variable {{{variable.Name}}}[{variable.AccountScope}, {variable.ProfileScope}] with the value '{variable.Value}' (Equation = {equation.Text})");
#endif
                            return ActionResult.Completed;
                        }
                        else
                        {
#if DEBUG
                            Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{nameof(VariableTools)}::{GetType().Name}[{ActionID}]: FAILED to set the value to the Variable {{{variableName}}}[{AccountScope}, {ProfileScope}]");
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
                    return $"{GetType().Name}: {variableName}[{AccountScope}, {ProfileScope}] := {equation.Text}";
                else return GetType().Name;
            }
        }

        public override string InternalDisplayName => string.Empty;
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