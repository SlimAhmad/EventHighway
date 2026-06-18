// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
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
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnBulkRemoveIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption eventV2ValidationException)
        {
            // given
            IQueryable<EventV2> someEventV2s = CreateRandomEventV2s();
            IEnumerable<EventV2> inputEventV2s = someEventV2s;

            var expectedEventV2ProcessingDependencyValidationException =
                new EventV2ProcessingDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: eventV2ValidationException.InnerException as Xeption);

            this.eventV2ServiceMock.Setup(service =>
                service.BulkRemoveEventV2sAsync(
                    inputEventV2s,
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(eventV2ValidationException);

            // when
            ValueTask bulkRemoveEventV2sTask =
                this.eventV2ProcessingService.BulkRemoveEventV2sAsync(
                    inputEventV2s,
                    TestContext.Current.CancellationToken);

            EventV2ProcessingDependencyValidationException
                actualEventV2ProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<EventV2ProcessingDependencyValidationException>(
                        bulkRemoveEventV2sTask.AsTask);

            // then
            actualEventV2ProcessingDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventV2ProcessingDependencyValidationException);

            this.eventV2ServiceMock.Verify(service =>
                service.BulkRemoveEventV2sAsync(
                    inputEventV2s,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2ProcessingDependencyValidationException))),
                        Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
