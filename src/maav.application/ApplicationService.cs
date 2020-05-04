using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MAAV.Application.Exceptions;
using MAAV.Application.Extensions;
using MAAV.DataContracts;
using MAAV.DataContracts.Extensions;
using MAAV.Domain.Repositories;

namespace MAAV.Application
{
    public class ApplicationService : IApplicationService, IVersionService
    {
        private readonly IApplicationRepository repository;
        private readonly ITeamRepository teamRepository;
        private readonly IOrganisationRepository organisationRepository;
        private readonly IKeyBranchVersionHistoryRepository versionHistoryRepository;
        public ApplicationService(IApplicationRepository applicationRepository, IOrganisationRepository organisationRepository, ITeamRepository teamRepository, IKeyBranchVersionHistoryRepository versionHistoryRepository)
        {
            this.repository = applicationRepository;
            this.teamRepository = teamRepository;
            this.organisationRepository = organisationRepository;
            this.versionHistoryRepository = versionHistoryRepository;
        }

        public async Task DeleteByIdAsync(string organisationId, string teamId, string appId)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Id == organisationId))
            {
                throw new ArgumentException($"The organisation {organisationId} not exists");
            }

            var teamEntity = await this.teamRepository.GetByAsync(t => t.TeamCode == teamId && t.OrganisationId == organisationId);
            if (teamEntity == null)
            {
                throw new ArgumentException($"The team {teamId} from organisation {organisationId} not exists");
            }

            teamEntity.Applications.RemoveAll(a => a.Id == appId);
            await this.versionHistoryRepository.DeleteAsync(h => h.OrganisationId == organisationId && h.TeamId == teamId && h.ApplicationId == appId);
            await this.teamRepository.UpdateAsync(teamEntity);
            await this.repository.DeleteAsync(app => appId.Equals(app.Id) && app.TeamCode == teamId && app.OrganisationId == organisationId);
        }

        public async Task<DataContracts.Application> GetByIdAsync(string organisationId, string teamId, string appId)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Id == organisationId))
            {
                throw new ArgumentException($"The organisation {organisationId} not exists");
            }

            if (!await this.teamRepository.ExistsByAsync(t => t.TeamCode == teamId && t.OrganisationId == organisationId))
            {
                throw new ArgumentException($"The team {teamId} from organisation {organisationId} not exists");
            }

            var application = await repository.GetByAsync(app => appId.Equals(app.Id) && app.TeamCode == teamId && app.OrganisationId == organisationId);

            return application?.ToContract();
        }

        public async Task<DataContracts.Application> AddAsync(string organisationId, string teamId, DataContracts.Application application)
        {
            var orgEntity = await organisationRepository.GetByAsync(o => o.Id == organisationId);
            if (orgEntity == null)
            {
                throw new ArgumentException($"The organisation {organisationId} not exists");
            }
            
            var teamEntity = await this.teamRepository.GetByAsync(t => t.TeamCode == teamId && t.OrganisationId == organisationId);
            if (teamEntity == null)
            {
                throw new ArgumentException($"The team {teamId} from organisation {organisationId} not exists");
            }

            var appEntity = application.ToEntity();
            appEntity.TeamCode = teamId;
            appEntity.OrganisationId = organisationId;

            var invalidBranchSetting = 
                application.Branches.Any(b => application.Branches.Count(x => x.Name == b.Name) > 1) ||
                application.Branches.Any(b => application.Branches.Count(x => x.BranchPattern == b.BranchPattern) > 1);

            if  (invalidBranchSetting)
            {
                throw new ArgumentException("Invalid branch map settings.");
            }

            if (!application.KeyBranches.Any())
            {
                throw new ArgumentException("Must create one key branch unless.");
            }

            if (!application.Branches.Any())
            {
                throw new ArgumentException("Must create one normal branch unless.");
            }

            appEntity.KeyBranchVersionings = appEntity.KeyBranches.Select(b => new Domain.Entities.KeyBranchVersioning
            {
                KeyBranchName = b.Name,
                UpdatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                CurrentVersion = appEntity.InitialVersion
            }).ToList();

            teamEntity.Applications.Add(new Domain.Entities.TeamApplication(appEntity));

            await this.teamRepository.UpdateAsync(teamEntity);

            var result = await repository.AddAsync(appEntity);

            appEntity.KeyBranches.ForEach(kb =>
            {
                var history = new Domain.Entities.KeyBranchVersionHistory
                {
                    ApplicationId = result.Id,
                    KeyBranchName = kb.Name,
                    OrganisationId = organisationId,
                    TeamId = teamId,
                    VersionHistory = new List<Domain.Entities.KeyBranchVersion>()
                };
                history.VersionHistory.Add(new Domain.Entities.KeyBranchVersion
                {
                    CreatedAt = DateTime.UtcNow,
                    FormatVersion = kb.FormatVersion,
                    Request = new Domain.Entities.BranchActionRequest
                    {
                        Message = "Initial Version",
                        From = "*",
                        To = "*",
                        BuildLabel = "",
                        PreReleaseLabel = "",
                        Commit = ""
                    },
                    Version = appEntity.InitialVersion
                });

                this.versionHistoryRepository.AddAsync(history).GetAwaiter().GetResult();
            });

            return result.ToContract();
        }

        public async Task<DataContracts.Application> UpdateAsync(string organisationId, string teamId, DataContracts.Application application)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Id == organisationId))
            {
                throw new ArgumentException($"The organisation {organisationId} not exists");
            }

            if (!await this.teamRepository.ExistsByAsync(t => t.TeamCode == teamId && t.OrganisationId == organisationId))
            {
                throw new ArgumentException($"The team {teamId} from organisation {organisationId} not exists");
            }

            var appLocated = await this.repository.GetByAsync(app => app.TeamCode == teamId && app.OrganisationId == organisationId && application.Id == app.Id);
            if (appLocated == null)
            {
                return null;
            }

            var appEntity = application.ToEntity();
            appEntity.OrganisationId = organisationId;
            appEntity.TeamCode = teamId;
            appEntity.KeyBranchVersionings = appLocated.KeyBranchVersionings;
            appEntity.GithubSecretKey =  string.IsNullOrWhiteSpace(appEntity.GithubSecretKey) ? appLocated.GithubSecretKey : appEntity.GithubSecretKey;

            var invalidBranchSetting =
                application.Branches.Any(b => application.Branches.Count(x => x.Name == b.Name) > 1) ||
                application.Branches.Any(b => application.Branches.Count(x => x.BranchPattern == b.BranchPattern) > 1);

            if (invalidBranchSetting)
            {
                throw new ArgumentException("Invalid branch map settings.");
            }

            if (!application.KeyBranches.Any())
            {
                throw new ArgumentException("Must create one key branch unless.");
            }

            if (!application.Branches.Any())
            {
                throw new ArgumentException("Must create one normal branch unless.");
            }

            appEntity.KeyBranchVersionings.RemoveAll(b => !appEntity.KeyBranches.Any(kb => kb.Name == b.KeyBranchName));

            appEntity.KeyBranchVersionings.AddRange(appEntity.KeyBranches.Where(b => !appEntity.KeyBranchVersionings.Any(kb => kb.KeyBranchName == b.Name)).Select(b => new Domain.Entities.KeyBranchVersioning
            {
                KeyBranchName = b.Name,
                UpdatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                CurrentVersion = appEntity.InitialVersion
            }).ToArray());

            appEntity.KeyBranches.ForEach(kb =>
            {
                if (!this.versionHistoryRepository.ExistsByAsync(h => h.OrganisationId == organisationId && h.TeamId == teamId && h.ApplicationId == appLocated.Id && h.KeyBranchName == kb.Name).GetAwaiter().GetResult())
                {
                    var history = new Domain.Entities.KeyBranchVersionHistory
                    {
                        ApplicationId = appLocated.Id,
                        KeyBranchName = kb.Name,
                        OrganisationId = organisationId,
                        TeamId = teamId,
                        VersionHistory = new List<Domain.Entities.KeyBranchVersion>()
                    };
                    history.VersionHistory.Add(new Domain.Entities.KeyBranchVersion
                    {
                        CreatedAt = DateTime.UtcNow,
                        FormatVersion = kb.FormatVersion,
                        Request = new Domain.Entities.BranchActionRequest
                        {
                            Message = "Initial Version",
                            From = "*",
                            To = "*",
                            BuildLabel = "",
                            PreReleaseLabel = "",
                            Commit = ""
                        },
                        Version = appEntity.InitialVersion
                    });

                    this.versionHistoryRepository.AddAsync(history).GetAwaiter().GetResult();
                }
            });

            var result = await repository.UpdateAsync(appEntity);
            return result.ToContract();
        }

        public async Task<List<DataContracts.Application>> LoadAllFromTeamAsync(string organisationId, string teamId)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Id == organisationId))
            {
                return null;
            }

            if (!await this.teamRepository.ExistsByAsync(t => t.TeamCode == teamId && t.OrganisationId == organisationId))
            {
                return null;
            }

            var applications = await repository.LoadByAsync(app => app.TeamCode == teamId && app.OrganisationId == organisationId);

            return applications?.ToContract().ToList();
        }

        public async Task<SemanticVersion> UpdateVersionOnBranches(string organisationId, string teamId, string appId, BranchActionRequest data)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Id == organisationId))
            {
                return null;
            }

            if (!await this.teamRepository.ExistsByAsync(t => t.TeamCode == teamId && t.OrganisationId == organisationId))
            {
                return null;
            }

            var appLocated = await this.repository.GetByAsync(app => app.TeamCode == teamId && app.OrganisationId == organisationId && app.Id == appId);
            if (appLocated == null)
            {
                return null;
            }

            var keyBranch = appLocated.KeyBranches.FirstOrDefault(b => Regex.IsMatch(data.To, b.BranchPattern));
            if (keyBranch == null)
            {
                return null;
            }

            var keyBranchVersion = appLocated.KeyBranchVersionings.FirstOrDefault(k => k.KeyBranchName == keyBranch.Name);
            if (keyBranchVersion == null)
            {
                return null;
            }

            var branch = appLocated.Branches.FirstOrDefault(b => Regex.IsMatch(data.From, b.BranchPattern));
            if (branch == null)
            {
                var fromKeyBranch = appLocated.KeyBranches.FirstOrDefault(b => Regex.IsMatch(data.From, b.BranchPattern));
                if (fromKeyBranch == null)
                {
                    return null;
                }

                var fromKeyBranchVersion = appLocated.KeyBranchVersionings.FirstOrDefault(kv => kv.KeyBranchName == fromKeyBranch.Name);
                if (fromKeyBranchVersion == null)
                {
                    return null;
                }

                keyBranchVersion.CurrentVersion.Major = fromKeyBranchVersion.CurrentVersion.Major;
                keyBranchVersion.CurrentVersion.Minor = fromKeyBranchVersion.CurrentVersion.Minor;
                keyBranchVersion.CurrentVersion.Patch = fromKeyBranchVersion.CurrentVersion.Patch;
                keyBranchVersion.CurrentVersion.Build = fromKeyBranchVersion.CurrentVersion.Build;
                keyBranchVersion.CurrentVersion.PreRelease = fromKeyBranchVersion.CurrentVersion.PreRelease;
            }
            else
            {
                var incrementMode = branch.AllowBumpMajor && data.Message.Contains(branch.BumpMajorText) ? Domain.Entities.IncrementMode.Major : branch.Increment;
                keyBranchVersion.CurrentVersion.Major += incrementMode == Domain.Entities.IncrementMode.Major ? 1 : 0;
                keyBranchVersion.CurrentVersion.Minor += incrementMode == Domain.Entities.IncrementMode.Minor ? 1 : 0;
                keyBranchVersion.CurrentVersion.Patch += incrementMode == Domain.Entities.IncrementMode.Patch ? 1 : 0;

                keyBranchVersion.CurrentVersion.Minor = incrementMode == Domain.Entities.IncrementMode.Major ? 0 : keyBranchVersion.CurrentVersion.Minor;
                keyBranchVersion.CurrentVersion.Patch = incrementMode == Domain.Entities.IncrementMode.Minor || incrementMode == Domain.Entities.IncrementMode.Major ? 0 : keyBranchVersion.CurrentVersion.Patch;
            }

            if (keyBranch.FormatVersion.ToLower().Contains("{prerelease}"))
            {
                keyBranchVersion.CurrentVersion.PreRelease = $"-{data.PreReleaseLabel}";

            }
            if (keyBranch.FormatVersion.ToLower().Contains("{build}"))
            {
                keyBranchVersion.CurrentVersion.Build = $"+{data.BuildLabel}";
            }

            keyBranchVersion.UpdatedAt = DateTime.UtcNow;

            var history = await this.versionHistoryRepository.GetByAsync(h => h.KeyBranchName == keyBranch.Name);
            if (history == null)
            {
                return null;
            }

            history.VersionHistory.Add(new Domain.Entities.KeyBranchVersion
            {
                CreatedAt = DateTime.UtcNow,
                FormatVersion = keyBranch.FormatVersion,
                Request = data.ToEntity(),
                Version = keyBranchVersion.CurrentVersion,
            });

            history.KeyBranchName = keyBranchVersion.KeyBranchName;
            history.OrganisationId = organisationId;
            history.TeamId = teamId;
            history.ApplicationId = appId;

            await this.versionHistoryRepository.UpdateAsync(history);
            await this.repository.UpdateAsync(appLocated);

            return keyBranchVersion.CurrentVersion.ToContract();
        }

        public async Task<SemanticVersion> GetVersionFromSourceBranchAsync(string organisationId, string teamId, string appId, BranchActionRequest data)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Id == organisationId))
            {
                return null;
            }

            if (!await this.teamRepository.ExistsByAsync(t => t.TeamCode == teamId && t.OrganisationId == organisationId))
            {
                return null;
            }

            var appLocated = await this.repository.GetByAsync(app => app.TeamCode == teamId && app.OrganisationId == organisationId && app.Id == appId);
            if (appLocated == null)
            {
                return null;
            }

            var keyBranch = appLocated.KeyBranches.FirstOrDefault(b => Regex.IsMatch(data.From, b.BranchPattern));
            if (keyBranch == null)
            {
                var branch = appLocated.Branches.FirstOrDefault(b => Regex.IsMatch(data.From, b.BranchPattern));
                if (branch == null)
                {
                    return null;
                }

                keyBranch = appLocated.KeyBranches.FirstOrDefault(b => b.Name == branch.InheritedFrom.Name);
                if (keyBranch == null)
                {
                    return null;
                }
            }

            var keyBranchVersion = appLocated.KeyBranchVersionings.FirstOrDefault(k => k.KeyBranchName == keyBranch.Name);
            if (keyBranchVersion == null)
            {
                return null;
            }

            if (keyBranch.FormatVersion.ToLower().Contains("{prerelease}"))
            {
                keyBranchVersion.CurrentVersion.PreRelease = data.PreReleaseLabel?.Trim().Length > 0 ? $"{data.PreReleaseLabel}" : string.Empty;

            }
            if (keyBranch.FormatVersion.ToLower().Contains("{build}"))
            {
                keyBranchVersion.CurrentVersion.Build = data.BuildLabel?.Trim().Length > 0 ?  $"{data.BuildLabel}" : string.Empty;
            }

            return keyBranchVersion.CurrentVersion.ToContract();
        }

        public async Task<KeyBranchVersionHistory> LoadHistoryFromKeyBranchName(string organisationId, string teamId, string appId, string keyBranchName)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Id == organisationId))
            {
                return null;
            }

            if (!await this.teamRepository.ExistsByAsync(t => t.TeamCode == teamId && t.OrganisationId == organisationId))
            {
                return null;
            }

            var appLocated = await this.repository.GetByAsync(app => app.TeamCode == teamId && app.OrganisationId == organisationId && app.Id == appId);
            if (appLocated == null)
            {
                return null;
            }

            var history = await this.versionHistoryRepository.GetByAsync(h => h.OrganisationId == organisationId && h.TeamId == teamId && h.ApplicationId == appId && h.KeyBranchName == keyBranchName);
            return history?.ToContract() ;
        }

        public async Task<bool> IsValidSha1Async(string orgId, string teamId, string appId, string sha1, string payload)
        {

            if (!await organisationRepository.ExistsByAsync(o => o.Id == orgId))
            {
                return false;
            }

            if (!await this.teamRepository.ExistsByAsync(t => t.TeamCode == teamId && t.OrganisationId == orgId))
            {
                return false;
            }

            var appLocated = await this.repository.GetByAsync(app => app.TeamCode == teamId && app.OrganisationId == orgId && app.Id == appId);
            if (appLocated == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(appLocated.GithubSecretKey))
            {
                return true;
            }

            var key = Encoding.ASCII.GetBytes(appLocated.GithubSecretKey);
            var newSha1 = new HMACSHA1(key);
            var body = Encoding.ASCII.GetBytes(payload);
            var bodyEnc = newSha1.ComputeHash(body);
            var secretSha1 ="sha1="+ bodyEnc.ToHexString();

            return sha1 == secretSha1;
        }
    }
}