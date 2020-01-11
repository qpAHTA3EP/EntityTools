using Astral.Logic.UCC.Classes;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityTools.UCC.Conditions
{
    public interface ICustomUCCCondition
    {
        bool Loked { get; set; }

        bool IsOK(UCCAction refAction = null);

        string TestInfos(UCCAction refAction = null);
    }
}
