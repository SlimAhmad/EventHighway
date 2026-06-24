// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventCalls.V2
{
    public partial class EventCallV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldSplitPromotedPropertyKeysAsync()
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
            IEnumerable<string> actualKeys =
                await this.eventCallV2ProcessingService
                    .SplitPromotedPropertyKeysAsync(
                        inputPromotedProperties,
                        TestContext.Current.CancellationToken);

            // then
            actualKeys.ToList().Should().BeEquivalentTo(expectedKeys);

            this.eventCallV2ServiceMock.VerifyNoOtherCalls();
            this.jsonBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

    }
}
