using EntityTools.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mount_Tutorial
{
    enum QuesterQctionRequestType
    {
        NeedToRun, // bool
        Run, // ActionResult
        ActionLabel, // string
        InternalConditions, //bool
        InternalValidity, // ActionValidity

        UseHotSpots, // bool
        InternalDestination, // Vector3

        InternalReset,  // void
        GatherInfos,  // void
        OnMapDraw// in (GraphicsNW graph), out void
    }

    interface IQuesterActionRequest : IMessage, IQuesterActionEngine
    {
        QuesterQctionRequestType QuesterQctionRequestType { get; }
    }
}
