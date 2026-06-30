// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.ListenerEventArchives.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.ListenerEventArchives.V2
{
    public partial class ListenerEventArchiveV2ClientTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowValidationExceptionOnRetrieveAllWithEventListenerV2IfValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedListenerEventArchiveV2ClientValidationException =
                new ListenerEventArchiveV2ClientValidationException(
                    message: "Listener event archive client validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption,
                    data: (validationException.InnerException as Xeption).Data);

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sWithEventListenerV2Async(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<IQueryable<ListenerEventArchiveV2>> retrieveAllTask =
                this.listenerEventArchiveV2Client
                    .RetrieveAllListenerEventArchiveV2sWithEventListenerV2Async(
                        randomCancellationToken);

            ListenerEventArchiveV2ClientValidationException actualListenerEventArchiveV2ClientValidationException =
                await Assert.ThrowsAsync<ListenerEventArchiveV2ClientValidationException>(
                    retrieveAllTask.AsTask);

            // then
            actualListenerEventArchiveV2ClientValidationException.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV2ClientValidationException);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sWithEventListenerV2Async(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveAllWithEventListenerV2IfDependencyErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var listenerEventArchiveV2DependencyException =
                new ListenerEventArchiveV2DependencyException(
                    someMessage,
                    someInnerException);

            var expectedListenerEventArchiveV2ClientDependencyException =
                new ListenerEventArchiveV2ClientDependencyException(
                    message: "Listener event archive client dependency error occurred, contact support.",
                    innerException: listenerEventArchiveV2DependencyException.InnerException as Xeption,
                    data: (listenerEventArchiveV2DependencyException.InnerException as Xeption).Data);

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sWithEventListenerV2Async(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(listenerEventArchiveV2DependencyException);

            // when
            ValueTask<IQueryable<ListenerEventArchiveV2>> retrieveAllTask =
                this.listenerEventArchiveV2Client
                    .RetrieveAllListenerEventArchiveV2sWithEventListenerV2Async(
                        randomCancellationToken);

            ListenerEventArchiveV2ClientDependencyException actualListenerEventArchiveV2ClientDependencyException =
                await Assert.ThrowsAsync<ListenerEventArchiveV2ClientDependencyException>(
                    retrieveAllTask.AsTask);

            // then
            actualListenerEventArchiveV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV2ClientDependencyException);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sWithEventListenerV2Async(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
