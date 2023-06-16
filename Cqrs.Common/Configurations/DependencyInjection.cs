using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Newtonsoft.Json.Converters;
using System;
using System.Linq;
using System.Net.Http;
using Cqrs.Common.ApiClient;
using Cqrs.Common.ApiClient.AppDataApi;
using Cqrs.Common.ApiClient.MetadataApi;
using Cqrs.Common.ApiClient.WorkflowApi;
using Cqrs.Common.EntityFramework;
using Cqrs.Common.Exceptions;
using Cqrs.Common.Interfaces;
using Cqrs.Common.Logging;
using Cqrs.Common.Services;
using Microsoft.AspNetCore.Hosting;

namespace Cqrs.Common.Configurations
{
    public static class DependencyInjection
    {
        public static void AddAllCommonServices(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddCommonB2CAuth(configuration);

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    var allowedOrigins = configuration.GetValue<string>("Settings:AllowedOrigins");
                    if (!string.IsNullOrEmpty(allowedOrigins))
                    {
                        builder.WithOrigins(allowedOrigins.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray())
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    }
                });
            });

            services.AddHttpContextAccessor();

            services.AddSingleton(typeof(ILoggingService<>), typeof(LoggingService<>));

            services.AddTransient<ApiClientHandlerMiddleware>();
            services.AddAppDataApiClient(httpClient => httpClient.BaseAddress = new(configuration.GetValue<string>("ApiClients:AppDataApi:Url")));
            services.AddMetadataApiClient(httpClient => httpClient.BaseAddress = new(configuration.GetValue<string>("ApiClients:MetadataApi:Url")));
            services.AddWorkflowApiClient(httpClient => httpClient.BaseAddress = new(configuration.GetValue<string>("ApiClients:WorkflowApi:Url")));

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
            });

            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddSingleton<ICurrentStateService, CurrentStateService>();

            services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.IsDynamic));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));

            services.AddMemoryCache();
            services.AddSingleton<ICacheService, CacheService>();

            services.AddControllers(options =>
            {
                options.Filters.Add<ControllerLoggingActionFilter>();
            })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                });

            services.AddHealthChecks();

            services.AddOpenApiDocument(options =>
            {
                options.Title = configuration.GetValue<string>("OpenApiDocument:Title");
                options.Version = "V1";
            });
        }

        public static void AddAppDataApiClient(this IServiceCollection services, Action<HttpClient> configureClient)
        {
            services.AddHttpClient<IAppDataApiClient, AppDataApiClient>(httpClient => configureClient(httpClient));
        }

        public static void AddMetadataApiClient(this IServiceCollection services, Action<HttpClient> configureClient)
        {
            services.AddHttpClient<IMetadataApiClient, MetadataApiClient>(httpClient => configureClient(httpClient));
        }

        public static void AddWorkflowApiClient(this IServiceCollection services, Action<HttpClient> configureClient)
        {
            services.AddHttpClient<IWorkflowApiClient, WorkflowApiClient>(httpClient => configureClient(httpClient));
        }

        public static void AddCommonB2CAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                   .AddMicrosoftIdentityWebApi(options =>
                   {
                       configuration.Bind("AzureAdB2C", options);
                       options.TokenValidationParameters.NameClaimType = "name";
                       options.TokenValidationParameters.ValidAudience = configuration["AzureAdB2C:ClientId"];
                   }, options => { configuration.Bind("AzureAdB2C", options); });
        }

        public static void AddCommonSqlDbContext<TDbContext>(
            this IServiceCollection services,
            IConfiguration configuration,
            string connectionString,
            bool useInMemoryDatabase,
            Action<DbContextOptionsBuilder, IConfiguration> additionalConfigurationAction = null)
                where TDbContext : DbContextBase
        {
            services.AddDbContext<TDbContext>(options =>
            {
                if (useInMemoryDatabase)
                {
                    options.UseInMemoryDatabase(nameof(TDbContext));
                }
                else
                {
                    options.UseSqlServer(connectionString);
                }

                additionalConfigurationAction?.Invoke(options, configuration);
            });
        }
    }
}
