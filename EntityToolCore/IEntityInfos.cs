using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityCore
{
    public interface IEntityInfos
    {
        Entity Target();
        bool TargetValidate();
        bool EntityDiagnosticString(out string infoString);
    }
}
