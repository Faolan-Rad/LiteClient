﻿using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR.Client;

using Newtonsoft.Json;

using RhubarbCloudClient.Model;

namespace RhubarbCloudClient
{
	public partial class RhubarbAPIClient : IDisposable
	{
		public const string AUTHPATH = "auth/";

		public PrivateUser User { get; private set; }
		public bool IsLogin => User is not null;

		public event Action<PrivateUser> OnLogin;

		public event Action OnLogout;
		private void LogInPros(PrivateUser privateUser) {
			User = privateUser;
			if (IsLogin) {
				OnLogin?.Invoke(User);
				InitSignlR();
				LoadStartDM().ConfigureAwait(false);
			}
			else {
				LogOutPros();
			}
		}
		HubConnection _hub;
		private void InitSignlR() {
			try {
				_hub = new HubConnectionBuilder()
					.WithUrl(new Uri(HttpClient.BaseAddress, "hub"), (x) => x.Cookies = Cookies)
					.WithAutomaticReconnect()
					.Build();
			}
			catch {
				_hub = new HubConnectionBuilder()
					.WithUrl(new Uri(HttpClient.BaseAddress, "hub"))
					.WithAutomaticReconnect()
					.Build();
			}
			//_hub.On(nameof(MessageNotify), MessageNotify);
			_hub.On<UserDM.MSG>(nameof(ReceivedMsg), ReceivedMsg);
			_hub.On<PrivateUserStatus>(nameof(LoadInStatusInfo), LoadInStatusInfo);
			_hub.StartAsync().ConfigureAwait(false);
		}

		public async Task UpdateUserStatus(PrivateUserStatus status) {
			if (_hub is null) {
				return;
			}
			await _hub.InvokeAsync("SetStatus", status);
		}

		public string ClientCompatibility = "WEB";
		public PrivateUserStatus Status { get; private set; }

		public async Task UpdateStatus() {
			await UpdateUserStatus(Status);
		}

		private async Task LoadInStatusInfo(PrivateUserStatus status) {
			Console.WriteLine("User Status Loaded");
			Status = status;
			status.ClientVersion = Environment.Version.ToString();
			status.Device = Wasm ? "WASM" : Environment.OSVersion.ToString();
			status.ClientCompatibility = ClientCompatibility;
			await UpdateUserStatus(status);
		}

		private void LogOutPros() {
			_hub?.DisposeAsync();
			_hub = null;
			User = null;
			OnLogout?.Invoke();

		}

		private void ProccessUserLoadin(ServerResponse<PrivateUser> res) {
			if (res.Error) {
				LogOutPros();
			}
			else {
				LogInPros(res.Data);
			}
		}

		public async Task<HttpDataResponse<string>> RegisterAccount(RUserRegistration rUserRegistration) {
			return await SendPost(API_PATH + AUTHPATH + "Register", rUserRegistration);
		}

		public async Task<ServerResponse<PrivateUser>> Login(RAccountLogin rUserLogin) {
			var req = await SendPostServerResponses<PrivateUser, RAccountLogin>(API_PATH + AUTHPATH + "Login", rUserLogin);
			ProccessUserLoadin(req);
			return req;
		}

		public async Task GetMe() {
			var req = await SendGetServerResponses<PrivateUser>(API_PATH + AUTHPATH + "GetMe");
			ProccessUserLoadin(req);
		}

		public async Task LogOut() {
			await SendGet(API_PATH + AUTHPATH + "LogOut");
			LogOutPros();
		}
	}
}
