using System;
using ValiablesAstralExtention.Classes;

namespace VariablesTests
{
    class Program
    {
        static void Main(string[] args)
        {
            VariableCollection variableItems = new VariableCollection
            {
                VariableItem.Make("Int", 99),
                VariableItem.Make("bl", true),
                VariableItem.Make("dt", DateTime.UtcNow),
                VariableItem.Make("vss", "Super"),
                VariableItem.Make("12", "Count[Artifactfood]"),
                VariableItem.Make("12", "ItemsCount[Gemfood]"),
                VariableItem.Make(VariableTypes.Boolean),
                VariableItem.Make(VariableTypes.Integer)
            };

            foreach (VariableItem item in variableItems)
            {
                Console.WriteLine(item.ToString());
            }
        }
    }
}
