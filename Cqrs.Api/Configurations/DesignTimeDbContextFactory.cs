using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using System;
using System.IO;
using Cqrs.Common.Interfaces;
using Cqrs.Infrastructure.Configurations;
using Cqrs.Infrastructure.Repositories.DbContexts;

namespace Cqrs.Api.Configurations
{
    public class DataDbContextFactory : IDesignTimeDbContextFactory<DataDbContext>
    {
        private readonly ICurrentStateService _currentStateService;

        //public MetadataDbContextFactory(ICurrentStateService currentStateService)
        //{
        //    _currentStateService = currentStateService;
        //}

        public DataDbContext CreateDbContext(string[] args)
        {
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            IConfigurationRoot configuration;
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables();
            var builtConfig = builder.Build();
            Console.WriteLine($"environment: {environment}");

            switch (environment)
            {
                case "CI":
                    var connectionString = Environment.GetEnvironmentVariable("Settings--ConnectionString");
                    Console.WriteLine($"connectionString: {connectionString}");
                    if (!string.IsNullOrEmpty(connectionString))
                    {
                        builtConfig.GetSection("ConnectionStrings")["DefaultConnection"] = connectionString;
                    }
                    configuration = builtConfig;
                    break;
                case "Production":
                    var keyVaultClient = KeyVaultConfig.GetKeyVaultClient();
                    configuration = builder.AddAzureKeyVault(
                        $"https://{builtConfig["KeyVaultName"]}.vault.azure.net/",
                        keyVaultClient,
                        new DefaultKeyVaultSecretManager())
                        .Build();
                    configuration.GetSection("ConnectionStrings")["DefaultConnection"] = configuration["Settings--ConnectionString"];
                    break;
                default:
                    configuration = builtConfig;
                    break;
            }

            var optionsBuilder = new DbContextOptionsBuilder<DataDbContext>();
            //optionsBuilder.UseInMemoryDatabase("CqrsDb");
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            return new DataDbContext(optionsBuilder.Options, _currentStateService);
        }
    }
}
