// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.HealthChecks.V2.Exceptions;
using EventHighway.Portal.Web.Models.Views.HealthDashboards;
using EventHighway.Portal.Web.Models.Views.HealthDashboards.Exceptions;
using Xeptions;

namespace EventHighway.Portal.Web.Services.Views.HealthDashboards
{
    public partial class HealthViewService
    {
        private delegate ValueTask<List<HealthRagTile>> ReturningHealthRagTilesFunction();

        private async ValueTask<List<HealthRagTile>> TryCatch(
            ReturningHealthRagTilesFunction returningHealthRagTilesFunction)
        {
            try
            {
                return await returningHealthRagTilesFunction();
            }
            catch (HealthStatusClientV2ValidationException healthStatusClientValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    healthStatusClientValidationException);
            }
        }

        private async ValueTask<HealthViewDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var healthViewDependencyValidationException =
                new HealthViewDependencyValidationException(innerException: exception);

            await this.loggingBroker.LogErrorAsync(healthViewDependencyValidationException);

            return healthViewDependencyValidationException;
        }
    }
}
