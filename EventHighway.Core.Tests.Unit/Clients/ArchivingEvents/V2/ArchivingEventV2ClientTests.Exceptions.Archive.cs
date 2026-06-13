// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Coordinations.ArchivingEvents.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.ArchivingEvents.V2
{
    public partial class ArchivingEventV2ClientTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowValidationExceptionOnArchiveIfValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedArchivingEventV2ClientValidationException =
                new ArchivingEventV2ClientValidationException(
                    message: "Archiving event client validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption,
                    data: (validationException.InnerException as Xeption).Data);

            this.archivingEventV2CoordinationServiceMock.Setup(service =>
                service.ArchiveDeadEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(validationException);

            // when
            ValueTask archiveDeadEventV2sTask =
                this.archivingEventV2Client.ArchiveDeadEventV2sAsync(randomCancellationToken);

            ArchivingEventV2ClientValidationException actualArchivingEventV2ClientValidationException =
                await Assert.ThrowsAsync<ArchivingEventV2ClientValidationException>(
                    archiveDeadEventV2sTask.AsTask);

            // then
            actualArchivingEventV2ClientValidationException.Should()
                .BeEquivalentTo(expectedArchivingEventV2ClientValidationException);

            this.archivingEventV2CoordinationServiceMock.Verify(service =>
                service.ArchiveDeadEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.archivingEventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnArchiveIfDependencyErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var archivingEventV2CoordinationDependencyException =
                new ArchivingEventV2CoordinationDependencyException(
                    someMessage,
                    someInnerException);

            var expectedArchivingEventV2ClientDependencyException =
                new ArchivingEventV2ClientDependencyException(
                    message: "Archiving event client dependency error occurred, contact support.",

                    innerException: archivingEventV2CoordinationDependencyException
                        .InnerException as Xeption,

                    data: (archivingEventV2CoordinationDependencyException
                        .InnerException as Xeption).Data);

            this.archivingEventV2CoordinationServiceMock.Setup(service =>
                service.ArchiveDeadEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(archivingEventV2CoordinationDependencyException);

            // when
            ValueTask archiveDeadEventV2sTask =
                this.archivingEventV2Client.ArchiveDeadEventV2sAsync(randomCancellationToken);

            ArchivingEventV2ClientDependencyException actualArchivingEventV2ClientDependencyException =
                await Assert.ThrowsAsync<ArchivingEventV2ClientDependencyException>(
                    archiveDeadEventV2sTask.AsTask);

            // then
            actualArchivingEventV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedArchivingEventV2ClientDependencyException);

            this.archivingEventV2CoordinationServiceMock.Verify(service =>
                service.ArchiveDeadEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.archivingEventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnArchiveIfServiceErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var archivingEventV2CoordinationServiceException =
                new ArchivingEventV2CoordinationServiceException(
                    someMessage,
                    someInnerException);

            var expectedArchivingEventV2ClientDependencyException =
                new ArchivingEventV2ClientDependencyException(
                    message: "Archiving event client dependency error occurred, contact support.",

                    innerException: archivingEventV2CoordinationServiceException
                        .InnerException as Xeption,

                    data: (archivingEventV2CoordinationServiceException
                        .InnerException as Xeption).Data);

            this.archivingEventV2CoordinationServiceMock.Setup(service =>
                service.ArchiveDeadEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(archivingEventV2CoordinationServiceException);

            // when
            ValueTask archiveDeadEventV2sTask =
                this.archivingEventV2Client.ArchiveDeadEventV2sAsync(randomCancellationToken);

            ArchivingEventV2ClientDependencyException actualArchivingEventV2ClientDependencyException =
                await Assert.ThrowsAsync<ArchivingEventV2ClientDependencyException>(
                    archiveDeadEventV2sTask.AsTask);

            // then
            actualArchivingEventV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedArchivingEventV2ClientDependencyException);

            this.archivingEventV2CoordinationServiceMock.Verify(service =>
                service.ArchiveDeadEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.archivingEventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
