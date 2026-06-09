// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventCalls.V2
{
    public partial class EventCallV2ServiceTests
    {
        [Fact]
        public async Task ShouldRunEventCallV2Async()
        {
            // given
            string randomHandlerName = GetRandomString();
            string randomConfigName = GetRandomString();
            string randomConfigValue = GetRandomString();

            EventCallV2 randomEventCallV2 = CreateRandomEventCallV2();
            randomEventCallV2.HandlerName = randomHandlerName;

            randomEventCallV2.HandlerConfigurations =
                new List<HandlerConfiguration>
                {
                    new HandlerConfiguration
                    {
                        Name = randomConfigName,
                        Value = randomConfigValue
                    }
                };

            EventCallV2 inputEventCallV2 = randomEventCallV2;

            EventHandlerResult randomEventHandlerResult =
                CreateRandomEventHandlerResult();

            EventHandlerResult returnedEventHandlerResult =
                randomEventHandlerResult;

            EventCallV2 expectedEventCallV2 =
                inputEventCallV2.DeepClone();

            expectedEventCallV2.IsSuccess =
                returnedEventHandlerResult.IsSuccess;

            expectedEventCallV2.Response =
                returnedEventHandlerResult.Response;

            expectedEventCallV2.ResponseCode =
                returnedEventHandlerResult.ResponseCode;

            expectedEventCallV2.ResponseMessage =
                returnedEventHandlerResult.ResponseMessage;

            this.eventHandlerBrokerMock.Setup(broker => broker.Name)
                .Returns(randomHandlerName);

            this.eventHandlerBrokerMock.Setup(broker => broker.RequiredParams)
                .Returns(new[] { randomConfigName });

            this.eventHandlerBrokerMock
                .Setup(broker =>
                    broker.HandleAsync(
                        inputEventCallV2.Content,
                        It.Is<IReadOnlyDictionary<string, string>>(d =>
                            d[randomConfigName] == randomConfigValue),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(returnedEventHandlerResult);

            // when
            EventCallV2 actualEventCallV2 =
                await this.eventCallV2Service
                    .RunEventCallV2Async(inputEventCallV2, TestContext.Current.CancellationToken);

            // then
            actualEventCallV2.Should().BeEquivalentTo(expectedEventCallV2);

            this.eventHandlerBrokerMock.VerifyGet(broker => broker.Name,
                Times.AtLeastOnce);

            this.eventHandlerBrokerMock.VerifyGet(broker => broker.RequiredParams,
                Times.AtLeastOnce);

            this.eventHandlerBrokerMock.Verify(broker =>
                broker.HandleAsync(
                    inputEventCallV2.Content,
                    It.Is<IReadOnlyDictionary<string, string>>(d =>
                        d[randomConfigName] == randomConfigValue),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
