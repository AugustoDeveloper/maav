using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MAAV.Application.Exceptions;
using MAAV.Application.Extensions;
using MAAV.DataContracts;
using MAAV.Domain.Repositories;

namespace MAAV.Application
{
    public class OrganisationService : IOrganisationService
    {
        private readonly IOrganisationRepository repository;
        public OrganisationService(IOrganisationRepository repository)
        {
            this.repository = repository;
        }

        public async Task DeleteByOrganisationNameAsync(string organisationName)
        {
            await repository.DeleteAsync(o => o.Name == organisationName);
        }

        public async Task<DataContracts.Organisation> GetByOrganisationNameAsync(string organisationName)
        {
            var organisation = await repository.GetByAsync(o => o.Name == organisationName);
            return organisation?.ToContract();
        }

        public async Task<Organisation> RegisterAsync(Organisation organisationContract)
        {
            if (await repository.ExistsByAsync(o => o.Name == organisationContract.Name))
            {
                throw new NameAlreadyUsedException(organisationContract.Name);
            }

            return (await repository.AddAsync(organisationContract.ToEntity())).ToContract();
        }

        public async Task<Organisation> UpdateAsync(Organisation organisationContract)
        {
            if (!await repository.ExistsByAsync(o => o.Name == organisationContract.Name))
            {
                return null;
            }

            return (await repository.UpdateAsync(organisationContract.ToEntity())).ToContract();
        }
    }
}