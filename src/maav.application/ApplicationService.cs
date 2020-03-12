using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MAAV.Application.Exceptions;
using MAAV.Application.Extensions;
using MAAV.DataContracts.Extensions;
using MAAV.Domain.Repositories;

namespace MAAV.Application
{
    public class ApplicationService : IApplicationService, IVersionService
    {
        private readonly IApplicationRepository repository;
        private readonly ITeamRepository teamRepository;
        private readonly IOrganisationRepository organisationRepository;
        public ApplicationService(IApplicationRepository applicationRepository, IOrganisationRepository organisationRepository, ITeamRepository teamRepository)
        {
            this.repository = applicationRepository;
            this.teamRepository = teamRepository;
            this.organisationRepository = organisationRepository;
        }

        public async Task DeleteByNameAsync(string organisationName, string teamName, string applicationName)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Name == organisationName))
            {
                throw new ArgumentException($"The organisation {organisationName} not exists");
            }

            if (!await this.teamRepository.ExistsByAsync(t => t.Name == teamName && t.OrganisationName == organisationName))
            {
                throw new ArgumentException($"The team {teamName} from organisation {organisationName} not exists");
            }

            await this.repository.DeleteAsync(app => app.Name == applicationName && app.TeamName == teamName && app.OrganisationName == organisationName);
        }

        public async Task<DataContracts.Application> GetByNameAsync(string organisationName, string teamName, string applicationName)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Name == organisationName))
            {
                throw new ArgumentException($"The organisation {organisationName} not exists");
            }

            if (await this.teamRepository.ExistsByAsync(t => t.Name == teamName && t.OrganisationName == organisationName))
            {
                throw new ArgumentException($"The team {teamName} from organisation {organisationName} not exists");
            }

            var application = await repository.GetByAsync(app => app.Name == applicationName && app.TeamName == teamName && app.OrganisationName == organisationName);

            return application?.ToContract();
        }

        public async Task<DataContracts.Application> AddAsync(string organisationName, string teamName, DataContracts.Application application)
        {
            var orgEntity = await organisationRepository.GetByAsync(o => o.Name == organisationName);
            if (orgEntity == null)
            {
                throw new ArgumentException($"The organisation {organisationName} not exists");
            }
            
            var teamEntity = await this.teamRepository.GetByAsync(t => t.Name == teamName && t.OrganisationName == organisationName);
            if (teamEntity == null)
            {
                throw new ArgumentException($"The team {teamName} from organisation {organisationName} not exists");
            }

            if (await this.repository.ExistsByAsync(app => app.TeamName == teamName && app.OrganisationName == organisationName && app.Name == application.Name))
            {
                throw new NameAlreadyUsedException($"The application {application.Name} team {teamName} from organisation {organisationName} not exists");
            }

            var appEntity = application.ToEntity();
            appEntity.TeamName = teamName;
            appEntity.OrganisationName = organisationName;
            appEntity.ScheMap = appEntity.ScheMap ?? teamEntity.ScheMap ?? orgEntity.ScheMap;

            if  (appEntity.ScheMap?.Branches?.Length < 1)
            {
                throw new ArgumentException("Create a BranchMap.");
            }

            var invalidBranchSetting = 
                application.ScheMap.Branches.Any(b => application.ScheMap.Branches.Count(x => x.Name == b.Name) > 1) ||
                application.ScheMap.Branches.Any(b => application.ScheMap.Branches.Count(x => x.BranchPattern == b.BranchPattern) > 1);

            if  (invalidBranchSetting)
            {
                throw new ArgumentException("Invalid branch map settings.");
            }

            if (application.ScheMap.Branches.Count(bm => bm.IsKeyBranch) < 1)
            {
                throw new ArgumentException("Must create one key branch unless.");
            }

            if (application.ScheMap.Branches.Count(bm => !bm.IsKeyBranch) < 1)
            {
                throw new ArgumentException("Must create one normal branch unless.");
            }

            var version = new DataContracts.Version(application.InitialVersion);
            appEntity.InitialVersion = version.ToEntity();

            appEntity.BranchVersions = application
                .ScheMap
                .Branches
                .Where(b => b.IsKeyBranch)
                .Select(b => new Domain.Entities.BranchVersion 
                { 
                    BranchMapName = b.Name, 
                    Version = new Domain.Entities.Version
                    {
                        Major = version.Major,
                        Minor = version.Minor,
                        Patch = version.Patch,
                        Label = b.Label.FormatLabel()
                    }  
                })
                .ToList();

            return (await repository.AddAsync(appEntity)).ToContract();
        }

        public async Task<DataContracts.Application> UpdateAsync(string organisationName, string teamName, DataContracts.Application application)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Name == organisationName))
            {
                throw new ArgumentException($"The organisation {organisationName} not exists");
            }

            if (!await this.teamRepository.ExistsByAsync(t => t.Name == teamName && t.OrganisationName == organisationName))
            {
                throw new ArgumentException($"The team {teamName} from organisation {organisationName} not exists");
            }

            var appLocated = await this.repository.GetByAsync(app => app.TeamName == teamName && app.OrganisationName == organisationName && app.Name == application.Name);
            if (appLocated == null)
            {
                return null;
            }

            var appEntity = application.ToEntity();
            appEntity.OrganisationName = organisationName;
            appEntity.TeamName = teamName;
            appEntity.BranchVersions = appLocated.BranchVersions;
            appEntity.Id = appLocated.Id;

            if  (appEntity.ScheMap?.Branches?.Length < 1)
            {
                throw new ArgumentException("Create a BranchMap.");
            }

            var invalidBranchSetting = 
                application.ScheMap.Branches.Any(b => application.ScheMap.Branches.Count(x => x.Name == b.Name) > 1) ||
                application.ScheMap.Branches.Any(b => application.ScheMap.Branches.Count(x => x.BranchPattern == b.BranchPattern) > 1);

            if  (invalidBranchSetting)
            {
                throw new ArgumentException("Invalid branch map settings.");
            }

            if (application.ScheMap.Branches.Count(bm => bm.IsKeyBranch) < 1)
            {
                throw new ArgumentException("Must create one key branch unless.");
            }

            if (application.ScheMap.Branches.Count(bm => !bm.IsKeyBranch) < 1)
            {
                throw new ArgumentException("Must create one normal branch unless.");
            }

            var version = new DataContracts.Version(application.InitialVersion);
            appEntity.InitialVersion = version.ToEntity();
            
            var branchVersions = application
                .ScheMap
                .Branches
                .Where(b => b.IsKeyBranch)
                .Select(b => new Domain.Entities.BranchVersion 
                { 
                    BranchMapName = b.Name, 
                    Version = new Domain.Entities.Version
                    {
                        Major = appEntity.InitialVersion.Major,
                        Minor = appEntity.InitialVersion.Minor,
                        Patch = appEntity.InitialVersion.Patch,
                        Label = b.Label.FormatLabel()
                    }
                })
                .ToList();
             
            appEntity.BranchVersions.Where(bvx => !branchVersions.Any(bv => bv.BranchMapName == bvx.BranchMapName)).ToList().ForEach(bv => 
            {
                bv.IsEnabled = false;
            });

            appEntity.BranchVersions.Where(bvx => branchVersions.Any(bv => bv.BranchMapName == bvx.BranchMapName)).ToList().ForEach(bv => 
            {
                var bvUpdate = branchVersions.FirstOrDefault(x => x.BranchMapName == bv.BranchMapName);
                bv.IsEnabled = true;
                bv.Version = bvUpdate?.Version;
            });

            branchVersions.Where(bvx => !appLocated.BranchVersions.Any(bv => bv.BranchMapName == bvx.BranchMapName)).ToList().ForEach(bv => 
            {
                appEntity.BranchVersions.Add(bv);
            });

            return (await repository.UpdateAsync(appEntity)).ToContract();
        }

        public async Task<List<DataContracts.Application>> LoadAllFromTeamAsync(string organisationName, string teamName)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Name == organisationName))
            {
                return null;
            }

            if (!await this.teamRepository.ExistsByAsync(t => t.Name == teamName && t.OrganisationName == organisationName))
            {
                return null;
            }

            var applications = await repository.LoadByAsync(app => app.TeamName == teamName && app.OrganisationName == organisationName);

            return applications?.ToContract().ToList();
        }

        public async Task<string> GetVersionFromSourceBranchAsync(string organisationName, string teamName, string appName, string source, object data)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Name == organisationName))
            {
                throw new ArgumentException($"The organisation {organisationName} not exists");
            }

            if (!await this.teamRepository.ExistsByAsync(t => t.Name == teamName && t.OrganisationName == organisationName))
            {
                throw new ArgumentException($"The team {teamName} from organisation {organisationName} not exists");
            }

            var appLocated = await this.repository.GetByAsync(app => app.TeamName == teamName && app.OrganisationName == organisationName && app.Name == appName);
            if (appLocated == null)
            {
                throw new ArgumentException($"The {appName} application from team {teamName} at organisation {organisationName} not exists");
            }

            var branchMap = appLocated.ScheMap.Branches.Where(x => new Regex(x.BranchPattern).IsMatch(source)).FirstOrDefault();
            if (branchMap == null)
            {
                throw new ArgumentException("Not could find source branch map for this parameter");
            }

            Domain.Entities.Version version = null;

            if (!string.IsNullOrWhiteSpace(branchMap.InheritedFrom))
            {
                version = appLocated.BranchVersions.FirstOrDefault(bv => bv.BranchMapName == branchMap.InheritedFrom).Version;
            }
            else 
            {
                version = appLocated.BranchVersions.FirstOrDefault(bv => bv.BranchMapName == branchMap.Name).Version;
            }

            if (version == null)
            {   
                throw new ArgumentException($"the {source} branch is invalid configuration registered.");
            }

            switch(branchMap.Increment)
            {
                case Domain.Entities.IncrementMode.Major : 
                    version.Major += 1;
                    break;
                case Domain.Entities.IncrementMode.Minor : 
                    version.Minor += 1;
                    break;
                case Domain.Entities.IncrementMode.Patch : 
                    version.Patch += 1;
                    break;
            }

            return $"{version.FormatVersion(appLocated.ScheMap.Format)}{branchMap.SuffixFormat?.Replace("{Label}", version.Label)}";

        }

        public async Task<string> GetVersionFromSourceAndTagetBranchAsync(string organisationName, string teamName, string appName,string source, string target, object data)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Name == organisationName))
            {
                throw new ArgumentException($"The organisation {organisationName} not exists");
            }

            if (!await this.teamRepository.ExistsByAsync(t => t.Name == teamName && t.OrganisationName == organisationName))
            {
                throw new ArgumentException($"The team {teamName} from organisation {organisationName} not exists");
            }

            var appLocated = await this.repository.GetByAsync(app => app.TeamName == teamName && app.OrganisationName == organisationName && app.Name == appName);
            if (appLocated == null)
            {
                throw new ArgumentException($"The {appName} application from team {teamName} at organisation {organisationName} not exists");
            }

            var sourcebranchMap = appLocated.ScheMap.Branches.Where(x => new Regex(x.BranchPattern).IsMatch(source)).FirstOrDefault();
            var targetbranchMap = appLocated.ScheMap.Branches.Where(x => new Regex(x.BranchPattern).IsMatch(target)).FirstOrDefault();

            if (sourcebranchMap == null || targetbranchMap == null)
            {
                throw new ArgumentException("Not could find source|target branch map for theses parameters");
            }

            Domain.Entities.Version version = null;

            if (!string.IsNullOrWhiteSpace(targetbranchMap.InheritedFrom))
            {
                version = appLocated.BranchVersions.FirstOrDefault(bv => bv.BranchMapName == targetbranchMap.InheritedFrom).Version;
            }
            else 
            {
                version = appLocated.BranchVersions.FirstOrDefault(bv => bv.BranchMapName == targetbranchMap.Name).Version;
            }

            if (version == null)
            {
                throw new ArgumentException($"the {target} branch is invalid configuration registered.");
            }

            switch(sourcebranchMap.Increment)
            {
                case Domain.Entities.IncrementMode.Major : 
                    version.Major += 1;
                    break;
                case Domain.Entities.IncrementMode.Minor : 
                    version.Minor += 1;
                    break;
                case Domain.Entities.IncrementMode.Patch : 
                    version.Patch += 1;
                    break;
                case Domain.Entities.IncrementMode.None:
                    if (sourcebranchMap.IsKeyBranch && targetbranchMap.IsKeyBranch)
                    {
                        var targetbranchVersion =  appLocated.BranchVersions.First(bv => bv.BranchMapName == targetbranchMap.Name);
                        var sourcebranchVersion = appLocated.BranchVersions.First(bv => bv.BranchMapName == sourcebranchMap.Name);
                        targetbranchVersion.Version = sourcebranchVersion.Version;
                        version  = targetbranchVersion.Version;
                    }

                    break;
            }
            await this.repository.UpdateAsync(appLocated);

            return $"{version.FormatVersion(appLocated.ScheMap.Format)}{targetbranchMap.SuffixFormat?.Replace("{Label}", version.Label)}";
        }
    }
}