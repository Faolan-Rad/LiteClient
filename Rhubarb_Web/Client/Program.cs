using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

using Rhubarb_Shared;

using Rhubarb_Web;
using RhubarbCloudClient;
using System.Net.Http;

namespace Rhubarb_Web
{
    public class Program
    {
        public class CookieHandler : DelegatingHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

                return await base.SendAsync(request, cancellationToken);
            }
        }

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");
#if DEBUG
            var targetURI = new Uri("http://localhost:5000/");
#else
var targetURI = new Uri("https://api.rhubarbvr.net/");
#endif
			builder.Services.AddScoped<LightModeManager>();

			builder.Services
                .AddTransient<CookieHandler>()
                .AddScoped(sp => sp
                    .GetRequiredService<IHttpClientFactory>()
                    .CreateClient("API"))
                .AddHttpClient("API", client => client.BaseAddress = targetURI).AddHttpMessageHandler<CookieHandler>();
            builder.Services.AddScoped(ser => {
                ser.GetRequiredService<NavigationManager>().NavigateTo("/");
				ser.GetService<LightModeManager>();
				var ret = new RhubarbAPIClient(ser.GetRequiredService<HttpClient>());
                ret.OnLogin += (user) => {
                    ser.GetRequiredService<NavigationManager>().NavigateTo("/Client");
                    Console.WriteLine($"Welcome: {user.UserName}");
                };
                ret.OnLogout += () => ser.GetRequiredService<NavigationManager>().NavigateTo("/Login");
                return ret;
            });

            await builder.Build().RunAsync();
        }
    }
}