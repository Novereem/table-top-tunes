using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Make Tests run sequentially
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace TTTBackend.Tests.Factories
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<ApplicationDbContext>(opts =>
                {
                    opts.UseInMemoryDatabase("TestDb");
                });
            });

            Environment.SetEnvironmentVariable("JWT_SECRET_KEY", "test-secret-key-that-is-very-long-because-it-needs-to-be");
            Environment.SetEnvironmentVariable("JWT_ISSUER", "test-issuer");
            Environment.SetEnvironmentVariable("JWT_AUDIENCE", "test-audience");
            Environment.SetEnvironmentVariable("DB_CONNECTION_STRING", "Server=localhost;Port=3306;Database=tttdatabase;User=root;Password=testpassword");

            return base.CreateHost(builder);
        }
    }
}
