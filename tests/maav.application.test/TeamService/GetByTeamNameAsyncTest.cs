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
        public async Task Given_Not_Registered_OrganisationName_Or_TeamName_When_Call_GetByTeamNameAsync_ShouldReturns_Null_Instance(string organisationName, string teamName)
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
            
            var service = new Application.TeamService(moqRepository.Object, moqOrgRepository.Object);
            Assert.Null(await service.GetByTeamNameAsync(organisationName, teamName));
            moqRepository.Verify();
        }

        [Theory]
        [InlineData("apple", "develop")]
        public async Task Given_Registered_OrganisationName_Or_TeamName_When_Call_GetByTeamNameAsync_ShouldReturns_Team_Instance(string organisationName, string teamName)
        {
            var moqRepository = new Mock<ITeamRepository>();
            moqRepository
                .Setup(t => t.GetByAsync(It.IsAny<Expression<Func<Domain.Entities.Team, bool>>>()))
                .ReturnsAsync(() => new Domain.Entities.Team { Name = teamName, OrganisationName = organisationName})
                .Verifiable();

            var moqOrgRepository = new Mock<IOrganisationRepository>();
            moqOrgRepository
                .Setup(t => t.ExistsByAsync(It.IsAny<Expression<Func<Domain.Entities.Organisation, bool>>>()))
                .ReturnsAsync(true)
                .Verifiable();  
            
            var service = new Application.TeamService(moqRepository.Object, moqOrgRepository.Object);
            var teamResult = await service.GetByTeamNameAsync(organisationName, teamName);
            moqRepository.Verify();
            Assert.NotNull(teamResult);
            Assert.Equal(teamName, teamResult.Name);
        }
    }
}