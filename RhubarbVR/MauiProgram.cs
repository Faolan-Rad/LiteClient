using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.LifecycleEvents;

using Rhubarb_Shared;

using RhubarbCloudClient;
using System.Net.Http;
using System.Reflection;

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
			var targetURI = RhubarbAPIClient.LocalUri;
#else
			var targetURI = new Uri("https://api.rhubarbvr.net/");
#endif
			LightModeManager.UseWeb = false;
			builder.Services.AddScoped<LightModeManager>();

			//TODO: Save Cookies
			builder.Services
#if ANDROID
				.AddScoped(sp => {
					var client = new HttpClient(new Xamarin.Android.Net.AndroidMessageHandler());
					client.BaseAddress = targetURI;
					return client;
				});
#else
				.AddScoped(sp => {
					var client = new HttpClient();
					client.BaseAddress = targetURI;
					return client;
				});
#endif
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
				ret.HasGoneOfline+=()=> ser.GetRequiredService<NavigationManager>().NavigateTo("/");
				return ret;
			});
			static IEnumerable<string> GetFiles() {
				return Assembly.GetAssembly(typeof(LightModeManager))?.GetManifestResourceNames() ?? Array.Empty<string>();
			}
			builder.Services.AddScoped<Localisation>((thing) => new DynamicLocalisation(GetFiles, (item) => new StreamReader(Assembly.GetAssembly(typeof(LightModeManager)).GetManifestResourceStream(item)).ReadToEnd(), () => {

			}));

			builder.Services.AddMauiBlazorWebView();
#if DEBUG
			builder.Services.AddBlazorWebViewDeveloperTools();
#endif		
			return builder.Build();
		}
	}
}