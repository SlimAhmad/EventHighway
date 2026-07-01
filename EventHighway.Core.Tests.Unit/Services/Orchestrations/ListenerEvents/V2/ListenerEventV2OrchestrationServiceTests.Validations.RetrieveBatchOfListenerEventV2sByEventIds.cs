// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.ListenerEvents.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.ListenerEvents.V2
{
    public partial class ListenerEventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveBatchOfListenerEventV2sByEventIdsIfEventV2IdsIsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEnumerable<Guid> nullEventV2Ids = null;
            int randomTake = GetRandomNumber();
            int inputTake = randomTake;

            var invalidListenerEventV2OrchestrationException =
                new InvalidListenerEventV2OrchestrationException(
                    message: "Listener event is invalid, fix the errors and try again.");

            invalidListenerEventV2OrchestrationException.AddData(
                key: "eventV2Ids",
                values: "Value is required");

            var expectedListenerEventV2OrchestrationValidationException =
                new ListenerEventV2OrchestrationValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: invalidListenerEventV2OrchestrationException);

            // when
            ValueTask<IEnumerable<ListenerEventV2>> retrieveBatchOfListenerEventV2sByEventIdsTask =
                this.listenerEventV2OrchestrationService
                    .RetrieveBatchOfListenerEventV2sByEventIdsAsync(
                        nullEventV2Ids,
                        inputTake,
                        randomCancellationToken);

            ListenerEventV2OrchestrationValidationException
                actualListenerEventV2OrchestrationValidationException =
                    await Assert.ThrowsAsync<ListenerEventV2OrchestrationValidationException>(
                        retrieveBatchOfListenerEventV2sByEventIdsTask.AsTask);

            // then
            actualListenerEventV2OrchestrationValidationException.Should()
                .BeEquivalentTo(expectedListenerEventV2OrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2OrchestrationValidationException))),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
