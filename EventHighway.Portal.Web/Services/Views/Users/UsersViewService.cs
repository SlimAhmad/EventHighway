// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Brokers.Identities;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Views.Users;

namespace EventHighway.Portal.Web.Services.Views.Users
{
    public partial class UsersViewService : IUsersViewService
    {
        private readonly IIdentityBroker identityBroker;
        private readonly ILoggingBroker loggingBroker;

        public UsersViewService(
            IIdentityBroker identityBroker,
            ILoggingBroker loggingBroker)
        {
            this.identityBroker = identityBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<List<UserView>> RetrieveAllUsersAsync(
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public ValueTask<UserView> RetrieveUserByIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public ValueTask<UserView> AddUserAsync(
            UserView user,
            string password,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public ValueTask RemoveUserByIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public ValueTask AddUserToRoleAsync(
            Guid userId,
            string roleName,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public ValueTask RemoveUserFromRoleAsync(
            Guid userId,
            string roleName,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public ValueTask<List<string>> RetrieveAllRoleNamesAsync(
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
