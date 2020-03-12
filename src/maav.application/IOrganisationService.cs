using System;
using System.Threading.Tasks;
using MAAV.DataContracts;

namespace MAAV.Application
{
    public interface IOrganisationService
    {
        Task<Organisation> GetByOrganisationNameAsync(string organisationName);
        Task<Organisation> RegisterAsync(Organisation organisationContract);
        Task<Organisation> UpdateAsync(Organisation organisationContract);
        Task DeleteByOrganisationNameAsync(string organisationName);
    }
}