namespace EntityTools.Core.Interfaces
{
    public interface IPowerCache
    {
        bool IsInitialized { get; }
        string PowerIdPattern { get; set; }
        MyNW.Classes.Power Power { get; }
        void Reset(string powerPattern = default);
        MyNW.Classes.Power GetPower();
    }
}
