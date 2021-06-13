using System;
using Astral.Logic.Classes.Map;
using MyNW.Classes;
using static Astral.Quester.Classes.Action;
using QuesterAction = Astral.Quester.Classes.Action;

namespace EntityTools.Core.Interfaces
{
    public interface IQuesterActionEngine : IDisposable
    {
        bool NeedToRun { get; }
        ActionResult Run();

        string ActionLabel { get; }

        bool InternalConditions { get; }
        ActionValidity InternalValidity { get; }

        bool UseHotSpots { get; }
        Vector3 InternalDestination { get; }

        void InternalReset();
        void GatherInfos();
        void OnMapDraw(GraphicsNW graph);

        //void OnPropertyChanged(object sender, PropertyChangedEventArgs e);
        void OnPropertyChanged(QuesterAction sender, string propertyName);
        bool Rebase(QuesterAction action); 
    }
}
