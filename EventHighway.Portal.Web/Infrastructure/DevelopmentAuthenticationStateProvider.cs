// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace EventHighway.Portal.Web.Infrastructure
{
    // TEMPORARY (Phases 1-4): there is no real authentication provider yet. This stand-in
    // authenticates a development user in both roles so every nav item and page renders while
    // the portal is being built. It is replaced by ASP.NET Core Identity in Phase 5, at which
    // point the role-aware nav and [Authorize] attributes start enforcing for real.
    public sealed class DevelopmentAuthenticationStateProvider : AuthenticationStateProvider
    {
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = new ClaimsIdentity(
                claims: new[]
                {
                    new Claim(ClaimTypes.Name, "developer"),
                    new Claim(ClaimTypes.Role, "Administrators"),
                    new Claim(ClaimTypes.Role, "Users"),
                },
                authenticationType: "Development");

            var principal = new ClaimsPrincipal(identity);

            return Task.FromResult(new AuthenticationState(principal));
        }
    }
}
