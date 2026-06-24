// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventCalls.V2
{
    public partial class EventCallV2ProcessingServiceTests
    {
        [Fact]
        public void ShouldSplitPromotedPropertyKeys()
        {
            // given
            string inputPromotedProperties = "name, age ,, address";

            var expectedKeys = new List<string>
            {
                "name",
                "age",
                "address"
            };

            // when
            List<string> actualKeys =
                this.eventCallV2ProcessingService
                    .SplitPromotedPropertyKeys(inputPromotedProperties)
                        .ToList();

            // then
            actualKeys.Should().BeEquivalentTo(expectedKeys);

            this.eventCallV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ShouldReturnEmptyKeysWhenPromotedPropertiesIsNullOrWhitespace(
            string inputPromotedProperties)
        {
            // given
            var expectedKeys = new List<string>();

            // when
            List<string> actualKeys =
                this.eventCallV2ProcessingService
                    .SplitPromotedPropertyKeys(inputPromotedProperties)
                        .ToList();

            // then
            actualKeys.Should().BeEquivalentTo(expectedKeys);

            this.eventCallV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
