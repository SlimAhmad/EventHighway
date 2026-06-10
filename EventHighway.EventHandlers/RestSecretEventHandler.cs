// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.EventHandlers.Brokers.Apis;
using EventHighway.EventHandlers.Models.Exposers.RestSecretEventHandlers.Exceptions;
using EventHighway.EventHandlers.Models.Foundations.Rest.Exceptions;
using EventHighway.EventHandlers.Services.Rest;
using EventHighway.EventHandlers.Servies.Rest;
using Xeptions;

namespace EventHighway.EventHandlers
{
    public class RestSecretEventHandler : IEventHandler
    {
        private readonly IRestService restService;

        public RestSecretEventHandler() =>
            this.restService = new RestService(new ApiBroker());

        internal RestSecretEventHandler(IRestService restService) =>
            this.restService = restService;

        public string Name => nameof(RestSecretEventHandler);
        public IEnumerable<string> RequiredParams => new[] { "Url", "Secret" };

        public async ValueTask<EventHandlerResult> HandleAsync(
            string content,
            IReadOnlyDictionary<string, string> handlerParams,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.restService.PostWithSecretAsync(content, handlerParams, cancellationToken);
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

        private static RestSecretEventHandlerValidationException CreateValidationException(
            Xeption innerException) =>
            new RestSecretEventHandlerValidationException(
                message: "Rest secret event handler validation error occurred, fix errors and try again.",
                innerException,
                data: innerException.Data);

        private static RestSecretEventHandlerDependencyException CreateDependencyException(
            Xeption innerException) =>
            new RestSecretEventHandlerDependencyException(
                message: "Rest secret event handler dependency error occurred, contact support.",
                innerException,
                data: innerException.Data);

        private static RestSecretEventHandlerServiceException CreateServiceException(
            Xeption innerException) =>
            new RestSecretEventHandlerServiceException(
                message: "Rest secret event handler service error occurred, contact support.",
                innerException,
                data: innerException.Data);
    }
}
