// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using FluentAssertions;
using Force.DeepCloner;

namespace EventHighway.EventHandlers.Tests.Acceptance.Exposers.DelegateEventHandlers
{
    public partial class DelegateEventHandlerTests
    {
        [Fact]
        public async Task ShouldHandleWithDelegateAsync()
        {
            // given
            string randomContent = GetRandomString();
            IReadOnlyDictionary<string, string> randomHandlerParams = CreateRandomHandlerParams();
            EventHandlerResult randomEventHandlerResult = CreateRandomEventHandlerResult();
            EventHandlerResult expectedEventHandlerResult = randomEventHandlerResult.DeepClone();

            string actualInvokedContent = null;
            IReadOnlyDictionary<string, string> actualInvokedHandlerParams = null;

            var delegateEventHandler = new DelegateEventHandler(
                handler: (content, handlerParams, cancellationToken) =>
                {
                    actualInvokedContent = content;
                    actualInvokedHandlerParams = handlerParams;

                    return new ValueTask<EventHandlerResult>(randomEventHandlerResult);
                });

            // when
            EventHandlerResult actualEventHandlerResult =
                await delegateEventHandler.HandleAsync(
                    content: randomContent,
                    handlerParams: randomHandlerParams,
                    cancellationToken: TestContext.Current.CancellationToken);

            // then
            actualEventHandlerResult.Should().BeEquivalentTo(expectedEventHandlerResult);
            actualInvokedContent.Should().Be(randomContent);
            actualInvokedHandlerParams.Should().BeEquivalentTo(randomHandlerParams);
        }
    }
}
