// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Portal.Web.Models.Foundations.Roles;
using EventHighway.Portal.Web.Models.Foundations.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Portal.Web.Data
{
    public class SecurityDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public SecurityDbContext(DbContextOptions<SecurityDbContext> options)
            : base(options)
        { }
    }
}
