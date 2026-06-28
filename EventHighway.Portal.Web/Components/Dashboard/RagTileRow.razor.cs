// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Models.Views.HealthDashboards;
using EventHighway.Portal.Web.Services.Views.HealthDashboards;
using Microsoft.AspNetCore.Components;

namespace EventHighway.Portal.Web.Components.Dashboard
{
    public partial class RagTileRow
    {
        [Inject]
        public IHealthViewService HealthViewService { get; set; } = default!;

        public RagTileRowState State { get; private set; } = RagTileRowState.Loading;

        public List<HealthRagTile> Tiles { get; private set; } = new();

        public string? ErrorMessage { get; private set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Tiles = await this.HealthViewService.RetrieveHealthRagTilesAsync();
                State = RagTileRowState.Content;
            }
            catch (Exception)
            {
                ErrorMessage = "Unable to load health status. Please try again.";
                State = RagTileRowState.Error;
            }
        }
    }
}
