// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

// Sample-data seeder for the EventHighway portal. Tops up the NFlix-NewReleases address with
// backdated EventV2 + ListenerEventV2 history so the portal lists/filters have several pages of
// data. Run the ClientV2.SubstrateApp sample first to create the base participant/address/listeners,
// then run this seeder. The hydration logic lives in DatabaseHydrator so any console app can reuse it.

using EventHighway.Portal.Seed;

const string connectionString =
    "Server=(localdb)\\MSSQLLocalDB;Database=EventHighwayDB;" +
    "Trusted_Connection=True;MultipleActiveResultSets=true";

await DatabaseHydrator.HydrateNewReleasesAsync(connectionString);
