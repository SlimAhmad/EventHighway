// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private static void ConfigureEventParticipantV2s(EntityTypeBuilder<EventParticipantV2> model)
        {
            model.ToTable("EventParticipantV2s");
            model.HasKey(eventParticipantV2 => eventParticipantV2.Id);

            model.HasMany(eventParticipantV2 => eventParticipantV2.EventV2s)
                .WithOne(eventV2 => eventV2.EventParticipantV2)
                .HasForeignKey(eventV2 => eventV2.EventParticipantV2Id)
                .OnDelete(DeleteBehavior.Restrict);

            model.HasMany(eventParticipantV2 => eventParticipantV2.EventArchiveV2s)
                .WithOne(eventArchiveV2 => eventArchiveV2.EventParticipantV2)
                .HasForeignKey(eventArchiveV2 => eventArchiveV2.EventParticipantV2Id)
                .OnDelete(DeleteBehavior.Restrict);

            model.HasMany(eventParticipantV2 => eventParticipantV2.EventListenerV2s)
                .WithOne(eventListenerV2 => eventListenerV2.EventParticipantV2)
                .HasForeignKey(eventListenerV2 => eventListenerV2.EventParticipantV2Id)
                .OnDelete(DeleteBehavior.Restrict);

            model.HasMany(eventParticipantV2 => eventParticipantV2.ListenerEventV2s)
                .WithOne(listenerEventV2 => listenerEventV2.EventParticipantV2)
                .HasForeignKey(listenerEventV2 => listenerEventV2.EventParticipantV2Id)
                .OnDelete(DeleteBehavior.Restrict);

            model.HasMany(eventParticipantV2 => eventParticipantV2.ListenerEventArchiveV2s)
                .WithOne(listenerEventArchiveV2 => listenerEventArchiveV2.EventParticipantV2)
                .HasForeignKey(listenerEventArchiveV2 => listenerEventArchiveV2.EventParticipantV2Id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
