// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations;
using EventHighway.Core.Models.Services.Foundations.PromotedProperties;
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

            this.eventHandlerBrokerMock.Setup(broker => broker.GetAll())
                .Returns(new[] { this.eventHandlerMock.Object });

            this.eventHandlerMock.SetupGet(handler => handler.Id)
                .Returns(inputEventCallV2.HandlerId);

            this.eventHandlerMock.SetupGet(handler => handler.Name)
                .Returns(randomHandlerName);

            this.eventHandlerMock.SetupGet(handler => handler.RequiredParams)
                .Returns(new[] { randomConfigName });

            this.eventHandlerMock
                .Setup(handler =>
                    handler.HandleAsync(
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

            this.eventHandlerBrokerMock.Verify(broker => broker.GetAll(),
                Times.AtLeastOnce);

            this.eventHandlerBrokerMock.VerifyNoOtherCalls();

            this.eventHandlerMock.VerifyGet(handler => handler.Id,
                Times.AtLeastOnce);

            this.eventHandlerMock.VerifyGet(handler => handler.Name,
                Times.AtLeastOnce);

            this.eventHandlerMock.VerifyGet(handler => handler.RequiredParams,
                Times.AtLeastOnce);

            this.eventHandlerMock.Verify(handler =>
                handler.HandleAsync(
                    inputEventCallV2.Content,
                    It.Is<IReadOnlyDictionary<string, string>>(d =>
                        d[randomConfigName] == randomConfigValue),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            this.eventHandlerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldSkipEventCallV2WhenFilterCriteriaDoesNotMatchAsync()
        {
            // given
            string randomHandlerName = GetRandomString();
            string randomConfigName = GetRandomString();
            string randomConfigValue = GetRandomString();
            string promotedPropertyName = "OrderType";
            string promotedPropertyValue = "Standard";

            EventCallV2 randomEventCallV2 = CreateRandomEventCallV2();
            randomEventCallV2.HandlerName = randomHandlerName;
            randomEventCallV2.FilterCriteria = $"meta(\"{promotedPropertyName}\") == \"Express\"";
            randomEventCallV2.RequiredPromotedProperties = new[] { promotedPropertyName };

            randomEventCallV2.PromotedProperties =
                new List<PromotedProperty>
                {
                    new PromotedProperty
                    {
                        Name = promotedPropertyName,
                        Value = promotedPropertyValue
                    }
                };

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

            EventCallV2 expectedEventCallV2 = inputEventCallV2.DeepClone();
            expectedEventCallV2.IsSuccess = true;
            expectedEventCallV2.ResponseCode = "SkippedNotMatchingFilter";
            expectedEventCallV2.ResponseMessage =
                "Event was not handled because it did not match the listener's filter criteria.";

            this.eventHandlerBrokerMock.Setup(broker => broker.GetAll())
                .Returns(new[] { this.eventHandlerMock.Object });

            this.eventHandlerMock.SetupGet(handler => handler.Id)
                .Returns(inputEventCallV2.HandlerId);

            this.eventHandlerMock.SetupGet(handler => handler.Name)
                .Returns(randomHandlerName);

            this.eventHandlerMock.SetupGet(handler => handler.RequiredParams)
                .Returns(new[] { randomConfigName });

            this.eventHandlerMock
                .Setup(handler =>
                    handler.HandleAsync(
                        It.IsAny<string>(),
                        It.IsAny<IReadOnlyDictionary<string, string>>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateRandomEventHandlerResult());

            // when
            EventCallV2 actualEventCallV2 =
                await this.eventCallV2Service
                    .RunEventCallV2Async(inputEventCallV2, TestContext.Current.CancellationToken);

            // then
            actualEventCallV2.Should().BeEquivalentTo(expectedEventCallV2);

            this.eventHandlerBrokerMock.Verify(broker => broker.GetAll(),
                Times.AtLeastOnce);

            this.eventHandlerBrokerMock.VerifyNoOtherCalls();

            this.eventHandlerMock.VerifyGet(handler => handler.Id,
                Times.AtLeastOnce);

            this.eventHandlerMock.VerifyGet(handler => handler.Name,
                Times.AtLeastOnce);

            this.eventHandlerMock.VerifyGet(handler => handler.RequiredParams,
                Times.AtLeastOnce);

            this.eventHandlerMock.Verify(handler =>
                handler.HandleAsync(
                    It.IsAny<string>(),
                    It.IsAny<IReadOnlyDictionary<string, string>>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            this.eventHandlerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
