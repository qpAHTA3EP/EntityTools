using Astral.Logic.UCC.Classes;

namespace EntityTools.Core.Interfaces
{
    public interface IUCCConditionEngine
    {
        bool IsOK(UCCAction refAction);

        string TestInfos(UCCAction refAction);

        string Label();
    }
}
