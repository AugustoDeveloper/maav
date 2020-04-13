using Xunit;
using Flurl;
using Flurl.Http;
using System.Threading.Tasks;

namespace MAAV.WebAPI.Integration.Test.Controllers
{
    public class OrganisationControllerTest
    {
        [Fact(Skip = "Ja foi executado")]
        public async Task Given_New_Organisation_With_Admin_Users_When_Send_Post_ShouldReturns_Organisation_Object()
        {
            var response = await "http://localhost:5892/api/v1/microsoft".PostJsonAsync(new 
            {
                name = "microsoft",
                adminUsers = new []
                { 
                    new { firstName = "admin", lastName = "admin", username = "admin", password = "123" }
                }
            });

            response.EnsureSuccessStatusCode();
        }
    }
}