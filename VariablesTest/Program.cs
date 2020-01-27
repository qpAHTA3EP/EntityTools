using VariableTools.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NumberAstNode = VariableTools.Expressions.AstNode<double>;

namespace VariablesTest
{
    class Program
    {
        static void Main(string[] args)
        {
            #region ParseNumber
            //string[] expressions = { "12345.67",
            //                         "12345,67",
            //                         "12345,",
            //                         "29348487;",
            //                         "12345)",
            //                         "12345,)",
            //                         "341(",
            //                         "341.(",
            //                         "12345 6789",
            //                         "12345+",
            //                         "2_sdfe_w234",
            //                         "sdfe_w234",
            //                         "$12316",
            //                         "`s;lsdkg"
            //                       };
            //ParseMethod testedParseMethod = AstralVariables.Expressions.NumberExpression.ParseNumber;
            //Console.WriteLine("===================ParseNumber==================");
            #endregion
            #region ParseVariable
            //string[] expressions = { "2_sdfe _w234",
            //                         "sdfe_w234",
            //                         "$12s_316",
            //                         "p12s_316(",
            //                         "u12s_316)",
            //                         "`s;lsdkg",
            //                         "ItemCount",
            //                         "sdfl_ + ldfj22",
            //                         "*gfrrr"
            //                       };
            //ParseMethod testedParseMethod = AstralVariables.Expressions.NumberExpression.ParseVariable;
            //Console.WriteLine("===================ParseVariable================");
            #endregion
            #region ParseValue
            //string[] expressions = { "12345.67",
            //                         "12345,67",
            //                         "12345,",
            //                         "29348487;",
            //                         "12345)",
            //                         "12345,)",
            //                         "341(",
            //                         "341.(",
            //                         "12345 6789",
            //                         "12345+",
            //                         "2_sdfe_w234",
            //                         "sdfe_w234",
            //                         "$12316",
            //                         "`s;lsdkg",
            //                         "sdfl_ + ldfj22",
            //                         "*gfrrr"
            //                       };
            //ParseMethod testedParseMethod = AstralVariables.Expressions.NumberExpression.ParseValue;
            //Console.WriteLine("===================ParseVariable================");
            #endregion
            #region ParseItemCount
            //string[] expressions = { /*"12345.67",
            //                         "2_sdfe_w234",
            //                         "sdfe_w234",
            //                         "$12316",
            //                         "`s;lsdkg",
            //                         "sdfl_ + ldfj22",
            //                         "*gfrrr",*/
            //                         "ItemCount",
            //                         "ItemCout(sdf)",
            //                         "ItemCount()",
            //                         "ItemCount(",
            //                         "ItemCount)",
            //                         "ItemCount(e)",
            //                         "ItemCount(5)",
            //                         "ItemCount(58d)",
            //                         "ItemCount(.*df)",
            //                       };
            //ParseMethod testedParseMethod = AstralVariables.Expressions.NumberExpression.ParseItemCount;
            //Console.WriteLine("===================ParseItemCount===============");
            #endregion
            #region ParseNumericCount
            //string[] expressions = { "12345.67",
            //                         "2_sdfe_w234",
            //                         "sdfe_w234",
            //                         "$12316",
            //                         "`s;lsdkg",
            //                         "sdfl_ + ldfj22",
            //                         "*gfrrr",
            //                         "NumericCount",
            //                         "NumerCount(sdf)",
            //                         "NumericCount()",
            //                         "NumericCount(",
            //                         "NumericCount)",
            //                         "NumericCount(e)",
            //                         "NumericCount(5)",
            //                         "NumericCount(58d)",
            //                         "NumericCount(.*df)",
            //                       };
            //ParseMethod testedParseMethod = AstralVariables.Expressions.NumberExpression.ParseNumericCount;
            //Console.WriteLine("===================ParseNumericCount============");
            #endregion
            #region ParseRandom
            //string[] expressions = { "12345.67",
            //                         "2_sdfe_w234",
            //                         "sdfe_w234",
            //                         "$12316",
            //                         "`s;lsdkg",
            //                         "sdfl_ + ldfj22",
            //                         "*gfrrr",
            //                         "Random( )",
            //                         "Rancom(",
            //                         "Random )",
            //                         "Random(e)",
            //                         "Random (  5)",
            //                         "Random (58d )",
            //                         "Random(*df  )",
            //                         "Random(5 + 19)",
            //                         "Random( 64*df)",
            //                         "Random( 64*(df+55) ) ",
            //                       };
            //ParseMethod testedParseMethod = AstralVariables.Expressions.NumberExpression.ParseRandom;
            //Console.WriteLine("===================ParseRandom==================");
            #endregion
            #region ParseMultiplication
            //string[] expressions = { "12345.67",
            //                         "12345,67",
            //                         "12345,",
            //                         "29348487;",
            //                         "12345)",
            //                         "12345,)",
            //                         "341(",
            //                         "341.(",
            //                         "12345 6789",
            //                         "12345+",
            //                         "2_sdfe_w234",
            //                         "sdfe_w234",
            //                         "$12316",
            //                         "12345.67",
            //                         "2_sdfe_w234",
            //                         "sdfe_w234",
            //                         "$12316",
            //                         "`s;lsdkg",
            //                         "12345.67 * ldfj22",
            //                         "ldfj22 * 12345.67",
            //                         "12345.67 * ldfj22 / 4.67",
            //                         "sdfl_ * ldfj22",
            //                         "sdfl_ / ldfj22 / 5987",
            //                         "sdfl_ * NumericCount(5)",
            //                         "NumericCount(5) * sdfl_ * NumericCount(5)",
            //                         "*gfrrr",
            //                         "NumericCount",
            //                         "NumerCount(sdf)",
            //                         "NumericCount()",
            //                         "NumericCount(",
            //                         "NumericCount)",
            //                         "NumericCount(e)",
            //                         "NumericCount(5)",
            //                         "NumericCount(58d)",
            //                         "NumericCount(.*df)",
            //                         "ItemCount",
            //                         "ItemCout(sdf)",
            //                         "ItemCount()",
            //                         "ItemCount(",
            //                         "ItemCount)",
            //                         "ItemCount(e)",
            //                         "ItemCount(5)",
            //                         "ItemCount(58d)",
            //                         "ItemCount(.*df)",
            //                         "( 5 )",
            //                         " ( asdf_3 )",
            //                         "( 654",
            //                         ") asdf",
            //                         "(asdf ( 234)",
            //                         " aasdf ( 234 + d)",
            //                         " aasdf + ( 234 + d)",
            //                         " aasdf * ( 234 + d)",
            //                         " (aasdf + ( 234) + d)",
            //                         " (aasdf - 234) + d)",
            //                         "( (aasdf - 234) + d)",
            //                         " (aasdf - 234) / d)",
            //                         " ((aasdf - 234) / d)",
            //                         " (aasdf - 234) / (d + 4956)",
            //                         " (aasdf - 234) / d + 4956)",
            //                         " (aasdf - 234) / (d + 4956",
            //                         " (aasdf - 234 / (d + 4956)",
            //                         " (aasdf - 234 / d + 4956)",/**/
            //                       };
            //ParseMethod testedParseMethod = AstralVariables.Expressions.NumberExpression.ParseMultiplication;
            //Console.WriteLine("=================ParseMultiplication============");
            #endregion
            #region ParseAddition
            //string[] expressions = { "12345.67",
            //                         "12345,67",
            //                         "12345,",
            //                         "29348487;",
            //                         "12345)",
            //                         "12345,)",
            //                         "341(",
            //                         "341.(",
            //                         "12345 6789",
            //                         "12345+",
            //                         "2_sdfe_w234",
            //                         "sdfe_w234",
            //                         "$12316",                                     
            //                         "`s;lsdkg",
            //                         "12345.67 * ldfj22",
            //                         "ldfj22 * 12345.67",
            //                         "12345.67 * ldfj22 / 4.67",
            //                         "sdfl_ * ldfj22",
            //                         "sdfl_ / ldfj22 / 5987",
            //                         "sdfl_ * NumericCount(5)",
            //                         "NumericCount(5)",
            //                         "NumericCount(5) * sdfl_ * NumericCount(5)",
            //                         "*gfrrr",
            //                         "NumericCount",
            //                         "NumerCount(sdf)",
            //                         "NumericCount()",
            //                         "NumericCount(",
            //                         "NumericCount)",
            //                         "NumericCount(e)",
            //                         "NumericCount(5)",
            //                         "NumericCount(58d)",
            //                         "NumericCount(.*df)",
            //                         "ItemCount",
            //                         "ItemCout(sdf)",
            //                         "ItemCount()",
            //                         "ItemCount(",
            //                         "ItemCount)",
            //                         "ItemCount(e)",
            //                         "ItemCount(5)",
            //                         "ItemCount(58d)",
            //                         "ItemCount(.*df)",
            //                         "( 5 )",
            //                         " ( asdf_3 )",
            //                         "( 654",
            //                         ") asdf",
            //                         "(asdf ( 234)",
            //                         " aasdf ( 234 + d)",
            //                         " aasdf + ( 234 + d)",
            //                         " aasdf * ( 234 + d)",
            //                         " (aasdf + ( 234) + d)",
            //                         " (aasdf - 234) + d)",
            //                         "( (aasdf - 234) + d)",
            //                         " (aasdf - 234) / d)",
            //                         " ((aasdf - 234) / d)",
            //                         " (aasdf - 234) / (d + 4956)",
            //                         " (aasdf - 234) / d + 4956)",
            //                         " (aasdf - 234) / (d + 4956",
            //                         " (aasdf - 234 / (d + 4956)",
            //                         " (aasdf - 234 / d + 4956)",
            //                         "5 + asd_2 - 123 + _alskdj"
            //                       };
            //ParseMethod testedParseMethod = AstralVariables.Expressions.NumberExpression.ParseAddition;
            //Console.WriteLine("=================ParseAddition============");
            #endregion
            #region ParseBracketedAddition
            //string[] expressions = { /*"( 5 )",
            //                         " ( asdf_3 )",
            //                         "( 654",
            //                         " 654 )",
            //                         " (   (654 ))",
            //                         ") asdf", */
            //                         "(asdf ( 234)",
            //                       };
            //ParseMethod testedParseMethod = AstralVariables.Expressions.NumberExpression.ParseBracketedAddition;
            //Console.WriteLine("=============ParseBracketedAddition=============");
            #endregion

            //for (int i = 0; i < expressions.Length; i++)
            //{
            //    Console.WriteLine($"Test {i}.");
            //    TestParse(ref expressions[i], testedParseMethod);
            //    Console.WriteLine("================================================");
            //    Console.ReadKey();
            //}

            //Console.WriteLine("Press 'Enter' to start manual input or 'Anykey' to finish");
            //while (Console.ReadKey().Key == ConsoleKey.Enter)
            //{

            //    Console.WriteLine("================================================");
            //    Console.WriteLine("Enter expression:");
            //    string expression = Console.ReadLine();
            //    TestParse(ref expression, AstralVariables.Expressions.NumberExpression.ParseVariable);
            //    Console.WriteLine("Press 'Enter' to repeat manual input or 'Anykey' to finish");
            //}

            #region Expression.Parse
            string[] expressions = { /*"12345.67",
                                     "12345,67",
                                     "12345,",
                                     "29348487;",
                                     "12345)",
                                     "12345,)",
                                     "341(",
                                     "341.(",
                                     "12345 6789",
                                     "12345+",
                                     "2_sdfe_w234",
                                     "sdfe_w234",
                                     " sdfe_w234  ",
                                     "sdfe_w234 [",
                                     "sdfe_w234 []",*/
                                     "sdfe_w234 [Account]",
                                     "sdfe_w234 [Character]",
                                     "sdfe_w234 [Global]",
                                     "sdfe_w234 [Common]",
                                     "sdfe_w234 [Profile]",
                                     "sdfe_w234 [Character, Profile]",
                                     "sdfe_w234 [  Character  , Profile ] ",
                                     /*"$12316",
                                     "`s;lsdkg",
                                     "12345.67 * ldfj22",
                                     "ldfj22 * 12345.67",
                                     "12345.67 * ldfj22 / 4.67",
                                     "sdfl_ * ldfj22",
                                     "sdfl_ / ldfj22 / 5987",
                                     "sdfl_ * NumericCount(5)",
                                     "NumericCount(5)",
                                     "NumericCount(5) * sdfl_ * NumericCount(5)",
                                     "*gfrrr",
                                     "NumericCount",
                                     "NumerCount(sdf)",
                                     "NumericCount()",
                                     "NumericCount(",
                                     "NumericCount)",
                                     "NumericCount(e)",
                                     "NumericCount(5)",
                                     "NumericCount(58d)",
                                     "NumericCount(.*df)",
                                     "ItemCount",
                                     "ItemCout(sdf)",
                                     "ItemCount()",
                                     "ItemCount(",
                                     "ItemCount)",
                                     "ItemCount(e)",
                                     "ItemCount(5)",
                                     "ItemCount(58d)",
                                     "ItemCount(.*df)",
                                     "( 5 )",
                                     " ( asdf_3 )",
                                     "( 654",
                                     ") asdf",
                                     "(asdf ( 234)",
                                     " aasdf ( 234 + d)",
                                     " aasdf + ( 234 + d)",
                                     " aasdf * ( 234 + d)",
                                     " (aasdf + ( 234) + d)",
                                     " (aasdf - 234) + d)",
                                     "( (aasdf - 234) + d)",
                                     " (aasdf - 234) / d)",
                                     " ((aasdf - 234) / d)",
                                     " (aasdf - 234) / (d + 4956)",
                                     " (aasdf - 234) / d + 4956)",
                                     " (aasdf - 234) / (d + 4956",
                                     " (aasdf - 234 / (d + 4956)",
                                     " (aasdf - 234 / d + 4956)",
                                     "5 + asd_2 - 123 + _alskdj"*/
                                   };
            //Console.WriteLine("=================Expression.Parse()===============");
            //NumberExpression numExpr = new NumberExpression();


            //for (int i = 0; i < expressions.Length; i++)
            //{
            //    Console.WriteLine($"Test {i}.");
            //    numExpr.Text = expressions[i];
            //    bool result = numExpr.IsValid;
            //    Console.WriteLine($"Parse expression: {numExpr.Text}");
            //    if (result)
            //    {
            //        Console.WriteLine($"Parse suceedeed\n" +
            //          $"  Type = {numExpr.AST?.GetType().Name}\n" +
            //          $"  Description = {numExpr.AST?.Description()}");
            //    }
            //    else
            //    {
            //        Console.WriteLine($"Parse faild");
            //        PrintErrorMessage(numExpr.ParseError);
            //    }
            //    Console.WriteLine("================================================");

            //Console.ReadKey();
            //}

            ParseMethod testedParseMethod = VariableTools.Expressions.NumberExpression.ParseVariable;
            for (int i = 0; i < expressions.Length; i++)
            {
                Console.WriteLine($"Test {i}.");
                TestParse(ref expressions[i], testedParseMethod);
                Console.WriteLine("================================================");
                Console.ReadKey();
            }
            Console.ReadKey();
            #endregion
        }
        private delegate NumberAstNode ParseMethod(ref string expression);

        private static void TestParse(ref string expression, ParseMethod parseMethod)
        {
            try
            {
                Console.WriteLine($"Parse expression: {expression}");
                NumberAstNode node = parseMethod(ref expression);
                Console.WriteLine($"Parse result:\n" +
                    $"  Type = {node.GetType().Name}\n" +
                    $"  Result = {node.Result}\n" +
                    $"  Description = {node.Description()}");
                Console.WriteLine($"Unparsed string is: {expression}");
            }
            catch (BaseParseError e)
            {
                Console.WriteLine("-------------------ParseError-------------------");
                Console.WriteLine($"In the expression: {e.expression}");
                Console.WriteLine($"At the positions: {expression.Length - e.expression.Length}");
                Console.WriteLine($"Error message:");
                PrintErrorMessage(e);
                Console.WriteLine("------------------------------------------------");
            }
        }

        private static void PrintErrorMessage(BaseParseError e, int indent = 0)
        {
            Console.WriteLine(Parser.MakeIndent(indent) + e.Message);
            foreach(BaseParseError e2 in e.ErrorStack)
            {
                PrintErrorMessage(e2, indent + 1);
            }
        }
    }
}
