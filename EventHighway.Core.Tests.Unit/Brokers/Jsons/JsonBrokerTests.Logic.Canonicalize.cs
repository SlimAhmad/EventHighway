// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using FluentAssertions;

namespace EventHighway.Core.Tests.Unit.Brokers.Jsons
{
    public partial class JsonBrokerTests
    {
        [Fact]
        public void ShouldProduceSameOutputForDifferentKeyOrderInputs()
        {
            // given
            string firstJson = "{\"b\":2,\"a\":1}";
            string secondJson = "{\"a\":1,\"b\":2}";

            // when
            string firstCanonical = this.jsonBroker.Canonicalize(firstJson);
            string secondCanonical = this.jsonBroker.Canonicalize(secondJson);

            // then
            firstCanonical.Should().Be(secondCanonical);
        }

        [Fact]
        public void ShouldProduceSameOutputForDifferentWhitespaceInputs()
        {
            // given
            string compactJson = "{\"a\":1}";
            string prettyJson = "{ \"a\" : 1 }";

            // when
            string compactCanonical = this.jsonBroker.Canonicalize(compactJson);
            string prettyCanonical = this.jsonBroker.Canonicalize(prettyJson);

            // then
            compactCanonical.Should().Be(prettyCanonical);
        }
    }
}
