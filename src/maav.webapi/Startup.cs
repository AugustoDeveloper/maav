using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MAAV.Application;
using MAAV.Application.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MAAV.Infrastructure.Repository.LiteDB.Extensions;
using MAAV.WebAPI.Extensions;
using MAAV.Infrastructure.Repository.MongoDB.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using MAAV.Domain.Repositories;

namespace MAAV.WebAPI
{
    public class Startup
    {
          public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddControllers();
            var healthCheckBuilder = services.AddHealthChecks();
            services
                .AddScoped<IOrganisationService, OrganisationService>()
                .AddScoped<IApplicationService, ApplicationService>()
                .AddScoped<IVersionService, ApplicationService>()
                .AddScoped<ITeamService, TeamService>()
                .AddScoped<IUserService, UserService>()
                .AddScoped<IGithubEventResultService, GithubEventResultService>()
                .AddSingleton<IGitHubWebHookService, GitHubWebHookService>(svc =>
                {
                    return new GitHubWebHookService(
                        (IGithubEventResultRepository)MongoExtension.GetRepository<IGithubEventResultRepository>(),
                        (IApplicationRepository)MongoExtension.GetRepository<IApplicationRepository>()
                    );
                })
                .AddMapping()
                //.AddLiteDb(Configuration)
                .AddMongoDB(Configuration, healthCheckBuilder)
                .AddAuthorization(options => 
                {
                    options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .Build();
                })
                .AddBearerTokenValidation(Configuration.GetValue<string>("Auth:SecretKey"), TimeSpan.FromMinutes(120));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            var constr = Configuration.GetConnectionString("VersionDbConnString");
            var logger = loggerFactory.CreateLogger<Startup>();

            app.UseRouting();
            app.UseAuthorization();
            app.UseAuthentication();

            app.UseCors(op =>
            {
                op.AllowAnyOrigin();
                op.AllowAnyMethod();
                op.AllowAnyHeader();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/healthcheck", new HealthCheckOptions
                {
                    ResultStatusCodes = 
                    {
                        [HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [HealthStatus.Unhealthy] = StatusCodes.Status500InternalServerError,
                        [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
                    },
                    AllowCachingResponses = false,
                    ResponseWriter = HealthCheckExtension.WriteHealthCheck
                });
            });
        }
    }
}
