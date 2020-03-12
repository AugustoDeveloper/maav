using LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MAAV.Domain.Entities;
using MAAV.Domain.Repositories;

namespace MAAV.Infrastructure.Repository.LiteDB.Extensions
{
    static public class LiteDbRepositoryService
    {
        public static IServiceCollection AddLiteDb(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("VersionDbConnString");

            services.AddScoped<IApplicationRepository>((s) => new ApplicationRepository(connectionString));
            services.AddScoped<IOrganisationRepository>((s) => new OrganisationRepository(connectionString));
            services.AddScoped<ITeamRepository>((s) => new TeamRepository(connectionString));
            services.AddScoped<IUserRepository>((s) => new UserRepository(connectionString));
            
            var mapper = BsonMapper.Global;
            mapper.Entity<Application>()
                .Id(a => a.Id)
                .Field(a => a.Name, "name")
                .Field(a => a.OrganisationName, "organisation_name")
                .Field(a => a.TeamName, "team_name")
                .Field(a => a.ScheMap, "schemap")
                .Field(a => a.BranchVersions, "branch_versions")
                .Field(a => a.InitialVersion, "initial_version");

            mapper.Entity<Version>()
                .Field(s => s.Label, "label")
                .Field(s => s.Major, "major_version")
                .Field(s => s.Minor, "minor_version")
                .Field(s => s.Patch, "patch_version");

            mapper.Entity<BranchVersion>()
                .Field(s => s.BranchMapName, "branch_map_name")
                .Field(s => s.IsEnabled, "is_enabled")
                .Field(s => s.Version, "version");

            mapper.Entity<Organisation>()
                .Field(s => s.Name, "name")
                    .Id(o => o.Name, false)
                .Field(s => s.ScheMap, "schemap")
                .Field(s => s.Teams, "teams");

            mapper.Entity<Team>()
                .Field(t => t.Id, "id").Id(t => t.Id, autoId: true)
                .Field(t => t.OrganisationName, "organisation_name")
                .Field(t => t.Name, "name")
                .Field(t => t.ScheMap, "schemap")
                .Field(t => t.Users, "users")
                .Field(t => t.Applications, "applications");

            mapper.Entity<User>()
                .Field(t => t.Id, "id").Id(t => t.Id, autoId: true)
                .Field(u => u.Username, "username")
                .Field(u => u.OrganisationName, "organisation_name")
                .Field(u => u.OrganisationRoles, "organisation_roles")
                .Field(u => u.PasswordHash, "password_hash")
                .Field(u => u.PasswordSalt, "password_salt")
                .Field(u => u.FirstName, "last_name")
                .Field(u => u.LastName, "last_name")
                .Field(u => u.TeamRoles, "team_roles");
            
            mapper.Entity<UserTeamRole>()
                .Field(utr => utr.TeamId, "team_id")
                .Field(utr => utr.Roles, "roles")
                .Field(utr => utr.TeamName, "team_name");

            mapper.Entity<ScheMapVersion>()
                .Field(smv => smv.Format, "fomat")
                .Field(smv => smv.Branches, "branches");

            mapper.Entity<BranchMap>()
                .Field(bm => bm.Name, "name")
                .Field(bm => bm.SuffixFormat, "suffix_format")
                .Field(bm => bm.Increment, "increment")
                .Field(bm => bm.Label, "label")
                .Field(bm => bm.BranchPattern, "branch_pattern")
                .Field(bm => bm.AllowBumpMajor, "allow_bump_major");


            return services;
        }
    }
}