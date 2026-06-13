// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Orchestrations.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.Events.V2.Exceptions;
using EventHighway.Core.Services.Processings.Events.V2;
using EventHighway.Core.Services.Processings.ListenerEvents.V2;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Services.Orchestrations.ArchivingEvents.V2
{
    internal partial class ArchivingEventV2OrchestrationService : IArchivingEventV2OrchestrationService
    {
        private readonly IEventV2ProcessingService eventV2ProcessingService;
        private readonly IListenerEventV2ProcessingService listenerEventV2ProcessingService;
        private readonly ILoggingBroker loggingBroker;

        public ArchivingEventV2OrchestrationService(
            IEventV2ProcessingService eventV2ProcessingService,
            IListenerEventV2ProcessingService listenerEventV2ProcessingService,
            ILoggingBroker loggingBroker)
        {
            this.eventV2ProcessingService = eventV2ProcessingService;
            this.listenerEventV2ProcessingService = listenerEventV2ProcessingService;
            this.loggingBroker = loggingBroker;
        }

        public async IAsyncEnumerable<EventV2> RetrieveAllDeadEventV2sWithListenersAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            IQueryable<EventV2> eventV2s;

            try
            {
                eventV2s =
                    await this.eventV2ProcessingService.RetrieveAllDeadEventV2sWithListenersAsync();
            }
            catch (EventV2ProcessingDependencyException eventV2ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventV2ProcessingDependencyException);
            }
            catch (EventV2ProcessingServiceException eventV2ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventV2ProcessingServiceException);
            }
            catch (Exception exception)
            {
                var failedArchivingEventV2OrchestrationServiceException =
                    new FailedArchivingEventV2OrchestrationServiceException(
                        message: "Failed event service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedArchivingEventV2OrchestrationServiceException);
            }

            await foreach (EventV2 eventV2 in StreamDeadEventV2sAsync(eventV2s, cancellationToken))
            {
                yield return eventV2;
            }
        }

        private static async IAsyncEnumerable<EventV2> StreamDeadEventV2sAsync(
            IQueryable<EventV2> query,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (query is IAsyncEnumerable<EventV2> asyncEnumerable)
            {
                await foreach (EventV2 eventV2 in asyncEnumerable.WithCancellation(cancellationToken))
                {
                    yield return eventV2;
                }
            }
            else
            {
                foreach (EventV2 eventV2 in query)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    yield return eventV2;
                }

                await Task.CompletedTask;
            }
        }

        public ValueTask RemoveEventV2AndListenerEventV2sAsync(
            EventV2 eventV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateEventV2IsNotNull(eventV2);

            foreach (ListenerEventV2 listenerEventV2 in eventV2.ListenerEventV2s)
            {
                await this.listenerEventV2ProcessingService
                    .RemoveListenerEventV2ByIdAsync(listenerEventV2.Id, cancellationToken);
            }

            await this.eventV2ProcessingService.RemoveEventV2ByIdAsync(eventV2.Id, cancellationToken);
        });
    }
}
