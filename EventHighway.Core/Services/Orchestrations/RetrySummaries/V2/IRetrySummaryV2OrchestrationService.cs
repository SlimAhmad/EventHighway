// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;

namespace EventHighway.Core.Services.Orchestrations.RetrySummaries.V2
{
    internal interface IRetrySummaryV2OrchestrationService
    {
        ValueTask<RetryHealthSummaryV2> RetrieveRetryHealthV2Async(
            CancellationToken cancellationToken = default);
    }
}
