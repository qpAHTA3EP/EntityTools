using Astral.Logic.UCC.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityTools.UCC.Conditions
{
    public interface ICustomUCCCondition
    {
        bool Loked { get; set; }
        bool IsOk(UCCAction refAction = null);
    }
}
