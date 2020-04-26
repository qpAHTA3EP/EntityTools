using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NumberAstNode = VariableTools.Expressions.AstNode<double>;


namespace VariableTools.Expressions
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
     * However, if the value of the current instance is MinValue, the method returns 0.  */

    public static class DateFunctors
    {
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

        public class DaysNumber : UnOperator<double>
        {
            DateTime dateTime = new DateTime();

            public DaysNumber(NumberAstNode dateTime)
            {
                Operand = dateTime;
            }

            public override bool Calculate(out double result)
            {
                if(Operand.Calculate(out double AOdateTime))
                {
                    //DateTime.FromOADate(dateTime);

                    result = DateTime.Now.ToOADate();
                    return true;
                }
                result = 0;
                return false;
            }
        }
    }
}
