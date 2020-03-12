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
    public class UpdateOrganisationAsyncTest
    {
        [Theory]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData(" ", null)]
        [InlineData("apple", null)]
        public async Task  Given_A_Invalid_OrganisationName_Or_Organisation_When_Call_UpdateOrganisationAsync_ShouldReturns_BadRequest(string organisationName, DataContracts.Organisation organisation)
        {
            var controller = new WebAPI.Controllers.OrganisationController();
            var result = await controller.UpdateOrganisationAsync(organisationName, organisation, null);
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.NotNull(badRequestResult);
            Assert.NotNull(badRequestResult.Value);
        }

        [Theory]
        [InlineData("microsoft")]
        public async Task  Given_A_Invalid_OrganisationName_And_Request_Organisation_Name_Is_Different_When_Call_UpdateOrganisationAsync_ShouldReturns_BadRequest(string organisationName)
        {
            var controller = new WebAPI.Controllers.OrganisationController();
            var result = await controller.UpdateOrganisationAsync(organisationName, new DataContracts.Organisation { Name = "Apple" }, null);
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.NotNull(badRequestResult);
            Assert.NotNull(badRequestResult.Value);
        }

        [Theory]
        [InlineData("Apple")]
        public async Task  Given_A_Invalid_OrganisationName_Not_Registered_When_Call_UpdateOrganisationAsync_ShouldReturns_BadRequest(string organisationName)
        {
            var moqService = new Mock<IOrganisationService>();
            moqService
                .Setup(o => o.UpdateAsync(It.IsAny<DataContracts.Organisation>()))
                .ReturnsAsync(() => null)
                .Verifiable();

            var controller = new WebAPI.Controllers.OrganisationController();
            var result = await controller.UpdateOrganisationAsync(organisationName, new DataContracts.Organisation { Name = organisationName }, moqService.Object);
            moqService.Verify();
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task  Given_A_Invalid_OrganisationService_When_Call_UpdateOrganisationAsync_ShouldReturns_InternalServerError()
        {
            var controller = new WebAPI.Controllers.OrganisationController();
            var result = await controller.UpdateOrganisationAsync("Apple", new DataContracts.Organisation { Name = "Apple" }, null);
            Assert.IsType<StatusCodeResult>(result);
            var statusCodeResult = (StatusCodeResult)result;
            Assert.NotNull(statusCodeResult);
            Assert.Equal(statusCodeResult.StatusCode, (int)HttpStatusCode.InternalServerError);
        }

        [Theory]
        [InlineData("microsoft")]
        [InlineData("google")]
        [InlineData("apple")]
        public async Task Given_A_Valid_OrganisationService_And_OrganisationName_And_OrganisationRequest_When_Call_UpdateOrganisationAsync_ShouldReturns_CreatedAtRouteResult(string organisationName)
        {
            var organisation = new DataContracts.Organisation { Name = organisationName };
            var moqService = new Mock<IOrganisationService>();
            moqService
                .Setup(o => o.UpdateAsync(It.IsAny<DataContracts.Organisation>()))
                .ReturnsAsync(organisation)
                .Verifiable();

            var controller = new WebAPI.Controllers.OrganisationController();
            var result = await controller.UpdateOrganisationAsync(organisationName, organisation, moqService.Object);
            moqService.Verify();

            Assert.IsType<OkObjectResult>(result);
            var okObjectResult = (OkObjectResult)result;
            Assert.NotNull(okObjectResult);
            Assert.NotNull(okObjectResult.Value);
            Assert.Equal(okObjectResult.Value, organisation);
        }
    }
}