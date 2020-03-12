using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MAAV.Application;
using Xunit;

namespace MAAV.WebAPI.Test.Controllers.UserController
{
    public class GetUserAsyncTest
    {
        [Theory]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData(" ", null)]
        [InlineData("apple", null)]
        [InlineData("apple", "")]
        [InlineData("apple", " ")]
        public async Task Given_Invalid_OrganisationName_Or_UserName_When_Call_GetUserAsync_ShouldReturns_BadRequest(string organisationName, string userName)
        {
            var controller = new WebAPI.Controllers.UserController();
            var result = await controller.GetUserAsync(organisationName, userName, null);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [InlineData("apple", "develop")]
        public async Task Given_Invalid_OrganisationService_When_Call_GetUserAsync_ShouldReturns_InternalServerError(string organisationName, string userName)
        {
            var moqService = new Mock<IUserService>();
            moqService
                .Setup(t => t.GetByUsernameAsync(It.IsAny<string>(),It.IsAny<string>()))
                .Throws<Exception>()
                .Verifiable();

            var controller = new WebAPI.Controllers.UserController();
            var result = await controller.GetUserAsync(organisationName, userName, moqService.Object);
            moqService.Verify();
            Assert.IsType<StatusCodeResult>(result);            
            var internalServerError = (StatusCodeResult)result;
            Assert.NotNull(internalServerError);
            Assert.Equal((int)HttpStatusCode.InternalServerError, internalServerError.StatusCode);
        }

        [Theory]
        [InlineData("apple", "develop")]
        public async Task Given_User_Not_Registered_When_Call_GetUserAsync_ShouldReturns_NotFoundResult(string organisationName, string userName)
        {
            var moqService = new Mock<IUserService>();
            moqService
                .Setup(t => t.GetByUsernameAsync(It.IsAny<string>(),It.IsAny<string>()))
                .ReturnsAsync(() => null)
                .Verifiable();

            var controller = new WebAPI.Controllers.UserController();
            var result = await controller.GetUserAsync(organisationName, userName, moqService.Object);
            moqService.Verify();
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData("apple", "develop")]
        public async Task Given_Valid_OrganisationService_And_OrganisationName_And_UserName_When_Call_GetUserAsync_ShouldReturns_OkObjectResult_And_UserInstance(string organisationName, string userName)
        {
            var moqService = new Mock<IUserService>();
            moqService
                .Setup(t => t.GetByUsernameAsync(It.IsAny<string>(),It.IsAny<string>()))
                .ReturnsAsync(new DataContracts.User { Username = userName })
                .Verifiable();

            var controller = new WebAPI.Controllers.UserController();
            var result = await controller.GetUserAsync(organisationName, userName, moqService.Object);
            
            moqService.Verify();
            Assert.IsType<OkObjectResult>(result);            
            
            var okObjectResult = (OkObjectResult)result;
            
            Assert.NotNull(okObjectResult);
            Assert.NotNull(okObjectResult.Value);
            
            var resultOrganisation = (DataContracts.User)okObjectResult.Value;
            Assert.NotNull(resultOrganisation);
            Assert.Equal(resultOrganisation.Username, userName);
        }
    }
}