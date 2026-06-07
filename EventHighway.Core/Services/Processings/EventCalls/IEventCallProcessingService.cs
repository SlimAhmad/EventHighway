// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventCall;

namespace EventHighway.Core.Services.Processings.EventCalls
{
    internal interface IEventCallProcessingService
    {
        ValueTask<EventCall> RunAsync(EventCall eventCall);
    }
}
