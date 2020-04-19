using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MAAV.Application;
using MAAV.Application.Exceptions;
using Xunit;

namespace MAAV.WebAPI.Test.Controllers.UserController
{
    public class UpdateUserAsyncTest
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
        public async Task Given_Invalid_OrganisationName_Or_UserName_When_Call_UpdateUserAsync_ShouldReturns_BadRequest(string organisationName, string userName, string instanceUserName)
        {
            var controller = new WebAPI.Controllers.UserController();
            var result = await controller.UpdateUserAsync(organisationName, userName, new DataContracts.User { Username = instanceUserName }, null);
            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Theory]
        [InlineData("apple", "develop", "devs")]
        public async Task Given_UserName_And_Instance_UserName_Is_Different_When_Call_UpdateUserAsync_ShouldReturns_BadRequest(string organisationName, string userName, string instanceUserName)
        {            
            var controller = new WebAPI.Controllers.UserController();
            var result = await controller.UpdateUserAsync(organisationName, userName, new DataContracts.User { Username = instanceUserName }, null);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [InlineData("apple", "develop", "develop")]
        public async Task Given_UserName_Is_Not_Registered_When_Call_UpdateUserAsync_ShouldReturns_NotFound(string organisationName, string userName, string instanceUserName)
        {
            var moqService = new Mock<IUserService>();
            moqService
                .Setup(t => t.UpdateAsync(It.IsAny<string>(), It.IsAny<DataContracts.User>(), It.IsAny<bool>()))
                .ReturnsAsync(() => null)
                .Verifiable();
            
            var controller = new WebAPI.Controllers.UserController();
            var result = await controller.UpdateUserAsync(organisationName, userName, new DataContracts.User { Username = instanceUserName }, moqService.Object);
            moqService.Verify();

            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData("apple", "develop", "develop")]
        public async Task Given_Invalid_UserService_When_Call_UpdateUserAsync_ShouldReturns_InternalServerError(string organisationName, string userName, string instanceUserName)
        {
            var controller = new WebAPI.Controllers.UserController();
            var result = await controller.UpdateUserAsync(organisationName, userName, new DataContracts.User { Username = instanceUserName }, null);
            Assert.IsType<StatusCodeResult>(result);
            var statusCodeResult = (StatusCodeResult)result;
            Assert.NotNull(statusCodeResult);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCodeResult.StatusCode);
        }

        [Theory]
        [InlineData("apple", "develop", "develop")]
        public async Task Given_UserName_Not_Registered_When_Call_UpdateUserAsync_ShouldReturns_CreateAtRouteResult(string organisationName, string userName, string instanceUserName)
        {
            var moqService = new Mock<IUserService>();
            moqService
                .Setup(t => t.UpdateAsync(It.IsAny<string>(), It.IsAny<DataContracts.User>(), It.IsAny<bool>()))
                .ReturnsAsync(new DataContracts.User { Username = userName })
                .Verifiable();
            
            var controller = new WebAPI.Controllers.UserController();
            var result = await controller.UpdateUserAsync(organisationName, userName, new DataContracts.User { Username = instanceUserName }, moqService.Object);
            moqService.Verify();

            Assert.IsType<OkObjectResult>(result);
        }
    }
}