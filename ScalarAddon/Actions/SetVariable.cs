using Astral.Logic.Classes.Map;
using AstralVariables.Editors;
using AstralVariables.Expressions;
using MyNW.Classes;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;

namespace AstralVariables.Actions
{
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
                VariablesAddon.Variables[variableName] = 0;
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
                if (value.Expression != equation.Expression)
                    equation = value;
            }
        }

        public override string ActionLabel
        {
            get
            {
                if (variableNameOk)
                    return $"{GetType().Name}: {variableName} => {equation.Expression}";
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
                //return new ActionValidity((equation.IsValid ? String.Empty : "Equation is incorrect\n") +
                //   (_variableNameOk ? String.Empty : "The name of the {Variable} contains forbidden symbols or equals to Function name"));

                if (equation.IsValid)
                {
                    if (variableNameOk)
                        return new ActionValidity(/*$"equation.IsValid={equation.IsValid} && variableNameOk={_variableNameOk}"*/);
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
            if (variableNameOk && equation.IsValid)
            {
                if(equation.Calcucate(out double result))
                {
                    VariablesAddon.Variables[variableName] = result;
#if DEBUG
                    Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"Variable {{{variableName}}} set to '{VariablesAddon.Variables[variableName]}' ({equation.Expression})");
#endif
                    return ActionResult.Completed;
                }
            }
            return ActionResult.Fail;
        }
    }

    //    public class SetVariable : Astral.Quester.Classes.Action
    //    {
    //        [XmlIgnore]
    //        private bool variableNameOk = false;
    //        [XmlIgnore]
    //        private string variableName = string.Empty;


    //        [Editor(typeof(VariableSelectUiEditor), typeof(UITypeEditor))]
    //        [Description("The Name of variable.\n" +
    //            "Идентификатор переменной.")]
    //        public string Variable
    //        {
    //            get => variableName;
    //            set
    //            {
    //                variableNameOk = !Parser.IsForbidden(value);
    //                variableName = value;
    //                VariablesAddon.Variables[variableName] = 0;
    //            }
    //        }

    //        private NumberExpression equation = new NumberExpression();

    //        [Description("An expression whose result is assigned to the {Variable}.\n" +
    //            "Выражение, результат которого присваивается переменной.")]
    //        public string Equation
    //        {
    //            get => equation.Expression;
    //            set
    //            {
    //                if (value != equation.Expression)
    //                {
    //                    equation.Expression = value;
    //                }
    //            }
    //        }

    //        public override string ActionLabel
    //        {
    //            get
    //            {
    //                if (variableNameOk)
    //                    return $"{GetType().Name}: {variableName} => {equation.Expression}";
    //                else return GetType().Name;
    //            }
    //        }

    //        public override bool NeedToRun => true;

    //        public override string InternalDisplayName => GetType().Name;

    //        public override bool UseHotSpots => false;

    //        protected override bool IntenalConditions => variableNameOk && equation.IsValid;

    //        protected override Vector3 InternalDestination => new Vector3();

    //        protected override ActionValidity InternalValidity
    //        {
    //            get
    //            {
    //                //return new ActionValidity((equation.IsValid ? String.Empty : "Equation is incorrect\n") +
    //                //   (_variableNameOk ? String.Empty : "The name of the {Variable} contains forbidden symbols or equals to Function name"));

    //                if (equation.IsValid)
    //                {
    //                    if (variableNameOk)
    //                        return new ActionValidity(/*$"equation.IsValid={equation.IsValid} && variableNameOk={_variableNameOk}"*/);
    //                    else return new ActionValidity("The name of the {Variable} contains forbidden symbols or equals to Function name");
    //                }
    //                else
    //                {
    //                    if (variableNameOk)
    //                        return new ActionValidity("Equation is incorrect");
    //                    else return new ActionValidity("Equation is incorrect.\n" +
    //                        "The name of the {Variable} contains forbidden symbols or equals to Function name");
    //                }
    //            }
    //        }

    //        public override void GatherInfos()
    //        {
    //            //InternalReset();
    //        }

    //        public override void InternalReset()
    //        {
    //            //_variableNameOk = !Parser.IsForbidden(_variableName);
    //            //equation.Parse();
    //        }

    //        public override void OnMapDraw(GraphicsNW graph) { }

    //        public override ActionResult Run()
    //        {
    //            if (variableNameOk && equation.IsValid)
    //            {
    //                if (equation.Calcucate(out double result))
    //                {
    //                    VariablesAddon.Variables[variableName] = result;
    //#if DEBUG
    //                    Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"Variable {{{variableName}}} set to '{VariablesAddon.Variables[variableName]}' ({equation.Expression})");
    //#endif
    //                    return ActionResult.Completed;
    //                }
    //            }
    //            return ActionResult.Fail;
    //        }
    //    }
}