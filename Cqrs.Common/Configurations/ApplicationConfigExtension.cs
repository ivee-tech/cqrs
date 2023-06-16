using Cqrs.Common.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cqrs.Common.Configurations
{
    public static class ApplicationConfigExtension
    {
        public static void UseCommonServices(this IApplicationBuilder app, IWebHostEnvironment env, IConfiguration configuration, ILogger logger, bool useAuthorization = false)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseAuthentication();

            app.UseCors();

            app.UseMiddleware<ExceptionHandlerMiddleware>();

            app.UseSession();

            app.UseHttpsRedirection();

            app.UseRouting();

            if(useAuthorization)
            {
                app.UseAuthorization();
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseHealthChecks("/health");

            logger.LogInformation($"The target environment is: '{env.EnvironmentName}'. ");
        }
    }
}
