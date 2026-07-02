// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V2
{
    public partial class EventArchiveV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldBulkAddEventArchiveV2sWithListenerEventArchiveV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEnumerable<EventArchiveV2> randomEventArchiveV2s =
                CreateRandomEventArchiveV2s();

            IEnumerable<EventArchiveV2> inputEventArchiveV2s =
                randomEventArchiveV2s;

            IEnumerable<ListenerEventArchiveV2> inputListenerEventArchiveV2s =
                inputEventArchiveV2s
                    .SelectMany(eventArchiveV2 => eventArchiveV2.ListenerEventArchiveV2s)
                    .ToList();

            IEnumerable<ListenerEventArchiveV2> archivedListenerEventArchiveV2s =
                inputListenerEventArchiveV2s;

            IEnumerable<EventArchiveV2> expectedEventArchiveV2s =
                inputEventArchiveV2s.DeepClone();

            this.listenerEventArchiveV2ProcessingServiceMock.Setup(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    It.Is<IEnumerable<ListenerEventArchiveV2>>(actualListenerEventArchiveV2s =>
                        actualListenerEventArchiveV2s.SequenceEqual(inputListenerEventArchiveV2s)),
                    randomCancellationToken))
                        .ReturnsAsync(archivedListenerEventArchiveV2s);

            // when
            IEnumerable<EventArchiveV2> actualEventArchiveV2s =
                await this.eventArchiveV2OrchestrationService
                    .BulkAddEventArchiveV2sWithListenerEventArchiveV2sAsync(
                        inputEventArchiveV2s,
                        randomCancellationToken);

            // then
            actualEventArchiveV2s.Should()
                .BeEquivalentTo(expectedEventArchiveV2s);

            this.listenerEventArchiveV2ProcessingServiceMock.Verify(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    It.Is<IEnumerable<ListenerEventArchiveV2>>(actualListenerEventArchiveV2s =>
                        actualListenerEventArchiveV2s.SequenceEqual(inputListenerEventArchiveV2s)),
                    randomCancellationToken),
                        Times.Once);

            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldReturnOnlyArchivedListenerEventArchiveV2sOnBulkAddEventArchiveV2sWithListenerEventArchiveV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            ListenerEventArchiveV2 firstArchivedListenerEventArchiveV2 =
                CreateRandomListenerEventArchiveV2();

            ListenerEventArchiveV2 firstUnarchivedListenerEventArchiveV2 =
                CreateRandomListenerEventArchiveV2();

            ListenerEventArchiveV2 secondArchivedListenerEventArchiveV2 =
                CreateRandomListenerEventArchiveV2();

            ListenerEventArchiveV2 secondUnarchivedListenerEventArchiveV2 =
                CreateRandomListenerEventArchiveV2();

            EventArchiveV2 firstEventArchiveV2 = CreateRandomEventArchiveV2();

            firstEventArchiveV2.ListenerEventArchiveV2s =
                new List<ListenerEventArchiveV2>
                {
                    firstArchivedListenerEventArchiveV2,
                    firstUnarchivedListenerEventArchiveV2
                };

            EventArchiveV2 secondEventArchiveV2 = CreateRandomEventArchiveV2();

            secondEventArchiveV2.ListenerEventArchiveV2s =
                new List<ListenerEventArchiveV2>
                {
                    secondArchivedListenerEventArchiveV2,
                    secondUnarchivedListenerEventArchiveV2
                };

            List<EventArchiveV2> inputEventArchiveV2s =
                new List<EventArchiveV2> { firstEventArchiveV2, secondEventArchiveV2 };

            List<ListenerEventArchiveV2> inputListenerEventArchiveV2s =
                new List<ListenerEventArchiveV2>
                {
                    firstArchivedListenerEventArchiveV2,
                    firstUnarchivedListenerEventArchiveV2,
                    secondArchivedListenerEventArchiveV2,
                    secondUnarchivedListenerEventArchiveV2
                };

            List<ListenerEventArchiveV2> archivedListenerEventArchiveV2s =
                new List<ListenerEventArchiveV2>
                {
                    firstArchivedListenerEventArchiveV2,
                    secondArchivedListenerEventArchiveV2
                };

            EventArchiveV2 firstExpectedEventArchiveV2 = firstEventArchiveV2.DeepClone();

            firstExpectedEventArchiveV2.ListenerEventArchiveV2s =
                new List<ListenerEventArchiveV2> { firstArchivedListenerEventArchiveV2 };

            EventArchiveV2 secondExpectedEventArchiveV2 = secondEventArchiveV2.DeepClone();

            secondExpectedEventArchiveV2.ListenerEventArchiveV2s =
                new List<ListenerEventArchiveV2> { secondArchivedListenerEventArchiveV2 };

            List<EventArchiveV2> expectedEventArchiveV2s =
                new List<EventArchiveV2> { firstExpectedEventArchiveV2, secondExpectedEventArchiveV2 };

            this.listenerEventArchiveV2ProcessingServiceMock.Setup(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    It.Is<IEnumerable<ListenerEventArchiveV2>>(actualListenerEventArchiveV2s =>
                        actualListenerEventArchiveV2s.SequenceEqual(inputListenerEventArchiveV2s)),
                    randomCancellationToken))
                        .ReturnsAsync(archivedListenerEventArchiveV2s);

            // when
            IEnumerable<EventArchiveV2> actualEventArchiveV2s =
                await this.eventArchiveV2OrchestrationService
                    .BulkAddEventArchiveV2sWithListenerEventArchiveV2sAsync(
                        inputEventArchiveV2s,
                        randomCancellationToken);

            // then
            actualEventArchiveV2s.Should()
                .BeEquivalentTo(expectedEventArchiveV2s);

            this.listenerEventArchiveV2ProcessingServiceMock.Verify(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    It.Is<IEnumerable<ListenerEventArchiveV2>>(actualListenerEventArchiveV2s =>
                        actualListenerEventArchiveV2s.SequenceEqual(inputListenerEventArchiveV2s)),
                    randomCancellationToken),
                        Times.Once);

            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
