using HueManatee.Abstractions;
using HueManatee.Exceptions;
using HueManatee.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace HueManatee
{
    internal class BridgeClientHttpWrapper : IHttpClientWrapper
    {
        private readonly HttpClient _httpClient;

        internal BridgeClientHttpWrapper(HttpClient client)
        {
            _httpClient = client;
        }

        public async Task<T> GetAsync<T>(string uri)
        {
            HttpResponseMessage response;

            try
            {
                response = await _httpClient.GetAsync(uri).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new BridgeClientException($"Error sending GET request to Hue Bridge: {ex.Message}", ex);
            }

            return await ParseResponse<T>(response);
        }

        public async Task<T> PostAsync<T>(string uri, object data)
        {
            HttpResponseMessage response;

            try
            {
                var requestJson = data == null ? null : new StringContent(JsonConvert.SerializeObject(data));
                response = await _httpClient.PostAsync(uri, requestJson).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new BridgeClientException($"Error sending POST request to Hue Bridge: {ex.Message}", ex);
            }

            return await ParseResponse<T>(response);
        }

        public async Task<T> PutAsync<T>(string uri, object data)
        {
            HttpResponseMessage response;

            try
            {
                var requestJson = data == null ? null : new StringContent(JsonConvert.SerializeObject(data));
                response = await _httpClient.PutAsync(uri, requestJson).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new BridgeClientException($"Error sending PUT request to Hue Bridge: {ex.Message}", ex);
            }

            return await ParseResponse<T>(response);
        }

        private async Task<T> ParseResponse<T>(HttpResponseMessage message)
        {
            var responseJson = string.Empty;

            try
            {
                responseJson = await message.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<T>(responseJson);
            }
            catch (Exception ex)
            {
                var (isBridgeError, errors) = GetErrors(responseJson);

                if (isBridgeError)
                {
                    var exceptionMessage = $"Unexpected Bridge Error{(errors.Count > 1 ? "s" : "")}:";

                    for (int i = 0; i < errors.Count; i++)
                    {
                        exceptionMessage += $" [{i + 1}]: {errors[i]?.Description} - Error Code {errors[i]?.Type}.";
                    }

                    throw new BridgeClientException(exceptionMessage);
                }

                throw new BridgeClientException($"Error parsing Hue Bridge response: {ex.Message}")
                {
                    ResponseJson = responseJson
                };
            }
        }

        private Tuple<bool, List<HueError>> GetErrors(string json)
        {
            try
            {
                var errors = JsonConvert.DeserializeObject<List<HueErrors>>(json);

                return new Tuple<bool, List<HueError>>(true, errors?.Select(e => new HueError()
                {
                    Address = e?.Error?.Address,
                    Description = e?.Error?.Description,
                    Type = e?.Error?.Type ?? 0
                })?.ToList());
            }
            catch (Exception)
            {
                // Ignore - non-Hue error.
            }

            return new Tuple<bool, List<HueError>>(false, null);
        }
    }
}
