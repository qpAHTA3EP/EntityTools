using Astral.Logic.UCC.Classes;
using MyNW.Classes;

namespace EntityTools.Core.Interfaces
{
    public interface IUccActionEngine
    {
        bool NeedToRun { get; }
        bool Run();
        Entity UnitRef { get; }
        string Label();

        bool Rebase(UCCAction action);
    }
}
