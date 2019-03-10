using System;
using System.Collections.Generic;
using Astral.Forms;
using MyNW.Classes;
using System.ComponentModel;
using Astral.Logic.Classes.Map;
using Astral.Quester.Classes;
using System.Drawing.Design;
using Astral.Quester.Forms;
using Astral.Quester.UIEditors;
using ValiablesAstralExtention.Classes;

namespace ValiablesAstralExtention
{
    public class ScalarCondition : Astral.Quester.Classes.Condition
    {
        [Editor(typeof(ScalarNamesEditor), typeof(UITypeEditor))]
        [Description("Имя переменной")]
        public string VariableName { get; set; } = String.Empty;

        //private Dictionary<String, int> Variables { get => VariablesAddon.Scalars; }

        [Description("Величина, сопоставляемая со значением переменной")]
        public int Value { get; set; } = 0;

        //[Editor(typeof(Astral.Quester.UIEditors.PositionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        //[Description("Final destination")]
        //public Vector3 Position { set; get; } = new Vector3();

        [Description("Способ сопоставления Value со значением переменной")]
        public Relation Sign { get; set; } = Relation.Equal;

        public override bool IsValid
        {
            get
            {
                //switch (this.Sign)
                //{
                //    case Astral.Quester.Classes.Condition.Relation.Equal:
                //        return (VariablesBase.GetValue(this.VariableName) == this.Value);

                //    case Astral.Quester.Classes.Condition.Relation.NotEqual:
                //        return (VariablesBase.GetValue(this.VariableName) != this.Value);

                //    case Astral.Quester.Classes.Condition.Relation.Inferior:
                //        return (VariablesBase.GetValue(this.VariableName) < this.Value);

                //    case Astral.Quester.Classes.Condition.Relation.Superior:
                //        return (VariablesBase.GetValue(this.VariableName) > this.Value);
                //}
                return false;
            }
        }

        public override void Reset()
        {
        }

        public override string TestInfos
        {
            get
            {
                //int val = VariablesBase.GetValue(VariableName);
                //switch (this.Sign)
                //{
                //    case Relation.Equal:
                //        if (val == this.Value)
                //            return "Sign \"" + Relation.Equal.ToString() + "\" is True: variable [" + VariableName + "] " + Relation.Equal.ToString() + " " + Value.ToString();
                //        else return "Sign \"" + Relation.Equal.ToString() + "\" is False: variable [" + VariableName + "] = " + val.ToString() + " " + Relation.NotEqual.ToString() + " " + Value.ToString();

                //    case Relation.NotEqual:
                //        if (val != this.Value)
                //            return "Sign \"" + Relation.NotEqual.ToString() + "\" is True: variable [" + VariableName + "] = " + val.ToString() + " " + Relation.NotEqual.ToString() + " " + Value.ToString();
                //        else return "Sign \"" + Relation.NotEqual.ToString() + "\" is False: variable [" + VariableName + "] " + Relation.NotEqual.ToString() + " " + Value.ToString();

                //    case Relation.Inferior:
                //        if (val < this.Value)
                //            return "Sign \"" + Relation.Inferior.ToString() + "\" is True: variable [" + VariableName + "] = " + val.ToString() + " " + Relation.Inferior.ToString() + " " + Value.ToString();
                //        else return "Sign \"" + Relation.Inferior.ToString() + "\" is False: variable [" + VariableName + "] " + val.ToString() + " " + Relation.Superior.ToString() + " " + Value.ToString();

                //    case Relation.Superior:
                //        if (val > this.Value)
                //            return "Sign \"" + Relation.Superior.ToString() + "\" is True: variable [" + VariableName + "] = " + val.ToString() + " " + Relation.Superior.ToString() + " " + Value.ToString();
                //        else return "Sign \"" + Relation.Superior.ToString() + "\" is False: variable [" + VariableName + "] " + val.ToString() + " " + Relation.Inferior.ToString() + " " + Value.ToString();
                //}
                //return "Not Indefinite";
                return string.Empty;
            }
        }
    }
}