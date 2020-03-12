using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MAAV.Application
{
    public interface IApplicationService
    {
        Task<DataContracts.Application> GetByNameAsync(string organisationName, string teamName, string applicationName);
        Task<List<DataContracts.Application>> LoadAllFromTeamAsync(string organisationName, string teamName);
        Task DeleteByNameAsync(string organisationName, string teamName, string applicationName);
        Task<DataContracts.Application> AddAsync(string organisationName, string teamName, DataContracts.Application application);
        Task<DataContracts.Application> UpdateAsync(string organisationName, string teamName, DataContracts.Application application);
    }
}
