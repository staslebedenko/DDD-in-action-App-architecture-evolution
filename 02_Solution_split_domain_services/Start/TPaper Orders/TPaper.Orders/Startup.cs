using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using TPaper.Orders;

[assembly: FunctionsStartup(typeof(Startup))]

namespace TPaper.Orders
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();

            builder.Services.AddLogging(options =>
            {
                options.AddFilter("TPaper.Orders", LogLevel.Information);
            });

            builder.Services.AddOptions<ProjectOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("ProjectOptions").Bind(settings);
                });

            string sqlString = Environment.GetEnvironmentVariable("SqlPaperString");
            string sqlPassword = Environment.GetEnvironmentVariable("SqlPaperPassword");
            string connectionString = new SqlConnectionStringBuilder(sqlString) { Password = sqlPassword }.ConnectionString;

            builder.Services.AddDbContextPool<PaperDbContext>(options =>
            {
                if (!string.IsNullOrEmpty(connectionString))
                {
                    options.UseSqlServer(connectionString);
                }
            });

            PaperDbContext.ExecuteMigrations(connectionString);

            builder.Services.AddHttpClient();
        }
    }
}
