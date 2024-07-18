using HueManatee.Request;
using HueManatee.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Threading.Tasks;

namespace HueManatee.ExampleFunction
{
    public class ChangeLight
    {
        private readonly IBridgeClient _hueManateeClient;

        public ChangeLight(IBridgeClient hueManateeClient)
        {
            _hueManateeClient = hueManateeClient;
        }

        [Function("ChangeLight")]
        [ProducesResponseType(typeof(OkResult), 200)]
        [ProducesResponseType(typeof(BadRequestObjectResult), 400)]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "lights/{id}")] HttpRequestData req, string id)
        {
            ChangeLightRequest request = await req.ReadFromJsonAsync<ChangeLightRequest>();
            ChangeLightResponse response = await _hueManateeClient.ChangeLight(id, request);

            return new OkObjectResult(response);
        }
    }
}
