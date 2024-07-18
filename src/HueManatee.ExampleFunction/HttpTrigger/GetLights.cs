using HueManatee.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HueManatee.ExampleFunction
{
    public class GetLights
    {
        private readonly IBridgeClient _hueManateeClient;

        public GetLights(IBridgeClient hueManateeClient)
        {
            _hueManateeClient = hueManateeClient;
        }

        [Function("GetLights")]
        [ProducesResponseType(typeof(OkObjectResult), 200)]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "lights")] HttpRequestData req)
        {
            IEnumerable<Light> lights = await _hueManateeClient.GetLightData();

            return new OkObjectResult(lights);
        }
    }
}
