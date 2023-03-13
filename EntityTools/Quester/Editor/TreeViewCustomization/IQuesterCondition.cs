namespace EntityTools.Quester.Editor.TreeViewCustomization
{
    public interface IQuesterCondition
    {
        bool IsValid { get; }

        void Reset();

        bool Locked { get; set; }

        string TestInfos { get; }
    }
}