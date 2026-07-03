// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Processings.ListenerEvents.V2.Exceptions;
using EventHighway.Core.Services.Orchestrations.ListenerEvents.V2;
using EventHighway.Core.Services.Processings.ListenerEvents.V2;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.ListenerEvents.V2
{
    public partial class ListenerEventV2OrchestrationServiceTests
    {
        private readonly Mock<IListenerEventV2ProcessingService> listenerEventV2ProcessingServiceMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IListenerEventV2OrchestrationService listenerEventV2OrchestrationService;

        public ListenerEventV2OrchestrationServiceTests()
        {
            this.listenerEventV2ProcessingServiceMock = new Mock<IListenerEventV2ProcessingService>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.listenerEventV2OrchestrationService =
                new ListenerEventV2OrchestrationService(
                    listenerEventV2ProcessingService: this.listenerEventV2ProcessingServiceMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();
            someInnerException.Data.Add("ErrorCode", new List<string> { "ValidationError" });

            return new TheoryData<Xeption>
            {
                new ListenerEventV2ProcessingValidationException(someMessage, someInnerException),
                new ListenerEventV2ProcessingDependencyValidationException(someMessage, someInnerException),
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();
            someInnerException.Data.Add("ErrorCode", new List<string> { "DependencyError" });

            return new TheoryData<Xeption>
            {
                new ListenerEventV2ProcessingDependencyException(someMessage, someInnerException),
                new ListenerEventV2ProcessingServiceException(someMessage, someInnerException),
            };
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static IEnumerable<Guid> CreateRandomEventV2Ids() =>
            Enumerable.Range(0, GetRandomNumber())
                .Select(_ => Guid.NewGuid())
                    .ToList();

        private static IEnumerable<ListenerEventV2> CreateRandomListenerEventV2s() =>
            CreateListenerEventV2Filler()
                .Create(count: GetRandomNumber())
                    .ToList();

        private static ListenerEventV2 CreateRandomListenerEventV2() =>
            CreateListenerEventV2Filler().Create();

        private static Filler<ListenerEventV2> CreateListenerEventV2Filler()
        {
            var filler = new Filler<ListenerEventV2>();

            filler.Setup()
                .OnType<DateTimeOffset>()
                    .Use(GetRandomDateTimeOffset)

                .OnType<EventParticipantV2>().IgnoreIt()

                .OnProperty(listenerEventV2 => listenerEventV2.EventV2)
                    .IgnoreIt()

                .OnProperty(listenerEventV2 => listenerEventV2.EventAddressV2)
                    .IgnoreIt()

                .OnProperty(listenerEventV2 => listenerEventV2.EventListenerV2)
                    .IgnoreIt();

            return filler;
        }
    }
}
