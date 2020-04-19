using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using MAAV.Application.Extensions;
using MAAV.Domain.Repositories;
using Xunit;

namespace MAAV.Application.Test.TeamService
{
    public class GetByTeamNameAsyncTest
    {
        public GetByTeamNameAsyncTest()
        {
            AutoMapperExtension.AddMapping(null);
        }

        [Theory]
        [InlineData("apple", "develop")]
        public async Task Given_Not_Registered_OrganisationId_Or_TeamName_When_Call_GetByTeamNameAsync_ShouldReturns_Null_Instance(string organisationId, string teamName)
        {
            var moqRepository = new Mock<ITeamRepository>();
            moqRepository
                .Setup(t => t.GetByAsync(It.IsAny<Expression<Func<Domain.Entities.Team, bool>>>()))
                .ReturnsAsync(() => null)
                .Verifiable();

            var moqOrgRepository = new Mock<IOrganisationRepository>();
            moqOrgRepository
                .Setup(t => t.ExistsByAsync(It.IsAny<Expression<Func<Domain.Entities.Organisation, bool>>>()))
                .ReturnsAsync(true)
                .Verifiable();  
            
            var service = new Application.TeamService(moqRepository.Object, moqOrgRepository.Object, null);
            Assert.Null(await service.GetByTeamNameAsync(organisationId, teamName));
            moqRepository.Verify();
        }

        [Theory]
        [InlineData("apple", "develop")]
        public async Task Given_Registered_OrganisationId_Or_TeamName_When_Call_GetByTeamNameAsync_ShouldReturns_Team_Instance(string organisationId, string teamName)
        {
            var moqRepository = new Mock<ITeamRepository>();
            moqRepository
                .Setup(t => t.GetByAsync(It.IsAny<Expression<Func<Domain.Entities.Team, bool>>>()))
                .ReturnsAsync(() => new Domain.Entities.Team { Name = teamName, OrganisationId = organisationId})
                .Verifiable();

            var moqOrgRepository = new Mock<IOrganisationRepository>();
            moqOrgRepository
                .Setup(t => t.ExistsByAsync(It.IsAny<Expression<Func<Domain.Entities.Organisation, bool>>>()))
                .ReturnsAsync(true)
                .Verifiable();  
            
            var service = new Application.TeamService(moqRepository.Object, moqOrgRepository.Object, null);
            var teamResult = await service.GetByTeamNameAsync(organisationId, teamName);
            moqRepository.Verify();
            Assert.NotNull(teamResult);
            Assert.Equal(teamName, teamResult.Name);
        }
    }
}