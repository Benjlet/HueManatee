using HueManatee.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace HueManatee.ExampleFunction
{
    public class ChangeGroup
    {
        private readonly IBridgeClient _hueManateeClient;

        public ChangeGroup(IBridgeClient hueManateeClient)
        {
            _hueManateeClient = hueManateeClient;
        }

        [FunctionName("ChangeGroup")]
        [ProducesResponseType(typeof(OkResult), 200)]
        [ProducesResponseType(typeof(BadRequestObjectResult), 400)]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "groups/{id}")] HttpRequest req, string id)
        {
            var requestJson = await req.ReadAsStringAsync();
            var request = JsonConvert.DeserializeObject<ChangeLightRequest>(requestJson);

            var response = await _hueManateeClient.ChangeGroup(id, request);

            return new OkObjectResult(response);
        }
    }
}
