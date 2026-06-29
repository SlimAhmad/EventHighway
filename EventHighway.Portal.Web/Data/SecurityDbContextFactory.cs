// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EventHighway.Portal.Web.Data
{
    // Design-time factory so `dotnet ef migrations` can construct the context without the
    // application's runtime DI wiring (keeps the DATA migration independent of EXPOSERS).
    public class SecurityDbContextFactory : IDesignTimeDbContextFactory<SecurityDbContext>
    {
        public SecurityDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration =
                new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

            string connectionString =
                configuration.GetConnectionString("EventHighwaySecurityConnection")!;

            var optionsBuilder = new DbContextOptionsBuilder<SecurityDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new SecurityDbContext(optionsBuilder.Options);
        }
    }
}
