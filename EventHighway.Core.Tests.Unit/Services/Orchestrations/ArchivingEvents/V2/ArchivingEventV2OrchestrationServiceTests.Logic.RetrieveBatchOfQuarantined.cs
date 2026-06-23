// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Configurations.LoopDetections;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.ArchivingEvents.V2
{
    public partial class ArchivingEventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveBatchOfQuarantinedEventV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset retrievedDateTimeOffset = GetRandomDateTimeOffset();

            var loopDetection = new LoopDetection
            {
                Window = TimeSpan.FromDays(30)
            };

            DateTimeOffset cutoff = retrievedDateTimeOffset - loopDetection.Window;

            BatchConfiguration randomBatchConfiguration = CreateRandomBatchConfiguration();
            BatchConfiguration retrievedBatchConfiguration = randomBatchConfiguration;
            int inputTake = retrievedBatchConfiguration.BatchSizeForBulkProcessing;

            var oldQuarantinedEventV2 = new EventV2
            {
                Status = EventStatusV2.Quarantined,
                CreatedDate = retrievedDateTimeOffset.AddDays(-31)
            };

            var recentQuarantinedEventV2 = new EventV2
            {
                Status = EventStatusV2.Quarantined,
                CreatedDate = retrievedDateTimeOffset.AddMinutes(-30)
            };

            var activeEventV2 = new EventV2
            {
                Status = EventStatusV2.Active,
                CreatedDate = retrievedDateTimeOffset.AddDays(-31)
            };

            IQueryable<EventV2> allEventV2s =
                new[] { oldQuarantinedEventV2, recentQuarantinedEventV2, activeEventV2 }
                    .AsQueryable();

            IEnumerable<EventV2> expectedEventV2s =
                new[] { oldQuarantinedEventV2 }.AsQueryable()
                    .Take(inputTake);

            var mockSequence = new MockSequence();

            this.dateTimeBrokerMock
                .InSequence(mockSequence)
                .Setup(broker => broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(retrievedDateTimeOffset);

            this.configurationBrokerMock
                .InSequence(mockSequence)
                .Setup(broker => broker.GetBatchConfiguration())
                    .Returns(retrievedBatchConfiguration);

            this.configurationBrokerMock
                .InSequence(mockSequence)
                .Setup(broker => broker.GetLoopDetectionConfiguration())
                    .Returns(loopDetection);

            this.eventV2ProcessingServiceMock
                .InSequence(mockSequence)
                .Setup(service => service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(allEventV2s);

            // when
            IEnumerable<EventV2> actualEventV2s =
                await this.archivingEventV2OrchestrationService
                    .RetrieveBatchOfQuarantinedEventV2sAsync(
                        randomCancellationToken);

            // then
            actualEventV2s.Should().BeEquivalentTo(expectedEventV2s);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetLoopDetectionConfiguration(),
                    Times.Once);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
