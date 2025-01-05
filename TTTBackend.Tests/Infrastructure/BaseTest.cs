using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTTBackend.Tests.Infrastructure
{
    public class BaseTest : IClassFixture<WebApplicationFactory<Program>>
    {
        protected readonly HttpClient _client;

        public BaseTest(WebApplicationFactory<Program> factory)
        {
            var configuredFactory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    DotNetEnv.Env.Load();
                });
            });

            _client = configuredFactory.CreateClient();
        }
    }
}
