// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Security.Cryptography;
using System.Text;

namespace EventHighway.ClientV2.SubstrateApp
{
    // Deterministic identifiers for the seed/sample data. The same natural key always maps to the
    // same Guid, so re-running the substrate app (or the external contribution flow) reuses the
    // existing participant/address/listener/secret rows instead of inserting duplicates.
    public static class SeedIdentifiers
    {
        public static Guid StableId(string key)
        {
            byte[] hash = MD5.HashData(Encoding.UTF8.GetBytes(key));

            return new Guid(hash);
        }
    }
}
