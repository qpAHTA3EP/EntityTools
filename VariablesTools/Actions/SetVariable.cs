using Astral.Logic.Classes.Map;
using VariableTools.Editors;
using VariableTools.Expressions;
using MyNW.Classes;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;

namespace VariableTools.Actions
{
    [Serializable]
    public class SetVariable : Astral.Quester.Classes.Action
    {
        [XmlIgnore]
        private bool variableNameOk = false;
        [XmlIgnore]
        private string variableName = string.Empty;


        [Editor(typeof(VariableSelectUiEditor), typeof(UITypeEditor))]
        [Description("The Name of {Variable}.\r\n" +
            "Идентификатор (имя) переменной.\r\n" +
            "В имени переменной допускается использовние букв, цифр и символа \'_\'")]
        public string Variable
        {
            get => variableName;
            set
            {
                variableNameOk = !Parser.IsForbidden(value);
                variableName = value;
            }
        }


        
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

        public override string ActionLabel
        {
            get
            {
                if (variableNameOk)
                    return $"{GetType().Name}: {variableName} => {equation.Text}";
                else return GetType().Name;
            }
        }

        public override bool NeedToRun => true;

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

        public override ActionResult Run()
        {
            if (variableNameOk && equation.IsValid)
            {
                if(equation.Calcucate(out double result))
                {
                    // Реализация через Dictionary<string, double>
                    //if (VariablesTools.Variables.ContainsKey(variableName))
                    //    VariablesTools.Variables[variableName] = result;
                    //else VariablesTools.Variables.Add(variableName, result);
#if DEBUG
                    Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{nameof(VariablesTools)}::{GetType().Name}[{ActionID}]: Equation '{equation.Text}' result is {result}.");
#endif

                    // Реализация через VariableCollection
                    if (!VariablesTools.Variables.TryAdd(variableName, result))
                        if (VariablesTools.Variables.Add(variableName, result) == null)
                        {
#if DEBUG
                            Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{nameof(VariablesTools)}::{GetType().Name}[{ActionID}]: FAILED to set the value to the Variable {{{variableName}}}");
#endif
                            return ActionResult.Fail;
                        }
#if DEBUG
                    Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{nameof(VariablesTools)}::{GetType().Name}[{ActionID}]: Variable {{{variableName}}} set to '{VariablesTools.Variables[variableName]}' (Equation = {equation.Text})");
#endif
                    return ActionResult.Completed;
                }
#if DEBUG
                else Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{nameof(VariablesTools)}::{GetType().Name}[{ActionID}]: Equation '{equation.Text}' calculation FAILED.");
#endif
            }
            return ActionResult.Fail;
        }


        public override void GatherInfos() { }
        public override void InternalReset() { }
        public override void OnMapDraw(GraphicsNW graph) { }
    }
}