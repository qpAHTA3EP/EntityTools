﻿using System;
using System.Collections.Generic;
using MyNW.Classes;
using MyNW.Internals;

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
        public string ParentName { get; set; }
        //public MyNW.Patchables.Enums.UIGenType Type { get; set; }
        public string Type { get; set; }
        public bool IsVisible { get; set; }
        public List<UIVarDef> uiVars { get; set; } = new List<UIVarDef>();

        public UIInterfaceDef()
        {
            Name = string.Empty;
            ParentName = string.Empty;
            Type = string.Empty; //MyNW.Patchables.Enums.UIGenType.Box;
            IsVisible = false;
        }
        public UIInterfaceDef(UIGen uiGen)
        {
            Name = uiGen.Name;
            ParentName = uiGen.Parent?.Name ?? string.Empty;
            Type = uiGen.Type.ToString();
            IsVisible = uiGen.IsVisible;
            foreach (UIVar v in uiGen.Vars)
            {
                uiVars.Add(new UIVarDef
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
    }
}
