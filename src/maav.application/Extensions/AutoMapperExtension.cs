using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace MAAV.Application.Extensions
{
    public static class AutoMapperExtension
    {
        private static IMapper CurrentMapper { get; set; }

        public static IServiceCollection AddMapping(this IServiceCollection services)
        {
            var configurationMapper = new MapperConfiguration(cfg =>
            {
                cfg
                    .CreateMap<DataContracts.Organisation, Domain.Entities.Organisation>()
                    .ForMember(o => o.Id, opt => opt.MapFrom(s => s.Id))
                    .ForMember(o => o.Name, opt => opt.MapFrom(s => s.Name))
                    .ForMember(o => o.Teams, opt => opt.Ignore());

                cfg
                    .CreateMap<DataContracts.BranchActionMap, Domain.Entities.BranchActionMap>()
                    .ForMember(bm => bm.Name, opt => opt.MapFrom(sbm => sbm.Name))
                    .ForMember(bm => bm.BranchPattern, opt => opt.MapFrom(sbm => sbm.BranchPattern))
                    .ForMember(bm => bm.Increment, opt => opt.MapFrom(sbm => sbm.Increment))
                    .ForMember(bm => bm.InheritedFrom, opt => opt.MapFrom(sbm => sbm.InheritedFrom))
                    .ForMember(bm => bm.BumpMajorText, opt => opt.MapFrom(sbm => sbm.BumpMajorText))
                    .ForMember(bm => bm.AllowBumpMajor, opt => opt.MapFrom(sbm => sbm.AllowBumpMajor));

                cfg
                    .CreateMap<DataContracts.Team, Domain.Entities.Team>()
                    .ForMember(t => t.TeamCode, opt => opt.MapFrom(ts => ts.Id))
                    .ForMember(t => t.CreatedAt, opt => opt.MapFrom(ts => ts.CreatedAt))
                    .ForMember(t => t.Applications, opt => opt.MapFrom(ts => ts.Applications))
                    .ForMember(t => t.Users, opt => opt.MapFrom(ts => ts.Users))
                    .ForMember(t => t.Name, opt => opt.MapFrom(ts => ts.Name));

                cfg
                    .CreateMap<DataContracts.User, Domain.Entities.User>()
                    .ForMember(u => u.Username, opt => opt.MapFrom(du => du.Username))
                    .ForMember(u => u.FirstName, opt => opt.MapFrom(du => du.FirstName))
                    .ForMember(u => u.LastName, opt => opt.MapFrom(du => du.LastName))
                    .ForMember(u => u.CreatedAt, opt => opt.MapFrom(du => du.CreatedAt))
                    .ForMember(u => u.OrganisationRoles, opt => opt.MapFrom(du => du.Roles))
                    .ForMember(u => u.TeamsPermissions, opt => opt.MapFrom(du => du.TeamsPermissions))
                    .ForMember(u => u.TeamsPermissions, opt => opt.MapFrom(du => du.TeamsPermissions));

                cfg.CreateMap<DataContracts.IncrementMode, Domain.Entities.IncrementMode>();

                cfg
                    .CreateMap<Domain.Entities.Organisation, DataContracts.Organisation>()
                    .ForMember(o => o.Id, opt => opt.MapFrom(s => s.Id))
                    .ForMember(o => o.Name, opt => opt.MapFrom(s => s.Name));

                cfg
                    .CreateMap<Domain.Entities.BranchActionMap, DataContracts.BranchActionMap>()

                    .ForMember(bm => bm.Name, opt => opt.MapFrom(sbm => sbm.Name))
                    .ForMember(bm => bm.BranchPattern, opt => opt.MapFrom(sbm => sbm.BranchPattern))
                    .ForMember(bm => bm.Increment, opt => opt.MapFrom(sbm => sbm.Increment))
                    .ForMember(bm => bm.InheritedFrom, opt => opt.MapFrom(sbm => sbm.InheritedFrom))
                    .ForMember(bm => bm.BumpMajorText, opt => opt.MapFrom(sbm => sbm.BumpMajorText))
                    .ForMember(bm => bm.AllowBumpMajor, opt => opt.MapFrom(sbm => sbm.AllowBumpMajor));

                cfg
                    .CreateMap<Domain.Entities.Team, DataContracts.Team>()
                    .ForMember(t => t.Id, opt => opt.MapFrom(ts => ts.TeamCode))
                    .ForMember(t => t.CreatedAt, opt => opt.MapFrom(ts => ts.CreatedAt))
                    .ForMember(t => t.Users, opt => opt.MapFrom(ts => ts.Users))
                    .ForMember(t => t.Applications, opt => opt.MapFrom(ts => ts.Applications))
                    .ForMember(t => t.Name, opt => opt.MapFrom(ts => ts.Name));

                cfg
                    .CreateMap<Domain.Entities.User, DataContracts.User>()
                    .ForMember(u => u.Username, opt => opt.MapFrom(du => du.Username))
                    .ForMember(u => u.FirstName, opt => opt.MapFrom(du => du.FirstName))
                    .ForMember(u => u.LastName, opt => opt.MapFrom(du => du.LastName))
                    .ForMember(u => u.CreatedAt, opt => opt.MapFrom(u => u.CreatedAt))
                    .ForMember(u => u.Roles, opt => opt.MapFrom(du => du.OrganisationRoles))
                    .ForMember(u => u.TeamsPermissions, opt => opt.MapFrom(du => du.TeamsPermissions));

                cfg
                .CreateMap<Domain.Entities.TeamPermission, DataContracts.TeamPermission>()
                .ForMember(u => u.TeamId, opt => opt.MapFrom(du => du.TeamCode));

                cfg
                .CreateMap<DataContracts.TeamPermission, Domain.Entities.TeamPermission>()
                .ForMember(u => u.TeamCode, opt => opt.MapFrom(du => du.TeamId));

                cfg
                    .CreateMap<Domain.Entities.Application, DataContracts.Application>()
                    .ForMember(a => a.Id, opt => opt.MapFrom(ae => ae.Id))
                    .ForMember(a => a.TeamId, opt => opt.MapFrom(ae => ae.TeamCode))
                    .ForMember(a => a.Name, opt => opt.MapFrom(ae => ae.Name))
                    .ForMember(a => a.CreatedAt, opt => opt.MapFrom(ae => ae.CreatedAt))
                    .ForMember(a => a.KeyBranches, opt => opt.MapFrom(ae => ae.KeyBranches))
                    .ForMember(a => a.Branches, opt => opt.MapFrom(ae => ae.Branches))
                    .ForMember(a => a.WebHookEnabled, opt => opt.MapFrom(ae => ae.WebHookEnabled))
                    .ForMember(a => a.GithubSecretKey, opt => opt.MapFrom(ae => ae.GithubSecretKey))
                    .ForMember(a => a.InitialVersion, opt => opt.MapFrom(ae => ae.InitialVersion));

                cfg
                    .CreateMap<DataContracts.Application, Domain.Entities.Application>()
                    .ForMember(a => a.Id, opt => opt.MapFrom(ae => ae.Id))
                    .ForMember(a => a.TeamCode, opt => opt.MapFrom(ae => ae.TeamId))
                    .ForMember(a => a.Name, opt => opt.MapFrom(ae => ae.Name))
                    .ForMember(a => a.CreatedAt, opt => opt.MapFrom(ae => ae.CreatedAt))
                    .ForMember(a => a.OrganisationId, opt => opt.Ignore())
                    .ForMember(a => a.TeamCode, opt => opt.Ignore())
                    .ForMember(a => a.WebHookEnabled, opt => opt.MapFrom(ae => ae.WebHookEnabled))
                    .ForMember(a => a.KeyBranches, opt => opt.MapFrom(au => au.KeyBranches))
                    .ForMember(a => a.GithubSecretKey, opt => opt.MapFrom(au => au.GithubSecretKey))
                    .ForMember(a => a.Branches, opt => opt.MapFrom(au => au.Branches));


                cfg.CreateMap<Domain.Entities.TeamUser, DataContracts.TeamUser>();
                cfg.CreateMap<DataContracts.TeamUser, Domain.Entities.TeamUser>();

                cfg.CreateMap<Domain.Entities.GithubEventResult, DataContracts.GitHub.GithubEventResult>();
                cfg.CreateMap<DataContracts.GitHub.GithubEventResult, Domain.Entities.GithubEventResult>();

                cfg.CreateMap<Domain.Entities.BranchActionRequest, DataContracts.BranchActionRequest>();
                cfg.CreateMap<DataContracts.BranchActionRequest, Domain.Entities.BranchActionRequest>();

                cfg.CreateMap<Domain.Entities.KeyBranchVersion, DataContracts.KeyBranchVersion>();
                cfg.CreateMap<DataContracts.KeyBranchVersion, Domain.Entities.KeyBranchVersion>();

                cfg.CreateMap<Domain.Entities.KeyBranchVersionHistory, DataContracts.KeyBranchVersionHistory>();
                cfg.CreateMap<DataContracts.KeyBranchVersionHistory, Domain.Entities.KeyBranchVersionHistory>();

                cfg.CreateMap<Domain.Entities.TeamApplication, DataContracts.TeamApplication>();
                cfg.CreateMap<DataContracts.TeamApplication, Domain.Entities.TeamApplication>();

                cfg.CreateMap<Domain.Entities.SemanticVersion, DataContracts.SemanticVersion>();
                cfg.CreateMap<DataContracts.SemanticVersion, Domain.Entities.SemanticVersion>();

                cfg.CreateMap<Domain.Entities.KeyBranch, DataContracts.KeyBranch>();
                cfg.CreateMap<DataContracts.KeyBranch, Domain.Entities.KeyBranch>();

                cfg.CreateMap<Domain.Entities.IncrementMode, DataContracts.IncrementMode>();
            });

            CurrentMapper = configurationMapper.CreateMapper();

            services?.AddSingleton<IMapper>(CurrentMapper);
            return services;
        }

        public static DataContracts.Organisation ToContract(this Domain.Entities.Organisation source)
            => ToContract<Domain.Entities.Organisation, DataContracts.Organisation>(CurrentMapper, source);

        public static Domain.Entities.Organisation ToEntity(this DataContracts.Organisation source)
            => ToEntity<DataContracts.Organisation, Domain.Entities.Organisation>(CurrentMapper, source);

        public static DataContracts.Team ToContract(this Domain.Entities.Team source)
            => ToContract<Domain.Entities.Team, DataContracts.Team>(CurrentMapper, source);

        public static Domain.Entities.Team ToEntity(this DataContracts.Team source)
            => ToEntity<DataContracts.Team, Domain.Entities.Team>(CurrentMapper, source);

        
        public static DataContracts.TeamPermission ToContract(this Domain.Entities.TeamPermission source)
            => ToContract<Domain.Entities.TeamPermission, DataContracts.TeamPermission>(CurrentMapper, source);

        public static Domain.Entities.TeamPermission ToEntity(this DataContracts.TeamPermission source)
            => ToEntity<DataContracts.TeamPermission, Domain.Entities.TeamPermission>(CurrentMapper, source);

        public static DataContracts.Application ToContract(this Domain.Entities.Application source) 
            => ToContract<Domain.Entities.Application, DataContracts.Application>(CurrentMapper, source);

        public static Domain.Entities.Application ToEntity(this DataContracts.Application source)
            => ToEntity<DataContracts.Application, Domain.Entities.Application>(CurrentMapper, source);

        public static DataContracts.SemanticVersion ToContract(this Domain.Entities.SemanticVersion source)
            => ToContract<Domain.Entities.SemanticVersion, DataContracts.SemanticVersion>(CurrentMapper, source);

        public static Domain.Entities.SemanticVersion ToEntity(this DataContracts.SemanticVersion source)
            => ToEntity<DataContracts.SemanticVersion, Domain.Entities.SemanticVersion>(CurrentMapper, source);


        public static DataContracts.BranchActionRequest ToContract(this Domain.Entities.BranchActionRequest source)
            => ToContract<Domain.Entities.BranchActionRequest, DataContracts.BranchActionRequest>(CurrentMapper, source);

        public static Domain.Entities.BranchActionRequest ToEntity(this DataContracts.BranchActionRequest source)
            => ToEntity<DataContracts.BranchActionRequest, Domain.Entities.BranchActionRequest>(CurrentMapper, source);

        public static DataContracts.KeyBranchVersion ToContract(this Domain.Entities.KeyBranchVersion source)
            => ToContract<Domain.Entities.KeyBranchVersion, DataContracts.KeyBranchVersion>(CurrentMapper, source);

        public static Domain.Entities.KeyBranchVersion ToEntity(this DataContracts.KeyBranchVersion source)
            => ToEntity<DataContracts.KeyBranchVersion, Domain.Entities.KeyBranchVersion>(CurrentMapper, source);

        public static DataContracts.KeyBranchVersionHistory ToContract(this Domain.Entities.KeyBranchVersionHistory source)
            => ToContract<Domain.Entities.KeyBranchVersionHistory, DataContracts.KeyBranchVersionHistory>(CurrentMapper, source);

        public static Domain.Entities.KeyBranchVersionHistory ToEntity(this DataContracts.KeyBranchVersionHistory source)
            => ToEntity<DataContracts.KeyBranchVersionHistory, Domain.Entities.KeyBranchVersionHistory>(CurrentMapper, source);

        public static DataContracts.GitHub.GithubEventResult ToContract(this Domain.Entities.GithubEventResult source)
            => ToContract<Domain.Entities.GithubEventResult, DataContracts.GitHub.GithubEventResult>(CurrentMapper, source);

        public static Domain.Entities.GithubEventResult ToEntity(this DataContracts.GitHub.GithubEventResult source)
            => ToEntity<DataContracts.GitHub.GithubEventResult, Domain.Entities.GithubEventResult>(CurrentMapper, source);

        public static IEnumerable<DataContracts.Team> ToContract(this IEnumerable<Domain.Entities.Team> source)
            => source.Select(t => t.ToContract());

        public static IEnumerable<DataContracts.User> ToContract(this IEnumerable<Domain.Entities.User> source)
            => source.Select(u => u.ToContract());

        public static IEnumerable<DataContracts.Application> ToContract(this IEnumerable<Domain.Entities.Application> source)
            => source.Select(t => t.ToContract());
        
        public static IEnumerable<Domain.Entities.TeamPermission> ToEntity(this IEnumerable<DataContracts.TeamPermission> source, Func<Domain.Entities.TeamPermission, Domain.Entities.TeamPermission> action)
            => source.Select(utr => action.Invoke(utr.ToEntity()));

        public static DataContracts.User ToContract(this Domain.Entities.User source)
            => ToContract<Domain.Entities.User, DataContracts.User>(CurrentMapper, source);

        public static Domain.Entities.User ToEntity(this DataContracts.User source)
            => ToEntity<DataContracts.User, Domain.Entities.User>(CurrentMapper, source);

        public static TResult ToContract<TEntity, TResult>(IMapper mapper, TEntity entity) where TEntity : Domain.Entities.IEntity
            => mapper.Map<TEntity, TResult>(entity);
        public static TResult ToEntity<TContract, TResult>(IMapper mapper, TContract contract) where TResult : Domain.Entities.IEntity
            => mapper.Map<TContract, TResult>(contract);
    }
}