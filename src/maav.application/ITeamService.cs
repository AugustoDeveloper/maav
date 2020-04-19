using System.Collections.Generic;
using System.Threading.Tasks;
using MAAV.DataContracts;

namespace MAAV.Application
{
    public interface ITeamService
    {
        Task<Team> GetByTeamNameAsync(string organisationId, string teamName);
        Task<Team> AddAsync(string organisationId, Team team);
        Task<Team> UpdateAsync(string organisationId, Team team);
        Task DeleteAsync(string organisationId, string teamName);
        
        //TODO: Need create unit test
        Task<Team[]> LoadByOrganisationIdAsync(string organisationId);
    }
}