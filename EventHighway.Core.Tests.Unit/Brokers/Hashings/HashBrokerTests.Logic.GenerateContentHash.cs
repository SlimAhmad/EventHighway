// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using FluentAssertions;

namespace EventHighway.Core.Tests.Unit.Brokers.Hashings
{
    public partial class HashBrokerTests
    {
        [Fact]
        public void ShouldGenerateSameHashForIdenticalContent()
        {
            // given
            string randomContent = GetRandomString();
            string inputContent = randomContent;

            // when
            string firstHash = this.hashBroker.GenerateSha256Hash(inputContent);
            string secondHash = this.hashBroker.GenerateSha256Hash(inputContent);

            // then
            firstHash.Should().Be(secondHash);
        }
    }
}
