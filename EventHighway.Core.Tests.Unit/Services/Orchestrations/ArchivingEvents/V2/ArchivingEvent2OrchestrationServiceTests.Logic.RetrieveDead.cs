// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.ArchivingEvents.V2
{
    public partial class ArchivingEvent2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllDeadEventV2sWithListenersAsync()
        {
            // given
            IQueryable<EventV2> randomEventV2s = CreateRandomEventV2s();
            IQueryable<EventV2> retrievedEventV2s = randomEventV2s;
            List<EventV2> expectedEventV2s = retrievedEventV2s.DeepClone().ToList();

            this.eventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllDeadEventV2sWithListenersAsync())
                    .ReturnsAsync(retrievedEventV2s);

            // when
            var actualEventV2s = new List<EventV2>();

            await foreach (EventV2 eventV2 in
                this.archivingEvent2OrchestrationService
                    .RetrieveAllDeadEventV2sWithListenersAsync(
                        TestContext.Current.CancellationToken))
            {
                actualEventV2s.Add(eventV2);
            }

            // then
            actualEventV2s.Should().BeEquivalentTo(expectedEventV2s);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllDeadEventV2sWithListenersAsync(),
                    Times.Once);

            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
