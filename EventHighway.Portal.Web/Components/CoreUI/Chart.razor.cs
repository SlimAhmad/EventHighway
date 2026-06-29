// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace EventHighway.Portal.Web.Components.CoreUI
{
    public partial class Chart : IAsyncDisposable
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; } = default!;

        [Parameter]
        public string ChartType { get; set; } = "line";

        [Parameter]
        public IReadOnlyList<string> Labels { get; set; } = new List<string>();

        [Parameter]
        public IReadOnlyList<ChartDataset> Datasets { get; set; } = new List<ChartDataset>();

        [Parameter]
        public int Height { get; set; } = 300;

        public string ElementId { get; } = "chart-" + Guid.NewGuid().ToString("N");

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await this.JSRuntime.InvokeVoidAsync(
                "eventHighwayCharts.render", ElementId, BuildConfig());
        }

        private object BuildConfig() =>
            new
            {
                type = ChartType,
                data = new
                {
                    labels = Labels,
                    datasets = Datasets.Select(dataset => new
                    {
                        label = dataset.Label,
                        data = dataset.Data,
                        backgroundColor = dataset.Colors,
                        borderColor = dataset.Colors.Count > 0
                            ? dataset.Colors[0]
                            : "#321fdb",
                        fill = ChartType == "line",
                        tension = 0.4
                    })
                },
                options = new
                {
                    responsive = true,
                    maintainAspectRatio = false
                }
            };

        public async ValueTask DisposeAsync()
        {
            try
            {
                await this.JSRuntime.InvokeVoidAsync("eventHighwayCharts.destroy", ElementId);
            }
            catch
            {
                // ignored — circuit may already be gone during disposal
            }
        }
    }
}
