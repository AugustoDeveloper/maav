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
                    .CreateMap<DataContracts.ScheMapVersion, Domain.Entities.ScheMapVersion>()
                    .ForMember(sm => sm.Format, opt => opt.MapFrom(ssm => ssm.Format))
                    .ForMember(sm => sm.Branches, opt => opt.MapFrom(ssm => ssm.Branches));

                cfg
                    .CreateMap<DataContracts.BranchMap, Domain.Entities.BranchMap>()
                    .ForMember(bm => bm.Name, opt => opt.MapFrom(sbm => sbm.Name))
                    .ForMember(bm => bm.BranchPattern, opt => opt.MapFrom(sbm => sbm.BranchPattern))
                    .ForMember(bm => bm.Increment, opt => opt.MapFrom(sbm => sbm.Increment))
                    .ForMember(bm => bm.Label, opt => opt.MapFrom(sbm => sbm.Label))
                    .ForMember(bm => bm.SuffixFormat, opt => opt.MapFrom(sbm => sbm.SuffixFormat))
                    .ForMember(bm => bm.AllowBumpMajor, opt => opt.MapFrom(sbm => sbm.AllowBumpMajor));

                cfg
                    .CreateMap<DataContracts.Team, Domain.Entities.Team>()
                    .ForMember(t => t.Id, opt => opt.MapFrom(ts => ts.Id))
                    .ForMember(t => t.CreatedAt, opt => opt.MapFrom(ts => ts.CreatedAt))
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
                cfg.CreateMap<DataContracts.LabelDescription, Domain.Entities.LabelDescription>();

                cfg
                    .CreateMap<Domain.Entities.Organisation, DataContracts.Organisation>()
                    .ForMember(o => o.Id, opt => opt.MapFrom(s => s.Id))
                    .ForMember(o => o.Name, opt => opt.MapFrom(s => s.Name));

                cfg.
                    CreateMap<Domain.Entities.ScheMapVersion, DataContracts.ScheMapVersion>()
                    .ForMember(sm => sm.Format, opt => opt.MapFrom(ssm => ssm.Format))
                    .ForMember(sm => sm.Branches, opt => opt.MapFrom(ssm => ssm.Branches));

                cfg
                    .CreateMap<Domain.Entities.BranchMap, DataContracts.BranchMap>()
                    .ForMember(bm => bm.Name, opt => opt.MapFrom(sbm => sbm.Name))
                    .ForMember(bm => bm.BranchPattern, opt => opt.MapFrom(sbm => sbm.BranchPattern))
                    .ForMember(bm => bm.Increment, opt => opt.MapFrom(sbm => sbm.Increment))
                    .ForMember(bm => bm.Label, opt => opt.MapFrom(sbm => sbm.Label))
                    .ForMember(bm => bm.SuffixFormat, opt => opt.MapFrom(sbm => sbm.SuffixFormat))
                    .ForMember(bm => bm.AllowBumpMajor, opt => opt.MapFrom(sbm => sbm.AllowBumpMajor));

                cfg
                    .CreateMap<Domain.Entities.Team, DataContracts.Team>()
                    .ForMember(t => t.Id, opt => opt.MapFrom(ts => ts.Id))
                    .ForMember(t => t.CreatedAt, opt => opt.MapFrom(ts => ts.CreatedAt))
                    .ForMember(t => t.Users, opt => opt.MapFrom(ts => ts.Users))
                    .ForMember(t => t.Name, opt => opt.MapFrom(ts => ts.Name));

                cfg
                    .CreateMap<Domain.Entities.User, DataContracts.User>()
                    .ForMember(u => u.Username, opt => opt.MapFrom(du => du.Username))
                    .ForMember(u => u.FirstName, opt => opt.MapFrom(du => du.FirstName))
                    .ForMember(u => u.LastName, opt => opt.MapFrom(du => du.LastName))
                    .ForMember(u => u.CreatedAt, opt => opt.MapFrom(u => u.CreatedAt))
                    .ForMember(u => u.Roles, opt => opt.MapFrom(du => du.OrganisationRoles))
                    .ForMember(u => u.TeamsPermissions, opt => opt.MapFrom(du => du.TeamsPermissions));

                cfg.CreateMap<Domain.Entities.TeamPermission, DataContracts.TeamPermission>();
                cfg.CreateMap<DataContracts.TeamPermission, Domain.Entities.TeamPermission>();

                cfg
                    .CreateMap<Domain.Entities.Application, DataContracts.Application>()
                    .ForMember(a => a.Name, opt => opt.MapFrom(ae => ae.Name))
                    .ForMember(a => a.ScheMap, opt => opt.MapFrom(ae => ae.ScheMap))
                    .ForMember(a => a.InitialVersion, opt => opt.Ignore());
                    
                cfg
                    .CreateMap<DataContracts.Application, Domain.Entities.Application>()
                    .ForMember(a => a.Name, opt => opt.MapFrom(ae => ae.Name))
                    .ForMember(a => a.ScheMap, opt => opt.MapFrom(ae => ae.ScheMap))
                    .ForMember(a => a.OrganisationId, opt => opt.Ignore())
                    .ForMember(a => a.TeamName, opt => opt.Ignore())
                    .ForMember(a => a.BranchVersions, opt => opt.MapFrom(au => au.BranchVersions))
                    .ForMember(a => a.InitialVersion, opt => opt.Ignore());


                cfg.CreateMap<Domain.Entities.TeamUser, DataContracts.TeamUser>();
                cfg.CreateMap<DataContracts.TeamUser, Domain.Entities.TeamUser>();

                cfg.CreateMap<Domain.Entities.Version, DataContracts.Version>();
                cfg.CreateMap<DataContracts.Version, Domain.Entities.Version>();

                cfg.CreateMap<Domain.Entities.BranchVersion, DataContracts.BranchVersion>();
                cfg.CreateMap<DataContracts.BranchVersion, Domain.Entities.BranchVersion>();

                cfg.CreateMap<Domain.Entities.IncrementMode, DataContracts.IncrementMode>();
                cfg.CreateMap<Domain.Entities.LabelDescription, DataContracts.LabelDescription>();
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
        {
            var appContract = ToContract<Domain.Entities.Application, DataContracts.Application>(CurrentMapper, source);
            appContract.InitialVersion = source.InitialVersion.FormatVersion(source.ScheMap.Format);
            return appContract;
        }

        public static Domain.Entities.Application ToEntity(this DataContracts.Application source)
            => ToEntity<DataContracts.Application, Domain.Entities.Application>(CurrentMapper, source);

        public static DataContracts.Version ToContract(this Domain.Entities.Version source)
            => ToContract<Domain.Entities.Version, DataContracts.Version>(CurrentMapper, source);

        public static Domain.Entities.Version ToEntity(this DataContracts.Version source)
            => ToEntity<DataContracts.Version, Domain.Entities.Version>(CurrentMapper, source);

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