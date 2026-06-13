// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Clients.ArchivingEvents.V2;
using EventHighway.Core.Models.Coordinations.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Services.Coordinations.ArchivingEvents.V2;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.ArchivingEvents.V2
{
    public partial class ArchivingEvent2ClientTests
    {
        private readonly Mock<IArchivingEventV2CoordinationService> archivingEventV2CoordinationServiceMock;
        private readonly IArchivingEvent2Client archivingEvent2Client;

        public ArchivingEvent2ClientTests()
        {
            this.archivingEventV2CoordinationServiceMock =
                new Mock<IArchivingEventV2CoordinationService>();

            this.archivingEvent2Client =
                new ArchivingEvent2Client(
                    archivingEventV2CoordinationService:
                        this.archivingEventV2CoordinationServiceMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            return new TheoryData<Xeption>
            {
                new ArchivingEventV2CoordinationValidationException(
                    someMessage,
                    someInnerException),

                new ArchivingEventV2CoordinationDependencyValidationException(
                    someMessage,
                    someInnerException),
            };
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();
    }
}
