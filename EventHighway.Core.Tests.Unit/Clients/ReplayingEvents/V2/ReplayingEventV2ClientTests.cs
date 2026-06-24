// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Clients.ReplayingEvents.V2;
using EventHighway.Core.Models.Coordinations.ReplayingEvents.V2.Exceptions;
using EventHighway.Core.Services.Coordinations.ReplayingEvents.V2;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.ReplayingEvents.V2
{
    public partial class ReplayingEventV2ClientTests
    {
        private readonly Mock<IReplayingEventV2CoordinationService> replayingEventV2CoordinationServiceMock;
        private readonly IReplayingEventV2Client replayingEventV2Client;

        public ReplayingEventV2ClientTests()
        {
            this.replayingEventV2CoordinationServiceMock =
                new Mock<IReplayingEventV2CoordinationService>();

            this.replayingEventV2Client =
                new ReplayingEventV2Client(
                    replayingEventV2CoordinationService:
                        this.replayingEventV2CoordinationServiceMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            return new TheoryData<Xeption>
            {
                new ReplayingEventV2CoordinationValidationException(someMessage, someInnerException),
                new ReplayingEventV2CoordinationDependencyValidationException(someMessage, someInnerException),
            };
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();
    }
}
