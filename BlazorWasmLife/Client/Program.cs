using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BlazorWasmLife.Shared;

namespace BlazorWasmLife.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            await LoadSettingsAsync(builder);

            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddTransient(sp =>
            {
                var configString = builder.Configuration["ConwayWebAPI"];
                var uri = new Uri(configString);
                var hc = new HttpClient
                {

                    BaseAddress = uri

                };
                hc.DefaultRequestHeaders.Add("Accept", "application/json");

                return new ConwayService(hc);
            });

            await builder.Build().RunAsync();
        }

        private static async Task LoadSettingsAsync(WebAssemblyHostBuilder builder)
        {
            // read JSON file as a stream for configuration
            var client = new HttpClient() { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
            // the appsettings file must be in 'wwwroot'
            using var response = await client.GetAsync("settings.json");
            using var stream = await response.Content.ReadAsStreamAsync();
            builder.Configuration.AddJsonStream(stream);
        }
    }
}
