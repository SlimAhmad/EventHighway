// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.Events.V2
{
    public partial class EventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldSplitPromotedPropertyKeysAsync()
        {
            // given
            CancellationToken randomCancellationToken = TestContext.Current.CancellationToken;
            string randomPromotedProperties = GetRandomString();
            string inputPromotedProperties = randomPromotedProperties;

            IEnumerable<string> randomKeys = new List<string>
            {
                GetRandomString(),
                GetRandomString()
            };

            IEnumerable<string> returnedKeys = randomKeys;
            IEnumerable<string> expectedKeys = returnedKeys;

            this.eventCallV2ProcessingServiceMock.Setup(service =>
                service.SplitPromotedPropertyKeysAsync(
                    inputPromotedProperties,
                    randomCancellationToken))
                        .ReturnsAsync(returnedKeys);

            // when
            IEnumerable<string> actualKeys =
                await this.eventV2OrchestrationService
                    .SplitPromotedPropertyKeysAsync(
                        inputPromotedProperties,
                        randomCancellationToken);

            // then
            actualKeys.Should().BeEquivalentTo(expectedKeys);

            this.eventCallV2ProcessingServiceMock.Verify(service =>
                service.SplitPromotedPropertyKeysAsync(
                    inputPromotedProperties,
                    randomCancellationToken),
                        Times.Once);

            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventAddressV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
