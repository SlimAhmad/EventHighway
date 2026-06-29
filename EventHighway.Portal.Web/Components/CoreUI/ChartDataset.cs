// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;

namespace EventHighway.Portal.Web.Components.CoreUI
{
    public sealed class ChartDataset
    {
        public string Label { get; set; } = string.Empty;

        public IReadOnlyList<double> Data { get; set; } = new List<double>();

        public IReadOnlyList<string> Colors { get; set; } = new List<string>();
    }
}
