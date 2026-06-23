// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Processings.Events.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.Events.V2
{
    public partial class EventV2ProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnBulkRestoreIfValidationExceptionOccursAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEnumerable<EventV2> someEventV2s = CreateRandomEventV2s();

            var expectedException =
                new EventV2ProcessingDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            this.eventV2ServiceMock.Setup(service =>
                service.BulkRestoreEventV2sAsync(
                    It.IsAny<IEnumerable<EventV2>>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask<IEnumerable<EventV2>> bulkRestoreEventV2sTask =
                this.eventV2ProcessingService.BulkRestoreEventV2sAsync(
                    someEventV2s,
                    randomCancellationToken);

            EventV2ProcessingDependencyValidationException actualException =
                await Assert.ThrowsAsync<EventV2ProcessingDependencyValidationException>(
                    bulkRestoreEventV2sTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.eventV2ServiceMock.Verify(service =>
                service.BulkRestoreEventV2sAsync(
                    It.IsAny<IEnumerable<EventV2>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
