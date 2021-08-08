using Astral.Logic.UCC.Classes;

namespace EntityTools.UCC.Conditions
{
    public interface ICustomUCCCondition
    {
        // TODO: Исправить опечатку Loked -> Locked
        bool Loсked { get; set; }

        bool IsOK(UCCAction refAction = null);

        string TestInfos(UCCAction refAction = null);
    }
}
