// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Brokers.EventHandlers;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Clients.ArchivingEvents.V2;
using EventHighway.Core.Clients.EventAddresses.V2;
using EventHighway.Core.Clients.EventListeners.V2;
using EventHighway.Core.Clients.Events.V2;
using EventHighway.Core.Clients.ListenerEvents.V2;
using EventHighway.Core.Services.Coordinations.ArchivingEvents.V2;
using EventHighway.Core.Services.Coordinations.Events.V2;
using EventHighway.Core.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Services.Foundations.EventArchives.V2;
using EventHighway.Core.Services.Foundations.EventCalls.V2;
using EventHighway.Core.Services.Foundations.EventHandlers.V2;
using EventHighway.Core.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Services.Foundations.Events.V2;
using EventHighway.Core.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Services.Orchestrations.ArchivingEvents.V2;
using EventHighway.Core.Services.Orchestrations.EventArchives.V2;
using EventHighway.Core.Services.Orchestrations.EventListeners.V2;
using EventHighway.Core.Services.Orchestrations.Events.V2;
using EventHighway.Core.Services.Processings.EventAddresses.V2;
using EventHighway.Core.Services.Processings.EventCalls.V2;
using EventHighway.Core.Services.Processings.EventListeners.V2;
using EventHighway.Core.Services.Processings.Events.V2;
using EventHighway.Core.Services.Processings.ListenerEvents.V2;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EventHighway.Core.Clients.EventHighways.V2
{
    internal class ClientV2 : IClientV2
    {
        private readonly string dataConnectionString;
        private readonly EventHandlerBroker eventHandlerBroker;
        private IEventHandlerV2Service eventHandlerV2Service;

        public ClientV2(string dataConnectionString)
        {
            this.dataConnectionString = dataConnectionString;
            this.eventHandlerBroker = new EventHandlerBroker();
            IServiceProvider serviceProvider = ConfigureDependencies();
            InitializeClients(serviceProvider);
        }

        public IClientV2 RegisterEventHandler(IEventHandler eventHandler)
        {
            this.eventHandlerV2Service.RegisterEventHandlerV2(eventHandler);
            return this;
        }

        public IArchivingEventV2Client ArchivingEventV2Client { get; private set; }
        public IEventAddressV2Client EventAddressV2Client { get; private set; }
        public IEventListenerV2Client EventListenerV2Client { get; private set; }
        public IEventV2Client EventV2Client { get; private set; }
        public IListenerEventV2Client ListenerEventV2Client { get; private set; }

        private void InitializeClients(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var storageBroker = scope.ServiceProvider.GetRequiredService<StorageBroker>();
                storageBroker.Database.Migrate();
            }

            this.eventHandlerV2Service =
                serviceProvider.GetRequiredService<IEventHandlerV2Service>();

            this.ArchivingEventV2Client =
                serviceProvider.GetRequiredService<IArchivingEventV2Client>();

            this.EventAddressV2Client =
                serviceProvider.GetRequiredService<IEventAddressV2Client>();

            this.EventListenerV2Client =
                serviceProvider.GetRequiredService<IEventListenerV2Client>();

            this.EventV2Client =
                serviceProvider.GetRequiredService<IEventV2Client>();

            this.ListenerEventV2Client =
                serviceProvider.GetRequiredService<IListenerEventV2Client>();
        }

        private IServiceProvider ConfigureDependencies()
        {
            var serviceCollection =
                new ServiceCollection();

            RegisterBrokers(serviceCollection);
            RegisterFoundationServices(serviceCollection);
            RegisterProcessingServices(serviceCollection);
            RegisterOrchestrationServices(serviceCollection);
            RegisterCoordinationServices(serviceCollection);
            RegisterClients(serviceCollection);

            return serviceCollection.BuildServiceProvider();
        }

        private void RegisterBrokers(IServiceCollection services)
        {
            services.AddLogging();
            services.AddTransient<ILoggingBroker, LoggingBroker>();
            services.AddTransient<IDateTimeBroker, DateTimeBroker>();

            services.AddTransient<
                IStorageBroker,
                StorageBroker>(_ =>
                    new StorageBroker(this.dataConnectionString));

            services.AddTransient<StorageBroker>(_ =>
                new StorageBroker(this.dataConnectionString));

            services.AddSingleton<IEventHandlerBroker>(this.eventHandlerBroker);
        }

        private static void RegisterFoundationServices(IServiceCollection services)
        {
            services.AddTransient<IEventHandlerV2Service, EventHandlerV2Service>();
            services.AddTransient<IEventV2Service, EventV2Service>();
            services.AddTransient<IEventAddressV2Service, EventAddressV2Service>();
            services.AddTransient<IEventListenerV2Service, EventListenerV2Service>();
            services.AddTransient<IListenerEventV2Service, ListenerEventV2Service>();
            services.AddTransient<IEventCallV2Service, EventCallV2Service>();
            services.AddTransient<IEventArchiveV2Service, EventArchiveV2Service>();
            services.AddTransient<IListenerEventArchiveV2Service, ListenerEventArchiveV2Service>();
        }

        private static void RegisterProcessingServices(IServiceCollection services)
        {
            services.AddTransient<
                IEventV2ProcessingService,
                EventV2ProcessingService>();

            services.AddTransient<
                IEventAddressV2ProcessingService,
                EventAddressV2ProcessingService>();

            services.AddTransient<
                IEventListenerV2ProcessingService,
                EventListenerV2ProcessingService>();

            services.AddTransient<
                IListenerEventV2ProcessingService,
                ListenerEventV2ProcessingService>();

            services.AddTransient<
                IEventCallV2ProcessingService,
                EventCallV2ProcessingService>();
        }

        private static void RegisterOrchestrationServices(IServiceCollection services)
        {
            services.AddTransient<
                IEventV2OrchestrationService,
                EventV2OrchestrationService>();

            services.AddTransient<
                IEventListenerV2OrchestrationService,
                EventListenerV2OrchestrationService>();

            services.AddTransient<
                IArchivingEventV2OrchestrationService,
                ArchivingEventV2OrchestrationService>();

            services.AddTransient<
                IEventArchiveV2OrchestrationService,
                EventArchiveV2OrchestrationService>();
        }

        private static void RegisterCoordinationServices(IServiceCollection services)
        {
            services.AddTransient<
                IEventV2CoordinationService,
                EventV2CoordinationService>();

            services.AddTransient<
                IArchivingEventV2CoordinationService,
                ArchivingEventV2CoordinationService>();
        }

        private static void RegisterClients(IServiceCollection services)
        {
            services.AddTransient<
                IEventAddressV2Client,
                EventAddressV2Client>();

            services.AddTransient<
                IEventListenerV2Client,
                EventListenerV2Client>();

            services.AddTransient<
                IEventV2Client,
                EventV2Client>();

            services.AddTransient<
                IArchivingEventV2Client,
                ArchivingEventV2Client>();

            services.AddTransient<
                IListenerEventV2Client,
                ListenerEventV2Client>();
        }
    }
}
