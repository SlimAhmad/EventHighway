// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.ArchivingEvents.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.ArchivingEvents.V2
{
    public partial class ArchivingEventV2CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnPurgeIfOlderThanIsInvalidAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset invalidOlderThan = default;

            var invalidArchivingEventV2CoordinationException =
                new InvalidArchivingEventV2CoordinationException(
                    message: "Archiving event is invalid, fix the errors and try again.");

            invalidArchivingEventV2CoordinationException.AddData(
                key: "OlderThan",
                values: "Required");

            var expectedArchivingEventV2CoordinationValidationException =
                new ArchivingEventV2CoordinationValidationException(
                    message: "Archiving event validation error occurred, fix the errors and try again.",
                    innerException: invalidArchivingEventV2CoordinationException);

            // when
            ValueTask purgeEventArchiveV2sTask =
                this.archivingEventV2CoordinationService.PurgeEventArchiveV2sAsync(
                    invalidOlderThan,
                    randomCancellationToken);

            ArchivingEventV2CoordinationValidationException
                actualArchivingEventV2CoordinationValidationException =
                    await Assert.ThrowsAsync<ArchivingEventV2CoordinationValidationException>(
                        purgeEventArchiveV2sTask.AsTask);

            // then
            actualArchivingEventV2CoordinationValidationException.Should()
                .BeEquivalentTo(expectedArchivingEventV2CoordinationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2CoordinationValidationException))),
                        Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.archivingEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
