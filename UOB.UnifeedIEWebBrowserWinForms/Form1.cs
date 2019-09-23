namespace UOL.UnifeedIEWebBrowserWinForms
{
	using System;
	using System.Collections.Specialized;
	using System.Web;
	using System.Windows.Forms;
	using Newtonsoft.Json;

	public partial class Form1 : Form
	{
		public const string ClientId = "2BA_DEMOAPPS_PKCE";
		public const string AuthorizeBaseUrl = "https://authorize-uol.2ba.nl";
		public const string UnifeedBaseUrl = "https://unifeed-uol-os.beta.2ba.nl";
		public const string UnifeedSchemeName = "nl.2ba.uol";
		public static readonly string AuthorizeUrl = $"{AuthorizeBaseUrl}/OAuth/Authorize";
		public static readonly string AuthorizeTokenUrl = $"{AuthorizeBaseUrl}/OAuth/Token";
		public static readonly string AuthorizeListenerAddress = $"http://localhost:43215/"; // Must end with slash
		public static readonly string AuthorizeHookUrl = $"{AuthorizeListenerAddress}tokenreceiver"; // = Redirect_uri as configured for client
		public static readonly string UnifeedHookUrl = $"{UnifeedSchemeName}://request";

		private SharedCode.Authentication.OAuthToken _currentToken = null;

		public Form1()
		{
			InitializeComponent();
		}

		private async void Form1_Load(object sender, EventArgs e)
		{
			Log($"Form loaded. starting authenticate");
			var authService = new SharedCode.Authentication.Authentication(new SharedCode.Authentication.AuthenticationConfig()
			{
				AuthorizeUrl = AuthorizeUrl,
				AuthorizeHookUrl = AuthorizeHookUrl,
				AuthorizeListenerAddress = AuthorizeListenerAddress,
				AuthorizeTokenUrl = AuthorizeTokenUrl,
				ClientId = ClientId,
				RequestedScope = "unifeed openid offline_access",
			});

			_currentToken = await authService.Authenticate();

			Log($"Authentication complete, starting Unifeed");
			StartUnifeed();
		}

		private void StartUnifeed()
		{
			var accessToken = _currentToken.AccessToken;
			var url = SharedCode.Web.HttpExtensions.Build(UnifeedBaseUrl, new NameValueCollection()
			{
				{ "accessToken", accessToken },
				{ "interface", (32 | 4).ToString() },
				{ "interfaceType", "JSONGET" },
				{ "hookUrl", UnifeedHookUrl },
			}).ToString();
			browser.Navigate(url);
			return;
		}

		private void Browser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
		{
			Log($"Navigating to: {e.Url}");
			if (e.Url.Scheme == UnifeedSchemeName)
			{
				e.Cancel = true;

				var queryString = HttpUtility.ParseQueryString(e.Url.Query);

				string data = queryString["json"];

				Log($"Interfaced! {e.Url}. Retrieved data: {data}");

				if (!string.IsNullOrEmpty(_currentToken.RefreshToken))
				{
					// Refresh token. Normally this is not needed for every call, only when the token is expired.
					// Only possible when offline_access scope is honored
					RefreshToken();
				}

				// Restart Unifeed
				StartUnifeed(); // browser.Refresh();
			}
		}

		private void Browser_Navigated(object sender, System.Windows.Forms.WebBrowserNavigatedEventArgs e)
		{
			// Log($"Navigated to: {e.Url}");
		}

		private void Log(string tekst)
		{
			logBox.Text += $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {tekst} {Environment.NewLine}";
			System.Diagnostics.Debug.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {tekst} {Environment.NewLine}");
		}

		private void RefreshToken()
		{
			Log($"Refreshing tokens. Old token retrieved: {_currentToken.TokenIssued.ToString("yyyyMMdd_HHmmss")}");
			var query = SharedCode.Web.HttpExtensions.BuildQuerystring(new NameValueCollection()
				{
					{ "client_id", ClientId },
					{ "grant_type", "refresh_token" },
					{ "refresh_token", _currentToken.RefreshToken },
				}).ToString();

			_currentToken = SharedCode.Authentication.TokenService.Get(AuthorizeTokenUrl, query);

			Log($"Refreshing tokens. New token retrieved: {_currentToken.TokenIssued.ToString("yyyyMMdd_HHmmss")}");
		}
	}
}
