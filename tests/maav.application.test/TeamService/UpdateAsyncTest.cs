using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using MAAV.Application.Exceptions;
using MAAV.Application.Extensions;
using MAAV.Domain.Entities;
using MAAV.Domain.Repositories;
using Xunit;

namespace MAAV.Application.Test.TeamService
{
    public class UpdateAsyncTest
    {
        public UpdateAsyncTest()
        {
            AutoMapperExtension.AddMapping(null);
        }

        [Fact]
        public async Task Given_Not_Exists_Team_When_Call_UpdateAsync_ShouldReturns_Null_Reference()
        {
            var moqRepository = new Mock<ITeamRepository>();
            moqRepository
                .Setup(t => t.GetByAsync(It.IsAny<Expression<Func<Team, bool>>>()))
                .ReturnsAsync(() => null)
                .Verifiable();
            
            var moqOrgRepository = new Mock<IOrganisationRepository>();
            moqOrgRepository
                .Setup(t => t.ExistsByAsync(It.IsAny<Expression<Func<Domain.Entities.Organisation, bool>>>()))
                .ReturnsAsync(true)
                .Verifiable();  

            var service = new Application.TeamService(moqRepository.Object, moqOrgRepository.Object, null);
            var teamResult = await service.UpdateAsync("", new DataContracts.Team { Name = "development" });
            moqRepository.Verify(t => t.UpdateAsync(It.IsAny<Team>()), Times.Never);
            moqRepository.Verify();
            Assert.Null(teamResult);
        }

        [Theory]
        [InlineData("apple", "develop")]
        public async Task Given_Exists_Team_When_Call_UpdateAsync_ShouldReturns_A_New_Instance_Of_Team(string organisationId, string teamName)
        {
            var moqRepository = new Mock<ITeamRepository>();
            moqRepository
                .Setup(t => t.GetByAsync(It.IsAny<Expression<Func<Team, bool>>>()))
                .ReturnsAsync(new Team { Name = teamName, OrganisationId = organisationId, TeamCode = Guid.NewGuid().ToString()})
                .Verifiable();

            moqRepository
                .Setup(t => t.UpdateAsync(It.IsAny<Team>()))
                .ReturnsAsync(new Team { Name = teamName, OrganisationId = organisationId })
                .Verifiable();
            
            var moqOrgRepository = new Mock<IOrganisationRepository>();
            moqOrgRepository
                .Setup(t => t.ExistsByAsync(It.IsAny<Expression<Func<Domain.Entities.Organisation, bool>>>()))
                .ReturnsAsync(true)
                .Verifiable();  
            
            var service = new Application.TeamService(moqRepository.Object, moqOrgRepository.Object, null);
            var teamResult = await service.UpdateAsync("organisationId", new DataContracts.Team { Name = teamName });
            moqRepository.VerifyAll();
            Assert.NotNull(teamResult);
            Assert.Equal(teamName, teamResult.Name);
        }
    }
}