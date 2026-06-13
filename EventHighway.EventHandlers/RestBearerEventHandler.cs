// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.EventHandlers.Brokers.Apis;
using EventHighway.EventHandlers.Models.Exposers.RestBearerEventHandlers.Exceptions;
using EventHighway.EventHandlers.Models.Foundations.Rest.Exceptions;
using EventHighway.EventHandlers.Services.Rest;
using EventHighway.EventHandlers.Servies.Rest;
using Xeptions;

namespace EventHighway.EventHandlers
{
    public class RestBearerEventHandler : IEventHandler
    {
        private readonly IRestService restService;

        public RestBearerEventHandler(Guid id)
        {
            this.Id = id;
            this.restService = new RestService(new ApiBroker());
        }

        internal RestBearerEventHandler(Guid id, IRestService restService)
        {
            this.Id = id;
            this.restService = restService;
        }

        public Guid Id { get; }
        public string Name => nameof(RestBearerEventHandler);
        public IEnumerable<string> RequiredParams =>
            new[] { "Url", "ClientId", "ClientSecret", "Scope", "GrantType", "TokenUrl" };


        public async ValueTask<EventHandlerResult> HandleAsync(
            string content,
            IReadOnlyDictionary<string, string> handlerParams,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.restService.PostWithBearerTokenAsync(content, handlerParams, cancellationToken);
            }
            catch (RestServiceValidationException restServiceValidationException)
            {
                throw CreateValidationException(
                    restServiceValidationException.InnerException as Xeption);
            }
            catch (RestServiceDependencyValidationException restServiceDependencyValidationException)
            {
                throw CreateValidationException(
                    restServiceDependencyValidationException.InnerException as Xeption);
            }
            catch (RestServiceDependencyException restServiceDependencyException)
            {
                throw CreateDependencyException(
                    restServiceDependencyException.InnerException as Xeption);
            }
            catch (RestServiceException restServiceException)
            {
                throw CreateDependencyException(
                    restServiceException.InnerException as Xeption);
            }
            catch (Exception exception)
            {
                throw CreateServiceException(
                    exception.InnerException as Xeption);
            }
        }

        private static RestBearerEventHandlerValidationException CreateValidationException(
            Xeption innerException) =>
            new RestBearerEventHandlerValidationException(
                message: "Rest bearer event handler validation error occurred, fix errors and try again.",
                innerException,
                data: innerException.Data);

        private static RestBearerEventHandlerDependencyException CreateDependencyException(
            Xeption innerException) =>
            new RestBearerEventHandlerDependencyException(
                message: "Rest bearer event handler dependency error occurred, contact support.",
                innerException,
                data: innerException.Data);

        private static RestBearerEventHandlerServiceException CreateServiceException(
            Xeption innerException) =>
            new RestBearerEventHandlerServiceException(
                message: "Rest bearer event handler service error occurred, contact support.",
                innerException,
                data: innerException.Data);
    }
}
