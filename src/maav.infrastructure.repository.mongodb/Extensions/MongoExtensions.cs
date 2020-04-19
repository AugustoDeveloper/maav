using MAAV.Domain.Entities;
using MAAV.Domain.Repositories;
using MAAV.Infrastrucuture.Repository.MongoDB.Checks;
using MAAV.Infrastrucuture.Repository.MongoDB.Conventions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using System.Collections.Generic;
using System.Linq;

namespace MAAV.Infrastructure.Repository.MongoDB.Extensions
{
    static public class MongoExtension
    {
        public static IServiceCollection AddMongoDB(this IServiceCollection collection, IConfiguration configuration, IHealthChecksBuilder healthCheckBuild)
        {
            var connectionString = configuration.GetConnectionString("MAAVConnectionString");
            
            collection.AddScoped<IApplicationRepository>((s) => new ApplicationRepository(connectionString));
            collection.AddScoped<IOrganisationRepository>((s) => new OrganisationRepository(connectionString));
            collection.AddScoped<ITeamRepository>((s) => new TeamRepository(connectionString));
            collection.AddScoped<IUserRepository>((s) => new UserRepository(connectionString));

            healthCheckBuild.AddCheck("mongodb_health", new MongoHealthCheck(connectionString),
                                                                     HealthStatus.Unhealthy,
                                                                     new [] { "mongodb"});

            ConfigureConvention();
            RegisterClassMap();
            return collection;
        }

        private static void ConfigureConvention()
        {
            var pack = new ConventionPack();
            pack.Add(new SnakeCaseElementNameConvention());

            ConventionRegistry.Register("snake_case", pack, _ => true);
        }

        private static void RegisterClassMap()
        {
            BsonClassMap.RegisterClassMap<Application>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(a => a.Id).SetIdGenerator(ObjectIdGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<Version>(cm =>
            {
                cm.AutoMap();
            });

            BsonClassMap.RegisterClassMap<BranchVersion>(cm =>
            {
                cm.AutoMap();
            });

            BsonClassMap.RegisterClassMap<Organisation>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(o => o.Name);
            });

            BsonClassMap.RegisterClassMap<Team>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(o => o.Id);
            });

            BsonClassMap.RegisterClassMap<User>(cm =>
            {
                cm.AutoMap();
            });

            BsonClassMap.RegisterClassMap<TeamUser>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(o => o.Id).SetIdGenerator(ObjectIdGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<TeamPermission>(cm =>
            {
                cm.AutoMap();
            });

            BsonClassMap.RegisterClassMap<ScheMapVersion>(cm =>
            {
                cm.AutoMap();
            });

            BsonClassMap.RegisterClassMap<BranchMap>(cm =>
            {
                cm.AutoMap();
            });
        }
    }
}