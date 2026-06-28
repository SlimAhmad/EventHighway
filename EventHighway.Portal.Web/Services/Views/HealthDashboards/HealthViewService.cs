// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Components.CoreUI;
using EventHighway.Portal.Web.Models.Views.HealthDashboards;

namespace EventHighway.Portal.Web.Services.Views.HealthDashboards
{
    public partial class HealthViewService : IHealthViewService
    {
        private readonly IEventHighwayBroker eventHighwayBroker;
        private readonly ILoggingBroker loggingBroker;

        public HealthViewService(
            IEventHighwayBroker eventHighwayBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventHighwayBroker = eventHighwayBroker;
            this.loggingBroker = loggingBroker;
        }

        public async ValueTask<List<HealthRagTile>> RetrieveHealthRagTilesAsync(
            CancellationToken cancellationToken = default)
        {
            IEnumerable<HealthCheckItemV2> healthCheckItems =
                await this.eventHighwayBroker.RetrieveHealthRagStatusV2Async(
                    cancellationToken);

            return healthCheckItems.Select(AsRagTile).ToList();
        }

        private static HealthRagTile AsRagTile(HealthCheckItemV2 healthCheckItem) =>
            new HealthRagTile
            {
                Grouping = healthCheckItem.Grouping,
                Label = healthCheckItem.Item,
                Value = healthCheckItem.Value,
                Description = healthCheckItem.Description,
                StatusCode = healthCheckItem.StatusCode,
                Variant = AsVariant(healthCheckItem.Status)
            };

        private static StatTileVariant AsVariant(string status) =>
            status switch
            {
                "Green" => StatTileVariant.Green,
                "Amber" => StatTileVariant.Amber,
                "Red" => StatTileVariant.Red,
                _ => StatTileVariant.Na
            };
    }
}
