using Astral.Logic.UCC.Classes;
using MyNW.Classes;
using System;

namespace EntityTools.Core.Interfaces
{
    public interface IUccActionEngine : IDisposable
    {
        bool NeedToRun { get; }
        bool Run();
        Entity UnitRef { get; }
        string Label();

        bool Rebase(UCCAction action);
    }
}
