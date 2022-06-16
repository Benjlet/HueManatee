using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace HueManatee
{
    internal class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly HttpClient _httpClient;

        internal HttpClientWrapper(HttpClient client)
        {
            _httpClient = client;
        }

        public async Task<T> GetAsync<T>(string uri)
        {
            var getResponse = await _httpClient.GetAsync(uri);
            var responseJson = getResponse?.Content?.ReadAsStringAsync()?.Result;

            return JsonConvert.DeserializeObject<T>(responseJson);
        }

        public async Task<T> PostAsync<T>(string uri, object data)
        {
            var requestJson = data == null ? null : new StringContent(JsonConvert.SerializeObject(data));
            var postResponse = await _httpClient.PostAsync(uri, requestJson);
            var responseJson = postResponse?.Content?.ReadAsStringAsync()?.Result;

            return JsonConvert.DeserializeObject<T>(responseJson);
        }

        public async Task<T> PutAsync<T>(string uri, object data)
        {
            var requestJson = data == null ? null : new StringContent(JsonConvert.SerializeObject(data));
            var putResponse = await _httpClient.PutAsync(uri, requestJson);
            var responseJson = putResponse?.Content?.ReadAsStringAsync()?.Result;

            return JsonConvert.DeserializeObject<T>(responseJson);
        }
    }
}
