using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Alura.ListaLeitura.HttpClients;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Alura.ListaLeitura.WebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly LivroApiClient _livroApiClient;

        public HomeController(LivroApiClient livroApiClient)
        {
            _livroApiClient = livroApiClient;
        }

        private async Task<IEnumerable<LivroApi>> ListaDoTipo(TipoListaLeitura tipo)
        {
            var list = await _livroApiClient.GetListaLeituraAsync(tipo);
            return list.Livros;
        }

        public async Task<IActionResult> Index()
        {
            var token = HttpContext.User.Claims.First(c => c.Type == "Token").Value;
            Console.WriteLine($"Token: {token}");
            
            var model = new HomeViewModel
            {
                ParaLer = await ListaDoTipo(TipoListaLeitura.ParaLer),
                Lendo = await ListaDoTipo(TipoListaLeitura.Lendo),
                Lidos = await ListaDoTipo(TipoListaLeitura.Lidos)
            };
            return View(model);
        }
    }
}