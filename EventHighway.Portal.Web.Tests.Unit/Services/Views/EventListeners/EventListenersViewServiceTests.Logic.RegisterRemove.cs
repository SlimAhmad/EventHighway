// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Portal.Web.Models.Views.EventListeners;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.EventListeners
{
    public partial class EventListenersViewServiceTests
    {
        [Fact]
        public async Task ShouldRegisterListenerAsync()
        {
            // given
            Guid addressId = Guid.NewGuid();
            DateTimeOffset now = GetRandomDateTimeOffset();

            EventListenerView inputView = new EventListenerView
            {
                Name = GetRandomString(),
                Description = GetRandomString(),
                HandlerName = GetRandomString(),
                HandlerId = Guid.NewGuid(),
                EventAddressId = addressId
            };

            EventListenerV2 returnedListener = new EventListenerV2
            {
                Id = Guid.NewGuid(),
                Name = inputView.Name,
                Description = inputView.Description,
                HandlerName = inputView.HandlerName,
                HandlerId = inputView.HandlerId,
                EventAddressId = addressId,
                CreatedDate = now,
                UpdatedDate = now
            };

            EventListenerView expectedView =
                MapToViews(new[] { returnedListener })[0];

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RegisterEventListenerV2Async(
                    It.IsAny<EventListenerV2>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(returnedListener);

            // when
            EventListenerView actualView =
                await this.eventListenersViewService.RegisterListenerAsync(
                    inputView, TestContext.Current.CancellationToken);

            // then
            actualView.Should().BeEquivalentTo(expectedView);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RegisterEventListenerV2Async(
                    It.Is<EventListenerV2>(listener =>
                        listener.Name == inputView.Name
                        && listener.HandlerId == inputView.HandlerId
                        && listener.EventAddressId == addressId
                        && listener.CreatedDate == now
                        && listener.UpdatedDate == now),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRemoveListenerByIdAsync()
        {
            // given
            Guid addressId = Guid.NewGuid();

            EventListenerV2 removedListener =
                CreateRandomListeners(addressId, count: 1)[0];

            Guid listenerId = removedListener.Id;

            EventListenerView expectedView =
                MapToViews(new[] { removedListener })[0];

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RemoveEventListenerV2ByIdAsync(
                    listenerId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(removedListener);

            // when
            EventListenerView actualView =
                await this.eventListenersViewService.RemoveListenerByIdAsync(
                    listenerId, TestContext.Current.CancellationToken);

            // then
            actualView.Should().BeEquivalentTo(expectedView);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RemoveEventListenerV2ByIdAsync(
                    listenerId, It.IsAny<CancellationToken>()),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
