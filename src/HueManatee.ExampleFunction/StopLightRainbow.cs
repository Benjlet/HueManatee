using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Threading.Tasks;

namespace HueManatee.ExampleFunction
{
    public class StopLightRainbow
    {
        private readonly BridgeClient _hueManateeClient;

        public StopLightRainbow(BridgeClient hueManateeClient)
        {
            _hueManateeClient = hueManateeClient;
        }

        [FunctionName("StopLightRainbow")]
        [ProducesResponseType(typeof(OkResult), 200)]
        [ProducesResponseType(typeof(BadRequestObjectResult), 400)]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "Lights/{id}/rainbow")] HttpRequest req, string id)
        {
            var userName = req.Headers["username"];

            if (string.IsNullOrWhiteSpace(userName))
                return new BadRequestObjectResult("Header 'username' required.");

            var response = await _hueManateeClient.StopColorLoop(userName, id);

            return new OkObjectResult(response);
        }
    }
}
