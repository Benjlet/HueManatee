using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Threading.Tasks;

namespace HueManatee.ExampleFunction
{
    public class GetLights
    {
        private readonly BridgeClient _hueManateeClient;

        public GetLights(BridgeClient hueManateeClient)
        {
            _hueManateeClient = hueManateeClient;
        }

        [FunctionName("GetLights")]
        [ProducesResponseType(typeof(OkObjectResult), 200)]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "lights")] HttpRequest req)
        {
            var response = await _hueManateeClient.GetLightData();
            return new OkObjectResult(response);
        }
    }
}
