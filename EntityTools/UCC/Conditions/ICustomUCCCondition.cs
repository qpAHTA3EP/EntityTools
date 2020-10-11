using Astral.Logic.UCC.Classes;

namespace EntityTools.UCC.Conditions
{
    public interface ICustomUCCCondition
    {
        bool Loked { get; set; }

        bool IsOK(UCCAction refAction = null);

        string TestInfos(UCCAction refAction = null);
    }
}
