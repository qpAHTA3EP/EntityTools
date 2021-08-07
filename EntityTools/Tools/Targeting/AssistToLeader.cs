using AcTp0Tools.Classes.UCC;

namespace EntityTools.UCC.Actions.TargetSelectors
{
    /// <summary>
    /// Целеуказатель на цель лидера группы
    /// </summary>
    public class AssistToLeader : UccTargetSelector
    {
        public override UccTargetSelector Clone()
        {
            return new AssistToLeader();
        }

        public override string ToString() => "AssistToLeader";
    }
}
