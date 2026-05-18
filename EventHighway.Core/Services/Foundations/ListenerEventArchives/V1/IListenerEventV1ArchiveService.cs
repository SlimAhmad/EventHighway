// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;

namespace EventHighway.Core.Services.Foundations.ListenerEventArchives.V1
{
    internal interface IListenerEventV1ArchiveService
    {
        ValueTask<ListenerEventArchiveV1> AddListenerEventV1ArchiveAsync(
            ListenerEventArchiveV1 listenerEventV1Archive);
    }
}
