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
     public class GetByOrganisationNameAsyncTest
     {
          public GetByOrganisationNameAsyncTest()
          {
               AutoMapperExtension.AddMapping(null);
          }

          [Theory]
          [InlineData("apple")]
          public async Task Given_OrganisationName_Not_Registered_When_Call_GetByOrganisationNameAsync_ShouldReturns_Null(string organisationName)
          {
               var moqRepository = new Mock<IOrganisationRepository>();
               moqRepository
                    .Setup(o => o.GetByAsync(It.IsAny<Expression<Func<Organisation, bool>>>()))
                    .Verifiable();

               var service = new Application.OrganisationService(moqRepository.Object);
               var organisation  = await service.GetByOrganisationNameAsync(organisationName);
               moqRepository.Verify();
               Assert.Null(organisation);
          }

          [Theory]
          [InlineData("apple")]
          [InlineData("amazon")]
          [InlineData("ebay")]
          public async Task Given_OrganisationName_Registered_When_Call_GetByOrganisationNameAsync_ShouldReturns_Organisation_Instance(string organisationName)
          {
               var moqRepository = new Mock<IOrganisationRepository>();
               moqRepository
                    .Setup(o => o.GetByAsync(It.IsAny<Expression<Func<Organisation, bool>>>()))
                    .ReturnsAsync(new Organisation{ Name = organisationName })
                    .Verifiable();

               var service = new Application.OrganisationService(moqRepository.Object);
               var organisation  = await service.GetByOrganisationNameAsync(organisationName);
               moqRepository.Verify();
               Assert.NotNull(organisation);
               Assert.NotNull(organisation.Name);
               Assert.Equal(organisationName, organisation.Name);
          }
     }
}