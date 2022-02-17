using Alura.ListaLeitura.Modelos;
using System.Net.Http;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.HttpClients
{
    public class LivroApiClient
    {
        private readonly HttpClient _httpClient;

        public LivroApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<LivroApi> GetLivroAsync(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"Livros/{id}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<LivroApi>();
        }

        internal async Task<byte[]> GetCapaLivroAsync(int id)
        {
            var response = await _httpClient.GetAsync($"Livros/{id}/capa");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}
