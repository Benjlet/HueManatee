using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace HueManatee.ExampleFunction
{
    public class ChangeLightState
    {
        private readonly HueManatee _hueManateeClient;

        public ChangeLightState(HueManatee hueManateeClient)
        {
            _hueManateeClient = hueManateeClient;
        }

        [FunctionName("ChangeLightState")]
        [ProducesResponseType(typeof(OkResult), 200)]
        [ProducesResponseType(typeof(BadRequestObjectResult), 400)]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "Lights/{id}")] HttpRequest req, string id)
        {
            var userName = req.Headers["username"];

            if (string.IsNullOrWhiteSpace(userName))
                return new BadRequestObjectResult("Header 'username' required.");

            var requestJson = await req.ReadAsStringAsync();
            var request = JsonConvert.DeserializeObject<LightStateRequest>(requestJson);

            var response = await _hueManateeClient.ChangeLightState(userName, id, request);

            return new OkObjectResult(response);
        }
    }
}
