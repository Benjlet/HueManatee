using HueManatee.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using System.Threading.Tasks;

namespace HueManatee.ExampleFunction
{
    public class GetGroup
    {
        private readonly IBridgeClient _hueManateeClient;

        public GetGroup(IBridgeClient hueManateeClient)
        {
            _hueManateeClient = hueManateeClient;
        }

        [Function("GetGroup")]
        [ProducesResponseType(typeof(OkObjectResult), 200)]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "groups/{id}")] HttpRequestData req, string id)
        {
            Group response = await _hueManateeClient.GetGroupData(id);
            return new OkObjectResult(response);
        }
    }
}
