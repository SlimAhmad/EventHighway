// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events;

namespace EventHighway.Core.Services.Coordinations.Events
{
    public interface IEventCoordinationService
    {
        ValueTask<Event> SubmitEventAsync(Event @event);
    }
}
