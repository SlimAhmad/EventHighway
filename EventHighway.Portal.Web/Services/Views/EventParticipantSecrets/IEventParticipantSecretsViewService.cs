// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Models.Views.EventParticipantSecrets;

namespace EventHighway.Portal.Web.Services.Views.EventParticipantSecrets
{
    public interface IEventParticipantSecretsViewService
    {
        ValueTask<List<EventParticipantSecretView>> RetrieveSecretsByParticipantAsync(
            Guid participantId,
            CancellationToken cancellationToken = default);

        ValueTask<EventParticipantSecretView> AddSecretAsync(
            EventParticipantSecretView secret,
            CancellationToken cancellationToken = default);
    }
}
