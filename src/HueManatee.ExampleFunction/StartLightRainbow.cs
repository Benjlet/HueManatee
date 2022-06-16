using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Threading.Tasks;

namespace HueManatee.ExampleFunction
{
    public class StartLightRainbow
    {
        private readonly BridgeClient _hueManateeClient;

        public StartLightRainbow(BridgeClient hueManateeClient)
        {
            _hueManateeClient = hueManateeClient;
        }

        [FunctionName("StartLightRainbow")]
        [ProducesResponseType(typeof(OkResult), 200)]
        [ProducesResponseType(typeof(BadRequestObjectResult), 400)]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "Lights/{id}/rainbow")] HttpRequest req, string id)
        {
            var userName = req.Headers["username"];

            if (string.IsNullOrWhiteSpace(userName))
                return new BadRequestObjectResult("Header 'username' required.");

            var response = await _hueManateeClient.StartColorLoop(userName, id);

            return new OkObjectResult(response);
        }
    }
}
