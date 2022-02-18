using Alura.ListaLeitura.Modelos;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Lista = Alura.ListaLeitura.Modelos.ListaLeitura;

namespace Alura.ListaLeitura.HttpClients
{
    public class LivroApiClient
    {
        private readonly HttpClient _httpClient;

        public LivroApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task PostLivroAsync(LivroUpload model)
        {
            HttpContent content = CreateMultipartFormDataContent(model);
            var response = await _httpClient.PostAsync($"Livros", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task<Lista> GetListaLeituraAsync(TipoListaLeitura tipo)
        {
            var response = await _httpClient.GetAsync($"ListaLeitura/{tipo}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<Lista>();
        }

        public async Task<LivroApi> GetLivroAsync(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"Livros/{id}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<LivroApi>();
        }

        public async Task<byte[]> GetCapaLivroAsync(int id)
        {
            var response = await _httpClient.GetAsync($"Livros/{id}/capa");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task PutLivroAsync(LivroUpload model)
        {
            HttpContent content = CreateMultipartFormDataContent(model);
            var response = await _httpClient.PutAsync($"Livros", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteLivroAsync(int id)
        {
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

        #endregion
    }
}
