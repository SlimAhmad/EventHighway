// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.PromotedProperties;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.Events.V2
{
    public partial class EventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldPromotePropertiesAsync()
        {
            // given
            CancellationToken randomCancellationToken = TestContext.Current.CancellationToken;
            string randomContent = GetRandomString();
            string randomPromotedProperties = GetRandomString();
            string inputContent = randomContent;
            string inputPromotedProperties = randomPromotedProperties;

            var returnedPromotedProperties = new List<PromotedProperty>
            {
                new PromotedProperty { Name = GetRandomString(), Value = GetRandomString() }
            };

            List<PromotedProperty> expectedPromotedProperties = returnedPromotedProperties;

            this.eventCallV2ProcessingServiceMock.Setup(service =>
                service.PromotePropertiesAsync(
                    inputContent,
                    inputPromotedProperties,
                    randomCancellationToken))
                        .ReturnsAsync(returnedPromotedProperties);

            // when
            List<PromotedProperty> actualPromotedProperties =
                await this.eventV2OrchestrationService
                    .PromotePropertiesAsync(
                        inputContent,
                        inputPromotedProperties,
                        randomCancellationToken);

            // then
            actualPromotedProperties.Should().BeEquivalentTo(expectedPromotedProperties);

            this.eventCallV2ProcessingServiceMock.Verify(service =>
                service.PromotePropertiesAsync(
                    inputContent,
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
