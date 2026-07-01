// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Bunit;
using EventHighway.Portal.Web.Models.Views.EventArchives;
using EventHighway.Portal.Web.Models.Views.ListenerEventArchives;
using EventHighway.Portal.Web.Services.Views.EventArchives;
using EventHighway.Portal.Web.Services.Views.ListenerEventArchives;
using EventHighway.Portal.Web.Services.Views.Replays;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Portal.Web.Tests.Unit.Components.Pages.Admin
{
    public partial class EventArchiveDetailPageComponentTests : BunitContext
    {
        private readonly Mock<IEventArchivesViewService> eventArchivesViewServiceMock;
        private readonly Mock<IListenerEventArchivesViewService> listenerEventArchivesViewServiceMock;
        private readonly Mock<IReplayViewService> replayViewServiceMock;

        public EventArchiveDetailPageComponentTests()
        {
            this.eventArchivesViewServiceMock = new Mock<IEventArchivesViewService>();
            this.listenerEventArchivesViewServiceMock =
                new Mock<IListenerEventArchivesViewService>();
            this.replayViewServiceMock = new Mock<IReplayViewService>();

            Services.AddSingleton(this.eventArchivesViewServiceMock.Object);
            Services.AddSingleton(this.listenerEventArchivesViewServiceMock.Object);
            Services.AddSingleton(this.replayViewServiceMock.Object);

            JSInterop.Mode = JSRuntimeMode.Loose;
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static EventArchiveView CreateRandomArchive() =>
            new EventArchiveView
            {
                Id = Guid.NewGuid(),
                EventName = GetRandomString(),
                Content = GetRandomString(),
                Type = "Immediate",
                Status = "Active",
                EventAddressId = Guid.NewGuid(),
                EventAddressName = GetRandomString()
            };

        private static ListenerEventArchiveView CreateListenerArchive(
            Guid eventArchiveId, string status) =>
            new ListenerEventArchiveView
            {
                Id = Guid.NewGuid(),
                Status = status,
                ResponseCode = status == "Success" ? "200" : "503",
                ResponseMessage = status,
                EventId = Guid.NewGuid(),
                EventAddressId = Guid.NewGuid(),
                EventListenerId = Guid.NewGuid(),
                EventArchiveV2Id = eventArchiveId,
                ListenerName = GetRandomString()
            };
    }
}
