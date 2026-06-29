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
            new Guid("a1000000-0000-0000-0000-000000000001");

        // Event listeners
        public static readonly Guid BingeBoxNewReleasesListener =
            new Guid("a2000000-0000-0000-0000-000000000001");

        public static readonly Guid JoeGoodMoviesListener =
            new Guid("a2000000-0000-0000-0000-000000000002");

        public static readonly Guid AnnNewReleasesListener =
            new Guid("a2000000-0000-0000-0000-000000000003");

        public static readonly Guid MediaItemServiceInternalListener =
            new Guid("a2000000-0000-0000-0000-000000000004");
    }
}
