// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.HealthChecks.V2.Exceptions;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2.Exceptions;
using EventHighway.Core.Services.Coordinations.HealthChecks.V2;
using Xeptions;

namespace EventHighway.Core.Clients.HealthChecks.V2
{
    internal class HealthV2Client : IHealthV2Client
    {
        private readonly IHealthV2CoordinationService healthV2CoordinationService;

        public HealthV2Client(IHealthV2CoordinationService healthV2CoordinationService) =>
            this.healthV2CoordinationService = healthV2CoordinationService;

        public async ValueTask<IEnumerable<HealthCheckItemV2>> RetrieveHealthSummaryV2Async(
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.healthV2CoordinationService
                    .RetrieveHealthSummaryV2Async(cancellationToken);
            }
            catch (HealthV2CoordinationValidationException
                healthV2CoordinationValidationException)
            {
                throw CreateHealthV2ClientValidationException(
                    healthV2CoordinationValidationException.InnerException as Xeption);
            }
            catch (HealthV2CoordinationDependencyValidationException
                healthV2CoordinationDependencyValidationException)
            {
                throw CreateHealthV2ClientValidationException(
                    healthV2CoordinationDependencyValidationException.InnerException as Xeption);
            }
            catch (HealthV2CoordinationDependencyException
                healthV2CoordinationDependencyException)
            {
                throw CreateHealthV2ClientDependencyException(
                    healthV2CoordinationDependencyException.InnerException as Xeption);
            }
            catch (HealthV2CoordinationServiceException
                healthV2CoordinationServiceException)
            {
                throw CreateHealthV2ClientDependencyException(
                    healthV2CoordinationServiceException.InnerException as Xeption);
            }
            catch (Exception exception)
            {
                throw CreateHealthV2ClientServiceException(exception as Xeption);
            }
        }

        private static HealthV2ClientValidationException
            CreateHealthV2ClientValidationException(Xeption innerException)
        {
            return new HealthV2ClientValidationException(
                message: "Health client validation error occurred, fix the errors and try again.",
                innerException: innerException,
                data: innerException.Data);
        }

        private static HealthV2ClientDependencyException
            CreateHealthV2ClientDependencyException(Xeption innerException)
        {
            return new HealthV2ClientDependencyException(
                message: "Health client dependency error occurred, contact support.",
                innerException: innerException,
                data: innerException.Data);
        }

        private static HealthV2ClientServiceException
            CreateHealthV2ClientServiceException(Xeption innerException)
        {
            return new HealthV2ClientServiceException(
                message: "Health client service error occurred, contact support.",
                innerException: innerException,
                data: innerException.Data);
        }
    }
}
