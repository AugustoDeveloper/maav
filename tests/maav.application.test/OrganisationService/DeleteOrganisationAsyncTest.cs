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
    public class DeleteByOrganisationNameAsyncTest
    {
        public DeleteByOrganisationNameAsyncTest()
        {
            AutoMapperExtension.AddMapping(null);
        }

        [Theory]
        [InlineData("apple")]
        [InlineData("microsoft")]
        [InlineData("ebay")]
        public async Task Given_OrganisationName_Registered_When_Call_DeleteByOrganisationNameAsync_ShouldReturns_Null(string organisationName)
        {
            var moqRepository = new Mock<IOrganisationRepository>();
            moqRepository
                .Setup(o => o.DeleteAsync(It.IsAny<Expression<Func<Organisation, bool>>>()))
                .Returns(Task.FromResult(false))
                .Verifiable();

            var service = new Application.OrganisationService(moqRepository.Object);
            await service.DeleteByOrganisationNameAsync(organisationName);
            moqRepository.Verify();
          }
     }
}