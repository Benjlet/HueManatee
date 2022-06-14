using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace HueManatee
{
    internal class HueManateeClient
    {
        private readonly HttpClient _httpClient;

        internal HueManateeClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        internal async Task<T> Post<T>(string endpoint, object data = null)
        {
            var requestJson = data == null ? null : new StringContent(JsonConvert.SerializeObject(data));
            var postResponse = await _httpClient.PostAsync(endpoint, requestJson);
            var responseJson = postResponse?.Content?.ReadAsStringAsync()?.Result;

            return JsonConvert.DeserializeObject<T>(responseJson);
        }

        internal async Task<T> Get<T>(string endpoint)
        {
            var getResponse = await _httpClient.GetAsync(endpoint);
            var responseJson = getResponse?.Content?.ReadAsStringAsync()?.Result;

            return JsonConvert.DeserializeObject<T>(responseJson);
        }

        internal async Task<T> Put<T>(string endpoint, object data = null)
        {
            var requestJson = data == null ? null : new StringContent(JsonConvert.SerializeObject(data));
            var putResponse = await _httpClient.PutAsync(endpoint, requestJson);
            var responseJson = putResponse?.Content?.ReadAsStringAsync()?.Result;

            return JsonConvert.DeserializeObject<T>(responseJson);
        }
    }
}
