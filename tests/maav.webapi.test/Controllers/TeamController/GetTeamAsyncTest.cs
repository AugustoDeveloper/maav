using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MAAV.Application;
using Xunit;

namespace MAAV.WebAPI.Test.Controllers.TeamController
{
    public class GetTeamAsyncTest
    {
        [Theory]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData(" ", null)]
        [InlineData("apple", null)]
        [InlineData("apple", "")]
        [InlineData("apple", " ")]
        public async Task Given_Invalid_OrganisationName_Or_TeamName_When_Call_GetTeamAsync_ShouldReturns_BadRequest(string organisationName, string teamName)
        {
            var controller = new WebAPI.Controllers.TeamController();
            var result = await controller.GetTeamAsync(organisationName, teamName, null);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [InlineData("apple", "develop")]
        public async Task Given_Invalid_OrganisationService_When_Call_GetTeamAsync_ShouldReturns_InternalServerError(string organisationName, string teamName)
        {
            var moqService = new Mock<ITeamService>();
            moqService
                .Setup(t => t.GetByTeamNameAsync(It.IsAny<string>(),It.IsAny<string>()))
                .Throws<Exception>()
                .Verifiable();

            var controller = new WebAPI.Controllers.TeamController();
            var result = await controller.GetTeamAsync(organisationName, teamName, moqService.Object);
            moqService.Verify();
            Assert.IsType<StatusCodeResult>(result);            
            var internalServerError = (StatusCodeResult)result;
            Assert.NotNull(internalServerError);
            Assert.Equal((int)HttpStatusCode.InternalServerError, internalServerError.StatusCode);
        }

        [Theory]
        [InlineData("apple", "develop")]
        public async Task Given_Team_Not_Registered_When_Call_GetTeamAsync_ShouldReturns_NotFoundResult(string organisationName, string teamName)
        {
            var moqService = new Mock<ITeamService>();
            moqService
                .Setup(t => t.GetByTeamNameAsync(It.IsAny<string>(),It.IsAny<string>()))
                .ReturnsAsync(() => null)
                .Verifiable();

            var controller = new WebAPI.Controllers.TeamController();
            var result = await controller.GetTeamAsync(organisationName, teamName, moqService.Object);
            moqService.Verify();
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData("apple", "develop")]
        public async Task Given_Valid_OrganisationService_And_OrganisationName_And_TeamName_When_Call_GetTeamAsync_ShouldReturns_OkObjectResult_And_TeamInstance(string organisationName, string teamName)
        {
            var moqService = new Mock<ITeamService>();
            moqService
                .Setup(t => t.GetByTeamNameAsync(It.IsAny<string>(),It.IsAny<string>()))
                .ReturnsAsync(new DataContracts.Team { Name = teamName })
                .Verifiable();

            var controller = new WebAPI.Controllers.TeamController();
            var result = await controller.GetTeamAsync(organisationName, teamName, moqService.Object);
            
            moqService.Verify();
            Assert.IsType<OkObjectResult>(result);            
            
            var okObjectResult = (OkObjectResult)result;
            
            Assert.NotNull(okObjectResult);
            Assert.NotNull(okObjectResult.Value);
            
            var resultOrganisation = (DataContracts.Team)okObjectResult.Value;
            Assert.NotNull(resultOrganisation);
            Assert.Equal(resultOrganisation.Name, teamName);
        }
    }
}