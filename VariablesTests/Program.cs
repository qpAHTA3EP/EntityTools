using System;
using System.Text.RegularExpressions;

namespace VariablesTests
{
    class Program
    {
        static void Main(string[] args)
        {
            //VariableCollection variableItems = new VariableCollection
            //{
            //    VariableItem.Make("Int", 99),
            //    VariableItem.Make("bl", true),
            //    VariableItem.Make("dt", DateTime.UtcNow),
            //    VariableItem.Make("vss", "Super"),
            //    VariableItem.Make("12", "Count[Artifactfood]"),
            //    VariableItem.Make("12", "ItemsCount[Gemfood]"),
            //    VariableItem.Make(VariableTypes.Boolean),
            //    VariableItem.Make(VariableTypes.Integer)
            //};

            //foreach (VariableItem item in variableItems)
            //{
            //    Console.WriteLine(item.ToString());
            //}

            string countPattern = @"^(ItemsCount|Items|NumericCount|Numeric|Counter|Count)\(\w*\)$",
                   countTrimPattern = @"(^(ItemsCount|Items|NumericCount|Numeric|Counter|Count)\()|(\)$)";

            string text = "ItemsCount(Aaafd)";

            if(Regex.IsMatch(text, countPattern))
                Console.WriteLine($"String '{text}' matches to pattern [{countPattern}]");
            else Console.WriteLine($"String '{text}' does not matche to pattern [{countPattern}]");

            string trimedText = Regex.Replace(text, countTrimPattern);
        }
}
}
