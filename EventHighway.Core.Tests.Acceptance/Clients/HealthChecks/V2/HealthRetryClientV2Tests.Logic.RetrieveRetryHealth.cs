// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using FluentAssertions;

namespace EventHighway.Core.Tests.Acceptance.Clients.HealthChecks.V2
{
    public partial class HealthRetryClientV2Tests
    {
        [Fact]
        public async Task ShouldRetrieveRetryHealthV2Async()
        {
            // given . when
            Func<Task> retrieveRetryHealthV2Task = async () =>
            {
                RetryHealthSummaryV2 actualSummary =
                    await this.clientBroker.RetrieveRetryHealthV2Async();

                actualSummary.Should().NotBeNull();
            };

            // then
            await retrieveRetryHealthV2Task.Should()
                .NotThrowAsync();
        }
    }
}
