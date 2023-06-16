using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Cqrs.Application.Interfaces;
using Cqrs.Application.Services;
// using Cqrs.Application.Services;

namespace Cqrs.Application.Configurations
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDataService, DataService>();
            return services;
        }
    }
}
