// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Models.Views.HealthDashboards;

namespace EventHighway.Portal.Web.Services.Views.HealthDashboards
{
    public interface IHealthViewService
    {
        ValueTask<List<HealthRagTile>> RetrieveHealthRagTilesAsync(
            CancellationToken cancellationToken = default);
    }
}
