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
    public class AddUserAsyncTest
    {
        [Theory]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData(" ", null)]
        [InlineData("apple", null)]
        [InlineData("apple", "")]
        [InlineData("apple", " ")]
        public async Task Given_Invalid_OrganisationName_Or_UserName_When_Call_AddUserAsync_ShouldReturns_BadRequest(string organisationName, string userName)
        {
            var controller = new WebAPI.Controllers.UserController();
            var result = await controller.AddUserAsync(organisationName, new DataContracts.User { Username = userName }, null);
            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Theory]
        [InlineData("apple", "develop")]
        public async Task Given_UserName_Is_Already_Registered_When_Call_AddUserAsync_ShouldReturns_BadRequest(string organisationName, string userName)
        {
            var moqService = new Mock<IUserService>();
            moqService
                .Setup(t => t.AddAsync(It.IsAny<string>(), It.IsAny<DataContracts.User>()))
                .ThrowsAsync(new NameAlreadyUsedException(userName))
                .Verifiable();
            
            var controller = new WebAPI.Controllers.UserController();
            var result = await controller.AddUserAsync(organisationName, new DataContracts.User { Username = userName }, moqService.Object);
            moqService.Verify();

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [InlineData("apple", "develop")]
        public async Task Given_Invalid_UserService_When_Call_AddUserAsync_ShouldReturns_InternalServerError(string organisationName, string userName)
        {
            var controller = new WebAPI.Controllers.UserController();
            var result = await controller.AddUserAsync(organisationName, new DataContracts.User { Username = userName }, null);
            Assert.IsType<StatusCodeResult>(result);
            var statusCodeResult = (StatusCodeResult)result;
            Assert.NotNull(statusCodeResult);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCodeResult.StatusCode);
        }

        [Theory]
        [InlineData("apple", "develop")]
        public async Task Given_UserName_Not_Registered_When_Call_AddUserAsync_ShouldReturns_CreateAtRouteResult(string organisationName, string userName)
        {
            var moqService = new Mock<IUserService>();
            moqService
                .Setup(t => t.AddAsync(It.IsAny<string>(), It.IsAny<DataContracts.User>()))
                .ReturnsAsync(new DataContracts.User { Username = userName })
                .Verifiable();
            
            var controller = new WebAPI.Controllers.UserController();
            var result = await controller.AddUserAsync(organisationName, new DataContracts.User { Username = userName }, moqService.Object);
            moqService.Verify();

            Assert.IsType<CreatedAtRouteResult>(result);
        }
    }
}