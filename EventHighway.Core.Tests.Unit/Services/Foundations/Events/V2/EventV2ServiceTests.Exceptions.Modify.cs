// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.Events.V2
{
    public partial class EventV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfTimeoutOccursAndLogItAsync()
        {
            // given
            EventV2 someEventV2 = CreateRandomEventV2();
            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutEventV2Exception =
                new TimeoutEventV2Exception(
                    message: "Failed event timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedEventV2DependencyException =
                new EventV2DependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: timeoutEventV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<EventV2> modifyEventV2Task =
                this.eventV2Service.ModifyEventV2Async(
                    someEventV2, TestContext.Current.CancellationToken);

            EventV2DependencyException actualEventV2DependencyException =
                await Assert.ThrowsAsync<EventV2DependencyException>(
                    modifyEventV2Task.AsTask);

            // then
            actualEventV2DependencyException.Should().BeEquivalentTo(
                expectedEventV2DependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2DependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventV2ByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEventV2Async(It.IsAny<EventV2>(), It.IsAny<CancellationToken>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnModifyAsync()
        {
            // given
            EventV2 someEventV2 = CreateRandomEventV2();

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<EventV2> modifyEventV2Task =
                this.eventV2Service.ModifyEventV2Async(someEventV2, cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    modifyEventV2Task.AsTask);

            actualException.Should().NotBeOfType<EventV2DependencyException>();
            actualException.Should().NotBeOfType<EventV2ServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
