using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Maui.LifecycleEvents;

using Rhubarb_Shared;

using RhubarbCloudClient;
using System.Net.Http;

namespace RhubarbVR
{
	public static class MauiProgram
	{
		public static void UpdateTheme(bool isLight) {
			Console.WriteLine("Theme Change");
			LightModeManager.IsLightMode = isLight;
			LightModeManager.Update();
		}

		public static MauiApp CreateMauiApp() {
			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
				.ConfigureFonts(fonts => fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"));

#if DEBUG
			var targetURI = new Uri("http://localhost:5000/");
#else
var targetURI = new Uri("https://api.rhubarbvr.net/");
#endif
			builder.Services.AddScoped<LightModeManager>();

			//TODO: Save Cookies
			builder.Services
				.AddScoped(sp => sp
					.GetRequiredService<IHttpClientFactory>()
					.CreateClient("API"))
				.AddHttpClient("API", client => client.BaseAddress = targetURI);
			builder.Services.AddScoped(ser => {
				ser.GetRequiredService<NavigationManager>().NavigateTo("/");
				ser.GetService<LightModeManager>();
				UpdateTheme(Application.Current.RequestedTheme == AppTheme.Light);
				Application.Current.RequestedThemeChanged += (e, a) => UpdateTheme(a.RequestedTheme == AppTheme.Light);
				var ret = new RhubarbAPIClient(ser.GetRequiredService<HttpClient>());
				ret.OnLogin += (user) => {
					ser.GetRequiredService<NavigationManager>().NavigateTo("/Client");
					Console.WriteLine($"Welcome: {user.UserName}");
				};
				ret.OnLogout += () => ser.GetRequiredService<NavigationManager>().NavigateTo("/Login");
				return ret;
			});

			builder.Services.AddMauiBlazorWebView();
#if DEBUG
			builder.Services.AddBlazorWebViewDeveloperTools();
#endif		
			return builder.Build();
		}
	}
}