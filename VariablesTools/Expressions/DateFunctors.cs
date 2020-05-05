using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NumberAstNode = VariableTools.Expressions.AstNode<double>;


namespace VariableTools.Expressions.DateFunctions
{
    /* An OLE Automation date is implemented as a floating-point number 
     * whose integral component is the number of days before or after midnight, 30 December 1899, 
     * and whose fractional component represents the time on that day divided by 24. 
     * For example, midnight, 31 December 1899 is represented by 1.0; 
     * 6 A.M., 1 January 1900 is represented by 2.25; 
     * midnight, 29 December 1899 is represented by -1.0; 
     * and 6 A.M., 29 December 1899 is represented by -1.25.

     * The base OLE Automation Date is midnight, 30 December 1899. 
     * The minimum OLE Automation date is midnight, 1 January 0100. 
     * The maximum OLE Automation Date is the same as DateTime..::.MaxValue, 
     * the last moment of 31 December 9999.

     * The ToOADate method throws an OverflowException 
     * if the current instance represents a date that is later than MinValue 
     * and earlier than midnight on January1, 0100. 
     * However, if the value of the current instance is MinValue, the method returns 0. */

    /// <summary>
    /// Возвращает текуще время 
    /// </summary>
    public class DateTimeNow : NumberAstNode
    {
        public override bool Calculate(out double result)
        {
            result = DateTime.Now.ToOADate();
            return true;
        }
    }

    /// <summary>
    /// Количество дней с начально даты 31.12.1899
    /// </summary>
    public class DaysNumber : UnOperator<double>
    {
        public DaysNumber(NumberAstNode dateTime)
        {
            Operand = dateTime;
        }

        public override bool Calculate(out double result)
        {
            if(Operand.Calculate(out double AOdateTime))
            {
                if(AOdateTime > 0)
                    result = Math.Floor(AOdateTime);
                else result = 0;

                return true;
            }
            result = 0;
            return false;
        }

        public override string Description(int indent = 0)
        {
            sb.Clear();
            sb.Insert(sb.Length, Parser.Indent, indent).Append(GetType().Name).Append(" {Value=").Append(Result).Append("}");
            return sb.ToString();
        }
    }

    /// <summary>
    /// Количество часов с начально даты 31.12.1899
    /// </summary>
    public class HoursNumber : UnOperator<double>
    {
        public HoursNumber(NumberAstNode dateTime)
        {
            Operand = dateTime;
        }

        public override bool Calculate(out double result)
        {
            if (Operand.Calculate(out double AOdateTime))
            {
                if (AOdateTime > 0)
                    result = Math.Floor(AOdateTime * 24);
                else result = 0;

                return true;
            }
            result = 0;
            return false;
        }

        public override string Description(int indent = 0)
        {
            sb.Clear();
            sb.Insert(sb.Length, Parser.Indent, indent).Append(GetType().Name).Append(" {Value=").Append(Result).Append("}");
            return sb.ToString();
        }
    }

    /// <summary>
    /// Количество минут с начально даты 31.12.1899
    /// </summary>
    public class MinutesNumber : UnOperator<double>
    {
        public MinutesNumber(NumberAstNode dateTime)
        {
            Operand = dateTime;
        }

        public override bool Calculate(out double result)
        {
            if (Operand.Calculate(out double AOdateTime))
            {
                if (AOdateTime > 0)
                    result = Math.Floor(AOdateTime * 1440);
                else result = 0;

                return true;
            }
            result = 0;
            return false;
        }

        public override string Description(int indent = 0)
        {
            sb.Clear();
            sb.Insert(sb.Length, Parser.Indent, indent).Append(GetType().Name).Append(" {Value=").Append(Result).Append("}");
            return sb.ToString();
        }
    }

    /// <summary>
    /// Количество секунд с начально даты 31.12.1899
    /// </summary>
    public class SecondsNumber : UnOperator<double>
    {
        public SecondsNumber(NumberAstNode dateTime)
        {
            Operand = dateTime;
        }

        public override bool Calculate(out double result)
        {
            if (Operand.Calculate(out double AOdateTime))
            {
                if (AOdateTime > 0)
                    result = Math.Floor(AOdateTime * 86400);
                else result = 0;

                return true;
            }
            result = 0;
            return false;
        }

        public override string Description(int indent = 0)
        {
            sb.Clear();
            sb.Insert(sb.Length, Parser.Indent, indent).Append(GetType().Name).Append(" {Value=").Append(Result).Append("}");
            return sb.ToString();
        }
    }
}
