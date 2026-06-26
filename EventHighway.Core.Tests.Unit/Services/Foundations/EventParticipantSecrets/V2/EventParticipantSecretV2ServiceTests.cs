// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Services.Foundations.EventParticipantSecrets.V2;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventParticipantSecrets.V2
{
    public partial class EventParticipantSecretV2ServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly IEventParticipantSecretV2Service eventParticipantSecretV2Service;

        public EventParticipantSecretV2ServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();

            this.eventParticipantSecretV2Service = new EventParticipantSecretV2Service(
                storageBroker: this.storageBrokerMock.Object);
        }

        private static EventParticipantSecretV2 CreateRandomEventParticipantSecretV2() =>
            CreateEventParticipantSecretV2Filler(dates: GetRandomDateTimeOffset()).Create();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static Filler<EventParticipantSecretV2> CreateEventParticipantSecretV2Filler(DateTimeOffset dates)
        {
            var filler = new Filler<EventParticipantSecretV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dates)

                .OnProperty(secret => secret.IsActive)
                    .Use(true)

                .OnProperty(secret => secret.ActiveFrom)
                    .IgnoreIt()

                .OnProperty(secret => secret.ActiveTo)
                    .IgnoreIt()

                .OnProperty(secret => secret.EventParticipantV2)
                    .IgnoreIt();

            return filler;
        }
    }
}
