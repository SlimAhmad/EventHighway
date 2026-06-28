// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventParticipants.V2.Exceptions;
using EventHighway.Portal.Web.Models.Views.EventParticipants.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.EventParticipants
{
    public partial class EventParticipantsViewServiceTests
    {
        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            var someInnerException = new Xeption(message: GetRandomString());

            return new TheoryData<Xeption>
            {
                new EventParticipantV2ClientValidationException(
                    message: GetRandomString(),
                    innerException: someInnerException,
                    data: new Hashtable()),

                new EventParticipantV2ClientDependencyValidationException(
                    message: GetRandomString(),
                    innerException: someInnerException,
                    data: new Hashtable()),
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            var someInnerException = new Xeption(message: GetRandomString());

            return new TheoryData<Xeption>
            {
                new EventParticipantV2ClientDependencyException(
                    message: GetRandomString(),
                    innerException: someInnerException,
                    data: new Hashtable()),

                new EventParticipantV2ClientServiceException(
                    message: GetRandomString(),
                    innerException: someInnerException,
                    data: new Hashtable()),
            };
        }

        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnRetrieveAllIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            var expectedViewDependencyValidationException =
                new EventParticipantsViewDependencyValidationException(
                    innerException: dependencyValidationException);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllEventParticipantV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyValidationException);

            // when
            EventParticipantsViewDependencyValidationException actualException =
                await Assert.ThrowsAsync<EventParticipantsViewDependencyValidationException>(
                    async () => await this.eventParticipantsViewService
                        .RetrieveAllParticipantsAsync(TestContext.Current.CancellationToken));

            // then
            actualException.Should().BeEquivalentTo(expectedViewDependencyValidationException);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveAllEventParticipantV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
