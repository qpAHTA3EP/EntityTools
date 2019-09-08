using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace EntityTools.Tools
{
    [Serializable]
    public class InterfaceWrapper
    {
        public List<UIInterfaceDef> Interfaces { get; set; } = new List<UIInterfaceDef>();  
        
        public InterfaceWrapper()
        {
            foreach (UIGen uiGen in UIManager.AllUIGen)
                Interfaces.Add(new UIInterfaceDef(uiGen));
        }
    }

    [Serializable]
    public class UIInterfaceDef
    {
        public string Name { get; set; }
        public MyNW.Patchables.Enums.UIGenType Type { get; set; }
        public bool IsVisible { get; set; }
        public List<UIVarDef> uiVars { get; set; } = new List<UIVarDef>();

        public UIInterfaceDef()
        {
            Name = string.Empty;
            Type = MyNW.Patchables.Enums.UIGenType.Box;
            IsVisible = false;            
        }
        public UIInterfaceDef(UIGen uiGen)
        {
            Name = uiGen.Name;
            Type = uiGen.Type;
            IsVisible = uiGen.IsVisible;
            foreach (UIVar v in uiGen.Vars)
            {
                uiVars.Add(new UIVarDef()
                {
                    Name = v.Name,
                    Value = v.Value
                });
            }
        }
    }

    [Serializable]
    public class UIVarDef
    {
        public string Name { get; set; } = string.Empty;
        public object Value { get; set; } = string.Empty;

        public UIVarDef() { }
    }
}
