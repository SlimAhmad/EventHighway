// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using Bunit;
using EventHighway.Portal.Web.Components.CoreUI;

namespace EventHighway.Portal.Web.Tests.Unit.Components.CoreUI
{
    public partial class FormSelectComponentTests : BunitContext
    {
        private static List<SelectOption> CreateOptions() =>
            new List<SelectOption>
            {
                new SelectOption { Value = "off", Text = "Off" },
                new SelectOption { Value = "5", Text = "5 min" },
            };
    }
}
