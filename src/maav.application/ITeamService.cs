using System.Collections.Generic;
using System.Threading.Tasks;
using MAAV.DataContracts;

namespace MAAV.Application
{
    public interface ITeamService
    {
        Task<Team> GetByTeamNameAsync(string organisationName, string teamName);
        Task<Team> AddAsync(string organisationName, Team team);
        Task<Team> UpdateAsync(string organisationName, Team team);
        Task DeleteAsync(string organisationName, string teamName);
        
        //TODO: Need create unit test
        Task<Team[]> LoadByOrganisationNameAsync(string organisationName);
    }
}