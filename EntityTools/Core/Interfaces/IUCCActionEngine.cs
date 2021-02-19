using Astral.Logic.UCC.Classes;
using MyNW.Classes;

namespace EntityTools.Core.Interfaces
{
    public interface IUCCActionEngine
    {
        bool NeedToRun { get; }
        bool Run();
        Entity UnitRef { get; }
        string Label();

#if false
        bool Rebase(UCCAction action); 
#endif
    }
}
