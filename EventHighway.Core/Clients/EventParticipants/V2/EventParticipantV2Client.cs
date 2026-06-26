// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Services.Foundations.EventParticipants.V2;

namespace EventHighway.Core.Clients.EventParticipants.V2
{
    internal class EventParticipantV2Client : IEventParticipantV2Client
    {
        private readonly IEventParticipantV2Service eventParticipantV2Service;

        public EventParticipantV2Client(IEventParticipantV2Service eventParticipantV2Service) =>
            this.eventParticipantV2Service = eventParticipantV2Service;

        public ValueTask<EventParticipantV2> AddEventParticipantV2Async(
            EventParticipantV2 eventParticipantV2,
            CancellationToken cancellationToken = default) =>
                throw new NotImplementedException();
    }
}
