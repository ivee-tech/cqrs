using AutoMapper;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using Cqrs.Common.Configurations;
using Cqrs.Application.Configurations;
using Cqrs.Infrastructure.Configurations;
using System.Linq;
using System.Security.Claims;
using Cqrs.Common.Utilities;
using Newtonsoft.Json.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Cqrs.Api.Configurations;

namespace Cqrs.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            if (Environment.IsProduction())
            {
                Configuration.GetSection("ConnectionStrings")["DefaultConnection"] = GetClientSecret("Settings--ConnectionString");
            }
            var mapperConfig = new MapperConfiguration(config =>
                config.AddMaps("Cqrs.Api", "Cqrs.Infrastructure"));
            services.AddSingleton(mapperConfig.CreateMapper());

            services.AddAllCommonServices(Configuration);

            services.AddApplication(Configuration);
            services.AddInfrastructure(Configuration);

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.Use(async (context, next) =>
            {
                bool bContinue = await CheckAccessToken(logger, context);
                if (bContinue)
                {
                    await next();
                }
            });
            app.UseCommonServices(env, Configuration, logger, useAuthorization: false);
        }

        private async Task<bool> CheckAccessToken(ILogger<Startup> logger, HttpContext context)
        {
            var noCheckMethods = new string[] { "CONNECT", "OPTIONS", "TRACE" };
            var bContinue = true;
            if (!noCheckMethods.Contains(context.Request.Method.ToUpper()))
            {
                var sToken = context.Request.Headers.Authorization.FirstOrDefault()?.Substring("Bearer ".Length);
                if (!string.IsNullOrEmpty(sToken))
                {
                    try
                    {
                        var token = JwtHelper.ParseToken(sToken);
                        var identity = new ClaimsIdentity(token.Claims);
                        context.User = new ClaimsPrincipal(identity);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex.ToString());
                        await ReturnErrorResponse(context, "Invalid token");
                        bContinue = false;
                    }
                }
                else
                {
                    // await ReturnErrorResponse(context, "Missing token");
                    bContinue = false;
                }
            }

            return true; // bContinue;
        }
        private async Task ReturnErrorResponse(HttpContext context, string msg)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync(msg);
        }

        private string GetClientSecret(string secretName)
        {
            var credential = new DefaultAzureCredential();
            var kvUrl = $"https://{Configuration["KeyVaultName"]}.vault.azure.net";
            Console.Write(kvUrl);
            var client = new SecretClient(new Uri(kvUrl), credential);
            var clientSecret = client.GetSecret(secretName);
            return clientSecret?.Value?.Value;
        }
    }
}
