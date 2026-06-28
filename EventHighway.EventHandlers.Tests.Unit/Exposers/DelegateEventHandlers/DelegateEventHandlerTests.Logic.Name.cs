// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using FluentAssertions;

namespace EventHighway.EventHandlers.Tests.Unit.Exposers.DelegateEventHandlers
{
    public partial class DelegateEventHandlerTests
    {
        [Fact]
        public void ShouldDefaultNameToTypeNameWhenNoNameIsSupplied()
        {
            // given
            Guid identifier = Guid.NewGuid();
            string expectedName = nameof(DelegateEventHandler);

            // when
            var delegateEventHandler = new DelegateEventHandler(
                Id: identifier,
                delegateService: this.delegateServiceMock.Object);

            // then
            delegateEventHandler.Name.Should().Be(expectedName);
        }

        [Fact]
        public void ShouldUseSuppliedNameWhenNameIsProvided()
        {
            // given
            Guid identifier = Guid.NewGuid();
            string inputName = GetRandomString();

            // when
            var delegateEventHandler = new DelegateEventHandler(
                Id: identifier,
                delegateService: this.delegateServiceMock.Object,
                name: inputName);

            // then
            delegateEventHandler.Name.Should().Be(inputName);
        }
    }
}
