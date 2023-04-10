using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
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

        [FunctionName("GetGroup")]
        [ProducesResponseType(typeof(OkObjectResult), 200)]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "groups/{id}")] HttpRequest req, string id)
        {
            var response = await _hueManateeClient.GetGroupData(id);
            return new OkObjectResult(response);
        }
    }
}
