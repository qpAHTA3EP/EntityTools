﻿using System;
using System.ComponentModel;
using System.Drawing.Design;
using Astral.Logic.UCC.Classes;
using EntityTools.UCC.Extensions;

namespace EntityTools.Editors
{
#if DEVELOPER
    public class UccActionEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
#if false
            if (UCCEditorExtensions.GetUccAction(out UCCAction uccAction))
                return uccAction; 
#else
            if (EntityTools.Core.GUIRequest_UCCAction(out UCCAction action))
                return action;
#endif

            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
#endif
}
