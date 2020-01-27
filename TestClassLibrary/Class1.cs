using Astral.Logic.UCC.Classes;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestClassLibrary
{
    public class TestClass
    {
        private Dictionary<string, Type> actions;
        private ListBoxControl typesList;

        private TestClass(Type usedType)
        {
            InitializeComponent();

            List<Type> types = Astral.Functions.XmlSerializer.GetExtraTypes(2);
            foreach (Type type in types)
            {
                Type baseType = type.BaseType;
                Type uccActionType = typeof(UCCAction);
                if (baseType == uccActionType)
                {
                    actions.Add(type.Name, type);
                    typesList.Items.Add(type.Name);
                }
            }
        }

        private void InitializeComponent()
        {
            actions = new Dictionary<string, Type>();
            typesList = new ListBoxControl();
        }
        

    }
}
