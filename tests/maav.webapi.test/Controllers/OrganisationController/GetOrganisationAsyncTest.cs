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
    public class GetOrganisationAsyncTest 
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task  Given_A_Invalid_OrganisationName_When_Call_GetOrganisationAsync_ShouldReturns_BadRequest(string organisationName)
        {
            var controller = new WebAPI.Controllers.OrganisationController();
            var result = await controller.GetOrganisationAsync(organisationName, null);
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.NotNull(badRequestResult);
            Assert.NotNull(badRequestResult.Value);
        }

        [Theory]
        [InlineData("microsoft")]
        [InlineData("google")]
        [InlineData("apple")]
        public async Task Given_A_Not_Exists_OrganisationName_When_Call_GetOrganisationAsync_ShouldReturns_NotFoundRequest(string organisationName)
        {
            var moqService = new Mock<IOrganisationService>();
            moqService
                .Setup(o => o.GetByOrganisationNameAsync(It.IsAny<string>()))
                .ReturnsAsync(() => null)
                .Verifiable();

            var controller = new WebAPI.Controllers.OrganisationController();
            var result = await controller.GetOrganisationAsync(organisationName, moqService.Object);
            moqService.Verify();
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Given_A_Invalid_OrganisationService_When_Call_GetOrganisationAsync_ShouldReturns_InternalServerError()
        {
            var controller = new WebAPI.Controllers.OrganisationController();
            var result = await controller.GetOrganisationAsync(Guid.NewGuid().ToString(), null);

            Assert.IsType<StatusCodeResult>(result);
            var statusResult = (StatusCodeResult)result;
            Assert.NotNull(statusResult);
            Assert.Equal(statusResult.StatusCode, (int)HttpStatusCode.InternalServerError);
        }

        
        [Theory]
        [InlineData("microsoft")]
        [InlineData("google")]
        [InlineData("apple")]
        public async Task Given_A_Valid_OrganisationService_And_OrganisationName_When_Call_GetOrganisationAsync_ShouldReturns_OkObjectResult(string organisationName)
        {
            var moqService = new Mock<IOrganisationService>();
            moqService
                .Setup(o => o.GetByOrganisationNameAsync(It.IsAny<string>()))
                .ReturnsAsync(() => new DataContracts.Organisation { Name = organisationName })
                .Verifiable();

            var controller = new WebAPI.Controllers.OrganisationController();
            var result = await controller.GetOrganisationAsync(organisationName, moqService.Object);
            moqService.Verify();

            Assert.IsType<OkObjectResult>(result);
            var okObjectResult = (OkObjectResult)result;
            Assert.NotNull(okObjectResult);
            Assert.NotNull(okObjectResult.Value);
            var organisation = (DataContracts.Organisation)okObjectResult.Value;
            Assert.NotNull(organisation);
            Assert.Equal(organisationName, organisation.Name);            
        }
    }
}