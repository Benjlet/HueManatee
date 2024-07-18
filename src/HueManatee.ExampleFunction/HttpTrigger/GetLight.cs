using HueManatee.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Threading.Tasks;

namespace HueManatee.ExampleFunction
{
    public class GetLight
    {
        private readonly IBridgeClient _hueManateeClient;

        public GetLight(IBridgeClient hueManateeClient)
        {
            _hueManateeClient = hueManateeClient;
        }

        [Function("GetLight")]
        [ProducesResponseType(typeof(OkObjectResult), 200)]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "lights/{id}")] HttpRequestData req, string id)
        {
            Light response = await _hueManateeClient.GetLightData(id);
            return new OkObjectResult(response);
        }
    }
}
