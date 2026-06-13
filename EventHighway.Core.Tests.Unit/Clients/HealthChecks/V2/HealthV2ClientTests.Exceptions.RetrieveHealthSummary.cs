// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.HealthChecks.V2.Exceptions;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.HealthChecks.V2
{
    public partial class HealthV2ClientTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowValidationExceptionOnRetrieveHealthSummaryIfValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedHealthV2ClientValidationException =
                new HealthV2ClientValidationException(
                    message: "Health client validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption,
                    data: (validationException.InnerException as Xeption).Data);

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveHealthSummaryV2Async(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(validationException);

            // when
            ValueTask<IEnumerable<HealthCheckItemV2>> retrieveTask =
                this.healthV2Client.RetrieveHealthSummaryV2Async(randomCancellationToken);

            HealthV2ClientValidationException actualException =
                await Assert.ThrowsAsync<HealthV2ClientValidationException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthV2ClientValidationException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveHealthSummaryV2Async(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveHealthSummaryIfDependencyErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var healthV2CoordinationDependencyException =
                new HealthV2CoordinationDependencyException(
                    someMessage,
                    someInnerException);

            var expectedHealthV2ClientDependencyException =
                new HealthV2ClientDependencyException(
                    message: "Health client dependency error occurred, contact support.",
                    innerException: healthV2CoordinationDependencyException.InnerException as Xeption,
                    data: (healthV2CoordinationDependencyException.InnerException as Xeption).Data);

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveHealthSummaryV2Async(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(healthV2CoordinationDependencyException);

            // when
            ValueTask<IEnumerable<HealthCheckItemV2>> retrieveTask =
                this.healthV2Client.RetrieveHealthSummaryV2Async(randomCancellationToken);

            HealthV2ClientDependencyException actualException =
                await Assert.ThrowsAsync<HealthV2ClientDependencyException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthV2ClientDependencyException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveHealthSummaryV2Async(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveHealthSummaryIfServiceErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var healthV2CoordinationServiceException =
                new HealthV2CoordinationServiceException(
                    someMessage,
                    someInnerException);

            var expectedHealthV2ClientDependencyException =
                new HealthV2ClientDependencyException(
                    message: "Health client dependency error occurred, contact support.",
                    innerException: healthV2CoordinationServiceException.InnerException as Xeption,
                    data: (healthV2CoordinationServiceException.InnerException as Xeption).Data);

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveHealthSummaryV2Async(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(healthV2CoordinationServiceException);

            // when
            ValueTask<IEnumerable<HealthCheckItemV2>> retrieveTask =
                this.healthV2Client.RetrieveHealthSummaryV2Async(randomCancellationToken);

            HealthV2ClientDependencyException actualException =
                await Assert.ThrowsAsync<HealthV2ClientDependencyException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthV2ClientDependencyException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveHealthSummaryV2Async(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
