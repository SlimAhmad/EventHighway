// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Bunit;
using EventHighway.Portal.Web.Components.CoreUI;
using Tynamix.ObjectFiller;

namespace EventHighway.Portal.Web.Tests.Unit.Components.CoreUI
{
    public partial class StatTileComponentTests : BunitContext
    {
        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        public static TheoryData<StatTileVariant, string> VariantCssClasses() =>
            new TheoryData<StatTileVariant, string>
            {
                { StatTileVariant.Green, "bg-success" },
                { StatTileVariant.Amber, "bg-warning" },
                { StatTileVariant.Red, "bg-danger" },
                { StatTileVariant.Na, "bg-secondary" },
            };
    }
}
