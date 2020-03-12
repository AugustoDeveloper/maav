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
     public class RegisterOrganisationAsyncTest
     {
          public RegisterOrganisationAsyncTest()
          {
               AutoMapperExtension.AddMapping(null);
          }

          [Theory]
          [InlineData("apple")]
          [InlineData("microsoft")]
          [InlineData("ebay")]
          public async Task Given_OrganisationName_Registered_When_Call_RegisterOrganisationAsync_ShouldReturns_NameAlreadyUsedException(string organisationName)
          {
              var requestOrganisation = new DataContracts.Organisation { Name = organisationName };
               var moqRepository = new Mock<IOrganisationRepository>();
               moqRepository
                    .Setup(o => o.ExistsByAsync(It.IsAny<Expression<Func<Organisation, bool>>>()))
                    .ReturnsAsync(true)
                    .Verifiable();

               var service = new Application.OrganisationService(moqRepository.Object);
               moqRepository.Verify(o => o.AddAsync(It.IsAny<Organisation>()), Times.Never);
               await Assert.ThrowsAsync<NameAlreadyUsedException>(() => service.RegisterAsync(requestOrganisation));               
          }

          [Theory]
          [InlineData("apple")]
          [InlineData("microsoft")]
          [InlineData("ebay")]
          public async Task Given_OrganisationName_Not_Registered_When_Call_RegisterOrganisationAsync_ShouldReturns_OrganisationInstance_With_ScheMap_Property_Null(string organisationName)
          {
              var requestOrganisation = new DataContracts.Organisation { Name = organisationName };
               var moqRepository = new Mock<IOrganisationRepository>();
               moqRepository
                    .Setup(o => o.ExistsByAsync(It.IsAny<Expression<Func<Organisation, bool>>>()))
                    .ReturnsAsync(false)
                    .Verifiable();
                moqRepository
                    .Setup(o => o.AddAsync(It.IsAny<Organisation>()))
                    .ReturnsAsync(new Organisation { Name = organisationName })
                    .Verifiable();

               var service = new Application.OrganisationService(moqRepository.Object);
               var newOrganisation = await service.RegisterAsync(requestOrganisation);
               moqRepository.VerifyAll();

               Assert.NotNull(newOrganisation);
               Assert.Null(newOrganisation.ScheMap);
               Assert.Equal(organisationName, newOrganisation.Name);
          }

          [Theory]
          [InlineData("apple")]
          [InlineData("microsoft")]
          [InlineData("ebay")]
          public async Task Given_OrganisationName_Not_Registered_When_Call_RegisterOrganisationAsync_ShouldReturns_OrganisationInstance_With_ScheMap_Property_Not_Null(string organisationName)
          {
              var requestOrganisation = new DataContracts.Organisation { Name = organisationName };
               var moqRepository = new Mock<IOrganisationRepository>();
               moqRepository
                    .Setup(o => o.ExistsByAsync(It.IsAny<Expression<Func<Organisation, bool>>>()))
                    .ReturnsAsync(false)
                    .Verifiable();
                moqRepository
                    .Setup(o => o.AddAsync(It.IsAny<Organisation>()))
                    .ReturnsAsync(new Organisation { Name = organisationName, ScheMap = new ScheMapVersion() })
                    .Verifiable();

               var service = new Application.OrganisationService(moqRepository.Object);
               var newOrganisation = await service.RegisterAsync(requestOrganisation);
               moqRepository.VerifyAll();

               Assert.NotNull(newOrganisation);
               Assert.NotNull(newOrganisation.ScheMap);
               Assert.Equal(organisationName, newOrganisation.Name);
          }
     }
}