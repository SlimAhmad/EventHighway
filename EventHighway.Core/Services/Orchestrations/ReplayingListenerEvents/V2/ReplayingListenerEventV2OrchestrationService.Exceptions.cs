// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;

namespace EventHighway.Core.Services.Orchestrations.ReplayingListenerEvents.V2
{
    internal partial class ReplayingListenerEventV2OrchestrationService
    {
        private delegate ValueTask<Models.Services.Foundations.ListenerEvents.V2.ListenerEventV2>
            ReturningListenerEventV2Function();

        private async ValueTask<Models.Services.Foundations.ListenerEvents.V2.ListenerEventV2>
            TryCatch(ReturningListenerEventV2Function returningListenerEventV2Function)
        {
            return await returningListenerEventV2Function();
        }
    }
}
