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
            services.AddControllers();
            services
                .AddScoped<IOrganisationService, OrganisationService>()
                .AddScoped<IApplicationService, ApplicationService>()
                .AddScoped<IVersionService, ApplicationService>()
                .AddScoped<ITeamService, TeamService>()
                .AddScoped<IUserService, UserService>()
                .AddMapping()
                .AddLiteDb(Configuration)                
                .AddAuthorization(options => 
                {
                    options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .Build();
                })
                .AddBearerTokenValidation(Configuration.GetValue<string>("Auth:SecretKey"), TimeSpan.FromMinutes(30));
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
