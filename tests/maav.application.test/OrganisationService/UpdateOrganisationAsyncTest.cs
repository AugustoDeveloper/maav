using System;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Moq;
using MAAV.Application;
using MAAV.Application.Exceptions;
using MAAV.Domain.Entities;
using MAAV.Domain.Repositories;
using Xunit;
using MAAV.Application.Extensions;

namespace MAAV.Application.Test.OrganisationService
{
     public class UpdateOrganisationAsyncTest
     {
          public UpdateOrganisationAsyncTest()
          {
               AutoMapperExtension.AddMapping(null);
          }

          [Theory]
          [InlineData("apple")]
          [InlineData("microsoft")]
          [InlineData("ebay")]
          public async Task Given_OrganisationName_Not_Registered_When_Call_UpdateOrganisationAsync_ShouldReturns_Null(string organisationName)
          {
               var requestOrganisation = new DataContracts.Organisation { Name = organisationName };
               var moqRepository = new Mock<IOrganisationRepository>();
               moqRepository
                    .Setup(o => o.ExistsByAsync(It.IsAny<Expression<Func<Organisation, bool>>>()))
                    .Returns(Task.FromResult(false))
                    .Verifiable();

               var service = new Application.OrganisationService(moqRepository.Object);
               var organisation = await service.UpdateAsync(requestOrganisation);
               moqRepository.Verify(o => o.UpdateAsync(It.IsAny<Organisation>()), Times.Never);
               moqRepository.Verify();
               Assert.Null(organisation);
          }

          [Theory]
          [InlineData("apple")]
          [InlineData("microsoft")]
          [InlineData("ebay")]
          public async Task Given_OrganisationName_Registered_When_Call_UpdateOrganisationAsync_ShouldReturns_Updated_Organisation(string organisationName)
          {
               var requestOrganisation = new DataContracts.Organisation { Name = organisationName };
               var moqRepository = new Mock<IOrganisationRepository>();
               moqRepository
                    .Setup(o => o.ExistsByAsync(It.IsAny<Expression<Func<Organisation, bool>>>()))
                    .Returns(Task.FromResult(true))
                    .Verifiable();
               moqRepository
                    .Setup(o => o.UpdateAsync(It.IsAny<Organisation>()))
                    .ReturnsAsync(new Organisation { Name = organisationName})
                    .Verifiable();

               var service = new Application.OrganisationService(moqRepository.Object);
               var organisation = await service.UpdateAsync(requestOrganisation);
               moqRepository.VerifyAll();
               Assert.NotNull(organisation);
               Assert.Equal(requestOrganisation.Name, organisation.Name);
          }
     }
}