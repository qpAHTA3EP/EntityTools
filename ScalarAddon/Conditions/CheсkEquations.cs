using System.ComponentModel;
using Astral.Quester.Classes;
using System.Drawing.Design;
using Astral.Quester.Forms;
using AstralVariables.Expressions;
using System.Text;
using AstralVariables.Editors;

namespace AstralVariables.Conditions
{
    public class CheсkEquations : Astral.Quester.Classes.Condition
    {
        private NumberExpression equation1 = new NumberExpression(),
                                 equation2 = new NumberExpression();

        [Editor(typeof(variableSelectUiEditor), typeof(UITypeEditor))]
        [Description("Левая часть выражения\r\n" +
            "Left part of the Equation")]
        public string Equation1
        {
            get => equation1.Expression;
            set => equation1.Expression = value;
        }

        [Description("Правая часть выражения\r\n" +
            "Left part of the Equation")]
        public string Equation2
        {
            get => equation2.Expression;
            set => equation2.Expression = value;
        }


        [Editor(typeof(variableSelectUiEditor), typeof(UITypeEditor))]
        [Description("How to compare the result of the {Equation1} with the result of the {Equation2}.\r\n" +
            "Тип cопоставления результатов выражений {Equation1} и {Equation2}")]
        public Condition.Relation Sign { get; set; }

        public override bool IsValid
        {
            get
            {
                if ( equation1.Calcucate(out double res1) && equation2.Calcucate(out double res2) )
                { 
                    switch(Sign)
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
                    sb.AppendLine().Append("Equation1 is not valid: ").Append(equation1.Expression);
                else
                {
                    if (equation1.Calcucate(out double res1))
                        sb.AppendLine().Append("Equation1 '").Append(equation1.Expression).Append("' has the result: ").Append(res1);
                    else sb.AppendLine().Append("Fail while calculate Equation1 '").Append(equation1.Expression).Append('\'');
                }
                sb.AppendLine().Append(equation1.Description());

                if (!equation2.IsValid)
                    sb.AppendLine().Append("Equation2 is not valid: ").Append(equation2.Expression);
                else
                {
                    if (equation2.Calcucate(out double res2))
                        sb.AppendLine().Append("Equation2 '").Append(equation2.Expression).Append("' has the result: ").Append(res2);
                    else sb.AppendLine().Append("Fail while calculate Equation2 '").Append(equation2.Expression).Append('\'');
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