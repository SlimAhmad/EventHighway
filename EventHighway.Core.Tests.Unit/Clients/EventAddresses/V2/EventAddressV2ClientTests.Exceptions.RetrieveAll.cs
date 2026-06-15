// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventAddresses.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.EventAddresses.V2
{
    public partial class EventAddressV2ClientTests
    {
        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveAllIfDependencyErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var eventAddressV2DependencyException =
                new EventAddressV2DependencyException(
                    someMessage,
                    someInnerException);

            var expectedEventAddressV2ClientDependencyException =
                new EventAddressV2ClientDependencyException(
                    message: "Event address client dependency error occurred, contact support.",

                    innerException: eventAddressV2DependencyException
                        .InnerException as Xeption,

                    data: (eventAddressV2DependencyException
                        .InnerException as Xeption).Data);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sAsync())
                    .ThrowsAsync(eventAddressV2DependencyException);

            // when
            ValueTask<IQueryable<EventAddressV2>> retrieveAllEventAddressV2sTask =
                this.eventAddressV2Client.RetrieveAllEventAddressV2sAsync(randomCancellationToken);

            EventAddressV2ClientDependencyException actualEventAddressV2ClientDependencyException =
                await Assert.ThrowsAsync<EventAddressV2ClientDependencyException>(
                    retrieveAllEventAddressV2sTask.AsTask);

            // then
            actualEventAddressV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedEventAddressV2ClientDependencyException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sAsync(),
                    Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveAllIfServiceErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var eventAddressV2ServiceException =
                new EventAddressV2ServiceException(
                    someMessage,
                    someInnerException);

            var expectedEventAddressV2ClientDependencyException =
                new EventAddressV2ClientDependencyException(
                    message: "Event address client dependency error occurred, contact support.",

                    innerException: eventAddressV2ServiceException
                        .InnerException as Xeption,

                    data: (eventAddressV2ServiceException
                        .InnerException as Xeption).Data);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sAsync())
                    .ThrowsAsync(eventAddressV2ServiceException);

            // when
            ValueTask<IQueryable<EventAddressV2>> retrieveAllEventAddressV2sTask =
                this.eventAddressV2Client.RetrieveAllEventAddressV2sAsync(randomCancellationToken);

            EventAddressV2ClientDependencyException actualEventAddressV2ClientDependencyException =
                await Assert.ThrowsAsync<EventAddressV2ClientDependencyException>(
                    retrieveAllEventAddressV2sTask.AsTask);

            // then
            actualEventAddressV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedEventAddressV2ClientDependencyException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sAsync(),
                    Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
