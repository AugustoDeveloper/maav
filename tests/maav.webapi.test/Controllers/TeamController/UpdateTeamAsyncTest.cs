using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MAAV.Application;
using MAAV.Application.Exceptions;
using Xunit;

namespace MAAV.WebAPI.Test.Controllers.TeamController
{
    public class UpdateTeamAsyncTest
    {
        [Theory]
        [InlineData(null, null, null)]
        [InlineData("", null, null)]
        [InlineData(" ", null, null)]
        [InlineData("apple", null, null)]
        [InlineData("apple", "", null)]
        [InlineData("apple", " ", null)]
        [InlineData("apple", "develop", "")]
        [InlineData("apple", "develop", " ")]
        [InlineData("apple", "develop", null)]
        public async Task Given_Invalid_OrganisationName_Or_TeamName_When_Call_UpdateTeamAsync_ShouldReturns_BadRequest(string organisationName, string teamName, string instanceTeamName)
        {
            var controller = new WebAPI.Controllers.TeamController();
            var result = await controller.UpdateTeamAsync(organisationName, teamName, new DataContracts.Team { Name = instanceTeamName }, null);
            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Theory]
        [InlineData("apple", "develop", "devs")]
        public async Task Given_TeamName_And_Instance_TeamName_Is_Different_When_Call_UpdateTeamAsync_ShouldReturns_BadRequest(string organisationName, string teamName, string instanceTeamName)
        {            
            var controller = new WebAPI.Controllers.TeamController();
            var result = await controller.UpdateTeamAsync(organisationName, teamName, new DataContracts.Team { Name = instanceTeamName }, null);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [InlineData("apple", "develop", "develop")]
        public async Task Given_TeamName_Is_Not_Registered_When_Call_UpdateTeamAsync_ShouldReturns_NotFound(string organisationName, string teamName, string instanceTeamName)
        {
            var moqService = new Mock<ITeamService>();
            moqService
                .Setup(t => t.UpdateAsync(It.IsAny<string>(), It.IsAny<DataContracts.Team>()))
                .ReturnsAsync(() => null)
                .Verifiable();
            
            var controller = new WebAPI.Controllers.TeamController();
            var result = await controller.UpdateTeamAsync(organisationName, teamName, new DataContracts.Team { Name = instanceTeamName }, moqService.Object);
            moqService.Verify();

            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData("apple", "develop", "develop")]
        public async Task Given_Invalid_TeamService_When_Call_UpdateTeamAsync_ShouldReturns_InternalServerError(string organisationName, string teamName, string instanceTeamName)
        {
            var controller = new WebAPI.Controllers.TeamController();
            var result = await controller.UpdateTeamAsync(organisationName, teamName, new DataContracts.Team { Name = instanceTeamName }, null);
            Assert.IsType<StatusCodeResult>(result);
            var statusCodeResult = (StatusCodeResult)result;
            Assert.NotNull(statusCodeResult);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCodeResult.StatusCode);
        }

        [Theory]
        [InlineData("apple", "develop", "develop")]
        public async Task Given_TeamName_Not_Registered_When_Call_UpdateTeamAsync_ShouldReturns_CreateAtRouteResult(string organisationName, string teamName, string instanceTeamName)
        {
            var moqService = new Mock<ITeamService>();
            moqService
                .Setup(t => t.UpdateAsync(It.IsAny<string>(), It.IsAny<DataContracts.Team>()))
                .ReturnsAsync(new DataContracts.Team { Name = teamName })
                .Verifiable();
            
            var controller = new WebAPI.Controllers.TeamController();
            var result = await controller.UpdateTeamAsync(organisationName, teamName, new DataContracts.Team { Name = instanceTeamName }, moqService.Object);
            moqService.Verify();

            Assert.IsType<OkObjectResult>(result);
        }
    }
}