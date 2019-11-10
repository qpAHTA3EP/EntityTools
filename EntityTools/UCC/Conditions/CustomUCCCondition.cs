using Astral.Logic.UCC.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EntityTools.UCC.Conditions
{
    public abstract class CustomUCCCondition
    {
        [Browsable(false)]
        public virtual bool IsOK(UCCAction refAction = null)
        {
            return false;
        }

        public bool Locked { get; set; }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}
