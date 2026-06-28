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
                { StatTileVariant.Green, "text-bg-success" },
                { StatTileVariant.Amber, "text-bg-warning" },
                { StatTileVariant.Red, "text-bg-danger" },
                { StatTileVariant.Na, "text-bg-secondary" },
            };
    }
}
