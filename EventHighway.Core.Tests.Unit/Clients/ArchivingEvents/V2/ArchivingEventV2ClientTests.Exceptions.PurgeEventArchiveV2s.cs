// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
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
        public async Task ShouldThrowValidationExceptionOnPurgeIfValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            DateTimeOffset someOlderThan = GetRandomDateTimeOffset();
            CancellationToken someCancellationToken = TestContext.Current.CancellationToken;

            var expectedArchivingEventV2ClientValidationException =
                new ArchivingEventV2ClientValidationException(
                    message: "Archiving event client validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption,
                    data: (validationException.InnerException as Xeption).Data);

            this.archivingEventV2CoordinationServiceMock.Setup(service =>
                service.PurgeEventArchiveV2sAsync(
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask purgeEventArchiveV2sTask =
                this.archivingEventV2Client
                    .PurgeEventArchiveV2sAsync(someOlderThan, someCancellationToken);

            ArchivingEventV2ClientValidationException actualArchivingEventV2ClientValidationException =
                await Assert.ThrowsAsync<ArchivingEventV2ClientValidationException>(
                    purgeEventArchiveV2sTask.AsTask);

            // then
            actualArchivingEventV2ClientValidationException.Should()
                .BeEquivalentTo(expectedArchivingEventV2ClientValidationException);

            this.archivingEventV2CoordinationServiceMock.Verify(service =>
                service.PurgeEventArchiveV2sAsync(
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.archivingEventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnPurgeIfDependencyErrorOccursAsync()
        {
            // given
            DateTimeOffset someOlderThan = GetRandomDateTimeOffset();
            CancellationToken someCancellationToken = TestContext.Current.CancellationToken;

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
                service.PurgeEventArchiveV2sAsync(
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(archivingEventV2CoordinationDependencyException);

            // when
            ValueTask purgeEventArchiveV2sTask =
                this.archivingEventV2Client
                    .PurgeEventArchiveV2sAsync(someOlderThan, someCancellationToken);

            ArchivingEventV2ClientDependencyException actualArchivingEventV2ClientDependencyException =
                await Assert.ThrowsAsync<ArchivingEventV2ClientDependencyException>(
                    purgeEventArchiveV2sTask.AsTask);

            // then
            actualArchivingEventV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedArchivingEventV2ClientDependencyException);

            this.archivingEventV2CoordinationServiceMock.Verify(service =>
                service.PurgeEventArchiveV2sAsync(
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.archivingEventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnPurgeIfServiceErrorOccursAsync()
        {
            // given
            DateTimeOffset someOlderThan = GetRandomDateTimeOffset();
            CancellationToken someCancellationToken = TestContext.Current.CancellationToken;

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
                service.PurgeEventArchiveV2sAsync(
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(archivingEventV2CoordinationServiceException);

            // when
            ValueTask purgeEventArchiveV2sTask =
                this.archivingEventV2Client
                    .PurgeEventArchiveV2sAsync(someOlderThan, someCancellationToken);

            ArchivingEventV2ClientDependencyException actualArchivingEventV2ClientDependencyException =
                await Assert.ThrowsAsync<ArchivingEventV2ClientDependencyException>(
                    purgeEventArchiveV2sTask.AsTask);

            // then
            actualArchivingEventV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedArchivingEventV2ClientDependencyException);

            this.archivingEventV2CoordinationServiceMock.Verify(service =>
                service.PurgeEventArchiveV2sAsync(
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.archivingEventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

    }
}
