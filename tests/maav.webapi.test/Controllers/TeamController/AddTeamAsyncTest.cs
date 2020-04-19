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
    public class AddTeamAsyncTest
    {
        [Theory]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData(" ", null)]
        [InlineData("apple", null)]
        [InlineData("apple", "")]
        [InlineData("apple", " ")]
        public async Task Given_Invalid_OrganisationName_Or_TeamName_When_Call_AddTeamAsync_ShouldReturns_BadRequest(string organisationName, string teamName)
        {
            var controller = new WebAPI.Controllers.TeamController();
            var result = await controller.AddTeamAsync(organisationName, new DataContracts.Team { Name = teamName }, null, null);
            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Theory]
        [InlineData("apple", "develop")]
        public async Task Given_TeamName_Is_Already_Registered_When_Call_AddTeamAsync_ShouldReturns_BadRequest(string organisationName, string teamName)
        {
            var moqService = new Mock<ITeamService>();
            moqService
                .Setup(t => t.AddAsync(It.IsAny<string>(), It.IsAny<DataContracts.Team>()))
                .ThrowsAsync(new NameAlreadyUsedException(teamName))
                .Verifiable();
            
            var controller = new WebAPI.Controllers.TeamController();
            var result = await controller.AddTeamAsync(organisationName, new DataContracts.Team { Name = teamName }, moqService.Object, null);
            moqService.Verify();

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [InlineData("apple", "develop")]
        public async Task Given_Invalid_TeamService_When_Call_AddTeamAsync_ShouldReturns_InternalServerError(string organisationName, string teamName)
        {
            var controller = new WebAPI.Controllers.TeamController();
            var result = await controller.AddTeamAsync(organisationName, new DataContracts.Team { Name = teamName }, null, null);
            Assert.IsType<StatusCodeResult>(result);
            var statusCodeResult = (StatusCodeResult)result;
            Assert.NotNull(statusCodeResult);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCodeResult.StatusCode);
        }

        [Theory]
        [InlineData("apple", "develop")]
        public async Task Given_TeamName_Not_Registered_When_Call_AddTeamAsync_ShouldReturns_CreateAtRouteResult(string organisationName, string teamName)
        {
            var moqService = new Mock<ITeamService>();
            moqService
                .Setup(t => t.AddAsync(It.IsAny<string>(), It.IsAny<DataContracts.Team>()))
                .ReturnsAsync(new DataContracts.Team { Name = teamName })
                .Verifiable();
            
            var controller = new WebAPI.Controllers.TeamController();
            var result = await controller.AddTeamAsync(organisationName, new DataContracts.Team { Name = teamName }, moqService.Object, null);
            moqService.Verify();

            Assert.IsType<CreatedAtRouteResult>(result);
        }
    }
}