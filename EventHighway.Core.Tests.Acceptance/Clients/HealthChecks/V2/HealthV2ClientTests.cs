// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Tests.Acceptance.Brokers;

namespace EventHighway.Core.Tests.Acceptance.Clients.HealthChecks.V2
{
    [Collection(nameof(ClientTestCollection))]
    public partial class HealthV2ClientTests
    {
        private readonly ClientBroker clientBroker;

        public HealthV2ClientTests() =>
            this.clientBroker = new ClientBroker();
    }
}
