using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MAAV.Application;
using MAAV.Application.Exceptions;
using MAAV.WebAPI.Controllers;
using Xunit;

namespace MAAV.WebAPI.Test.Controllers.OrganisationController
{
    public class DeleteOrganisationAsyncTest 
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task  Given_A_Invalid_OrganisationName_When_Call_DeleteOrganisationAsync_ShouldReturns_BadRequest(string organisationName)
        {
            var controller = new WebAPI.Controllers.OrganisationController();
            var result = await controller.DeleteOrganisationAsync(organisationName, null);
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.NotNull(badRequestResult);
            Assert.NotNull(badRequestResult.Value);
        }

        [Theory]
        [InlineData("apple")]
        [InlineData("microsoft")]
        [InlineData("ebay")]
        public async Task  Given_A_Invalid_OrganisationService_When_Call_DeleteOrganisationAsync_ShouldReturns_InternalServerError(string organisationName)
        {
            var controller = new WebAPI.Controllers.OrganisationController();
            var result = await controller.DeleteOrganisationAsync(organisationName, null);
            Assert.IsType<StatusCodeResult>(result);
            var internalServerErrorResult = (StatusCodeResult)result;
            Assert.NotNull(internalServerErrorResult);
            Assert.Equal((int)HttpStatusCode.InternalServerError, internalServerErrorResult.StatusCode);
        }

        [Theory]
        [InlineData("apple")]
        [InlineData("microsoft")]
        [InlineData("ebay")]
        public async Task  Given_A_Valid_OrganisationName_And_OrganisationService_When_Call_DeleteOrganisationAsync_ShouldReturns_NoContentResult(string organisationName)
        {
            var moqService = new Mock<IOrganisationService>();
            moqService
                .Setup(o => o.DeleteByOrganisationNameAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var controller = new WebAPI.Controllers.OrganisationController();
            var result = await controller.DeleteOrganisationAsync(organisationName, moqService.Object);
            moqService.Verify();
            Assert.IsType<NoContentResult>(result);
        }
    }
}