using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Threading.Tasks;

namespace HueManatee.ExampleFunction
{
    public class GetLights
    {
        private readonly HueManatee _hueManateeClient;

        public GetLights(HueManatee hueManateeClient)
        {
            _hueManateeClient = hueManateeClient;
        }

        [FunctionName("GetLights")]
        [ProducesResponseType(typeof(OkObjectResult), 200)]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Lights")] HttpRequest req)
        {
            var userName = req.Headers["username"];

            if (string.IsNullOrWhiteSpace(userName))
                return new BadRequestObjectResult("Header 'username' required.");

            var response = await _hueManateeClient.GetLights(userName);

            return new OkObjectResult(response);
        }
    }
}
