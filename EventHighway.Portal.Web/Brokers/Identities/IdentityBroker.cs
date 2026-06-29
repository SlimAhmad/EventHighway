// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Models.Foundations.Roles;
using EventHighway.Portal.Web.Models.Foundations.Users;
using Microsoft.AspNetCore.Identity;

namespace EventHighway.Portal.Web.Brokers.Identities
{
    public sealed class IdentityBroker : IIdentityBroker
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<AppRole> roleManager;

        public IdentityBroker(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public IQueryable<AppUser> SelectAllUsers() =>
            this.userManager.Users;

        public async ValueTask<AppUser> SelectUserByIdAsync(Guid userId) =>
            await this.userManager.FindByIdAsync(userId.ToString());

        public async ValueTask<IdentityResult> InsertUserAsync(AppUser user, string password) =>
            await this.userManager.CreateAsync(user, password);

        public async ValueTask<IdentityResult> DeleteUserAsync(AppUser user) =>
            await this.userManager.DeleteAsync(user);

        public async ValueTask<IList<string>> SelectUserRolesAsync(AppUser user) =>
            await this.userManager.GetRolesAsync(user);

        public async ValueTask<IdentityResult> InsertUserToRoleAsync(AppUser user, string roleName) =>
            await this.userManager.AddToRoleAsync(user, roleName);

        public async ValueTask<IdentityResult> DeleteUserFromRoleAsync(AppUser user, string roleName) =>
            await this.userManager.RemoveFromRoleAsync(user, roleName);

        public async ValueTask<IList<AppUser>> SelectUsersInRoleAsync(string roleName) =>
            await this.userManager.GetUsersInRoleAsync(roleName);

        public IQueryable<AppRole> SelectAllRoles() =>
            this.roleManager.Roles;
    }
}
