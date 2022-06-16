using System.Threading.Tasks;

namespace HueManatee
{
    internal interface IHttpClientWrapper
    {
        Task<T> GetAsync<T>(string uri);
        Task<T> PostAsync<T>(string uri, object package);
        Task<T> PutAsync<T>(string uri, object package);
    }
}
