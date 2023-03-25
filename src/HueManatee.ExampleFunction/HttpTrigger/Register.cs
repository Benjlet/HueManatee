using HueManatee.Request;
using HueManatee.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace HueManatee.ExampleFunction
{
    public class Register
    {
        private readonly BridgeRegistrationClient _hueManateeClient;

        public Register(BridgeRegistrationClient hueManateeClient)
        {
            _hueManateeClient = hueManateeClient;
        }

        [FunctionName("Register")]
        [ProducesResponseType(typeof(RegisterResponse), 200)]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "register")] HttpRequest req)
        {
            var requestJson = await req.ReadAsStringAsync();
            var request = JsonConvert.DeserializeObject<RegisterRequest>(requestJson);

            var response = await _hueManateeClient.Register(request);

            return new OkObjectResult(response);
        }
    }
}
