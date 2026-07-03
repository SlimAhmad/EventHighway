// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.HealthChecks.V2.Exceptions;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Orchestrations.ParticipantSummaries.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.HealthChecks.V2
{
    public partial class HealthParticipantClientV2Tests
    {
        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveParticipantSummaryIfDependencyErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.Data.Add("ErrorCode", new List<string> { "DependencyError" });

            var participantSummaryV2OrchestrationDependencyException =
                new ParticipantSummaryV2OrchestrationDependencyException(
                    someMessage,
                    someInnerException);

            var expectedHealthParticipantClientV2DependencyException =
                new HealthParticipantClientV2DependencyException(
                    message: "Health client dependency error occurred, contact support.",
                    innerException: participantSummaryV2OrchestrationDependencyException.InnerException as Xeption,
                    data: (participantSummaryV2OrchestrationDependencyException.InnerException as Xeption).Data);

            this.participantSummaryV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveParticipantSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(participantSummaryV2OrchestrationDependencyException);

            // when
            ValueTask<IEnumerable<ParticipantSummaryV2>> retrieveTask =
                this.healthParticipantClientV2.RetrieveParticipantSummaryV2Async(
                    GetRandomTrafficPeriodV2(), GetRandomDateTimeOffset(), randomCancellationToken);

            HealthParticipantClientV2DependencyException actualException =
                await Assert.ThrowsAsync<HealthParticipantClientV2DependencyException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthParticipantClientV2DependencyException);

            this.participantSummaryV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveParticipantSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.participantSummaryV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveParticipantSummaryIfServiceErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var participantSummaryV2OrchestrationServiceException =
                new ParticipantSummaryV2OrchestrationServiceException(
                    someMessage,
                    someInnerException);

            var expectedHealthParticipantClientV2DependencyException =
                new HealthParticipantClientV2DependencyException(
                    message: "Health client dependency error occurred, contact support.",
                    innerException: participantSummaryV2OrchestrationServiceException.InnerException as Xeption,
                    data: (participantSummaryV2OrchestrationServiceException.InnerException as Xeption).Data);

            this.participantSummaryV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveParticipantSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(participantSummaryV2OrchestrationServiceException);

            // when
            ValueTask<IEnumerable<ParticipantSummaryV2>> retrieveTask =
                this.healthParticipantClientV2.RetrieveParticipantSummaryV2Async(
                    GetRandomTrafficPeriodV2(), GetRandomDateTimeOffset(), randomCancellationToken);

            HealthParticipantClientV2DependencyException actualException =
                await Assert.ThrowsAsync<HealthParticipantClientV2DependencyException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthParticipantClientV2DependencyException);

            this.participantSummaryV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveParticipantSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.participantSummaryV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveParticipantSummaryIfUnexpectedErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var someXeption = new Xeption(message: GetRandomString());

            var expectedHealthParticipantClientV2ServiceException =
                new HealthParticipantClientV2ServiceException(
                    message: "Health client service error occurred, contact support.",
                    innerException: someXeption,
                    data: someXeption.Data);

            this.participantSummaryV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveParticipantSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(someXeption);

            // when
            ValueTask<IEnumerable<ParticipantSummaryV2>> retrieveTask =
                this.healthParticipantClientV2.RetrieveParticipantSummaryV2Async(
                    GetRandomTrafficPeriodV2(), GetRandomDateTimeOffset(), randomCancellationToken);

            HealthParticipantClientV2ServiceException actualException =
                await Assert.ThrowsAsync<HealthParticipantClientV2ServiceException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthParticipantClientV2ServiceException);

            this.participantSummaryV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveParticipantSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.participantSummaryV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveParticipantSummaryAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var operationCanceledException =
                new OperationCanceledException();

            this.participantSummaryV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveParticipantSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IEnumerable<ParticipantSummaryV2>> retrieveTask =
                this.healthParticipantClientV2.RetrieveParticipantSummaryV2Async(
                    GetRandomTrafficPeriodV2(), GetRandomDateTimeOffset(), randomCancellationToken);

            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(operationCanceledException);

            this.participantSummaryV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveParticipantSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.participantSummaryV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
