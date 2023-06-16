using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Cqrs.Common.Configurations;
using Cqrs.Application.Interfaces;
using Cqrs.Infrastructure.Repositories;
// using Cqrs.Infrastructure.Repositories.Data;
using Cqrs.Infrastructure.Repositories.DbContexts;

namespace Cqrs.Infrastructure.Configurations
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCommonSqlDbContext<Repositories.DbContexts.DataDbContext>(configuration,
                configuration.GetConnectionString("DefaultConnection"),
                configuration.GetValue<bool>("UseInMemoryDatabase")
                , (DbContextOptionsBuilder options, IConfiguration configuration) => { options.EnableSensitiveDataLogging(true); });

            var sp = services.BuildServiceProvider();
            var dbContext = sp.GetRequiredService<Repositories.DbContexts.DataDbContext>();
            dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            // SeedData.Initiate(dbContext);

            services.AddScoped<IDataRepository, DataRepository>();

            return services;
        }
    }
}
