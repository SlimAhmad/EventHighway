// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.PromotedProperties;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventCalls.V2
{
    public partial class EventCallV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldPromotePropertiesAsync()
        {
            // given
            string inputContent = "{ \"name\": \"value\", \"age\": \"42\" }";
            string inputPromotedProperties = "name, age";

            var expectedPromotedProperties = new List<PromotedProperty>
            {
                new PromotedProperty { Name = "name", Value = "value" },
                new PromotedProperty { Name = "age", Value = "42" }
            };

            var jsonBrokerSequence = new MockSequence();

            this.jsonBrokerMock.InSequence(jsonBrokerSequence).Setup(broker =>
                broker.CheckIfPropertyExist(inputContent, "name"))
                    .Returns(true);

            this.jsonBrokerMock.InSequence(jsonBrokerSequence).Setup(broker =>
                broker.GetJsonPropertyValue(inputContent, "name"))
                    .Returns("value");

            this.jsonBrokerMock.InSequence(jsonBrokerSequence).Setup(broker =>
                broker.CheckIfPropertyExist(inputContent, "age"))
                    .Returns(true);

            this.jsonBrokerMock.InSequence(jsonBrokerSequence).Setup(broker =>
                broker.GetJsonPropertyValue(inputContent, "age"))
                    .Returns("42");

            // when
            List<PromotedProperty> actualPromotedProperties =
                await this.eventCallV2ProcessingService.PromotePropertiesAsync(
                    inputContent,
                    inputPromotedProperties,
                    TestContext.Current.CancellationToken);

            // then
            actualPromotedProperties.Should().BeEquivalentTo(
                expectedPromotedProperties);

            this.jsonBrokerMock.Verify(broker =>
                broker.CheckIfPropertyExist(inputContent, "name"),
                    Times.Once);

            this.jsonBrokerMock.Verify(broker =>
                broker.GetJsonPropertyValue(inputContent, "name"),
                    Times.Once);

            this.jsonBrokerMock.Verify(broker =>
                broker.CheckIfPropertyExist(inputContent, "age"),
                    Times.Once);

            this.jsonBrokerMock.Verify(broker =>
                broker.GetJsonPropertyValue(inputContent, "age"),
                    Times.Once);

            this.eventCallV2ServiceMock.VerifyNoOtherCalls();
            this.jsonBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
