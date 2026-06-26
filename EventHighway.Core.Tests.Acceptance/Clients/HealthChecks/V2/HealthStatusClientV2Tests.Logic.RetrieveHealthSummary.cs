// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using FluentAssertions;

namespace EventHighway.Core.Tests.Acceptance.Clients.HealthChecks.V2
{
    public partial class HealthStatusClientV2Tests
    {
        [Fact]
        public async Task ShouldRetrieveHealthRagStatusV2Async()
        {
            // given . when
            Func<Task> retrieveHealthSummaryV2Task = async () =>
            {
                IEnumerable<HealthCheckItemV2> actualHealthCheckItemV2s =
                    await this.clientBroker.RetrieveHealthRagStatusV2Async();

                actualHealthCheckItemV2s.Should().NotBeNull();
            };

            // then
            await retrieveHealthSummaryV2Task.Should()
                .NotThrowAsync();
        }
    }
}
