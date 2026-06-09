// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Abstractions.EventHandlers.Exceptions;
using EventHighway.Core.Brokers.EventHandlers;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2;
using EventHighway.Core.Services.Foundations.EventCalls.V2;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventCalls.V2
{
    public partial class EventCallV2ServiceTests
    {
        private readonly Mock<IEventHandlerBroker> eventHandlerBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventCallV2Service eventCallV2Service;

        public EventCallV2ServiceTests()
        {
            this.eventHandlerBrokerMock = new Mock<IEventHandlerBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.eventCallV2Service = new EventCallV2Service(
                eventHandlerBrokers: new[] { this.eventHandlerBrokerMock.Object },
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static EventHandlerResult CreateRandomEventHandlerResult() =>
            new Filler<EventHandlerResult>().Create();

        private static EventCallV2 CreateRandomEventCallV2() =>
            CreateEventCallV2Filler().Create();

        public static TheoryData<Exception> CriticalEventHandlerDependencyExceptions() =>
            new TheoryData<Exception>
            {
                new SomeCriticalEventHandlerDependencyException()
            };

        public static TheoryData<Exception> DependencyValidationExceptions() =>
            new TheoryData<Exception>
            {
                new SomeDependencyValidationEventHandlerException()
            };

        private class SomeCriticalEventHandlerDependencyException
            : Exception, IEventHandlerDependencyException
        { }

        private class SomeDependencyValidationEventHandlerException
            : Exception, IEventHandlerDependencyValidationException
        { }

        private static Filler<EventCallV2> CreateEventCallV2Filler()
        {
            var filler = new Filler<EventCallV2>();

            filler.Setup()
                .OnProperty(call => call.HandlerConfigurations)
                .IgnoreIt();

            return filler;
        }
    }
}
