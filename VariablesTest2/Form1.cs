#define TEST3

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using AstralVars;
using AstralVars.Classes;
using AstralVars.Forms;
using AstralVars.Expressions;
using AstralVars.Expressions.Numbers;

// This is the code for your desktop app.
// Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.

namespace VariablesTest2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AbstractTerm result;

            try
            {
                string expression = tbExpression.Text;
                result = NumberExpression.ParseNumber(ref expression);
                MessageBox.Show($"Result is [{result.Result}]\n" +
                    $"Unparsed expression is [{expression}]");
            }

            catch(ParseError error)
            {
                MessageBox.Show(error.Message);
            }
        }
    }
}
