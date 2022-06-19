using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Threading.Tasks;

namespace HueManatee.ExampleFunction.HttpTrigger
{
    public class GetGroups
    {
        private readonly BridgeClient _hueManateeClient;

        public GetGroups(BridgeClient hueManateeClient)
        {
            _hueManateeClient = hueManateeClient;
        }

        [FunctionName("GetGroups")]
        [ProducesResponseType(typeof(OkObjectResult), 200)]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "groups")] HttpRequest req)
        {
            var response = await _hueManateeClient.GetGroupData();
            return new OkObjectResult(response);
        }
    }
}
