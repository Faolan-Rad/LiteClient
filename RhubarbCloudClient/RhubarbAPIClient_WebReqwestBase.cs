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
		public class HttpDataResponse<T>
		{
			public T Data { get; set; }

			public HttpResponseMessage HttpResponseMessage { get; set; }

			public async Task<string> RawData() {
				return await HttpResponseMessage.Content.ReadAsStringAsync();
			}

			public async static Task<HttpDataResponse<T>> Build(HttpResponseMessage httpResponseMessage) {
				var httpDataResponse = new HttpDataResponse<T> {
					HttpResponseMessage = httpResponseMessage,
					Data = JsonConvert.DeserializeObject<T>(await httpResponseMessage.Content.ReadAsStringAsync())
				};
				return httpDataResponse;
			}

			public async static Task<HttpDataResponse<string>> BuildString(HttpResponseMessage httpResponseMessage) {
				var httpDataResponse = new HttpDataResponse<string> {
					HttpResponseMessage = httpResponseMessage,
					Data = await httpResponseMessage.Content.ReadAsStringAsync()
				};
				return httpDataResponse;
			}
			public bool IsDataNull => Data is null;

			public bool IsDataGood => HttpResponseMessage?.IsSuccessStatusCode ?? false & Data is not null;
		}
		
		
		public async Task<HttpDataResponse<string>> SendPost<T>(string path, T value) {
			var httpContent = new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json");
			var request = await HttpClient.PostAsync(path, httpContent);
			return await HttpDataResponse<string>.BuildString(request);
		}
		public async Task<HttpDataResponse<R>> SendPost<R, T>(string path, T value) {
			var httpContent = new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json");
			var request = await HttpClient.PostAsync(path, httpContent);
			return await HttpDataResponse<R>.Build(request);
		}
		public async Task<HttpDataResponse<string>> SendGet(string path) {
			var request = await HttpClient.GetAsync(path);
			return await HttpDataResponse<string>.BuildString(request);
		}
		public async Task<HttpDataResponse<R>> SendGet<R>(string path) {
			var request = await HttpClient.GetAsync(path);
			return await HttpDataResponse<R>.Build(request);
		}

		public async Task<ServerResponse<T>> SendGetServerResponses<T>(string path) {
			var request = await HttpClient.GetAsync(path);
			var build = await HttpDataResponse<ServerResponse<T>>.Build(request);
			return build.IsDataNull ? new ServerResponse<T>(request.StatusCode.ToString()) : build.Data;
		}
		public async Task<ServerResponse<T>> SendPostServerResponses<T,P>(string path,P value) {
			var request = await SendPost<ServerResponse<T>,P>(path,value);
			return request.IsDataNull ? new ServerResponse<T>(request.HttpResponseMessage.StatusCode.ToString()) : request.Data;
		}
	}
}
