// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Models.Views.EventAddresses;

namespace EventHighway.Portal.Web.Services.Views.EventAddresses
{
    public interface IEventAddressesViewService
    {
        ValueTask<List<EventAddressView>> RetrieveAllAddressesAsync(
            CancellationToken cancellationToken = default);

        ValueTask<EventAddressView> RegisterAddressAsync(
            EventAddressView address,
            CancellationToken cancellationToken = default);

        ValueTask<EventAddressView> RemoveAddressByIdAsync(
            Guid addressId,
            CancellationToken cancellationToken = default);
    }
}
