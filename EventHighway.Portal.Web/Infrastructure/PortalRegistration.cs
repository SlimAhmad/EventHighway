// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using EventHighway.Core.Clients.EventHighways;
using EventHighway.Core.Clients.EventHighways.V2;
using EventHighway.Core.Models.Configurations;
using EventHighway.Portal.Web.Brokers.DateTimes;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Services.Views.EventAddresses;
using EventHighway.Portal.Web.Services.Views.EventListeners;
using EventHighway.Portal.Web.Services.Views.EventParticipants;
using EventHighway.Portal.Web.Services.Views.EventParticipantSecrets;
using EventHighway.Portal.Web.Services.Views.HealthDashboards;

namespace EventHighway.Portal.Web.Infrastructure
{
    public static class PortalRegistration
    {
        public static IServiceCollection AddPortalBrokers(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // ExecutionAndPublication so the EventHighwayClient is constructed EXACTLY ONCE, even
            // when many dashboard panels first-touch the broker concurrently during interactive
            // prerender. PublicationOnly would let several constructions race, each running
            // Database.Migrate() on the same Core database simultaneously, which fails every call.
            services.AddSingleton(_ => new Lazy<IClientV2>(
                valueFactory: () => CreateClientV2(configuration),
                mode: LazyThreadSafetyMode.ExecutionAndPublication));

            services.AddSingleton<IEventHighwayBroker, EventHighwayBroker>();
            services.AddTransient<IDateTimeBroker, DateTimeBroker>();
            services.AddTransient<ILoggingBroker, LoggingBroker>();

            return services;
        }

        public static IServiceCollection AddPortalViewServices(
            this IServiceCollection services)
        {
            services.AddTransient<IHealthViewService, HealthViewService>();
            services.AddTransient<IEventParticipantsViewService, EventParticipantsViewService>();
            services.AddTransient<
                IEventParticipantSecretsViewService,
                EventParticipantSecretsViewService>();
            services.AddTransient<IEventAddressesViewService, EventAddressesViewService>();
            services.AddTransient<IEventListenersViewService, EventListenersViewService>();

            return services;
        }

        // The EventHighway V2 client is constructed once (it builds its own in-process
        // service provider and migrates the Core database) and registered as a singleton,
        // mirroring SubstrateAppRegistration.CreateEventSubstrateBroker.
        private static IClientV2 CreateClientV2(IConfiguration configuration)
        {
            string connectionString =
                configuration.GetConnectionString("EventHighwayCoreConnection")
                    ?? throw new InvalidOperationException(
                        "Missing connection string 'EventHighwayCoreConnection'.");

            var eventHighwayConfiguration = new EventHighwayConfiguration();

            var eventHighwayClient =
                new EventHighwayClient(connectionString, eventHighwayConfiguration);

            return eventHighwayClient.V2;
        }
    }
}
