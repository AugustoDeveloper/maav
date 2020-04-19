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
    public class AddAsyncTest
    {
        public AddAsyncTest()
        {
            AutoMapperExtension.AddMapping(null);
        }

        [Fact]
        public async Task Given_Exists_Team_When_Call_AddAsync_ShouldReturns_NameAlreadyUseException()
        {
            var moqRepository = new Mock<ITeamRepository>();
            moqRepository
                .Setup(t => t.ExistsByAsync(It.IsAny<Expression<Func<Team, bool>>>()))
                .ReturnsAsync(true)
                .Verifiable();

            var moqOrgRepository = new Mock<IOrganisationRepository>();
            moqOrgRepository
                .Setup(t => t.ExistsByAsync(It.IsAny<Expression<Func<Organisation, bool>>>()))
                .ReturnsAsync(true)
                .Verifiable();
            
            var service = new Application.TeamService(moqRepository.Object, moqOrgRepository.Object, null);
            await Assert.ThrowsAsync<NameAlreadyUsedException>(() => service.AddAsync("", new DataContracts.Team { Name = "development" }));
            moqRepository.Verify(t => t.AddAsync(It.IsAny<Team>()), Times.Never);
            moqRepository.Verify();
            moqOrgRepository.Verify();
        }

        [Theory]
        [InlineData("apple", "develop")]
        public async Task Given_Not_Exists_Team_When_Call_AddAsync_ShouldReturns_A_New_Instance_Of_Team(string organisationId, string teamName)
        {
            var moqRepository = new Mock<ITeamRepository>();
            moqRepository
                .Setup(t => t.ExistsByAsync(It.IsAny<Expression<Func<Team, bool>>>()))
                .ReturnsAsync(false)
                .Verifiable();

            moqRepository
                .Setup(t => t.AddAsync(It.IsAny<Team>()))
                .ReturnsAsync(new Team { Name = teamName, OrganisationId = organisationId })
                .Verifiable();

            var moqOrgRepository = new Mock<IOrganisationRepository>();
            moqOrgRepository
                .Setup(t => t.ExistsByAsync(It.IsAny<Expression<Func<Organisation, bool>>>()))
                .ReturnsAsync(true)
                .Verifiable();
            
            var service = new Application.TeamService(moqRepository.Object, moqOrgRepository.Object,null);
            var teamResult = await service.AddAsync("organisationId", new DataContracts.Team { Name = teamName });
            moqRepository.VerifyAll();
            moqOrgRepository.VerifyAll();
            Assert.NotNull(teamResult);
            Assert.Equal(teamName, teamResult.Name);
        }
    }
}