// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Coordinations.ReplayingEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.ReplayingEvents.V2
{
    public partial class ReplayingEventV2CoordinationServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnReplayIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid? eventAddressId = GetRandomId();
            List<Guid> eventListenerIds = new List<Guid> { GetRandomId() };
            DateTimeOffset? startDate = GetRandomDateTimeOffset();
            DateTimeOffset? endDate = startDate.Value.AddDays(GetRandomNumber());

            BatchConfiguration batchConfiguration = CreateBatchConfiguration(GetRandomNumber());

            var expectedException =
                new ReplayingEventV2CoordinationDependencyValidationException(
                    message: "Replaying event validation error occurred, fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveBatchOfListenerEventArchiveV2sAsync(
                    It.IsAny<Guid?>(), It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>(),
                    It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask replayTask =
                this.replayingEventV2CoordinationService.ReplayEventArchiveV2sAsync(
                    eventAddressId, eventListenerIds, startDate, endDate, randomCancellationToken);

            ReplayingEventV2CoordinationDependencyValidationException actualException =
                await Assert.ThrowsAsync<ReplayingEventV2CoordinationDependencyValidationException>(
                    replayTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfListenerEventArchiveV2sAsync(
                    It.IsAny<Guid?>(), It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>(),
                    It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(), Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.restoringEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
