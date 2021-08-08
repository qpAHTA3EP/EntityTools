using Astral.Logic.UCC.Classes;
using System;

namespace EntityTools.Core.Interfaces
{
    public interface IUccConditionEngine : IDisposable
    {
        bool IsOK(UCCAction refAction);

        string TestInfos(UCCAction refAction);

        string Label();

        bool Rebase(UCCCondition refAction);
    }
}
