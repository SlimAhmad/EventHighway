// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using EventHighway.Core.Clients.HealthChecks.V2;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2.Exceptions;
using EventHighway.Core.Services.Coordinations.HealthChecks.V2;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.HealthChecks.V2
{
    public partial class HealthParticipantClientV2Tests
    {
        private readonly Mock<IHealthV2CoordinationService> healthV2CoordinationServiceMock;
        private readonly IHealthParticipantClientV2 healthParticipantClientV2;

        public HealthParticipantClientV2Tests()
        {
            this.healthV2CoordinationServiceMock =
                new Mock<IHealthV2CoordinationService>();

            this.healthParticipantClientV2 =
                new HealthParticipantClientV2(
                    healthV2CoordinationService:
                        this.healthV2CoordinationServiceMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.Data.Add("ErrorCode", new List<string> { "ValidationError" });

            return new TheoryData<Xeption>
            {
                new HealthV2CoordinationValidationException(
                    someMessage,
                    someInnerException),

                new HealthV2CoordinationDependencyValidationException(
                    someMessage,
                    someInnerException),
            };
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static TrafficPeriodV2 GetRandomTrafficPeriodV2() =>
            TrafficPeriodV2.Day;

        private static IEnumerable<ParticipantSummaryV2> CreateRandomParticipantSummaryV2s() =>
            Enumerable.Range(start: 0, count: new IntRange(min: 2, max: 9).GetValue())
                .Select(_ => CreateRandomParticipantSummaryV2())
                    .ToList();

        private static ParticipantSummaryV2 CreateRandomParticipantSummaryV2() =>
            new ParticipantSummaryV2
            {
                EventParticipantV2Id = Guid.NewGuid(),
                Name = GetRandomString(),
                ContactEmail = GetRandomString(),
                ContactPhone = GetRandomString(),
                IsActive = true,
                Period = GetRandomTrafficPeriodV2(),
                WindowStart = GetRandomDateTimeOffset(),
                WindowEnd = GetRandomDateTimeOffset(),
                WindowLabel = GetRandomString(),
                TotalEventsSubmitted = new IntRange(min: 2, max: 9).GetValue(),
                ActiveEventAddresses = new IntRange(min: 2, max: 9).GetValue(),
                ActiveEventAddressNames = new List<string>(),
                TotalListenerEvents = new IntRange(min: 2, max: 9).GetValue(),
                OwnedListeners = new IntRange(min: 2, max: 9).GetValue(),
                PublisherErrorRate = new IntRange(min: 0, max: 5).GetValue(),
                ListenerErrorRate = new IntRange(min: 0, max: 5).GetValue(),
                LoopsDetected = new IntRange(min: 0, max: 5).GetValue(),
                DuplicatesDetected = new IntRange(min: 0, max: 5).GetValue(),
                Status = HealthStatusV2.Green,
                LastActivity = GetRandomDateTimeOffset()
            };
    }
}
