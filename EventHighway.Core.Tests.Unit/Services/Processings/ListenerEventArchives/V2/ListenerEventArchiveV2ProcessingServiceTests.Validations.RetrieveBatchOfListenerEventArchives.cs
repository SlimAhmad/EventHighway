// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Processings.ListenerEventArchives.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEventArchives.V2
{
    public partial class ListenerEventArchiveV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveBatchOfListenerEventArchivesIfSkipIsInvalidAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int invalidSkip = GetRandomNegativeNumber();
            int take = GetRandomNumber();

            var invalidListenerEventArchiveV2ProcessingException =
                new InvalidListenerEventArchiveV2ProcessingException(
                    message: "Listener event archive is invalid, fix the errors and try again.");

            invalidListenerEventArchiveV2ProcessingException.UpsertDataList(
                key: "skip",
                value: "Value must be greater than or equal to 0");

            var expectedException =
                new ListenerEventArchiveV2ProcessingValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidListenerEventArchiveV2ProcessingException);

            // when
            ValueTask<List<ListenerEventArchiveV2>> retrieveBatchTask =
                this.listenerEventArchiveV2ProcessingService
                    .RetrieveBatchOfListenerEventArchiveV2sAsync(
                        eventAddressId: null,
                        eventListenerIds: null,
                        startDate: null,
                        endDate: null,
                        skip: invalidSkip,
                        take: take,
                        cancellationToken: randomCancellationToken);

            // then
            ListenerEventArchiveV2ProcessingValidationException actualException =
                await Assert.ThrowsAsync<
                    ListenerEventArchiveV2ProcessingValidationException>(
                        retrieveBatchTask.AsTask);

            actualException.Should().BeEquivalentTo(expectedException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Never);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveBatchOfListenerEventArchivesIfTakeIsInvalidAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int skip = GetRandomNumber();
            int invalidTake = GetRandomNegativeNumber();

            var invalidListenerEventArchiveV2ProcessingException =
                new InvalidListenerEventArchiveV2ProcessingException(
                    message: "Listener event archive is invalid, fix the errors and try again.");

            invalidListenerEventArchiveV2ProcessingException.UpsertDataList(
                key: "take",
                value: "Value must be greater than or equal to 0");

            var expectedException =
                new ListenerEventArchiveV2ProcessingValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidListenerEventArchiveV2ProcessingException);

            // when
            ValueTask<List<ListenerEventArchiveV2>> retrieveBatchTask =
                this.listenerEventArchiveV2ProcessingService
                    .RetrieveBatchOfListenerEventArchiveV2sAsync(
                        eventAddressId: null,
                        eventListenerIds: null,
                        startDate: null,
                        endDate: null,
                        skip: skip,
                        take: invalidTake,
                        cancellationToken: randomCancellationToken);

            // then
            ListenerEventArchiveV2ProcessingValidationException actualException =
                await Assert.ThrowsAsync<
                    ListenerEventArchiveV2ProcessingValidationException>(
                        retrieveBatchTask.AsTask);

            actualException.Should().BeEquivalentTo(expectedException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Never);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
