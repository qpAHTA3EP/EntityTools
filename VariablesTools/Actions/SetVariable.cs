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
        //[XmlIgnore]
        //private string variableName = string.Empty;
        //[XmlIgnore]
        //internal VariableContainer variableContainer = new VariableContainer();

        //[Category("Variable")]
        //[DisplayName("Variable")]
        //[TypeConverter(typeof(ExpandableObjectConverter))]
        //public VariableScopeIdentifier VarContainer { get => variableContainer; set => variableContainer = value; }


        [XmlIgnore]
        private VariableKey key = new VariableKey();

        [Description("Идентификатор (имя) переменной.\n" +
            "В имени переменной допускается использовние букв, цифр и символа \'_\'\n" +
            "The Name of the {Variable}.")]
        [Category("Variable")]
        [DisplayName("Variable")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Editor(typeof(VariableSelectUiEditor), typeof(UITypeEditor))]
        public VariableKey Key
        {
            get => key;
            set
            {
                if(!ReferenceEquals(key, value))
                {
                    key = value;
                }                
            }
        }

        [Browsable(false)]
        public string Variable
        {
            get => key.Name;
            set
            {
                if (key.Name != value)
                {
                    if (Parser.CorrectForbiddenName(value, out string corrected))
                    {
                        // Имя переменной некорректно
                        // Запрашиваем замену
                        if (XtraMessageBox.Show($"Задано недопустимое имя переменно '{value}'!\n" +
                                                $"Хотите его исправить на '{corrected}'?\n" +
                                                $"The name '{value}' is incorrect! \n" +
                                                $"Whould you like to change it to '{corrected}'?",
                                                "Warring", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                        {
                            // Пользователь не согласился заменить некорректное имя переменной
                            //Save = false;
                            //AccountScope = AccountScopeType.Global;
                            //ProfileScope = ProfileScopeType.Common;
                            //StoredValue = 0;
                            //variableContainer = null;

                            key.Name = value;
                            variableNameOk = false;
                        }
                        else
                        {
                            // Пользователь согласился заменить имя переменной на корректное
                            key.Name = corrected;
                            variableNameOk = true;
                        }
                    }
                    else
                    {
                        key.Name = value;
                        variableNameOk = true;
                    }
                }
            }
        }

        //[Category("Variable")]
        //public AccountScopeType AccountScope { get; set; }

        //[Category("Variable")]
        //public ProfileScopeType ProfileScope { get; set; }

        //[Category("Variable")]
        //[XmlIgnore]
        //[ReadOnly(true)]
        //public bool Save { get; protected set; }

        //[Category("Variable")]
        //[XmlIgnore]
        //[ReadOnly(true)]
        //public double StoredValue { get; protected set; }

        private NumberExpression equation = new NumberExpression();

        [Editor(typeof(EquationUiEditor), typeof(UITypeEditor))]
        [Description("An expression whose result is assigned to the {Variable}.\r\n" +
                     "Выражение, результат которого присваивается переменной {Variable}.")]
        [Category("Value that assigns to the Variable")]
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
                if (equation.Calcucate(out double result))
                {
                    // Реализация через Dictionary<string, double>
                    //if (VariableTools.Variables.ContainsKey(variableName))
                    //    VariableTools.Variables[variableName] = result;
                    //else VariableTools.Variables.Add(variableName, result);
#if DEBUG
                    if (VariableTools.DebugMessage)
                        Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{nameof(VariableTools)}::{GetType().Name}[{ActionID}]: Equation '{equation.Text}' result is {result}.");
#endif

                    // Реализация через VariableCollection
                    //if (VariableTools.Variables.TryGetValue(out VariableContainer variable, variableName, AccountScope, ProfileScope))
                    if (VariableTools.Variables.TryGetValue(out VariableContainer variable, Key))
                    {
                        variable.Value = result;
                        //Save = variable.Save;
                        //StoredValue = variable.Value;
#if DEBUG
                        if (VariableTools.DebugMessage)
                            Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{nameof(VariableTools)}::{GetType().Name}[{ActionID}]: Variable {{{variable.Name}}}[{variable.AccountScope}, {variable.ProfileScope}] set to '{variable.Value}' (Equation = {equation.Text})");
#endif
                        return ActionResult.Completed;
                    }
                    else
                    {
                        //variable = VariableTools.Variables.Add(result, variableName, AccountScope, ProfileScope);
                        variable = VariableTools.Variables.Add(result, Key.Name, Key.AccountScope, Key.ProfileScope);
                        if (variable != null)
                        {
#if DEBUG
                            if (VariableTools.DebugMessage)
                                //Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{nameof(VariableTools)}::{GetType().Name}[{ActionID}]: Add variable {{{variable.Name}}}[{variable.AccountScope}, {variable.ProfileScope}] with the value '{variable.Value}' (Equation = {equation.Text})");
                                Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{nameof(VariableTools)}::{GetType().Name}[{ActionID}]: Add variable {{{variable.ToString()}}} with the value '{variable.Value}' (Equation = {equation.Text})");
#endif
                            return ActionResult.Completed;
                        }
                        else
                        {
#if DEBUG
                            if(VariableTools.DebugMessage)
                                //Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{nameof(VariableTools)}::{GetType().Name}[{ActionID}]: FAILED to set the value to the Variable {{{variableName}}}[{AccountScope}, {ProfileScope}]");
                                Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{nameof(VariableTools)}::{GetType().Name}[{ActionID}]: FAILED to set the value to the Variable {{{Key.ToString()}}}");
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
                if (Key != null)
                    //return $"{GetType().Name}: {variableName}[{AccountScope}, {ProfileScope}] := {equation.Text}";
                    return $"{GetType().Name}: {Key.ToString()} := {equation.Text}";
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