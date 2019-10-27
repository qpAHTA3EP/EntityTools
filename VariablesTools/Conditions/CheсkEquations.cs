using System.ComponentModel;
using Astral.Quester.Classes;
using System.Drawing.Design;
using Astral.Quester.Forms;
using AstralVariables.Expressions;
using System.Text;
using AstralVariables.Editors;
using System;

namespace AstralVariables.Conditions
{
    [Serializable]
    public class CheсkEquations : Astral.Quester.Classes.Condition
    {
        private NumberExpression equation1 = new NumberExpression(),
                                 equation2 = new NumberExpression();

        [Editor(typeof(EquationUiEditor), typeof(UITypeEditor))]
        [Description("Левая часть выражения\r\n" +
            "Left part of the Equation")]
        public NumberExpression Equation1
        {
            get => equation1;
            set
            {
                if(equation1.Expression != value.Expression)
                    equation1 = value;
            }
        }

        [Editor(typeof(EquationUiEditor), typeof(UITypeEditor))]
        [Description("Правая часть выражения\r\n" +
            "Left part of the Equation")]
        public NumberExpression Equation2
        {
            get => equation2;
            set
            {
                if(equation2.Expression != value.Expression)
                    equation2 = value;
            }
        }


        [Description("How to compare the result of the {Equation1} with the result of the {Equation2}.\r\n" +
            "Тип cопоставления результатов выражений {Equation1} и {Equation2}")]
        public Condition.Relation Sign { get; set; }

        public override bool IsValid
        {
            get
            {
                if (equation1.Calcucate(out double res1) && equation2.Calcucate(out double res2))
                {
                    switch (Sign)
                    {
                        case Relation.Equal:
                            return res1 == res2;
                        case Relation.NotEqual:
                            return res1 != res2;
                        case Relation.Inferior:
                            return res1 < res2;
                        case Relation.Superior:
                            return res1 > res2;
                    }
                }

                return false;
            }
        }

        public override void Reset() { }

        public override string TestInfos
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                if (!equation1.IsValid)
                    sb.Append("Equation1 is not valid: ").Append(equation1.Expression).AppendLine();
                else
                {
                    if (equation1.Calcucate(out double res1))
                        sb.Append("Equation1 '").Append(equation1.Expression).Append("' has the result: ").Append(res1).AppendLine();
                    else sb.Append("Fail while calculate Equation1 '").Append(equation1.Expression).Append('\'').AppendLine();
                }
                sb.Append(equation1.Description()).AppendLine();

                if (!equation2.IsValid)
                    sb.Append("Equation2 is not valid: ").Append(equation2.Expression).AppendLine();
                else
                {
                    if (equation2.Calcucate(out double res2))
                        sb.Append("Equation2 '").Append(equation2.Expression).Append("' has the result: ").Append(res2).AppendLine();
                    else sb.Append("Fail while calculate Equation2 '").Append(equation2.Expression).Append('\'').AppendLine();
                }
                sb.AppendLine().Append(equation2.Description());

                return sb.ToString();
            }
        }

        public override string ToString()
        {
            return $"{GetType().Name}: ({equation1.Expression}) {Sign} ({equation2.Expression})";
        }
    }
}