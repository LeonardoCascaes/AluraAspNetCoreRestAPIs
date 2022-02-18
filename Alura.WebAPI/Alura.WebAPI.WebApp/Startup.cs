using Alura.ListaLeitura.Seguranca;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Alura.WebAPI.WebApp.Formatters;
using Alura.ListaLeitura.HttpClients;
using System;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Alura.ListaLeitura.WebApp
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration config)
        {
            Configuration = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => {
                    options.LoginPath = "/Usuario/Login";
                });

            services.AddHttpClient<LivroApiClient>(options =>
            {
                options.BaseAddress = new Uri("http://localhost:6000/api/");
            });
            services.AddHttpClient<AuthApiClient>(options =>
            {
                options.BaseAddress = new Uri("http://localhost:5000/api/");
            });

            services.AddHttpContextAccessor();

            services.AddMvc(opt => {
                opt.OutputFormatters.Add(new LivroCsvFormatter());
            }).AddXmlSerializerFormatters();

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
