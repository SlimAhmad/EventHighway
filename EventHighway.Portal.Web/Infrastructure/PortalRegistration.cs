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
            // PublicationOnly so a failed first attempt (e.g. database unavailable) is not cached
            // permanently — the next request retries construction and the portal recovers once the
            // Core database is reachable, without an app restart.
            services.AddSingleton(_ => new Lazy<IClientV2>(
                valueFactory: () => CreateClientV2(configuration),
                mode: LazyThreadSafetyMode.PublicationOnly));

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
