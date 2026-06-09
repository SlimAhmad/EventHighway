// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        ValueTask<HandlerConfiguration> InsertHandlerConfigurationAsync(HandlerConfiguration handlerConfiguration, CancellationToken cancellationToken = default);
        ValueTask<IQueryable<HandlerConfiguration>> SelectAllHandlerConfigurationsAsync();
        ValueTask<HandlerConfiguration> SelectHandlerConfigurationByIdAsync(Guid handlerConfigurationId, CancellationToken cancellationToken = default);
        ValueTask<HandlerConfiguration> UpdateHandlerConfigurationAsync(HandlerConfiguration handlerConfiguration, CancellationToken cancellationToken = default);
        ValueTask<HandlerConfiguration> DeleteHandlerConfigurationAsync(HandlerConfiguration handlerConfiguration, CancellationToken cancellationToken = default);
    }
}
