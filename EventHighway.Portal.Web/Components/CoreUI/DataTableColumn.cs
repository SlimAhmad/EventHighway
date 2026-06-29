// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.Portal.Web.Components.CoreUI
{
    public sealed class DataTableColumn<TItem>
    {
        public string Title { get; set; } = string.Empty;

        public Func<TItem, object?> Value { get; set; } = _ => null;

        public bool Sortable { get; set; } = true;
    }
}
