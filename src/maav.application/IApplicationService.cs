using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MAAV.Application
{
    public interface IApplicationService
    {
        Task<DataContracts.Application> GetByIdAsync(string organisationName, string teamName, string appId);
        Task<List<DataContracts.Application>> LoadAllFromTeamAsync(string organisationName, string teamName);
        Task DeleteByIdAsync(string organisationName, string teamName, string appId);
        Task<DataContracts.Application> AddAsync(string organisationName, string teamName, DataContracts.Application application);
        Task<DataContracts.Application> UpdateAsync(string organisationName, string teamName, DataContracts.Application application);
        Task<bool> IsValidSha1Async(string orgId, string teamId, string appId, string sha1, string payload);
    }
}
