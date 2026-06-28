// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Bunit;
using EventHighway.Portal.Web.Components.CoreUI;
using EventHighway.Portal.Web.Models.Views.HealthDashboards;
using EventHighway.Portal.Web.Services.Views.HealthDashboards;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Portal.Web.Tests.Unit.Components.Dashboard
{
    public partial class RagTileRowComponentTests : BunitContext
    {
        private readonly Mock<IHealthViewService> healthViewServiceMock;

        public RagTileRowComponentTests()
        {
            this.healthViewServiceMock = new Mock<IHealthViewService>();
            Services.AddSingleton(this.healthViewServiceMock.Object);
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static List<HealthRagTile> CreateRandomRagTiles() =>
            Enumerable.Range(0, 4).Select(_ => new HealthRagTile
            {
                Grouping = GetRandomString(),
                Label = GetRandomString(),
                Value = GetRandomString(),
                Description = GetRandomString(),
                Variant = StatTileVariant.Green
            }).ToList();
    }
}
