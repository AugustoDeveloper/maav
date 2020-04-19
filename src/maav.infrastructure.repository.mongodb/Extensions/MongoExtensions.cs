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
        private static string ConnectionString { get; set; }

        public static IServiceCollection AddMongoDB(this IServiceCollection collection, IConfiguration configuration, IHealthChecksBuilder healthCheckBuild)
        {
            ConnectionString = configuration.GetConnectionString("MAAVConnectionString");
            
            collection.AddScoped<IApplicationRepository>((s) => new ApplicationRepository(ConnectionString));
            collection.AddScoped<IOrganisationRepository>((s) => new OrganisationRepository(ConnectionString));
            collection.AddScoped<ITeamRepository>((s) => new TeamRepository(ConnectionString));
            collection.AddScoped<IUserRepository>((s) => new UserRepository(ConnectionString));
            collection.AddScoped<IKeyBranchVersionHistoryRepository>((s) => new KeyBranchVersionHistoryRepository(ConnectionString));
            collection.AddScoped<IGithubEventResultRepository>((s) => new GithubEventResultRepository(ConnectionString));


            healthCheckBuild.AddCheck("mongodb_health", new MongoHealthCheck(ConnectionString),
                                                                     HealthStatus.Unhealthy,
                                                                     new [] { "mongodb"});

            ConfigureConvention();
            RegisterClassMap();
            return collection;
        }

        public static IRepository GetRepository<TRepository>() where TRepository : IRepository
        {
            if (typeof(TRepository) == typeof(IGithubEventResultRepository))
            {
                return new GithubEventResultRepository(ConnectionString);
            }

            if (typeof(TRepository) == typeof(IApplicationRepository))
            {
                return new ApplicationRepository(ConnectionString);
            }

            return default;
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
            });

            BsonClassMap.RegisterClassMap<TeamApplication>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(a => a.Id).SetIdGenerator(StringObjectIdGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<SemanticVersion>(cm =>
            {
                cm.AutoMap();
            });

            BsonClassMap.RegisterClassMap<KeyBranch>(cm =>
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
                cm.MapIdMember(o => o.Id).SetIdGenerator(StringObjectIdGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<TeamPermission>(cm =>
            {
                cm.AutoMap();
            });

            BsonClassMap.RegisterClassMap<BranchActionMap>(cm =>
            {
                cm.AutoMap();
            });

            BsonClassMap.RegisterClassMap<KeyBranchVersioning>(cm =>
            {
                cm.AutoMap();
            });

            BsonClassMap.RegisterClassMap<KeyBranchVersionHistory>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(o => o.Id).SetIdGenerator(StringObjectIdGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<KeyBranchVersion>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(o => o.Id).SetIdGenerator(StringObjectIdGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<BranchActionRequest>(cm =>
            {
                cm.AutoMap();
            });

            BsonClassMap.RegisterClassMap<GithubEventResult>(cm =>
            {
                cm.MapIdMember(e => e.Id).SetIdGenerator(StringObjectIdGenerator.Instance);
                cm.AutoMap();
            });
        }
    }
}