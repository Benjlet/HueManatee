using HueManatee.Request;
using HueManatee.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Threading.Tasks;

namespace HueManatee.ExampleFunction
{
    public class Register
    {
        private readonly IBridgeRegistrationClient _hueManateeClient;

        public Register(IBridgeRegistrationClient hueManateeClient)
        {
            _hueManateeClient = hueManateeClient;
        }

        [Function("Register")]
        [ProducesResponseType(typeof(RegisterResponse), 200)]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "register")] HttpRequestData req)
        {
            RegisterRequest request = await req.ReadFromJsonAsync<RegisterRequest>();
            RegisterResponse response = await _hueManateeClient.Register(request);

            return new OkObjectResult(response);
        }
    }
}
