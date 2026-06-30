// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.ClientV2.Seed
{
    // Fixed identifiers for the shared sample data. Both console apps (BasicApp and SubstrateApp)
    // use these SAME Guids, so re-running either app (or both) reuses the existing rows instead of
    // inserting duplicates.
    public static class SeedIdentifiers
    {
        // Participants
        public static readonly Guid NFlixParticipant =
            new Guid("b0000000-0000-0000-0000-000000000001");

        public static readonly Guid MediaItemServiceParticipant =
            new Guid("b0000000-0000-0000-0000-000000000002");

        public static readonly Guid BingeBoxParticipant =
            new Guid("b0000000-0000-0000-0000-000000000003");

        public static readonly Guid JoeParticipant =
            new Guid("b0000000-0000-0000-0000-000000000004");

        public static readonly Guid AnnParticipant =
            new Guid("b0000000-0000-0000-0000-000000000005");

        // Participant secrets
        public static readonly Guid NFlixSecret =
            new Guid("c0000000-0000-0000-0000-000000000001");

        // Event addresses
        public static readonly Guid NFlixNewReleasesAddress =
            new Guid("be0dd6e0-b545-435d-9541-d1ac386469ce");

        // A second address whose downstream is partially unreliable, used to seed
        // partial-success events (one reliable listener + one always-failing listener).
        public static readonly Guid NFlixPartnerFeedAddress =
            new Guid("7abdf5a3-d099-41d8-9ec1-67f6d1580901");

        // Event listeners
        public static readonly Guid BingeBoxNewReleasesListener =
            new Guid("a2000000-0000-0000-0000-000000000001");

        public static readonly Guid JoeGoodMoviesListener =
            new Guid("a2000000-0000-0000-0000-000000000002");

        public static readonly Guid AnnNewReleasesListener =
            new Guid("a2000000-0000-0000-0000-000000000003");

        public static readonly Guid MediaItemServiceInternalListener =
            new Guid("a2000000-0000-0000-0000-000000000004");

        // Listeners on the partner-feed address: one reliable, one always failing.
        public static readonly Guid PartnerFeedReliableListener =
            new Guid("b9171e0f-5cd0-489b-83e2-7dbd6626de28");

        public static readonly Guid PartnerFeedFailingListener =
            new Guid("64f7e061-34dc-4b30-b5c8-558bd3b8228e");
    }
}
