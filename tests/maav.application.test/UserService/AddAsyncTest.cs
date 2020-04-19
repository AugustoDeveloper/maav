
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
    public class AddAsyncTest
    {
        public AddAsyncTest()
        {
            AutoMapperExtension.AddMapping(null);
        }

        [Fact]
        public async Task Given_Exists_User_When_Call_AddAsync_ShouldReturns_NameAlreadyUseException()
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
            
            var service = new Application.UserService(moqRepository.Object, moqOrgRepository.Object);
            await Assert.ThrowsAsync<NameAlreadyUsedException>(() => service.AddAsync("", new DataContracts.User { Username = "development" }));
            moqRepository.Verify(t => t.AddAsync(It.IsAny<User>()), Times.Never);
            moqRepository.Verify();
        }

        [Theory]
        [InlineData("apple", "develop")]
        public async Task Given_Not_Exists_User_When_Call_AddAsync_ShouldReturns_A_New_Instance_Of_User(string organisationId, string userName)
        {
            var moqRepository = new Mock<IUserRepository>();
            moqRepository
                .Setup(t => t.ExistsByAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(false)
                .Verifiable();

            moqRepository
                .Setup(t => t.AddAsync(It.IsAny<User>()))
                .ReturnsAsync(new User { Username = userName, OrganisationId = organisationId })
                .Verifiable();

            var moqOrgRepository = new Mock<IOrganisationRepository>();
            moqOrgRepository
                .Setup(t => t.ExistsByAsync(It.IsAny<Expression<Func<Domain.Entities.Organisation, bool>>>()))
                .ReturnsAsync(true)
                .Verifiable();  
            
            var service = new Application.UserService(moqRepository.Object, moqOrgRepository.Object);
            var userResult = await service.AddAsync(organisationId, new DataContracts.User { Username = userName });
            moqRepository.VerifyAll();
            Assert.NotNull(userResult);
            Assert.Equal(userName, userResult.Username);
        }
    }
}