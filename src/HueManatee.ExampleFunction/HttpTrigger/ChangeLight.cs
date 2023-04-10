using HueManatee.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
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

        [FunctionName("ChangeLight")]
        [ProducesResponseType(typeof(OkResult), 200)]
        [ProducesResponseType(typeof(BadRequestObjectResult), 400)]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "lights/{id}")] HttpRequest req, string id)
        {
            var requestJson = await req.ReadAsStringAsync();
            var request = JsonConvert.DeserializeObject<ChangeLightRequest>(requestJson);

            var response = await _hueManateeClient.ChangeLight(id, request);

            return new OkObjectResult(response);
        }
    }
}
