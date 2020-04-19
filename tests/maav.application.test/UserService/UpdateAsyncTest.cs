using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using MAAV.Application.Exceptions;
using MAAV.Application.Extensions;
using MAAV.Domain.Entities;
using MAAV.Domain.Repositories;
using Xunit;

namespace MAAV.Application.Test.UserService
{
    public class UpdateAsyncTest
    {
        public UpdateAsyncTest()
        {
            AutoMapperExtension.AddMapping(null);
        }

        [Fact]
        public async Task Given_Not_Exists_User_When_Call_UpdateAsync_ShouldReturns_Null_Reference()
        {
            var moqRepository = new Mock<IUserRepository>();
            moqRepository
                .Setup(t => t.ExistsByAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(false)
                .Verifiable();

            var moqOrgRepository = new Mock<IOrganisationRepository>();
            moqOrgRepository
                .Setup(t => t.ExistsByAsync(It.IsAny<Expression<Func<Domain.Entities.Organisation, bool>>>()))
                .ReturnsAsync(true)
                .Verifiable();  
            
            var service = new Application.UserService(moqRepository.Object, moqOrgRepository.Object);
            var userResult = await service.UpdateAsync("", new DataContracts.User { Username = "development" });
            moqRepository.Verify(t => t.UpdateAsync(It.IsAny<User>()), Times.Never);
            moqRepository.Verify();
            Assert.Null(userResult);
        }

        [Theory]
        [InlineData("apple", "develop")]
        public async Task Given_Exists_User_When_Call_UpdateAsync_ShouldReturns_A_New_Instance_Of_User(string organisationId, string userName)
        {
            var moqRepository = new Mock<IUserRepository>();
            moqRepository
                .Setup(t => t.ExistsByAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(true)
                .Verifiable();
            
            var moqOrgRepository = new Mock<IOrganisationRepository>();
            moqOrgRepository
                .Setup(t => t.ExistsByAsync(It.IsAny<Expression<Func<Domain.Entities.Organisation, bool>>>()))
                .ReturnsAsync(true)
                .Verifiable();  

            moqRepository
                .Setup(t => t.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(new User { Username = userName, OrganisationId = organisationId })
                .Verifiable();
            
            var service = new Application.UserService(moqRepository.Object, moqOrgRepository.Object);
            var userResult = await service.UpdateAsync("organisationId", new DataContracts.User { Username = userName });
            moqRepository.VerifyAll();
            Assert.NotNull(userResult);
            Assert.Equal(userName, userResult.Username);
        }
    }
}