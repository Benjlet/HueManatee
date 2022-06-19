using HueManatee.Abstractions;
using HueManatee.Json;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
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
            var response = await _httpClient.GetAsync(uri);
            return await ParseResponse<T>(response);
        }

        public async Task<T> PostAsync<T>(string uri, object data)
        {
            var requestJson = data == null ? null : new StringContent(JsonConvert.SerializeObject(data));
            var response = await _httpClient.PostAsync(uri, requestJson);
            return await ParseResponse<T>(response);
        }

        public async Task<T> PutAsync<T>(string uri, object data)
        {
            var requestJson = data == null ? null : new StringContent(JsonConvert.SerializeObject(data));
            var response = await _httpClient.PutAsync(uri, requestJson);
            return await ParseResponse<T>(response);
        }

        private async Task<T> ParseResponse<T>(HttpResponseMessage message)
        {
            var responseJson = string.Empty;

            try
            {
                message.EnsureSuccessStatusCode();
                responseJson = await message?.Content?.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseJson);
            }
            catch (JsonException ex)
            {
                var isBridgeError = TryGetFirstError(responseJson, out var error);
                throw isBridgeError ? new JsonException($"Unexpected Bridge Error: ({error?.Type}) {error?.Description}") : ex;
            }
        }

        private bool TryGetFirstError(string json, out HueError error)
        {
            try
            {
                error = JsonConvert.DeserializeObject<List<HueErrors>>(json).FirstOrDefault()?.Error;
                return true;
            }
            catch
            {
                error = null;
                return false;
            }
        }
    }
}
