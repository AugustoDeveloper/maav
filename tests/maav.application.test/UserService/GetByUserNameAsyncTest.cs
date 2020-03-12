using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using MAAV.Application.Extensions;
using MAAV.Domain.Repositories;
using Xunit;

namespace MAAV.Application.Test.UserService
{
    public class GetByUserNameAsyncTest
    {
        public GetByUserNameAsyncTest()
        {
            AutoMapperExtension.AddMapping(null);
        }

        [Theory]
        [InlineData("apple", "develop")]
        public async Task Given_Not_Registered_OrganisationName_Or_UserName_When_Call_GetByUserNameAsync_ShouldReturns_Null_Instance(string organisationName, string userName)
        {
            var moqRepository = new Mock<IUserRepository>();
            moqRepository
                .Setup(t => t.GetByAsync(It.IsAny<Expression<Func<Domain.Entities.User, bool>>>()))
                .ReturnsAsync(() => null)
                .Verifiable();

            var moqOrgRepository = new Mock<IOrganisationRepository>();
            moqOrgRepository
                .Setup(t => t.ExistsByAsync(It.IsAny<Expression<Func<Domain.Entities.Organisation, bool>>>()))
                .ReturnsAsync(true)
                .Verifiable();  
            
            var service = new Application.UserService(moqRepository.Object, moqOrgRepository.Object);
            Assert.Null(await service.GetByUsernameAsync(organisationName, userName));
            moqRepository.Verify();
        }

        [Theory]
        [InlineData("apple", "develop")]
        public async Task Given_Registered_OrganisationName_Or_UserName_When_Call_GetByUserNameAsync_ShouldReturns_User_Instance(string organisationName, string userName)
        {
            var moqRepository = new Mock<IUserRepository>();
            moqRepository
                .Setup(t => t.GetByAsync(It.IsAny<Expression<Func<Domain.Entities.User, bool>>>()))
                .ReturnsAsync(() => new Domain.Entities.User { Username = userName, OrganisationName = organisationName})
                .Verifiable();

            var moqOrgRepository = new Mock<IOrganisationRepository>();
            moqOrgRepository
                .Setup(t => t.ExistsByAsync(It.IsAny<Expression<Func<Domain.Entities.Organisation, bool>>>()))
                .ReturnsAsync(true)
                .Verifiable();  
            
            var service = new Application.UserService(moqRepository.Object, moqOrgRepository.Object);
            var userResult = await service.GetByUsernameAsync(organisationName, userName);
            moqRepository.Verify();
            Assert.NotNull(userResult);
            Assert.Equal(userName, userResult.Username);
        }
    }
}