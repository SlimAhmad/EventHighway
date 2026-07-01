// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace EventHighway.ClientV2.Seed
{
    // Fixed identifiers for the shared sample data. Both console apps (BasicApp and SubstrateApp)
    // use these SAME Guids, so re-running either app (or both) reuses the existing rows instead of
    // inserting duplicates.
    public static class SeedIdentifiers
    {
        // Participants
        public static readonly Guid NFlixParticipant =
            new Guid("a817f520-c7e5-4831-a67b-171902bf28ba");

        public static readonly Guid MediaItemServiceParticipant =
            new Guid("7a7513d8-bc8f-4a7f-b740-b00bbca519c6");

        public static readonly Guid BingeBoxParticipant =
            new Guid("72edb46a-4e55-49dc-8b92-16baf040c6fd");

        public static readonly Guid JoeParticipant =
            new Guid("523a9adc-a582-42da-ab0d-762eb8782962");

        public static readonly Guid AnnParticipant =
            new Guid("ab496d88-7cf5-4e8f-af45-5e75583fb5d0");

        // Participant secrets
        public static readonly Guid NFlixSecret =
            new Guid("5b1f7ee4-d421-4e2a-a534-c41cb1627bd1");

        // Event addresses
        public static readonly Guid NFlixNewReleasesAddress =
            new Guid("be0dd6e0-b545-435d-9541-d1ac386469ce");

        // Event listeners
        public static readonly Guid BingeBoxNewReleasesListener =
            new Guid("07864612-508c-4177-a0b6-061f9efa48d8");

        public static readonly Guid JoeGoodMoviesListener =
            new Guid("523a9adc-a582-42da-ab0d-762eb8782962");

        public static readonly Guid AnnNewReleasesListener =
            new Guid("ab496d88-7cf5-4e8f-af45-5e75583fb5d0");

        public static readonly Guid MediaItemServiceInternalListener =
            new Guid("5b1f7ee4-d421-4e2a-a534-c41cb1627bd1");

        // Event handlers. Stable Ids (shared by both console apps and the Portal.Seed hydrator) so a
        // listener registered by one app references a handler the other app also registers under the
        // same Id — dispatch then works regardless of which app created the listener or the run order.
        public static readonly Guid BingeBoxHandler =
            new Guid("6326cae3-04ff-411f-93fb-e606859390f6");

        public static readonly Guid JoeHandler =
            new Guid("9846c9e3-2843-4a2e-a586-4321c3a5f1a9");

        public static readonly Guid AnnHandler =
            new Guid("a9079276-fbbe-4176-9744-9fee3354f3e7");

        public static readonly Guid FlakyBoxHandler =
            new Guid("9cc4ac0e-6ef2-4c99-a0cd-831867fff9df");

        public static readonly Guid MediaItemServiceHandler =
            new Guid("6743a4f1-07c4-4def-9d1f-e0ca926c6b90");
    }
}
