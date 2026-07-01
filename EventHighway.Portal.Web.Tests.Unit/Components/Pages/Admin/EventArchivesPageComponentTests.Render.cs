// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using Bunit;
using EventHighway.Portal.Web.Components.Pages.Admin;
using EventHighway.Portal.Web.Models.Views.EventArchives;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Components.Pages.Admin
{
    public partial class EventArchivesPageComponentTests
    {
        [Fact]
        public void ShouldRenderStatusBadgesAndRowColors()
        {
            // given
            EventArchiveView quarantined = CreateArchive("Quarantined", remainingRetryAttempts: 3);
            EventArchiveView dead = CreateArchive("Active", remainingRetryAttempts: 0);

            this.eventArchivesViewServiceMock.Setup(service =>
                service.RetrieveAllEventArchivesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new List<EventArchiveView> { quarantined, dead });

            // when
            IRenderedComponent<EventArchivesPage> renderedPage = Render<EventArchivesPage>();

            // then
            renderedPage.Markup.Should().Contain(quarantined.EventAddressName);
            renderedPage.Markup.Should().Contain("Dead");
            renderedPage.FindAll("tr.table-danger").Should().NotBeEmpty();
            renderedPage.FindAll("tr.table-warning").Should().NotBeEmpty();
            renderedPage.FindAll("span.badge.text-bg-danger").Should().NotBeEmpty();
            renderedPage.FindAll("span.badge.text-bg-warning").Should().NotBeEmpty();
        }

        [Fact]
        public void ShouldFilterArchivesByStatus()
        {
            // given
            EventArchiveView quarantined = CreateArchive("Quarantined", remainingRetryAttempts: 3);
            EventArchiveView active = CreateArchive("Active", remainingRetryAttempts: 3);

            this.eventArchivesViewServiceMock.Setup(service =>
                service.RetrieveAllEventArchivesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new List<EventArchiveView> { quarantined, active });

            IRenderedComponent<EventArchivesPage> renderedPage = Render<EventArchivesPage>();

            // when — Type select is [0], Status select is [1]
            renderedPage.FindAll("select")[1].Change("Quarantined");

            // then
            renderedPage.Markup.Should().Contain(quarantined.EventAddressName);
            renderedPage.Markup.Should().NotContain(active.EventAddressName);
        }
    }
}
