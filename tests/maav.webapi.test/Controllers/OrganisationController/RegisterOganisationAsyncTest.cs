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
    public class RegisterOganisationAsyncTest 
    {

        [Theory]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData(" ", null)]
        [InlineData("apple", null)]
        public async Task  Given_A_Invalid_OrganisationName_When_Call_RegisterOrganisationAsync_ShouldReturns_BadRequest(string organisationName, DataContracts.Organisation organisation)
        {
            var controller = new WebAPI.Controllers.OrganisationController();
            var result = await controller.RegisterOrganisationAsync(organisationName, organisation, null);
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.NotNull(badRequestResult);
            Assert.NotNull(badRequestResult.Value);
        }

        [Theory]
        [InlineData("microsoft")]
        public async Task  Given_A_Invalid_OrganisationName_And_Request_Organisation_Name_Is_Different_When_Call_RegisterOrganisationAsync_ShouldReturns_BadRequest(string organisationName)
        {
            var controller = new WebAPI.Controllers.OrganisationController();
            var result = await controller.RegisterOrganisationAsync(organisationName, new DataContracts.Organisation { Name = "Apple" }, null);
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.NotNull(badRequestResult);
            Assert.NotNull(badRequestResult.Value);
        }

        [Theory]
        [InlineData("Apple")]
        public async Task  Given_A_Invalid_OrganisationName_Already_Registered_When_Call_RegisterOrganisationAsync_ShouldReturns_BadRequest(string organisationName)
        {
            var moqService = new Mock<IOrganisationService>();
            moqService
                .Setup(o => o.RegisterAsync(It.IsAny<DataContracts.Organisation>()))
                .ThrowsAsync(new NameAlreadyUsedException(organisationName))
                .Verifiable();

            var controller = new WebAPI.Controllers.OrganisationController();
            var result = await controller.RegisterOrganisationAsync(organisationName, new DataContracts.Organisation { Name = organisationName}, moqService.Object);
            moqService.Verify();
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.NotNull(badRequestResult);
            Assert.NotNull(badRequestResult.Value);
        }

        [Theory]
        [InlineData("Apple")]
        public async Task  Given_A_Invalid_OrganisationService_Is_Null_When_Call_RegisterOrganisationAsync_ShouldReturns_InternalServerError(string organisationName)
        {
            var controller = new WebAPI.Controllers.OrganisationController();
            var result = await controller.RegisterOrganisationAsync(organisationName, new DataContracts.Organisation { Name = organisationName }, null);
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
                .Setup(o => o.RegisterAsync(It.IsAny<DataContracts.Organisation>()))
                .ReturnsAsync(organisation)
                .Verifiable();

            var controller = new WebAPI.Controllers.OrganisationController();
            var routeName = nameof(controller.GetOrganisationAsync);
            var result = await controller.RegisterOrganisationAsync(organisationName, organisation, moqService.Object);
            moqService.Verify();

            Assert.IsType<CreatedAtRouteResult>(result);
            var createdObjectResult = (CreatedAtRouteResult)result;
            Assert.NotNull(createdObjectResult);
            Assert.NotNull(createdObjectResult.Value);
            Assert.Equal(routeName, createdObjectResult.RouteName);
            Assert.NotNull((DataContracts.Organisation)createdObjectResult.Value);
            Assert.Equal(organisation.Name, ((DataContracts.Organisation)createdObjectResult.Value).Name);
        }
    }
}