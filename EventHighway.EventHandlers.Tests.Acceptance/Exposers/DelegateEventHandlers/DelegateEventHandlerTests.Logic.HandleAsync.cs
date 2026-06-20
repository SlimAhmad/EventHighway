// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
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
            EventHandlerResult randomEventHandlerResult = CreateRandomEventHandlerResult();
            EventHandlerResult expectedEventHandlerResult = randomEventHandlerResult.DeepClone();

            string actualInvokedContent = null;
            Guid identifier = Guid.NewGuid();

            var delegateEventHandler = new DelegateEventHandler(
                Id: identifier,
                handler: (content, cancellationToken) =>
                {
                    actualInvokedContent = content;

                    return new ValueTask<EventHandlerResult>(randomEventHandlerResult);
                });

            // when
            EventHandlerResult actualEventHandlerResult =
                await delegateEventHandler.HandleAsync(
                    content: randomContent,
                    cancellationToken: TestContext.Current.CancellationToken);

            // then
            actualEventHandlerResult.Should().BeEquivalentTo(expectedEventHandlerResult);
            actualInvokedContent.Should().Be(randomContent);
        }
    }
}
