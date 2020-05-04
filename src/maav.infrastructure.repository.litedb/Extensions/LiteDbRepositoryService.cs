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
                .Field(a => a.OrganisationId, "organisation_name")
                .Field(a => a.TeamCode, "team_code")
                .Field(a => a.KeyBranches, "key_branches")
                .Field(a => a.InitialVersion, "initial_version");

            mapper.Entity<SemanticVersion>()
                .Field(s => s.Major, "major_version")
                .Field(s => s.Minor, "minor_version")
                .Field(s => s.Patch, "patch_version")
                .Field(s => s.PreRelease, "pre_release")
                .Field(s => s.Build, "minor_version");

            mapper.Entity<KeyBranch>()
                .Field(s => s.Name, "branch_map_name")
                .Field(s => s.IsEnabled, "is_enabled")
                .Field(s => s.BranchPattern, "branch_pattern")
                .Field(s => s.FormatVersion, "format_version");

            mapper.Entity<Organisation>()
                .Field(s => s.Name, "name")
                    .Id(o => o.Name, false)
                .Field(s => s.Teams, "teams");

            mapper.Entity<Team>()
                .Field(t => t.TeamCode, "id").Id(t => t.TeamCode, autoId: true)
                .Field(t => t.OrganisationId, "organisation_id")
                .Field(t => t.Name, "name")
                .Field(t => t.Users, "users")
                .Field(t => t.Applications, "applications");

            mapper.Entity<User>()
                .Field(t => t.Id, "id").Id(t => t.Id, autoId: true)
                .Field(u => u.Username, "username")
                .Field(u => u.OrganisationId, "organisation_id")
                .Field(u => u.OrganisationRoles, "organisation_roles")
                .Field(u => u.PasswordHash, "password_hash")
                .Field(u => u.PasswordSalt, "password_salt")
                .Field(u => u.FirstName, "last_name")
                .Field(u => u.LastName, "last_name")
                .Field(u => u.TeamsPermissions, "team_roles");

            mapper.Entity<TeamPermission>()
                .Field(utr => utr.TeamCode, "team_id")
                .Field(utr => utr.IsOwner, "is_owner")
                .Field(utr => utr.IsReader, "is_reader")
                .Field(utr => utr.IsWriter, "is_writer");

            mapper.Entity<BranchActionMap>()
                .Field(bm => bm.Name, "name")
                .Field(bm => bm.BranchPattern, "branch_pattern")
                .Field(bm => bm.Increment, "increment")
                .Field(bm => bm.BumpMajorText, "bump_major_text")
                .Field(bm => bm.InheritedFrom, "inherited_from")
                .Field(bm => bm.AllowBumpMajor, "allow_bump_major");


            return services;
        }
    }
}