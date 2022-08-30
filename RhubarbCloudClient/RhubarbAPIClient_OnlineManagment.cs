using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using RhubarbCloudClient.Model;

namespace RhubarbCloudClient
{
	public partial class RhubarbAPIClient : IDisposable
	{
		public bool _isOnline = false;
		public static bool Wasm { get; private set; } = false;

		public event Action HasGoneOfline;
		public event Action HasGoneOnline;

		public bool IsOnline
		{
			get => _isOnline;
			set {
				if (!value) {
					IsGoneOfline();
					HasGoneOfline?.Invoke();
				}
				_isOnline = value;
			}
		}

		private void IsGoneOfline() {
			LogOutPros();
		}

		private void IsGoneOnline() {
			GetMe().ConfigureAwait(false);
		}

		public static async Task<bool> CheckForInternetConnection(int timeoutMs = 10000, string url = null) {
			try {
				url ??= CultureInfo.InstalledUICulture switch { { Name: var n } when n.StartsWith("fa") => // Iran
						"http://www.aparat.com", { Name: var n } when n.StartsWith("zh") => // China
						"http://www.baidu.com",
					_ =>
						"http://www.gstatic.com/generate_204",
				};

				var request = (HttpWebRequest)WebRequest.Create(url);
				request.KeepAlive = false;
				request.Timeout = timeoutMs;
				using var response = await request.GetResponseAsync();
				return true;
			}
			catch(PlatformNotSupportedException e) {
				Console.WriteLine("Is On WASM");
				Wasm = true;
				return true;
			}
			catch {
				Console.WriteLine("Is Offline");
				return false;
			}
		}

		private void UpdateCheckForInternetConnection() {
			Task.Run(async () => {
				IsOnline = await CheckForInternetConnection();
				if (IsOnline) {
					IsGoneOnline();
					HasGoneOnline?.Invoke();
				}
			});
		}
	}
}
