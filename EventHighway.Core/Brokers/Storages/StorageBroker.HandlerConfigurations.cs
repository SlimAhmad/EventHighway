// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<HandlerConfiguration> HandlerConfigurations { get; set; }

        public async ValueTask<HandlerConfiguration> InsertHandlerConfigurationAsync(
            HandlerConfiguration handlerConfiguration,
            CancellationToken cancellationToken = default) =>
            await InsertAsync(handlerConfiguration, cancellationToken);

        public async ValueTask<IQueryable<HandlerConfiguration>> SelectAllHandlerConfigurationsAsync() =>
            SelectAll<HandlerConfiguration>();

        public async ValueTask<HandlerConfiguration> SelectHandlerConfigurationByIdAsync(
            Guid handlerConfigurationId,
            CancellationToken cancellationToken = default) =>
            await SelectAsync<HandlerConfiguration>(new object[] { handlerConfigurationId }, cancellationToken);

        public async ValueTask<HandlerConfiguration> UpdateHandlerConfigurationAsync(
            HandlerConfiguration handlerConfiguration,
            CancellationToken cancellationToken = default) =>
            await UpdateAsync(handlerConfiguration, cancellationToken);

        public async ValueTask<HandlerConfiguration> DeleteHandlerConfigurationAsync(
            HandlerConfiguration handlerConfiguration,
            CancellationToken cancellationToken = default) =>
            await DeleteAsync(handlerConfiguration, cancellationToken);
    }
}
