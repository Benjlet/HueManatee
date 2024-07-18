using HueManatee.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HueManatee.ExampleFunction.HttpTrigger
{
    public class GetGroups
    {
        private readonly IBridgeClient _hueManateeClient;

        public GetGroups(IBridgeClient hueManateeClient)
        {
            _hueManateeClient = hueManateeClient;
        }

        [Function("GetGroups")]
        [ProducesResponseType(typeof(OkObjectResult), 200)]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "groups")] HttpRequestData req)
        {
            IEnumerable<Group> response = await _hueManateeClient.GetGroupData();
            return new OkObjectResult(response);
        }
    }
}
