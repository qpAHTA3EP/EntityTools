using Astral.Logic.UCC.Classes;

namespace EntityTools.Core.Interfaces
{
    public interface IUccConditionEngine
    {
        bool IsOK(UCCAction refAction);

        string TestInfos(UCCAction refAction);

        string Label();

        bool Rebase(UCCCondition refAction);
    }
}
