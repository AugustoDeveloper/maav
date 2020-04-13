using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Flurl.Http;
using Xunit;

namespace MAAV.WebAPI.Integration.Test.Controllers
{
    public class AuthControllerTest
    {
        [Fact]
        public async Task Given_Username_And_Password_When_Send_Post_Request_Should_Returns_Token()
        {
            var response = await "http://localhost:5892/api/v1/microsoft/authenticate".PostJsonAsync(new 
            {
                username = "admin", 
                password = "123"
            }).ReceiveJson();

            Console.WriteLine(response.accessToken);
        } 
    }
}