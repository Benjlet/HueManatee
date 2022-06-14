using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Threading.Tasks;

namespace HueManatee.ExampleFunction
{
    public class GetLight
    {
        private readonly HueManatee _hueManateeClient;

        public GetLight(HueManatee hueManateeClient)
        {
            _hueManateeClient = hueManateeClient;
        }

        [FunctionName("GetLight")]
        [ProducesResponseType(typeof(OkObjectResult), 200)]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Lights/{id}")] HttpRequest req, string id)
        {
            var userName = req.Headers["username"];

            if (string.IsNullOrWhiteSpace(userName))
                return new BadRequestObjectResult("Header 'username' required.");

            var response = await _hueManateeClient.GetLightData(userName, id);

            return new OkObjectResult(response);
        }
    }
}
