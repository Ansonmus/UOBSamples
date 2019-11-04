#define BETA
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

#if BETA
		public const string AuthorizeBaseUrl = "https://uol-auth.beta.2ba.nl";
		public const string UnifeedBaseUrl = "https://uol-unifeed.beta.2ba.nl";
		public const string ApiBaseUrl = "https://uol-api.beta.2ba.nl/1";
#else
		public const string AuthorizeBaseUrl = "https://uol-auth.2ba.nl";
		public const string UnifeedBaseUrl = "https://uol-unifeed.2ba.nl";
		public const string ApiBaseUrl = "https://uol-api.2ba.nl/1";
#endif

		public const string UnifeedSchemeName = "nl.2ba.uol";
		public static readonly string AuthorizeUrl = $"{AuthorizeBaseUrl}/OAuth/Authorize";
		public static readonly string AuthorizeTokenUrl = $"{AuthorizeBaseUrl}/OAuth/Token";
		public static readonly string AuthorizeListenerAddress = $"http://localhost:43215/"; // Must end with slash
		public static readonly string AuthorizeHookUrl = $"{AuthorizeListenerAddress}tokenreceiver"; // = Redirect_uri as configured for client
		public static readonly string UnifeedHookUrl = $"{UnifeedSchemeName}://request";

		private SharedCode.Authentication.OAuthToken _currentToken = null;

		private SharedCode.Models.Interface _lastRetrievedObject = null;

		public Form1()
		{
			InitializeComponent();

			this.browser.Navigating += Browser_Navigating;
			this.browser.Navigated += Browser_Navigated;

			browser.ScriptErrorsSuppressed = false;
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

		private void StartUnifeed(long? interfaceObjectId = null)
		{
			var accessToken = _currentToken.AccessToken;

			var url = SharedCode.Web.HttpExtensions.Build(UnifeedBaseUrl, new NameValueCollection()
			{
				{ "accessToken", accessToken },
				{ "interface", 32.ToString() },
				{ "interfaceType", "JSONGET" },
				{ "interfaceName", "UOB DemoApplication" },
				{ "hookUrl", UnifeedHookUrl },
				{ "interfaceObjectId", interfaceObjectId?.ToString() },
			}).ToString();

			Log($"Starting Unifeed with url: {url}");

			browser.Navigate(url);
			return;
		}

		private async void Browser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
		{
			// Log($"Navigating to: {e.Url}");
			if (e.Url.Scheme == UnifeedSchemeName)
			{
				e.Cancel = true;
				Log($"Interfaced! {e.Url}");

				// Get id from data
				var queryString = HttpUtility.ParseQueryString(e.Url.Query);
				string data = queryString["json"];
				var interfaceId = JsonConvert.DeserializeAnonymousType(data, new { Id = 0L }).Id;
				Log($"Retrieved id for interface object: {interfaceId}");

				// Call interface webservice
				var url = SharedCode.Web.HttpExtensions.Build($"{ApiBaseUrl}/json/UOB/Interface", new NameValueCollection()
				{
					{ "id", interfaceId.ToString() },
				}).ToString();

				Log($"Calling service url: {url}");

				var interfaceObjectJson = SharedCode.WebService.WebServiceHelper.GetJson(url, _currentToken.AccessToken);
				Log($"Retrieved interface object: {Newtonsoft.Json.Linq.JToken.Parse(interfaceObjectJson).ToString(Formatting.Indented)}");

				_lastRetrievedObject = JsonConvert.DeserializeObject<SharedCode.Models.Interface>(interfaceObjectJson);
				btnStartWithLastObject.Enabled = true;

				// Restart situation
				if (!string.IsNullOrEmpty(_currentToken.RefreshToken))
				{
					// Refresh token. Normally this is not needed for every call, only when the token is expired.
					// Only possible when offline_access scope is honored
					RefreshToken();
				}

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
			// System.Diagnostics.Debug.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {tekst} {Environment.NewLine}");

			logBox.SelectionStart = logBox.Text.Length;
			logBox.ScrollToCaret();
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

			_currentToken = SharedCode.Authentication.TokenService.RetrieveToken(AuthorizeTokenUrl, query);

			Log($"Refreshing tokens. New token retrieved: {_currentToken.TokenIssued.ToString("yyyyMMdd_HHmmss")}");
		}

		private void btnClearLog_Click(object sender, EventArgs e)
		{
			logBox.Text = string.Empty;
		}

		private async void btnStartWithLastObject_Click(object sender, EventArgs e)
		{
			if (_lastRetrievedObject == null)
			{
				btnStartWithLastObject.Enabled = false;
				return;
			}

			var interfaceObjectJson = JsonConvert.SerializeObject(_lastRetrievedObject);

			// Post the interfaceObject back
			var postUrl = SharedCode.Web.HttpExtensions.Build($"{ApiBaseUrl}/json/UOB/Interface").ToString();
			Log($"Posting object back to: {postUrl}");
			var output = await SharedCode.WebService.WebServiceHelper.PostJson(postUrl, _currentToken.AccessToken, interfaceObjectJson);

			Log($"Retrieved interface id: {output}");

			long lastInterfaceId = long.Parse(output);
			StartUnifeed(lastInterfaceId);
		}

		private void tbnStartClear_Click(object sender, EventArgs e)
		{
			StartUnifeed();
		}

		private async void btnDownload_Click(object sender, EventArgs e)
		{
			var accessToken = _currentToken.AccessToken;

			var downloadUrl = SharedCode.Web.HttpExtensions.Build(UnifeedBaseUrl, new NameValueCollection()
			{
				{ "accessToken", accessToken },
				{ "interface", 32.ToString() },
				{ "interfaceType", "DOWNLOAD" },
				{ "interfaceName", "lokaal apparaat (download)" },
				{ "uobApplication", "Revit" },
				{ "uobApplicationVersion", "2018" },
			}).ToString();

			Log($"Link to initiate download: {downloadUrl}");

			SharedCode.Web.SystemBrowser.OpenBrowser(downloadUrl);
		}
	}
}
