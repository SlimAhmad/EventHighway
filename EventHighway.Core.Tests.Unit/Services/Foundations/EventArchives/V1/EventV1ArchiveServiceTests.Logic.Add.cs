// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventArchives.V1
{
    public partial class EventV1ArchiveServiceTests
    {
        [Fact]
        public async Task ShouldAddEventV1ArchiveAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset =
                GetRandomDateTimeOffset();

            EventArchiveV1 randomEventV1Archive =
                CreateRandomEventV1Archive(
                    date: randomDateTimeOffset);

            EventArchiveV1 inputEventV1Archive =
                randomEventV1Archive;

            EventArchiveV1 insertedEventV1Archive =
                inputEventV1Archive;

            EventArchiveV1 expectedEventV1Archive =
                insertedEventV1Archive.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertEventV1ArchiveAsync(
                    inputEventV1Archive))
                        .ReturnsAsync(insertedEventV1Archive);

            // when
            EventArchiveV1 actualEventV1Archive =
                await this.eventV1ArchiveService
                    .AddEventArchiveV1Async(
                        inputEventV1Archive);

            // then
            actualEventV1Archive.Should().BeEquivalentTo(
                expectedEventV1Archive);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventV1ArchiveAsync(
                    inputEventV1Archive),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
