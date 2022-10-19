namespace EntityCore.Quester.Classes
{
    public interface IQuesterCondition
    {
        bool IsValid { get; }

        void Reset();

        bool Locked { get; set; }

        string TestInfos { get; }
    }
}