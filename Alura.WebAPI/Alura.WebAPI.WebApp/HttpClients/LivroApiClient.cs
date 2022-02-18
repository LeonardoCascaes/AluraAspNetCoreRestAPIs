using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Seguranca;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Lista = Alura.ListaLeitura.Modelos.ListaLeitura;

namespace Alura.ListaLeitura.HttpClients
{
    public class LivroApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly AuthApiClient _authApiClient;
        private readonly IHttpContextAccessor _accessor;

        public LivroApiClient(HttpClient httpClient, AuthApiClient authApiClient, IHttpContextAccessor accessor)
        {
            _httpClient = httpClient;
            _authApiClient = authApiClient;
            _accessor = accessor;
        }

        public async Task PostLivroAsync(LivroUpload model)
        {
            AddBearertoken();
            HttpContent content = CreateMultipartFormDataContent(model);
            var response = await _httpClient.PostAsync($"Livros", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task<Lista> GetListaLeituraAsync(TipoListaLeitura tipo)
        {
            AddBearertoken();
            var response = await _httpClient.GetAsync($"ListaLeitura/{tipo}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<Lista>();
        }

        public async Task<LivroApi> GetLivroAsync(int id)
        {
            AddBearertoken();
            HttpResponseMessage response = await _httpClient.GetAsync($"Livros/{id}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<LivroApi>();
        }

        public async Task<byte[]> GetCapaLivroAsync(int id)
        {
            AddBearertoken();
            var response = await _httpClient.GetAsync($"Livros/{id}/capa");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task PutLivroAsync(LivroUpload model)
        {
            AddBearertoken();
            HttpContent content = CreateMultipartFormDataContent(model);
            var response = await _httpClient.PutAsync($"Livros", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteLivroAsync(int id)
        {
            AddBearertoken();
            var response = await _httpClient.DeleteAsync($"Livros/{id}");
            response.EnsureSuccessStatusCode();
        }

        #region Private
        private HttpContent CreateMultipartFormDataContent(LivroUpload model)
        {
            var content = new MultipartFormDataContent
            {
                { new StringContent(model.Titulo), EnvolverComAspasDupla("titulo") },
                { new StringContent(model.Lista.ParaString()), EnvolverComAspasDupla("lista") },
            };

            if (!string.IsNullOrEmpty(model.Subtitulo))
            {
                content.Add(new StringContent(model.Subtitulo), EnvolverComAspasDupla("subtitulo"));
            }

            if (!string.IsNullOrEmpty(model.Resumo))
            {
                content.Add(new StringContent(model.Resumo), EnvolverComAspasDupla("resumo"));
            }

            if (!string.IsNullOrEmpty(model.Autor))
            {
                content.Add(new StringContent(model.Autor), EnvolverComAspasDupla("autor"));
            }

            if (model.Id > 0)
            {
                content.Add(new StringContent(model.Id.ToString()), EnvolverComAspasDupla("id"));
            }

            if (model.Capa != null)
            {
                var imagemContent = new ByteArrayContent(model.Capa.ConvertToBytes());
                imagemContent.Headers.Add("content-type", "image/png");
                content.Add(imagemContent, EnvolverComAspasDupla("capa"), EnvolverComAspasDupla("capa.png"));
            }


            return content;
        }

        private string EnvolverComAspasDupla(string valor)
        {
            return $"\"{valor}\"";
        }

        private void AddBearertoken()
        {
            var token = _accessor.HttpContext.User.Claims.First(c => c.Type == "Token").Value;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        }
        #endregion
    }
}
