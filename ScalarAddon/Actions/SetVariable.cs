using Astral.Logic.Classes.Map;
using AstralVariables.Editors;
using AstralVariables.Expressions;
using MyNW.Classes;
using System.ComponentModel;
using System.Drawing.Design;

namespace AstralVariables.Actions
{
    public class SetVariable : Astral.Quester.Classes.Action
    {
        private bool _variableNameOk = false;
        private string _variableName = string.Empty;


        [Editor(typeof(variableSelectUiEditor), typeof(UITypeEditor))]
        [Description("The Name of variable.\n" +
            "Идентификатор переменной.")]
        public string Variable
        {
            get => _variableName;
            set
            {
                _variableNameOk = !Parser.IsForbidden(value);
                _variableName = value;
                VariablesAddon.Variables[_variableName] = 0;
            }
        }

        private NumberExpression equation = new NumberExpression();

        [Description("An expression whose result is assigned to the {Variable}.\n" +
            "Выражение, результат которого присваивается переменной.")]
        public string Equation
        {
            get => equation.Expression;
            set
            {
                if (value != equation.Expression)
                {
                    equation.Expression = value;
                }
            }
        }

        public override string ActionLabel
        {
            get
            {
                if (_variableNameOk)
                    return $"{GetType().Name}: {_variableName} => {equation.Expression}";
                else return GetType().Name;
            }
        }

        public override bool NeedToRun => true;

        public override string InternalDisplayName => GetType().Name;

        public override bool UseHotSpots => false;

        protected override bool IntenalConditions => _variableNameOk && equation.IsValid;

        protected override Vector3 InternalDestination => new Vector3();

        protected override ActionValidity InternalValidity
        {
            get
            {
                //return new ActionValidity((equation.IsValid ? String.Empty : "Equation is incorrect\n") +
                //   (_variableNameOk ? String.Empty : "The name of the {Variable} contains forbidden symbols or equals to Function name"));

                if (equation.IsValid)
                {
                    if (_variableNameOk)
                        return new ActionValidity(/*$"equation.IsValid={equation.IsValid} && variableNameOk={_variableNameOk}"*/);
                    else return new ActionValidity("The name of the {Variable} contains forbidden symbols or equals to Function name");
                }
                else
                {
                    if (_variableNameOk)
                        return new ActionValidity("Equation is incorrect");
                    else return new ActionValidity("Equation is incorrect.\n" +
                        "The name of the {Variable} contains forbidden symbols or equals to Function name");
                }
            }
        }

        public override void GatherInfos()
        {
            //InternalReset();
        }

        public override void InternalReset()
        {
            //_variableNameOk = !Parser.IsForbidden(_variableName);
            //equation.Parse();
        }

        public override void OnMapDraw(GraphicsNW graph){ }

        public override ActionResult Run()
        {
            if (_variableNameOk && equation.IsValid)
            {
                if(equation.Calcucate(out double result))
                {
                    VariablesAddon.Variables[_variableName] = result;
#if DEBUG
                    Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"Variable {{{_variableName}}} set to '{VariablesAddon.Variables[_variableName]}' ({equation.Expression})");
#endif
                    return ActionResult.Completed;
                }
            }
            return ActionResult.Fail;
        }
    }
}