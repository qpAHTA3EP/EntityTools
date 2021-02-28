using QuesterCondition = Astral.Quester.Classes.Condition;

namespace EntityTools.Quester.Conditions
{
    public interface IQuesterConditionEngine
    {
        bool IsValid { get; }
        void Reset();
        string TestInfos { get; }
        string Label();

        bool Rebase(QuesterCondition condition); 
    }
}
