// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Brokers.Identities;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Foundations.Users;
using EventHighway.Portal.Web.Models.Views.Users;
using EventHighway.Portal.Web.Models.Views.Users.Exceptions;

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

        private const string AdministratorsRole = "Administrators";

        public ValueTask<List<UserView>> RetrieveAllUsersAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(() =>
        {
            List<UserView> users = this.identityBroker.SelectAllUsers()
                .Select(AsView)
                .ToList();

            return new ValueTask<List<UserView>>(users);
        });

        public ValueTask<UserView> RetrieveUserByIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            AppUser user = await this.identityBroker.SelectUserByIdAsync(userId);

            IList<string> roles = await this.identityBroker.SelectUserRolesAsync(user);

            UserView view = AsView(user);
            view.Roles = roles.ToList();

            return view;
        });

        public ValueTask<UserView> AddUserAsync(
            UserView user,
            string password,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            var userToAdd = new AppUser
            {
                UserName = user.UserName,
                Email = string.IsNullOrWhiteSpace(user.Email)
                    ? $"{user.UserName}@eventhighway.local"
                    : user.Email,
                EmailConfirmed = true
            };

            await this.identityBroker.InsertUserAsync(userToAdd, password);

            return AsView(userToAdd);
        });

        public ValueTask RemoveUserByIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            AppUser user = await this.identityBroker.SelectUserByIdAsync(userId);

            IList<string> roles = await this.identityBroker.SelectUserRolesAsync(user);

            if (roles.Contains(AdministratorsRole))
            {
                await EnsureNotLastAdministratorAsync();
            }

            await this.identityBroker.DeleteUserAsync(user);
        });

        public ValueTask AddUserToRoleAsync(
            Guid userId,
            string roleName,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            AppUser user = await this.identityBroker.SelectUserByIdAsync(userId);

            await this.identityBroker.InsertUserToRoleAsync(user, roleName);
        });

        public ValueTask RemoveUserFromRoleAsync(
            Guid userId,
            string roleName,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            if (roleName == AdministratorsRole)
            {
                await EnsureNotLastAdministratorAsync();
            }

            AppUser user = await this.identityBroker.SelectUserByIdAsync(userId);

            await this.identityBroker.DeleteUserFromRoleAsync(user, roleName);
        });

        public ValueTask<List<string>> RetrieveAllRoleNamesAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(() =>
        {
            List<string> roleNames = this.identityBroker.SelectAllRoles()
                .Select(role => role.Name)
                .Where(name => name != null)
                .Select(name => name!)
                .ToList();

            return new ValueTask<List<string>>(roleNames);
        });

        public ValueTask<UserView> ModifyUserAsync(
            UserView user,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public ValueTask ConfirmUserEmailAsync(
            Guid userId,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public ValueTask<string> GenerateEmailConfirmationTokenAsync(
            Guid userId,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public ValueTask<string> GeneratePasswordResetTokenAsync(
            Guid userId,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public ValueTask LockUserAsync(
            Guid userId,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public ValueTask UnlockUserAsync(
            Guid userId,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public ValueTask ResetAccessFailedCountAsync(
            Guid userId,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public ValueTask SetTwoFactorEnabledAsync(
            Guid userId,
            bool enabled,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public ValueTask DisableUserAsync(
            Guid userId,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public ValueTask EnableUserAsync(
            Guid userId,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        private async ValueTask EnsureNotLastAdministratorAsync()
        {
            IList<AppUser> administrators =
                await this.identityBroker.SelectUsersInRoleAsync(AdministratorsRole);

            if (administrators.Count <= 1)
            {
                throw new LastAdministratorUsersViewException();
            }
        }

        private static UserView AsView(AppUser user) =>
            new UserView
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Roles = new List<string>()
            };
    }
}
