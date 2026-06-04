// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents;

namespace EventHighway.Core.Services.Processings.ListenerEvents
{
    internal interface IListenerEventProcessingService
    {
        ValueTask<ListenerEvent> AddListenerEventAsync(ListenerEvent listenerEvent);
        ValueTask<ListenerEvent> ModifyListenerEventAsync(ListenerEvent listenerEvent);
    }
}
