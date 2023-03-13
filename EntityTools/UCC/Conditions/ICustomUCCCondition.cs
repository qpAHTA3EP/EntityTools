using Astral.Logic.UCC.Classes;

namespace EntityTools.UCC.Conditions
{
    public interface ICustomUCCCondition
    {
        bool Locked { get; set; }

        bool IsOK(UCCAction refAction = null);

        ICustomUCCCondition Clone();

        string TestInfos(UCCAction refAction = null);
    }
}
