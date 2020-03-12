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

namespace MAAV.Application.Test.UserService
{
    public class DeleteAsyncTest
    {
        public DeleteAsyncTest()
        {
            AutoMapperExtension.AddMapping(null);
        }

        [Theory]
        [InlineData("apple", "develop")]
        public async Task Given_User_Registered_When_Call_DeleteAsync_ShouldReturns_TaskCompleted(string organisationName, string userName)
        {
            var moqRepository = new Mock<IUserRepository>();
            moqRepository
                .Setup(t => t.DeleteAsync(It.IsAny<Expression<Func<Domain.Entities.User, bool>>>()))
                .ReturnsAsync(true)
                .Verifiable();

            var moqOrgRepository = new Mock<IOrganisationRepository>();
            moqOrgRepository
                .Setup(t => t.ExistsByAsync(It.IsAny<Expression<Func<Domain.Entities.Organisation, bool>>>()))
                .ReturnsAsync(true)
                .Verifiable();  
            
            var service = new Application.UserService(moqRepository.Object, moqOrgRepository.Object);
            await service.DeleteAsync(organisationName, userName);
            moqRepository.VerifyAll();
          }
     }
}